using System;
using UnityEngine;
using YooAsset;

public class Throwable : ItemBase
{
    public cfg.item.ThrowObjects ItemData;
    public Rigidbody ThrowableRigidbody;
    
    // 可能存在的抽象方法，子类实现方法体
    protected void Throw()
    {
        
    }
    // 物品初始化
    public override void InitItem(int id)
    {
        ItemType = GameItemType.Throwable;

        try
        {
            ItemData = GameTableDataAgent.ThrowObjectsTable.Get(id);
            ItemID = ItemData.ID;
        }
        catch (Exception e)
        {
            ColorfulDebugger.DebugError("可投掷物品ID" + id +"不存在，物品名称" + gameObject.name,ColorfulDebugger.Instance.Data);
        }
        ItemSpriteName = ItemData.SpriteName;
    }
    public override void OnItemPickUp()
    {
        base.OnItemPickUp();
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
    
    
    public override void OnRightInteract()
    {
        Debug.Log("投掷右键");
    }

    public override void OnLeftInteract()
    {
        Debug.Log("投掷左键");
    }
}
