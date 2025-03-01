using System;
using UnityEngine;
using YooAsset;

public class Tool : ItemBase, ISlotable
{
    public cfg.item.Tools data;
    // 物品初始化

    public override void InitItem(int id)
    {
        ItemType = GameItemType.ToolItem;
        ItemData = GameTableDataAgent.ToolsTable.Get(id);
        try
        {
            ItemData = GameTableDataAgent.ToolsTable.Get(id);
            data = ItemData as cfg.item.Tools;
            ItemID = data.ID;
        }
        catch (Exception e)
        {
            ColorfulDebugger.DebugError("工具物品ID" + id +"不存在，物品名称" + gameObject.name,ColorfulDebugger.Instance.Data);
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

    public int GetItemId()
    {
        return ItemID;
    }
}
