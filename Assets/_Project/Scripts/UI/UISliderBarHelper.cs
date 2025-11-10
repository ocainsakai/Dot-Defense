using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Quản lý một cặp Slider và TextMeshPro để hiển thị giá trị (ví dụ: HP, Mana).
/// Cung cấp hàm cập nhật giá trị và hiệu ứng "Flash" cho thanh fill.
/// </summary>
public class UISliderBarHelper : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI valueText;

    private Image fillImage;
    private Color originalColor;
    private Coroutine activeFlashCoroutine;

    private void Awake()
    {
        // 1. Tự động lấy component nếu chưa gán
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }

        if (valueText == null)
        {
            valueText = GetComponentInChildren<TextMeshProUGUI>();
        }

        // 2. Khởi tạo Slider
        if (slider != null)
        {
            slider.value = slider.maxValue = 1;

            // 3. Lấy Fill Image và màu gốc
            if (slider.fillRect != null)
            {
                fillImage = slider.fillRect.GetComponent<Image>();
                if (fillImage != null)
                {
                    originalColor = fillImage.color;
                }
                else
                {
                    Debug.LogWarning("Slider 'fillRect' không chứa component Image.", this);
                }
            }
            else
            {
                Debug.LogWarning("Slider 'fillRect' chưa được gán. Không thể dùng VFX Flash.", this);
            }
        }
        else
        {
            Debug.LogError("UISliderBarHelper không tìm thấy Slider component!", this);
        }

        // 4. Khởi tạo Text
        if (valueText != null)
        {
            valueText.text = "--/--";
        }
    }

    /// <summary>
    /// Cập nhật giá trị cho slider và text (dùng số float).
    /// </summary>
    /// <param name="current">Giá trị hiện tại</param>
    /// <param name="max">Giá trị tối đa</param>
    /// <param name="valueFormat">Format string cho text (ví dụ: "F0" = không số lẻ, "F1" = 1 số lẻ)</param>
    public void UpdateValue(float current, float max, string valueFormat = "F0")
    {
        if (slider != null)
        {
            slider.maxValue = max;
            slider.value = current;
        }

        if (valueText != null)
        {
            // Dùng ToString(format) để kiểm soát số lẻ
            valueText.text = $"{current.ToString(valueFormat)}/{max.ToString(valueFormat)}";
        }
    }

    /// <summary>
    /// Cập nhật giá trị cho slider và text (dùng số int).
    /// </summary>
    public void UpdateValue(int current, int max)
    {
        // Tự động gọi hàm float với format "F0" (không số lẻ)
        UpdateValue((float)current, (float)max, "F0");
    }
    public void UpdateValue(float current, float max)
    {
        // Tự động gọi hàm float với format "F0" (không số lẻ)
        UpdateValue((float)current, (float)max, "F0");
    }
    /// <summary>
    /// Kích hoạt hiệu ứng nhấp nháy trên thanh fill.
    /// </summary>
    /// <param name="flashColor">Màu muốn nháy tới (mặc định là Trắng)</param>
    /// <param name="duration">Tổng thời gian hiệu ứng</param>
    public void FlashVFX(Color? flashColor = null, float duration = 0.2f)
    {
        if (fillImage == null)
        {
            // Đã cảnh báo trong Awake, không cần spam log
            return;
        }

        // 1. Dừng Coroutine cũ (nếu đang chạy) để tránh xung đột
        if (activeFlashCoroutine != null)
        {
            StopCoroutine(activeFlashCoroutine);
        }
        
        // 2. Đặt màu flash (dùng màu trắng nếu không chỉ định)
        Color targetColor = flashColor ?? Color.white;
        
        // 3. Bắt đầu Coroutine mới
        activeFlashCoroutine = StartCoroutine(FlashCoroutine(targetColor, duration));
    }

    /// <summary>
    /// Coroutine thực thi logic nhấp nháy: Gốc -> Flash -> Gốc
    /// </summary>
    private IEnumerator FlashCoroutine(Color targetColor, float duration)
    {
        float halfDuration = duration / 2f;
        
        // --- Giai đoạn 1: Từ màu gốc -> màu Flash ---
        float timer = 0f;
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            // Lerp: nội suy tuyến tính giữa 2 màu
            fillImage.color = Color.Lerp(originalColor, targetColor, timer / halfDuration);
            yield return null; // Chờ frame tiếp theo
        }
        fillImage.color = targetColor; // Đảm bảo đạt đúng màu flash

        // --- Giai đoạn 2: Từ màu Flash -> về màu gốc ---
        timer = 0f;
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            fillImage.color = Color.Lerp(targetColor, originalColor, timer / halfDuration);
            yield return null;
        }

        // --- Hoàn tất ---
        fillImage.color = originalColor; // Đảm bảo trở về màu gốc
        activeFlashCoroutine = null; // Báo hiệu Coroutine đã xong
    }
}