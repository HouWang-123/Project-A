using System;
using UnityEngine;
using YooAsset;

public class Throwable : ItemBase
{
    public cfg.item.ThrowObjects ItemData;
    public Rigidbody ThrowableRigidbody;
    public void Awake()
    {
        InitItem();
    }
    // 可能存在的抽象方法，子类实现方法体
    protected void Throw()
    {
        
    }
    // 物品初始化
    protected override void InitItem()
    {
        ItemType = GameItemType.Throwable;

        try
        {
            ItemData = GameTableDataAgent.ThrowObjectsTable.Get(ItemID);
        }
        catch (Exception e)
        {
            ColorfulDebugger.DebugError("可投掷物品ID" + ItemID +"不存在，物品名称" + gameObject.name,ColorfulDebugger.Instance.Data);
        }
    }
    protected override void InitItem(int id)
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
    }
    public override void OnItemPickUp()
    {
        base.OnItemPickUp();
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
    
    
    public override void OnRightInteract()
    {
        throw new NotImplementedException();
    }

    public override void OnLeftInteract()
    {
        throw new NotImplementedException();
    }
}
