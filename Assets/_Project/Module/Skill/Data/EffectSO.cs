using UnityEngine;

public abstract class EffectSO : ScriptableObject
{
    // Thực thi hành động lên mục tiêu,
    // do "caster" (người dùng) gây ra.
    public abstract void Execute(GameObject caster, GameObject target);
}