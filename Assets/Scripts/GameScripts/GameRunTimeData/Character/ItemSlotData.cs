using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SlotItem
{
    public SlotItem(int itemID, int stackValue, ItemBase itemBase)
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
    
    private Dictionary<int, ItemBase> AllSlotItems = new Dictionary<int, ItemBase>(); // Number,Item
    
    private int CurrentFocusSlot; // 玩家手中的物品对应的Number Key
    
    public void SetMaxSlotCount(int number)
    {
        CurrentMaxSlotCount = number;
    }

    public void ActiveCurrentItem()
    {
        foreach (var V in AllSlotItems)
        {
            V.Value.gameObject.SetActive(false);
        }

        if (AllSlotItems.ContainsKey(CurrentFocusSlot))
        {
            AllSlotItems[CurrentFocusSlot].gameObject.SetActive(true);
            SetCharacterInUseItem(AllSlotItems[CurrentFocusSlot]);
        }
        else
        {
            SetCharacterInUseItem(null);
        }

    }
    /// <summary>
    /// 注：int下标从1开始，对应道具栏1~6
    /// </summary>
    private Dictionary<int, SlotItem> ItemList = new ();
    /// <summary>
    /// 物品id对应道具栏1~6对应的
    /// </summary>
    private Dictionary<int, int> ItemID2Key = new ();

    /// <summary>
    /// 获取合适的SlotNumber
    /// </summary>
    /// <param name="InsertItem"></param>
    /// <returns></returns>
    private int GetItemProperIndex(ItemBase InsertItem)
    {
        // 可否堆叠
        if (InsertItem.GetType().IsAssignableFrom(typeof(IStackable)))
        {
            // 遍历道具栏
            foreach (var VARIABLE in ItemList)
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
        return FindEmptySlot();
    }

    /// <summary>
    /// 获取空Slot位置
    /// </summary>
    /// <returns></returns>
    private int FindEmptySlot()
    {
        SlotItem item;
        for (int index = 1; index <= CurrentMaxSlotCount; index++)
        {
            if (!ItemList.TryGetValue(index, out item))
            {
                return index;
            }
        }

        return -1; // 没有合适的空位置
    }

    /// <summary>
    /// 返回 -1 表示插入失败
    /// 返回  0 表示
    /// 已验证
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int InsertOrUpdateItemSlotData(ItemBase item)
    {
        SlotItem NewItem = new SlotItem(item.ItemID, 1,item);
        int slotNumber = CurrentFocusSlot;
        // 获取合适的ItemSlotIndex 包括可否堆叠位置
        if (ItemList.ContainsKey(CurrentFocusSlot))
        {
            slotNumber = GetItemProperIndex(item);
        }
        
        // 插入物品失败
        if (slotNumber == -1)
        {
            return -1;
        }
        
        SlotItem OldItem;
        if (ItemList.TryGetValue(slotNumber, out OldItem))
        {
            // 对原有物品进行更新
            OldItem.StackValue++;
            ItemList[slotNumber] = OldItem;
        }
        else
        {
            // 插入物品成功
            ItemList[slotNumber] = NewItem;
        }
        
        
        AllSlotItems.Add(slotNumber,item);
        ItemID2Key.Add(slotNumber,item.ItemID);
        GameHUD.Instance.SlotManagerHUD.UpdateItem(ItemList);
        
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
    public void ClearHandItem(bool fastDrop , bool playerReversed)
    {
        if (ItemList.ContainsKey(CurrentFocusSlot))
        {
            CurrentItem.CheckReverse(playerReversed);
            
            CurrentItem.transform.SetParent(GameControl.Instance.GetSceneItemList().transform);
            
            if (ItemList[CurrentFocusSlot].StackValue>1)
            {
                SlotItem tempItem = ItemList[CurrentFocusSlot];
                tempItem.StackValue--;
                ItemList[CurrentFocusSlot] = tempItem;
            }
            else
            {
                ItemList.Remove(CurrentFocusSlot);
            }
            CurrentItem.OnItemDrop(fastDrop);
            GameHUD.Instance.SlotManagerHUD.UpdateItem(ItemList);
            CurrentItem = null;
            ItemID2Key.Remove(CurrentFocusSlot);
            AllSlotItems.Remove(CurrentFocusSlot);
        }
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