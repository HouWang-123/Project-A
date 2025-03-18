using System;
using UnityEngine;
using DG.Tweening;
using FEVM.ObjectPool;
using FEVM.Timmer;

public class BulletBehaviour : MonoBehaviour,IDoDamageHandler,FT_IPoolable
{
    private PoolableTrail MyTrailRender;
    
    public int WeaponId;
    
    public ShotBehaviour m_ShotBehaviour;
    public float Speed = 100;
    public float DamageAmount;
    private float flyDistance; // Dang
    
    private int myBucketId;
    
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
            damageable = other.gameObject.GetComponentInParent<IDamageable>();
        }

        if (damageable == null)
        {
            // shot pool null
            RecycleTrail();
            GameObjectPool.Instance.RecycleGameObject(gameObject,myBucketId);
            return;
        }
        
        damageable?.DamageReceive(DamageAmount);
        //生成特效
        //......
        //********
        
        
        // shot pool null
        RecycleTrail();
        GameObjectPool.Instance.RecycleGameObject(gameObject,myBucketId);
    }

    public void OnRecycle()
    {

    }

    public void OnReEnable()
    {

    }

    public void SetBucketId(int ID)
    {
        WeaponId = -ID;
        myBucketId = ID;
    }
}
