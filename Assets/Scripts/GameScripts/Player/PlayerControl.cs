using UnityEngine;
using YooAsset;
using UnityEngine.Events;
using Spine.Unity;

public class PlayerControl : MonoBehaviour
{
    public Transform ItemHoldPosition; // 玩家拿取物品位置
    public Transform ItemLiftPostion; // 举起物品的位置
    private PlayerInputControl inputControl;
    private Rigidbody playerRG;
    private Transform playerRenderer;
    private Transform useObjParent;
    private PlayerPickupController _pickupController;
    private bool playerReversed; // 判断角色是否发生了偏转，与拾取和丢弃道具物品有关系
    
    private CharacterStat characterStat;
    
    private EPlayerAnimator animatorEnum = EPlayerAnimator.Idle;
    public EPlayerAnimator PlayerAnimatorEnum
    {
        set
        {
            if(value == animatorEnum)
            {
                return;
            }
            animatorEnum = value;
            var entry = playerSpin.AnimationState.SetAnimation(0, animatorEnum.ToString(), true);
            playerSpin.timeScale = 1;
        }
        get
        {
            return animatorEnum;
        }
    }
    private SkeletonAnimation playerSpin;



    //前后移动的速度比率
    private float fToB = 0.6f;

    public Transform ItemReleasePoint;

    //Action
    private UnityAction leftMouseAction = null;
    private bool leftMous = false;
    private UnityAction rightMouseAction = null;
    private bool rightMous = false;
    private bool shiftButt = false;
    
    private void Awake()
    {
        inputControl = new PlayerInputControl();
    }

    private void OnEnable()
    {
        inputControl?.Enable();
    }

    private float ScrollActionTimer;

    private void Start()
    {
        GameRunTimeData.Instance.CharacterItemSlotData.ChangeFocusSlotNumber(1); // 默认启用道具栏
        characterStat = GameRunTimeData.Instance.CharacterBasicStat.GetStat();
        _pickupController = GetComponent<PlayerPickupController>();
        playerRG = GetComponent<Rigidbody>();
        playerRenderer = transform.GetChild(0);
        playerRenderer.localEulerAngles = GameConstData.DefAngles;
        useObjParent = transform.GetChild(1);
        ItemHoldPosition = useObjParent.GetChild(0);
        playerSpin = transform.Find("Renderer").GetComponentInChildren<SkeletonAnimation>();
        if(playerSpin != null)
        {
            playerSpin.AnimationState.SetAnimation(0, animatorEnum.ToString(), true);
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
                Debug.Log(GetType() + "/ 现在是游戏时间[天数: " + gameDay + " ,小时: " + gameHour + " ,分钟: " + gameMinute + " ]");
                Debug.Log(GetType() + "/ 触发了主角拉屎");
            }
        };
        TimeSystemManager.Instance.PhasedChangedScheduledEvents.Add(phasedEvent);

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

        InputControl.Instance._1Key.started += (item) =>
        {
            ChangeMouseAction(1);
        };
        InputControl.Instance._2Key.started += (item) =>
        {
            ChangeMouseAction(2);

        };
        InputControl.Instance._3Key.started += (item) =>
        {
            ChangeMouseAction(3);

        };
        InputControl.Instance._4Key.started += (item) =>
        {
            ChangeMouseAction(4);
        };
        InputControl.Instance._5Key.started += (item) =>
        {
            ChangeMouseAction(5);

        };
        InputControl.Instance._6Key.started += (item) =>
        {
            ChangeMouseAction(6);

        };
        InputControl.Instance.MouseScroll.started += (item) =>
        {
            if (characterStat.LiftedItem != null)
            {
                return;
            }
            if(ScrollActionTimer <= 0.1f)
            {
                return;
            }

            Vector2 readValue = item.ReadValue<Vector2>();
            if(readValue.y > 0)
            {
                GameHUD.Instance.ISM_LastFocusItem();
                GameRunTimeData.Instance.CharacterItemSlotData.ChangeFocusSlotNumber(false);
            }
            else if(readValue.y < 0)
            {
                GameHUD.Instance.ISM_NextFocusItem();
                GameRunTimeData.Instance.CharacterItemSlotData.ChangeFocusSlotNumber(true);
            }
            GameRunTimeData.Instance.CharacterItemSlotData.ActiveCurrentItem();
            ItemBase characterInUseItem = GameRunTimeData.Instance.CharacterItemSlotData.GetCharacterInUseItem();
            if(characterInUseItem != null)
            {
                leftMouseAction = characterInUseItem.OnLeftInteract;
                rightMouseAction = characterInUseItem.OnRightInteract;
            }
            else
            {
                leftMouseAction = null;
                rightMouseAction = null;
            }

            ScrollActionTimer = 0f;
        };
        //测试切换地面物品
        //=================
        InputControl.Instance.QButton.started += (item) => { _pickupController.ChangeItemToogle(true); };
        InputControl.Instance.QButton.canceled += (item) => { _pickupController.ChangeItemToogle(false); };
        //==================
        InputControl.Instance.EButton.started += (item) => { PickItem(); };

