using System;
using FEVM.Timmer;
using UnityEngine;
using UnityEngine.Events;

public class RiddleTriggerNode : MonoBehaviour
{
    public float TriggerDelay;
    public Color TriggerColor;
    
    [Header("触发器条件")]
    public GameObject RiddleConditionNode;
    [Header("触发效果")]
    public UnityEvent Trigger;
    
    private IRiddleNode<bool> PreConditionRiddleNode;
    
    public bool hasCondition;
    public void OnTrigger()
    {
        bool res;
        if (hasCondition)
        {
            res = PreConditionRiddleNode.GetResult();
            if (res)
            {
                TimeMgr.Instance.AddTask(TriggerDelay,false,()=>Trigger?.Invoke());
            }
        }
        else
        {
            TimeMgr.Instance.AddTask(TriggerDelay,false,()=>Trigger?.Invoke());
        }
    }

    private void Start()
    {
        hasCondition = false;
        if (RiddleConditionNode != null)
        {
            IRiddleNode<bool> riddleNode = RiddleConditionNode.transform.GetComponent<IRiddleNode<bool>>();
            if (riddleNode != null)
            {
                PreConditionRiddleNode = riddleNode;
                hasCondition = true;
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = TriggerColor;
        Gizmos.DrawSphere(transform.position,0.1f);
        if (RiddleConditionNode != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(RiddleConditionNode.transform.position,0.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position,RiddleConditionNode.transform.position);
        }

    }
}
