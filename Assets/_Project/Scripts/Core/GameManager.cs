using UnityEngine;
// KHÔNG CẦN using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState currentState;

    [Header("Core Components")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private WaveManager waveManager; // THAY ĐỔI: Dùng WaveManager
    [SerializeField] private Health playerBaseHealth; // THÊM: Để biết khi thua
    [SerializeField] private TowerResource towerResource;
    [SerializeField] private Tower tower;
    [SerializeField] private SkillManager skillManager;
 //   [SerializeField] private Tower tower; // THÊM: Để tải nâng cấp

    private int currentWave = 1;
    public int CurrentWave => currentWave;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 1. Tải dữ liệu đã lưu
        LoadGameData();

        // 2. Đăng ký sự kiện
        if (playerBaseHealth != null)
        {
            playerBaseHealth.OnDeath += HandlePlayerLoss;
        }
        if (waveManager != null)
        {
            waveManager.OnWaveComplete += HandleWaveComplete;
        }

        // 3. Bắt đầu ở Main Menu
        ChangeState(GameState.MainMenu);
    }

    /// <summary>
    /// Tải Wave và Nâng cấp từ PlayerPrefs
    /// </summary>
    private void LoadGameData()
    {
        // Tải wave cuối cùng
        currentWave = SaveManager.LoadCurrentWave();
        
        skillManager.Initialize(towerResource,tower);
        
    }

    private void HandlePlayerLoss()
    {
        ChangeState(GameState.GameOver);
    }

    /// <summary>
    /// Được gọi bởi sự kiện OnWaveComplete của WaveManager
    /// </summary>
    private void HandleWaveComplete(int completedWave)
    {
        // Tăng wave và lưu lại
        currentWave++;
        SaveManager.SaveCurrentWave(currentWave);
        
        // (WaveManager sẽ tự động chạy wave tiếp theo)
    }

    /// <summary>
    /// Công tắc chính điều khiển trạng thái game
    /// </summary>
    public void ChangeState(GameState newState)
    {
        currentState = newState;
        
        switch (newState)
        {
            case GameState.MainMenu:
                uiManager.ShowStartPanel(); 
                Time.timeScale = 0f; 
                break;
                
            case GameState.Playing:
                uiManager.ShowGameplayUI();
                Time.timeScale = 1f;
                // Báo WaveManager bắt đầu từ wave hiện tại
                waveManager.StartWaves(currentWave); 
                break;
                
            case GameState.GameOver:
                uiManager.ShowGameOverPanel();
                Time.timeScale = 0f; 
                waveManager.StopWaves();
                break;
        }
    }

    // --- Các hàm này sẽ được gọi bởi NÚT (Button) ---

    /// <summary>
    /// Được gọi bởi nút "Start"
    /// </summary>
    public void StartGame()
    {
        ChangeState(GameState.Playing);
    }

    /// <summary>
    /// THAY THẾ "RestartGame"
    /// Được gọi bởi nút "Retry" (Thử lại) trên panel GameOver
    /// </summary>
    public void RetryWave()
    {
        // 1. Dọn dẹp tất cả quái cũ
        ObjectPooler.Instance.ResetAllPools(); // (Bạn cần viết hàm này)
        
        // 2. Reset máu trụ
        playerBaseHealth.ResetHealth();
        
        // 3. Bắt đầu lại màn chơi (vẫn ở wave đó)
        ChangeState(GameState.Playing);
    }

    /// <summary>
    /// Hủy đăng ký sự kiện để tránh lỗi
    /// </summary>
    private void OnDestroy()
    {
        if (playerBaseHealth != null)
        {
            playerBaseHealth.OnDeath -= HandlePlayerLoss;
        }
        if (waveManager != null)
        {
            waveManager.OnWaveComplete -= HandleWaveComplete;
        }
    }
}