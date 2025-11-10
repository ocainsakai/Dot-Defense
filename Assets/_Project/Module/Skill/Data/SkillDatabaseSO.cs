using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "Skill Module/Skill Database")]
public class SkillDatabaseSO : ScriptableObject
{
    public List<SkillSO> allSkills;

    public SkillSO GetSkillByID(string id)
    {
        return allSkills.FirstOrDefault(skill => skill.SkillID == id);
    }
}
