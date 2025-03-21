using System;
using TMPro;
using UnityEngine;
using YooAsset;
using Random = UnityEngine.Random;

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
    public bool PickUpTargeted; // 是否被拾取系统选中
    private bool ItemReversed;
    private bool ignoreAngleCorrect;
    public bool DropState;
    public bool IgnoreDefaultItemDrop;
    public int StackCount = 1;
    private GameItemPickupTip pickupTips;
    protected bool IsholdByPlayer;
    
    public virtual ItemStatus GetItemStatus()
    {
        return null;
    }

    public virtual void SetItemStatus(ItemStatus itemStatus)
    {
    }

    private void SetRendererImage()
    {
        AssetHandle loadAssetSync;
        loadAssetSync = YooAssets.LoadAssetSync<Sprite>(ItemSpriteName);
        if (loadAssetSync.AssetObject == null)
        {
            loadAssetSync = YooAssets.LoadAssetSync<Sprite>("SpriteNotFound_Default");
        }

        ItemRenderer.sprite = loadAssetSync.AssetObject as Sprite;
        loadAssetSync.Release();
    }

    protected void Start()
    {
        IgnoreDefaultItemDrop = false;
        _collider = GetComponentInChildren<Collider2D>();
        if (!ignoreAngleCorrect)
        {
            ItemRenderer.transform.localEulerAngles = GameConstData.DefAngles;
        }

        if (ItemData == null)
        {
            InitItem(ItemID); // 非动态生成的物品，拖拽进入的物品
        }

        CheckIsStackedItem();
        SetRendererImage();
    }

    public void OnDestroy()
    {
        GameRunTimeData.Instance.ItemManager.UnRegistItem(this);
        if (pickupTips != null)
        {
            pickupTips.OnItemPicked();
        }
    }

    // 新添加接口，通过设置id定义物品
    public void SetItemId(int id)
    {
        InitItem(id);
        SetRendererImage();
    }

    public void SetIgnoreAngle(bool val)
    {
        ignoreAngleCorrect = val;
    }

    private void CheckIsStackedItem()
    {
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

    public bool IsItemInPickupRange; // 可否拾取

    public void SetPickupable(bool v)
    {
        IsItemInPickupRange = v;
    }

    // 物品拾取器，shader
    public void SetTargerted(bool v) // 拾取系统相关功能，与拾取标识相关
    {
        PickUpTargeted = v;
        if (PickUpTargeted)
        {
            if (DropState) return;
            if (IsholdByPlayer) return;
            ItemRenderer.material.shader = oulineShader;
            if (pickupTips != null)
            {
                pickupTips.PlayInitAnimation();
                return;
            }
            
            AssetHandle loadAssetAsync = YooAssets.LoadAssetAsync<GameObject>("P_UI_WorldUI_ItemPickupTip");
            loadAssetAsync.Completed += (loadAssetAsync) =>
            {
                GameObject objAssetObject = loadAssetAsync.AssetObject as GameObject;

                GameObject instantiate = Instantiate(objAssetObject);

                instantiate.transform.position = transform.position;
                instantiate.transform.position += new Vector3(0, 1f, 0);
                instantiate.transform.localEulerAngles = GameConstData.DefAngles;
                pickupTips = instantiate.GetComponent<GameItemPickupTip>();
                pickupTips.PlayInitAnimation();
            };
        }
        else
        {
            ItemRenderer.material.shader = DefaultSpriteShader;
            if (pickupTips != null)
            {
                pickupTips.OnDetargeted();
            }
        }
    }

    public void ChangeRendererSortingOrder(int OrderNumber)
    {
        ItemRenderer.sortingOrder = OrderNumber;
    }


    // 动态生成时必须调用一次初始化，用来设置物品数据
    public abstract void InitItem(int id); // 物品数据初始化
    public abstract Sprite GetItemIcon();
    public abstract string GetPrefabName();

    private float gravity = -9.81f;
    private float upspeed = 20f;
    private float damp = 60f;
    private float groundCheckDistance = 0.2f;
    private Vector3 velocity = GameConstData.VthrowSpeed;
    private float H_BiasSpeed;

    private float V_BiasSpeed;
    private Collider2D _collider;

    // 物品掉落相关物理逻辑
    protected virtual void FixedUpdate()
    {
        F_Update_ItemDorp();
        F_UpdateWeaponCDRecover();
    }

    protected virtual void F_UpdateWeaponCDRecover()
    {
    }

    public Action OnDropCallback;

    private void F_Update_ItemDorp()
    {
        if (IgnoreDefaultItemDrop)
        {
            return;
        }
        if (DropState)
        {
            SpeedDamp();
            Vector3 origin = transform.position;
            bool isGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance, GameRoot.Instance.FloorLayer);
            if (!isGrounded)
            {
                velocity.y += (gravity + upspeed) * Time.deltaTime;
                velocity.x += H_BiasSpeed * Time.deltaTime;
                velocity.z += V_BiasSpeed * Time.deltaTime;
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                velocity = GameConstData.VthrowSpeed;
                RaycastHit hit;
                if (Physics.Raycast(origin, Vector3.down, out hit, groundCheckDistance, GameRoot.Instance.FloorLayer))
                {
                    var position = transform.position;
                    position = new Vector3(position.x, hit.point.y, position.z);
                    transform.position = position;
                }

                DropState = false;
                upspeed = 20f;
                OnDropCallback?.Invoke();
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
    public virtual void OnItemPickUp()
    {
        if (pickupTips != null)
        {
            pickupTips.OnItemPicked();
        }
    }

    public virtual void OnItemDrop(bool fastDrop, bool IgnoreBias = false, bool Playerreversed = false)
    {
        if (Playerreversed)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        H_BiasSpeed = Random.Range(-10, 10);
        V_BiasSpeed = 0;
        if (IgnoreBias)
        {
            H_BiasSpeed = 0f;
            V_BiasSpeed = 0f;
        }

        DropState = true;

        if (fastDrop)
        {
            Vector3 groundLocation = transform.position;
            groundLocation.y = 0;
            DropState = false;
        }
    }

    // 交互逻辑
    // Fixed Update 调用
    public virtual void OnRightInteract()
    {
    }

    // Fixed Update 调用
    public virtual void OnLeftInteract()
    {
    }

    /// 运动阻力
    private void SpeedDamp()
    {
        // UpSpeed
        if (upspeed > 0)
        {
            upspeed -= damp * Time.deltaTime;
        }

        // LeftOrRight
        if (Math.Abs(H_BiasSpeed) > 0f)
        {
            if (H_BiasSpeed > 0)
            {
                H_BiasSpeed -= damp * Time.deltaTime;
                if (H_BiasSpeed < 0)
                {
                    H_BiasSpeed = 0;
                }
            }

            if (H_BiasSpeed < 0)
            {
                H_BiasSpeed += damp * Time.deltaTime;
                if (H_BiasSpeed > 0)
                {
                    H_BiasSpeed = 0;
                }
            }
        }

        // Upper Or Lower
        if (Math.Abs(V_BiasSpeed) > 0f)
        {
            if (V_BiasSpeed > 0)
            {
                V_BiasSpeed -= damp * Time.deltaTime;
                if (V_BiasSpeed < 0)
                {
                    V_BiasSpeed = 0;
                }
            }

            if (V_BiasSpeed < 0)
            {
                V_BiasSpeed += damp * Time.deltaTime;
                if (V_BiasSpeed > 0)
                {
                    V_BiasSpeed = 0;
                }
            }
        }
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == GameRoot.Instance.FloorLayer)
        {
            H_BiasSpeed = 0;
            V_BiasSpeed = 0;
        }
    }
}