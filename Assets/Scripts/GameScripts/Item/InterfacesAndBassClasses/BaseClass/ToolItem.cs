using System;
using UnityEngine;
using UnityEngine.Serialization;
using YooAsset;

public class Tool : ItemBase, ISlotable
{
    public cfg.item.Tools data;
    // 物品初始化
    public ToolBehaviour m_ToolBehaviour;
    public LightBehaviour m_LightBehaviour;
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
        m_ToolBehaviour = GetComponent<ToolBehaviour>();
        if (m_ToolBehaviour is LightBehaviour)
        {
            m_LightBehaviour = m_ToolBehaviour as LightBehaviour;
        }
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

    public int GetItemId()
    {
        return ItemID;
    }

    public override void OnItemPickUp()
    {
        base.OnItemPickUp();
        if (m_LightBehaviour != null)
        {
            m_LightBehaviour.LightOn();
        }
    }

    public override void OnItemDrop(bool fastDrop, bool IgnoreBias = false)
    {
        base.OnItemDrop(fastDrop, IgnoreBias);
        if (m_LightBehaviour != null)
        {
            m_LightBehaviour.LightOff();
        }
    }
}
