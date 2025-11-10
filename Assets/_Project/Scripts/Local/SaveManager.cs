using UnityEngine;

public static class SaveManager
{
    // --- Keys (Tên file lưu) ---
    private const string WAVE_KEY = "CurrentWave";
    private const string DAMAGE_UPGRADE_KEY = "DamageLevel";
    private const string SPEED_UPGRADE_KEY = "SpeedLevel";

    // === LƯU NÂNG CẤP RIÊNG (Model-Specific) ===
    
    // Key sẽ là: "Model_FireTower_DamageLevel"
    private static string GetModelStatKey(string modelID, string statName)
    {
        return $"Model_{modelID}_{statName}_Level";
    }

    public static void SaveModelStatLevel(string modelID, string statName, int level)
    {
        PlayerPrefs.SetInt(GetModelStatKey(modelID, statName), level);
        PlayerPrefs.Save();
    }

    public static int LoadModelStatLevel(string modelID, string statName, int start = 0)
    {
        return PlayerPrefs.GetInt(GetModelStatKey(modelID, statName), start); // Bắt đầu từ 0
    }

    // === LƯU NÂNG CẤP CHUNG (Global) ===

    // Key sẽ là: "Global_BaseDamage_Level"
    private static string GetGlobalStatKey(string statName)
    {
        return $"Global_{statName}_Level";
    }
    
    public static void SaveGlobalStatLevel(string statName, int level)
    {
        PlayerPrefs.SetInt(GetGlobalStatKey(statName), level);
        PlayerPrefs.Save();
    }

    public static int LoadGlobalStatLevel(string statName, int start = 0)
    {
        return PlayerPrefs.GetInt(GetGlobalStatKey(statName), start);
    }
    // === WAVE ===
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

    // === DAMAGE UPGRADE ===
    public static void SaveDamageLevel(int level)
    {
        PlayerPrefs.SetInt(DAMAGE_UPGRADE_KEY, level);
        PlayerPrefs.Save();
    }

    public static int LoadDamageLevel()
    {
        return PlayerPrefs.GetInt(DAMAGE_UPGRADE_KEY, 0); // Trả về 0
    }

    // (Làm tương tự cho Speed, Range, v.v...)
    private const string MONEY_KEY = "PlayerMoney";
    
    // ... (Các hàm Save/Load khác của bạn) ...

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
    // (Tùy chọn) Nút Reset cứng
    public static void HardReset()
    {
        PlayerPrefs.DeleteKey(WAVE_KEY);
        PlayerPrefs.DeleteKey(DAMAGE_UPGRADE_KEY);
        PlayerPrefs.DeleteKey(SPEED_UPGRADE_KEY);
        PlayerPrefs.Save();
    }
    private const string ACTIVE_MODEL_KEY = "LastActiveModel";

    public static void SaveActiveModel(int modelIndex)
    {
        PlayerPrefs.SetInt(ACTIVE_MODEL_KEY, modelIndex);
        PlayerPrefs.Save();
    }

    public static int LoadActiveModel()
    {
        return PlayerPrefs.GetInt(ACTIVE_MODEL_KEY, 0); // Trả về 0 (model đầu tiên)
    }
}