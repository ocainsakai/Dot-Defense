using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect_ApplyChangeProjectile", 
    menuName = "Skill Module/Effects/Apply IceShot")]
public class Effect_ApplyChangeProjectile : EffectSO
{
    public ProjectileDataSO projData;

    public override void Execute(GameObject caster, GameObject target)
    {
        Tower actor = caster.GetComponent<Tower>();
        ProjectileDataSO origin;
        if (actor != null)
        {
            origin = actor.CurrentProjectileData;
            actor.ChangeProjectile(projData);
        }
        else
        {
            Debug.LogWarning($"SkillModule: {caster.name} không implement IShooterActions!");
        }
        
    }
    
}