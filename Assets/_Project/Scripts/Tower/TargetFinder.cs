// File: TargetFinder.cs

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component này chịu trách nhiệm tìm kiếm và
/// lựa chọn mục tiêu dựa trên các tiêu chí.
/// Nó được thiết kế để Tower sử dụng.
/// </summary>
public class TargetFinder : MonoBehaviour
{
    [Header("Targeting Config")]
    [Tooltip("Layer của kẻ thù")]
    [SerializeField] private LayerMask enemyLayer;
    
    [Tooltip("Ưu tiên mục tiêu (Nearest, Strongest, etc.)")]
    [SerializeField] private TargetPriority targetPriority = TargetPriority.Nearest;

    /// <summary>
    /// API công khai chính: Tìm mục tiêu tốt nhất dựa trên cấu hình.
    /// </summary>
    /// <param name="range">Tầm bắn hiện tại (do Tower cung cấp).</param>
    /// <returns>Transform của mục tiêu, hoặc null nếu không tìm thấy.</returns>
    public Transform FindBestTarget(float range)
    {
        // Bước 1: Tìm tất cả mục tiêu hợp lệ trong tầm
        List<Transform> validTargets = FindAllValidTargets(range);

        if (validTargets.Count == 0)
        {
            return null;
        }

        // Bước 2: Chọn mục tiêu tốt nhất từ danh sách
        return SelectTargetByPriority(validTargets);
    }

    /// <summary>
    /// API công khai: Thay đổi ưu tiên mục tiêu (dùng cho Skill hoặc UI).
    /// </summary>
    public void SetTargetPriority(TargetPriority priority)
    {
        targetPriority = priority;
    }

    #region Target Finding Logic
    
    /// <summary>
    /// Quét một vòng tròn và trả về danh sách các mục tiêu hợp lệ (còn sống, có Health).
    /// </summary>
    private List<Transform> FindAllValidTargets(float range)
    {
        // Dùng transform.position của chính component này
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
        List<Transform> validTargets = new List<Transform>();
        
        if (colliders.Length == 0)
        {
            // 1. Tầm bắn (Range) quá nhỏ.
            // 2. 'enemyLayer' (LayerMask) gán sai.
            // 3. Enemy không có Collider2D.
            // Debug.LogWarning("2a. [FindTarget] Không tìm thấy Collider2D nào.");
            return validTargets; // Trả về danh sách rỗng
        }

        foreach (Collider2D col in colliders)
        {
            // Tối ưu: Giả sử Layer "Enemy" chỉ chứa Enemy có Health
            if (col.TryGetComponent<Health>(out var health) && !health.IsDead)
            {
                validTargets.Add(col.transform);
            }
        }
        
        if (validTargets.Count == 0)
        {
            // 1. Enemy thiếu script "Health".
            // 2. Enemy đã IsDead.
            // Debug.LogWarning("2b. [FindTarget] Đã thấy Collider, nhưng không có target HỢP LỆ.");
        }

        return validTargets;
    }
    
    /// <summary>
    /// Chọn mục tiêu từ danh sách dựa trên `targetPriority`.
    /// </summary>
    private Transform SelectTargetByPriority(List<Transform> targets)
    {
        switch (targetPriority)
        {
            case TargetPriority.Nearest:
                return GetNearestTarget(targets);
            
            case TargetPriority.Farthest:
                return GetFarthestTarget(targets);
            
            case TargetPriority.Strongest:
                return GetStrongestTarget(targets);
            
            case TargetPriority.Weakest:
                return GetWeakestTarget(targets);
            
            default:
                return targets[0]; // Mặc định
        }
    }
    
    // --- CÁC HÀM HELPER (Giữ nguyên) ---
    
    private Transform GetNearestTarget(List<Transform> targets)
    {
        Transform nearest = null;
        float minDistance = Mathf.Infinity;
        foreach (Transform target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = target;
            }
        }
        return nearest;
    }

    private Transform GetFarthestTarget(List<Transform> targets)
    {
        Transform farthest = null;
        float maxDistance = 0f;
        foreach (Transform target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthest = target;
            }
        }
        return farthest;
    }

    private Transform GetStrongestTarget(List<Transform> targets)
    {
        Transform strongest = null;
        int maxHp = 0;
        foreach (Transform target in targets)
        {
            Health health = target.GetComponent<Health>();
            if (health != null && health.CurrentHealth > maxHp)
            {
                maxHp = health.CurrentHealth;
                strongest = target;
            }
        }
        return strongest ?? targets[0];
    }

    private Transform GetWeakestTarget(List<Transform> targets)
    {
        Transform weakest = null;
        int minHp = int.MaxValue;
        foreach (Transform target in targets)
        {
            Health health = target.GetComponent<Health>();
            if (health != null && health.CurrentHealth < minHp)
            {
                minHp = health.CurrentHealth;
                weakest = target;
            }
        }
        return weakest ?? targets[0];
    }
    
    #endregion
}