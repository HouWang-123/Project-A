public class GameHUD : UIBase
{
    public static GameHUD Instance;
    /// <summary>
    /// ItemSlotManager 道具栏管理器，API 方法前缀: ISM
    /// </summary>
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
    
#region ItemSlotManagerAPI
    public void ISM_NextFocusItem()
    {
        SlotManagerHUD.ChangeFocus(true);
    }

    public void ISM_SetFocus(int Number)
    {
        SlotManagerHUD.ChangeFocus(Number);
    }
    public void ISM_LastFocusItem()
    {
        SlotManagerHUD.ChangeFocus(false);
    }
#endregion
}
