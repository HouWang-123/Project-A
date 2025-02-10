using System;

public abstract class ToolItem : ItemBase
{
    public cfg.item.SceneObjects ItemData;
    public void Awake()
    {
        InitItem();
    }
    // 可能存在的抽象方法，子类实现方法体
    public abstract void Use();
    // 物品初始化
    protected override void InitItem()
    {
        ItemType = GameItemType.ToolItem;
        ItemData = GameTableDataAgent.SceneObjectsTable.Get(ItemID);
        try
        {
            ItemData = GameTableDataAgent.SceneObjectsTable.Get(ItemID);
        }
        catch (Exception e)
        {
            ColorfulDebugger.DebugError("工具物品ID" + ItemID +"不存在，物品名称" + gameObject.name,ColorfulDebugger.Instance.Data);
        }
    }
}
