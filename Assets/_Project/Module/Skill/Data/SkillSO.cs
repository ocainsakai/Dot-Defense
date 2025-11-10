using UnityEngine;

public abstract class SkillSO : ScriptableObject
{
    public string SkillID;
    public string DisplayName;
    public Sprite Icon;
    [TextArea] public string Description;
}
