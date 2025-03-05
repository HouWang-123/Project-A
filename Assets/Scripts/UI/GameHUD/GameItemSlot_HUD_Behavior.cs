using DG.Tweening;
using JetBrains.Annotations;
using TMPro;

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameItemSlot_HUD_Behavior : UIBase
{
    public Image ItemIcon;
    public Button OverLappsButton;
    public GameObject ActiveTip;
    public Animator FocusAnimator;
    public TextMeshProUGUI ItemCount;
    public TextMeshProUGUI ItemSlotNumber;
    private static readonly int IsFocus = Animator.StringToHash("IsFocus");

    private int Key;
    
    void Start()
    {
        ActiveTip.transform.localScale = Vector3.zero;
        EmptySlot();
    }
    protected override void AddListen()
    {
        OverLappsButton.onClick.AddListener(OnClick);
    }

    public void OnFocus()
    {
        if (GameRunTimeData.Instance.CharacterBasicStat.GetStat().LiftedItem != null) return;
        GameHUD_ItemSlotManager.ACTIVE_ITEM_SLOT = this;
        GameControl.Instance.PlayerControl.ChangeMouseAction(Key);
        SetAsActiveItem();
        GameHUD_ItemSlotManager.Instance.OnItemSwitch();
    }
    public void OnClick()
    {
        OnFocus();
    }
    public void SetItemSlotNumber(int number)
    {
        Key = number;
        ItemSlotNumber.text = number.ToString();
    }
    
    public void SetSlotItem(int ItemId,int count)
    {
        GameItemTool.GetItemIcon(ItemId, (sprite) =>
        {
            ItemIcon.sprite = sprite;
        });
        
        ItemIcon.gameObject.SetActive(true);
        ItemCount.gameObject.SetActive(true);
        ItemCount.text = count.ToString();
        if (count == 1)
        {
            ItemCount.gameObject.SetActive(false);
        }
    }
    
    public void EmptySlot()
    {
        ItemIcon.sprite = null;
        ItemIcon.gameObject.SetActive(false);
        ItemCount.gameObject.SetActive(false);
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
