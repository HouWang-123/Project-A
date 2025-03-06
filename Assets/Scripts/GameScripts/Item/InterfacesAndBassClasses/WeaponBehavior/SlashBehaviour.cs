using System;
using UnityEngine;

public class SlashBehaviour : BaseWeaponBehavior,IDoDamageHandler
{
    private bool attacked = false;
    public float DamageAmount;
    public Collider DamageCollider;
    public override void OnWeaponAttack(float DamageAmount)
    {
        // Play Animations; Enable Coliders;
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

    public void SetInitialDamage(float amount)
    {
        DamageAmount = amount;
    }
}
