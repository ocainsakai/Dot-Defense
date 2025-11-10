using UnityEngine;

[CreateAssetMenu(fileName = "TowerTypeDataSO", menuName = "SO/Tower Type Data")]
public class TowerTypeDataSO : ScriptableObject
{
    // ----------------------------------------------------------------
    // SECTION 1: DỮ LIỆU (FIELDS) - PRIVATE
    // ----------------------------------------------------------------
    
    [Header("Basic Info")]
    [SerializeField] private string modelID = "unique_tower_id";
    [SerializeField] private string towerName = "Tower";
    [SerializeField] [TextArea(2, 4)] private string description = "Tower description";
    [SerializeField] private Sprite uiIcon; // Đổi tên từ 'icon' cho rõ
    [SerializeField] private Sprite towerSprite; // Sprite cho tháp trong game

    [Header("Prefab")]
    [SerializeField] private GameObject towerPrefab; // Prefab nếu bạn muốn spawn (không cần nếu chỉ đổi sprite)

    [Header("Base Stats (Level 1)")]
    [SerializeField] private float baseDamage = 30f;
    [SerializeField] private float baseAttackSpeed = 1f; // Tấn công mỗi giây
    [SerializeField] private float baseAttackRange = 10f;
    
    [Header("Projectile")]
    [SerializeField] private ProjectileDataSO projectileData;
    
    [Header("Economy (Base)")]
    [SerializeField] private int buildCost = 100;
    [SerializeField] private int baseUpgradeCost = 50;
    [SerializeField] private float upgradeCostMultiplier = 1.5f; // Mỗi level tăng 1.5x
    [SerializeField] private float sellValueMultiplier = 0.7f; // Bán lại 70% giá trị đã đầu tư

    [Header("Upgrade Scaling (Per Level)")]
    // Dùng float (1.1f) cho phép bạn dùng phép nhân (Pow)
    // Dùng int (5) cho phép bạn dùng phép cộng
    [SerializeField] private float damagePerLevel = 5f; // Mỗi level +5 damage (ví dụ)
    [SerializeField] private float attackSpeedPerLevel = 0.1f; // Mỗi level +0.1 speed
    [SerializeField] private float rangePerLevel = 1f; // Mỗi level +1 range
    [SerializeField] private int maxLevel = 10;
    
    [Header("Targeting Priority")]
    [SerializeField] private TargetPriority defaultPriority = TargetPriority.Nearest;
    
    [Header("Visual & Audio")]
    [SerializeField] private GameObject muzzleFlashVFX;
    [SerializeField] private AudioClip attackSound;

    // ----------------------------------------------------------------
    // SECTION 2: TRUY CẬP (PROPERTIES) - PUBLIC GETTERS
    // ----------------------------------------------------------------

    // Các script khác chỉ có thể ĐỌC data này, không thể SỬA
    public string ModelID => modelID;
    public string TowerName => towerName;
    public string Description => description;
    public Sprite UiIcon => uiIcon;
    public Sprite TowerSprite => towerSprite;
    public GameObject TowerPrefab => towerPrefab;
    public float BaseDamage => baseDamage;
    public float BaseAttackSpeed => baseAttackSpeed;
    public float BaseAttackRange => baseAttackRange;
    public ProjectileDataSO ProjectileData => projectileData;
    public int BuildCost => buildCost;
    public int BaseUpgradeCost => baseUpgradeCost;
    public float UpgradeCostMultiplier => upgradeCostMultiplier;
    public float SellValueMultiplier => sellValueMultiplier;
    public float DamagePerLevel => damagePerLevel;
    public float AttackSpeedPerLevel => attackSpeedPerLevel;
    public float RangePerLevel => rangePerLevel;
    public int MaxLevel => maxLevel;
    public TargetPriority DefaultPriority => defaultPriority;
    public GameObject MuzzleFlashVFX => muzzleFlashVFX;
    public AudioClip AttackSound => attackSound;
    private void OnValidate()
    {
        baseDamage = Mathf.Max(1f, baseDamage);
        baseAttackSpeed = Mathf.Max(0.1f, baseAttackSpeed);
        baseAttackRange = Mathf.Max(1f, baseAttackRange);
        buildCost = Mathf.Max(0, buildCost);
        baseUpgradeCost = Mathf.Max(0, baseUpgradeCost);
        maxLevel = Mathf.Clamp(maxLevel, 1, 99);
        
        if (string.IsNullOrEmpty(modelID))
        {
            modelID = name; // Tự động gán ID nếu trống
        }
    }
}