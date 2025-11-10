using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class EnemySO : ScriptableObject
{
    public int MaxHealth;
    public int Speed;
    public int Damage;
    public int Reward;
}
