using UnityEngine;

[CreateAssetMenu(fileName = "StunGameEffectSo", menuName = "Scriptable Objects/StunGameEffectSo")]
public class StunGameEffectSo: GameEffectSO
{
    [Header("Stun Settings")]
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private bool canRefresh = true; // Có thể refresh duration
    
    public override void ApplyEffect(GameObject target, float damageMultiplier = 1f)
    {
        MoveEnemy movement = target.GetComponent<MoveEnemy>();
        if (movement == null)
        {
            Debug.LogWarning($"No EnemyMovement component found on {target.name}");
            return;
        }
        
        // Apply stun
        movement.ApplyStun(duration, canRefresh);
        
        // Effects
        SpawnVFX(target.transform.position);
        PlaySound(target.transform.position);
    }
}
