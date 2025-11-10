using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Health component quản lý HP của Entity (Enemy, Tower, PlayerBase)
/// Tuân thủ Single Responsibility Principle - chỉ quản lý HP logic
/// </summary>
public class Health : MonoBehaviour
{
    #region Properties
    
    public int MaxHealth
    {
        get => maxHealth;
        set
        {
            maxHealth = Mathf.Max(1, value); // Đảm bảo MaxHealth >= 1
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }
    }

    public int CurrentHealth
    {
        get => currentHealth;
        private set // Private set để control thông qua methods
        {
            int oldHealth = currentHealth;
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            
            // Chỉ trigger event nếu HP thực sự thay đổi
            if (oldHealth != currentHealth)
            {
                OnHealthChanged?.Invoke(currentHealth);
                
                // Trigger event khi chết
                if (currentHealth <= 0 && oldHealth > 0)
                {
                    Debug.Log("Health destroyed");
                    OnDeath?.Invoke();
                }
            }
        }
    }

    public bool IsDead => currentHealth <= 0;
    
    public float HealthPercentage => maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
    
    #endregion

    #region Serialized Fields
    
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;
    
    #endregion

    #region Events
    
    /// <summary>
    /// Event được gọi khi HP thay đổi
    /// Parameter: Current HP
    /// </summary>
    public UnityAction<int> OnHealthChanged;
    
    /// <summary>
    /// Event được gọi khi HP giảm (nhận damage)
    /// Parameters: damage amount, current HP
    /// </summary>
    public UnityAction<int, int> OnDamaged;
    
    /// <summary>
    /// Event được gọi khi HP tăng (heal)
    /// Parameters: heal amount, current HP
    /// </summary>
    public UnityAction<int, int> OnHealed;
    
    /// <summary>
    /// Event được gọi khi entity chết (HP <= 0)
    /// </summary>
    public UnityAction OnDeath;
    
    #endregion

    #region Unity Lifecycle
    
