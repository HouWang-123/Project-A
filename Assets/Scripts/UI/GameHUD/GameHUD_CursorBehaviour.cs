

using DG.Tweening;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD_CursorBehaviour : MonoBehaviour
{
    public Image cursorImage;
    public RectTransform safeArea;
    
    public Sprite DefaultCursor;
    public Sprite OnItemCursor;
    private Sprite CurrentCursor;
    public Transform PlayerUseItemTransfrom;
    private bool HasFocus =  true;
    private void Awake()
    {
        cursorImage = GetComponent<Image>();
        InputControl.Instance.LeftMouse.started += (item)=>
        {
            OnLeftMouseDown();
        };
        InputControl.Instance.LeftMouse.canceled += (item)=>
        {
            OnLeftMouseUp();
        };
        EventManager.Instance.RegistEvent<ItemBase>(EventConstName.OnMouseFocusItemChanges, CursorImgaeSwicher);
    }

    private void OnLeftMouseDown()
    {
        DOTween.Kill(transform);
        transform.DOScale(0.8f, 0.1f);
    }

    private void OnLeftMouseUp()
    {
        DOTween.Kill(transform);
        transform.DOScale(Vector3.one, 0.3f);
    }
    public void SetPlayerItemTransform(Transform transform)
    {
        PlayerUseItemTransfrom = transform;
    }
    public void SetCursor(Sprite cursor)
    {
        CurrentCursor = cursor;
        cursorImage.sprite = cursor;
        cursorImage.SetNativeSize();
    }
    

    void Update()
    {
        CalculateCuresorPostionAndRotation();
        ClickHandler();
    }

    /// <summary>
    /// You may add more cursor style here
    /// 
    /// </summary>
    private void CursorImgaeSwicher(ItemBase itembase)
    {
        if (itembase == null)
        {
            SetCursor(DefaultCursor);
        }
        else
        {
            SetCursor(OnItemCursor);
        }
    }
    
    private void ClickHandler()
    {
        
    }
    private void CalculateCuresorPostionAndRotation()
    {
        if (!HasFocus)
        {
            Cursor.visible = true;
            return;
        }
        if (PlayerUseItemTransfrom == null) return;
        Vector3 PlayerOnScreenPosition = Camera.main.WorldToScreenPoint(PlayerUseItemTransfrom.position);
        float Dis_x = PlayerOnScreenPosition.x - InputControl.Instance.GetLook().x;
        float Dis_y = PlayerOnScreenPosition.y - InputControl.Instance.GetLook().y;
        // 获取坐标计算距离
        float atan2 = Mathf.Atan2(Dis_y, Dis_x); // 计算 arctan 得到弧度
        float rotate = Mathf.Rad2Deg * atan2;    // 转换为角度
        if (RectTransformUtility.RectangleContainsScreenPoint(safeArea, InputControl.Instance.GetLook()))
        {
            transform.position = InputControl.Instance.GetLook();
            transform.DORotate(new Vector3(0, 0, rotate), 0.02f);
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }
    }
    private void OnApplicationFocus(bool hasFocus)
    {
        HasFocus = hasFocus;
    }
    private void OnDestroy()
    {
        EventManager.Instance.RemoveEvent<ItemBase>(EventConstName.OnMouseFocusItemChanges,CursorImgaeSwicher);
    }
}
