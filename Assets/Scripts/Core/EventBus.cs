using System;
using UnityEngine;

public class EventBus
{
    // —— 单例 —— 
    private static EventBus theInstance;
    public static EventBus Instance => theInstance ??= new EventBus();

    // —— 伤害事件 —— 
    public event Action<Vector3, Damage, Hittable> OnDamage;
    public void DoDamage(Vector3 where, Damage dmg, Hittable target)
        => OnDamage?.Invoke(where, dmg, target);

    // —— 击杀事件 —— 
    public event Action OnKill;
    public void TriggerOnKill()
        => OnKill?.Invoke();

    // —— 移动/静止事件 —— 
    public event Action OnMove;
    public void TriggerMove()
        => OnMove?.Invoke();

    public event Action OnStandStill;
    public void TriggerStandStill()
        => OnStandStill?.Invoke();
}
