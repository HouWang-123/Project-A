using UnityEngine;
using YooAsset;
using UnityEngine.Events;
using Spine.Unity;
using System;
using Spine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl Instance;
    public Transform ItemHoldPosition; // 玩家拿取物品位置
    public Transform ItemLiftPostion; // 举起物品的位置
    private PlayerInputControl inputControl;
    private Rigidbody playerRG;
    private Transform playerRenderer;
    private Transform useObjParent;
    private PlayerPickupController _pickupController;
    private bool PlayerReversed;
    private CharacterStat characterStat;
    private PlayerInteractController _interactController;
    private LayerMask FloorBaseLayer;
    public Vector3 PlayerLookatDirection;
    public Vector3 ScreenToWorldPostion;
    
    public bool InteractionButtonPressed;
    
    
    private EPAMoveState moveState = EPAMoveState.Idle;

    public EPAMoveState MoveState
    {
        set
        {
            if (moveState == value) return;
            moveState = value;
            UpdatePlayerAnimatorEnum();
        }
        get { return moveState; }
    }

    private EPAHandState handState = EPAHandState.Default;

    public EPAHandState HandState
    {
        set
        {
            if (handState == value) return;
            handState = value;
            UpdatePlayerAnimatorEnum();
        }
        get { return handState; }
    }

    private EPlayerAnimator animatorEnum = EPlayerAnimator.Idle;

    public EPlayerAnimator PlayerAnimatorEnum
    {
        set
        {
            if (handState == EPAHandState.Default && (animatorEnum == EPlayerAnimator.Run_Blink ||
                                                      animatorEnum == EPlayerAnimator.Walk_Blink))
            {
                return;
            }

            SetPlayerAnimatorEnum(value);
        }
        get { return animatorEnum; }
    }

    private SkeletonAnimation playerSpin;

    //前后移动的速度比率
    private float fToB = 0.6f;

    public Transform ItemReleasePoint;

    private bool isMove = true;
    private bool isDead = false;

    //Action
    private UnityAction leftMouseAction = null;
    private bool leftMous = false;
    private UnityAction rightMouseAction = null;
    private bool rightMous = false;
    private bool shiftButt = false;
    private bool Interact = false;


    private void Awake()
    {
        Instance = this;
        inputControl = new PlayerInputControl();
        isDead = false;
        FloorBaseLayer = LayerMask.GetMask("BaseFloor");
    }

    private void OnEnable()
    {
        inputControl?.Enable();
    }

    private float ScrollActionTimer;

    private void Start()
    {
        #region GamePreSettings

        GameRunTimeData.Instance.CharacterItemSlotData.ChangeFocusSlotNumber(1); // 默认启用道具栏
        characterStat = GameRunTimeData.Instance.CharacterBasicStat.GetStat();

        _pickupController = GetComponentInChildren<PlayerPickupController>();
        _interactController = GetComponentInChildren<PlayerInteractController>();

        playerRG = GetComponent<Rigidbody>();
        playerRenderer = transform.GetChild(0);
        playerRenderer.localEulerAngles = GameConstData.DefAngles;
        useObjParent = transform.GetChild(1);
        ItemHoldPosition = useObjParent.GetChild(0);
        playerSpin = transform.Find("Renderer").GetComponentInChildren<SkeletonAnimation>();
        if (playerSpin != null)
        {
            playerSpin.AnimationState.SetAnimation(0, animatorEnum.ToString(), true);
            playerSpin.AnimationState.Complete += PlayerAnimationComplete;
        }

        // 模拟添加时间段改变的事件
        TimeSystemManager.PhasedChangedEvent phasedEvent = new()
        {
            phase = TimePhaseEnum.Day,
            onTrigger = () =>
            {
                int gameDay = TimeSystemManager.Instance.GameDay;
                int gameHour = TimeSystemManager.Instance.GameHour;
                int gameMinute = TimeSystemManager.Instance.GameMinute;
                // Debug.Log(GetType() + "/ 现在是游戏时间[天数: " + gameDay + " ,小时: " + gameHour + " ,分钟: " + gameMinute + " ]");
                // Debug.Log(GetType() + "/ 触发了主角拉屎");
            }
        };
        TimeSystemManager.Instance.PhasedChangedScheduledEvents.Add(phasedEvent);

        #endregion

        // 分钟改变时间：TimeSystemManager.GameMinuteEvent， 小时改变时间：GameHourEvent

        #region InputSystem

        InputControl.Instance.GamePlayerEnable();
        InputControl.Instance.UIDisable();

        InputControl.Instance.LeftMouse.started += (item) => { leftMous = true; };
        InputControl.Instance.LeftMouse.performed += (item) => { };
        InputControl.Instance.LeftMouse.canceled += (item) => { leftMous = false; };
        InputControl.Instance.RightMouse.started += (item) => { rightMous = true; };
        InputControl.Instance.RightMouse.performed += (item) => { };
        InputControl.Instance.RightMouse.canceled += (item) => { rightMous = false; };

        InputControl.Instance.ShiftButton.started += (item) => { shiftButt = true; };
        InputControl.Instance.ShiftButton.performed += (item) => { };
        InputControl.Instance.ShiftButton.canceled += (item) => { shiftButt = false; };

        InputControl.Instance._1Key.started += (item) => { ChangeMouseAction(1); };
        InputControl.Instance._2Key.started += (item) => { ChangeMouseAction(2); };
        InputControl.Instance._3Key.started += (item) => { ChangeMouseAction(3); };
        InputControl.Instance._4Key.started += (item) => { ChangeMouseAction(4); };
        InputControl.Instance._5Key.started += (item) => { ChangeMouseAction(5); };
        InputControl.Instance._6Key.started += (item) => { ChangeMouseAction(6); };
        InputControl.Instance.MouseScroll.started += (item) =>
        {
            if (characterStat.LiftedItem != null)
            {
                return;
            }

            if (ScrollActionTimer <= 0.1f)
            {
                return;
            }

            Vector2 readValue = item.ReadValue<Vector2>();
            if (readValue.y > 0)
            {
                GameHUD.Instance.ISM_LastFocusItem();
                GameRunTimeData.Instance.CharacterItemSlotData.ChangeFocusSlotNumber(false);
            }
            else if (readValue.y < 0)
            {
                GameHUD.Instance.ISM_NextFocusItem();
                GameRunTimeData.Instance.CharacterItemSlotData.ChangeFocusSlotNumber(true);
            }

            (int, ItemStatus) itemId = GameRunTimeData.Instance.CharacterItemSlotData.GetCurrentFocusedItemId();
            RefreshItemOnHand(itemId);
            ScrollActionTimer = 0f;
        };


        InputControl.Instance.QButton.started += (item) => { };
        InputControl.Instance.QButton.canceled += (item) => { };


        InputControl.Instance.EButton.started += (item) =>
        {
            bool actioned = false;


            if (!actioned)
            {
                actioned = PickItem();
            }
        };
        // 物品丢弃
        InputControl.Instance.GButton.started += (item) =>
        {
            if (characterStat.ItemOnHand != null)
            {
                DropItem(false);
            }

            if (characterStat.LiftedItem != null)
            {
                DropItem(false);
            }
        };
        // 时间加速
        InputControl.Instance.TimeSpeed.started += (item) =>
        {
            Debug.Log(GetType() + "按住了时间加速");
            TimeSystemManager.Instance.TimeSpeed = 5f;
        };
        InputControl.Instance.TimeSpeed.canceled += (item) =>
        {
            Debug.Log(GetType() + "取消时间加速");
            TimeSystemManager.Instance.TimeSpeed = 1f;
        };
        
        // 交互模块
        InputControl.Instance.Interact.started += (item) =>
        {
            Interact = true;
        };
        InputControl.Instance.Interact.performed += (item) => { };
        InputControl.Instance.Interact.canceled += (item) =>
        {
            Interact = false;
        };
        
        #endregion

        #region EventSystem

        EventManager.Instance.RegistEvent<EPAHandState>(EventConstName.PlayerHandItem, SetHandState);
        EventManager.Instance.RegistEvent(EventConstName.PlayerHurtAnimation, PlayerHurt);
        EventManager.Instance.RegistEvent(EventConstName.PlayerOnDeadAnimation, PlayerDead);

        #endregion

        GameHUD.Instance.SetPlayerItemTransform(useObjParent);
    }

    private void FixedUpdate()
    {
        GameRunTimeData.Instance.CharacterBasicStat.UpdatePlayerStat();
        ProcessMove();
        ScrollActionTimer += Time.deltaTime;
        ProcessWeaponNodeRotation();
        ProcessMouseAction();
        CalculatePlayerToMouseDirection();
        ProcessInteract();
    }

    void ProcessInteract()
    {
        if (Interact)
        {
            if (InteractionButtonPressed) return;
            _interactController.InteractItem();    // 确保按下只执行一次
            InteractionButtonPressed = true;
        }
        else
        {
            InteractionButtonPressed = false;
            _interactController.PlayerCancleInteract();
        }
    }

    void CalculatePlayerToMouseDirection()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        bool hit = Physics.Raycast(ray.origin, ray.direction, out var hitinfo, 20000, FloorBaseLayer);
        if (hit)
        {
            ScreenToWorldPostion = hitinfo.point;
        }
        Gizmos.color = Color.magenta;
        ScreenToWorldPostion.y = 0;
        PlayerLookatDirection = (ScreenToWorldPostion - transform.position).normalized;
    }

    private void OnDrawGizmos()
    {
        float lineLength = 1f; // 可自由调整长度
        Vector3 lineEndPoint = transform.position + PlayerLookatDirection * lineLength;

        Gizmos.DrawLine(transform.position, lineEndPoint);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(ScreenToWorldPostion, 0.1f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(PlayerLookatDirection, 0.1f);
    }

    Vector3 u, v, l, a, b;
    float angle;

    private void CalculateUseObjectRotation()
    {
        v = Camera.main.WorldToScreenPoint(transform.position);
        l = InputControl.Instance.GetLook();
        if (l.x < v.x)
        {
            playerRenderer.localScale = GameConstData.XReverseScale;
            PlayerReversed = true;
        }
        else
        {
            playerRenderer.localScale = Vector3.one;
            PlayerReversed = false;
        }

        u = Camera.main.WorldToScreenPoint(useObjParent.position);
        l = l - u;
        l.y *= 1.4f;
        a = -Vector3.up * Mathf.Atan2(l.y, l.x) * Mathf.Rad2Deg;
    }

    public void ChangeMouseAction(int Number)
    {
        if (characterStat.LiftedItem != null)
        {
            Debug.Log("手中存在举起的道具时无法切换物品");
            return;
        }

        GameHUD.Instance.ISM_SetFocus(Number);
        GameRunTimeData.Instance.CharacterItemSlotData.ChangeFocusSlotNumber(Number);
        (int, ItemStatus) currentFocusedItemId = GameRunTimeData.Instance.CharacterItemSlotData.GetCurrentFocusedItemId();
        RefreshItemOnHand(currentFocusedItemId);
    }

    private void ProcessMove()
    {
        if (GameRunTimeData.Instance.CharacterBasicStat.GetStat().Dead) return;
        if (isMove)
        {
            PlayerMove(InputControl.Instance.MovePoint, characterStat.WalkSpeed);
        }

        if (!isDead && !pickupLock)
        {
            CalculateUseObjectRotation();
            useObjParent.localEulerAngles = a;
        }
    }

    private void ProcessWeaponNodeRotation()
    {
        if (GameRunTimeData.Instance.CharacterBasicStat.GetStat().Dead) return;
        // Rotate WeaponTr
        Transform weaponTr = useObjParent.GetChild(0);
        angle = a.y;
        angle %= 360;
        if (angle < 0)
        {
            angle += 360;
        }

        if (angle > 90f && angle <= 270f)
        {
            //-45cos(πx/180)-90
            angle = -45 * Mathf.Cos(Mathf.PI * angle / 180) - 90;
        }
        else
        {
            //-45cos(πx/180)+90
            angle = -45 * Mathf.Cos(Mathf.PI * angle / 180) + 90;
        }

        b = weaponTr.localEulerAngles;
        b.x = angle;
        b.y = 0;
        b.z = 0;
        weaponTr.localEulerAngles = b;
    }

    private void ProcessMouseAction()
    {
        if (GameRunTimeData.Instance.CharacterBasicStat.GetStat().Dead) return;
        // 武器使用相关
        if (leftMous)
        {
            leftMouseAction?.Invoke();
        }

        if (rightMous)
        {
            rightMouseAction?.Invoke();
        }
    }

    private void OnDisable()
    {
        inputControl?.Dispose();
    }

    private void OnDestroy()
    {
        Instance = null;
        EventManager.Instance.RemoveEvent<EPAHandState>(EventConstName.PlayerHandItem, SetHandState);
        EventManager.Instance.RemoveEvent(EventConstName.PlayerHurtAnimation, PlayerHurt);
        EventManager.Instance.RemoveEvent(EventConstName.PlayerOnDeadAnimation, PlayerDead);
    }

    private bool stopmove = false;

    private void PlayerMove(Vector3 vector, float speed)
    {
        vector.z = vector.y;
        vector.y = 0;
        if ((vector - Vector3.zero).sqrMagnitude > 0.01)
        {
            if ((vector.x > 0 && playerRenderer.localScale.x < 0) || (vector.x < 0 && playerRenderer.localScale.x > 0))
            {
                speed *= fToB;
                MoveState = EPAMoveState.Walk_Backwards;
                //playerSpin.timeScale = fToB;              //匹配动画速度
                characterStat.IsWalk = true;
                characterStat.IsRun = false;
            }
            else if (shiftButt)
            {
                speed *= characterStat.RunSpeedScale;
                MoveState = EPAMoveState.Run;
                playerSpin.timeScale = speed * 0.6f; //匹配动画速度
                characterStat.IsWalk = false;
                characterStat.IsRun = true;
            }
            else
            {
                MoveState = EPAMoveState.Walk;
                playerSpin.timeScale = speed / 0.6f; //匹配动画速度
                characterStat.IsWalk = true;
                characterStat.IsRun = false;
            }

            //playerRG.Move(vector * speed + transform.position, Quaternion.identity);
        }
        else
        {
            MoveState = EPAMoveState.Idle;
            characterStat.IsWalk = false;
            characterStat.IsRun = false;
        }

        if (playerRG != null && !stopmove)
        {
            playerRG.linearVelocity = vector * speed;
        }
    }

    private bool dropKeyPressed = false;
    private bool pickupLock; // 拾取锁

    public void DropItem(bool fastDrop)
    {
        // 丢下举起的物品逻辑
        if (characterStat.LiftedItem != null)
        {
            characterStat.LiftedItem.gameObject.transform.SetParent(GameControl.Instance.GetSceneItemList().transform);
            
            characterStat.LiftedItem.OnItemDrop(false, true);
            characterStat.LiftedItem.ChangeRendererSortingOrder(GameConstData.BelowPlayerOrder);

            characterStat.LiftedItem = null;
            (int, ItemStatus) currentFocusedItemId = GameRunTimeData.Instance.CharacterItemSlotData.GetCurrentFocusedItemId();
            RefreshItemOnHand(currentFocusedItemId);
            GameHUD.Instance.slotManager.EnableHud();
            return;
        }

        bool removestack = GameRunTimeData.Instance.CharacterItemSlotData.ClearHandItem(fastDrop, ItemReleasePoint, PlayerReversed);

        if (removestack)
        {
            string uri = characterStat.ItemOnHand.GetPrefabName();
            int itemID = characterStat.ItemOnHand.ItemID;
            AssetHandle loadAssetAsync = YooAssets.LoadAssetAsync<GameObject>(uri);
            loadAssetAsync.Completed += handle =>
            {
                GameObject instantiate = Instantiate(loadAssetAsync.AssetObject, ItemReleasePoint) as GameObject;
                instantiate.transform.SetParent(GameControl.Instance.GetSceneItemList().transform);
                ItemBase ib = instantiate.GetComponent<ItemBase>();
                ib.InitItem(itemID);
                
                // GameRunTimeData.Instance.ItemManager.RegistItem(ib);
                
                ib.OnItemDrop(false);
            };
        }

        ChangeMouseAction(GameRunTimeData.Instance.CharacterItemSlotData.GetCurrentFocusSlot());
    }

    private bool PickUpValidation()
    {
        if (_pickupController.currentPickup == null)
        {
            return false;
        }
        if (_pickupController.currentPickup.DropState)
        {
            return false;
        }
        if (characterStat.LiftedItem != null)
        {
            return false;
        }
        return true;
    }
    public bool PickItem() // 拾取物品
    {
        if (GameRunTimeData.Instance.CharacterBasicStat.GetStat().Dead) return false;

        if (!PickUpValidation())
            return false;

        pickupLock = true;

        ItemBase toPickUpItem;
        toPickUpItem = _pickupController.currentPickup;
        
        // GameRunTimeData.Instance.ItemManager.UnRegistItem(toPickUpItem);
        if (toPickUpItem is IDataItem @base)
        {
            @base.OnItemGet();
            return true;
        }
        if (toPickUpItem is ISlotable)
        {
            // 背包数据更新
            bool stackOverFlowed;
            int Restult = GameRunTimeData.Instance.CharacterItemSlotData.InsertOrUpdateItemSlotData(toPickUpItem, out stackOverFlowed);
            if (toPickUpItem is IStackable)
            {
                if (Restult != -1) // 进入手中
                {
                    IStackable stackable = toPickUpItem as IStackable;
                    int overFlowedCount = stackable.GetStackCount();
                    // stackable 插入失败后该值会自动变为溢出量
                    if (stackOverFlowed)
                    {
                        GameItemTool.GenerateStackableItemAtTransform(
                            toPickUpItem.ItemID,
                            overFlowedCount,
                            ItemReleasePoint,
                            false,
                            (item) => { item.OnItemDrop(false); }
                        );
                    }
                }
            }

            // 一般逻辑
            if (Restult != -1) // 可以拾取物品
            {
                _pickupController.PlayerPickupItem();
                _pickupController.ChangeNextPickupTarget();
                pickupLock = false;
            }
            else
            {
                pickupLock = false;
            }

            ChangeMouseAction(GameRunTimeData.Instance.CharacterItemSlotData.GetCurrentFocusSlot());
        }
        else
        {
            // 举起物体
            if (toPickUpItem is ILiftable)
            {
                if (characterStat.ItemOnHand != null)
                {
                    Destroy(characterStat.ItemOnHand.gameObject);
                    characterStat.ItemOnHand = null;
                }

                _pickupController.PlayerPickupItem();
                characterStat.LiftedItem = toPickUpItem;
                RefreshItemLifted(toPickUpItem.ItemID);
                pickupLock = false;
                GameHUD.Instance.slotManager.DisableHud(false, null);
            }
        }

        return true;
    }

    private void RefreshItemOnHand((int, ItemStatus) Item)
    {
        int ItemId = Item.Item1;
        ItemStatus itemStatus = Item.Item2;
        if (characterStat.ItemOnHand != null)
        {
            if (characterStat.ItemOnHand.ItemID == Item.Item1)
            {
                characterStat.ItemOnHand.SetItemStatus(itemStatus);
                return;
            }

            Destroy(characterStat.ItemOnHand.gameObject);
            characterStat.ItemOnHand = null;
        }

        leftMouseAction = null;
        rightMouseAction = null;
        if (ItemId != -1)
        {
            GameItemTool.GenerateItemAtTransform(ItemId, ItemHoldPosition, true,
                (item) =>
                {
                    characterStat.ItemOnHand = item;
                    item.ChangeRendererSortingOrder(GameConstData.PlayerOrder);
                    leftMouseAction = item.OnLeftInteract;
                    rightMouseAction = item.OnRightInteract;
                    item.OnItemPickUp(); // 拾取物体后立即完成xxxx
                    item.SetItemStatus(itemStatus);
                }
            );
        }
    }

    private void RefreshItemLifted(int ItemId)
    {
        leftMouseAction = null;
        rightMouseAction = null;
        if (characterStat.ItemOnHand != null)
        {
            if (characterStat.ItemOnHand.ItemID == ItemId)
            {
                return;
            }

            Destroy(characterStat.ItemOnHand.gameObject);
            characterStat.ItemOnHand = null;
        }

        if (ItemId != -1)
        {
            GameItemTool.GenerateItemAtTransform(ItemId, ItemLiftPostion, false,
                (item) =>
                {
                    characterStat.LiftedItem = item;
                    item.ChangeRendererSortingOrder(GameConstData.PlayerOrder);
                    leftMouseAction = item.OnLeftInteract;
                    rightMouseAction = item.OnRightInteract;
                    item.OnItemPickUp(); // 举起物体后立即完成XXXXXX
                }
            );
        }
    }

    private void UpdatePlayerAnimatorEnum()
    {
        SetPlayerAnimatorEnum((EPlayerAnimator)((int)MoveState + (int)HandState));
    }

    private void SetPlayerAnimatorEnum(EPlayerAnimator value)
    {
        if (!isDead && value == animatorEnum)
        {
            return;
        }

        animatorEnum = value;
        var entry = playerSpin.AnimationState.SetAnimation(0, animatorEnum.ToString(), true);
        playerSpin.timeScale = 1;
    }

    //设置手部状态
    private void SetHandState(EPAHandState arg0)
    {
        HandState = arg0;
    }

    private void PlayerDead()
    {
        isDead = true;
        playerSpin.AnimationState.SetAnimation(0, EPlayerAnimator.OnDead.ToString(), false);
        playerSpin.timeScale = 1;
    }

    private void PlayerHurt()
    {
        SetPlayerAnimatorEnum(EPlayerAnimator.Hurt);
        isMove = false;
    }

    private int blinkNum = 0, blinkNumber = 6;

    private void PlayerAnimationComplete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name.Equals(EPlayerAnimator.Walk.ToString()) ||
            trackEntry.Animation.Name.Equals(EPlayerAnimator.Run.ToString()))
        {
            blinkNum++;
            if (blinkNum >= blinkNumber)
            {
                blinkNum = 0;
                switch (moveState)
                {
                    case EPAMoveState.Run:
                        PlayerAnimatorEnum = EPlayerAnimator.Run_Blink;
                        break;
                    case EPAMoveState.Walk:
                        PlayerAnimatorEnum = EPlayerAnimator.Walk_Blink;
                        break;
                }
            }
        }
        else if (trackEntry.Animation.Name.Equals(EPlayerAnimator.Run_Blink.ToString()) ||
                 trackEntry.Animation.Name.Equals(EPlayerAnimator.Walk_Blink.ToString()) ||
                 trackEntry.Animation.Name.Equals(EPlayerAnimator.Hurt.ToString()))
        {
            SetPlayerAnimatorEnum((EPlayerAnimator)((int)MoveState + (int)HandState));
            if (trackEntry.Animation.Name.Equals(EPlayerAnimator.Hurt.ToString()))
            {
                isMove = true;
            }
        }
    }
}


