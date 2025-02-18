using System;
using DG.Tweening;
using Spine.Unity.Examples;
using UnityEngine;
using YooAsset;

public class Weapon : ItemBase, IItemSlotable, IStackable
{
    public cfg.item.Weapon ItemData;
    private void Awake()
    {
        InitItem();
    }
    // 可能存在的抽象方法，子类实现方法体
    public void Fire()
    {
        
    }
    // 物品初始化
    protected override void InitItem()
    {
        ItemType = GameItemType.Weapon;
        try
        {
            ItemData = GameTableDataAgent.WeaponTable.Get(ItemID);
        }
        catch (Exception e)
        {
            ColorfulDebugger.DebugError("武器物品ID" + ItemID +"不存在，物品名称" + gameObject.name,ColorfulDebugger.Instance.Data);
        }
    }
    
    // 动态生成物品
    protected override void InitItem( int ID )
    {
        ItemType = GameItemType.Weapon;
        try
        {
            ItemData = GameTableDataAgent.WeaponTable.Get(ID);
            ItemID = ItemData.ID;
        }
        catch (Exception e)
        {
            ColorfulDebugger.DebugError("武器物品ID" + ID +"不存在，物品名称" + gameObject.name,ColorfulDebugger.Instance.Data);
        }
    }

    public override Sprite GetItemIcon()
    {
        AssetHandle loadAssetSync = YooAssets.LoadAssetSync<Sprite>(ItemData.IconName);
        return Instantiate(loadAssetSync.AssetObject, transform) as Sprite;
    }

    public override string GetPrefabName()
    {
        return ItemData.PrefabName;
    }
    public void UpdateItemSlot()
    {
        
    }
    public override void OnRightInteract( )
    {
        throw new NotImplementedException();
    }

    public override void OnLeftInteract( )
    {
        throw new NotImplementedException();
    }

    public int GetMaxStackValue()
    {
        return ItemData.MaxStackCount;
    }
    
}
