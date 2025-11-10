using UnityEngine;

[CreateAssetMenu(fileName = "SlowGameEffectSo", menuName = "Scriptable Objects/SlowGameEffectSo")]
public class SlowGameEffectSo: GameEffectSO
{
    [Header("Slow Settings")]
    [SerializeField] [Range(0f, 1f)] private float slowPercentage = 0.5f; // 50% slow
    [SerializeField] private float duration = 2f;
    [SerializeField] private bool isStackable = false;
    
    public override void ApplyEffect(GameObject target, float damageMultiplier = 1f)
    {
        // Tìm EnemyMovement component (hoặc interface ISlowable)
        MoveEnemy movement = target.GetComponent<MoveEnemy>();
        if (movement == null)
        {
            Debug.LogWarning($"No EnemyMovement component found on {target.name}");
            return;
        }
        
        // Apply slow
        movement.ApplySlowEffect(slowPercentage, duration);
        
        // Effects
        SpawnVFX(target.transform.position);
        PlaySound(target.transform.position);
    }
    
    public float GetSlowPercentage() => slowPercentage;
    public float GetDuration() => duration;
}
