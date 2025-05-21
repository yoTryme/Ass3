using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpellController : MonoBehaviour
{


    [Header("属性")]
    public float health = 100;
    public int power = 50; // 当前法术强度
    public float mana = 100f; // 当前法力值
    public float manaRegeneration = 15;
    public float speed = 5f;
    public float max_mana = 100;

    private PlayerModifierController modifierController;

    private float currentCooldown;

    private SpellData currentSpell;
    // 新增字段：标识是否为自动施法
    private bool isAutoCast = false;
    // —— 新增：Golden Mask 下次施法加成缓存
    public int nextSpellBonus = 0;

    // —— 新增：Jade Elephant 临时法术强度增益
    private int tempSpellBonus = 0;

    // —— 新增：被 Cursed Scroll、其他系统调用，直接补充法力
    public void AddMana(int amount)
    {
        mana += amount;
        if (mana > max_mana) mana = max_mana;
    }

    // —— 新增：Jade Elephant 增益调用
    public void AddTempSpell(int amount)
    {
        tempSpellBonus += amount;
    }

    // —— 新增：在移动时或其他时机清除静止增益
    public void ClearTempSpell()
    {
        tempSpellBonus = 0;
    }
    // HealEffect 调用
    public void Heal(int amount)
    {
        health += amount;
        // 如果你有最大生命上限，可以在这里 clamp：
        // health = Mathf.Min(health, maxHealth);
    }

    // Swift Stone 调用，立即减少当前冷却
    public void ReduceCooldown(float seconds)
    {
        currentCooldown = Mathf.Max(0, currentCooldown - seconds);
    }

    // Rage Emblem 调用，为下次施法增强伤害
    public void BoostNextSpellDamage(float percent)
    {
        // percent 比如 0.2f 表示 +20%
        // 假设你的 power 是法术基础强度：
        int bonus = Mathf.RoundToInt(power * percent);
        nextSpellBonus += bonus;
        Debug.Log($"[PlayerSpellController] BoostNextSpellDamage applied, bonus={bonus}");
    }

    void Start()
    {
        modifierController = GetComponent<PlayerModifierController>();
    }
    string spellId = "arcane_bolt";
    void Update()
    {
        currentCooldown -= Time.deltaTime;

        // 改为鼠标左键点击触发
        if (Input.GetMouseButtonDown(0))
        {
            TryCastSpell(GameManager.Instance.currentSpellKey);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            spellId = "magic_missile";
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            spellId = "arcane_blast";
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            spellId = "arcane_spray";
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            spellId = "arcane_large";
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
         //   string spellId = "arcane_bolt";
            string modifierId = "splitter";
            GetComponent<PlayerModifierController>().ApplyModifier(spellId, modifierId);
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            //   string spellId = "arcane_bolt";
            string modifierId = "doubler";
            GetComponent<PlayerModifierController>().ApplyModifier(spellId, modifierId);
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            //   string spellId = "arcane_bolt";parabola
            string modifierId = "chaos";
            GetComponent<PlayerModifierController>().ApplyModifier(spellId, modifierId);
        }
        
        if (Input.GetKeyDown(KeyCode.J))
        {
            //   string spellId = "arcane_bolt";parabola
            string modifierId = "parabola";
            GetComponent<PlayerModifierController>().ApplyModifier(spellId, modifierId);
        }
    }

    public void SetCharacter(CharacterStats mageStats)
    {
        health = mageStats.health;
        mana = mageStats.mana;
        manaRegeneration = mageStats.manaRegeneration;
        power = (int)mageStats.spellpower;
        speed = mageStats.speed;
        max_mana = mageStats.mana;
    }
    
    public IEnumerator ManaRegeneration()
    {
        while (true)
        {
            mana += manaRegeneration;
            mana = Mathf.Min(mana, max_mana);
            yield return new WaitForSeconds(1);
        }
    }

    public bool TryCastSpell(string spellId)
    {
        // 1) 取到经过 Modifier 修正后的法术数据
        SpellData modifiedSpell = modifierController.GetModifiedSpell(spellId, power);
        if (modifiedSpell == null) return false;
        currentSpell = modifiedSpell;

        // 2) 计算施法方向
        Vector2 dir = GetMouseDirection();

        // 3) 计算实际威力（基础 + Golden Mask 加成 + Jade Elephant 加成）
        int basePower = power;
        int actualPower = basePower;
        if (nextSpellBonus != 0)
        {
            actualPower += nextSpellBonus;
            Debug.Log($"[PlayerSpellController] Applied Golden Mask bonus, power={actualPower}");
            nextSpellBonus = 0;
        }
        actualPower += tempSpellBonus;

        // 4) 计算消耗和冷却
        float manaCost = RpnCalculator.Calculate(modifiedSpell.mana_cost, basePower);
        float cooldown = modifiedSpell.cooldown;
        if (currentCooldown > 0 || mana < manaCost)
            return false;
        mana -= manaCost;
        currentCooldown = cooldown;

        // 5) 处理各种分支
        if (spellId == "arcane_spray")
        {
            // 锥形弹幕
            int count = Mathf.RoundToInt(RpnCalculator.Calculate(modifiedSpell.N, basePower));
            float startAngle = -45f;
            for (int i = 0; i < count; i++)
            {
                float angle = startAngle + (90f / (count - 1)) * i;
                Vector2 sprayDir = Quaternion.Euler(0, 0, angle) * dir;
                CreateProjectile(modifiedSpell.projectile, sprayDir, modifiedSpell);
            }
        }
        else
        {
            // splitter 修饰符
            if (modifierController.HasModifier(spellId, "split"))
            {
                float splitAngle = modifierController
                    .GetModifierValue<float>(spellId, "split", "angle", 10f);
                Vector2 leftDir = Quaternion.Euler(0, 0, splitAngle) * dir;
                Vector2 rightDir = Quaternion.Euler(0, 0, -splitAngle) * dir;
                CreateProjectile(modifiedSpell.projectile, leftDir, modifiedSpell);
                CreateProjectile(modifiedSpell.projectile, rightDir, modifiedSpell);
            }
            // 不走 splitter，就走普通或 doubler
            else
            {
                CreateProjectile(modifiedSpell.projectile, dir, modifiedSpell);
            }
        }

        return true;
    }


    // public bool TryCastSpell(string spellId)
    // {
    //     // 获取修正后的法术数据
    //     SpellData modifiedSpell = modifierController.GetModifiedSpell(spellId, power);
    //     currentSpell = modifiedSpell;
    //     // 计算实际消耗和冷却
    //     float finalManaCost = RpnCalculator.Calculate(modifiedSpell.mana_cost, power);
    //     float finalCooldown = modifiedSpell.cooldown;
    //
    //     if (currentCooldown > 0 || mana < finalManaCost) return false;
    //
    //     // 生成投射物
    //     Vector2 dir = GetMouseDirection();
    //     CreateProjectile(modifiedSpell.projectile, dir, modifiedSpell);
    //
    //     Debug.Log("modifiedSpell" + modifiedSpell.projectile.speed);
    //
    //     // 应用消耗
    //     mana -= finalManaCost;
    //     currentCooldown = finalCooldown;
    //     
    //     return true;
    // }

    // 新增协程：延迟施法
    private IEnumerator DelayedCast(string spellId, float delay)
    {
        yield return new WaitForSeconds(delay);
        isAutoCast = true;
        TryCastSpell(spellId);
        isAutoCast = false;
    }

    void CreateProjectile(ProjectileData config, Vector2 dir, SpellData spell)
    {
        // 从修正后的数据中获取实际速度
        float actualSpeed = config.speed;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        GameObject projectile = SpellManager.Instance.CreateProjectile(
            config,
            transform.position,
            CalculateRotation(dir)
            // Quaternion.AngleAxis(angle, Vector3.forward)
        );
        // —— 新增：通知“cast-spell” —— 
        EventBus.Instance.TriggerOnCastSpell();



        Debug.Log("config.trajectory" + config.trajectory);
        if (spell.name == "Arcane Blast")
        {
            var controller = projectile.GetComponent<ProjectileController>();
            controller.Initialize(config, dir, currentSpell,power); // 传递法术数据
            // 订阅碰撞事件
            controller.OnProjectileCollision += HandleMainProjectileHit;
            return;
        }

        projectile.GetComponent<ProjectileController>()
            .Initialize(config, dir, config.trajectory,currentSpell,power);


     
    }

    private Quaternion CalculateRotation(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // public bool TryCastSpell(string spellId)
    // {
    //     currentSpell = SpellManager.Instance.GetSpell(spellId);
    //     if (currentSpell == null) return false;
    //
    //     if (currentCooldown > 0 || !CheckManaCost()) return false;
    //
    //     // 获取鼠标方向
    //     Vector2 direction = GetMouseDirection();
    //
    //     // 根据方向计算旋转角度（使投射物朝向移动方向）
    //     float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    //     Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    //
    //     // 生成主投射物
    //     GameObject mainProjectile = SpellManager.Instance.CreateProjectile(
    //         currentSpell.projectile,
    //         transform.position,
    //         rotation // 使用计算后的旋转
    //     );
    //
    //     // 初始化投射物方向
    //     // 获取控制器并初始化
    //     var controller = mainProjectile.GetComponent<ProjectileController>();
    //     controller.Initialize(currentSpell.projectile, direction, currentSpell); // 传递法术数据
    //
    //     
    //     // 订阅碰撞事件
    //     controller.OnProjectileCollision += HandleMainProjectileHit;
    //     
    //     currentCooldown = currentSpell.cooldown;
    //     return true;
    // }
    //
    // /// <summary>
    // /// 主投射物碰撞事件处理
    // /// </summary>
    private void HandleMainProjectileHit(Vector2 hitPosition, SpellData spellData)
    {
        // 计算次级投射物数量
        int secondaryCount = Mathf.RoundToInt(RpnCalculator.Calculate(spellData.N, power));

        // 生成次级投射物
        for (int i = 0; i < secondaryCount; i++)
        {
            // 计算扩散角度（例如：均匀分布在180度范围内）
            float angleOffset = Mathf.Lerp(-90f, 90f, (float)i / (secondaryCount - 1));
            Vector2 secondaryDir = Quaternion.Euler(0, 0, angleOffset) * Vector2.right;

            // 创建次级投射物
            GameObject secondaryProjectile = SpellManager.Instance.CreateProjectile(
                spellData.secondary_projectile,
                hitPosition,
                Quaternion.identity
            );

            // 初始化运动
            var secondaryController = secondaryProjectile.GetComponent<ProjectileController>();
            secondaryController.Initialize(
                spellData.secondary_projectile,
                secondaryDir,
                spellData,
                power,
                true
            );
        }
    }

    private Vector2 GetMouseDirection()
    {
        // // 获取鼠标在屏幕上的位置
        // Vector3 mouseScreenPos = Input.mousePosition;
        // // 转换为世界坐标（假设使用正交相机，z轴为0）
        // Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
        //     new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10)
        // );
        // mouseWorldPos.z = transform.position.z; // 保持与玩家相同z轴
        //
        Vector2 mouseScreen = Mouse.current.position.value;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;
        // 计算方向向量并归一化
        Vector2 direction = (mouseWorld - transform.position).normalized;
        return direction;
    }

    bool CheckManaCost()
    {
        float cost = RpnCalculator.Calculate(currentSpell.mana_cost, power);
        if (mana >= cost)
        {
            mana -= cost;
            return true;
        }

        return false;
    }

    // // 由主投射物碰撞时调用
    // public void OnMainProjectileHit(Vector2 position)
    // {
    //     // 生成次级投射物
    //     int count = Mathf.RoundToInt(RpnCalculator.Calculate(currentSpell.N, power));
    //     
    //     for (int i = 0; i < count; i++)
    //     {
    //         GameObject secondaryProjectile = SpellManager.Instance.CreateProjectile(
    //             currentSpell.secondary_projectile,
    //             position,
    //             Quaternion.Euler(0, 0, i * 360f / count)
    //         );
    //
    //         secondaryProjectile.GetComponent<ProjectileController>()
    //             .Initialize(currentSpell.secondary_projectile, secondaryProjectile.transform.right);
    //     }
    // }
}