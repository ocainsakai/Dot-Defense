using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("Danh sách TẤT CẢ các loại enemy mà pooler cần biết")]
    public List<EnemyTypeSO> allEnemyTypes;
    public Transform[] spawnPoints;
    
    // Event "báo cáo" cho WaveManager
    public System.Action OnEnemyDied;

    private void Start()
    {
        // Đăng ký tất cả pools
        foreach (EnemyTypeSO type in allEnemyTypes)
        {
            ObjectPooler.Instance.RegisterPool(type.poolTag, type.prefab, type.poolSize);
        }
    }

    /// <summary>
    /// Hàm "thực thi" được WaveManager gọi
    /// </summary>
    public void SpawnEnemy(EnemyTypeSO type, Transform targetBase)
    {
        // 1. Chọn spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // 2. Lấy từ pool
        GameObject enemyGO = ObjectPooler.Instance.SpawnFromPool(
            type.poolTag,
            spawnPoint.position,
            Quaternion.identity
        );

        if (enemyGO == null) return;

        // 3. Setup Enemy (Quan trọng)
        Enemy enemyScript = enemyGO.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            // 3a. Tiêm (Inject) data
            enemyScript.Setup(type.enemyStats, type.poolTag);
            

            // 3c. Đăng ký theo dõi cái chết
            Health enemyHealth = enemyScript.health;
            if (enemyHealth != null)
            {
                UnityAction deathCallback = null;
                deathCallback = () => {
                    // Khi con quái này chết, báo cáo cho Manager
                    HandleEnemyDeath(enemyHealth, deathCallback);
                };
                enemyHealth.OnDeath += deathCallback;
            }
        }
    }

    /// <summary>
    /// Hàm nội bộ xử lý khi quái chết
    /// </summary>
    private void HandleEnemyDeath(Health enemyHealth, UnityAction callback)
    {
        // Hủy đăng ký ngay
        if (enemyHealth != null)
        {
            enemyHealth.OnDeath -= callback;
        }
        
        // "Báo cáo" lên cho WaveManager
        OnEnemyDied?.Invoke();
    }
}