using System;
using DG.Tweening;
using Spine.Unity.Examples;
using UnityEngine;
using YooAsset;

public class Weapon : ItemBase, ISlotable
{
    public cfg.item.Weapon data;
    
    // 动态生成物品
    public override void InitItem( int ID )
    {
        ItemType = GameItemType.Weapon;
        try
        {
            ItemData = GameTableDataAgent.WeaponTable.Get(ID);
            data = ItemData as cfg.item.Weapon;
            ItemID = data.ID;
        }
        catch (Exception e)
        {
            ColorfulDebugger.DebugError("武器物品ID" + ID +"不存在，物品名称" + gameObject.name,ColorfulDebugger.Instance.Data);
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

    public override string GetPrefabName()
    {
        return data.PrefabName;
    }
    public override void OnRightInteract( )
    {
        
    }

    public override void OnLeftInteract( )
    {
        
    }

    public int GetMaxStackValue()
    {
        return data.MaxStackCount;
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
