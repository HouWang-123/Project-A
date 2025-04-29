using System.Collections.Generic;
using cfg.interact;
public static class GameItemInteractionHub
{
    public static Dictionary<int, List<int>> InterActionList = new ();
    public static Dictionary<(int, int), int> FindEffectById = new ();
    public static void InitInteraction(InteractEffect effect)
    {
        FindEffectById.Add( (effect.InteractItemID,effect.ToInteractItemID) , effect.ID);
        if (InterActionList.ContainsKey(effect.InteractItemID))
        {
            InterActionList[effect.InteractItemID].Add(effect.ToInteractItemID);
        }
        else
        {
            InterActionList.Add(effect.InteractItemID,new List<int>(){effect.ToInteractItemID});
        }
    }
    // 获取某个物品可与多少种其他物品交互
    public static int GetItemInteractableItemList(int id) { return InterActionList[id].Count; }
    /// <summary>
    /// 判断物品A与物品B是否存在交互关系。传入ID。注: ItemA 为玩家手持的物品，ItemB为待交互物品，空手传入-1
    ///
    /// 2025年4月29日 17点43分 添加 id -2, 配置到表中，表示通用交互，即忽略玩家手中的物品
    /// </summary>
    /// <param name="ItemA"></param>
    /// <param name="ItemB"></param>
    /// <returns></returns>
    public static bool HasInteract(int ItemA, int ItemB)
    {
        if (!InterActionList.ContainsKey(ItemA)) return false;
        if (InterActionList[-2].Contains(ItemB)) return true;
        if (InterActionList[ItemA].Contains(ItemB)) return true;
        return false;
    }

    /// <summary>
    /// 获取交互ID。传入ID。注: ItemA 为玩家手持的物品，ItemB为待交互物品，空手传入-1
    /// </summary>
    /// <param name="ItemA"></param>
    /// <param name="ItemB"></param>
    /// <returns></returns>
    public static int FindInteractEffectById(int ItemA, int ItemB)
    {
        if (FindEffectById.ContainsKey((ItemA,ItemB)))
        {
            return FindEffectById[(ItemA, ItemB)];
        }
        else
        {
            return FindEffectById[(-2, ItemB)];
        }
    }
}
