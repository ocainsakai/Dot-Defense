using UnityEngine;

[CreateAssetMenu(fileName = "WaveConfigSO", menuName = "Scriptable Objects/WaveConfigSO")]
public class WaveConfigSO : ScriptableObject
{
    [Header("Progression Settings")]
    [Tooltip("Số enemy ban đầu mỗi wave")]
    public int baseEnemiesPerWave = 5;
    
    [Tooltip("Tăng bao nhiêu enemy mỗi wave (có thể là số lẻ)")]
    public float enemyIncreasePerWave = 1.2f;
    
    [Tooltip("Số enemy tối đa mỗi wave")]
    public int maxEnemiesPerWave = 50;
    
    [Header("Timing Settings")]
    [Tooltip("Thời gian nghỉ giữa các wave")]
    public float timeBetweenWaves = 5f;
    
    [Tooltip("Giảm bao nhiêu giây mỗi wave (wave càng cao càng nghỉ ít)")]
    public float timeBetweenWavesDecrease = 0.1f;
    
    [Tooltip("Thời gian nghỉ tối thiểu")]
    public float minTimeBetweenWaves = 1f;
    
    [Header("Spawn Interval")]
    [Tooltip("Khoảng cách spawn ban đầu (giây)")]
    public float baseSpawnInterval = 0.8f;
    
    [Tooltip("Giảm bao nhiêu giây mỗi wave")]
    public float spawnIntervalDecrease = 0.03f;
    
    [Tooltip("Khoảng cách spawn tối thiểu")]
    public float minSpawnInterval = 0.2f;

    [Header("Difficulty Scaling")]
    [Tooltip("Cứ mỗi X wave thì tăng spawn weight của enemy mạnh hơn")]
    public int wavesToIncreaseHarderEnemies = 5;
    
    [Tooltip("Tăng weight bao nhiêu mỗi lần")]
    public float weightIncreaseAmount = 0.1f;

    // --- Chuyển các hàm tính toán vào đây ---

    public int CalculateEnemyCount(int currentWave)
    {
        int count = Mathf.RoundToInt(baseEnemiesPerWave + (currentWave - 1) * enemyIncreasePerWave);
        return Mathf.Min(count, maxEnemiesPerWave);
    }

    public float CalculateSpawnInterval(int currentWave)
    {
        float interval = baseSpawnInterval - (currentWave - 1) * spawnIntervalDecrease;
        return Mathf.Max(interval, minSpawnInterval);
    }

    public float CalculateRestTime(int currentWave)
    {
        float restTime = timeBetweenWaves - (currentWave - 1) * timeBetweenWavesDecrease;
        return Mathf.Max(restTime, minTimeBetweenWaves);
    }
}
