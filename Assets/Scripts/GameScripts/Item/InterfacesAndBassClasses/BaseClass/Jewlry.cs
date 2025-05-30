using System;
using UnityEngine;
using YooAsset;

public class Jewlry : ItemBase
{
    public cfg.item.Jewelry data;
    
    // 可能存在的抽象方法，子类实现方法体
    public override void InitItem(int id,TrackerData trackerData = null)
    {
        base.InitItem(id,trackerData);
        ItemType = GameItemType.Jewelry;
        try
        {
            ItemData = GameTableDataAgent.JewelryTable.Get(id);
            data = ItemData as cfg.item.Jewelry;
            ItemID = data.ID;
        }
        catch (Exception e)
        {
            Debug.LogError("饰品ID" + id +"不存在，物品名称" + gameObject.name);
        }
    }
    public override Sprite GetItemIcon()
    {
        return null;
    }
    public override string GetPrefabName()
    {
        throw new NotImplementedException();
    }
    
    
    public override void OnRightInteract( )
    {
        base.OnRightInteract();
        throw new NotImplementedException();
    }

    public override void OnLeftInteract( )
    {
        base.OnLeftInteract();
        throw new NotImplementedException();
    }
}
