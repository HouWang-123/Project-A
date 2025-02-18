using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SlotItemStatus
{
    public SlotItemStatus(int itemID, int stackValue, ItemBase itemBase)
    {
        ItemID = itemID;
        StackValue = stackValue;
        ItemBase = itemBase;
    }

    [SerializeField] 
    public ItemBase ItemBase;
    [SerializeField] 
    public int ItemID;
    [SerializeField]
    public int StackValue; // 最小为1
}

public class ItemSlotData
{
    private int CurrentMaxSlotCount;
    private ItemBase CurrentItem; // 玩家手中的物品
    
    private Dictionary<int, ItemBase> AllCharacterItems = new Dictionary<int, ItemBase>(); // Number,Item
    
    private int CurrentFocusSlot; // 玩家手中的物品对应的Number Key
    
    public void SetMaxSlotCount(int number)
    {
        CurrentMaxSlotCount = number;
    }

    public void ActiveCurrentItem()
    {
        foreach (var V in AllCharacterItems)
        {
            V.Value.DisableRenderer();
        }

        if (SlotItemDataList.ContainsKey(CurrentFocusSlot))
        {
            AllCharacterItems[CurrentFocusSlot].EnableRenderer();
            SetCharacterInUseItem(AllCharacterItems[CurrentFocusSlot]);
        }
        else
        {
            SetCharacterInUseItem(null);
        }

    }
    /// <summary>
    /// 注：int下标从1开始，对应道具栏1~6
    /// </summary>
    private Dictionary<int, SlotItemStatus> SlotItemDataList = new ();
    /// <summary>
    /// 物品id对应道具栏1~6对应的
    /// </summary>
    private Dictionary<int, int> ItemID2Key = new ();

    /// <summary>
    /// 为可堆叠物品获取合适的SlotNumber
    /// </summary>
    /// <param name="InsertItem"></param>
    /// <returns></returns>
    private int GetItemProperStackKeyNumber(ItemBase InsertItem)
    {
        // 遍历道具栏
        foreach (var VARIABLE in SlotItemDataList)
        {
            // 判断是否同一个物品
            if (VARIABLE.Value.ItemID == InsertItem.ItemID)
            {
                // 判断堆叠数量
                IStackable iStackable = InsertItem as IStackable;
                if (iStackable.GetMaxStackValue() > VARIABLE.Value.StackValue)
                {
                    return VARIABLE.Key;
                }
            }
        }
        return FindEmptySlot();
    }

    /// <summary>
    /// 获取空Slot位置
    /// </summary>
    /// <returns></returns>
    private int FindEmptySlot()
    {
        SlotItemStatus itemStatus;
        for (int index = 1; index <= CurrentMaxSlotCount; index++)
        {
            if (!SlotItemDataList.TryGetValue(index, out itemStatus))
            {
                return index;
            }
        }
        return -1; // 没有合适的空位置
    }

    /// <summary>
    /// 返回 -1 表示插入失败
    /// 返回  0 表示空手正常拾取
    /// 返回  1 表示拾取后进入背包
    /// 返回  2 表示拾取到可堆叠的物品
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int InsertOrUpdateItemSlotData(ItemBase item)
    {
        int slotNumber = CurrentFocusSlot; // 默认插入位置为当前焦点位置
        SlotItemStatus newItemStatus = new SlotItemStatus(item.ItemID, 1,item);
        
        if (SlotItemDataList.ContainsKey(slotNumber))
        {
            slotNumber = FindEmptySlot();
        }
        
        // 获取合适的 Key 包括可否堆叠位置
        if (ItemID2Key.ContainsValue(item.ItemID))
        {
            if (item is IStackable)
            {
                slotNumber = GetItemProperStackKeyNumber(item);
            }
        }


        // 插入物品失败，取消拾取动作
        if (slotNumber == -1)
        {
            return -1;
        }
        
        SlotItemStatus stackedItemStatusStatus = null;
        if (SlotItemDataList.TryGetValue(slotNumber, out stackedItemStatusStatus))
        {
            // 对原有物品进行更新
            stackedItemStatusStatus.StackValue++;
            SlotItemDataList[slotNumber] = stackedItemStatusStatus;
        }
        else
        {
            // 插入物品成功
            SlotItemDataList[slotNumber] = newItemStatus;
        }
        
        GameHUD.Instance.SlotManagerHUD.UpdateItem(SlotItemDataList);
        
        if (item is IStackable && stackedItemStatusStatus != null)
        {
            return 2; // 发生了堆叠物品操作
        }

        if (stackedItemStatusStatus == null)
        {
            AllCharacterItems.Add(slotNumber,item);
            ItemID2Key.Add(slotNumber,item.ItemID);
        }

        
        if (slotNumber != CurrentFocusSlot)
        {
            return 1;
        }
        
        SetCharacterInUseItem(item);
        return 0;
    }

    private void SetCharacterInUseItem(ItemBase Item)
    {
        CurrentItem = Item;
    }

    public ItemBase GetCharacterInUseItem()
    {
        return CurrentItem;
    }

    // todo 堆叠清空逻辑
    public bool ClearHandItem(bool fastDrop , bool playerReversed)
    {
        // 初步检查
        if (SlotItemDataList.ContainsKey(CurrentFocusSlot))
        {
            bool removeStack = false;
            
            if (SlotItemDataList[CurrentFocusSlot].StackValue>1)
            {
                SlotItemStatus tempItemStatus = SlotItemDataList[CurrentFocusSlot];
                tempItemStatus.StackValue--;
                SlotItemDataList[CurrentFocusSlot] = tempItemStatus;
                GameHUD.Instance.SlotManagerHUD.UpdateItem(SlotItemDataList);
                return true; // 对于多个的情况
            }

            CurrentItem.CheckReverse(playerReversed);
            CurrentItem.transform.SetParent(GameControl.Instance.GetSceneItemList().transform);
            CurrentItem.OnItemDrop(fastDrop);
            SlotItemDataList.Remove(CurrentFocusSlot);   // 对于一个的情况
            CurrentItem = null;
            ItemID2Key.Remove(CurrentFocusSlot);
            AllCharacterItems.Remove(CurrentFocusSlot);
            GameHUD.Instance.SlotManagerHUD.UpdateItem(SlotItemDataList);
            return false;
        }
        return false;
    }

    public void ChangeFocusSlotNumber(int key)
    {
        CurrentFocusSlot = key;
    }

    public void ChangeFocusSlotNumber(bool next)
    {
        if (next)
        {
            CurrentFocusSlot++;
        }
        else
        {
            CurrentFocusSlot--;
        }

        if (CurrentFocusSlot == 0)
        {
            CurrentFocusSlot = CurrentMaxSlotCount;
        }
        
        if (CurrentFocusSlot == CurrentMaxSlotCount + 1)
        {
            CurrentFocusSlot = 1;
        }
    }
}