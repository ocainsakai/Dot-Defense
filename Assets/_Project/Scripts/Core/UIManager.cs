using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gameplayHUD;

    [Header("Core Components")]
    [Tooltip("Kéo WaveManager vào đây để UI tự cập nhật")]
    [SerializeField] private WaveManager waveManager;

    [Header("Text")] 
    [SerializeField] private TextMeshProUGUI waveTxt;
    
    [Header("Buttons")]
    [SerializeField] private Button startButton;


    void Start()
    {
        // Gán sự kiện cho các nút
        
        // 1. Nút Start (Bắt đầu game)
        startButton?.onClick.AddListener(() => {
            GameManager.Instance.StartGame();
        });

        // 4. Tự động đăng ký nghe sự kiện Wave
        if (waveManager != null)
        {
            waveManager.OnWaveStart += UpdateWaveText;
        }

        // Cập nhật text wave ban đầu (nếu cần)
        UpdateWaveText(GameManager.Instance.CurrentWave);
    }

    /// <summary>
    /// Được gọi bởi WaveManager.OnWaveStart
    /// </summary>
    public void UpdateWaveText(int waveNumber)
    {
        if (waveTxt != null)
        {
            waveTxt.text = $"Wave {waveNumber}";
        }
    }
    
    // --- Các hàm quản lý Panel (Giữ nguyên) ---

    public void ShowStartPanel()
    {
        HideAllPanels();
        startPanel.SetActive(true);
    }

    public void ShowGameplayUI()
    {
        HideAllPanels();
        gameplayHUD.SetActive(true);
    }
    
  
    private void HideAllPanels()
    {
        // Thêm kiểm tra null
        startPanel?.SetActive(false);
        gameplayHUD?.SetActive(false);
    }

    /// <summary>
    /// Hủy đăng ký sự kiện khi UI bị hủy
    /// </summary>
    private void OnDestroy()
    {
        if (waveManager != null)
        {
            waveManager.OnWaveStart -= UpdateWaveText;
        }
    }
}