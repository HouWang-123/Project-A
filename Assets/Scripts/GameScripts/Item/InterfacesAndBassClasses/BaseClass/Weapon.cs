using System;
using DG.Tweening;
using Spine.Unity.Examples;
using UnityEngine;
using YooAsset;

public class Weapon : ItemBase, IItemSlotable
{
    public cfg.item.Weapon ItemData;
    
    // 可能存在的抽象方法，子类实现方法体
    public void Fire()
    {
        
    }
    
    // 动态生成物品
    public override void InitItem( int ID )
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

    public override string GetPrefabName()
    {
        return ItemData.PrefabName;
    }
    public void UpdateItemSlot()
    {
        
    }
    public override void OnRightInteract( )
    {

    }

    public override void OnLeftInteract( )
    {

    }

    public int GetMaxStackValue()
    {
        return ItemData.MaxStackCount;
    }

    public void ChangeStackCount(int Count)
    {
        StackNuberText.text = "X " + Count;
        StackCount = Count;
        if (Count == 1)
        {
            HideStackNumber();
        }
    }

    public int GetStackCount()
    {
        return StackCount;
    }
}
