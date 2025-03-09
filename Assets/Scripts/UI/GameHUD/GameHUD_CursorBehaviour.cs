
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameHUD_CursorBehaviour : MonoBehaviour
{
    public Image cursorImage;
    public RectTransform safeArea;
    private Sprite CurrentCursor;
    public Transform PlayerUseItemTransfrom;
    private void Awake()
    {
        cursorImage = GetComponent<Image>();
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
        if (PlayerUseItemTransfrom == null) return;
        //检测左键按下
        //if(Input.GetMouseButtonDown(0))
        //{
        //}

        //检测左键弹起
        //if(Input.GetMouseButtonUp(0))
        //{
        //}

        Vector3 PlayerOnScreenPosition = Camera.main.WorldToScreenPoint(PlayerUseItemTransfrom.position);
        float Dis_x = PlayerOnScreenPosition.x - InputControl.Instance.GetLook().x;
        float Dis_y = PlayerOnScreenPosition.y - InputControl.Instance.GetLook().y;
        // 获取坐标计算距离
        float atan2 = Mathf.Atan2(Dis_y, Dis_x); // 计算 arctan 得到弧度
        float rotate = Mathf.Rad2Deg * atan2;    // 转换为角度
        if (RectTransformUtility.RectangleContainsScreenPoint(safeArea, InputControl.Instance.GetLook()))
        {
            transform.DOMove(InputControl.Instance.GetLook(), 0.02f);
            transform.DORotate(new Vector3(0, 0, rotate), 0.02f);
        }
    }
}
