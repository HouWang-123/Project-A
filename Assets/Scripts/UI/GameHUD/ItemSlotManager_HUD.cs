using System;
using System.Collections.Generic;
using DG.Tweening;
using FEVM.Timmer;
using NUnit.Framework;
using PixelCrushers;
using Spine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemSlotManager_HUD : UIBase
{
    
    public List<SlotItem> _SlotItemData = new List<SlotItem>();
    
    private bool switchHUD;
    public static ItemSlotManager_HUD Instance;
    [SerializeField] private List<GameItemSlot_HUD_Behavior> ItemSlots;
    public Button ItemSlotsHUDSwitch_N;
    [Space] public static GameItemSlot_HUD_Behavior ACTIVE_ITEM_SLOT;
    public float ItemSlotAnimationInterval;
    public float ItemSlotAnimationTime;

    private void Start()
    {
        Instance = this;
        ACTIVE_ITEM_SLOT = ItemSlots[0];
        ACTIVE_ITEM_SLOT.SetAsActiveItem();
        SetDisplayHotKeys();
    }

    public void UpdateItem(Dictionary<int,SlotItem> SlotItemData)
    {
        foreach (var V in ItemSlots)
        {
            V.EmptySlot();
        }
        foreach (var V in SlotItemData)
        {
            ItemSlots[V.Key -1].SetSlotItem(V.Value.ItemBase,V.Value.StackValue);
        }
    }
    private void SetDisplayHotKeys()
    {
        int i = 1;
        foreach (var slot in ItemSlots)
        {
            slot.SetItemSlotNumber(i);
            i++;
        }
    }

    #region 控制层API

    /// <summary>
    /// Next为true则切换为下一个物品否则切换为上一个物品
    /// </summary>
    /// <param name="Next"></param>
    public void ChangeFocus(bool next)
    {
        int currentIndex = ItemSlots.IndexOf(ACTIVE_ITEM_SLOT);
        int newIndex = (currentIndex + (next ? 1 : -1) + ItemSlots.Count) % ItemSlots.Count;
        ACTIVE_ITEM_SLOT.SetAsNonActiveItem();
        ItemSlots[newIndex].SetAsActiveItem();
        ACTIVE_ITEM_SLOT = ItemSlots[newIndex];
    }

    /// <summary>
    /// 直接修改道具栏焦点
    /// 传入的数字是道具栏快捷键设置
    /// </summary>
    /// <param name="Number"></param>
    public void ChangeFocus(int Number)
    {
        Number--;
        if (ItemSlots[Number] == ACTIVE_ITEM_SLOT)
        {
            return;
        }

        ACTIVE_ITEM_SLOT.SetAsNonActiveItem();
        ItemSlots[Number].SetAsActiveItem();
        ACTIVE_ITEM_SLOT = ItemSlots[Number];
    }

    /// <summary>
    /// 推荐使用 SwitchHUD 做隐藏和开启
    /// </summary>
    public void SwitchHUD()
    {
        if (isplayingOpeningOrClosing) return;
        if (switchHUD)
        {
            HUDDisable(false, null);
        }
        else
        {
            HUDEnable();
        }
    }

    #endregion

    private bool isplayingOpeningOrClosing;

    public override void Show()
    {
        if (isplayingOpeningOrClosing)
        {
            return;
        }

        gameObject.SetActive(true);
        foreach (var go in ItemSlots)
        {
            go.gameObject.transform.localScale = Vector3.zero;
        }

        // MakesureHUD ShowUp Always Play Animation
        HUDEnable();
    }

    public override void Hide()
    {
        if (isplayingOpeningOrClosing)
        {
            return;
        }

        HUDDisable(true, () => { gameObject.SetActive(false); });
    }

    private void HUDEnable()
    {
        if (switchHUD)
        {
            return;
        }

        LockSwitcher(null);
        OpenAnimationSequence();
        switchHUD = true;
    }

    private void HUDDisable(bool isClose, UnityAction cb)
    {
        if (!switchHUD)
        {
            return;
        }

        LockSwitcher(cb);
        CLoseAnimationSequence();
        switchHUD = false;
        if (isClose)
        {
            OnGameHUDCloseFunction();
        }
    }

    #region Animations

    void CLoseAnimationSequence()
    {
        int current_index = 1;
        for (int SlotIndex = ItemSlots.Count - 1; SlotIndex >= 0; SlotIndex--)
        {
            DoItemSlotHideAnimation(ItemSlots[SlotIndex], current_index);
            current_index++;
        }
    }

    void OpenAnimationSequence()
    {
        int i = 1;
        foreach (var slot in ItemSlots)
        {
            i++;
            DoItemSlotShowAnimation(slot, i);
        }
    }

    void DoItemSlotShowAnimation(GameItemSlot_HUD_Behavior slotHUDBehavior, int delayedTimeScale)
    {
        TimeMgr.Instance.AddTask(ItemSlotAnimationInterval * (delayedTimeScale - 1), false,
            () => { slotHUDBehavior.transform.DOScale(Vector3.one, ItemSlotAnimationTime); });
    }

    void DoItemSlotHideAnimation(GameItemSlot_HUD_Behavior slotHUDBehavior, int delayedTimeScale)
    {
        TimeMgr.Instance.AddTask(ItemSlotAnimationInterval * (delayedTimeScale - 1), false,
            () => { slotHUDBehavior.transform.DOScale(Vector3.zero, ItemSlotAnimationTime); });
    }

    void LockSwitcher(UnityAction cb)
    {
        isplayingOpeningOrClosing = true;
        float lockTime;
        lockTime = (ItemSlots.Count - 1) * ItemSlotAnimationInterval + ItemSlotAnimationTime;
        TimeMgr.Instance.AddTask(lockTime, false, () =>
        {
            isplayingOpeningOrClosing = false;
            OnItemSlotHUDFinishAniamtion();
            cb?.Invoke();
        });
    }

    // todo HUD打开后可以进行额外的动画或者调用其他函数
    void OnItemSlotHUDFinishAniamtion()
    {
    }

    // todo HUD关闭后可以进行额外的动画或者调用其他函数
    void OnGameHUDCloseFunction()
    {
    }

    #endregion

    #region Listeners

    protected override void AddListen()
    {
        //ItemSlotsHUDSwitch_N.onClick.AddListener(() => { ChangeFocus(false); });
    }

    public void OnItemSwitch()
    {
        foreach (var ItemSlot in ItemSlots)
        {
            if (ItemSlot == ACTIVE_ITEM_SLOT)
            {
                continue;
            }

            ItemSlot.SetAsNonActiveItem();
        }
    }

    #endregion
}