
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD_CursorBehaviour : MonoBehaviour
{
    public Image cursorImage;
    public RectTransform safeArea;
    private Sprite CurrentCursor;

    private void Awake()
    {
        cursorImage = GetComponent<Image>();
    }

    public void SetCursor(Sprite cursor)
    {
        CurrentCursor = cursor;
        cursorImage.sprite = cursor;
        cursorImage.SetNativeSize();
    }
    
    void Update()
    {
        // 检测左键按下
        if (Input.GetMouseButtonDown(0))
        {
        }
        
        // 检测左键弹起
        if (Input.GetMouseButtonUp(0))
        {
        }

        if (RectTransformUtility.RectangleContainsScreenPoint(safeArea, Input.mousePosition))
        {
            transform.position = Input.mousePosition;
        }
    }
}
