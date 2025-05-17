using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class SpellUIItem : MonoBehaviour
{
    public Image highLightImg;

    public Image spellIconImg;
    
    public TMP_Text manaCostText;
    
    public TMP_Text damageText;

    public Button spellButton;
    
    public Button unloadButton;

    public SpellData currentSpellData;
    public string currentSpellKey;
    public int index;

  
    private void Awake()
    {
        spellButton.onClick.AddListener(OnSpellButtonClick);
        unloadButton.onClick.AddListener(OnUnloadButtonClick);
    }

    private void OnUnloadButtonClick()
    {
        transform.parent.GetComponent<SpellBuilder>().RemoveSelectedSpell(currentSpellData);
        // ResetSpellUI();
        Destroy(gameObject);
    }

    private void OnSpellButtonClick()
    {
        GameManager.Instance.currentSpellKey = currentSpellKey;
        transform.parent.GetComponent<SpellBuilder>().CloseHightLight();
        highLightImg.gameObject.SetActive(true);
    }

    public void SetSpellUI(int uiIndex ,SpellData spellData,string  spellKey)
    {
        index = uiIndex;
        currentSpellData = spellData;
        currentSpellKey = spellKey;
        manaCostText.text = RpnCalculator.Calculate(spellData.mana_cost, FindFirstObjectByType<PlayerSpellController>().power).ToString();
        damageText.text =  Mathf.RoundToInt(RpnCalculator.Calculate(spellData.spell_damage.amount, FindFirstObjectByType<PlayerSpellController>().power)).ToString();
        spellIconImg.sprite = SpellManager.Instance.projectileSprites[0];
    }


    public void ResetSpellUI()
    {
        index = 0;
        highLightImg.gameObject.SetActive(false);
        currentSpellData = null;
        currentSpellKey = null;
        manaCostText.text = "";
        damageText.text ="";
        spellIconImg.sprite = null;

        transform.parent.GetComponent<SpellBuilder>().activeSpells.Remove(currentSpellData);
    }

    public void CloseHightLight()
    {
        highLightImg.gameObject.SetActive(false);
    }
}