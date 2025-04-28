
using System;
using System.Linq.Expressions;
using UnityEngine;

public class CompareRiddleNode : MonoBehaviour, IRiddleNode
{
    public string nodeKey;
    public GameObject RiddleValueA;
    public GameObject RiddleValueB;
    private enum CompareType
    {
        Greater,
        less,
        equal,
        GreaterOrEqual,
        LessOrEqual
    }

    [SerializeField] private CompareType _compareType;
    private IRiddleValueNode nodea;
    private IRiddleValueNode nodeb;
    
    public void Start()
    {
        if (RiddleValueA != null)
        {
            IRiddleValueNode node = RiddleValueA.GetComponent<IRiddleValueNode>();
            if (node != null)
            {
                nodea = node;
            }
            else
            {
                Debug.LogError("谜题数值组件缺失");
            }
        }
        if (RiddleValueB != null)
        {
            IRiddleValueNode node = RiddleValueB.GetComponent<IRiddleValueNode>();
            if (node != null)
            {
                nodeb = node;
            }
            else
            {
                Debug.LogError("谜题数值组件缺失");
            }
        }
    }

    public string GetNodeKey()
    {
        return nodeKey;
    }

    public bool GetResult()
    {
        switch (_compareType)
        {
            case CompareType.equal:
                return nodea.GetNum() == nodeb.GetNum();
            case CompareType.less:
                return nodea.GetNum() < nodeb.GetNum();
            case CompareType.Greater:
                return nodea.GetNum() > nodeb.GetNum();
            case CompareType.GreaterOrEqual:
                return nodea.GetNum() >= nodeb.GetNum();
            case CompareType.LessOrEqual:
                return nodea.GetNum() <= nodeb.GetNum();
            default:
                return false;
        }
    }
    private void OnDrawGizmosSelected()
    {
        // 只有当这个物体被选中时才会绘制
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, 0.3f * Vector3.one);
        if (RiddleValueA != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(RiddleValueA.transform.position, 0.3f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(RiddleValueA.transform.position,transform.position);
        }

        if (RiddleValueB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(RiddleValueB.transform.position, 0.3f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(RiddleValueB.transform.position,transform.position);
        }
    }
}
