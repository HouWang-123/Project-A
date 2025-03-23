using System.Collections.Generic;
using UnityEngine;
// 房间谜题基类
public abstract class RoomRiddleMonoBase : MonoBehaviour
{
    [SerializeField]
    [Header("谜题物体")]
    protected List<RoomRiddleItemBase> riddleItems;
    protected bool isRiddleReady = false;

    protected virtual void Awake()
    {
        // 确保列表已初始化
        riddleItems ??= new List<RoomRiddleItemBase>();
        // 若列表为空，自动查找标签物体
        if (riddleItems.Count == 0)
        {
            var riddleItems = transform.GetComponentsInChildren<RoomRiddleItemBase>();
            if (riddleItems != null)
            {
                foreach (var item in riddleItems)
                {
                    this.riddleItems.Add(item);
                }
            }
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
    /// <summary>
    /// 具体如何设置谜题
    /// </summary>
    protected abstract void SetRiddleAction();
    /// <summary>
    /// 检查并设置谜题
    /// </summary>
    public virtual bool SetRiddle()
    {
        if (BeforeCondition())
        {
            Debug.Log(GetType() + "DoRiddle() => 谜题可以进行设置。现在进行设置。。。");
            SetRiddleAction();
            return true;
        }
        else
        {
            Debug.Log(GetType() + "DoRiddle() => 谜题条件未达成，无法设置谜题。。。");
            return false;
        }
    }
    public abstract void DoRiddle();

}