public enum EPAMoveState : int
{
    Idle = 1 << 6,
    Run = 1 << 7,
    Walk = 1 << 8,
    Walk_Backwards = 1 << 9,
}

public enum EPAHandState : int
{
    Default = 1 << 0,
    Hand = 1 << 1,
    Head = 1 << 2,
}

public enum EPlayerAnimator : int
{
    Hurt,
    Idle = EPAMoveState.Idle | EPAHandState.Default,
    Idle_Hand = EPAMoveState.Idle | EPAHandState.Hand,
    Idle_Head = EPAMoveState.Idle | EPAHandState.Head,
    OnDead,
    OnUse_1,
    Run = EPAMoveState.Run | EPAHandState.Default,
    Run_Hand = EPAMoveState.Run | EPAHandState.Hand,
    Run_Head = EPAMoveState.Run | EPAHandState.Head,
    Run_Blink,
    Walk = EPAMoveState.Walk | EPAHandState.Default,
    Walk_Hand = EPAMoveState.Walk | EPAHandState.Hand,
    Walk_Head = EPAMoveState.Walk | EPAHandState.Head,
    Walk_Backwards = EPAMoveState.Walk_Backwards | EPAHandState.Default,
    Walk_B_Hand = EPAMoveState.Walk_Backwards | EPAHandState.Hand,
    Walk_B_Head = EPAMoveState.Walk_Backwards | EPAHandState.Head,
    Walk_Blink,
}