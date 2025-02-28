using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class SceneItemStatus
{
    public int SceneId;
    public int itemId;
    public Vector3 itemPosition;
    public int StackCount;
}
public class ItemManager
{
    public Dictionary<ItemBase, SceneItemStatus> ItemList = new Dictionary<ItemBase, SceneItemStatus>(); // RunTime
    
    public void RegistItem(ItemBase itemBase)
    {
        SceneItemStatus status = new SceneItemStatus();
        if (itemBase is IStackable stackable)
        {
            status.StackCount = stackable.GetStackCount();
        }
        status.itemId = itemBase.ItemID;
        status.itemPosition = itemBase.transform.position;
        status.SceneId = GameControl.Instance.GetRoomData().ID;
        
        if (ItemList.ContainsKey(itemBase))
        {
            ItemList.Remove(itemBase);
        }
        ItemList.Add(itemBase,status);
    }

    public void UnRegistItem(ItemBase itemBase)
    {
        ItemList.Remove(itemBase);
    }
}