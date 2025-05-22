using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterClassData
{
    public string health;
    public string mana;
    public string spellpower;
    public string speed;
}

public class CharacterClassManager : MonoBehaviour
{
    public static Dictionary<string, CharacterClassData> Classes;

    void Awake()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("classes");
        Classes = JsonUtility.FromJson<Wrapper>(jsonFile.text).ToDictionary();
    }

    [System.Serializable]
    private class Wrapper
    {
        public CharacterClassData mage;
        public CharacterClassData warlock;
        public CharacterClassData battlemage;

        public Dictionary<string, CharacterClassData> ToDictionary()
        {
            var dict = new Dictionary<string, CharacterClassData>
            {
                { "mage", mage },
                { "warlock", warlock },
                { "battlemage", battlemage }
            };
            return dict;
        }
    }
}
