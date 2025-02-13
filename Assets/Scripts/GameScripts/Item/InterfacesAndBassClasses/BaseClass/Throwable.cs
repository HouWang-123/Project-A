using System;
using UnityEngine;

public abstract class Throwable : ItemBase
{
    public cfg.item.ThrowObjects ItemData;
    
    public Rigidbody throwRigid;
    
    public void Awake()
    {
        InitItem();
    }
    // 可能存在的抽象方法，子类实现方法体
    protected abstract void Throw();
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

    public override void OnItemPickUp()
    {
        base.OnItemPickUp();
        throwRigid.Sleep();
    }
}
