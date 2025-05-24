using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelicPanelController : MonoBehaviour
{
    [Header("Prefab & Container")]
    [SerializeField] GameObject relicViewPrefab; // Prefab for relic display
    [SerializeField] Transform container;        // Parent for relic entries

    [Header("Sprites & Choices")]
    [SerializeField] Sprite[] sprites;           // Relic icon sprites
    [SerializeField] int choiceCount = 3;        // Number of relics to choose from

    List<RelicInfo> allRelics;
    List<RelicInfo> pick3;

    void Awake()
    {
        Debug.Log("✅ RelicPanelController Awake");

        // Load relics from JSON
        var txt = Resources.Load<TextAsset>("relics");
        var wrapper = JsonUtility.FromJson<RelicInfoContainer>("{\"relics\":" + txt.text + "}");
        allRelics = wrapper.relics;

        gameObject.SetActive(false);

        // Listen for new, dedicated events
        EventCenter.AddListener(EventDefine.ShowRelicPanel, ShowChoices);
        EventCenter.AddListener(EventDefine.CloseRelicPanel, HidePanel);
    }

    void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowRelicPanel, ShowChoices);
        EventCenter.RemoveListener(EventDefine.CloseRelicPanel, HidePanel);
    }

    void ShowChoices()
    {
        Debug.Log("✅ ShowChoices called");

        // Get 3 random relics that the player doesn’t already have
        pick3 = allRelics
            .Where(r => !RelicManager.Instance.HasRelic(r.name))
            .OrderBy(_ => UnityEngine.Random.value)
            .Take(choiceCount)
            .ToList();

        // Clear old entries
        foreach (Transform child in container)
            Destroy(child.gameObject);

        // Instantiate relic choices
        foreach (var info in pick3)
        {
            var go = Instantiate(relicViewPrefab, container);
            Debug.Log("➡️ Created Relic: " + info.name);

            var icon = go.transform.Find("Icon").GetComponent<Image>();
            var desc = go.transform.Find("DescText").GetComponent<TMP_Text>();
            var btn = go.GetComponent<Button>() ?? go.transform.Find("Button").GetComponent<Button>();
            var name = btn.transform.Find("NameText").GetComponent<TMP_Text>();

            icon.sprite = sprites[info.sprite];
            desc.text = info.effect.description;
            name.text = info.name;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnPick(info));
        }

        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    void OnPick(RelicInfo chosen)
    {
        RelicManager.Instance.ActivateRelic(chosen.name);

        HidePanel();
        EventCenter.Broadcast(EventDefine.CloseRelicPanel);

        // Continue to next wave
        var spawner = FindFirstObjectByType<EnemySpawner>();
        if (spawner != null)
            spawner.NextWave();
    }

    void HidePanel()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    [Serializable]
    class RelicInfoContainer { public List<RelicInfo> relics; }

    [Serializable]
    public class RelicInfo
    {
        public string name;
        public int sprite;
        public Trigger trigger;
        public Effect effect;
    }

    [Serializable] public class Trigger { public string description, type, amount; }
    [Serializable] public class Effect { public string description, type, amount, until; }
}
