using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using YooAsset;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Events;

public class PlayerControl : MonoBehaviour
{
    public ItemBase ItemOnHand;             // 玩家当前手中的物品
    public Transform ItemHoldPosition;      // 玩家拿取物品位置
    private PlayerInputControl inputControl;
    private Rigidbody playerRG;
    private Transform playerRenderer;
    private Transform useObjParent;
    private PlayerPickupController _pickupController;
    private bool playerReversed;
    public float Speed = 5f;
    //前后移动的速度比率
    private float fToB = 0.6f;
    //奔跑速度比率
    private float runeToB = 1.5f;

    //Action
    private UnityAction leftMouseAction = null;
    private bool leftMous = false;
    private UnityAction rightMouseAction = null;
    private bool rightMous = false;
    private bool shiftButt = false;
    private void Awake() { inputControl = new PlayerInputControl(); }
    private void OnEnable() { inputControl?.Enable(); }

    private void Start()
    {
        _pickupController = GetComponent<PlayerPickupController>();
        playerRG = GetComponent<Rigidbody>();
        playerRenderer = transform.GetChild(0);
        playerRenderer.localEulerAngles = GameConstData.DefAngles;
        useObjParent = transform.GetChild(1);
        ItemHoldPosition = useObjParent.GetChild(0);
        if(ItemHoldPosition.childCount > 0)
        {
            ItemOnHand = ItemHoldPosition.GetChild(0).GetComponent<ItemBase>();
        }
        InputControl.Instance.GamePlayerEnable();
        InputControl.Instance.UIDisable();
        InputControl.Instance.LeftMouse.started += (item) =>
        {
            leftMous = true;
        };
        InputControl.Instance.LeftMouse.performed += (item) =>
        {

        };
        InputControl.Instance.LeftMouse.canceled += (item) =>
        {
            leftMous = false;
        };
        InputControl.Instance.RightMouse.started += (item) =>
        {
            rightMous = true;
        };
        InputControl.Instance.RightMouse.performed += (item) =>
        {

        };
        InputControl.Instance.RightMouse.canceled += (item) =>
        {
            rightMous = false;
        };
        InputControl.Instance.ShiftButton.started += (item) =>
        {
            shiftButt = true;
        };
        InputControl.Instance.ShiftButton.performed += (item) =>
        {

        };
        InputControl.Instance.ShiftButton.canceled += (item) =>
        {
            shiftButt = false;
        };
    }
    //AssetHandle asset;
    //float time = 0;
    //bool fire = false;
    //float Waitime = 0.3f;
    //private void FireCancelTest(UnityEngine.InputSystem.InputAction.CallbackContext text)
    //{
    //    fire = false;
    //    // Debug.Log("点击结束");
    //}
    //private void FireStarTest(UnityEngine.InputSystem.InputAction.CallbackContext text)
    //{
    //    asset = YooAssets.LoadAssetSync("ZiDan");
    //    fire = true;
    //    // Debug.Log("操作开始");
    //}
    //private void FireTest(UnityEngine.InputSystem.InputAction.CallbackContext text)
    //{
    //    // Debug.Log("点击");
    //}

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
    private void FixedUpdate()
    {
        PlayerMove(InputControl.Instance.MovePoint, Speed * Time.deltaTime);
        if(!pickupLock)
        {
            CalculateUseObjectRotation();
            useObjParent.localEulerAngles = a;
        }

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
        //if(fire)
        //{
        //    if (ItemOnHand == null)
        //    {
        //        return;
        //    }
        //    if(time > 0)
        //    {
        //        time -= Time.deltaTime;
        //    }
        //    else
        //    {   // 实例化子弹
        //        time = Waitime;
        //        GameObject zidan = Instantiate(asset.AssetObject, null) as GameObject;
        //        zidan.transform.eulerAngles = weaponTr.eulerAngles;
        //        zidan.transform.position = weaponTr.position;
        //    }
        //}
        DropItemDetection();
    }

    private void OnDisable()
    {
        inputControl?.Dispose();
    }

    private void PlayerMove(Vector3 vector, float speed)
    {
        vector.z = vector.y;
        vector.y = 0;
        if(playerRG != null)
        {
            if((vector.x > 0 && playerRenderer.localScale.x < 0) || (vector.x < 0 && playerRenderer.localScale.x > 0))
            {
                speed *= fToB;
            }
            else if(shiftButt)
            {
                speed *= runeToB;
            }
            playerRG.Move(vector * speed + transform.position, Quaternion.identity);
        }
    }

    public void SetMouseAction(UnityAction leftAction =null,UnityAction rightAction = null)
    {
        leftMouseAction = leftAction;
        rightMouseAction = rightAction;
    }

    // 物品拾取
    public PlayerPickupController PlayerPickupController;
    public GameObject PickUpPosition;
    private bool dropKeyPressed = false;
    private bool pickupLock;      // 拾取锁
    private void DropItemDetection()
    {
        if(pickupLock)
        {
            return;
        }
        if(Keyboard.current.gKey.isPressed)
        {
            if(dropKeyPressed)
            {
                return;
            }
            if(ItemOnHand != null)
            {
                ItemOnHand.CheckReverse(playerReversed);
                ItemOnHand.transform.SetParent(GameControl.Instance.GetSceneItemList().transform);
                ItemOnHand.OnItemDrop();
                ItemOnHand = null;
            }
            else
            {
                PickItem();
            }
            dropKeyPressed = true;
        }
        else
        {
            dropKeyPressed = false;
        }
    }

    public void PickItem() // 拾取物品
    {
        if(playerReversed)
        { useObjParent.localEulerAngles = GameConstData.ReversedRotation; }
        else
        { useObjParent.localEulerAngles = Vector3.zero; }
        if(_pickupController.currentPickup == null)
        { return; }
        if(_pickupController.currentPickup.DropState)
        { return; }
        pickupLock = true;
        _pickupController.currentPickup.transform.SetParent(ItemHoldPosition);
        _pickupController.currentPickup.CheckReverse(playerReversed);
        _pickupController.PlayerPickupItem();
        ItemHoldPosition.GetChild(0).transform.DOLocalMove(Vector3.zero, 0.3f).OnComplete(() =>
        {
            ItemOnHand = ItemHoldPosition.GetChild(0).GetComponent<ItemBase>();
            pickupLock = false;
        });
    }
}
