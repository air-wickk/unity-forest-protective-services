using UnityEngine;
using System.Collections;

public class Honey : ItemBase
{
    public int healAmount = 50;
    public float buffDuration = 10f;
    public float attackBuff = 1.5f;
    public float defenseBuff = 1.5f;

    public override void Use(GameObject user)
    {
        PlayerHealth health = user.GetComponent<PlayerHealth>();
        PlayerBuffs buffs = user.GetComponent<PlayerBuffs>();
        if (health != null)
        {
            health.Heal(healAmount);
        }
        if (buffs != null)
        {
            buffs.ApplyBuffs(attackBuff, defenseBuff, buffDuration);
        }
        Destroy(gameObject);
    }
}
