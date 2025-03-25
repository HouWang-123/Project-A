using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 数据类型的道具
/// </summary>
[Serializable]
public class InventroyData
{
    public Dictionary<int, int> ItemData = new Dictionary<int, int>();
}

/// <summary>
/// 道具管理器
/// </summary>
public class InventoryManger
{
    private InventroyData _inventroyData = new InventroyData();
    
    // 预留存档功能使用
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
    /// <summary>
    /// 道具使用
    /// </summary>
    /// <param name="itemid"></param>
    /// <returns></returns>
    public bool HasItem(int itemid)
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