        InputControl.Instance.GButton.started += (item) =>
        {
            if(GameRunTimeData.Instance.CharacterItemSlotData.GetCharacterInUseItem() != null)
            {
                DropItem(false);
            }
            if (characterStat.LiftedItem != null)
            {
                DropItem(false);
            }
        };
        #endregion
    }
    
    Vector3 u, v, l, a, b;
    float angle;

    private void CalculateUseObjectRotation()
    {
        v = Camera.main.WorldToScreenPoint(transform.position);
        l = InputControl.Instance.GetLook();
        if(l.x < v.x)
        {
            playerRenderer.localScale = GameConstData.ReverseScale;
            playerReversed = true;
        }
        else
        {
            playerRenderer.localScale = Vector3.one;
            playerReversed = false;
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
        GameRunTimeData.Instance.CharacterItemSlotData.ActiveCurrentItem();
        ItemBase characterInUseItem = GameRunTimeData.Instance.CharacterItemSlotData.GetCharacterInUseItem();
        if(characterInUseItem != null)
        {
            leftMouseAction = characterInUseItem.OnLeftInteract;
            rightMouseAction = characterInUseItem.OnRightInteract;
        }
        else
        {
            leftMouseAction = null;
            rightMouseAction = null;
        }
    }
    private void FixedUpdate()
    {
        GameRunTimeData.Instance.CharacterBasicStat.UpdatePlayerStat();
        
        PlayerMove(InputControl.Instance.MovePoint, characterStat.WalkSpeed);
        
        if(!pickupLock)
        {
            CalculateUseObjectRotation();
            useObjParent.localEulerAngles = a;
        }

        ScrollActionTimer += Time.deltaTime;

        // Rotate WeaponTr
        Transform weaponTr = useObjParent.GetChild(0);
        angle = a.y;
        angle %= 360;
        if(angle < 0)
        {
            angle += 360;
        }

        if(angle > 90f && angle <= 270f)
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
        // 武器使用相关
        if(leftMous)
        {
            leftMouseAction?.Invoke();
        }

        if(rightMous)
        {
            rightMouseAction?.Invoke();
        }
        
    }
    
    private void OnDisable()
    {
        inputControl?.Dispose();
    }

    private bool stopmove = false;

    private void PlayerMove(Vector3 vector, float speed)
    {
        vector.z = vector.y;
        vector.y = 0;
        if((vector - Vector3.zero).sqrMagnitude > 0.01)
        {
            if((vector.x > 0 && playerRenderer.localScale.x < 0) || (vector.x < 0 && playerRenderer.localScale.x > 0))
            {
                speed *= fToB;
                PlayerAnimatorEnum = EPlayerAnimator.Walk_Backwards;
                //playerSpin.timeScale = fToB;              //匹配动画速度
            }
            else if(shiftButt)
            {
                speed *= characterStat.RunSpeedScale;
                PlayerAnimatorEnum = EPlayerAnimator.Run;
                playerSpin.timeScale = speed * 0.6f;        //匹配动画速度
            }
            else
            {
                PlayerAnimatorEnum = EPlayerAnimator.Walk;
                playerSpin.timeScale = speed / 0.6f;      //匹配动画速度
            }

            //playerRG.Move(vector * speed + transform.position, Quaternion.identity);
        }
        else
        {
            PlayerAnimatorEnum = EPlayerAnimator.Idle;
        }
        if(playerRG != null && !stopmove)
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
            GameRunTimeData.Instance.CharacterItemSlotData.GetCharacterInUseItem()?.EnableRenderer();
            characterStat.LiftedItem.gameObject.transform.SetParent(GameControl.Instance.GetSceneItemList().transform);
            characterStat.LiftedItem.OnItemDrop(false);
            characterStat.LiftedItem.ChangeRendererSortingOrder(GameConstData.BelowPlayerOrder);
            characterStat.LiftedItem = null;
            GameHUD.Instance.SlotManagerHUD.EnableHud();
            return;
        }
        Debug.Log("丢下");
        // 表现  // 背包数据更新
        // todo : 批量丢弃堆叠物品

        bool removestack = GameRunTimeData.Instance.CharacterItemSlotData.ClearHandItem(fastDrop, playerReversed,ItemReleasePoint);
        
        if(removestack)
        {
            string uri = GameRunTimeData.Instance.CharacterItemSlotData.GetCharacterInUseItem().GetPrefabName();
            AssetHandle loadAssetAsync = YooAssets.LoadAssetAsync<GameObject>(uri);
            loadAssetAsync.Completed += handle =>
            {
                GameObject instantiate = Instantiate(loadAssetAsync.AssetObject, ItemReleasePoint) as GameObject;
                instantiate.transform.SetParent(GameControl.Instance.GetSceneItemList().transform);
                ItemBase ib = instantiate.GetComponent<ItemBase>();
                ib.OnItemDrop(false);
            };
        }
        ChangeMouseAction(GameRunTimeData.Instance.CharacterItemSlotData.GetCurrentFocusSlot());
    }

    public void PickItem() // 拾取物品
    {
        if(_pickupController.currentPickup == null)
        {
            return;
        }
        // 表现层
        if(playerReversed)
        {
            useObjParent.localEulerAngles = GameConstData.ReversedRotation;
        }
        else
        {
            useObjParent.localEulerAngles = Vector3.zero;
        }
        if(_pickupController.currentPickup.DropState)
        {
            return;
        }

        if (characterStat.LiftedItem != null)
        {
            return;
        }

        pickupLock = true;
        
        ItemBase toPickUpItem;
        toPickUpItem = _pickupController.currentPickup;
        
        ///////////可堆叠或者不可堆叠物品进入背包并更新HUD和人物渲染的逻辑 /////////////
        
        if (toPickUpItem is ISlotable)
        {
            // 背包数据更新
            bool stackOverFlowed;
            int  Restult = GameRunTimeData.Instance.CharacterItemSlotData.InsertOrUpdateItemSlotData(toPickUpItem, out stackOverFlowed);
            
            if (toPickUpItem is IStackable)
            {
                if (Restult != -1) // 进入手中
                {
                    IStackable stackable = toPickUpItem as IStackable;
                    int overFlowedCount = stackable.GetStackCount();
                    if (stackOverFlowed)
                    {
                        string uri = toPickUpItem.GetPrefabName();
                        AssetHandle loadAssetAsync = YooAssets.LoadAssetAsync<GameObject>(uri);
                        loadAssetAsync.Completed += handle =>
                        {
                            GameObject instantiate = Instantiate(loadAssetAsync.AssetObject, ItemReleasePoint) as GameObject;
                            instantiate.transform.SetParent(GameControl.Instance.GetSceneItemList().transform);
                            ItemBase ib = instantiate.GetComponent<ItemBase>();
                            IStackable ib1 = ib as IStackable;
                            ib1.ChangeStackCount(overFlowedCount);
                            ib.OnItemDrop(false);
                        };
                    }
                    stackable.ChangeStackCount(1);
                }
            }
            if(Restult != -1) // 可以拾取物品
            {
                _pickupController.currentPickup.transform.SetParent(ItemHoldPosition);
                _pickupController.currentPickup.CheckReverse(playerReversed);
                _pickupController.PlayerPickupItem();
                SetItemBaseToPlayerHand(toPickUpItem);
                if(Restult == 1) // 手中存在物品
                {
                    toPickUpItem.DisableRenderer();
                }
                if (Restult == 2) // 堆叠物品
                {
                    Destroy(toPickUpItem.gameObject);
                }
                pickupLock = false;
                _pickupController.currentPickup = null;
                _pickupController.ChangePickupTarget();
            }
            else
            {
                pickupLock = false;
            }
        
            ChangeMouseAction(GameRunTimeData.Instance.CharacterItemSlotData.GetCurrentFocusSlot());
            void SetItemBaseToPlayerHand(ItemBase itemBase)
            {
                Vector3 transformLocalEulerAngles = itemBase.gameObject.transform.localEulerAngles;
                transformLocalEulerAngles.x = -45;
                itemBase.gameObject.transform.localEulerAngles = transformLocalEulerAngles;
                itemBase.gameObject.transform.localPosition = Vector3.zero;
            }
        }
        else
        {
            if (toPickUpItem is ILiftable)
            {
                ///// todo 举起物品逻辑  /////////
                GameRunTimeData.Instance.CharacterItemSlotData.GetCharacterInUseItem()?.DisableRenderer();
                characterStat.LiftedItem = toPickUpItem;
                toPickUpItem.ChangeRendererSortingOrder(GameConstData.OverPlayerOrder);
                toPickUpItem.gameObject.transform.SetParent(ItemLiftPostion);
                toPickUpItem.CheckReverse(playerReversed);
                toPickUpItem.gameObject.transform.localPosition = Vector3.zero;
                leftMouseAction = toPickUpItem.OnLeftInteract;
                rightMouseAction = toPickUpItem.OnRightInteract;
                _pickupController.PlayerPickupItem();
                pickupLock = false;
                GameHUD.Instance.SlotManagerHUD.DisableHud(false,null);
            }
        }
    }
}

public enum EPlayerAnimator
{
    Hurt,
    Idle,
    Idle_Hand,
    Idle_Head,
    OnDead,
    OnUse_1,
    Run,
    Run_Blink,
    Run_Hand,
    Run_Head,
    Walk,
    Walk_Backwards,
    Walk_Blink,
    Walk_B_Hand,
    Walk_B_Head,
    Walk_Hand,
    Walk_Head,
}