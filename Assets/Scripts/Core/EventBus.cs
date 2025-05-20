using System;
using UnityEngine;

public class EventBus
{
    // ���� ���� ���� 
    private static EventBus theInstance;
    public static EventBus Instance => theInstance ??= new EventBus();

    // ���� �˺��¼� ���� 
    public event Action<Vector3, Damage, Hittable> OnDamage;
    public void DoDamage(Vector3 where, Damage dmg, Hittable target)
        => OnDamage?.Invoke(where, dmg, target);

    // ���� ��ɱ�¼� ���� 
    public event Action OnKill;
    public void TriggerOnKill()
        => OnKill?.Invoke();

    // ���� �ƶ�/��ֹ�¼� ���� 
    public event Action OnMove;
    public void TriggerMove()
        => OnMove?.Invoke();

    public event Action OnStandStill;
    public void TriggerStandStill()
        => OnStandStill?.Invoke();
}
