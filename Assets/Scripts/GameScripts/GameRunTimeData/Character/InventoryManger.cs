using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventroyData
{
    public Dictionary<int, int> ItemData;
}

/// <summary>
/// 道具管理器
/// </summary>
public class InventoryManger
{
    private InventroyData _inventroyData = new InventroyData();

    public void InitManager(InventroyData inventroyData)
    {
        _inventroyData = inventroyData;
    }

    // 获得物品接口
    public void OnGetItem(int itemid, int amount)
    {
        if (_inventroyData.ItemData.ContainsKey(itemid))
        {
            _inventroyData.ItemData[itemid] += amount;
        }
        else
        {
            _inventroyData.ItemData.Add(itemid,1);
        }
    }
    // 使用物品
    public bool UseItem(int itemid, int amount)
    {
        if (HasItem(itemid))
        {
            _inventroyData.ItemData[itemid]-= amount;
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool HasItem(int itemid)
    {
        if (_inventroyData.ItemData.ContainsKey(itemid))
        {
            if (_inventroyData.ItemData[itemid] > 0)
            {
                return true;
            }
        }

        return false;
    }
}