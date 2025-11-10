using UnityEngine;

// Yêu cầu 2 component này phải có, vì Enemy là "bộ não" điều khiển chúng
[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour, IPooledObject
{
    [Header("Components (Read Only)")]
    // Dùng { get; private set; } để đóng gói (encapsulation)
    public Health health { get; private set; }

    private MoveEnemy movement;

    [Header("Data (Injected by Spawner)")] private EnemySO enemySo;
    private string poolTag;

    private void Awake()
    {
        // Lấy các component mà nó điều khiển
        health = GetComponent<Health>();
        movement = GetComponent<MoveEnemy>();
    }

    /// <summary>
    /// Đăng ký (subscribe) các sự kiện khi object được bật (lấy từ pool)
    /// </summary>
    private void OnEnable()
    {
        if (health != null)
        {
            health.OnDeath += Die; // Lắng nghe sự kiện Chết
        }

        if (movement != null)
        {
            movement.OnReachTarget += HandleReachTarget; // Lắng nghe sự kiện Đến Đích
        }
    }

    /// <summary>
    /// Hủy đăng ký (unsubscribe) khi object bị tắt (trả về pool)
    /// </summary>
    private void OnDisable()
    {
        if (health != null)
        {
            health.OnDeath -= Die;
        }

        if (movement != null)
        {
            movement.OnReachTarget -= HandleReachTarget;
        }
    }

    /// <summary>
    /// HÀM MỚI: Được gọi bởi EnemySpawner ngay sau khi spawn
    /// </summary>
    public void Setup(EnemySO data, string tag)
    {
        this.enemySo = data;
        this.poolTag = tag;

        // Sau khi có data, gọi OnObjectSpawn để reset
        OnObjectSpawn();
    }

    /// <summary>
    /// (Từ IPooledObject) Reset trạng thái
    /// </summary>
    public void OnObjectSpawn()
    {
        // Reset máu
        if (enemySo != null && health != null)
        {
            health.MaxHealth = enemySo.MaxHealth;
            health.SetHealth(enemySo.MaxHealth); // SetHealth (an toàn hơn Reset)
        }

        // Reset vị trí/di chuyển
        if (movement != null)
        {
            movement.ResetMovement();

            // Cập nhật tốc độ từ SO (rất quan trọng!)
            if (enemySo != null)
            {
                movement.SetMoveSpeed(enemySo.Speed);
            }
        }
    }

    /// <summary>
    /// Được gọi bởi event 'movement.OnReachTarget' (thay thế OnCollisionEnter2D)
    /// </summary>
    private void HandleReachTarget(Transform targetTransform)
    {
        // 1. Gây sát thương cho mục tiêu
        if (targetTransform.TryGetComponent<Health>(out Health targetHealth))
        {
            targetHealth.TakeDamage(enemySo.Damage);
        }

        // 2. Sau khi tấn công, tự hủy
        Die();
    }

    /// <summary>
    /// Được gọi bởi event 'health.OnDeath'
    /// </summary>
    public void Die()
    {
        // Failsafe
        if (string.IsNullOrEmpty(poolTag) || ObjectPooler.Instance == null)
        {
            Debug.LogWarning($"{gameObject.name} cannot return to pool. Destroying.");
            Destroy(gameObject);
            return;
        }

        // Trả về pool
        ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
    }
}