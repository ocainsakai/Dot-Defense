using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour, IPooledObject
{
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;
    
    // Data reference (injected khi spawn)
    private ProjectileDataSO projectileData;
    private float damageMultiplier = 1f;
    private string poolTag;
    
    // State
    private float spawnTime;
    private bool isActive = false;
    private bool hasHit = false;
    
    // Components
    private TargetMover mover;
    private Collider2D col;
    private TrailRenderer trailRenderer;
    
    #region Unity Lifecycle
    
    private void Awake()
    {
        mover = GetComponent<ProjectileMover2D>();
        col = GetComponent<Collider2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        
        // Setup Collider as Trigger
        if (col != null)
        {
            col.isTrigger = true;
        }
        
        // Subscribe to TargetMover events
        if (mover != null)
        {
            mover.OnReachTarget += OnReachedTarget;
        }
    }
    
    private void Update()
    {
        if (!isActive || hasHit) return;
        
        // Check lifetime
        if (projectileData != null && Time.time - spawnTime > projectileData.Lifetime)
        {
            if (showDebugLogs)
                Debug.Log($"⏰ Projectile expired: {poolTag}");
            ReturnToPool();
            return;
        }
        
        // Rotate to face direction (optional visual)
        RotateTowardsVelocity();
    }
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isActive || hasHit) return;
            
            // Check if hit valid target
            if (projectileData != null && (projectileData.TargetLayer.value & (1 << other.gameObject.layer)) > 0)
            {
                // Nếu ĐÚNG (va chạm với layer hợp lệ)
                OnHitTarget(other.gameObject);
            }
        }
    
    private void OnDestroy()
    {
        // Unsubscribe events
        if (mover != null)
        {
            mover.OnReachTarget -= OnReachedTarget;
        }
    }
    
    #endregion
    
    #region IPooledObject Implementation
    
    /// <summary>
    /// Được gọi bởi ObjectPooler khi spawn
    /// </summary>
    public void OnObjectSpawn()
    {
        // Reset state
        isActive = true;
        hasHit = false;
        spawnTime = Time.time;
        
        // Clear trail
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"🚀 Projectile spawned from pool: {poolTag}");
        }
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Initialize projectile (gọi ngay sau OnObjectSpawn)
    /// </summary>
    public void Initialize(
        ProjectileDataSO data, 
        float damageMultiplier, 
        Transform target,
        string poolTag)
    {
        this.projectileData = data;
        this.damageMultiplier = damageMultiplier;
        this.poolTag = poolTag;
        isActive = true;
        // Configure TargetMover
        if (mover != null)
        {
            // Set target
            mover.SetTarget(target, resetMovement: true);
            
            // Configure movement mode
            // trackTarget = true → Homing (bám đuổi)
            // trackTarget = false → Ballistic (bay thẳng)
            // Sử dụng reflection hoặc expose property trong TargetMover
            SetMovementMode(data.IsHoming);
            
            // Set speed từ ProjectileData
            SetMoveSpeed(data.Speed);
        }
        
        // Play launch sound
        if (data.LaunchSound != null)
        {
            AudioSource.PlayClipAtPoint(data.LaunchSound, transform.position, 0.5f);
        }
    }
    
    #endregion
    
    #region Movement Configuration
    
    /// <summary>
    /// Set movement mode: Homing hoặc Ballistic
    /// </summary>
    private void SetMovementMode(bool isHoming)
    {
        // Sử dụng Reflection để set trackTarget trong TargetMover
        // (Hoặc bạn có thể expose SetTrackTarget() method trong TargetMover)
        var field = typeof(TargetMover).GetField("trackTarget", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            field.SetValue(mover, isHoming);
        }
        
        // Set stopAtDistance = false (projectile không dừng, chỉ va chạm)
        var stopField = typeof(TargetMover).GetField("stopAtDistance", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
        
        if (stopField != null)
        {
            stopField.SetValue(mover, false);
        }
    }
    
    /// <summary>
    /// Set move speed từ ProjectileData
    /// </summary>
    private void SetMoveSpeed(float speed)
    {
        var field = typeof(TargetMover).GetField("moveSpeed", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            field.SetValue(mover, speed);
        }
        
        // Reset speed để apply
        mover?.ResetSpeed();
    }
    
    #endregion
    
    #region Visual
    
    /// <summary>
    /// Xoay projectile theo hướng bay (visual only)
    /// </summary>
    private void RotateTowardsVelocity()
    {
        if (mover == null || !mover.IsMoving) return;
        
        // Get velocity từ Rigidbody2D
        Rigidbody2D rb = mover.GetComponent<Rigidbody2D>();
        if (rb != null && rb.linearVelocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    
    #endregion
    
    #region Hit Logic
    
    /// <summary>
    /// Được gọi khi projectile chạm target (OnTriggerEnter2D)
    /// </summary>
    private void OnHitTarget(GameObject target)
    {
        if (projectileData == null || hasHit) return;
        
        hasHit = true;
        isActive = false;
        
        if (showDebugLogs)
        {
            Debug.Log($"💥 Projectile hit: {target.name}");
        }
        
        // Stop movement
        if (mover != null)
        {
            mover.SetTarget(null);
        }
        
        // Apply all effects
        projectileData.ApplyAllEffects(target, damageMultiplier);
        
        // Spawn hit VFX
        SpawnHitVFX();
        
        // Play hit sound
        if (projectileData.HitSound != null)
        {
            AudioSource.PlayClipAtPoint(projectileData.HitSound, transform.position, 0.5f);
        }
        
        // Return to pool
        ReturnToPool();
    }
    
    /// <summary>
    /// Được gọi khi projectile đến target nhưng không va chạm
    /// (Ballistic mode hoặc target chết trước khi chạm)
    /// </summary>
    private void OnReachedTarget(Transform target)
    {
        if (!isActive || hasHit) return;
        
        if (showDebugLogs)
        {
            Debug.Log($"📍 Projectile reached target position (no collision): {poolTag}");
        }
        
        // Return to pool (missed target)
        ReturnToPool();
    }
    
    private void SpawnHitVFX()
    {
        if (projectileData.HitVFX != null)
        {
            GameObject vfx = Instantiate(
                projectileData.HitVFX, 
                transform.position, 
                Quaternion.identity
            );
            Destroy(vfx, 2f);
        }
    }
    
    #endregion
    
    #region Pooling
    
    private void ReturnToPool()
    {
        isActive = false;
        hasHit = false;
        
        // Stop movement
        if (mover != null)
        {
            mover.SetTarget(null);
        }
        
        // Return to ObjectPooler
        ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
    }
    
    #endregion
}
