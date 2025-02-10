
using UnityEngine;
/// <summary>
/// 可是拾取接口
/// </summary>
public interface IPickUpable
{
    void OnItemPickUp();
    void OnItemDrop();
}
public class ItemBase : MonoBehaviour , IPickUpable
{
    public GameObject PlayerHoldPoint; // 拾取物品逻辑
    private bool canbePickup;
    private bool isTargeted;
    public void setCanBePickUp( bool v)
    {
        canbePickup = v;
    }

    public void setTargerted(bool v)
    {
        isTargeted = v;
    }
    public virtual void OnItemPickUp()
    {
    }

    public virtual void  OnItemDrop()
    {
    }
}
