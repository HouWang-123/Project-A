using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class SlotItemStatus
{
    public SlotItemStatus(int itemID, int stackValue)
    {
        ItemID = itemID;
        StackValue = stackValue;
    }
    public SlotItemStatus(int itemID, int stackValue, int key)
    {
        Key = key;
        ItemID = itemID;
        StackValue = stackValue;
    }
    public int ItemID;
    public int StackValue; // 最小为1
    public int Key; // 对应道具栏格位
}

public class ItemSlotData
{
    // =================================================================================================================
    // ======================================       Members       ======================================================
    // =================================================================================================================
    private int CurrentMaxSlotCount;
    private int CurrentFocusSlot; // 玩家手中的物品对应的Number Key
    /// <summary>
    /// 注：int下标从1开始，对应道具栏1~6
    /// </summary>
    private Dictionary<int, SlotItemStatus> SlotItemDataList = new();
    
    // =================================================================================================================
    // ===========================================   Getters&Setters   =================================================
    // =================================================================================================================
    public int GetCurrentFocusSlot()
    {
        return CurrentFocusSlot;
    }
    public void ChangeFocusSlotNumber(int key)
    {
        CurrentFocusSlot = key;
    }

    public void ChangeFocusSlotNumber(bool next)
    {
        int delta = next ? 1 : -1;
        CurrentFocusSlot = ((CurrentFocusSlot - 1 + delta + CurrentMaxSlotCount) % CurrentMaxSlotCount) + 1;
    }
    // 设置最大道具栏数量
    public void SetMaxSlotCount(int number)
    {
        CurrentMaxSlotCount = number;
    }
    // 获取当前道具栏焦点位置的物品ID
    public int GetCurrentFocusedItemId()
    {
        if (SlotItemDataList.TryGetValue(CurrentFocusSlot, out var IslotItemStatus))
        {
            return IslotItemStatus.ItemID;
        }
        return -1;
    }
    // =================================================================================================================
    // ==============================================   InsertProcessor    =============================================
    // =================================================================================================================
    /// <summary>
    /// 返回 -1 表示插入失败
    /// 返回  0 表示空手正常拾取
    /// 返回  1 表示拾取后进入背包
    /// 返回  2 表示插入堆叠物品
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int InsertOrUpdateItemSlotData(ItemBase item, out bool StackOvered)
    {
        int res = -1;
        StackOvered = false;
        if (item is IStackable)
        {
            IStackable StackItem = item as IStackable;
            // 当前可用数量
            int currentPossibleCount = GetMaxPossibleStackableCount(item.ItemID, StackItem.GetMaxStackValue());

            if (StackItem.GetStackCount() > StackItem.GetMaxStackValue()) // 拾取起来的大于最大堆叠
            {
                int InsertItemCount = StackItem.GetStackCount();
                int iteration = 0;
                int lastcount = 0;
                if (currentPossibleCount < StackItem.GetStackCount()) // 应对全物品栏溢出情况
                {
                    StackOvered = true;
                    iteration = currentPossibleCount / StackItem.GetMaxStackValue() + 1;
                    lastcount = currentPossibleCount % StackItem.GetMaxStackValue();
                    for (int i = 0; i < iteration; i++)
                    {
                        if (i == iteration - 1) // 最后一次迭代
                        {
                            StackItem.ChangeStackCount(lastcount);
                            InsertOrUpdateItemSlotData_Stack(item);
                        }
                        else
                        {
                            StackItem.ChangeStackCount(StackItem.GetMaxStackValue());
                            InsertOrUpdateItemSlotData_Stack(item);
                        }
                    }
                    StackItem.ChangeStackCount(InsertItemCount - currentPossibleCount);
                    res = 0;
                }
                else
                {
                    iteration = StackItem.GetStackCount() / StackItem.GetMaxStackValue() + 1;
                    lastcount = StackItem.GetStackCount() % StackItem.GetMaxStackValue();
                    for (int i = 0; i < iteration; i++)
                    {
                        if (i == iteration - 1) // 最后一次迭代
                        {
                            StackItem.ChangeStackCount(lastcount);
                            res = InsertOrUpdateItemSlotData_Stack(item);
                        }
                        else
                        {
                            StackItem.ChangeStackCount(StackItem.GetMaxStackValue());
                            res = InsertOrUpdateItemSlotData_Stack(item);
                        }
                    }
                }
            }
            else
            {
                res = InsertOrUpdateItemSlotData_Stack(item); // 没超过最大堆叠
            }
        }
        else
        {
            res = InsertOrUpdateItemSlotData_Non_Stack(item);
        }
        GameHUD.Instance.slotManager.UpdateItem(SlotItemDataList);
        return res;
    }
    /// <summary>
    /// 插入可堆叠道具
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private int InsertOrUpdateItemSlotData_Stack(ItemBase item)
    {
        IStackable stackable = item as IStackable;
        SlotItemStatus newItemStatus = new SlotItemStatus(item.ItemID, stackable.GetStackCount());
        bool outofBound = false;
        // 默认插入位置为当前焦点位置
        int slotNumber = CurrentFocusSlot;
        int extralVal = 0;
        if ( PlayerHaveItem(item.ItemID) ) // 道具栏中可以进行堆叠整理
        {
            slotNumber = GetItemProperStackKeyNumberAndStackItem(item, stackable.GetStackCount(), out outofBound, out extralVal);
            // 无法放入
            if (slotNumber == -1)
            {
                stackable.ChangeStackCount(extralVal);
                if (extralVal == 0)
                {
                    return 2;
                }
                return -1;
            }
            // 正常溢出到下一个道具槽位
            if (outofBound && extralVal > 0)
            {
                // 判断当前焦点位置是否存在物品
                newItemStatus.StackValue = extralVal;
                SlotItemDataList[slotNumber] = newItemStatus;
                newItemStatus.Key = slotNumber;
                return 2;
            }
        }
        if (SlotItemDataList.ContainsKey(slotNumber))
        {
            slotNumber = Find_A_Empty_Slot();
        }
        SlotItemDataList[slotNumber] = newItemStatus;
        if (slotNumber != CurrentFocusSlot)
        {
            return 1;
        }
        return 0;
    }
    /// <summary>
    /// 为可堆叠物品获取合适的SlotNumber，同时进行道具栏中冗余量的填充
    /// </summary>
    /// <param name="InsertItem"></param>
    /// <returns></returns>
    private int GetItemProperStackKeyNumberAndStackItem(ItemBase InsertItem, int Count, out bool outOfBound, out int extraVal)
    {
        IStackable iStackable = InsertItem as IStackable;
        int maxStacksize = iStackable.GetMaxStackValue();
        // 遍历道具栏
        foreach (var SlotData in SlotItemDataList)
        {
            // 判断是否同一个物品
            if (SlotData.Value.ItemID == InsertItem.ItemID)
            {
                // 是否满载
                if (SlotData.Value.StackValue == maxStacksize) continue; 

                // 堆叠超出
                if (SlotData.Value.StackValue + Count >= maxStacksize)
                {
                    int putted = maxStacksize - SlotData.Value.StackValue; // 计算已经放入的量
                    SlotData.Value.StackValue = maxStacksize; // 设置当前槽为最大
                    Count -= putted; // 数量减去已经放入的
                    extraVal = Count;
                    outOfBound = true;
                }
                else if (iStackable.GetMaxStackValue() > SlotData.Value.StackValue + Count)
                {
                    SlotData.Value.StackValue += iStackable.GetStackCount();
                    outOfBound = false;
                    extraVal = 0;
                    return SlotData.Key;
                }
            }
        }
        outOfBound = true;
        extraVal = Count;
        return Find_A_Empty_Slot();
    }
    
