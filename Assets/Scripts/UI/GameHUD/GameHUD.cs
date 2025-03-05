using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GameHUD : UIBase
{
    public static GameHUD Instance;
    /// <summary>
    /// ItemSlotManager 道具栏管理器，API 方法前缀: ISM
    /// </summary>
    public GameHUD_ItemSlotManager slotManager;
    public GameHUD_AreaNotificationBehaviour gameHUDAreaNotificationBehaviour;
    public GameHUD_HpIndicator HpIndicator;
    public GameHUD_CursorBehaviour Cursor;

    public void SetPlayerItemTransform(Transform _transform)
    {
        Cursor.SetPlayerItemTransform(_transform);
    }
    public void SetGameCursor(Sprite sprite)
    {
        Cursor.SetCursor(sprite);
    }
    public void OnAreaNotificaiton(string Area_text)
    {
        gameHUDAreaNotificationBehaviour.OnNotification(Area_text);
    }
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        slotManager = GetComponentInChildren<GameHUD_ItemSlotManager>();
        gameHUDAreaNotificationBehaviour = GetComponentInChildren<GameHUD_AreaNotificationBehaviour>();
        HpIndicator = GetComponentInChildren<GameHUD_HpIndicator>();
        Cursor = GetComponentInChildren<GameHUD_CursorBehaviour>();
    }
    protected override void AddListen()
    {
    }

    public override void Show()
    {
        slotManager.Show();
    }
    // Do Not Call Base.Hide();
    public override void Hide()
    {
        slotManager.Hide();
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

    public void UpdateHp()
    {
        HpIndicator.UpdateHp();
    }
    private void RefreshHUDs()    // 界面数据刷新
    {
        
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
        slotManager.ChangeFocus(true);
    }

    public void ISM_SetFocus(int Number)
    {
        slotManager.ChangeFocus(Number);
    }
    public void ISM_LastFocusItem()
    {
        slotManager.ChangeFocus(false);
    }
#endregion
}
