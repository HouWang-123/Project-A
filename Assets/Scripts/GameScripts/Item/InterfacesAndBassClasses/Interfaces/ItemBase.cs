using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using YooAsset;
using Random = UnityEngine.Random;
///
///================================！继承前请阅读所有代码！===================================
///
/// <summary>
/// ！！！！！！！请不要直接继承ItemBase，否则相当于为游戏添加了一种类型的游戏品类！！！！！！！
///
///功能：
///  1. 可以实现基础的拾取，举起和交互行为逻辑
///  2. 可以实现物品与物品之间的交互逻辑
///  3. 合理重写接口来实现特定功能
/// 
/// </summary>
///
///================================！继承前请阅读所有代码！===================================
/// 
public abstract class ItemBase : MonoBehaviour, ITrackable
{
    protected Luban.BeanBase ItemData;
    public int ItemID; // 物品ID
    [HideInInspector] public GameItemType ItemType; // 物品类型
    [Space] public SpriteRenderer ItemRenderer; // 物品目标渲染
    public Shader oulineShader; // 选中时的 shader
    public Shader DefaultSpriteShader; // 默认     shader
    public TextMeshPro StackNuberText; // 显示堆叠数量的TMP
    public String ItemSpriteName;      //
    public bool PickUpTargeted; // 是否被拾取系统选中
    public bool DropState;      // 物品是否处于掉落状态
    public int StackCount = 1;  // 物品堆叠数量，仅实现了 IStackable的ItemBase适用
    private bool ItemReversed;
    private bool ignoreAngleCorrect;

    protected bool IgnoreDefaultItemDrop;

    private GameItemPickupTip pickupTips;
    protected bool IsholdByPlayer;
    protected ItemStatus MyItemStatus;
    private Vector3 originalInitPosition;
    protected float startactionTime;
    
    public virtual ItemStatus GetItemStatus()
    {
        return MyItemStatus;
    }

    public virtual void SetItemStatus(ItemStatus itemStatus)
    {
        MyItemStatus = itemStatus;
    }

    private void SetRendererImage()
    {
        AssetHandle loadAssetSync;
        if (String.IsNullOrEmpty(ItemSpriteName)) return;
        loadAssetSync = YooAssets.LoadAssetSync<Sprite>(ItemSpriteName);
        if (loadAssetSync.AssetObject == null)
        {
            loadAssetSync = YooAssets.LoadAssetSync<Sprite>("SpriteNotFound_Default");
        }

        ItemRenderer.sprite = loadAssetSync.AssetObject as Sprite;
        loadAssetSync.Release();
    }
    
    protected virtual void Start()
    {
        if (!ignoreAngleCorrect)
        {
            if (ItemRenderer != null)
            {
                ItemRenderer.transform.localEulerAngles = GameConstData.DefAngles;
            }
        }

        if (ItemData == null)
        {
            InitItem(ItemID); // 非动态生成的物品，拖拽进入的物品
        }

        GenerateItemStatus();
        CheckIsStackedItem();
        SetRendererImage();
        if (!IsholdByPlayer)
        {
            RegisterTracker();
        }
    }

    public void SetRendererAngle()
    {
        ItemRenderer.transform.localEulerAngles = GameConstData.DefAngles;
    }

    protected virtual void GenerateItemStatus()
    {
        if (MyItemStatus != null) return;
        MyItemStatus = new ItemStatus();
    }

