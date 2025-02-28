using System;
using UnityEngine;
using YooAsset;

public class SceneObjects : ItemBase
{
    public cfg.item.SceneObjects data;

    public override void InitItem(int id)
    {
        ItemType = GameItemType.SceneObject;
        try
        {
            ItemData = GameTableDataAgent.SceneObjectsTable.Get(id);
            data = ItemData as cfg.item.SceneObjects;
            ItemID = data.ID;
        }
        catch (Exception e)
        {
            ColorfulDebugger.DebugError("场景物品ID" + ItemID +"不存在，物品名称" + gameObject.name,ColorfulDebugger.Instance.Data);
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
    public override string GetPrefabName()
    {
        return data.PrefabName;
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
