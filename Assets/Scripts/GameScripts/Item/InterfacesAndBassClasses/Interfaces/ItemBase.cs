using System;
using DG.Tweening;
using Spine.Unity.Examples;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class ItemBase : MonoBehaviour, IPickUpable
{
    public int ItemID; // 物品ID
    [HideInInspector] public GameItemType ItemType; // 物品类型
    [Space] public SpriteRenderer ItemRenderer; // 物品目标渲染
    public Shader oulineShader; // 选中时的 shader
    public Shader DefaultSpriteShader; // 默认     shader
    public TextMeshPro StackNuberText;
    
    private bool Targeted; // 是否被拾取系统选中
    private bool ItemReversed;
    
    public bool DropState;
    Vector3 OriginalRendererScale;
    
    public int StackCount = 1;
    
    public void Start()
    {
        var RendererTr = ItemRenderer.transform;
        RendererTr.localEulerAngles = GameConstData.DefAngles;
        OriginalRendererScale = RendererTr.localScale;
        OnItemDrop(false);
        
        if (this is IStackable)
        {
            HideStackNumber();
            if (StackCount > 1)
            {
                IStackable stackable = this as IStackable;
                stackable.ChangeStackCount(StackCount);
                ShowStackNumber();
            }
        }
    }

    public void HideStackNumber()
    {
        StackNuberText.gameObject.SetActive(false);
    }

    public void ShowStackNumber()
    {
        StackNuberText.gameObject.SetActive(true);
    }
    public void DisableRenderer()
    {
        ItemRenderer.enabled = false;
    }

    public void EnableRenderer()
    {
        ItemRenderer.enabled = true;
    }

    private bool IsItemInPickupRange; // 可否拾取

    public void SetPickupable(bool v)
    {
        IsItemInPickupRange = v;
    }

    public void SetTargerted(bool v) // 拾取系统相关功能，与拾取标识相关
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

    protected abstract void InitItem(); // 物品数据初始化
    protected abstract void InitItem(int id); // 物品数据初始化
    public abstract Sprite GetItemIcon();
    public abstract string GetPrefabName();

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
            Vector3 origin = transform.position;
            bool isGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance, GameRoot.Instance.FloorLayer);

            if (!isGrounded)
            {
                velocity.y += gravity * Time.deltaTime;
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                velocity = Vector3.zero;
                RaycastHit hit;
                if (Physics.Raycast(origin, Vector3.down, out hit, groundCheckDistance, GameRoot.Instance.FloorLayer))
                {
                    transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                }

                DropState = false;
            }
        }
        if (transform.position.y < 0)
        {
            DropState = false;
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }
    public virtual void OnItemPickUp() { }
    public virtual void OnItemDrop(bool fastDrop)
    {
        DropState = true;
        transform.DOLocalRotate(Vector3.zero, 0f);
        if (fastDrop)
        {
            Vector3 groundLocation = transform.position;
            groundLocation.y = 0;
            DropState = false;
        }
    }
    public abstract void OnRightInteract( );
    public abstract void OnLeftInteract( );
}