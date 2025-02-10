using System;
using UnityEngine;

public abstract class Weapon : ItemBase
{
    public int id;
    private cfg.item.Weapon ItemData;
    private void Awake()
    {
        ItemData = GameTableDataAgent.WeaponTable.Get(id);
        InitItem();
    }
    public abstract void Fire();
    public override void InitItem()
    {
        Debug.Log(id);
    }
}
