using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance;
    
    [Header("配置")]
    public TextAsset spellsJson;      // JSON配置文件
    public List<Sprite> projectileSprites; // 投射物精灵列表

    [Header("预制体")]
    public GameObject projectilePrefab; // 基础投射物预制体

    private Dictionary<string, SpellData> spells = new Dictionary<string, SpellData>();
    
    public Dictionary<string, SpellModifier> modifiers = new Dictionary<string, SpellModifier>();

    void Awake()
    {
        Instance = this;
        LoadSpells();
    }

    void LoadSpells()
    {
        // var settings = new JsonSerializerSettings
        // {
        //     MissingMemberHandling = MissingMemberHandling.Error
        // };
        //
        // var spellDict = JsonConvert.DeserializeObject<Dictionary<string, SpellData>>(
        //     spellsJson.text, 
        //     settings
        // );
        //
        // foreach (var kvp in spellDict)
        // {
        //     spells.Add(kvp.Key, kvp.Value);
        // }
        
        JObject root = JObject.Parse(spellsJson.text);

        // 加载法术
        JObject spellsData = (JObject)root["spells"];
        foreach (var spell in spellsData)
        {
            spells.Add(spell.Key, JsonConvert.DeserializeObject<SpellData>(spell.Value.ToString()));
        }

        // 加载修正器
        JObject modsData = (JObject)root["modifiers"];
        foreach (var mod in modsData)
        {
            modifiers.Add(mod.Key, JsonConvert.DeserializeObject<SpellModifier>(mod.Value.ToString()));
        }
    }

    /// <summary>
    /// 获取法术配置
    /// </summary>
    public SpellData GetSpell(string spellId)
    {
        if (spells.TryGetValue(spellId, out SpellData data))
            return data;
        return null;
    }
    
    
    // 新增：获取修正器列表
    public List<SpellModifier> GetModifiersForSpell(string spellId)
    {
        // 这里可以扩展匹配逻辑（示例直接返回所有）
        return new List<SpellModifier>(modifiers.Values);
    }
    
    // 新增方法
    public List<string> GetAllSpellIds() => new List<string>(spells.Keys);
    public List<string> GetAllModifierIds() => new List<string>(modifiers.Keys);
    
    
    

    /// <summary>
    /// 创建投射物实例
    /// </summary>
    public GameObject CreateProjectile(ProjectileData config, Vector2 position, Quaternion rotation)
    {
        GameObject projectile = Instantiate(projectilePrefab, position, rotation);
        // SpriteRenderer sr = projectile.GetComponent<SpriteRenderer>();
        //
        // if (config.sprite < projectileSprites.Count && config.sprite >= 0)
        //     sr.sprite = projectileSprites[config.sprite];
        
        return projectile;
    }
    
  
    
    
}