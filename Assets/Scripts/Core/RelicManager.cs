using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Object = UnityEngine.Object;


public class RelicManager : MonoBehaviour
{
    public static RelicManager Instance { get; private set; }
    private List<RelicData> relicConfigs;
    private List<RelicData> activeRelics;
    private PlayerSpellController psc;

    void Awake()
    {
        Instance = this;
        psc = Object.FindFirstObjectByType<PlayerSpellController>();
        psc.mana = 0;
        GameManager.Instance.currentWave = 1;
        // ���� relics.json
        var txt = Resources.Load<TextAsset>("relics");
        relicConfigs = JsonUtility
            .FromJson<RelicsContainer>("{\"relics\":" + txt.text + "}")
            .relics.ToList();
        activeRelics = new List<RelicData>();

        // ��ʱ�������
        ActivateRelic("Golden Mask");
        ActivateRelic("Green Gem");
        ActivateRelic("Cursed Scroll");
        ActivateRelic("Jade Elephant");
        Debug.Log("Active relics: " + string.Join(", ", activeRelics.Select(r => r.name)));
        // �����¼�
        EventBus.Instance.OnDamage += HandleTakeDamage;
        EventBus.Instance.OnKill += HandleKill;
        EventBus.Instance.OnMove += HandleMove;
        EventBus.Instance.OnStandStill += HandleStandStill;
    }

    public void ActivateRelic(string name)
    {
        var r = relicConfigs.FirstOrDefault(x => x.name == name);
        if (r != null) activeRelics.Add(r);
    }

    void HandleTakeDamage(Vector3 p, Damage d, Hittable t)
    {
        Debug.Log("[RelicManager] HandleTakeDamage called");

        foreach (var r in activeRelics.Where(x => x.trigger.type == "take-damage"))
        {
            ApplyEffect(r);
        }
    }

    public void HandleKill()
    {
        Debug.Log("[RelicManager] HandleKill called");

        foreach (var r in activeRelics.Where(x => x.trigger.type == "on-kill"))
        {
            Debug.Log($"[RelicManager] Trigger match for relic: {r.name}");
            ApplyEffect(r);
        }

    }

    public void HandleStandStill()
    {
        Debug.Log("[RelicManager] HandleStandStill called");

        foreach (var r in activeRelics.Where(x => x.trigger.type == "stand-still"))
        {
            // �Ȱ� amount �ַ����з�
            var parts = r.effect.amount.Split(' ');
            // parts[0] Ӧ���ǻ���ֵ "10"
            int baseAmt = int.Parse(parts[0]);
            // ������� parts ���� "wave" ��һ�����֣���ȡ����
            int waveBonus = 0;
            var idx = System.Array.IndexOf(parts, "wave");
            if (idx >= 0 && idx + 1 < parts.Length)
                waveBonus = GameManager.Instance.currentWave * int.Parse(parts[idx + 1]);
            int total = baseAmt + waveBonus;
            // ���� �������ӡ������������� ���� 
            Debug.Log($"[RelicManager] StandStill �� relic={r.name}, base={baseAmt}, waveBonus={waveBonus}, total={total}");

            psc.AddTempSpell(baseAmt + waveBonus);
        }
    }

    public void HandleMove()
    {
        Debug.Log("[RelicManager] HandleMove called");

        psc.ClearTempSpell();
    }

    void ApplyEffect(RelicData r)
    {
        if (r.effect.type == "gain-mana")
        {
            int amt = int.Parse(r.effect.amount);
            psc.AddMana(amt);
            Debug.Log($"[Relic] {r.name} granted {amt} mana");
        }
        else if (r.effect.type == "gain-spellpower")
        {
            int amt = int.Parse(r.effect.amount);
            // ����� Golden Mask����Ҫ�´�ʩ���ӳ�
            if (r.effect.until == "cast-spell")
            {
                psc.nextSpellBonus += amt;
                Debug.Log($"[Relic] {r.name}: next spell +{amt} spellpower");
            }
            else
            {
                // Ĭ����ʱ�ӳɣ�Jade Elephant��
                psc.AddTempSpell(amt);
                Debug.Log($"[Relic] {r.name} granted temporary +{amt} spellpower");
            }
        }
    }

}
