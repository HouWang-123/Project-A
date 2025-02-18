using System;
using UnityEngine;
using YooAsset;

public class Jewlry : ItemBase
{
    public cfg.item.Jewelry ItemData;
    public void Awake()
    {
        InitItem();
    }
    // 可能存在的抽象方法，子类实现方法体
    public void Equip()
    {
        
    }
    // 物品初始化
    protected override void InitItem()
    {
        ItemType = GameItemType.Jewelry;
        
        try
        {
            ItemData = GameTableDataAgent.JewelryTable.Get(ItemID);
        }
        catch (Exception e)
        {
            ColorfulDebugger.DebugError("饰品ID" + ItemID +"不存在，物品名称" + gameObject.name,ColorfulDebugger.Instance.Data);
        }
    }
    protected override void InitItem(int id)
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
    
    
    public override void OnRightInteract(InterActionData interActionData)
    {
        throw new NotImplementedException();
    }

    public override void OnLeftInteract(InterActionData interActionData)
    {
        throw new NotImplementedException();
    }
}
