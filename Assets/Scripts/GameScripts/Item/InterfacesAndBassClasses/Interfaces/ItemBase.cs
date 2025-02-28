using System;
using DG.Tweening;
using Spine.Unity.Examples;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using YooAsset;

public abstract class ItemBase : MonoBehaviour, IPickUpable
{
    protected Luban.BeanBase ItemData;
    public int ItemID; // 物品ID
    [HideInInspector] public GameItemType ItemType; // 物品类型
    [Space] public SpriteRenderer ItemRenderer; // 物品目标渲染
    public Shader oulineShader; // 选中时的 shader
    public Shader DefaultSpriteShader; // 默认     shader
    public TextMeshPro StackNuberText;
    public String ItemSpriteName;
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
        AssetHandle loadAssetSync;
        
        if (ItemData == null)
        {
            InitItem(ItemID); // 非动态生成的物品，拖拽进入的物品
        }
        loadAssetSync = YooAssets.LoadAssetSync<Sprite>(ItemSpriteName);
        if (loadAssetSync.AssetObject == null)
        {
            loadAssetSync = YooAssets.LoadAssetSync<Sprite>("SpriteNotFound_Default");
        }
        ItemRenderer.sprite = loadAssetSync.AssetObject as Sprite;
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
    // 新添加接口，通过设置id定义物品
    public void SetItemId(int id)
    {
        InitItem(id);
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

    public void SetPickupable(bool v) { IsItemInPickupRange = v; }
    // 物品拾取器，shader
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

    public void ChangeRendererSortingOrder(int OrderNumber)
    {
        ItemRenderer.sortingOrder = OrderNumber;
    }
    // 拾取后物品方向控制
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
    // 动态生成时必须调用一次初始化，用来设置物品数据
    public abstract void InitItem(int id); // 物品数据初始化
    public abstract Sprite GetItemIcon();
    public abstract string GetPrefabName();
    
    private float gravity = -9.81f;
    private float groundCheckDistance = 0.2f;
    private Vector3 velocity = Vector3.zero;
    // 物品掉落相关物理逻辑
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
        //防止物品掉出世界
        if (transform.position.y < 0)
        {
            DropState = false;
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }
    // 物品拾取和丢弃
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
    // 交互逻辑
    // Fixed Update 调用
    public abstract void OnRightInteract();
    // Fixed Update 调用
    public abstract void OnLeftInteract();
}