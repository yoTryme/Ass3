// using UnityEngine;
// using UnityEngine.InputSystem;
// using Newtonsoft.Json.Linq;
// using Newtonsoft.Json;
// using System.IO;
// using System.Collections.Generic;

// public class PlayerController : MonoBehaviour
// {
//     public Hittable hp;
//     public HealthBar healthui;
//     public ManaBar manaui;

//     public SpellCaster spellcaster;
//     public SpellUI spellui;

//     public int speed;

//     public Unit unit;

//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         unit = GetComponent<Unit>();
//         GameManager.Instance.player = gameObject;
//     }

//     public void StartLevel()
//     {
//         spellcaster = new SpellCaster(125, 8, Hittable.Team.PLAYER);
//         StartCoroutine(spellcaster.ManaRegeneration());

//         hp = new Hittable(100, Hittable.Team.PLAYER, gameObject);
//         hp.OnDeath += Die;
//         hp.team = Hittable.Team.PLAYER;

//         // tell UI elements what to show
//         healthui.SetHealth(hp);
//         manaui.SetSpellCaster(spellcaster);
//         spellui.SetSpell(spellcaster.spell);
//         GetComponent<PlayerDeathHandler>().InitHP(100);   // or whatever max HP

//     }

//     // Update is called once per frame
//     void Update()
//     {

//     }

//     void OnAttack(InputValue value)
//     {
//         if (GameManager.Instance.state == GameManager.GameState.PREGAME || GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;
//         Vector2 mouseScreen = Mouse.current.position.value;
//         Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
//         mouseWorld.z = 0;
//         StartCoroutine(spellcaster.Cast(transform.position, mouseWorld));
//     }

//     void OnMove(InputValue value)
//     {
//         if (GameManager.Instance.state == GameManager.GameState.PREGAME || GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;
//         unit.movement = value.Get<Vector2>()*speed;
//     }

//     void Die()
//     {
//         Debug.Log("You Lost");
//     }

// }


// PlayerController.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Hittable hp;
    public HealthBar healthui;
    public ManaBar manaui;
    public SpellUI spellui;
    public int speed;
    public Unit unit;
    public CharacterStats characterStats;
    private PlayerSpellController spellController;
    private RelicManager relicManager;
    public PlayerTile playerTile;
    // 用于检测玩家是否移动或静止
    private Vector3 lastPosition;
    private bool wasMovingLastFrame;
    private float standStillStartTime;

    void Start()
    {
        unit = GetComponent<Unit>();
        spellController = GetComponent<PlayerSpellController>();
        GameManager.Instance.player = gameObject;
        //playerTile = new PlayerTile();
        // 拿到场景中的 RelicManager 实例
        relicManager = FindFirstObjectByType<RelicManager>();

        // 初始化移动检测状态
        lastPosition = transform.position;
        wasMovingLastFrame = true;         // 假设玩家一开始是“移动”状态
        standStillStartTime = Time.time;

        // 订阅事件
        EventBus.Instance.OnMove += relicManager.HandleMove;
        EventBus.Instance.OnStandStill += relicManager.HandleStandStill;
    }
    public void ReadDeclareCharacter(string index)
    {
        characterStats = ClassManager.Instance.GetClassStats(index, GameManager.Instance.currentWave);
        spellController.health = characterStats.health;
        spellController.mana = characterStats.mana;
        spellController.power = (int)characterStats.spellpower;
        spellController.manaRegeneration = characterStats.manaRegeneration;
        speed = (int)characterStats.speed;
        
        playerTile.SetClassSprite(characterStats.sprite);


    }
    void Update()
    {
        if (GameManager.Instance.state != GameManager.GameState.INWAVE) return;


        Vector3 currentPos = transform.position;
        bool isMoving = Vector3.Distance(currentPos, lastPosition) > 0.01f;

        // 刚开始移动 => 触发 Move
        if (isMoving && !wasMovingLastFrame)
        {
            EventBus.Instance.TriggerMove();
        }
        // 刚开始静止 => 记录静止开始时间
        if (!isMoving && wasMovingLastFrame)
        {
            standStillStartTime = Time.time;
        }
        // 持续静止超过 3 秒 => 触发 StandStill
        if (!isMoving && !wasMovingLastFrame && Time.time - standStillStartTime > 3f)
        {
            EventBus.Instance.TriggerStandStill();
        }

        wasMovingLastFrame = isMoving;
        lastPosition = currentPos;
    }

    public void NextLevel()
    {
        hp.SetMaxHP((int)spellController.health);
    }

    public void StartLevel()
    {
        speed = (int)spellController.speed;

        // 取到挂在玩家物体上的 Hittable 组件（Inspector 上要挂这个脚本）
        hp = GetComponent<Hittable>();
        // 初始化属性
        hp.max_hp = (int)spellController.health;
        hp.hp = hp.max_hp;
        hp.team = Hittable.Team.PLAYER;
        hp.owner = gameObject;
        hp.OnDeath += Die;

        healthui.SetHealth(hp);

        // 开始法力恢复协程
        StartCoroutine(spellController.ManaRegeneration());
        manaui.SetManaBar(spellController);

        // Hook 死亡处理
        GetComponent<PlayerDeathHandler>().InitHP(hp);
    }

    void OnAttack(InputValue value)
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME ||
            GameManager.Instance.state == GameManager.GameState.GAMEOVER)
            return;

        // 如果你在这里要调用施法：
        // spellController.TryCastSpell(GameManager.Instance.currentSpellKey);
    }

    void OnMove(InputValue value)
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME ||
            GameManager.Instance.state == GameManager.GameState.GAMEOVER)
            return;

        unit.movement = value.Get<Vector2>() * speed;
    }

    void Die()
    {
        Debug.Log("You Lost");
        unit.movement = Vector2.zero;
        enabled = false;
    }

    void OnDestroy()
    {
        // 记得退订，防止场景切换后仍然调用
        if (EventBus.Instance != null && relicManager != null)
        {
            EventBus.Instance.OnMove -= relicManager.HandleMove;
            EventBus.Instance.OnStandStill -= relicManager.HandleStandStill;
        }
    }
}