    private void Awake()
    {
        // Initialize HP nếu chưa set
        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;
        }
    }

    private void OnValidate()
    {
        // Đảm bảo giá trị hợp lệ trong Editor
        maxHealth = Mathf.Max(1, maxHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
    
    #endregion

    #region Public Methods
    
    /// <summary>
    /// Nhận sát thương
    /// </summary>
    /// <param name="damage">Lượng damage nhận vào (phải > 0)</param>
    /// <returns>True nếu entity chết sau khi nhận damage</returns>
    public bool TakeDamage(int damage)
    {
        if (IsDead)
        {
            if (showDebugLogs)
                Debug.LogWarning($"{gameObject.name} is already dead!");
            return true;
        }

        if (damage <= 0)
        {
            if (showDebugLogs)
                Debug.LogWarning($"Damage must be positive! Received: {damage}");
            return false;
        }

        int oldHealth = currentHealth;
        CurrentHealth -= damage;
        
        OnDamaged?.Invoke(damage, currentHealth);
        
        if (showDebugLogs)
        {
            Debug.Log($"{gameObject.name} took {damage} damage. HP: {oldHealth} -> {currentHealth}");
        }

        return IsDead;
    }

    /// <summary>
    /// Hồi máu
    /// </summary>
    /// <param name="amount">Lượng HP hồi (phải > 0)</param>
    /// <returns>Lượng HP thực sự hồi được</returns>
    public int Heal(int amount)
    {
        if (IsDead)
        {
            if (showDebugLogs)
                Debug.LogWarning($"{gameObject.name} is dead, cannot heal!");
            return 0;
        }

        if (amount <= 0)
        {
            if (showDebugLogs)
                Debug.LogWarning($"Heal amount must be positive! Received: {amount}");
            return 0;
        }

        int oldHealth = currentHealth;
        CurrentHealth += amount;
        int actualHealed = currentHealth - oldHealth;
        
        OnHealed?.Invoke(actualHealed, currentHealth);
        
        if (showDebugLogs)
        {
            Debug.Log($"{gameObject.name} healed {actualHealed} HP. HP: {oldHealth} -> {currentHealth}");
        }

        return actualHealed;
    }

    /// <summary>
    /// Hồi máu đầy
    /// </summary>
    public void HealToFull()
    {
        if (currentHealth < maxHealth)
        {
            int healed = maxHealth - currentHealth;
            CurrentHealth = maxHealth;
            OnHealed?.Invoke(healed, currentHealth);
            
            if (showDebugLogs)
            {
                Debug.Log($"{gameObject.name} fully healed to {maxHealth} HP");
            }
        }
    }

    /// <summary>
    /// Set HP về một giá trị cụ thể (dùng cho initialization)
    /// </summary>
    public void SetHealth(int health)
    {
        CurrentHealth = health;
        
        if (showDebugLogs)
        {
            Debug.Log($"{gameObject.name} HP set to {currentHealth}");
        }
    }

    /// <summary>
    /// Reset HP về MaxHealth
    /// </summary>
    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
        
        if (showDebugLogs)
        {
            Debug.Log($"{gameObject.name} HP reset to {maxHealth}");
        }
    }

    /// <summary>
    /// Tăng MaxHealth và heal theo tỉ lệ
    /// </summary>
    /// <param name="additionalMaxHp">Lượng max HP thêm vào</param>
    /// <param name="healPercentage">% HP hồi (0-1)</param>
    public void IncreaseMaxHealth(int additionalMaxHp, float healPercentage = 1f)
    {
        if (additionalMaxHp <= 0) return;

        float currentPercent = HealthPercentage;
        maxHealth += additionalMaxHp;
        
        // Heal theo tỉ lệ
        int newHealth = Mathf.RoundToInt(maxHealth * Mathf.Lerp(currentPercent, 1f, healPercentage));
        CurrentHealth = newHealth;
        
        if (showDebugLogs)
        {
            Debug.Log($"{gameObject.name} MaxHealth increased by {additionalMaxHp}. New MaxHealth: {maxHealth}");
        }
    }

    /// <summary>
    /// Kill entity ngay lập tức
    /// </summary>
    public void Kill()
    {
        if (!IsDead)
        {
            CurrentHealth = 0;
            
            if (showDebugLogs)
            {
                Debug.Log($"{gameObject.name} was killed instantly");
            }
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Kiểm tra có đủ HP hay không
    /// </summary>
    public bool HasHealthAbove(int threshold)
    {
        return currentHealth > threshold;
    }

    /// <summary>
    /// Kiểm tra HP có dưới một ngưỡng hay không
    /// </summary>
    public bool IsHealthBelow(int threshold)
    {
        return currentHealth < threshold;
    }

    /// <summary>
    /// Kiểm tra HP có ở trạng thái critical (dưới 25%)
    /// </summary>
    public bool IsCriticalHealth()
    {
        return HealthPercentage < 0.25f;
    }

    #endregion

    #region Debug

    /// <summary>
    /// Hiển thị thông tin health trong Console
    /// </summary>
    [ContextMenu("Log Health Info")]
    private void LogHealthInfo()
    {
        Debug.Log($"=== {gameObject.name} Health Info ===\n" +
                  $"Current HP: {currentHealth}\n" +
                  $"Max HP: {maxHealth}\n" +
                  $"Percentage: {HealthPercentage:P1}\n" +
                  $"Is Dead: {IsDead}\n" +
                  $"Is Critical: {IsCriticalHealth()}");
    }

    /// <summary>
    /// Test damage trong Editor
    /// </summary>
    [ContextMenu("Test Take 50 Damage")]
    private void TestDamage()
    {
        TakeDamage(50);
    }

    /// <summary>
    /// Test heal trong Editor
    /// </summary>
    [ContextMenu("Test Heal 50")]
    private void TestHeal()
    {
        Heal(50);
    }

    #endregion
}
