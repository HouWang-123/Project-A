using System;
using cfg.interact;
using UnityEngine;
using YooAsset;

public class Food : ItemBase, IItemSlotable,IStackable
{
    public cfg.item.Food ItemData;
    
    // 可能存在的抽象方法，子类实现方法体
    public void Eat()
    {
        
    }
    
    // 物品初始化
    public override void InitItem(int id)
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
        ItemSpriteName = ItemData.SpriteName;
    }

    public override Sprite GetItemIcon()
    {
        AssetHandle loadAssetSync = YooAssets.LoadAssetSync<Sprite>(ItemData.IconName);
        if (loadAssetSync.AssetObject == null)
        {
            loadAssetSync = YooAssets.LoadAssetSync<Sprite>("SpriteNotFound_Default");
        }
        return Instantiate(loadAssetSync.AssetObject, transform) as Sprite;
    }

    public int GetMaxStackValue()
    {
        return ItemData.MaxStackCount;
    }

    public void ChangeStackCount(int Count)
    {
        StackCount = Count;
        StackNuberText.text = "X " + Count;
        if (Count == 1)
        {
            HideStackNumber();
        }
    }

    public int GetStackCount()
    {
        return StackCount;
    }

    public override string GetPrefabName()
    {
        return ItemData.PrefabName;
    }
    
    
    public override void OnRightInteract( )
    {
        Debug.Log("食物右键");
    }

    public override void OnLeftInteract( )
    {
        Debug.Log("食物左键");
    }
}
