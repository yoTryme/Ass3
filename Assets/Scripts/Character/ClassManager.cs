using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class ClassManager : MonoBehaviour
{
    private Dictionary<string, CharacterStats> classData;
    public static ClassManager Instance;

    void Awake()
    {
        Instance = this;
        LoadClassData();
    }

    private void LoadClassData()
    {
        classData = new Dictionary<string, CharacterStats>();

        TextAsset jsonFile = Resources.Load<TextAsset>("classes");
        if (jsonFile == null)
        {
            Debug.LogError("Class data file not found!");
            return;
        }

        JObject parsed = JObject.Parse(jsonFile.text);

        foreach (var pair in parsed)
        {
            string classKey = pair.Key;
            JObject data = (JObject)pair.Value;

            CharacterStats stats = new CharacterStats
            {
                sprite = data["sprite"]?.ToObject<int>() ?? 0,
                health = Evaluate(data, "health", 0),
                mana = Evaluate(data, "mana", 0),
                manaRegeneration = Evaluate(data, "mana_regeneration", 0),
                spellpower = Evaluate(data, "spellpower", 0),
                speed = Evaluate(data, "speed", 0)
            };

            classData[classKey] = stats;
        }
    }

    public CharacterStats GetClassStats(string classKey, int wave)
    {
        if (!classData.ContainsKey(classKey))
        {
            Debug.LogWarning($"Class {classKey} not found.");
            return new CharacterStats();
        }

        CharacterStats baseStats = classData[classKey];
        JObject data = JObject.Parse(Resources.Load<TextAsset>("classes").text)[classKey] as JObject;

        return new CharacterStats
        {
            sprite = baseStats.sprite,
            health = Evaluate(data, "health", wave),
            mana = Evaluate(data, "mana", wave),
            manaRegeneration = Evaluate(data, "mana_regeneration", wave),
            spellpower = Evaluate(data, "spellpower", wave),
            speed = Evaluate(data, "speed", wave)
        };
    }

    private float Evaluate(JObject data, string key, int wave)
    {
        if (!data.ContainsKey(key)) return 0f;
        string expr = data[key].ToString();
        return RpnCalculator.Calculate(expr, 0, wave);
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