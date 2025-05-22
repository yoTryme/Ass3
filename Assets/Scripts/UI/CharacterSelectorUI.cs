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

    private Dictionary<string, CharacterClass> classData;
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

        //DisplayClassInfo();
    }

    void DisplayClassInfo()
    {
        ChImfomation1.text = FormatClassInfo("mage");
        ChImfomation2.text = FormatClassInfo("warlock");
        ChImfomation3.text = FormatClassInfo("battlemage");
    }

    string FormatClassInfo(string className)
    {
        if (!classData.ContainsKey(className)) return "No data";
        CharacterClass c = classData[className];

        return $"Class: {className}\n" +
               $"Health: {c.health}\n" +
               $"Mana: {c.mana}\n" +
               $"Spell Power: {c.spellpower}\n" +
               $"Speed: {c.speed}";
    }

    public void ChooseCharacter(string className)
    {
        player.ReadDeclareCharacter(className);

        // Hide UI or go to difficulty selection
        gameObject.SetActive(false);
        difficultSelector.SetActive(true);

    }
}
