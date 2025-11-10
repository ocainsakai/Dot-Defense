using UnityEngine;

[CreateAssetMenu(fileName = "EnemyTypeSO", menuName = "Scriptable Objects/EnemyTypeSO")]
public class EnemyTypeSO : ScriptableObject
{
    [Header("Identification")]
    public string enemyName = "Basic Enemy";
    public string poolTag = "Enemy";

    [Header("Prefab & Pooling")]
    public GameObject prefab;
    public int poolSize = 10;

    [Header("Spawn Settings")]
    public int minWaveToAppear = 1;
    [Range(0f, 5f)]
    public float spawnWeight = 1.0f;

    [Header("Enemy Stats (Reference to EnemySO)")]
    public EnemySO enemyStats; // Giữ các thông số như MaxHealth, Damage, Speed, v.v.
}
