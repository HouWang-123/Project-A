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
    /// <summary>
    /// 注：int下标从1开始，对应道具栏1~6
    /// </summary>
    private Dictionary<int, SlotItemStatus> SlotItemDataList = new ();
    /// <summary>
    /// 物品id对应道具栏1~6对应的
    /// </summary>
    private Dictionary<int, int> ItemID2Key = new ();
    
    
    private int CurrentFocusSlot; // 玩家手中的物品对应的Number Key
    public int GetCurrentFocusSlot()
    {
        return CurrentFocusSlot;
    }
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
    /// 为可堆叠物品获取合适的SlotNumber
    /// </summary>
    /// <param name="InsertItem"></param>
    /// <returns></returns>
    private int GetItemProperStackKeyNumber(ItemBase InsertItem, int Count, out bool outOfBound, out int extraVal)
    {
        IStackable iStackable = InsertItem as IStackable;
        // 遍历道具栏
        foreach (var VARIABLE in SlotItemDataList)
        {
            // 判断是否同一个物品
            if (VARIABLE.Value.ItemID == InsertItem.ItemID)
            {
                // 堆叠超出
                if (VARIABLE.Value.StackValue == iStackable.GetMaxStackValue())
                {
                    continue;
                }
                if (VARIABLE.Value.StackValue + Count >= iStackable.GetMaxStackValue())
                {
                    extraVal = VARIABLE.Value.StackValue + Count - iStackable.GetMaxStackValue();
                    VARIABLE.Value.StackValue = iStackable.GetMaxStackValue();
                    outOfBound = true;
                    return FindEmptySlot();
                }
                
                // 堆叠没超出
                if (iStackable.GetMaxStackValue() > VARIABLE.Value.StackValue + Count)
                {
                    VARIABLE.Value.StackValue += iStackable.GetStackCount();
                    outOfBound = false;
                    extraVal = 0;
                    return VARIABLE.Key;
                }
            }
        }
        outOfBound = true;
        extraVal = Count;
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
    /// 返回  2 表示插入堆叠物品
    /// 返回  3 表示第一个进入背包的堆叠物品
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int InsertOrUpdateItemSlotData(ItemBase item)
    {
        int res;
        if (item is IStackable)
        {
            res =  InsertOrUpdateItemSlotData_Stack(item);
        }
        else
        {
            res = InsertOrUpdateItemSlotData_Non_Stack(item);
        }
        GameHUD.Instance.SlotManagerHUD.UpdateItem(SlotItemDataList);
        return res;
    }
    
    public int InsertOrUpdateItemSlotData_Stack(ItemBase item)
    {
        IStackable stackable = item as IStackable;
        SlotItemStatus newItemStatus = new SlotItemStatus(item.ItemID, stackable.GetStackCount(),item);
        bool outofBound = false;
        // 默认插入位置为当前焦点位置
        int slotNumber = CurrentFocusSlot;
        int extralVal = 0;
        if (ItemID2Key.ContainsValue(item.ItemID))
        {
            slotNumber = GetItemProperStackKeyNumber(item,stackable.GetStackCount(),out outofBound,out extralVal);
            if (slotNumber == -1)
            {
                stackable.ChangeStackCount(extralVal);
                if (extralVal == 0)
                {
                    return 2;
                }
                return -1;
            }
            if (outofBound && extralVal >0)
            {
                // 判断当前焦点位置是否存在物品
                newItemStatus.StackValue = extralVal;
                newItemStatus.ItemBase = item;
                bool keepStackItemOnHand = !SlotItemDataList.ContainsKey(CurrentFocusSlot);
                SlotItemDataList[slotNumber] = newItemStatus;
                ItemID2Key.Add(slotNumber,item.ItemID);
                AllCharacterItems.Add(slotNumber,item);
                if (keepStackItemOnHand)
                {
                    SetCharacterInUseItem(item);
                    return 0;
                }
            }
            else
            {
                return 2;
            }
        }
        else
        {
            /////////////////////////////////////  一个堆叠进入情况
            slotNumber = FindEmptySlot();
            SetCharacterInUseItem(item);
            AllCharacterItems.Add(slotNumber,item);
            ItemID2Key.Add(slotNumber,item.ItemID);
            SlotItemDataList[slotNumber] = newItemStatus;
            if (slotNumber != CurrentFocusSlot)
            {
                return 1;
            }
            return 0;
        }
        return 1;
    }
    private int InsertOrUpdateItemSlotData_Non_Stack(ItemBase item)
    {
        SlotItemStatus newItemStatus = new SlotItemStatus(item.ItemID, 1,item);
        
        // 默认插入位置为当前焦点位置
        int slotNumber = CurrentFocusSlot;
        if (SlotItemDataList.ContainsKey(slotNumber))
        {
            slotNumber = FindEmptySlot();
        }
        // 插入物品失败，取消拾取动作
        if (slotNumber == -1)
        {
            return -1;
        }
        
        AllCharacterItems.Add(slotNumber,item);
        ItemID2Key.Add(slotNumber,item.ItemID);
        SlotItemDataList[slotNumber] = newItemStatus;
        if (slotNumber != CurrentFocusSlot)
        {
            if (SlotItemDataList.ContainsKey(CurrentFocusSlot))
            {
                return 1; // 手中存在物品
            }
            SetCharacterInUseItem(item);
            return 0;
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