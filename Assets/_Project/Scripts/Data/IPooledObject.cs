public interface IPooledObject
{
    /// <summary>
    /// Được gọi khi object được spawn từ pool
    /// Dùng để reset các giá trị về mặc định
    /// </summary>
    void OnObjectSpawn();
}