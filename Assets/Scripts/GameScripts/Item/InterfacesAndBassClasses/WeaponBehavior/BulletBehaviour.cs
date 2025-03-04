using System;
using UnityEngine;
using DG.Tweening;

public class BulletBehaviour : MonoBehaviour,IDoDamageHandler
{
    public float Speed = 10;
    public float DamageAmount;
    public void SetDamage(float amount)
    {
        DamageAmount = amount;
    }
    private void FixedUpdate()
    {
        transform.position += transform.right * Speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision other)
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
        Destroy(gameObject);
    }

    public void DoDamage(IDamageable obj)
    {
        obj.DamageReceive(DamageAmount);
    }
}
