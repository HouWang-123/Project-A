using System;
using System.Collections;
using System.Collections.Generic;
using FEVM.ObjectPool;
using UnityEngine;

public class AutoRecycledParticleSystem : MonoBehaviour, FT_IPoolable
{
    private int BucketID;
    public ParticleSystem MainSystem;
    // Start is called before the first frame update
    public ParticleSystem[] sys;
    public float sysLifeTime;
    public float exsistTime;
    public Action OnStopCallBack;
    void Start()
    {
        float maxtime = 0;
        sys = GetComponentsInChildren<ParticleSystem>();
        foreach (var VARIABLE in sys)
        {
            if (VARIABLE.main.duration > maxtime)
            {
                maxtime = VARIABLE.main.duration / VARIABLE.main.simulationSpeed;
            }
        }
        sysLifeTime = maxtime;
    }
    
    // Update is called once per frame
    void Update()
    {
        exsistTime += Time.deltaTime;
        if (exsistTime > sysLifeTime)
        {
            float updatedMaxTime = 0;
            Array.ForEach(sys, p =>
            {
                float currentMaxTime = p.main.duration / p.main.simulationSpeed;
                if (updatedMaxTime < currentMaxTime)
                {
                    updatedMaxTime = currentMaxTime;
                }
                p.Stop();
            });
            sysLifeTime = updatedMaxTime; // keep Track Particle's Time Data
            exsistTime = 0f;
            OnStopCallBack?.Invoke();
        }
    }
    
    public void OnRecycle()
    {
        gameObject.SetActive(false);
    }

    public void OnReEnable()
    {
        
    }

    public void SetBucketId(int ID)
    {
        BucketID = ID;
    }
}
