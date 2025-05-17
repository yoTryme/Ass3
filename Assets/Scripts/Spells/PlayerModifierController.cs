using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class PlayerModifierController : MonoBehaviour
{
    // 当前激活的修正器
    private Dictionary<string, List<SpellModifier>> activeModifiers = new Dictionary<string, List<SpellModifier>>();

    // 应用修正器到指定法术
    public void ApplyModifier(string spellId, string modifierId)
    {
        if (SpellManager.Instance.modifiers.TryGetValue(modifierId, out SpellModifier mod))
        {
            if (!activeModifiers.ContainsKey(spellId))
                activeModifiers[spellId] = new List<SpellModifier>();
            
            activeModifiers[spellId].Add(mod);
            Debug.Log($"已为{spellId}应用修正：{mod.name}");
        }
    }

    // 获取最终法术属性
    public SpellData GetModifiedSpell(string spellId, float power)
    {
        SpellData baseSpell = SpellManager.Instance.GetSpell(spellId);
        SpellData modifiedSpell = JsonConvert.DeserializeObject<SpellData>(JsonConvert.SerializeObject(baseSpell));

        
        
        if (!activeModifiers.ContainsKey(spellId)) return modifiedSpell;

        foreach (SpellModifier mod in activeModifiers[spellId])
        {
            // 应用伤害数值修正
            modifiedSpell.spell_damage.amount = CombineRPN(
                modifiedSpell.spell_damage.amount, 
                mod.damage_multiplier, 
                power, 
                "*");
            // 应用法力修正
            modifiedSpell.mana_cost = CombineRPN(
                modifiedSpell.mana_cost, 
                mod.mana_multiplier, 
                power, 
                "*");
            // 投射物速度修正
            modifiedSpell.projectile.speed = RpnCalculator.Calculate(
                $"{modifiedSpell.projectile.speed} {mod.speed_multiplier} *",
                power
            );

            // 冷却时间修正
            modifiedSpell.cooldown *= RpnCalculator.Calculate(
                mod.cooldown_multiplier, 
                power
            );
            
            
            // // 记录延迟时间（在施法逻辑中使用）
            // modifiedSpell.delay = RpnCalculator.Calculate(mod.delay, power);

            // 应用特殊效果
            if (!string.IsNullOrEmpty(mod.projectile_trajectory))
                modifiedSpell.projectile.trajectory = mod.projectile_trajectory;
        }

        return modifiedSpell;
    }
    
    // 新增方法：检查是否存在特定修正器
    public bool HasModifier(string spellId, string modifierName)
    {
        if (activeModifiers.TryGetValue(spellId, out List<SpellModifier> mods))
        {
            return mods.Exists(m => m.name == modifierName);
        }
        return false;
    }

// 新增方法：获取修正器的参数
    public T GetModifierValue<T>(string spellId, string modifierName, string fieldName, T defaultValue)
    {
        if (activeModifiers.TryGetValue(spellId, out List<SpellModifier> mods))
        {
            foreach (SpellModifier mod in mods)
            {
                if (mod.name == modifierName)
                {
                    System.Reflection.FieldInfo field = typeof(SpellModifier).GetField(fieldName);
                    if (field != null)
                    {
                        object value = field.GetValue(mod);
                        if (value is T)
                            return (T)value;
                    }
                }
            }
        }
        return defaultValue;
    }

    private string CombineRPN(string baseExp, string modifierExp, float power, string op)
    {
        if (modifierExp == "1") return baseExp; // 优化默认值
    
        // 示例结果："20 power 3 / + 1.5 *"
        return $"{baseExp} {modifierExp} {op}";
    }
}