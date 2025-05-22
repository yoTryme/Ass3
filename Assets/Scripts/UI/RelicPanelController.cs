using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelicPanelController : MonoBehaviour
{
    [Header("Prefab & 父容器")]
    [SerializeField] GameObject relicViewPrefab;  // 拖入 Prefab Assets/Prefabs/UI/RelicView.prefab
    [SerializeField] Transform container;        // 拖入 RelicPanel 下的空 GameObject (如叫 RelicContainer)

    [Header("图标集 & 选几条")]
    [SerializeField] Sprite[] sprites;          // 数组长度 ≥ JSON 最大 sprite 索引+1
    [SerializeField] int choiceCount = 3;  // 随机三选一就留 3

    List<RelicInfo> allRelics;
    List<RelicInfo> pick3;

    void Awake()
    {
        Debug.Log("RelicPanelController Awake");

        // 加载 relics.json（放在 Assets/Resources/relics.json）
        var txt = Resources.Load<TextAsset>("relics");
        var wrapper = JsonUtility.FromJson<RelicInfoContainer>(
            "{\"relics\":" + txt.text + "}"
        );
        allRelics = wrapper.relics;

        // 默认隐藏面板
        gameObject.SetActive(false);

        // 监听法术面板打开/关闭
        EventCenter.AddListener(EventDefine.ShowSpellPart, ShowChoices);
        EventCenter.AddListener(EventDefine.CloseSpellPart, HidePanel);
    }

    void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowSpellPart, ShowChoices);
        EventCenter.RemoveListener(EventDefine.CloseSpellPart, HidePanel);
    }

    void ShowChoices()
    {
        Debug.Log("RelicPanelController ShowChoices");

        // 1) 随机取未拥有的三条
        pick3 = allRelics
            .Where(r => !RelicManager.Instance.HasRelic(r.name))
            .OrderBy(_ => UnityEngine.Random.value)
            .Take(choiceCount)
            .ToList();

        // 2) 清空旧的槽
        Debug.Log($"清空前 container.childCount={container.childCount}");

        foreach (Transform child in container)
            Destroy(child.gameObject);
        Debug.Log($"清空后 container.childCount={container.childCount}");


        // 3) 克隆并初始化
        foreach (var info in pick3)
        {
            var go = Instantiate(relicViewPrefab, container);
            Debug.Log("克隆了一个 RelicView 实例 => " + info.name);

            // 找到子组件
            var icon = go.transform.Find("Icon").GetComponent<Image>();
            var desc = go.transform.Find("DescText").GetComponent<TMP_Text>();
            var btn = go.GetComponent<Button>()
                       ?? go.transform.Find("Button").GetComponent<Button>();
            var name = btn.transform.Find("NameText").GetComponent<TMP_Text>();

            // 填数据
            icon.sprite = sprites[info.sprite];
            desc.text = info.effect.description;
            name.text = info.name;

            // 按钮回调
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnPick(info));
        }

        // 4) 显示 & 暂停
        Debug.Log($"克隆后 container.childCount={container.childCount}");

        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    void OnPick(RelicInfo chosen)
    {
        // 激活
        RelicManager.Instance.ActivateRelic(chosen.name);

        // 关闭自己 & 关闭法术面板
        HidePanel();
        EventCenter.Broadcast(EventDefine.CloseSpellPart);

        // 继续下一波
        var sp = UnityEngine.Object.FindFirstObjectByType<EnemySpawner>();
        if (sp != null) sp.NextWave();
    }

    void HidePanel()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    // —— 内部数据结构 —— 
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
