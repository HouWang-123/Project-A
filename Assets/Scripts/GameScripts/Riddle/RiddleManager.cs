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
    }

    public RiddleItemBase GetRiddleItemByKey(string riddleKey)
    {
        return key2ItemList[riddleKey];
    }
    public void ExecuteRiddleLogic(string key)
    {
        ExecuteList[key].Invoke();
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
}