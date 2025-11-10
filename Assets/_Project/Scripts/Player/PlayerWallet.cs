using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Singleton: Quản lý "Ví tiền" (Money) của người chơi.
/// Chịu trách nhiệm Load, Save, và cung cấp API giao dịch.
/// Implement IResourceProvider CHỈ cho "Money".
/// </summary>
public class PlayerWallet : MonoBehaviour, IResourceProvider
{

    [Header("Runtime State")]
    [Tooltip("Số tiền hiện tại của người chơi")]
    [SerializeField] private int currentMoney;

    // Event cho UI (Text hiển thị tiền)
    public UnityEvent<int> OnMoneyChanged;

    #region Unity Lifecycle

    private void Awake()
    {
        LoadMoney();
    }

    private void Start()
    {
        // Cập nhật UI ngay khi bắt đầu
        OnMoneyChanged?.Invoke(currentMoney);
    }

    #endregion

    #region Save & Load Logic

    private void LoadMoney()
    {
        // Giả định bạn có một class SaveManager tĩnh (static)
        currentMoney = SaveManager.LoadMoney(); 
    }

    private void SaveMoney()
    {
        SaveManager.SaveMoney(currentMoney);
    }

    #endregion

    #region Public API (Cho Nâng Cấp, Shop,...)

    /// <summary>
    /// Thêm tiền vào ví của người chơi (ví dụ: nhặt được, thưởng).
    /// </summary>
    public void AddMoney(int amount)
    {
        if (amount <= 0) return;

        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney);
        SaveMoney(); // Tự động lưu khi có thay đổi
    }

    #endregion

    #region IResourceProvider Implementation

    /// <summary>
    /// Kiểm tra xem người chơi có đủ tiền ("Money") không.
    /// </summary>
    public bool HasResource(string resourceType, int amount)
    {
        // Service này CHỈ xử lý "Money"
        if (resourceType != "Money")
        {
            return false;
        }
        
        return currentMoney >= amount;
    }

    /// <summary>
    /// Tiêu tiền của người chơi (dùng cho Nâng cấp, Mua vật phẩm).
    /// </summary>
    public void SpendResource(string resourceType, int amount)
    {
        // Chỉ tiêu "Money" và phải có đủ
        if (resourceType == "Money" && HasResource(resourceType, amount))
        {
            currentMoney -= amount;
            OnMoneyChanged?.Invoke(currentMoney);
            SaveMoney(); // Tự động lưu khi tiêu
        }
    }

    #endregion
}