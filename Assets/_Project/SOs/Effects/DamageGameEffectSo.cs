using UnityEngine;

[CreateAssetMenu(fileName = "DamageGameEffectSo", menuName = "Scriptable Objects/DamageGameEffectSo")]
public class DamageGameEffectSo : GameEffectSO
{
    [Header("Damage Settings")]
    [SerializeField] private int baseDamage = 30;
    [SerializeField] private bool isCritical = false;
    [SerializeField] [Range(0f, 1f)] private float criticalChance = 0.1f;
    [SerializeField] private float criticalMultiplier = 2f;
    
    public override void ApplyEffect(GameObject target, float damageMultiplier = 1f)
    {
        // Tìm Health component
        Health health = target.GetComponent<Health>();
        if (health == null || health.IsDead)
        {
            return;
        }
        
        // Tính damage với multiplier
        int finalDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);
        
        // Critical hit chance
        if (isCritical && Random.value < criticalChance)
        {
            finalDamage = Mathf.RoundToInt(finalDamage * criticalMultiplier);
        }
        
        // Apply damage
        health.TakeDamage(finalDamage);
        
        // Effects
        SpawnVFX(target.transform.position);
        PlaySound(target.transform.position);
        
    }
    
    public int GetBaseDamage() => baseDamage;
}