    private int InsertOrUpdateItemSlotData_Non_Stack(ItemBase item)
    {
        // 默认插入位置为当前焦点位置
        int slotNumber = CurrentFocusSlot;
        if (SlotItemDataList.ContainsKey(slotNumber))
        {
            slotNumber = Find_A_Empty_Slot();
        }
        // 插入物品失败，取消拾取动作
        if (slotNumber == -1) { return -1; }
        SlotItemStatus newItemStatus = new SlotItemStatus(item.ItemID, 1, slotNumber);
        SlotItemDataList[slotNumber] = newItemStatus;
        return 0;
    }
    // =================================================================================================================
    // ==============================================   DesertProcessor     ============================================
    // =================================================================================================================
    public bool ClearHandItem(bool fastDrop,Transform releaspt)
    {
        // 初步检查
        if (SlotItemDataList.ContainsKey(CurrentFocusSlot))
        {
            if (SlotItemDataList[CurrentFocusSlot].StackValue > 1)
            {
                SlotItemStatus tempItemStatus = SlotItemDataList[CurrentFocusSlot];
                tempItemStatus.StackValue--;
                SlotItemDataList[CurrentFocusSlot] = tempItemStatus;
                GameHUD.Instance.slotManager.UpdateItem(SlotItemDataList);
                return true; // 对于多个的情况
            }
            SlotItemDataList.Remove(CurrentFocusSlot); // 对于一个的情况
            ItemBase itemOnHand = GameRunTimeData.Instance.CharacterBasicStat.GetStat().ItemOnHand;
            itemOnHand.transform.SetParent(releaspt);
            itemOnHand.transform.localPosition = Vector3.zero;
            itemOnHand.transform.SetParent(GameControl.Instance.GetSceneItemList().transform); // 移动节点
            GameRunTimeData.Instance.ItemManager.RegistItem(itemOnHand);
            itemOnHand.OnItemDrop(fastDrop);
            itemOnHand.ChangeRendererSortingOrder(GameConstData.BelowPlayerOrder);
            GameRunTimeData.Instance.CharacterBasicStat.GetStat().ItemOnHand = null;
            GameHUD.Instance.slotManager.UpdateItem(SlotItemDataList);
        }
        return false;
    }
    // =================================================================================================================
    // ============================================    SlotManagerUtils     ============================================
    // =================================================================================================================
    private bool PlayerHaveItem(int itemId)
    {
        foreach (var VARIABLE in SlotItemDataList.Values)
        {
            if (VARIABLE.ItemID == itemId)
            {
                return true;
            }
        }
        return false;
    }
    private int Find_A_Empty_Slot()
    {
        for (int index = 1; index <= CurrentMaxSlotCount; index++)
        {
            if (!SlotItemDataList.TryGetValue(index, out var itemStatus))
            {
                return index;
            }
        }
        return -1;
    }
    private int GetMaxPossibleStackableCount(int Itemid, int MaxStackCount)
    {
        int eptSlot = CurrentMaxSlotCount - SlotItemDataList.Count;
        int canInsert = 0;
        foreach (var V in SlotItemDataList)
        {
            if (V.Value.ItemID == Itemid)
            {
                canInsert += MaxStackCount - V.Value.StackValue;
            }
        }
        return eptSlot * MaxStackCount + canInsert;
    }
}