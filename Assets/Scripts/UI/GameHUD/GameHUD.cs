using UI;
using UnityEngine;

public class GameHUD : UIBase
{
    public ItemSlotManager_HUD SlotManagerHUD;
    protected override void Awake()
    {
        base.Awake();
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
}
