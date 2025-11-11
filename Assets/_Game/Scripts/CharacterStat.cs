using UnityEngine;

// Đặt [System.Serializable] để bạn có thể thấy và chỉnh nó
// trực tiếp trên Inspector của Unity.
[System.Serializable]
public struct CharacterStats
{
    // Sử dụng long (hoặc double) thay vì int/float 
    // vì game idle thường có số rất lớn.
    
    [Header("Chỉ số Cơ bản")]
    public long Health;      // Máu
    public long Attack;      // Công
    public long Defense;     // Thủ
    public long AttackSpeed;
    [Header("Chỉ số Phụ (Ví dụ)")]
    public float CritChance;  // Tỷ lệ chí mạng (ví dụ: 0.05 = 5%)
    public float CritDamage;  // Sát thương chí mạng (ví dụ: 1.5 = 150%)
}