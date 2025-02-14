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
    public Sprite SlotSprite;                        // 新增，道具栏上显示的icon;
    [HideInInspector] public GameItemType ItemType;  // 物品类型
    [Space]
    public SpriteRenderer ItemRenderer;              // 物品目标渲染
    public Shader oulineShader;                      // 选中时的 shader
    public Shader DefaultSpriteShader;               // 默认     shader
    [Space]
    public ItemProperty ItemProperty;                // 物品属性
    private bool IsItemInPickupRange;                // 可否拾取
    private bool Targeted;                           // 是否被拾取系统选中
    private bool ItemReversed;
    
    private int TTL;
    
    public bool DropState;
    Vector3 OriginalRendererScale;
    public void Start()
    {
        var RendererTr = ItemRenderer.transform;
        RendererTr.localEulerAngles = GameConstData.DefAngles;
        OriginalRendererScale = RendererTr.localScale;
    }
    public void SetPickupable( bool v) { IsItemInPickupRange = v; }
    public void setTargerted(bool v)              // 拾取系统相关功能，与拾取标识相关
    {
        Targeted = v;
        if (Targeted)
        {
            ItemRenderer.material.shader = oulineShader;
        }
        else
        {
            ItemRenderer.material.shader = DefaultSpriteShader;
        }
    }
    // 拾取接口相关控制
    public void CheckReverse(bool reversed)
    {
        if (reversed && !ItemReversed)
        {
            Vector3 ReversedScale = OriginalRendererScale;
            ReversedScale.x = -ReversedScale.x;
            ItemRenderer.transform.localScale = ReversedScale;
            ItemReversed = true;
            return;
        }
        if (!reversed && ItemReversed)
        {
            ItemRenderer.transform.localScale = OriginalRendererScale;
            ItemReversed = false;
        }
    }          
    protected abstract void InitItem();           // 物品数据初始化
    
    //dorp Item Test

    // 重力加速度（负数表示向下）
    private float gravity = -9.81f;
    // 检测射线的长度（根据物体尺寸和实际需求调整）
    private float groundCheckDistance = 0.2f;
    
    // 物体当前的速度（包括垂直方向）
    private Vector3 velocity = Vector3.zero;

    public void FixedUpdate()
    {
        if (DropState)
        {
            // 以物体当前位置为射线原点（如果物体中心不合适，可加上一个偏移量，如物体底部）
            Vector3 origin = transform.position;
            // 发射一条向下的射线检测地面
            bool isGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance, GameRoot.Instance.FloorLayer);
        
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
                if (Physics.Raycast(origin, Vector3.down, out hit, groundCheckDistance, GameRoot.Instance.FloorLayer))
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
    public virtual void OnItemDrop(bool fastDrop)
    {
        DropState = true;
        transform.DOLocalRotate(Vector3.zero, 0f);
        if (fastDrop)
        {
            Vector3 groundLocation = transform.position;
            groundLocation.y = 0;
            DropState = true;
        }
    }

    public virtual void OnRightInteract()
    {
        
    }

    public virtual void OnLeftInteract()
    {
        
    }
}
