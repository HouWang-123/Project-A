using System.Collections.Generic;
using UnityEngine;
// 房间谜题基类
public abstract class RoomRiddleMonoBase : MonoBehaviour
{
    [SerializeField]
    [Header("谜题物体")]
    protected List<GameObject> riddleGameObjects;
    protected bool isRiddleReady = false;

    protected virtual void Awake()
    {
        // 确保列表已初始化
        riddleGameObjects ??= new List<GameObject>();
        // 若列表为空，自动查找标签物体
        if (riddleGameObjects.Count == 0)
        {
            GameObject.FindGameObjectsWithTag(GameConstData.RIDDLE_TAG, riddleGameObjects);
        }
    }

    protected virtual void Start()
    {

    }
    /// <summary>
    /// 谜题的前提条件
    /// </summary>
    /// <returns>是否满足谜题的前提条件</returns>
    protected abstract bool BeforeCondition();
    protected virtual void SetRiddleAction()
    {
        isRiddleReady = true;
    }
    /// <summary>
    /// 设置谜题
    /// </summary>
    protected virtual bool SetRiddle()
    {
        if (!BeforeCondition())
        {
            isRiddleReady = false;
        }
        SetRiddleAction();
        return isRiddleReady;
    }
    public virtual void DoRiddle()
    {
        if (SetRiddle())
        {
            Debug.Log(GetType() + "DoRiddle() => 谜题已设置");
        }
        else
        {
            Debug.Log(GetType() + "DoRiddle() => 谜题未设置");
        }
    }
}

