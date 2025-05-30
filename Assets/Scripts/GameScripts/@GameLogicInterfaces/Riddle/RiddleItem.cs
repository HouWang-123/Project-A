
using System.Collections.Generic;
using cfg.buff;
using UnityEngine;
/// <summary>
/// 一个谜题物品可能存在多条件
/// </summary>
public interface IRiddleItem
{
    public string GetRiddleKey();
    public void SetRiddleKey(string key);
    public RiddleItemBaseStatus GetRiddleStatus();
    public void SetRiddleItemStatus(RiddleItemBaseStatus baseStatus);
    public void SetRiddleManager(RiddleManager riddleManager);
    public bool GetRiddleItemResult();
}

public interface IRiddleNode <T>
{
    public string GetNodeKey();
    public T GetResult();
}

public interface IRiddleValueNode
{
    public int GetNum();
    public void SetNum(int num);
}