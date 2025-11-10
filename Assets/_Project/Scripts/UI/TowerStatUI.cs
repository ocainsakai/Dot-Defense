using UnityEngine;
using TMPro;

/// <summary>
/// Class này nằm trên UI Panel, chịu trách nhiệm hiển thị
/// chỉ số của Tower HIỆN TẠI đang được active.
/// Nó tự động lắng nghe sự kiện global từ TowerStatsManager.
/// </summary>
public class TowerStatUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject attackText;
    [SerializeField] private GameObject speedText;
    [SerializeField] private GameObject rangeText;
    [SerializeField] private GameObject maxManaText;
    [SerializeField] private GameObject manaRegenText;

    // Biến để lưu trữ SO hiện tại đang lắng nghe
    private TowerRuntimeSO currentTowerStats;

    private void OnEnable()
    {
        // 1. Đăng ký nghe sự kiện "global" khi có tower mới
        TowerStatsManager.OnTowerStatsInitialized += HandleNewTowerStats;
    }

    private void OnDisable()
    {
        // 2. Hủy đăng ký sự kiện "global"
        TowerStatsManager.OnTowerStatsInitialized -= HandleNewTowerStats;

        // 3. Hủy đăng ký sự kiện "local" (của SO cũ)
        UnsubscribeFromCurrentStats();
    }

    /// <summary>
    /// Được gọi bởi sự kiện global khi có một tower mới được active.
    /// </summary>
    public void HandleNewTowerStats(TowerRuntimeSO newStats)
    {
        // 1. Hủy đăng ký listener khỏi SO cũ (nếu có)
        UnsubscribeFromCurrentStats();

        // 2. Gán SO mới
        currentTowerStats = newStats;

        // 3. Đăng ký listener với SO mới
        if (currentTowerStats != null)
        {
            currentTowerStats.OnStatsChanged += UpdateUI;
            
            // 4. Cập nhật UI ngay lập tức
            UpdateUI();
        }
        else
        {
            // Nếu không có stats (ví dụ: tower bị hủy)
            ClearUI();
        }
    }

    /// <summary>
    /// Hủy đăng ký sự kiện OnStatsChanged khỏi SO hiện tại
    /// </summary>
    private void UnsubscribeFromCurrentStats()
    {
        if (currentTowerStats != null)
        {
            currentTowerStats.OnStatsChanged -= UpdateUI;
        }
    }

    /// <summary>
    /// Cập nhật tất cả text trên UI dựa trên 'currentTowerStats'.
    /// (Đây là hàm được gọi bởi event OnStatsChanged)
    /// </summary>
    private void UpdateUI()
    {
        if (currentTowerStats == null)
        {
            ClearUI();
            return;
        }

        // "F1" = 1 chữ số thập phân, "F0" = 0 chữ số thập phân
        if (attackText != null)
            UpdateElement(attackText,"ATTACK", currentTowerStats.Attack);
        if (speedText != null)
            UpdateElement(speedText,"ATTACK SPEED", currentTowerStats.AttackSpeed);

        if (rangeText != null)
            UpdateElement(rangeText,"RANGE", currentTowerStats.Range);

        if (maxManaText != null)
            UpdateElement(maxManaText,"MANA", currentTowerStats.MaxMana);

        if (manaRegenText != null)
            UpdateElement(manaRegenText,"MANA REGEN", currentTowerStats.ManaRegenRate);
    }

    private void UpdateElement(GameObject go, string title, float value)
    {
        var titleComponent = go.transform.Find("Title").GetComponent<TextMeshProUGUI>();
        var valueComponent = go.transform.Find("Value").GetComponent<TextMeshProUGUI>();
        var upgradeComponent = go.GetComponentInChildren<UIUpgradeButton>();
        titleComponent.text = title;
        valueComponent.text = value.ToString("F1");
        upgradeComponent.UpgradeCost = 100;
    }
    private void ClearElement(GameObject go, string title)
    {
        // Kiểm tra an toàn
        if (go == null) return; 

        // Tìm các component con (Dùng '?' để tránh lỗi nếu không tìm thấy)
        var titleComponent = go.transform.Find("Title")?.GetComponent<TextMeshProUGUI>();
        var valueComponent = go.transform.Find("Value")?.GetComponent<TextMeshProUGUI>();

        if (titleComponent != null)
        {
            titleComponent.text = title;
        }
        if (valueComponent != null)
        {
            valueComponent.text = "-"; // Ký tự mặc định
        }
    }
    
    private void ClearUI()
    {
        ClearElement(attackText, "ATTACK");
        ClearElement(speedText, "ATTACK SPEED");
        ClearElement(rangeText, "RANGE");
        ClearElement(maxManaText, "MANA");
        ClearElement(manaRegenText, "MANA REGEN");
    }
}