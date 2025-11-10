using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaveManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private EnemySpawner enemySpawner; // "Đôi tay"
    [SerializeField] private WaveConfigSO waveConfig;  // "Data"
    [SerializeField] private Transform targetBase;     // "Mục tiêu"

    [Header("Enemy Types")]
    public List<EnemyTypeSO> enemyTypes; // Danh sách enemy sẽ spawn

    [Header("Runtime Info (Read Only)")]
    [SerializeField] private int currentWave = 0;
    [SerializeField] private int currentEnemiesAlive = 0;
    [SerializeField] private int enemiesToSpawnThisWave = 0;
    
    public System.Action<int> OnWaveStart;
    public System.Action<int> OnWaveComplete;
    public System.Action<int, int> OnEnemyCountChanged;

    private bool isSpawning = false;
    private Coroutine waveCoroutine;
    private Dictionary<string, float> originalWeights = new Dictionary<string, float>();

    private void Start()
    {
        // Đăng ký nghe "báo cáo" từ Spawner
        enemySpawner.OnEnemyDied += HandleEnemyDeath;

        // Lưu weight gốc (để reset)
        foreach (EnemyTypeSO type in enemyTypes)
        {
            originalWeights[type.poolTag] = type.spawnWeight;
        }
    }

    public void StartWaves(int startWave = 1)
    {
        if (isSpawning) return;

        isSpawning = true;
        currentWave = startWave - 1; // Bắt đầu ở wave 0, coroutine sẽ ++ lên 1
        waveCoroutine = StartCoroutine(EndlessWaveSystem());
    }

    public void StopWaves()
    {
        isSpawning = false;
        if (waveCoroutine != null)
        {
            StopCoroutine(waveCoroutine);
        }
    }

    private void HandleEnemyDeath()
    {
        currentEnemiesAlive--;
        OnEnemyCountChanged?.Invoke(currentEnemiesAlive, enemiesToSpawnThisWave);
    }

    private IEnumerator EndlessWaveSystem()
    {
        while (isSpawning)
        {
            currentWave++;
            SaveManager.SaveCurrentWave(currentWave);
            // Đọc data từ SO
            enemiesToSpawnThisWave = waveConfig.CalculateEnemyCount(currentWave);
            float spawnInterval = waveConfig.CalculateSpawnInterval(currentWave);
            float restTime = waveConfig.CalculateRestTime(currentWave);

            AdjustDifficultyScaling();

            OnWaveStart?.Invoke(currentWave);

            // Bắt đầu spawn
            yield return StartCoroutine(SpawnWave(enemiesToSpawnThisWave, spawnInterval));

            // Chờ wave kết thúc
            yield return new WaitUntil(() => currentEnemiesAlive == 0);

            OnWaveComplete?.Invoke(currentWave);
            
            yield return new WaitForSeconds(restTime);
        }
    }

    private IEnumerator SpawnWave(int count, float interval)
    {
        currentEnemiesAlive = 0;
        for (int i = 0; i < count; i++)
        {
            if (!isSpawning) yield break; 

            // 1. Quyết định spawn con gì
            EnemyTypeSO selectedType = GetWeightedRandomEnemy();
            if (selectedType == null) continue;

            // 2. Ra lệnh cho "Spawner"
            enemySpawner.SpawnEnemy(selectedType, targetBase);
            
            currentEnemiesAlive++;
            OnEnemyCountChanged?.Invoke(currentEnemiesAlive, enemiesToSpawnThisWave);

            yield return new WaitForSeconds(interval);
        }
    }

    private EnemyTypeSO GetWeightedRandomEnemy()
    {
        List<EnemyTypeSO> availableTypes = enemyTypes.FindAll(
            type => type.minWaveToAppear <= currentWave
        );
        if (availableTypes.Count == 0) return null;

        float totalWeight = availableTypes.Sum(type => type.spawnWeight);
        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        foreach (EnemyTypeSO type in availableTypes)
        {
            cumulativeWeight += type.spawnWeight;
            if (randomValue <= cumulativeWeight)
            {
                return type;
            }
        }
        return availableTypes[0]; // Fallback
    }

    private void AdjustDifficultyScaling()
    {
        if (currentWave % waveConfig.wavesToIncreaseHarderEnemies == 0)
        {
            foreach (EnemyTypeSO type in enemyTypes)
            {
                if (type.minWaveToAppear > 1)
                {
                    float multiplier = type.minWaveToAppear * 0.5f;
                    type.spawnWeight += waveConfig.weightIncreaseAmount * multiplier;
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (enemySpawner != null)
        {
            enemySpawner.OnEnemyDied -= HandleEnemyDeath;
        }
    }
}