using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill_Active", menuName = "Skill Module/Active Skill")]
public class ActiveSkillSO : SkillSO
{
    public float Cooldown;
    public int Cost = 10;
    public string ResourceType = "Mana"; // "Game" sẽ định nghĩa "Mana" là gì

    // Skill này sẽ làm gì? Nó sẽ thực thi 1 danh sách các "Hiệu ứng"
    public List<EffectSO> EffectsToApply;
}


// Lớp trừu tượng cho mọi "hành động" (Gây damage, Hồi máu, Triệu hồi)