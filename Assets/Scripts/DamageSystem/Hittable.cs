using UnityEngine;
using System;

public class Hittable : MonoBehaviour
{
    public enum Team { PLAYER, MONSTERS }
    public Team team;

    public int hp;
    public int max_hp;

    public GameObject owner;

    private bool isDead;

    void Awake()
    {
        owner = this.gameObject;
        hp = max_hp;
        isDead = false;
    }

    public void Damage(Damage damage)
    {
        EventBus.Instance.DoDamage(owner.transform.position, damage, this);
        hp -= damage.amount;
        if (hp <= 0 && !isDead)
        {
            hp = 0;
            OnDeath?.Invoke();
            isDead = true;
        }
    }

    public event Action OnDeath;

    public void SetMaxHP(int newMaxHp)
    {
        float perc = (float)hp / max_hp;
        max_hp = newMaxHp;
        hp = Mathf.RoundToInt(perc * max_hp);
    }
}
