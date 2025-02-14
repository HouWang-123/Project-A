using UI;
using UnityEngine;

public class GameHUD : UIBase
{
    public static GameHUD Instance;
    public ItemSlotManager_HUD SlotManagerHUD;
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }
    protected override void AddListen()
    {
    }

    public override void Show()
    {
        SlotManagerHUD.Show();
    }
    // Do Not Call Base.Hide();
    public override void Hide()
    {
        SlotManagerHUD.Hide();
    }

    public void NextFocusItem()
    {
        SlotManagerHUD.ChangeFocus(true);
    }

    public void SetFocus(int Number)
    {
        SlotManagerHUD.ChangeFocus(Number);
    }
    public void LastFocusItem()
    {
        SlotManagerHUD.ChangeFocus(false);
    }
}
