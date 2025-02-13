using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameItemSlot_HUD_Behavior : UIBase
{
    public Image ItemImage;

    public TextMeshProUGUI ItemSlotNumber;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { EmptySlot(); }
    protected override void AddListen()
    {
        
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
    }
}
