
using System.Collections.Generic;
using cfg.buff;
using UnityEngine;
/// <summary>
/// 一个谜题物品可能存在多条件
/// </summary>
public interface IRiddleItem
{
    public List<RiddleCondition> GetMyRiddleConditions();
    public void UpdateRiddleCondtion() {}
    public bool SelfCheckConditon();
}
