using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
// 房间谜题基类
public enum RiddleConditionType
{
    Bool,
    Int,
    Float
}

public enum RiddleCompareType
{
    Equal,
    Greater,
    Less,
    NotEqual
}
[System.Serializable]
public class RiddleCondition
{
    [HorizontalGroup("Row")]
    [HideLabel, LabelWidth(80)]
    public string RiddleName;

    [HorizontalGroup("Row")]
    [HideLabel, LabelWidth(80)]
    public RiddleConditionType ConditionType;

    [HorizontalGroup("Row")]
    [HideLabel, LabelWidth(100)]
    public RiddleCompareType CompareType;

    [HorizontalGroup("Row")]
    [HideLabel]
    [ShowIf("ConditionType", RiddleConditionType.Bool)]
    public bool BoolValue;

    [HorizontalGroup("Row")]
    [HideLabel]
    [ShowIf("ConditionType", RiddleConditionType.Int)]
    public int IntValue;

    [HorizontalGroup("Row")]
    [HideLabel]
    [ShowIf("ConditionType", RiddleConditionType.Float)]
    public float FloatValue;

    public bool CheckCondition(object value)
    {
        switch (ConditionType)
        {
            case RiddleConditionType.Bool:
                return (bool)value == BoolValue;
            case RiddleConditionType.Int:
                return Compare((int)value, IntValue);
            case RiddleConditionType.Float:
                return Compare((float)value, FloatValue);
            default:
                return false;
        }
    }

    private bool Compare<T>(T current, T target) where T : System.IComparable
    {
        switch (CompareType)
        {
            case RiddleCompareType.Equal: return current.CompareTo(target) == 0;
            case RiddleCompareType.NotEqual: return current.CompareTo(target) != 0;
            case RiddleCompareType.Greater: return current.CompareTo(target) > 0;
            case RiddleCompareType.Less: return current.CompareTo(target) < 0;
            default: return false;
        }
    }
}

