using UnityEngine;

using System;

[Serializable]
public class TriggerData
{
    public string description;
    public string type;   // "take-damage","on-kill","stand-still"…
    public int amount; // 用于 stand-still 秒数
}

[Serializable]
public class EffectData
{
    public string description;
    public string type;   // "gain-mana","gain-spellpower"…
    public string amount; // 数值或表达式
    public string until;  // 可选，如 "move","cast-spell"
}

[Serializable]
public class RelicData
{
    public string name;
    public int sprite;
    public TriggerData trigger;
    public EffectData effect;
}

[Serializable]
public class RelicsContainer
{
    public RelicData[] relics;
}
