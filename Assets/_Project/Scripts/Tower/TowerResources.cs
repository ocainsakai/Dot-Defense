using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Singleton: Quản lý pool Mana TOÀN CỤC của người chơi.
/// Nó implement IResourceProvider CHỈ cho tài nguyên "Mana".
/// Nó được TowerStatsManager "cấu hình" khi một Tower mới được spawn.
/// </summary>
public class TowerResource : MonoBehaviour, IResourceProvider
{

    [Header("Runtime State")]
    [Tooltip("Lượng mana hiện tại của người chơi")]
    [SerializeField] private float currentMana;

    // --- Các trường này là những gì bạn yêu cầu ---
    [Header("Current Tower Stats")]
    [Tooltip("Lượng mana tối đa, được set bởi TowerStatsManager")]
    private float currentMaxMana = 100; // Mặc định
    
    [Tooltip("Tốc độ hồi mana, được set bởi TowerStatsManager")]
    private float currentManaRegenRate = 5; // Mặc định

    // Event cho UI (Thanh Mana)
    public UnityEvent<float, float> OnManaChanged;

    #region Unity Lifecycle
    
    private void Start()
    {
        // Khởi tạo (set đầy mana lúc bắt đầu game)
        currentMana = currentMaxMana;
        OnManaChanged?.Invoke(currentMana, currentMaxMana);
    }

    private void Update()
    {
        // (Bạn nên có logic kiểm tra 'GameIsPaused' ở đây)
        // if (GameManager.Instance.currentState != GameState.Playing) return;
        
        // Hồi mana
        if (currentMana < currentMaxMana)
        {
            currentMana += currentManaRegenRate * Time.deltaTime;
            currentMana = Mathf.Min(currentMana, currentMaxMana);
            OnManaChanged?.Invoke(currentMana, currentMaxMana);
        }
    }

    #endregion

    #region Public API

    /// <summary>
    /// API: Được TowerStatsManager gọi khi một Tower mới được spawn
    /// để cập nhật chỉ số Mana của người chơi.
    /// </summary>
    public void SetManaStats(float maxMana, float manaRegenRate)
    {
        currentMaxMana = maxMana;
        currentManaRegenRate = manaRegenRate;
        
        // Khi đổi tower, mana được set đầy (hoặc bạn có thể giữ nguyên %
        // tùy theo thiết kế game)
        currentMana = currentMaxMana; 
        
        OnManaChanged?.Invoke(currentMana, currentMaxMana);
    }

    #endregion

    #region IResourceProvider Implementation

    /// <summary>
    /// Kiểm tra xem người chơi có đủ tài nguyên không.
    /// Class này CHỈ xử lý "Mana".
    /// </summary>
    public bool HasResource(string resourceType, int amount)
    {
        // Phải đúng là "Mana"
        if (resourceType != "Mana")
        {
            return false;
        }
        
        return currentMana >= amount;
    }

    /// <summary>
    /// Tiêu tài nguyên của người chơi.
    /// Class này CHỈ xử lý "Mana".
    /// </summary>
    public void SpendResource(string resourceType, int amount)
    {
        // Chỉ tiêu "Mana" và phải có đủ
        if (resourceType == "Mana" && HasResource(resourceType, amount))
        {
            currentMana -= amount;
            OnManaChanged?.Invoke(currentMana, currentMaxMana);
        }
    }

    #endregion
}