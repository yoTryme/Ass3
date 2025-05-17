using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SpellPart : MonoBehaviour
{
    public Image spellImg;

    public TMP_Text spellNameText;
    
    public TMP_Text spellDescriptionText;
    
    public Button acceptButton;

    public GameObject tipObj;

    public string spellKey;
    
    public SpellData spellData;


    public SpellBuilder spellBuilder;
    
    public bool isAccepted = false;
    private void Awake()
    {
        acceptButton.onClick.AddListener(OnAccept);
        
        EventCenter.AddListener(EventDefine.ShowSpellPart,ShowSpellPart);
        EventCenter.AddListener(EventDefine.CloseSpellPart,CloseSpellPart);
        
    }

    private void OnAccept()
    {
        if (!isAccepted)
        {
            if (spellBuilder.CanAddNewSpell())
            {
            
                spellBuilder.AddNewSpellBySpell(spellData,spellKey);
                isAccepted = true;
                tipObj.SetActive(false);
            }
            else
            {
                tipObj.SetActive(true);
                Debug.Log("Can't add new spell");
            }
           
        }
        
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        gameObject.SetActive(false);
    }

    void ShowSpellPart()
    {
        tipObj.SetActive(false);
        gameObject.SetActive(true);
        isAccepted = false;
        Dictionary<string, SpellData> randomSpell = spellBuilder.GetNewSpell();
        
        var spellKeys = new List<string>(randomSpell.Keys);
        spellKey = spellKeys[0];
        
        spellData = randomSpell[spellKeys[0]];
        
        spellImg.sprite = SpellManager.Instance.projectileSprites[0];
        
        spellNameText.text = spellData.name;
        
        spellDescriptionText.text = spellData.description;
    }

    void CloseSpellPart()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowSpellPart,ShowSpellPart);
        EventCenter.RemoveListener(EventDefine.CloseSpellPart,CloseSpellPart);

    }
}
