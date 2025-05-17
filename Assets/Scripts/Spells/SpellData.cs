using UnityEngine;

[System.Serializable]
public class ProjectileData
{
    public string trajectory; // 弹道类型
    public float speed;       // 移动速度
    public float lifetime;    // 存在时间（次级投射物）
    public int sprite;        // 使用的精灵索引
}

[System.Serializable]
public class SpellData
{
    public string name;
    public string description;
    public int icon;
    public string N;                     // 次级子弹数量表达式
    public SpellDamage spell_damage;     // 主伤害
    public string secondary_damage;      // 次级伤害表达式
    public string mana_cost;             // 法力消耗表达式
    public float cooldown;               // 冷却时间
    public ProjectileData projectile;            // 主投射物参数
    public ProjectileData secondary_projectile;  // 次级投射物参数
    public float delay;

    public float spray; // 新增喷雾距离字段
    
}

[System.Serializable]
public class SpellDamage
{
    public string amount; // 伤害量表达式
    public string type;   // 伤害类型
}

// 新增修正器数据结构
[System.Serializable]
public class SpellModifier
{
    public string name;
    public string description;
    public string damage_multiplier = "1";
    public string mana_multiplier = "1";
    public string speed_multiplier = "1";
    public string cooldown_multiplier = "1";
    public string delay;
    public string angle;
    public string mana_adder;
    public string projectile_trajectory;
}