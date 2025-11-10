using UnityEngine;

public class BurnHandler : MonoBehaviour
{
    private Health health;
    private int currentDps;
    private float remainingDuration;
    private float tickRate;
    private float nextTickTime;
    private int stackCount = 0;
    
    private void Awake()
    {
        health = GetComponent<Health>();
    }
    
    public void StartBurn(int dps, float duration, float tickRate, bool isStackable)
    {
        if (isStackable)
        {
            stackCount++;
            currentDps += dps;
        }
        else
        {
            currentDps = dps;
        }
        
        this.tickRate = tickRate;
        remainingDuration = Mathf.Max(remainingDuration, duration);
        
        if (!enabled)
        {
            enabled = true;
            nextTickTime = Time.time + tickRate;
        }
    }
    
    private void Update()
    {
        if (health == null || health.IsDead)
        {
            enabled = false;
            return;
        }
        
        remainingDuration -= Time.deltaTime;
        
        if (remainingDuration <= 0)
        {
            enabled = false;
            stackCount = 0;
            currentDps = 0;
            return;
        }
        
        if (Time.time >= nextTickTime)
        {
            int damage = Mathf.RoundToInt(currentDps * tickRate);
            health.TakeDamage(damage);
            nextTickTime = Time.time + tickRate;
        }
    }
}