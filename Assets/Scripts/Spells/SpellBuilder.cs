using System;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Random = UnityEngine.Random;

public class SpellBuilder : MonoBehaviour
{
    [Header("配置文件")]
    public TextAsset spellsJson;

    [Header("UI配置")]
    public Transform spellUIParent;    // UI槽位父节点
    public GameObject spellUIPrefab;   // 单个法术UI预制体

    private Dictionary<string, SpellData> spells = new Dictionary<string, SpellData>();
    private Dictionary<string, SpellModifier> modifiers = new Dictionary<string, SpellModifier>();
    public List<SpellData> activeSpells = new List<SpellData>(); // 当前显示的四个法术
    private int selectedSpellIndex = -1; // 当前选中的法术索引

    void Start()
    {
        LoadSpellsAndModifiers();
        // InitializeUI();
        AddNewSpell();
        AddNewSpell();
        AddNewSpell();
        AddNewSpell();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            AddNewSpell();
        }
    }

    /// <summary>
    /// 加载法术和修正器数据
    /// </summary>
    private void LoadSpellsAndModifiers()
    {
        JObject root = JObject.Parse(spellsJson.text);

        // 加载法术
        JObject spellsData = (JObject)root["spells"];
        foreach (var spell in spellsData)
        {
            spells.Add(spell.Key, JsonConvert.DeserializeObject<SpellData>(spell.Value.ToString()));
        }

        // 加载修正器
        JObject modifiersData = (JObject)root["modifiers"];
        foreach (var mod in modifiersData)
        {
            modifiers.Add(mod.Key, JsonConvert.DeserializeObject<SpellModifier>(mod.Value.ToString()));
        }
    }

    /// <summary>
    /// 初始化UI槽位
    /// </summary>
    private void InitializeUI()
    {
        for (int i = 0; i < 1; i++)
        {
            GameObject go = Instantiate(spellUIPrefab, spellUIParent);
            go.GetComponent<SpellUIItem>().ResetSpellUI();
        }
        
    }

    /// <summary>
    /// 随机选择一个法术或修正器
    /// </summary>
    public void RandomSelect()
    {
        bool isSpell = Random.Range(0, 2) == 0; // 50%概率选法术

        if (isSpell)
        {
            AddNewSpell();
        }
        else
        {
            ApplyModifierToSelectedSpell();
        }
    }

    /// <summary>
    /// 添加新法术到UI
    /// </summary>
    private void AddNewSpell()
    {
        if (activeSpells.Count >= 4)
        {
            Debug.LogWarning("已达最大法术数量（4个）");
            return;
        }

        // 随机选择一个法术
        var spellKeys = new List<string>(spells.Keys);
        Debug.Log("spellKeys.Count   "+spellKeys.Count);
        string randomSpellKey = spellKeys[Random.Range(0, spellKeys.Count)];
        Debug.Log("randomSpellKey   "+randomSpellKey);
        Debug.Log("randomSpellKey   "+Random.Range(0, spellKeys.Count));
        Debug.Log("randomSpellKey   "+Random.Range(0, spellKeys.Count));
        Debug.Log("randomSpellKey   "+Random.Range(0, spellKeys.Count));
        SpellData newSpell = spells[randomSpellKey];

        // 添加到列表并更新UI
        GameObject go = Instantiate(spellUIPrefab, spellUIParent);
        go.GetComponent<SpellUIItem>().ResetSpellUI();
        activeSpells.Add(newSpell);
        UpdateSpellUI(activeSpells.Count - 1, newSpell,randomSpellKey);
    }

    public Dictionary<string, SpellData> GetNewSpell()
    {

        // 随机选择一个法术
        var spellKeys = new List<string>(spells.Keys);
        string randomSpellKey = spellKeys[Random.Range(0, spellKeys.Count)];
        SpellData newSpell = spells[randomSpellKey];
        
        Dictionary<string, SpellData> randomSpell = new Dictionary<string, SpellData>();
        randomSpell.Add(randomSpellKey, newSpell);
        
        return randomSpell;
    }

    public void RemoveSelectedSpell(SpellData spell)
    {
        foreach (SpellData activeSpell in activeSpells)
        {
            if (activeSpell == spell)
            {
                
                activeSpells.Remove(activeSpell);
                break;
            }
            
        }
    }

    public bool CanAddNewSpell()
    {
        Debug.Log("activeSpells.Count   "+ activeSpells.Count);
        if (activeSpells.Count >= 4)
        {
            Debug.LogWarning("已达最大法术数量（4个）");
            return false;
        }
        else
        {
            return true;
        }
    }

    public void AddNewSpellBySpell(SpellData newSpell, string randomSpellKey)
    {
        if (activeSpells.Count >= 4)
        {
            Debug.LogWarning("已达最大法术数量（4个）");
            return;
        }
        GameObject go = Instantiate(spellUIPrefab, spellUIParent);
        go.GetComponent<SpellUIItem>().ResetSpellUI();
        activeSpells.Add(newSpell);
        UpdateSpellUI(activeSpells.Count - 1, newSpell,randomSpellKey);
    }


    public Dictionary<string, SpellModifier> GetModifier()
    {
        var modKeys = new List<string>(modifiers.Keys);
        string randomModKey = modKeys[Random.Range(0, modKeys.Count)];
        SpellModifier mod = modifiers[randomModKey];
        
        
        Dictionary<string, SpellModifier> randomModifier = new Dictionary<string, SpellModifier>();
        randomModifier.Add(randomModKey, mod);
        return randomModifier;
    }

    /// <summary>
    /// 应用修正器到选中的法术
    /// </summary>
    private void ApplyModifierToSelectedSpell()
    {
        if (selectedSpellIndex == -1)
        {
            Debug.LogWarning("未选中任何法术");
            return;
        }

        // 随机选择一个修正器
        var modKeys = new List<string>(modifiers.Keys);
        string randomModKey = modKeys[Random.Range(0, modKeys.Count)];
        SpellModifier mod = modifiers[randomModKey];

        // // 应用修正器逻辑（示例：叠加伤害乘数）
        // SpellData spell = activeSpells[selectedSpellIndex];
        // spell.spell_damage.amount = RpnCalculator.CombineRPN(
        //     spell.spell_damage.amount,
        //     mod.damage_multiplier,
        //     "*"
        // );

        // 更新UI显示
        // UpdateSpellUI(selectedSpellIndex, spell);
    }

    /// <summary>
    /// 更新指定槽位的UI
    /// </summary>
    private void UpdateSpellUI(int index, SpellData spell,string spellKey)
    {
        Transform slot = spellUIParent.GetChild(index);
        slot.GetComponent<SpellUIItem>().SetSpellUI(index,spell,spellKey);
        
        // SpellUIElement ui = slot.GetComponent<SpellUIElement>();
        
        // ui.SetIcon(spell.icon);
        // ui.SetDescription($"{spell.name}\n伤害: {spell.spell_damage.amount}");
    }

    /// <summary>
    /// 玩家点击UI槽位时调用
    /// </summary>
    public void OnSpellSelected(int index)
    {
        selectedSpellIndex = index;
        Debug.Log($"选中法术: {activeSpells[index].name}");
    }

    public void CloseHightLight()
    {
        foreach (Transform child in spellUIParent)
        {
            child.gameObject.GetComponent<SpellUIItem>().CloseHightLight();
        }
    }
}