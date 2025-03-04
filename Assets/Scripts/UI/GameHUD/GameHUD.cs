using System;

public class GameHUD : UIBase
{
    public static GameHUD Instance;
    /// <summary>
    /// ItemSlotManager 道具栏管理器，API 方法前缀: ISM
    /// </summary>
    public ItemSlotManager_HUD SlotManagerHUD;

    public AreaNotificationBehaviour AreaNotificationBehaviour;
    public void OnAreaNotificaiton(string Area_text)
    {
        AreaNotificationBehaviour.OnNotification(Area_text);
    }
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

    public void SetHUDTime()
    {
        
    }
    
#region UpdateHUD
    public GameHUD_HpIndicator HpIndicator;
    private void RefreshHUDs()    // 界面数据刷新
    {
        HpIndicator.UpdateHp();
    }
    private int GameTics;
    private void FixedUpdate()
    {
        if (GameTics % 2 == 0)
        {
            RefreshHUDs(); // 每秒界面更新频率为 25
        }
        GameTics++;
    }
#endregion

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
