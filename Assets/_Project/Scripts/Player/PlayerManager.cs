using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Lớp C# thuần túy (POCO) để giữ trạng thái
// Lớp này giờ chỉ làm "Controller", điều phối các hệ thống khác
public class PlayerManager : MonoBehaviour, IResourceProvider
{
    public static PlayerManager Instance;

    [Header("Dependencies")]
    [SerializeField] private List<TowerTypeDataSO> allTowerModels; // Database
    [SerializeField] private TowerRuntimeSO currentStats; // State
    private float currentMana;
    // Runtime state
    private TowerTypeDataSO currentModelTypeData;
    private int activeModelIndex = 0;
    
    // Public properties cho Tower
    public TowerRuntimeSO CurrentStats => currentStats;
    public TowerTypeDataSO CurrentModel => currentModelTypeData;
    public int CurrentMoney => playerData?.Money ?? 0;
    private PlayerData playerData;
    // EVENT: Báo cho Tower (View)
    public UnityEvent<Sprite, ProjectileDataSO, TowerRuntimeSO> OnTowerPropertiesChanged;
    public UnityEvent<TowerTypeDataSO, TowerRuntimeSO> OnTowerChanged;
    public UnityEvent<int> OnMoneyChanged; // Event mới cho UI tiền
    public UnityEvent<float, float> OnManaChanged;
    private void Awake()
    {
        Instance = this;
        LoadPlayerData();
    }

    private void Start()
    {
        // 1. Tải model đã lưu (GỌI SAVEMANAGER)
        activeModelIndex = SaveManager.LoadActiveModel();
        SwitchToModel(activeModelIndex);
        OnMoneyChanged?.Invoke(playerData.Money);
        currentMana = currentStats.MaxMana;
        OnManaChanged?.Invoke(currentMana, currentStats.MaxMana);
        OnMoneyChanged?.Invoke(playerData.Money);
    }
    private void Update()
    {
        // Chỉ chạy khi game đang "Playing"
        if (GameManager.Instance.currentState != GameState.Playing) return;
        
        if (currentStats == null || currentMana >= currentStats.MaxMana)
        {
            return; // Không làm gì nếu mana đầy
        }

        // Hồi mana
        currentMana += currentStats.ManaRegenRate * Time.deltaTime;
        currentMana = Mathf.Min(currentMana, currentStats.MaxMana);
        
        OnManaChanged?.Invoke(currentMana, currentStats.MaxMana);
    }
    private void LoadPlayerData()
    {
        playerData = new PlayerData();
        playerData.ActiveModelIndex = SaveManager.LoadActiveModel();
        playerData.Money = SaveManager.LoadMoney();
        
        // (Tải các level nâng cấp...)
    }
    /// <summary>
    /// HÀM CHÍNH: Được gọi bởi 4 nút UI
    /// </summary>
    public void SwitchToModel(int modelIndex)
    {
        if (modelIndex < 0 || modelIndex >= allTowerModels.Count) return;
        
        activeModelIndex = modelIndex;
        currentModelTypeData = allTowerModels[activeModelIndex];
        
        // 1. TÍNH TOÁN: Ủy quyền cho StatsCalculator (GỌI SERVICE)
        StatsCalculator.CalculateAndApplyStats(currentModelTypeData, currentStats);
        
        // 2. LƯU: Ủy quyền cho SaveManager (GỌI SERVICE)
        SaveManager.SaveActiveModel(activeModelIndex);
        RegisterProjectilePool();
        // 3. PHÁT SỰ KIỆN: Báo cho View (Tower)
        OnTowerChanged?.Invoke(currentModelTypeData, currentStats);
        OnTowerPropertiesChanged?.Invoke(
            currentModelTypeData.TowerSprite,   // Thuộc tính 1
            currentModelTypeData.ProjectileData, // Thuộc tính 2
            currentStats                         // Thuộc tính 3
        );
        currentMana = currentStats.MaxMana;
        OnManaChanged?.Invoke(currentMana, currentStats.MaxMana);
    }
    private void RegisterProjectilePool()
    {
        if (currentModelTypeData != null && currentModelTypeData.ProjectileData != null)
        {
            ObjectPooler.Instance.RegisterPool(
                currentModelTypeData.ProjectileData.PoolTag,
                currentModelTypeData.ProjectileData.ProjectilePrefab,
                currentModelTypeData.ProjectileData.PoolSize
            );
        }
    }
    // XÓA HÀM CalculateFinalStats() (đã chuyển đi)
    // XÓA HÀM RegisterProjectilePool() (sẽ chuyển đi)
    public bool HasResource(string resourceType, int amount)
    {
        if (resourceType == "Mana")
        {
            return currentMana >= amount; // Kiểm tra Mana (runtime)
        }
        
        if (resourceType == "Money") 
        {
            return playerData.Money >= amount; // Kiểm tra Tiền (persistent)
        }
        
        return false;
        
    }

    public void SpendResource(string resourceType, int amount)
    {
        if (resourceType == "Mana")
        {
            if (currentMana >= amount)
            {
                currentMana -= amount;
                OnManaChanged?.Invoke(currentMana, currentStats.MaxMana);
            }
        }
        else if (resourceType == "Money")
        {
            if (playerData.Money >= amount)
            {
                playerData.Money -= amount;
                SaveManager.SaveMoney(playerData.Money);
                OnMoneyChanged?.Invoke(playerData.Money);
            }
        }
        
    }
}