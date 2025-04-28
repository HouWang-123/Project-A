
using UnityEngine;

public class TrippleAndNode : MonoBehaviour,IRiddleNode
{
    public string RiddleNodeKey;
    // 这里可以放入RiddleItem或者RiddleNode
    public GameObject RiddleObjA;
    public GameObject RiddleObjB;
    public GameObject RiddleObjC;
    public string GetNodeKey()
    {
        return RiddleNodeKey;
    }
    public bool GetResult()
    {
        IRiddleItem RiddleItemA = RiddleObjA.GetComponent<IRiddleItem>();
        IRiddleItem RiddleItemB = RiddleObjB.GetComponent<IRiddleItem>();
        IRiddleItem RiddleItemC = RiddleObjB.GetComponent<IRiddleItem>();
        
        IRiddleNode riddleNodeA = RiddleObjA.GetComponent<IRiddleNode>();
        IRiddleNode riddleNodeB = RiddleObjB.GetComponent<IRiddleNode>();
        IRiddleNode riddleNodeC = RiddleObjB.GetComponent<IRiddleNode>();
        bool ResultA = false;
        bool ResultB = false;
        bool ResultC = false;
        if (RiddleItemA == null && riddleNodeA == null)
        {
            Debug.LogWarning(RiddleObjA.name +" 缺少相关解谜组件");
        }
        if (RiddleItemB == null && riddleNodeB == null)
        {
            Debug.LogWarning(RiddleObjB.name +" 缺少相关解谜组件");
        }        
        if (RiddleItemC == null && riddleNodeC == null)
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
        
        if (RiddleItemC != null)
        {
            ResultB = RiddleItemC.GetRiddleItemResult();
        }
        if (riddleNodeC != null)
        {
            ResultB = riddleNodeC.GetResult();
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
        
        if (RiddleObjC != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(RiddleObjC.transform.position, 0.3f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(RiddleObjC.transform.position,transform.position);
        }
    }
}
