using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(menuName = "Scriptable Objects/Tower Runtime", fileName = "TowerRuntimeSO")]
public class TowerRuntimeSO : ScriptableObject
{
   
    [Header("Current Stats")]
    // Dùng [field: SerializeField] để hiện private field trong Inspector
    // nhưng vẫn dùng public property (chỉ get) ở ngoài.
    [field: SerializeField] public float Attack { get; private set; }
    [field: SerializeField] public float AttackSpeed { get; private set; }
    [field: SerializeField] public float Range { get; private set; }
    [field: SerializeField] public float DamageMultiplier { get; private set; } = 1f;
    [field: SerializeField] public float MaxMana { get; private set; }
    [field: SerializeField] public float ManaRegenRate { get; private set; }
    // Event để báo cho UI biết khi stats thay đổi
    public UnityAction OnStatsChanged;

    /// <summary>
    /// Hàm chính để Tower/PlayerManager cập nhật
    /// </summary>
    public void UpdateStats(float newAttack, float newSpeed, float newRange, float newMultiplier
                            ,float newMaxMana, float newManaRegen)
    {
        Attack = newAttack;
        AttackSpeed = newSpeed;
        Range = newRange;
        DamageMultiplier = (newMultiplier == 0) ? 1f : newMultiplier;
        MaxMana = newMaxMana;
        ManaRegenRate = newManaRegen;
        
        OnStatsChanged?.Invoke();
    }

    /// <summary>
    /// Được gọi khi game bắt đầu để đảm bảo data sạch
    /// </summary>
    public void Reset()
    {
        Attack = 0;
        AttackSpeed = 0;
        Range = 0;
        DamageMultiplier = 1f;
        MaxMana = 0;
        ManaRegenRate = 0;
    }
}