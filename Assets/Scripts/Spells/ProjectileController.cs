using System.Collections;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    
    // 弹道类型参数
    private float spiralAngle = 0;
    private Transform target;
    
    
    // 定义带参数的委托
    public delegate void ProjectileHitHandler(Vector2 hitPosition, SpellData spellData);
    public event ProjectileHitHandler OnProjectileCollision;
    // 携带的法术数据
    private SpellData linkedSpellData;
    private int currentPower;
    private bool isSecondSpell;
    
    private float speed;
    private Vector2 direction;
    
    
    private float gravity = -15f;   // 重力加速度（可配置）
    private float launchAngle = 45f; // 发射仰角（单位：度）

    
    // private float speed;
    private float lifetime;
    // private Vector2 direction;

    public void Initialize(ProjectileData config, Vector2 dir,SpellData spell,int power,bool isSecond = false)
    {
        linkedSpellData = spell; // 存储关联的法术配置
        speed = config.speed;
        lifetime = config.lifetime;
        direction = dir.normalized;
    
        currentPower = power;
        isSecondSpell = isSecond;
        // 初始旋转
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (lifetime > 0)
        {
            Destroy(gameObject, lifetime);
        }
        else
        {
            Destroy(gameObject, 10.0f);
        }
        
        switch (config.trajectory)
        {
            case "spiraling":
                StartCoroutine(SpiralMovement(config.speed));
                break;
            case "homing":
                target = FindNearestEnemy();
                StartCoroutine(HomingMovement(config.speed));
                break;
            case "drift":
                StartCoroutine(DriftMovement(dir,config.speed));
                StartCoroutine(DriftLarge());
                break;
            case "parabola":
                StartCoroutine(ParabolaMovement(dir,config.speed));
                break;
            default:
                // StartCoroutine(SpiralMovement( config.speed));
                // StartCoroutine(HomingMovement(config.speed));
                StartCoroutine(StraightMovement(dir, config.speed));
                break;
        }
    }
    
    public void Initialize(ProjectileData config, Vector2 dir, string trajectoryType,SpellData spell,int power,bool isSecond = false)
    {
        // 基础初始化
        // GetComponent<SpriteRenderer>().sprite = SpellManager.Instance.projectileSprites[config.sprite];
        linkedSpellData = spell; // 存储关联的法术配置
        currentPower = power;
        isSecondSpell = isSecond;
        // 解析lifetime的RPN表达式
        lifetime = config.lifetime;
        // 设置自动销毁
        if (lifetime > 0)
            Destroy(gameObject, lifetime);

        // 应用喷雾效果
        if (spell.spray != 0.0f)
        {
            float sprayRange = spell.spray;
            transform.position += (Vector3)dir * sprayRange;
        }
        
        // 特殊弹道处理
        switch (trajectoryType)
        {
            case "spiraling":
                StartCoroutine(SpiralMovement(config.speed));
                break;
            case "homing":
                target = FindNearestEnemy();
                StartCoroutine(HomingMovement(config.speed));
                break;
            case "drift":
                StartCoroutine(DriftMovement(dir,config.speed));
                StartCoroutine(DriftLarge());
                break;
            case "parabola":
                StartCoroutine(ParabolaMovement(dir,config.speed));
                break;
            default:
                // StartCoroutine(SpiralMovement( config.speed));
                // StartCoroutine(HomingMovement(config.speed));
                StartCoroutine(StraightMovement(dir, config.speed));
                break;
        }
    }

    // void Update()
    // {
    //     transform.Translate(direction * speed * Time.deltaTime, Space.World);
    // }



    IEnumerator ParabolaMovement(Vector2 dir, float speed)
    { 
        float timer = 0f;
        
        // 计算初始速度分量
        float radian = launchAngle * Mathf.Deg2Rad;
        Vector2 velocity = new Vector2(
            Mathf.Cos(radian) * speed,
            Mathf.Sin(radian) * speed
        );

        // 运动更新循环
        while (timer < 10.0f)
        {
            // 计算位移
            Vector2 displacement = velocity * Time.deltaTime *dir;
            displacement.y += 0.5f * gravity * Time.deltaTime * Time.deltaTime;
            
            // 更新位置
            transform.Translate(displacement, Space.World);
            
            // 更新垂直速度
            velocity.y += gravity * Time.deltaTime;
            
            // 更新计时器
            timer += Time.deltaTime;
            yield return null;
        }
    }
    
    
    
    
    IEnumerator DriftMovement(Vector2 dir, float speed)
    {
        while (true)
        {
        
            transform.Translate(dir * speed * Time.deltaTime, Space.World);
            yield return null;
        }
    }
    IEnumerator DriftLarge()
    {
        while (true)
        {
            transform.localScale += new Vector3(0.3f, 0.5f, 0);
            yield return new WaitForSeconds(1.5f);
        }
    }
    
    
    
    IEnumerator StraightMovement(Vector2 dir, float speed)
    {
        while (true)
        {
            transform.Translate(dir * speed * Time.deltaTime, Space.World);
            yield return null;
        }
    }

    IEnumerator SpiralMovement(float speed)
    {
        float radius = 0;
        while (true)
        {
            radius += Time.deltaTime;
            spiralAngle += 360 * Time.deltaTime;
            Vector2 offset = new Vector2(
                Mathf.Cos(spiralAngle * Mathf.Deg2Rad),
                Mathf.Sin(spiralAngle * Mathf.Deg2Rad)
            ) * radius;
            
            transform.Translate(offset.normalized * speed * Time.deltaTime, Space.World);
            yield return null;
        }
    }

    IEnumerator HomingMovement(float speed)
    {
        // while (target != null)
        // {
        //     Vector2 dir = (target.position - transform.position).normalized;
        //     transform.Translate(dir * speed * Time.deltaTime, Space.World);
        //     yield return null;
        // }
        
        // 初始方向（防止未找到敌人时直线飞行）
        Vector2 defaultDir = transform.right;
    
        while (true)
        {
            // 动态查找最近敌人
            Transform currentTarget = FindNearestEnemy();
        
            // 计算方向
            Vector2 dir = currentTarget != null ? 
                (currentTarget.position - transform.position).normalized :
                defaultDir;

            // 移动逻辑
            transform.Translate(dir * speed * Time.deltaTime, Space.World);
        
            // 更新旋转
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
            yield return null;
        }
    }
    
    
    private Transform FindNearestEnemy()
    {
        // 1. 获取场景中所有激活的敌人对象
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
    
        // 2. 初始化最短距离和最近敌人
        float shortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;

        // 3. 遍历所有敌人
        foreach (GameObject enemy in enemies)
        {
            // 跳过无效对象（可能已被销毁）
            if (enemy == null) continue;

            // 4. 计算当前敌人距离
            float distanceToEnemy = Vector2.Distance(
                transform.position, 
                enemy.transform.position
            );

            // 5. 更新最近敌人
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy.transform;
            }
        }

        // 6. 返回结果（可能为null）
        return nearestEnemy;
    }
    

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // 触发事件（传递碰撞位置和法术数据）
            OnProjectileCollision?.Invoke(transform.position, linkedSpellData);
            
            Debug.Log("伤害数值：" );
            // 命中处理逻辑

            var ec = other.gameObject.GetComponent<EnemyController>();
            if (ec != null)
            {
                int damage;
                if (isSecondSpell)
                {
                    damage =
                        Mathf.RoundToInt(RpnCalculator.Calculate(linkedSpellData.secondary_damage, currentPower));
                    Debug.Log("secondary_damage "+damage);
                }
                else
                {
                    damage =
                        Mathf.RoundToInt(RpnCalculator.Calculate(linkedSpellData.spell_damage.amount, currentPower));
                    Debug.Log("damage "+damage);
                }
              
                
                ec.hp.Damage(new Damage( damage,Damage.Type.ARCANE));
                EventBus.Instance.TriggerDealDamage(
                    transform.position,
                    new Damage(damage, Damage.Type.ARCANE),
                    ec.hp
                );
            }
            
            Destroy(gameObject);
        }
        
       
    }
}