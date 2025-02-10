using System;
using DG.Tweening;
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
    private bool ItemReversed;
    public bool DropState;
    public void Start()
    {
        PickupTip.gameObject.SetActive(false);
        ItemRenderer.transform.localEulerAngles = GameConstData.DefAngles;
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
    // 拾取接口相关控制
    public void CheckReverse(bool reversed)
    {
        if (reversed && !ItemReversed)
        {
            transform.localScale = GameConstData.ReverseScale;
            ItemReversed = true;
        }

        if (!reversed && ItemReversed)
        {
            transform.localScale = Vector3.one;
            ItemReversed = false;
        }
    }          
    protected abstract void InitItem();           // 物品数据初始化
    
    //dorp Item Test

    // 重力加速度（负数表示向下）
    public float gravity = -9.81f;
    // 检测射线的长度（根据物体尺寸和实际需求调整）
    public float groundCheckDistance = 0.2f;
    // 只检测“floor”层级，确保 Inspector 中 groundLayer 只包含这一层
    public LayerMask groundLayer;
    
    // 物体当前的速度（包括垂直方向）
    private Vector3 velocity = Vector3.zero;

    public void FixedUpdate()
    {
        if (DropState)
        {
            // 以物体当前位置为射线原点（如果物体中心不合适，可加上一个偏移量，如物体底部）
            Vector3 origin = transform.position;
            // 发射一条向下的射线检测地面
            bool isGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance, groundLayer);
        
            if (!isGrounded)
            {
                // 如果未接触到地面，则累加重力（模拟自由落体）
                velocity.y += gravity * Time.deltaTime;
                // 更新物体位置
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                // 如果检测到地面，则重置速度
                velocity = Vector3.zero;
                // 可选：通过射线获取准确的地面高度，将物体位置修正到地面
                RaycastHit hit;
                if (Physics.Raycast(origin, Vector3.down, out hit, groundCheckDistance, groundLayer))
                {
                    transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                }
                DropState = false;
            }
        }
    }

    public virtual void OnItemPickUp()
    {
        
    }

    [ContextMenu("Drop")]
    public virtual void OnItemDrop()
    {
        DropState = true;
        transform.DOLocalRotate(Vector3.zero, 0f);
    }
}
