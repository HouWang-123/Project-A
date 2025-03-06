using System;
using UnityEngine;
using DG.Tweening;
using FEVM.Timmer;

public class BulletBehaviour : MonoBehaviour,IDoDamageHandler
{
    private PoolableTrail MyTrailRender;
    
    public int WeaponId;
    
    public ShotBehaviour m_ShotBehaviour;
    public float Speed = 100;
    public float DamageAmount;
    private float flyDistance; // Dang

    public void GetBulletTrail()
    {
        MyTrailRender = GameTrailRendererManager.Instance.GetATrail(WeaponId,transform);
        MyTrailRender.transform.position = transform.position;
    }

    public void RecycleTrail()
    {
        PoolableTrail poolableTrail = MyTrailRender.GetComponent<PoolableTrail>();
        poolableTrail.RecycleTrail();
    }
    public void SetShotParent(ShotBehaviour behaviour)
    {
        m_ShotBehaviour = behaviour;
    }
    public void SetInitialDamage(float amount)
    {
        DamageAmount = amount;
    }
    private void FixedUpdate()
    {
        Vector3 lastpPosition = transform.position;
        transform.position += transform.right * Speed * Time.deltaTime;
        Vector3 nextPosition = transform.position;
        flyDistance += Vector3.Distance(lastpPosition, nextPosition);
        MyTrailRender.transform.position = transform.position;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == GameRoot.Instance.FloorLayer)
        {
            Destroy(gameObject);
        }
        IDamageable damageable = other.gameObject.GetComponentInChildren<IDamageable>();
        if(damageable == null)
        {
            other.gameObject.GetComponentInParent<IDamageable>();
        }
        damageable?.DamageReceive(DamageAmount);
        //生成特效
        //......
        //********
        m_ShotBehaviour.RecycleBullet(this);
    }
}
