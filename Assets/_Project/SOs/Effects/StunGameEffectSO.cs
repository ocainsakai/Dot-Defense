using UnityEngine;

[CreateAssetMenu(fileName = "StunGameEffectSO", menuName = "Scriptable Objects/StunGameEffectSO")]
public class StunGameEffectSO: GameEffectSO
{
    [Header("Stun Settings")]
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private bool canRefresh = true; // Có thể refresh duration
    
    public override void ApplyEffect(GameObject target, float damageMultiplier = 1f)
    {
        TargetMover movement = target.GetComponent<TargetMover>();
        if (movement == null)
        {
            Debug.LogWarning($"No EnemyMovement component found on {target.name}");
            return;
        }
        
        // Apply stun
        movement.ApplyStun(duration);
        
        // Effects
        SpawnVFX(target.transform.position);
        PlaySound(target.transform.position);
    }
}
