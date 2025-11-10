using UnityEngine;

/// <summary>
/// Class tĩnh (static) quản lý toàn bộ việc
/// lưu và tải dữ liệu game bằng PlayerPrefs.
/// Đã được sắp xếp lại cho dễ đọc.
/// </summary>
public static class SaveManager
{
    #region --- KEY CONSTANTS ---
    
    // (Tập trung tất cả các key lưu trữ cố định ở một nơi)

    // Player Resources
    private const string MONEY_KEY = "PlayerMoney";

    // Player Progress
    private const string ACTIVE_MODEL_KEY = "LastActiveModel";
    private const string WAVE_KEY = "CurrentWave";

    // --- Các key cho hệ thống nâng cấp cũ ---
    // (Đã được thay thế bằng hệ thống Global/Model ở dưới)
    // private const string DAMAGE_UPGRADE_KEY = "DamageLevel";
    // private const string SPEED_UPGRADE_KEY = "SpeedLevel";

    #endregion

    //---------------------------------------------------------------------

    #region === Player Money ===

    public static void SaveMoney(int amount)
    {
        PlayerPrefs.SetInt(MONEY_KEY, amount);
        PlayerPrefs.Save();
    }

    public static int LoadMoney()
    {
        // Ví dụ: Bắt đầu game với 100 tiền
        return PlayerPrefs.GetInt(MONEY_KEY, 100); 
    }

    #endregion

    //---------------------------------------------------------------------

    #region === Player Progress (Wave & Model) ===

    public static void SaveActiveModel(int modelIndex)
    {
        PlayerPrefs.SetInt(ACTIVE_MODEL_KEY, modelIndex);
        PlayerPrefs.Save();
    }

    public static int LoadActiveModel()
    {
        // Trả về 0 (model đầu tiên)
        return PlayerPrefs.GetInt(ACTIVE_MODEL_KEY, 0); 
    }

    public static void SaveCurrentWave(int wave)
    {
        PlayerPrefs.SetInt(WAVE_KEY, wave);
        PlayerPrefs.Save();
    }

    public static int LoadCurrentWave()
    {
        // Trả về wave 1 nếu chưa từng lưu
        return PlayerPrefs.GetInt(WAVE_KEY, 1); 
    }

    #endregion

    //---------------------------------------------------------------------

    #region === Upgrade Stats (Global) ===

    public static void SaveGlobalStatLevel(string statName, int level)
    {
        PlayerPrefs.SetInt(GetGlobalStatKey(statName), level);
        PlayerPrefs.Save();
    }

    public static int LoadGlobalStatLevel(string statName, int start = 0)
    {
        return PlayerPrefs.GetInt(GetGlobalStatKey(statName), start);
    }

    #endregion

    //---------------------------------------------------------------------

    #region === Upgrade Stats (Model-Specific) ===

    public static void SaveModelStatLevel(string modelID, string statName, int level)
    {
        PlayerPrefs.SetInt(GetModelStatKey(modelID, statName), level);
        PlayerPrefs.Save();
    }

    public static int LoadModelStatLevel(string modelID, string statName, int start = 0)
    {
        return PlayerPrefs.GetInt(GetModelStatKey(modelID, statName), start); // Bắt đầu từ 0
    }

    #endregion

    //---------------------------------------------------------------------


    #region --- Private Key Helpers ---
    
    // (Các hàm private giúp tạo key động nên ở cuối cùng)

    // Key sẽ là: "Global_BaseDamage_Level"
    private static string GetGlobalStatKey(string statName)
    {
        return $"Global_{statName}_Level";
    }

    // Key sẽ là: "Model_FireTower_DamageLevel"
    private static string GetModelStatKey(string modelID, string statName)
    {
        return $"Model_{modelID}_{statName}_Level";
    }

    #endregion

    //---------------------------------------------------------------------

    #region === Utility (Reset) ===

    /// <summary>
    /// Xóa SẠCH SẼ tất cả dữ liệu đã lưu.
    /// (An toàn hơn là xóa từng key thủ công)
    /// </summary>
    public static void HardReset()
    {
        Debug.LogWarning("HARD RESET: Đã xóa tất cả PlayerPrefs!");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    #endregion
}