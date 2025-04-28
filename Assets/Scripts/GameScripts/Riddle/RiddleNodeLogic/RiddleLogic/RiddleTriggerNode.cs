using System;
using FEVM.Timmer;
using UnityEngine;
using UnityEngine.Events;

public class RiddleTriggerNode : MonoBehaviour
{
    public float TriggerDelay;
    public Color TriggerColor;
    public UnityEvent Trigger;
    public void OnTrigger()
    {
        TimeMgr.Instance.AddTask(TriggerDelay,false,()=>Trigger?.Invoke());
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = TriggerColor;
        Gizmos.DrawSphere(transform.position,0.1f);
    }
}
