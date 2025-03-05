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
        IDamageable damageable = other.gameObject.GetComponentInChildren<IDamageable>();
        if(damageable == null)
        {
            other.gameObject.GetComponentInParent<IDamageable>();
        }
        damageable?.DamageReceive(DamageAmount);
        //生成特效
        //......
        //********
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
