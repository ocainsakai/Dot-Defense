using UnityEngine;

[CreateAssetMenu(fileName = "BurnGameEffectSo", menuName = "Scriptable Objects/BurnGameEffectSo")]
public class BurnGameEffectSo : GameEffectSO
{
    [Header("Burn Settings")]
    [SerializeField] private int damagePerSecond = 5;
    [SerializeField] private float duration = 3f;
    [SerializeField] private float tickRate = 0.5f; // Damage mỗi 0.5 giây
    [SerializeField] private bool isStackable = true;
    
    public override void ApplyEffect(GameObject target, float damageMultiplier = 1f)
    {
        // Tìm hoặc thêm BurnHandler component
        BurnHandler burnHandler = target.GetComponent<BurnHandler>();
        if (burnHandler == null)
        {
            burnHandler = target.AddComponent<BurnHandler>();
        }
        
        // Apply burn
        int finalDps = Mathf.RoundToInt(damagePerSecond * damageMultiplier);
        burnHandler.StartBurn(finalDps, duration, tickRate, isStackable);
        
        // Effects
        SpawnVFX(target.transform.position);
        PlaySound(target.transform.position);
        
        Debug.Log($"🔥 {effectName} applied {finalDps} DPS for {duration}s to {target.name}");
    }
}