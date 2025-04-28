using System;
using UnityEngine;
using UnityEngine.Events;

public class ExternalValueNode : MonoBehaviour, IRiddleValueNode
{
    // 角色脚本
    [SerializeField]
    private MonoBehaviour targetScript;

    // 角色脚本上要取的字段名
    [SerializeField]
    private string fieldName;

    private Func<int> getter;  // 取数值的方法

    private void Awake()
    {
        if (targetScript != null && !string.IsNullOrEmpty(fieldName))
        {
            var type = targetScript.GetType();
            var field = type.GetField(fieldName);
            if (field != null)
            {
                getter = () => (int)field.GetValue(targetScript);
            }
            else
            {
                var prop = type.GetProperty(fieldName);
                if (prop != null)
                {
                    getter = () => (int)prop.GetValue(targetScript);
                }
                else
                {
                    Debug.LogError($"Field or Property '{fieldName}' not found on {type.Name}");
                }
            }
        }
    }

    public int GetNum()
    {
        if (getter != null)
        {
            return getter();
        }
        Debug.LogError("Getter not initialized.");
        return 0;
    }

    public void SetNum(int num)
    {
        throw new System.NotImplementedException();
    }
}