using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Component di chuyển Enemy về phía target (Player Base/Tower)
/// Tuân thủ Single Responsibility - chỉ quản lý movement logic
/// </summary>
public class MoveEnemy : MonoBehaviour
{
    #region Serialized Fields
    private Rigidbody2D rb;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    
    [Header("Target")]
    [SerializeField] private Transform target;
    
    [Header("Detection")]
    [SerializeField] private float reachDistance = 0.5f;
    [SerializeField] private float stoppingDistance = 0.3f;
    
    [Header("Options")]
    [SerializeField] private bool lookAtTarget = true;
    [SerializeField] private bool autoFindTarget = true;
    [SerializeField] private string targetTag = "PlayerBase";
    
    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private bool showDebugLogs = false;
    
    #endregion
    
    #region Properties
    
    public Transform Target
    {
        get => target;
        set => target = value;
    }
    
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = Mathf.Max(0f, value);
    }
    
    public bool IsMoving { get; private set; }
    public bool HasReachedTarget { get; private set; }
    public float DistanceToTarget => target != null ? Vector3.Distance(transform.position, target.position) : float.MaxValue;
    public Vector3 DirectionToTarget => target != null ? (target.position - transform.position).normalized : Vector3.zero;
    
    #endregion
    
    #region Events
    
    /// <summary>
    /// Event khi enemy bắt đầu di chuyển
    /// </summary>
    public UnityAction OnStartMoving;
    
    /// <summary>
    /// Event khi enemy dừng di chuyển
    /// </summary>
    public UnityAction OnStopMoving;
    
    /// <summary>
    /// Event khi enemy đến gần target (trong reachDistance)
    /// </summary>
    public UnityAction<Transform> OnReachTarget;
    
    /// <summary>
    /// Event mỗi frame khi đang di chuyển
    /// Parameter: distance to target
    /// </summary>
    public UnityAction<float> OnMoving;
    
    #endregion
    
    #region Unity Lifecycle
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        // Tự động tìm target nếu chưa set
        if (target == null && autoFindTarget)
        {
            FindTarget();
        }
        
        if (target == null)
        {
            Debug.LogWarning($"{gameObject.name}: No target found! Enemy will not move.");
        }
    }
    
    void FixedUpdate()
    {
        if (target == null || HasReachedTarget)
        {
            if (IsMoving)
            {
                StopMoving();
            }
            return;
        }
        
        float distanceToTarget = DistanceToTarget;
        
        // Check if reached target
        if (distanceToTarget <= reachDistance)
        {
            ReachTarget();
            return;
        }
        
        // Check if should stop (very close but not reached)
        if (distanceToTarget <= stoppingDistance)
        {
            if (IsMoving)
            {
                StopMoving();
            }
            return;
        }
        
        // Start moving if not moving
        if (!IsMoving)
        {
            StartMoving();
        }
        
        // Move towards target
        MoveTowardsTarget();
        
        // Rotate towards target
        if (lookAtTarget)
        {
            RotateTowardsTarget();
        }
        
        // Trigger moving event
        OnMoving?.Invoke(distanceToTarget);
        
        if (showDebugLogs)
        {
            Debug.Log($"{gameObject.name} moving. Distance: {distanceToTarget:F2}");
        }
    }
    
    void OnDrawGizmos()
    {
        if (!showDebugGizmos || target == null)
            return;
        
        // Draw line to target
        Gizmos.color = HasReachedTarget ? Color.green : Color.yellow;
        Gizmos.DrawLine(transform.position, target.position);
        
        // Draw reach distance sphere
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(target.position, reachDistance);
        
        // Draw stopping distance sphere
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(target.position, stoppingDistance);
        
        // Draw direction arrow
        Vector3 direction = DirectionToTarget;
        Vector3 arrowEnd = transform.position + direction * 0.5f;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, arrowEnd);
    }
    
    #endregion
    
    #region Movement Methods
    
    private void MoveTowardsTarget()
    {
        if (target == null)
        {
            rb.linearVelocity = Vector2.zero; // Dừng lại nếu không có target
            return;
        }
    
        // Tính toán hướng (nên dùng Vector2 cho 2D)
        Vector2 direction = (target.position - transform.position).normalized;
    
        // Gán vận tốc. Rigidbody sẽ tự xử lý việc di chuyển mượt mà
        rb.linearVelocity = direction * moveSpeed;
    }
    
    private void RotateTowardsTarget()
    {
        if (target == null) return;

        if (DirectionToTarget.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }
    
    private void StartMoving()
    {
        IsMoving = true;
        OnStartMoving?.Invoke();
        
        if (showDebugLogs)
        {
            Debug.Log($"{gameObject.name} started moving towards {target.name}");
        }
    }
    
    private void StopMoving()
    {
        IsMoving = false;
        OnStopMoving?.Invoke();
        
        if (showDebugLogs)
        {
            Debug.Log($"{gameObject.name} stopped moving");
        }
    }
    
    private void ReachTarget()
    {
        if (HasReachedTarget)
            return;
        
        HasReachedTarget = true;
        IsMoving = false;
        
        OnReachTarget?.Invoke(target);
        
        if (showDebugLogs)
        {
            Debug.Log($"{gameObject.name} reached target: {target.name}");
        }
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Set target mới
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        HasReachedTarget = false;
        
        if (showDebugLogs)
        {
            Debug.Log($"{gameObject.name} target set to: {newTarget?.name ?? "null"}");
        }
    }
    
    /// <summary>
    /// Tìm target tự động theo tag
    /// </summary>
    public void FindTarget()
    {
        GameObject targetObj = GameObject.FindGameObjectWithTag(targetTag);
        
        if (targetObj != null)
        {
            SetTarget(targetObj.transform);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: Could not find target with tag '{targetTag}'");
        }
    }
    
    /// <summary>
    /// Tìm target gần nhất với tag
    /// </summary>
    public void FindNearestTarget(string tag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
        
        if (targets.Length == 0)
        {
            Debug.LogWarning($"{gameObject.name}: No targets found with tag '{tag}'");
            return;
        }
        
        Transform nearest = null;
        float minDistance = float.MaxValue;
        
        foreach (GameObject obj in targets)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = obj.transform;
            }
        }
        
        if (nearest != null)
        {
            SetTarget(nearest);
        }
    }
    
    /// <summary>
    /// Pause di chuyển
    /// </summary>
    public void Pause()
    {
        enabled = false;
        StopMoving();
    }
    
    /// <summary>
    /// Resume di chuyển
    /// </summary>
    public void Resume()
    {
        enabled = true;
        HasReachedTarget = false;
    }
    
    /// <summary>
    /// Reset trạng thái
    /// </summary>
    public void ResetMovement()
    {
        HasReachedTarget = false;
        IsMoving = false;
    }
    
    /// <summary>
    /// Set tốc độ di chuyển (có thể dùng cho slow effect)
    /// </summary>
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = Mathf.Max(0f, newSpeed);
    }
    
    /// <summary>
    /// Tăng tốc độ (multiplier)
    /// </summary>
    public void MultiplySpeed(float multiplier)
    {
        moveSpeed *= Mathf.Max(0f, multiplier);
    }
    
    #endregion
    
    #region Helper Methods
    
    /// <summary>
    /// Kiểm tra có đang tiến gần target không
    /// </summary>
    public bool IsApproachingTarget()
    {
        if (target == null || !IsMoving)
            return false;
        
        return Vector3.Dot(DirectionToTarget, transform.forward) > 0;
    }
    
    /// <summary>
    /// Tính thời gian ước tính để đến target
    /// </summary>
    public float GetEstimatedTimeToTarget()
    {
        if (target == null || moveSpeed <= 0f)
            return float.MaxValue;
        
        return DistanceToTarget / moveSpeed;
    }
    
    #endregion
    
    public void ApplySlowEffect(float slowMultiplier, float slowDuration)
    {
        StartCoroutine(SlowCoroutine(slowMultiplier, slowDuration));
    }
    
    private IEnumerator SlowCoroutine(float slowMultiplier, float slowDuration)
    {
        float originalSpeed = MoveSpeed;
        SetMoveSpeed(originalSpeed * slowMultiplier);
        
        yield return new WaitForSeconds(slowDuration);
        
        SetMoveSpeed(originalSpeed);
    }

    public void ApplyStun(float duration, bool canRefresh)
    {
        StartCoroutine(StunCoroutine(duration, canRefresh));
    }

    private IEnumerator StunCoroutine(float duration, bool canRefresh)
    {
        IsMoving = false;
        yield return new WaitForSeconds(duration);
        // do effect
        IsMoving = true;
    }
}
