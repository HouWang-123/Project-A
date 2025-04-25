using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
/// <summary>
/// 谜题场景，可能存在多个物品
/// </summary>
public class RiddleManager : MonoBehaviour
{
    [TableList(ShowIndexLabels = true)]
    [LabelText("谜题物品列表")]
    public List<GameObject> RiddleItems = new();

    /// <summary>
    /// 通过事件或外部调用更新条件
    /// </summary>
    public bool ValidateItemCondition(GameObject riddleItem, object value)
    {
        var item = RiddleItems.Find(c => c == riddleItem);
        if (item != null)
        {
            IRiddleItem Riddle = item.GetComponent<IRiddleItem>();
            if (Riddle == null)
            {
                Debug.LogError("谜题物品没有实现相关接口 RiddleItem");
                return false;
            }
            bool selfCheckConditon = Riddle.SelfCheckConditon();
            return selfCheckConditon;
        }
        else
        {
            return false;
        }
    }
    public bool ValidateAllItemCondition()
    {
        foreach (var riddleItem in RiddleItems)
        {
            IRiddleItem Riddle = riddleItem.GetComponent<IRiddleItem>();
            if (Riddle == null)
            {
                Debug.LogError("谜题物品没有实现相关接口 RiddleItem");
                break;
            }
            bool selfCheckConditon = Riddle.SelfCheckConditon();
            if (!selfCheckConditon)
            {
                return false;
            }
        }
        return true;
    }
}