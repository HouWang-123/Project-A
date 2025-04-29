using System;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class ExecuteListDictionary<k,v> : SerializableDictionaryBase<string,UnityEvent>{}
public class GameObjectNodeDictionary<k,v> : SerializableDictionaryBase<string,GameObject>{}
/// <summary>
/// 谜题场景，可能存在多个物品
/// </summary>
public class RiddleManager : SerializedMonoBehaviour
{
    public List<RiddleItemBase> RiddleItems = new();
    /// <summary>
    /// 执行列表
    /// </summary>
    public ExecuteListDictionary<string, UnityEvent> ExecuteList = new ();
    
    public GameObjectNodeDictionary<string, GameObject> NodeValidations = new ();
    
    private Dictionary<string,RiddleItemBase> key2ItemList = new ();

    public void Start()
    {
        foreach (var riddleItem in RiddleItems)
        {
            IRiddleItem RiddleHandler = riddleItem.transform.GetComponent<IRiddleItem>();
            if (RiddleHandler != null)
            {
                RiddleHandler.SetRiddleManager(this);
            }
            key2ItemList.Add(RiddleHandler.GetRiddleKey(),riddleItem);
        }
        Dictionary<string,RiddleItemBaseStatus> LoadedItemStatus = 
            GameRunTimeData.Instance.RiddleItemStatusManager.LoadRiddleItemBaseStatusMap(GameControl.Instance.GetRoomData().ID);
        if (LoadedItemStatus != null)
        {
            if (LoadedItemStatus.Count > 0)
            {
                foreach (var kv in LoadedItemStatus)
                {
                    key2ItemList[kv.Key].SetRiddleItemStatus(kv.Value);
                }
            }
        }
        SaveOrUpdateItemStatus();
    }
    public void OnRiddleItemStatusChange(RiddleItemBase riddleItemBase){
        Debug.Log("物品发生改变，ID: " + riddleItemBase.GetRiddleKey());
        SaveOrUpdateItemStatus();
    }

    public void SaveOrUpdateItemStatus()
    {
        GameRunTimeData.Instance.RiddleItemStatusManager.SaveRiddleStatusFromManager(this);
    }

    public List<RiddleItemBase> GetAllRiddleItem()
    {
        return RiddleItems;
    }
    // 运行所有检查项，不推荐直接执行，可通过配置触发器延迟一些功能点的触发
    [ContextMenu("ExecuteAllRiddleFunctions")]
    public void ExecuteAllFunctionsManager()
    {
        foreach (var node in NodeValidations)
        {
            if (String.IsNullOrEmpty(node.Key))
            {
                continue;
            }
            if(node.Value == null) continue;
            
            string ExecuteKey = node.Key;
            ExecuteSingleRiddleLogic(ExecuteKey);
        }
    }
    // 检查功能点
    public void ExecuteSingleRiddleLogic(string key)
    {
        GameObject nodeValue = NodeValidations[key];
        IRiddleNode<bool> riddleNode = nodeValue.transform.GetComponent<IRiddleNode<bool>>();
        
        bool result = riddleNode.GetResult();
        
        if (result)
        {
            if (ExecuteList.ContainsKey(key))
            {
                ExecuteList[key]?.Invoke();
            }
        }
    }
}