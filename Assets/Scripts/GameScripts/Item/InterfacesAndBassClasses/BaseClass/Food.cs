using System;
using UnityEngine;
using YooAsset;

public class Food : ItemBase ,IPickUpable ,IStackable,IBuffedItem
{
    public cfg.item.Food data;
    // 可能存在的抽象方法，子类实现方法体
    public cfg.item.Food GetItemData() { return data; }
    // 物品初始化
    public override void InitItem(int id,TrackerData trackerData = null)
    {
        base.InitItem(id, trackerData);
        ItemType = GameItemType.Food;
        try
        {
            ItemData = GameTableDataAgent.FoodTable.Get(id);
            data = ItemData as cfg.item.Food;
            ItemID = data.ID;
        }
        catch (Exception e)
        {
            Debug.LogError("食物ID" + id +"不存在，物品名称" + gameObject.name);
        }
        ItemSpriteName = data.SpriteName;
    }
    public override Sprite GetItemIcon()
    {
        AssetHandle loadAssetSync = YooAssets.LoadAssetSync<Sprite>(data.IconName);
        if (loadAssetSync.AssetObject == null)
        {
            loadAssetSync = YooAssets.LoadAssetSync<Sprite>("SpriteNotFound_Default");
        }
        return Instantiate(loadAssetSync.AssetObject, transform) as Sprite;
    }
    public int GetMaxStackValue() { return data.MaxStackCount; }
    public void ChangeStackCount(int Count)
    {
        StackCount = Count;
        StackNuberText.text = "X " + Count;
        if (Count == 1)
        {
            HideStackNumber();
        }
    }
    public int GetStackCount() { return StackCount; }
    public override string GetPrefabName() { return data.PrefabName; }

    public override void OnRightInteract()
    {
        if (startactionTime < 0.1f)
        {
            return;
        }
    }

    public override void OnLeftInteract()
    {
        if (startactionTime < 0.1f)
        {
            return;
        }
    }
    public int GetBuffId()
    {
        return data.BuffID;
    }

    public int GetItemId()
    {
        return ItemID;
    }
}
