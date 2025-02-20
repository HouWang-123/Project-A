using System;
using UnityEngine;
using YooAsset;

public class TinyItem : ItemBase
{
    public cfg.item.TinyObjects ItemData;
    
    // 可能存在的抽象方法，子类实现方法体
    
    // 物品初始化

    public override void InitItem(int id)
    {
        ItemType = GameItemType.TinyItem;

        try
        {
            ItemData = GameTableDataAgent.TinyObjectsTable.Get(id);
            ItemID = ItemData.ID;
        }
        catch (Exception e)
        {
            ColorfulDebugger.DebugError("小物品ID" + id +"不存在，物品名称" + gameObject.name,ColorfulDebugger.Instance.Data);
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
    
    
    public override void OnRightInteract( )
    {
        throw new NotImplementedException();
    }

    public override void OnLeftInteract( )
    {
        throw new NotImplementedException();
    }
}
