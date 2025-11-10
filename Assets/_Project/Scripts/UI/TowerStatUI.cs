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
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private TextMeshProUGUI maxManaText;
    [SerializeField] private TextMeshProUGUI manaRegenText;

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
            attackText.text = currentTowerStats.Attack.ToString("F1");

        if (speedText != null)
            speedText.text = currentTowerStats.AttackSpeed.ToString("F1");

        if (rangeText != null)
            rangeText.text = currentTowerStats.Range.ToString("F1");

        if (maxManaText != null)
            maxManaText.text = currentTowerStats.MaxMana.ToString("F0");

        if (manaRegenText != null)
            manaRegenText.text = currentTowerStats.ManaRegenRate.ToString("F1");
    }

    /// <summary>
    /// Dọn dẹp UI khi không có Tower nào được chọn
    /// </summary>
    private void ClearUI()
    {
        if (attackText != null) attackText.text = "-";
        if (speedText != null) speedText.text = "-";
        if (rangeText != null) rangeText.text = "-";
        if (maxManaText != null) maxManaText.text = "-";
        if (manaRegenText != null) manaRegenText.text = "-";
    }
}