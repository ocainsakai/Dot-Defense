using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayHUD_View : MonoBehaviour
{
    [Header("Dependencies")]
    [Tooltip("Kéo WaveManager vào đây")]
    [SerializeField] private WaveManager waveManager;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI waveTxt;
    [SerializeField] private TextMeshProUGUI moneyTxt;
    // [SerializeField] private Slider baseHealthSlider;

    [Header("Button")]
    
    [SerializeField] private Button homeButton;
    [SerializeField] private Button splitButton;
    [SerializeField] private Button flameButton;
    [SerializeField] private Button wetButton;
    [SerializeField] private Button iceButton;
    // Tự đăng ký khi được bật (enabled)
    private void OnEnable()
    {
        if (waveManager != null)
        {
            waveManager.OnWaveStart += UpdateWaveText;
        }
        
        // Cập nhật text ban đầu
        if (GameManager.Instance != null)
        {
            UpdateWaveText(GameManager.Instance.CurrentWave);
        }
    }

    // Tự hủy đăng ký khi bị tắt (disabled)
    private void OnDisable()
    {
        if (waveManager != null)
        {
            waveManager.OnWaveStart -= UpdateWaveText;
        }
    }

    /// <summary>
    /// Hàm này chỉ làm 1 việc: cập nhật text
    /// </summary>
    public void UpdateWaveText(int waveNumber)
    {
        if (waveTxt != null)
        {
            waveTxt.text = $"Wave {waveNumber}";
        }
    }
}