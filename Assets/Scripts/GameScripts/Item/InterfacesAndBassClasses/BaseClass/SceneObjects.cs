using System;
using System.Collections.Generic;
using cfg.interact;
using UnityEngine;
using YooAsset;

public class SceneObjects : ItemBase , IInteractableItemReceiver
{
    public cfg.item.SceneObjects data;

    public override void InitItem(int id, TrackerData trackerData = null)
    {
        base.InitItem(id,trackerData);
        ItemType = GameItemType.SceneObject;
        try
        {
            ItemData = GameTableDataAgent.SceneObjectsTable.Get(id);
            data = ItemData as cfg.item.SceneObjects;
            ItemID = data.ID;
        }
        catch (Exception e)
        {
            Debug.LogError("场景物品ID" + ItemID +"不存在，物品名称" + gameObject.name);
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
    
    // Interact
    public void OnPlayerFocus()
    {
        
    }

    public void OnPlayerDefocus()
    {
        
    }

    public MonoBehaviour getMonoBehaviour()
    {
        return this;
    }

    public void OnPlayerStartInteract()
    {
        
    }

    public void OnPlayerInteract()
    {
        
    }

    public void OnPlayerInteractCancel()
    {

    }

    public bool hasInteraction(int itemid) // interacted Item Id;
    {
        return false;
    }
}
