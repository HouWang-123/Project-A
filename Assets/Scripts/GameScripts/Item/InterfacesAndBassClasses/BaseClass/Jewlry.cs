using System;
using UnityEngine;
using YooAsset;

public class Jewlry : ItemBase
{
    public cfg.item.Jewelry ItemData;
    
    // 可能存在的抽象方法，子类实现方法体
    public void Equip()
    {
        
    }

    public override void InitItem(int id)
    {
        ItemType = GameItemType.Jewelry;
        
        try
        {
            ItemData = GameTableDataAgent.JewelryTable.Get(id);
            ItemID = ItemData.ID;
        }
        catch (Exception e)
        {
            ColorfulDebugger.DebugError("饰品ID" + id +"不存在，物品名称" + gameObject.name,ColorfulDebugger.Instance.Data);
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
        throw new NotImplementedException();
    }

    public override void OnLeftInteract( )
    {
        throw new NotImplementedException();
    }
}
