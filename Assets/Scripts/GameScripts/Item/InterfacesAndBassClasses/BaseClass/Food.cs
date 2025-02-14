using System;
using cfg.interact;

public class Food : ItemBase
{
    public cfg.item.Food ItemData;
    public void Awake()
    {
        InitItem();
    }
    // 可能存在的抽象方法，子类实现方法体
    public void Eat()
    {
        
    }
    
    // 物品初始化
    protected override void InitItem()
    {
        ItemType = GameItemType.Food;
        try
        {
            ItemData = GameTableDataAgent.FoodTable.Get(ItemID);
        }
        catch (Exception e)
        {
            ColorfulDebugger.DebugError("食物ID" + ItemID +"不存在，物品名称" + gameObject.name,ColorfulDebugger.Instance.Data);
        }
    }
}
