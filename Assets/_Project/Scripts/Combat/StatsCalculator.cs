public static class StatsCalculator
{
    // Hàm này nhận data, tính toán, và cập nhật kết quả vào SO
    public static void CalculateAndApplyStats(
        TowerTypeDataSO modelData, 
        TowerRuntimeSO runtimeStats)
    {
        if (modelData == null || runtimeStats == null) return;

        // 1. Get Global levels
        int globalDamageLevel = SaveManager.LoadGlobalStatLevel("BaseDamage");
        int globalSpeedLevel = SaveManager.LoadGlobalStatLevel("BaseSpeed", 1);
        int globalRangeLevel = SaveManager.LoadGlobalStatLevel("BaseRange");
        int globalManaLvl = SaveManager.LoadGlobalStatLevel("MaxMana");
        int globalRegenLvl = SaveManager.LoadGlobalStatLevel("ManaRegen");
        // 2. Get Model levels
        string modelID = modelData.ModelID;
        int modelDamageLevel = SaveManager.LoadModelStatLevel(modelID, "Damage");
        int modelSpeedLevel = SaveManager.LoadModelStatLevel(modelID, "Speed", 1);
        int modelRangeLevel = SaveManager.LoadModelStatLevel(modelID, "Range");

        // 3. Calculate
        float finalDamage = modelData.BaseDamage + (modelDamageLevel * modelData.DamagePerLevel) + (globalDamageLevel * 5f);
        float finalSpeed = modelData.BaseAttackSpeed + (modelSpeedLevel * modelData.AttackSpeedPerLevel) + (globalSpeedLevel * 0.1f);
        float finalRange = modelData.BaseAttackRange + (modelRangeLevel * modelData.RangePerLevel) + (globalRangeLevel * 10f);
        float finalMaxMana = 100 + (globalManaLvl * 10);
        float finalManaRegen = 1 + (globalRegenLvl * 0.2f);
        // 4. Cập nhật vào SO
        runtimeStats.UpdateStats(
            finalDamage, finalSpeed, finalRange, 1,
            finalMaxMana, finalManaRegen 
        );
    }
}