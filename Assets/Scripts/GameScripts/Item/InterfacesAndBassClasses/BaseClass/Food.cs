using System;
using cfg.interact;
using UnityEngine;
using YooAsset;

public class Food : ItemBase, ISlotable,IStackable,IBuffedItem
{
    public cfg.item.Food data;
    // 可能存在的抽象方法，子类实现方法体
    public cfg.item.Food GetItemData()
    {
        return data;
    }
    // 物品初始化
    public override void InitItem(int id)
    {
        ItemType = GameItemType.Food;
        try
        {
            ItemData = GameTableDataAgent.FoodTable.Get(id);
            data = ItemData as cfg.item.Food;
            ItemID = data.ID;
        }
        catch (Exception e)
        {
            ColorfulDebugger.DebugError("食物ID" + id +"不存在，物品名称" + gameObject.name,ColorfulDebugger.Instance.Data);
        }
        ItemSpriteName = data.SpriteName;
        GameRunTimeData.Instance.ItemManager.RegistItem(this);
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
    public int GetMaxStackValue()
    {
        return data.MaxStackCount;
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
        return data.PrefabName;
    }
    public override void OnRightInteract()
    {
        Debug.Log("食物右键");
    }

    public override void OnLeftInteract()
    {
        Debug.Log("食物左键");
    }
    // todo
    public int GetBuffId()
    {
        throw new NotImplementedException();
    }
}
