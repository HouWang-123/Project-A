using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 谜题场景，可能存在多个物品
/// </summary>
public class RiddleManager : SerializedMonoBehaviour
{
    public List<RiddleItemBase> RiddleItems = new();
    /// <summary>
    /// 执行列表
    /// </summary>
    public Dictionary<string, UnityEvent> ExecuteList;

    private Dictionary<int,RiddleItemBase> key2ItemList = new ();

    public void Start()
    {
        foreach (var riddleItem in RiddleItems)
        {
            IRiddleItem RiddleHandler = riddleItem.transform.GetComponent<IRiddleItem>();
            if (RiddleHandler != null)
            {
                RiddleHandler.SetRiddleManager(this);
            }
        }
    }

    public void ExecuteRiddle(string key)
    {
        ExecuteList[key].Invoke();
    }

    public void OnRiddleItemStatusChange(RiddleItemBase riddleItemBase){
        Debug.Log("物品发生改变，ID: " + riddleItemBase.GetRiddleKey());
    }
}