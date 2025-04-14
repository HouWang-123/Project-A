using System;
using UnityEngine;
using YooAsset;

public class TinyItem : ItemBase , IDataItem
{
    public cfg.item.TinyObjects data;
    
    // 可能存在的抽象方法，子类实现方法体
    
    // 物品初始化

    public override void InitItem(int id,TrackerData trackerData = null)
    {
        base.InitItem(id,trackerData);
        ItemType = GameItemType.TinyItem;

        try
        {
            ItemData = GameTableDataAgent.TinyObjectsTable.Get(id);
            data = ItemData as cfg.item.TinyObjects;
            ItemID = data.ID;
        }
        catch (Exception e)
        {
            Debug.LogError("小物品ID" + id +"不存在，物品名称" + gameObject.name);
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
    
    public void OnItemGet()
    {
        GameRunTimeData.Instance.InventoryManger.OnGetItem(ItemID,1);
    }

    public override void OnItemPickUp()
    {
        OnItemGet();
        Destroy(gameObject);
    }
}
