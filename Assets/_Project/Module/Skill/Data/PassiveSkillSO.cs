using UnityEngine;

[CreateAssetMenu(fileName = "Skill_Passive", menuName = "Skill Module/Passive Skill")]
public class PassiveSkillSO : SkillSO
{
    // Bạn có thể mở rộng thành List<StatModifier>
    public float DamageMultiplier = 0.1f; // +10% Dmg
    public float SpeedMultiplier = 0f;
}