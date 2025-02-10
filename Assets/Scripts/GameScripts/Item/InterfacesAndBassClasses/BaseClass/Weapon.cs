using System;
using Spine.Unity.Examples;
using UnityEngine;

public abstract class Weapon : ItemBase
{
    public cfg.item.Weapon ItemData;
    private void Awake()
    {
        InitItem();
    }
    // 可能存在的抽象方法，子类实现方法体
    public abstract void Fire();
    // 物品初始化
    protected override void InitItem()
    {
        ItemType = GameItemType.Weapon;
        try
        {
            ItemData = GameTableDataAgent.WeaponTable.Get(ItemID);
        }
        catch (Exception e)
        {
            ColorfulDebugger.DebugError("武器物品ID" + ItemID +"不存在，物品名称" + gameObject.name,ColorfulDebugger.Instance.Data);
        }
    }
}
