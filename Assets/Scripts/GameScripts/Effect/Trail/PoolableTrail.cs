using System;
using System.Collections;
using FEVM.ObjectPool;
using UnityEngine;

public class PoolableTrail : MonoBehaviour,FT_IPoolable
{
    public int PoolBucketId;
    public TrailRenderer trail;



    public void SetBucketId(int id)
    {
        PoolBucketId = id;
    }
    public void Awake()
    {
        trail = GetComponent<TrailRenderer>();
    }
    
    public void OnRecycle()
    {
        trail.Clear();
        trail.emitting = false; // 停止生成新拖尾
    }
    public void OnReEnable()
    {
        trail.Clear();
        trail.emitting = true;
    }
    
    
    IEnumerator Recycle()
    {
        yield return new WaitForSeconds(trail.time);
        GameTrailRendererManager.Instance.RecycleTrail(PoolBucketId,gameObject);
    }

    public void RecycleTrail()
    {
        StartCoroutine(Recycle());
    }
}