using System;
using UnityEngine;
using UnityEngine.Serialization;
using YooAsset;

public class Tool : ItemBase, ISlotable
{
    public cfg.item.Tools data;
    // 物品初始化
    public ToolBehaviour m_ToolBehaviour;
    public FlashLightBehaviour m_LightBehaviour;

    protected ToolStatus ToolStatus
    {
        set
        {
            _toolStatus = value;
            if (_toolStatus.ToolOn)
            {
                m_LightBehaviour.LightOn();
                ToolStatus.ToolOn = true;
            }
            else
            {
                m_LightBehaviour.LightOff();
                ToolStatus.ToolOn = false;
            }
        }
        get
        {
            return _toolStatus;
        }
    }

    private ToolStatus _toolStatus;
    private float SwitchCD = 0.2f;
    
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
        if (m_ToolBehaviour is FlashLightBehaviour)
        {
            m_LightBehaviour = m_ToolBehaviour as FlashLightBehaviour;
        }
        _toolStatus = new ToolStatus();
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

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        SwitchCD_2 += Time.deltaTime;
        if (Mathf.Abs(SwitchCD - SwitchCD_2) > 0.2f)
        {
            SwitchCD_2 = 0f;
            SwitchCD = 0f;
            actioned = false;
        }
    }

    private float SwitchCD_2;
    private bool actioned = false;
    public override void OnLeftInteract( )
    {
        SwitchCD += Time.deltaTime;
        if (actioned)
        {
            return;
        }
        if (m_LightBehaviour != null)
        {
            actioned = true;
            SwitchCD_2 = SwitchCD;
            if (m_LightBehaviour.isOn)
            {
                m_LightBehaviour.LightOff();
                ToolStatus.ToolOn = false;
            }
            else
            {
                m_LightBehaviour.LightOn();
                ToolStatus.ToolOn = true;
            }
        }
    }
    public int GetItemId()
    {
        return ItemID;
    }

    public override void SetItemStatus(ItemStatus itemStatus)
    {
        ToolStatus = itemStatus as ToolStatus;
    }

    public override ItemStatus GetItemStatus()
    {
        return _toolStatus;
    }
}
