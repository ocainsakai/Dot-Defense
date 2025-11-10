using UnityEngine;

[CreateAssetMenu(fileName = "Effect_ApplySpreadShot", menuName = "Skill Module/Effects/Enable SpreadShot")]
public class Effect_ApplySpreadShot : EffectSO
{
    public int projectileCount = 5;
    public float spreadAngle = 45f;
    public float duration = 3f;

    public override void Execute(GameObject caster, GameObject target)
    {
        Tower actor = caster.GetComponent<Tower>();
        
        if (actor != null)
        {
            actor.EnableSpreadShotTimed(projectileCount, spreadAngle, duration);
        }
        else
        {
            Debug.LogWarning($"SkillModule: {caster.name} không implement IShooterActions!");
        }
    }
}