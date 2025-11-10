using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// MỘT NHIỆM VỤ: Khởi tạo và Quản lý Data của Tower.
/// Lấy dữ liệu thô (Model Data) và tính toán ra chỉ số runtime (Stats).
/// Cung cấp data đã xử lý cho các component khác (như PlayerTower).
/// </summary>
public class TowerStatsManager : MonoBehaviour
{
    [Header("Data Definition (Input)")]
    [Tooltip("ĐỊNH NGHĨA: Gán TowerTypeDataSO (ví dụ: Fire Tower Lvl 1) vào đây.")]
    [SerializeField] private TowerTypeDataSO towerModelData;
    
    [Header("Data Instance (Output)")]
    [Tooltip("TRẠNG THÁI RUNTIME: Gán một TowerRuntimeSO (instance) vào đây.")]
    [SerializeField] private TowerRuntimeSO currentStats;
    public TowerRuntimeSO Stats => currentStats;
    public ProjectileDataSO ProjectileData { get; private set; }
    public Sprite TowerSprite { get; private set; }
    
    // --- KHỞI TẠO ---
    public static Action<TowerRuntimeSO> OnTowerStatsInitialized;

    public UnityEvent<TowerRuntimeSO> OnStatChange;
    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        OnStatChange?.Invoke(Stats);
    }

    /// <summary>
    /// Tự đọc data, tính stats, set sprite và đăng ký pool.
    /// </summary>
    private void Initialize()
    {
        if (towerModelData == null || currentStats == null)
        {
            Debug.LogError($"[TowerStatsManager] Cấu hình bị thiếu! Hãy gán 'Tower Model Data' và 'Current Stats' cho {gameObject.name}", this);
            return;
        }

        // 1. TÍNH TOÁN STATS: Tự gọi StatsCalculator
        StatsCalculator.CalculateAndApplyStats(towerModelData, currentStats);

        // 2. SET PROJECTILE: "Publish" data đạn
        this.ProjectileData = towerModelData.ProjectileData;

        // 3. SET SPRITE: "Publish" data hình ảnh
        this.TowerSprite = towerModelData.TowerSprite;

        // 4. ĐĂNG KÝ POOL: Tự đăng ký
        RegisterProjectilePool();
        OnTowerStatsInitialized?.Invoke(currentStats);
    }
    
    private void RegisterProjectilePool()
    {
        if (ProjectileData != null)
        {
            ObjectPooler.Instance.RegisterPool(
                ProjectileData.PoolTag,
                ProjectileData.ProjectilePrefab,
                ProjectileData.PoolSize
            );
        }
    }
}