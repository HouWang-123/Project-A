
using System;
using UnityEngine;

public class NegRiddleNode : MonoBehaviour, IRiddleValueNode
{
    public GameObject RiddleValueA;

    private IRiddleValueNode nodea;
    
    public void Start()
    {
        if (RiddleValueA == null)
        {
            Debug.LogError("运算组件缺少输入");
        }
        nodea = RiddleValueA.GetComponent<IRiddleValueNode>();
    }

    public int GetNum()
    {
        int va = 0;
        va = nodea.GetNum();
        return -va;
    }

    public void SetNum(int num)
    {
        throw new NotImplementedException();
    }
}
