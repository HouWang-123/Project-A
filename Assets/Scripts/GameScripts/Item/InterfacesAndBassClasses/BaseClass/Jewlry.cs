using System;

public class Jewlry : ItemBase
{
    public cfg.item.Jewelry ItemData;
    public void Awake()
    {
        InitItem();
    }
    // 可能存在的抽象方法，子类实现方法体
    public void Equip()
    {
        
    }
    // 物品初始化
    protected override void InitItem()
    {
        ItemType = GameItemType.Jewelry;
        
        try
        {
            ItemData = GameTableDataAgent.JewelryTable.Get(ItemID);
        }
        catch (Exception e)
        {
            ColorfulDebugger.DebugError("饰品ID" + ItemID +"不存在，物品名称" + gameObject.name,ColorfulDebugger.Instance.Data);
        }
    }
}
