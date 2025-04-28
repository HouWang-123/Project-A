using System;
using System.Collections.Generic;
using UnityEngine;

public class AddRiddleNode : MonoBehaviour, IRiddleValueNode
{
    public List<GameObject> RiddleValueA = new ();
    private List<IRiddleValueNode> Handlerlist;
    
    public void Start()
    {
        if (RiddleValueA.Count == 0)
        {
            Debug.LogError("运算组件缺少输入");
        }
        foreach (var node in RiddleValueA)
        {
            Handlerlist.Add(node.GetComponent<IRiddleValueNode>());
        }
    }
    public int GetNum()
    {
        int result = 0;
        foreach (var node in Handlerlist)
        {
            result += node.GetNum();
        }
        return result;
    }
    public void SetNum(int num)
    {
        throw new NotImplementedException();
    }
}
