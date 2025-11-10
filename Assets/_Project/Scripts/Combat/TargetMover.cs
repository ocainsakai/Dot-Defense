using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class TargetMover : MonoBehaviour
{
    #region Serialized Fields
    
    [Header("Core Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 5f;

    [Header("Behaviour")]
    [Tooltip("Nếu 'true' (Homing), sẽ liên tục bám theo target. Nếu 'false' (Ballistic), sẽ chỉ bay theo hướng target ban đầu.")]
    [SerializeField] private bool trackTarget = true; // <-- BIẾN MỚI
    
    [Tooltip("Nếu 'true', sẽ dừng lại ở stoppingDistance. Nếu 'false' (cho Projectile), sẽ di chuyển cho đến khi va chạm.")]
    [SerializeField] private bool stopAtDistance = true; 
    [SerializeField] private float stoppingDistance = 0.5f;

    [Header("Targeting")]
    [SerializeField] private bool autoFindTarget = true;
    [SerializeField] private string targetTag;
    
    #endregion
    
    #region Properties
    
    public Transform Target { get => target; private set => target = value; }
    public float BaseMoveSpeed { get; private set; }
    public float CurrentMoveSpeed { get; private set; }
    public bool IsMoving { get; private set; }
    public bool HasReachedTarget { get; private set; }
    
    // Các lớp con (2D/3D) sẽ implement các hàm trừu tượng này
    protected abstract float GetDistanceToTarget();
    protected abstract Vector3 GetDirectionToTarget();
    protected abstract void ApplyVelocity(Vector3 direction, float speed);
    protected abstract void StopVelocity();

    #endregion

    #region State
    private Coroutine slowCoroutine;
    private Coroutine stunCoroutine;
    private bool isStunned = false;

    private Vector3 initialDirection; // <-- BIẾN MỚI
    private bool hasInitialDirection = false; // <-- BIẾN MỚI
    #endregion

    #region Events
    public UnityAction OnStartMoving;
    public UnityAction OnStopMoving;
    public UnityAction<Transform> OnReachTarget;
    #endregion

    #region Unity Lifecycle
    
    protected virtual void Awake()
    {
        BaseMoveSpeed = moveSpeed;
        CurrentMoveSpeed = moveSpeed;
    }

    protected virtual void Start()
    {
        if (target == null && autoFindTarget)
        {
            FindTargetByTag(targetTag);
        }
    }

    protected virtual void FixedUpdate()
    {
        // 1. Ưu tiên Stun
        if (isStunned)
        {
            StopVelocity();
            return;
        }

        // === LOGIC MỚI ===
        if (trackTarget)
        {
            // === CHẾ ĐỘ 1: BÁM ĐUỔI (Homing) ===
            HandleHomingMovement();
        }
        else
        {
            // === CHẾ ĐỘ 2: BAY THẲNG (Ballistic) ===
            HandleBallisticMovement();
        }
    }
    
    #endregion

    #region Movement Logic

    /// <summary>
    /// Logic bám đuổi (cho Enemy)
    /// </summary>
    private void HandleHomingMovement()
    {
        // 2. Kiểm tra Target
        if (target == null)
        {
            if (IsMoving) StopMoving();
            return;
        }

        // 3. Kiểm tra logic dừng
        if (stopAtDistance)
        {
            float distance = GetDistanceToTarget();
            if (distance <= stoppingDistance)
            {
                if (!HasReachedTarget) ReachTarget();
                if (IsMoving) StopMoving();
                return;
            }
        }
        
        // Bắt đầu di chuyển
        if (!IsMoving) StartMoving();
        
        // 4. Di chuyển (luôn cập nhật hướng)
        ApplyVelocity(GetDirectionToTarget(), CurrentMoveSpeed);
    }

    /// <summary>
    /// Logic bay thẳng (cho Projectile)
    /// </summary>
    private void HandleBallisticMovement()
    {
        // Bắt đầu di chuyển (nếu chưa)
        if (!IsMoving) StartMoving();

        // 1. Nếu chưa có hướng, tính toán
        if (!hasInitialDirection)
        {
            if (target != null)
            {
                initialDirection = GetDirectionToTarget();
                hasInitialDirection = true;
            }
            else
            {
                // Không có target, không có hướng, đứng yên
                initialDirection = Vector3.zero; 
                StopMoving();
                return;
            }
        }
        
        // 2. Di chuyển (luôn dùng hướng ban đầu)
        ApplyVelocity(initialDirection, CurrentMoveSpeed);
    }

    #endregion

    #region Movement State
    
    private void StartMoving()
    {
        IsMoving = true;
        HasReachedTarget = false;
        OnStartMoving?.Invoke();
    }

    private void StopMoving()
    {
        IsMoving = false;
        StopVelocity();
        OnStopMoving?.Invoke();
    }

    private void ReachTarget()
    {
        HasReachedTarget = true;
        OnReachTarget?.Invoke(target);
    }
    
    #endregion

    #region Public API
    
    public void SetTarget(Transform newTarget, bool resetMovement = true)
    {
        target = newTarget;
        hasInitialDirection = false; // <-- QUAN TRỌNG: Reset để tính lại hướng
        
        if (resetMovement)
        {
            HasReachedTarget = false;
            if (!isStunned) StartMoving();
        }
    }

    public void FindTargetByTag(string tag)
    {
        GameObject targetObj = GameObject.FindGameObjectWithTag(tag);
        if (targetObj != null)
        {
            SetTarget(targetObj.transform);
        }
    }

    public void ResetSpeed()
    {
        CurrentMoveSpeed = BaseMoveSpeed;
    }
    
    #endregion

    #region Effects (Slow / Stun)

    public void ApplySlow(float multiplier, float duration)
    {
        if (slowCoroutine != null) StopCoroutine(slowCoroutine);
        slowCoroutine = StartCoroutine(SlowCoroutine(multiplier, duration));
    }

    private IEnumerator SlowCoroutine(float multiplier, float duration)
    {
        CurrentMoveSpeed = BaseMoveSpeed * multiplier;
        yield return new WaitForSeconds(duration);
        CurrentMoveSpeed = BaseMoveSpeed;
        slowCoroutine = null;
    }

    public void ApplyStun(float duration)
    {
        if (stunCoroutine != null) StopCoroutine(stunCoroutine);
        stunCoroutine = StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
        stunCoroutine = null;
    }
    
    #endregion
}