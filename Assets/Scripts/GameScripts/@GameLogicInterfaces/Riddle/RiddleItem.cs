
using System.Collections.Generic;
using cfg.buff;
using UnityEngine;
/// <summary>
/// 一个谜题物品可能存在多条件
/// </summary>
public interface IRiddleItem
{
    public int GetRiddleKey();
    public void SetRiddleKey(int key);
    public RiddleItemBaseStatus GetRiddleStatus();
    public void SetRiddleItemStatus(RiddleItemBaseStatus baseStatus);
    public void SetRiddleManager(RiddleManager riddleManager);
}
