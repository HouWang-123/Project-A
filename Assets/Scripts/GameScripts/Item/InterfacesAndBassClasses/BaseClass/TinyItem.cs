using System;
using UnityEngine;
using YooAsset;

public class TinyItem : ItemBase
{
    public cfg.item.TinyObjects ItemData;
    public void Awake()
    {
        InitItem();
    }
    // 可能存在的抽象方法，子类实现方法体
    
    // 物品初始化
    protected override void InitItem()
    {
        ItemType = GameItemType.TinyItem;

        try
        {
            ItemData = GameTableDataAgent.TinyObjectsTable.Get(ItemID);
        }
        catch (Exception e)
        {
            ColorfulDebugger.DebugError("小物品ID" + ItemID +"不存在，物品名称" + gameObject.name,ColorfulDebugger.Instance.Data);
        }
    }
    protected override void InitItem(int id)
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
    }
    public override Sprite GetItemIcon()
    {
        AssetHandle loadAssetSync = YooAssets.LoadAssetSync<Sprite>(ItemData.IconName);
        return Instantiate(loadAssetSync.AssetObject, transform) as Sprite;
    }
}
