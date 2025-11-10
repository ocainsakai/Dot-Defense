using UnityEngine;

[CreateAssetMenu(fileName = "SlowGameEffectSO", menuName = "Scriptable Objects/SlowGameEffectSO")]
public class SlowGameEffectSO: GameEffectSO
{
    [Header("Slow Settings")]
    [SerializeField] [Range(0f, 1f)] private float slowPercentage = 0.5f; // 50% slow
    [SerializeField] private float duration = 2f;
    [SerializeField] private bool isStackable = false;
    
    public override void ApplyEffect(GameObject target, float damageMultiplier = 1f)
    {
        // Tìm EnemyMovement component (hoặc interface ISlowable)
        TargetMover movement = target.GetComponent<TargetMover>();
        if (movement == null)
        {
            Debug.LogWarning($"No EnemyMovement component found on {target.name}");
            return;
        }
        
        
        movement.ApplySlow(slowPercentage, duration);
        
        // Effects
        SpawnVFX(target.transform.position);
        PlaySound(target.transform.position);
    }
    
    public float GetSlowPercentage() => slowPercentage;
    public float GetDuration() => duration;
}
