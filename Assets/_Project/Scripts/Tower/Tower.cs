using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lớp này là một "Vũ khí" (Prefab) hoàn chỉnh và độc lập.
/// Nó tự sở hữu dữ liệu (Model) và tự quản lý chỉ số (Stats).
/// Nó không còn lắng nghe bất kỳ Manager nào.
/// </summary>
public class Tower : MonoBehaviour, ICasterProvider
{
    // === THAY ĐỔI: TOWER TỰ QUẢN LÝ DATA ===

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform firePoint; 
    
    [Header("Dependencies")]
    [SerializeField] private TowerStatsManager statsManager;
    [SerializeField] private Shooter shooter; 
    [SerializeField] private TargetFinder targetFinder; 

    // === THAY ĐỔI: Các trường private này giờ sẽ được set trong Awake() ===
    private TowerRuntimeSO currentStats;
    private ProjectileDataSO currentProjectileData;    
    public ProjectileDataSO CurrentProjectileData => currentProjectileData;
    private float lastAttackTime = 0f;
    private Transform currentTarget;
    
    // (State của Skill... giữ nguyên)
    private bool multiShotEnabled = false;
    private int multiShotCount = 1;
    private bool spreadShotEnabled = false;
    private int spreadShotCount = 1;
    private float spreadShotAngle = 30f;
    private Coroutine multiShotCoroutine;
    private Coroutine spreadShotCoroutine;

    #region Unity Lifecycle

    private void Awake()
    {
        // === THAY ĐỔI: Tự khởi tạo ===
        InitializeTower();
        
        if (targetFinder == null)
        {
            targetFinder = GetComponent<TargetFinder>();
            if (targetFinder == null)
            {
                Debug.LogError("Tower cần một component TargetFinder!", this);
            }
        }
    }

    void Start()
    {
        // (Logic cũ giữ nguyên)
        if (firePoint == null)
        {
            firePoint = transform;
        }
    }

    // === THAY ĐỔI: Thêm hàm khởi tạo ===
    /// <summary>
    /// Tự đọc data, tính stats, set sprite và đăng ký pool.
    /// </summary>
    private void InitializeTower()
    {
        if (statsManager == null)
        {
            statsManager = GetComponent<TowerStatsManager>();
            if (statsManager == null)
            {
                Debug.LogError("PlayerTower cần một component TowerStatsManager!", this);
                return;
            }
        }

        // 1. LẤY STATS
        this.currentStats = statsManager.Stats;

        // 2. LẤY PROJECTILE
        this.currentProjectileData = statsManager.ProjectileData;

        // 3. SET SPRITE
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = statsManager.TowerSprite;
        }
        // 4. ĐĂNG KÝ POOL: Tự đăng ký (lấy logic từ PlayerManager cũ)
    }
    
    /// <summary>
    /// Tự đăng ký pool đạn của mình với ObjectPooler.
    /// </summary>
    private void RegisterProjectilePool()
    {
        if (currentProjectileData != null)
        {
            ObjectPooler.Instance.RegisterPool(
                currentProjectileData.PoolTag,
                currentProjectileData.ProjectilePrefab,
                currentProjectileData.PoolSize
            );
        }
    }

    // (Update và FixedUpdate giữ nguyên, chúng đã dùng currentStats/currentTarget)
    void Update()
    {
        if (currentTarget == null || currentStats == null) return; 

        if (CanAttack())
        {
            Attack();
        }
    }

    void FixedUpdate()
    {
        if (currentStats == null) 
        {
            currentTarget = null;
            return;
        }
        currentTarget = targetFinder.FindBestTarget(currentStats.Range);
    }
    
    // === REGION NÀY ĐÃ BỊ XÓA ===
    // #region Event Handling
    // (Toàn bộ logic TrySubscribe, HandleTowerPropertiesChanged, OnDestroy đã bị xóa
    // vì chúng ta không còn lắng nghe PlayerManager)
    // #endregion
    
    #endregion

    #region Attack Logic
    
    private bool CanAttack()
    {
        if (currentStats == null) return false;
    
        float cooldown = 1f / currentStats.AttackSpeed;
        return Time.time - lastAttackTime >= cooldown;
    }
    
    // (Hàm Attack giữ nguyên, nó đã được sửa ở lần trước)
    private void Attack()
    {
        if (currentProjectileData == null) return;
        
        lastAttackTime = Time.time;

        if (spreadShotEnabled)
        {
            shooter.ShootSpreadAngular(
                currentProjectileData,
                currentTarget,
                currentStats.Attack,
                firePoint,
                spreadShotCount, 
                spreadShotAngle  
            );
        }
        else if (multiShotEnabled) 
        {
            shooter.ShootMultiParallel(
                currentProjectileData,
                currentTarget,
                currentStats.Attack,
                firePoint,
                multiShotCount,
                0.3f 
            );
        }
        else
        {
            shooter.ShootSingle(
                currentProjectileData,
                currentTarget,
                currentStats.Attack,
                firePoint
            );
        }
    }
    #endregion
    
    // (Các Region khác: Public API, Skill Coroutines, Gizmos, ICasterProvider... giữ nguyên)
    
    #region Public API for Skills
    // (Không thay đổi)
    public void EnableMultiShotTimed(int count, float duration)
    {
        if (multiShotCoroutine != null) StopCoroutine(multiShotCoroutine);
        multiShotCoroutine = StartCoroutine(MultiShotCoroutine(count, duration));
    }
    public void EnableSpreadShotTimed(int count, float angle, float duration)
    {
        if (spreadShotCoroutine != null) StopCoroutine(spreadShotCoroutine);
        spreadShotCoroutine = StartCoroutine(SpreadShotCoroutine(count, angle, duration));
    }
    public void SetTargetPriority(TargetPriority priority)
    {
        if (targetFinder != null) targetFinder.SetTargetPriority(priority);
    }

    public void ChangeProjectile(ProjectileDataSO newProjectileData)
    {
        currentProjectileData = newProjectileData;
    }
    #endregion

    #region Skill Coroutines
    // (Không thay đổi)
    private IEnumerator MultiShotCoroutine(int count, float duration)
    {
        multiShotEnabled = true;
        multiShotCount = count;
        yield return new WaitForSeconds(duration);
        multiShotEnabled = false;
        multiShotCount = 1;
        multiShotCoroutine = null;
    }
    private IEnumerator SpreadShotCoroutine(int count, float angle, float duration)
    {
        spreadShotEnabled = true;
        spreadShotCount = count;
        spreadShotAngle = angle;
        yield return new WaitForSeconds(duration);
        spreadShotEnabled = false;
        spreadShotCount = 1;
        spreadShotCoroutine = null;
    }
    #endregion

    #region Gizmos
    // (Không thay đổi)
    private void OnDrawGizmosSelected()
    {
        if (currentStats == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, currentStats.Range);
    }
    #endregion

    #region ICasterProvider
    // (Không thay đổi)
    public GameObject GetCaster()
    {
        return gameObject;
    }
    public GameObject GetCurrentTarget()
    {
        return currentTarget?.gameObject;    
    }
    #endregion

  
}