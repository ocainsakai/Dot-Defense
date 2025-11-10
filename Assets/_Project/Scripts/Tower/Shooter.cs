using UnityEngine;

/// <summary>
/// Một component "dịch vụ" (service) chuyên nghiệp.
/// Cung cấp một "Toolbox" các kiểu bắn.
/// Không tự suy nghĩ (không có state), chỉ thực thi lệnh.
/// Người gọi (Caller) phải cung cấp mọi thông tin cần thiết.
/// </summary>
public class Shooter : MonoBehaviour
{
    // --- CÔNG CỤ 1: BẮN 1 TIA (Đã sửa) ---

    /// <summary>
    /// Bắn 1 viên đạn duy nhất, tự động xoay về phía mục tiêu.
    /// </summary>
    /// <param name="firePoint">Vị trí bắn (do người gọi cung cấp)</param>
    public void ShootSingle(ProjectileDataSO projData, Transform target, float damage, Transform firePoint)
    {
        if (target == null) return;

        // Tính toán vị trí và hướng bắn
        Vector3 spawnPos = firePoint.position;
        Quaternion spawnRot = CalculateAimRotation(firePoint.position, target.position);

        // Gọi helper
        SpawnOneProjectile(projData, damage, target, spawnPos, spawnRot);
    }

    // --- CÔNG CỤ 2: BẮN N TIA SONG SONG (Đã sửa) ---

    /// <summary>
    /// Bắn N viên đạn song song, cách nhau một khoảng 'offset'.
    /// Tất cả đạn đều hướng về mục tiêu.
    /// </summary>
    /// <param name="firePoint">Vị trí bắn (do người gọi cung cấp)</param>
    public void ShootMultiParallel(ProjectileDataSO projData, Transform target, float damage, Transform firePoint, int count, float offsetDistance)
    {
        if (target == null) return;

        // Tính toán hướng bắn (chung cho tất cả)
        Quaternion spawnRot = CalculateAimRotation(firePoint.position, target.position);
        
        // Tính vị trí bắt đầu
        float startOffset = -((count - 1) / 2f) * offsetDistance;
        
        for (int i = 0; i < count; i++)
        {
            // Tính vị trí spawn
            // Dùng firePoint.right để đảm bảo offset vuông góc với hướng bắn của Tower
            Vector3 offset = firePoint.right * (startOffset + i * offsetDistance);
            Vector3 spawnPos = firePoint.position + offset;
            
            // Spawn
            SpawnOneProjectile(projData, damage, target, spawnPos, spawnRot);
        }
    }

    // --- CÔNG CỤ 3: BẮN N TIA HÌNH QUẠT (Đã sửa) ---

    /// <summary>
    /// Bắn N viên đạn tỏa ra theo hình quạt.
    /// </summary>
    /// <param name="firePoint">Vị trí bắn (do người gọi cung cấp)</param>
    /// <param name="spreadAngle">Tổng góc của hình quạt (ví dụ: 30 độ)</param>
    public void ShootSpreadAngular(ProjectileDataSO projData, Transform target, float damage, Transform firePoint, int count, float spreadAngle)
    {
        if (target == null) return;

        // Vị trí bắn là cố định
        Vector3 spawnPos = firePoint.position;

        // Tính toán các góc
        float angleStep = (count > 1) ? spreadAngle / (count - 1) : 0;
        float startAngle = -spreadAngle / 2f;

        // Tính hướng trung tâm (hướng tới mục tiêu)
        Vector3 directionToTarget = (target.position - firePoint.position).normalized;
        float centerAngleRad = Mathf.Atan2(directionToTarget.y, directionToTarget.x); // (Tính cho 2D)

        for (int i = 0; i < count; i++)
        {
            // Tính góc xoay cho viên đạn này
            float currentAngleDeg = startAngle + (i * angleStep);
            float finalAngleRad = centerAngleRad + (currentAngleDeg * Mathf.Deg2Rad);
            
            // Tạo Quaternion xoay (cho 2D)
            Quaternion spawnRotation = Quaternion.Euler(0, 0, finalAngleRad * Mathf.Rad2Deg);
            
            // Spawn
            SpawnOneProjectile(projData, damage, target, spawnPos, spawnRotation);
        }
    }


    // --- HÀM HELPER (Private, đã tinh giản) ---

    /// <summary>
    /// Tính toán góc xoay (cho 2D) để nhìn về phía 'targetPos'.
    /// </summary>
    private Quaternion CalculateAimRotation(Vector3 fromPos, Vector3 targetPos)
    {
        Vector3 direction = (targetPos - fromPos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// Hàm helper nội bộ cuối cùng: Chỉ spawn và "tiêm" data.
    /// </summary>
    private void SpawnOneProjectile(
        ProjectileDataSO projData, 
        float damage, 
        Transform target, 
        Vector3 spawnPos, 
        Quaternion spawnRotation)
    {
        if (projData == null || target == null)
        {
            Debug.LogError("Shooter: ProjectileData hoặc Target bị NULL!", this);
            return;
        }

        // 1. Lấy đạn từ ObjectPooler
        GameObject projectileGO = ObjectPooler.Instance.SpawnFromPool(
            projData.PoolTag,
            spawnPos,
            spawnRotation, 
            projData.ProjectilePrefab // Cung cấp prefab dự phòng
        );

        if (projectileGO == null)
        {
            Debug.LogError($"Shooter: FAILED! Pool '{projData.PoolTag}' bị rỗng hoặc sai tag?");
            return;
        }

        // 2. "Tiêm" (Inject) data
        Projectile projectileComponent = projectileGO.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.Initialize(
                projData,
                damage,
                target,
                projData.PoolTag
            );
        }
        else
        {
            Debug.LogError($"Shooter: Prefab '{projData.ProjectilePrefab.name}' thiếu script 'Projectile'!", this);
        }
    }
}