    protected virtual void OnDestroy()
    {
        if (pickupTips != null)
        {
            pickupTips.OnItemPicked();
        }

        UnRegisterTracker();
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
        MyItemStatus.StackCount = StackCount;
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

    private void LoadPickupTipUI()
    {
        AssetHandle loadAssetAsync = YooAssets.LoadAssetAsync<GameObject>("P_UI_WorldUI_ItemPickupTip");
        loadAssetAsync.Completed += (loadAssetAsync) =>
        {
            GameObject objAssetObject = loadAssetAsync.AssetObject as GameObject;
            GameObject instantiate = Instantiate(objAssetObject);
            instantiate.transform.position = transform.position;
            instantiate.transform.position += new Vector3(0, 1f, 0);
            instantiate.transform.localEulerAngles = GameConstData.DefAngles;
            instantiate.transform.SetParent(transform);
            pickupTips = instantiate.GetComponent<GameItemPickupTip>();
            pickupTips.PlayInitAnimation();
        };
    }

    // 物品拾取器，shader
    public virtual void SetTargerted(bool v) // 拾取系统相关功能，与拾取标识相关
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

            LoadPickupTipUI();
        }
        else
        {
            ItemRenderer.material.shader = DefaultSpriteShader;
            if (pickupTips != null) pickupTips.OnDetargeted();
        }
    }

    public void ChangeRendererSortingOrder(int OrderNumber)
    {
        ItemRenderer.sortingOrder = OrderNumber;
    }

    // 动态生成时必须调用一次初始化，用来设置物品数据
    public virtual void InitItem(int id, TrackerData trackerdata = null)
    {
    }

    public abstract Sprite GetItemIcon();
    public abstract string GetPrefabName();
    private float gravity = -9.81f;
    private float upspeed = 20f;
    private float damp = 60f;
    private float groundCheckDistance = 0.2f;
    private Vector3 velocity = GameConstData.VthrowSpeed;
    private float H_BiasSpeed;
    // 持续保持原位时间
    private float ttl = 0f;
    // 物品掉落相关物理逻辑
    protected virtual void FixedUpdate()
    {
        startactionTime += Time.deltaTime;
        ttl += Time.deltaTime;
        F_Update_ItemDorp();
        F_UpdateWeaponCDRecover();
    }

    protected virtual void F_UpdateWeaponCDRecover()
    {
    }

    public Action OnDropCallback;

    protected virtual void F_Update_ItemDorp()
    {
        if (IgnoreDefaultItemDrop) return;

        if (DropState)
        {
            SpeedDamp();
            Vector3 origin = transform.position;
            bool isGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance, GameRoot.Instance.FloorLayer);
            if (!isGrounded)
            {
                velocity.y += (gravity + upspeed) * Time.deltaTime;
                velocity.x += H_BiasSpeed * Time.deltaTime;
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
        startactionTime = 0;
        if (pickupTips != null) pickupTips.OnItemPicked();
        IsholdByPlayer = true;
        UnRegisterTracker();
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

        IsholdByPlayer = false;
        H_BiasSpeed = Random.Range(-10, 10);
        if (IgnoreBias) H_BiasSpeed = 0f;
        DropState = true;
        if (fastDrop)
        {
            Vector3 groundLocation = transform.position;
            groundLocation.y = 0;
            DropState = false;
        }

        RegisterTracker();
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
                if (H_BiasSpeed < 0) H_BiasSpeed = 0;
            }

            if (H_BiasSpeed < 0)
            {
                H_BiasSpeed += damp * Time.deltaTime;
                if (H_BiasSpeed > 0) H_BiasSpeed = 0;
            }
        }
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == GameRoot.Instance.FloorLayer) H_BiasSpeed = 0;
    }

    /// <summary>
    /// 场景切换持久化实例状态示例DEMO
    /// </summary>
    /// <returns></returns>
    public TrackerData CollectTrackedData()
    {
        return new TrackerData(
            ItemID,
            TrackType.Item,
            new Vector3(transform.position.x, transform.position.y, transform.position.z),
            transform.eulerAngles,
            transform.localScale,
            MyItemStatus
        );
    }

    public void RegisterTracker()
    {
        GameRunTimeData.Instance.MapTrackDataManager.RegisterTracker(this);
    }

    public void UnRegisterTracker()
    {
        GameRunTimeData.Instance.MapTrackDataManager.UnRegisterTracker(this);
    }

    [GUIColor(0.3f, 0.8f, 0.8f, 1f)] public int ChangeToItemId;

    [Button("转换到其他物品")]
    [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
    private void ChangeToOtherItem()
    {
        ChangeToItem(ChangeToItemId);
    }

    /// <summary>
    /// 回调表示对生成后的物体做后续操作
    /// </summary>
    /// <param name="id"></param>
    /// <param name="callback"></param>
    public void ChangeToItem(int id, Action<ItemBase> callback = null)
    {
        GameItemTool.GenerateItemAtPosition(id, transform.parent, transform.position,
            res: (item) =>
            {
                callback?.Invoke(item);
                Destroy(gameObject);
        });
    }
}