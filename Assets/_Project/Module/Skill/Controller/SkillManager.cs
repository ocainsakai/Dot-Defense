using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;
    
    // "Hợp đồng" mà "Game" phải cung cấp
    private IResourceProvider resourceProvider;
    private ICasterProvider casterProvider;

    // Quản lý Cooldown (Nội bộ)
    private Dictionary<ActiveSkillSO, float> cooldowns;

    private void Awake()
    {
        Instance = this;
        cooldowns = new Dictionary<ActiveSkillSO, float>();
    }

    /// <summary>
    /// "Game" phải gọi hàm này lúc khởi tạo
    /// </summary>
    public void Initialize(IResourceProvider resources, ICasterProvider caster)
    {
        this.resourceProvider = resources;
        this.casterProvider = caster;
    }

    /// <summary>
    /// Được gọi bởi Nút bấm UI (từ "Game")
    /// </summary>
    public void AttemptToUseSkill(ActiveSkillSO skill)
    {
        // 1. Kiểm tra "Game" xem có đủ tài nguyên
        if (!resourceProvider.HasResource(skill.ResourceType, skill.Cost))
        {
            Debug.Log("SkillModule: Không đủ " + skill.ResourceType);
            return;
        }

        // 2. Kiểm tra Cooldown (nội bộ)
        if (cooldowns.GetValueOrDefault(skill, 0f) > Time.time)
        {
            Debug.Log("SkillModule: Đang cooldown!");
            return;
        }

        // 3. KÍCH HOẠT
        // 3a. Bảo "Game" trừ tài nguyên
        resourceProvider.SpendResource(skill.ResourceType, skill.Cost);
        
        // 3b. Lấy thông tin Caster/Target từ "Game"
        GameObject caster = casterProvider.GetCaster();
        GameObject target = casterProvider.GetCurrentTarget();

        // 3c. Thực thi tất cả hiệu ứng
        foreach (var effect in skill.EffectsToApply)
        {
            effect.Execute(caster, target);
        }

        // 3d. Bắt đầu Cooldown
        cooldowns[skill] = Time.time + skill.Cooldown;
    }
    
    // (Hàm này cho UI của "Game" gọi để vẽ cooldown)
    public float GetCooldownProgress(ActiveSkillSO skill)
    {
        float cooldownEndTime = cooldowns.GetValueOrDefault(skill, 0f);
        if (cooldownEndTime > Time.time)
        {
            return (cooldownEndTime - Time.time) / skill.Cooldown;
        }
        return 0f;
    }
}