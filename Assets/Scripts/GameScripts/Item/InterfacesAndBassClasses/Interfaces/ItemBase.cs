using System;
using Spine.Unity.Examples;
using UnityEngine;
using UnityEngine.Serialization;
[Serializable]
public class ItemProperty
{
    public bool Stackable;               // 可否堆叠
    public bool Equipable;               // 可否装备上手
    public bool EquipOnHand;             // 是否正在装备在手上
}
public abstract class ItemBase : MonoBehaviour , IPickUpable
{
    public int ItemID;                               // 物品ID
    [HideInInspector] public GameItemType ItemType;  // 物品类型
    [Space]
    public SpriteRenderer ItemRenderer;              // 物品目标渲染
    public GameObject PickupTip;                     // 拾取提示
    [Space]
    public ItemProperty ItemProperty;                // 物品属性
    private bool IsItemInPickupRange;                // 可否拾取
    private bool Targeted;                           // 是否被拾取系统选中
    public void Start()
    {
        PickupTip.gameObject.SetActive(false);
    }
    public void SetPickupable( bool v) { IsItemInPickupRange = v; }
    public void setTargerted(bool v)              // 拾取系统相关功能，与拾取标识相关
    {
        Targeted = v;
        if (Targeted)
        {
            PickupTip.gameObject.SetActive(true);
        }
        else
        {
            PickupTip.gameObject.SetActive(false);
        }
    }
    public abstract void OnItemPickUp();          // 拾取接口
    public abstract void OnItemDrop();            // 放下接口
    protected abstract void InitItem();           // 物品数据初始化
}
