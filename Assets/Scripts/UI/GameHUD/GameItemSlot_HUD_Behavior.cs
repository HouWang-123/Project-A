using DG.Tweening;
using JetBrains.Annotations;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class GameItemSlot_HUD_Behavior : UIBase
{
    public Image ItemImage;
    public Button OverLappsButton;
    public GameObject ActiveTip;
    public Animator FocusAnimator;
    
    public TextMeshProUGUI ItemSlotNumber;
    private static readonly int IsFocus = Animator.StringToHash("IsFocus");

    void Start()
    {
        EmptySlot();
    }
    protected override void AddListen()
    {
        OverLappsButton.onClick.AddListener(OnClick);
    }

    public void OnFocus()
    {
        ItemSlotManager_HUD.ACTIVE_ITEM_SLOT = this;
        SetAsActiveItem();
        ItemSlotManager_HUD.Instance.OnItemSwitch();
    }
    public void OnClick()
    {
        OnFocus();
    }
    public void SetItemSlotNumber(int number)
    {
        ItemSlotNumber.text = number.ToString();
    }
    public void SetSlotItem( [CanBeNull] ItemBase itemBase )
    {
        EmptySlot();
        if (itemBase != null)
        {
            ItemImage.sprite = itemBase.SlotSprite;
            ItemImage.gameObject.SetActive(true);
        }

    }
    private void EmptySlot()
    {
        ItemImage.sprite = null;
        ItemImage.gameObject.SetActive(false);
        ActiveTip.transform.localScale = Vector3.zero;
    }

    public void SetAsActiveItem()
    {
        ActiveTip.transform.DOScale(Vector3.one, 0.2f);
        FocusAnimator.SetBool(IsFocus,true);
    }

    public void SetAsNonActiveItem()
    {
        ActiveTip.transform.DOScale(Vector3.zero, 0.2f);
        FocusAnimator.SetBool(IsFocus,false);
    }
}
