using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class ClassManager : MonoBehaviour
{
    public static ClassManager Instance;
    
    [Header("配置文件")]
    public TextAsset classesJson;

    private Dictionary<string, CharacterClass> classes = 
        new Dictionary<string, CharacterClass>();

    void Awake()
    {
        Instance = this;
        LoadClasses();
    }

    private void LoadClasses()
    {
        string json = classesJson.text;
        var data = JsonConvert.DeserializeObject<Dictionary<string, CharacterClass>>(json);
        
        foreach (var kvp in data)
        {
            classes.Add(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// 获取角色属性值
    /// </summary>
    /// <param name="className">职业名称（如 "mage"）</param>
    /// <param name="wave">当前波次</param>
    public CharacterStats GetClassStats(string className, int wave)
    {
        if (!classes.ContainsKey(className))
        {
            throw new System.ArgumentException($"无效职业名称: {className}");
        }

        CharacterClass cls = classes[className];
        return new CharacterStats
        {
            sprite = cls.sprite,
            health = RpnCalculator.Calculate(cls.health, waveValue: wave),
            mana = RpnCalculator.Calculate(cls.mana, waveValue: wave),
            manaRegeneration = RpnCalculator.Calculate(cls.mana_regeneration, waveValue: wave),
            spellpower = RpnCalculator.Calculate(cls.spellpower, waveValue: wave),
            speed = RpnCalculator.Calculate(cls.speed, waveValue: wave)
        };
    }
}

/// <summary>
/// 计算后的角色属性
/// </summary>
public struct CharacterStats
{
    public int sprite;
    public float health;
    public float mana;
    public float manaRegeneration;
    public float spellpower;
    public float speed;
}