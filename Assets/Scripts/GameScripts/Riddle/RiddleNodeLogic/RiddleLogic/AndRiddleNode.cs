using UnityEngine;
using UnityEngine.Serialization;

public class AndRiddleNode : MonoBehaviour,IRiddleNode
{
    public string RiddleNodeKey;
    // 这里可以放入RiddleItem或者RiddleNode
    public GameObject RiddleObjA;
    public GameObject RiddleObjB;
    public string GetNodeKey()
    {
        return RiddleNodeKey;
    }
    public bool GetResult()
    {
        IRiddleItem RiddleItemA = RiddleObjA.GetComponent<IRiddleItem>();
        IRiddleItem RiddleItemB = RiddleObjB.GetComponent<IRiddleItem>();
        IRiddleNode riddleNodeA = RiddleObjA.GetComponent<IRiddleNode>();
        IRiddleNode riddleNodeB = RiddleObjB.GetComponent<IRiddleNode>();
        bool ResultA = false;
        bool ResultB = false;
        if (RiddleItemA == null && riddleNodeA == null)
        {
            Debug.LogWarning(RiddleObjA.name +" 缺少相关解谜组件");
        }
        if (RiddleItemB == null && riddleNodeB == null)
        {
            Debug.LogWarning(RiddleObjB.name +" 缺少相关解谜组件");
        }

        if (RiddleItemA != null)
        {
            ResultA = RiddleItemA.GetRiddleItemResult();
        }
        if (riddleNodeA != null)
        {
            ResultA = riddleNodeA.GetResult();
        }
        if (RiddleItemB != null)
        {
            ResultB = RiddleItemB.GetRiddleItemResult();
        }
        if (riddleNodeB != null)
        {
            ResultB = riddleNodeB.GetResult();
        }
        
        if (ResultA && ResultB)
        {
            return true;
        }
        return false;
    }
    private void OnDrawGizmosSelected()
    {
        // 只有当这个物体被选中时才会绘制
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position, 0.3f * Vector3.one);
        if (RiddleObjA != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(RiddleObjA.transform.position, 0.3f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(RiddleObjA.transform.position,transform.position);
        }

        if (RiddleObjB != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(RiddleObjB.transform.position, 0.3f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(RiddleObjB.transform.position,transform.position);
        }
    }
}
