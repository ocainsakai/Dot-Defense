using UnityEngine;

public abstract class GameEffectSO : ScriptableObject
{
    [Header("Effect Info")] [SerializeField]
    protected string effectName = "Effect";

    [SerializeField] [TextArea(2, 4)] protected string description = "Effect description";

    [Header("Visual")] [SerializeField] protected GameObject vfxPrefab;
    [SerializeField] protected AudioClip soundEffect;

    /// <summary>
    /// Apply effect lên target GameObject
    /// Override method này trong các concrete effect
    /// </summary>
    /// <param name="target">GameObject nhận effect (Enemy)</param>
    /// <param name="damageMultiplier">Multiplier từ Tower (sau upgrade)</param>
    public abstract void ApplyEffect(GameObject target, float damageMultiplier = 1f);

    /// <summary>
    /// Spawn VFX nếu có
    /// </summary>
    protected void SpawnVFX(Vector3 position)
    {
        if (vfxPrefab != null)
        {
            GameObject vfx = Instantiate(vfxPrefab, position, Quaternion.identity);
            Destroy(vfx, 2f); // Auto destroy after 2 seconds
        }
    }

    /// <summary>
    /// Play sound effect nếu có
    /// </summary>
    protected void PlaySound(Vector3 position)
    {
        if (soundEffect != null)
        {
            AudioSource.PlayClipAtPoint(soundEffect, position);
        }
    }
}