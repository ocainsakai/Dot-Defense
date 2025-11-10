using System;
using System.Collections; // <-- Thêm thư viện này để dùng Coroutine
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradeButton : MonoBehaviour
{
    [SerializeField] Button upgradeButton;
    [SerializeField] TextMeshProUGUI upgradeButtonText;
    
    [Header("Config")]
    public string StatName;
    public int UpgradeCost;

    // === THÊM MỚI: Cấu hình cho hiệu ứng ===
    [Header("Effects")]
    [SerializeField] private Color notEnoughMoneyColor = Color.red;
    [SerializeField] private float flashDuration = 0.5f;

    private Color originalButtonTextColor;
    private Coroutine flashCoroutine;
    // === KẾT THÚC THÊM MỚI ===

    private void Awake()
    {
        upgradeButton.onClick.AddListener(() => Upgrade(StatName, UpgradeCost));

        // === THÊM MỚI: Lưu lại màu gốc của text ===
        if (upgradeButtonText != null)
        {
            originalButtonTextColor = upgradeButtonText.color;
        }
    }

    private void Start()
    {
        upgradeButtonText.text = $"{UpgradeCost} Coin";
        
    }

    private void Upgrade(string statName, int cost)
    {
        // (Giả sử bạn có Singleton tên là PlayerWallet)
        if (PlayerWallet.instance.HasResource("Money", cost))
        {
            PlayerWallet.instance.SpendResource("Money", cost);
            int currentLevel = SaveManager.LoadGlobalStatLevel(statName);
            currentLevel++;
            SaveManager.SaveGlobalStatLevel(statName, currentLevel);
            TowerStatsManager.instance.OnUpdateStat();
        }
        else
        {
            // === THÊM MỚI: Kích hoạt hiệu ứng "Không đủ tiền" ===
            TriggerNotEnoughMoneyEffect();
        }
    }
    
    // === THÊM MỚI: Các hàm xử lý hiệu ứng ===

    /// <summary>
    /// Kích hoạt hiệu ứng nhấp nháy
    /// </summary>
    private void TriggerNotEnoughMoneyEffect()
    {
        // (Tùy chọn: Phát âm thanh "lỗi" ở đây)
        // AudioManager.Instance.PlaySound("ErrorSound");

        // Dừng Coroutine cũ (nếu đang chạy) để tránh xung đột
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            // Đặt lại màu gốc ngay lập tức
            upgradeButtonText.color = originalButtonTextColor;
        }

        // Bắt đầu Coroutine nhấp nháy mới
        flashCoroutine = StartCoroutine(FlashTextCoroutine());
    }

    /// <summary>
    /// Coroutine thực thi logic nhấp nháy (Đỏ -> Chờ -> Trở về màu gốc)
    /// </summary>
    private IEnumerator FlashTextCoroutine()
    {
        // 1. Chuyển sang màu đỏ
        upgradeButtonText.color = notEnoughMoneyColor;

        // 2. Chờ
        yield return new WaitForSeconds(flashDuration);

        // 3. Trở về màu gốc
        upgradeButtonText.color = originalButtonTextColor;

        // 4. Báo hiệu coroutine đã hoàn thành
        flashCoroutine = null;
    }
}