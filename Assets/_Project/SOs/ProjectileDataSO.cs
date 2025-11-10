using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData_SO", menuName = "Scriptable Objects/ProjectileData_SO")]
public class ProjectileDataSO : ScriptableObject
{
    [Header("Basic Info")]
    [SerializeField] private string projectileName = "Projectile";
    [SerializeField] [TextArea(2, 3)] private string description = "Projectile description";
    
    [Header("Prefab")]
    [SerializeField] private GameObject projectilePrefab;
    
    [Header("Movement")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 5f; // Tự hủy sau X giây
    [SerializeField] private bool isHoming = true; // Có tự bám mục tiêu không
    [SerializeField] private float rotationSpeed = 200f; // Tốc độ xoay khi homing
    
    [Header("Targeting")]
    [SerializeField] private string targetTag = "Enemy";
    [SerializeField] private LayerMask targetLayer;
    
    [Header("Effects on Hit")]
    [SerializeField] private List<GameEffectSO> onHitEffects = new List<GameEffectSO>();
    
    [Header("Visual & Audio")]
    [SerializeField] private GameObject hitVFX;
    [SerializeField] private AudioClip launchSound;
    [SerializeField] private AudioClip hitSound;
    
    [Header("Pooling")]
    [SerializeField] private string poolTag = "Projectile";
    [SerializeField] private int poolSize = 20;
    
    // Getters
    public string ProjectileName => projectileName;
    public GameObject ProjectilePrefab => projectilePrefab;
    public float Speed => speed;
    public float Lifetime => lifetime;
    public bool IsHoming => isHoming;
    public float RotationSpeed => rotationSpeed;
    public string TargetTag => targetTag;
    public LayerMask TargetLayer => targetLayer;
    public List<GameEffectSO> OnHitEffects => onHitEffects;
    public GameObject HitVFX => hitVFX;
    public AudioClip LaunchSound => launchSound;
    public AudioClip HitSound => hitSound;
    public string PoolTag => poolTag;
    public int PoolSize => poolSize;
    
    /// <summary>
    /// Apply tất cả effects lên target
    /// </summary>
    public void ApplyAllEffects(GameObject target, float damageMultiplier = 1f)
    {
        if (onHitEffects == null || onHitEffects.Count == 0)
        {
            Debug.LogWarning($"No effects defined for {projectileName}");
            return;
        }
        
        foreach (GameEffectSO effect in onHitEffects)
        {
            if (effect != null)
            {
                effect.ApplyEffect(target, damageMultiplier);
            }
        }
    }
}
