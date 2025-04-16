using System.Collections.Generic;
using UnityEngine;
public class SoundSWGroup : MonoBehaviour
{
    public List<SoundSWMono> switches;

    public enum OpenCondition
    {
        ///奇数 
        XOR,
        /// 偶数
        EvenBlocked,
        /// 所有
        AllTriggered,
        /// 任意一个
        AnyTriggered,
        Custom
    }

    public OpenCondition condition = OpenCondition.XOR;

    private HashSet<SoundSWMono> triggeredThisRound = new HashSet<SoundSWMono>();

    public void RegisterTrigger(SoundSWMono triggered)
    {
        triggeredThisRound.Add(triggered);

        // 所有开关都触发一次声音之后才判断（防止提前判断）
        if (triggeredThisRound.Count < switches.Count)
            return;

        if (EvaluateCondition())
        {
            Debug.Log("满足开门条件！");
        }
        else
        {
            Debug.Log("不满足开门条件");
        }

        ResetAllSwitches();
    }

    bool EvaluateCondition()
    {
        int blockedCount = 0;
        foreach (var s in switches)
        {
            if (s.IsBlocked) blockedCount++;
        }

        switch (condition)
        {
            case OpenCondition.XOR:
                return blockedCount % 2 == 1; 
            case OpenCondition.EvenBlocked:
                return blockedCount % 2 == 0 && blockedCount > 0;
            case OpenCondition.AllTriggered:
                return switches.TrueForAll(s => s.IsBlocked);
            case OpenCondition.AnyTriggered:
                return switches.Exists(s => s.IsBlocked);
            case OpenCondition.Custom:
                //todo 后续扩展
                return false;
            default:
                return false;
        }
    }


    void ResetAllSwitches()
    {
        triggeredThisRound.Clear();
        foreach (var s in switches)
        {
            s.ResetTrigger();
        }
    }
    public void ResetGroup()
    {
        // 清空触发记录
        triggeredThisRound.Clear();

        // 重置每个开关状态
        foreach (var s in switches)
        {
            s.ResetTrigger();
        }

        // todo 重置门

        Debug.Log($"VoiceSwitchGroup '{gameObject.name}' 已重置。");
    }

}