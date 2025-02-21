using System;

public class GameHUD : UIBase
{
    public static GameHUD Instance;
    public int GameTics;
    
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
    public static CharacterStat CharacterStatHUDStat;

    public void SetHUDStat(CharacterStat s)
    {
        CharacterStatHUDStat = s;
    }

    private void RefreshHUDs()    // 界面数据刷新
    {
        
    }
    private void FixedUpdate()
    {
        if (GameTics % 2 == 0)
        {
            RefreshHUDs(); // 每秒界面更新频率为 25
        }
        GameTics++;
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
