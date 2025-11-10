using UnityEngine;

[CreateAssetMenu(fileName = "TowerDataSO", menuName = "Scriptable Objects/TowerDataSO")]
public class TowerDataSO : ScriptableObject
{
    [Header("Base Stats")]
    public float baseMaxHP = 1000f;
    public float baseAttack = 50f;
    public float baseDefense = 10f;
    public float baseAttackSpeed = 1.0f;
    public float baseRange = 200f;

    [Header("Level Scaling")]
    public float attackPerLevel = 0.1f; // +10% mỗi level
    public float defensePerLevel = 0.05f;
    public float attackSpeedPerLevel = 0.02f;
    public float rangePerLevel = 0.03f;

    // Calculate stat tại level cụ thể
    public float GetAttack(int level) => baseAttack * (1 + level * attackPerLevel);
    public float GetDefense(int level) => baseDefense * (1 + level * defensePerLevel);
    public float GetAttackSpeed(int level) => baseAttackSpeed * (1 + level * attackSpeedPerLevel);
    public float GetRange(int level) => baseRange * (1 + level * rangePerLevel);
}
