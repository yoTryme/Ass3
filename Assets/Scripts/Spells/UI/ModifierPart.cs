using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ModifierPart : MonoBehaviour
{
    public Image spellImg;

    public TMP_Text spellNameText;
    
    public TMP_Text spellDescriptionText;
    
    public Button acceptButton;

    public string modifierKey;
    
    public SpellModifier modifierData;


    public SpellBuilder spellBuilder;
    
    public bool isAccepted = false;
    private void Awake()
    {
        acceptButton.onClick.AddListener(OnAccept);
        
        EventCenter.AddListener(EventDefine.ShowModifierPart,ShowModifierPart);
        EventCenter.AddListener(EventDefine.CloseModifierPart,CloseModifierPart);
        
    }

    private void OnAccept()
    {
        if (!isAccepted)
        {
            if (GameManager.Instance.currentSpellKey != "")
            {
                FindFirstObjectByType<PlayerModifierController>().ApplyModifier(GameManager.Instance.currentSpellKey, modifierKey);
                
                isAccepted = true;
            }
            else
            {
                Debug.Log("choose base spell ");
            }
          
        }
        
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        gameObject.SetActive(false);
    }

    void ShowModifierPart()
    {
        gameObject.SetActive(true);
        isAccepted = false;
        Dictionary<string, SpellModifier> randomModifier = spellBuilder.GetModifier();
        
        var modifierKeys = new List<string>(randomModifier.Keys);
        modifierKey = modifierKeys[0];
        
        modifierData = randomModifier[modifierKeys[0]];
        
        spellImg.sprite = SpellManager.Instance.projectileSprites[0];
        
        spellNameText.text = modifierData.name;
        
        spellDescriptionText.text = modifierData.description;
    }

    void CloseModifierPart()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowModifierPart,ShowModifierPart);
        EventCenter.RemoveListener(EventDefine.CloseModifierPart,CloseModifierPart);

    }
}
