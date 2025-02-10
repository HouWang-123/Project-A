using System;
using UnityEngine;
using YooAsset;
using UnityEngine.InputSystem.Interactions;

public class PlayerControl : MonoBehaviour
{
    private PlayerInputControl inputControl;

    private Rigidbody playerRG;
    private Transform playerRenderer;
    private Transform useObjParent;

    public float Speed = 5f;
    
    //前后移动的速度比率
    private float fToB = 0.6f;


    private void Awake()
    {
        inputControl = new PlayerInputControl();
    }

    private void OnEnable()
    {
        inputControl?.Enable();
    }

    private void Start()
    {
        playerRG = GetComponent<Rigidbody>();
        playerRenderer = transform.GetChild(0);
        playerRenderer.localEulerAngles = GameConstData.DefAngles;

        useObjParent = transform.GetChild(1);

        InputControl.Instance.LeftMouse.started += FireStarTest;
        InputControl.Instance.LeftMouse.performed += FireTest;
        InputControl.Instance.LeftMouse.canceled += FireCancelTest;
    }

    private void FireCancelTest(UnityEngine.InputSystem.InputAction.CallbackContext text)
    {
        fire = false;
        // Debug.Log("点击结束");
    }

    private void FireStarTest(UnityEngine.InputSystem.InputAction.CallbackContext text)
    {
        asset = YooAssets.LoadAssetSync("ZiDan");
        fire = true;
        // Debug.Log("操作开始");
    }
    AssetHandle asset;
    float Waitime = 0.3f;
    float time = 0;
    bool fire = false;
    private void FireTest(UnityEngine.InputSystem.InputAction.CallbackContext text)
    {
        // Debug.Log("点击");
    }

    Vector3 u, v, l, a, b;
    float angle;
    private void FixedUpdate()
    {
        PlayerMove(InputControl.Instance.MovePoint, Speed * Time.deltaTime);
        v = Camera.main.WorldToScreenPoint(transform.position);
        l = InputControl.Instance.GetLook();
        if(l.x < v.x)
        {
            playerRenderer.localScale = GameConstData.ReverseScale;
            
            // // 翻转 usepoint 位置
            // Vector3 originalUsePosition = useObjParent.localPosition;
            // originalUsePosition.x *= -1;
            // useObjParent.localPosition = originalUsePosition;
        }
        else
        {
            playerRenderer.localScale = Vector3.one;
            
            // // 翻转 usepoint 位置
            // Vector3 originalUsePosition = useObjParent.localPosition;
            // originalUsePosition.x *= -1;
            // useObjParent.localPosition = originalUsePosition;
            
        }
        u = Camera.main.WorldToScreenPoint(useObjParent.position);
        l = l - u;
        l.y *= 1.4f;
        a = -Vector3.up * Mathf.Atan2(l.y, l.x) * Mathf.Rad2Deg;
        useObjParent.localEulerAngles = a;
        //Test
        Transform tran = useObjParent.GetChild(0);
        if(tran != null)
        {
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
            b = tran.localEulerAngles;
            b.x = angle;
            b.y = 0;
            b.z = 0;
            tran.localEulerAngles = b;
        }
        if(fire)
        {
            if(time > 0)
            {
                time -= Time.deltaTime;
            }
            else
            {
                time = Waitime;
                GameObject zidan = Instantiate(asset.AssetObject, null) as GameObject;
                zidan.transform.eulerAngles = useObjParent.GetChild(0).eulerAngles;
                zidan.transform.position = useObjParent.GetChild(0).position;
            }
        }
        //
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
            if ((vector.x > 0 && playerRenderer.localScale.x < 0) || (vector.x < 0 && playerRenderer.localScale.x > 0))
            {
                speed *= fToB;
            }
            playerRG.Move(vector * speed + transform.position, Quaternion.identity);
        }
    }
    // 物品拾取
    public PlayerPickupController PlayerPickupController;
    public GameObject PickUpPosition;
    public void DropItem()  // 丢下当前物品
    {
        
    }

    public void PickItem() // 拾取物品
    {
        
    }
}
