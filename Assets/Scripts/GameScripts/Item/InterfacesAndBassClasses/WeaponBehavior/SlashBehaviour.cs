using System;
using UnityEngine;

public class SlashBehaviour : BaseWeaponBehavior,IDoDamageHandler
{
    private bool attacked = false;
    public float DamageAmount;
    public Collider DamageCollider;
    public override void OnWeaponAttack(float DamageAmount)
    {
        SetDamageAmount(DamageAmount);
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other is IDamageable damageable)
        {
            DoDamage(damageable);
        }
    }

    public void SetDamageAmount(float amount)
    {
        DamageAmount = amount;
    }
    public void DoDamage(IDamageable obj)
    {
        obj.DamageReceive(DamageAmount);
    }
}
