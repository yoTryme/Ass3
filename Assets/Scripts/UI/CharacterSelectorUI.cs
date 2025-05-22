using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Newtonsoft.Json;

public class CharacterSelectorUI : MonoBehaviour
{
    public SpriteView spriteView1;
    public SpriteView spriteView2;
    public SpriteView spriteView3;

    public TextMeshProUGUI ChImfomation1;
    public TextMeshProUGUI ChImfomation2;
    public TextMeshProUGUI ChImfomation3;
    public PlayerController player;

    public GameObject difficultSelector;
    void Start()
    {
        // Load classes.json from Resources
        TextAsset jsonFile = Resources.Load<TextAsset>("classes");
        //classData = JsonConvert.DeserializeObject<Dictionary<string, CharacterClass>>(jsonFile.text);

        // Apply sprites
        spriteView1.Apply("mage", GameManager.Instance.playerSpriteManager.Get(0));
        spriteView2.Apply("warlock", GameManager.Instance.playerSpriteManager.Get(1));
        spriteView3.Apply("battlemage", GameManager.Instance.playerSpriteManager.Get(2));

        DisplayClassInfo();
    }

    void DisplayClassInfo()
    {
        ChImfomation1.text = FormatClassInfo("mage");
        ChImfomation2.text = FormatClassInfo("warlock");
        ChImfomation3.text = FormatClassInfo("battlemage");
    }

    string FormatClassInfo(string className)
    {
        CharacterStats stats = ClassManager.Instance.GetClassStats(className, 0); // wave 0 for preview

        string description = GetClassDescription(className);

        return $"{description}\n" +
               $"❤️ Health: {stats.health}\n" +
               $"🔮 Mana: {stats.mana}\n" +
               $"💧 Mana Regen: {stats.manaRegeneration}\n" +
               $"🔥 Spell Power: {stats.spellpower}\n" +
               $"⚡ Speed: {stats.speed}";
    }

    string GetClassDescription(string className)
    {
        switch (className)
        {
            case "mage":
                return " *Mage*";
            case "warlock":
                return " *Warlock*";
            case "battlemage":
                return " *Battlemage*";
            default:
                return "Unknown Class";
        }
    }    public void ChooseCharacter(string className)
    {
        player.ReadDeclareCharacter(className);

        // Hide UI or go to difficulty selection
        gameObject.SetActive(false);
        difficultSelector.SetActive(true);

    }
}
