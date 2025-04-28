using UnityEngine;

public class NotRiddleNode : MonoBehaviour,IRiddleNode<bool>
{
    public string RiddleNodeKey;
    // 这里可以放入RiddleItem或者RiddleNode
    public GameObject RiddleObjA;
    public string GetNodeKey()
    {
        return RiddleNodeKey;
    }
    public bool GetResult()
    {
        IRiddleItem RiddleItemA = RiddleObjA.GetComponent<IRiddleItem>();
        IRiddleNode<bool> riddleNodeA = RiddleObjA.GetComponent<IRiddleNode<bool>>();
        bool ResultA = false;
        if (RiddleItemA == null && riddleNodeA == null)
        {
            Debug.LogWarning(RiddleObjA.name +" 缺少相关解谜组件");
        }

        if (RiddleItemA != null)
        {
            ResultA = RiddleItemA.GetRiddleItemResult();
        }
        if (riddleNodeA != null)
        {
            ResultA = riddleNodeA.GetResult();
        }
        return !ResultA;
    }
    private void OnDrawGizmosSelected()
    {
        // 只有当这个物体被选中时才会绘制
        Gizmos.color = Color.blue;  
        Gizmos.DrawCube(transform.position, 0.3f * Vector3.one);
        if (RiddleObjA != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(RiddleObjA.transform.position, 0.3f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(RiddleObjA.transform.position,transform.position);
        }
    }
}
