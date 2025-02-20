using System;
using UnityEngine;
using YooAsset;

public class Throwable : ItemBase
{
    public cfg.item.ThrowObjects data;
    public Rigidbody ThrowableRigidbody;

    // 物品初始化
    public override void InitItem(int id)
    {
        ItemType = GameItemType.Throwable;

        try
        {
            ItemData = GameTableDataAgent.ThrowObjectsTable.Get(id);
            data = ItemData as cfg.item.ThrowObjects;
            ItemID = data.ID;
        }
        catch (Exception e)
        {
            ColorfulDebugger.DebugError("可投掷物品ID" + id +"不存在，物品名称" + gameObject.name,ColorfulDebugger.Instance.Data);
        }
        ItemSpriteName = data.SpriteName;
    }
    public override void OnItemPickUp()
    {
        base.OnItemPickUp();
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
    
    
    public override void OnRightInteract()
    {
        Debug.Log("投掷右键");
    }

    public override void OnLeftInteract()
    {
        Debug.Log("投掷左键");
    }
}
