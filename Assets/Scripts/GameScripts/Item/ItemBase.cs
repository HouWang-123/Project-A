
using System;
using UnityEngine;
/// <summary>
/// 可是拾取接口
/// </summary>
public interface IPickUpable
{
    void OnItemPickUp();
    void OnItemDrop();
}
public abstract class ItemBase : MonoBehaviour , IPickUpable
{
    public GameObject PickupTip;
    public GameObject PlayerHoldPoint; // 拾取物品逻辑
    private bool canbePickup;
    private bool isTargeted;

    public void Start()
    {
        PickupTip.gameObject.SetActive(false);
    }
    
    public void setCanBePickUp( bool v)
    {
        canbePickup = v;
    }

    public void setTargerted(bool v)
    {
        isTargeted = v;
        if (isTargeted)
        {
            PickupTip.gameObject.SetActive(true);
        }
        else
        {
            PickupTip.gameObject.SetActive(false);
        }

    }

    public abstract void OnItemPickUp();// 拾取
    public abstract void OnItemDrop(); // 丢弃
    public abstract void InitItem();

}
