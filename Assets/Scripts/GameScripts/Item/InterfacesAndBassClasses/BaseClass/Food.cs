using System;
using cfg.interact;
using UnityEngine;
using YooAsset;

public class Food : ItemBase, IItemSlotable,IStackable
{
    public cfg.item.Food ItemData;
    public void Awake()
    {
        InitItem();
    }
    // 可能存在的抽象方法，子类实现方法体
    public void Eat()
    {
        
    }
    
    // 物品初始化
    protected override void InitItem()
    {
        ItemType = GameItemType.Food;
        try
        {
            ItemData = GameTableDataAgent.FoodTable.Get(ItemID);
        }
        catch (Exception e)
        {
            ColorfulDebugger.DebugError("食物ID" + ItemID +"不存在，物品名称" + gameObject.name,ColorfulDebugger.Instance.Data);
        }
    }
    protected override void InitItem(int id)
    {
        ItemType = GameItemType.Food;
        try
        {
            ItemData = GameTableDataAgent.FoodTable.Get(id);
            ItemID = ItemData.ID;
        }
        catch (Exception e)
        {
            ColorfulDebugger.DebugError("食物ID" + id +"不存在，物品名称" + gameObject.name,ColorfulDebugger.Instance.Data);
        }
    }

    public override Sprite GetItemIcon()
    {
        AssetHandle loadAssetSync = YooAssets.LoadAssetSync<Sprite>(ItemData.IconName);
        return Instantiate(loadAssetSync.AssetObject, transform) as Sprite;
    }

    public int GetMaxStackValue()
    {
        return ItemData.MaxStackCount;
    }
    public override string GetPrefabName()
    {
        return ItemData.PrefabName;
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
