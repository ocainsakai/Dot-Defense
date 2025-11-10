using UnityEngine;

[CreateAssetMenu(fileName = "Effect_ApplyMultiShot", 
    menuName = "Skill Module/Effects/Apply MultiShot")]
public class Effect_ApplyMultiShot : EffectSO
{
    public int projectileCount = 3;
    public float duration = 5f;

    public override void Execute(GameObject caster, GameObject target)
    {
        // 1. Module không tìm "Tower".
        // 2. Nó tìm BẤT CỨ AI có khả năng "IShooterActions".
        Tower actor = caster.GetComponent<Tower>();
        
        if (actor != null)
        {
            // 3. Ra lệnh qua Interface
            actor.EnableMultiShotTimed(projectileCount, duration);
        }
        else
        {
            Debug.LogWarning($"SkillModule: {caster.name} không implement IShooterActions!");
        }
    }
}