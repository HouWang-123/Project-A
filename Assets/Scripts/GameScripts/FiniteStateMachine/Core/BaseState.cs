using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 有限状态机的状态鸡肋
/// </summary>
public abstract class BaseState
{
    // 时间缩放
    protected float m_timeScale;
    public float TimeScale { get { return m_timeScale; } set { m_timeScale = value; } }
    // 一个状态对应一个state的枚举
    protected StateEnum m_stateEnum;

    public StateEnum StateEnum {  get { return m_stateEnum; } }
    // 状态转换对应的状态ID
    protected Dictionary<TransitionEnum, StateEnum> m_keyValuePairs = new ();
    // 有限状态机，方便Condition()函数满足后进行状态转换
    protected FiniteStateMachine m_finiteStateMachine;
    // 所挂载对象的引用
    protected GameObject m_gameObject;
    // 怪物的状态机
    protected MonsterBaseFSM m_monsterBaseFSM;

    public BaseState(FiniteStateMachine finiteStateMachine, GameObject gameObject)
    {
        m_timeScale = 1f;
        m_finiteStateMachine = finiteStateMachine;
        m_gameObject = gameObject;
        m_monsterBaseFSM = gameObject.GetComponent<MonsterBaseFSM>();
    }
    /// <summary>
    /// 添加状态过渡对应的状态到字典中
    /// </summary>
    /// <param name="transition"></param>
    /// <param name="state"></param>
    /// <exception cref="ArgumentException">参数错误</exception>
    public void AddTransition(TransitionEnum transition, StateEnum state)
    {
        if (TransitionEnum.None == transition)
        {
            Debug.LogError(GetType() + "/AddTransition()/ transition is None");
            throw new ArgumentException();
        }
        if (StateEnum.None == state)
        {
            Debug.LogError(GetType() + "/AddTransition()/ state is None");
            throw new ArgumentException();
        }
        if (m_keyValuePairs.ContainsKey(transition))
        {
            Debug.LogError(GetType() + "/AddTransition()/ transition has already existed, transition = " + transition);
            throw new ArgumentException();
        }

        m_keyValuePairs.Add(transition, state);
    }

    /// <summary>
    /// 删除状态过渡对应的状态
    /// </summary>
    /// <param name="transition">状态过度</param>
    /// <exception cref="ArgumentException">参数错误</exception>
    public void RemoveTransition(TransitionEnum transition)
    {
        if (transition == TransitionEnum.None)
        {
            Debug.LogError(GetType() + "/DeleteTransition()/ transition is None");
            throw new ArgumentException();
        }

        if (m_keyValuePairs.ContainsKey(transition) == false)
        {
            Debug.LogError(GetType() + "/DeleteTransition()/ transition has not existed, transition = " + transition);
            throw new ArgumentException();
        }

        m_keyValuePairs.Remove(transition);
    }

    /// <summary>
    /// 删除状态过渡对应的状态
    /// </summary>
    /// <param name="transition">状态过渡</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">参数错误</exception>
    public StateEnum GetStateWithTransition(TransitionEnum transition)
    {
        if (transition == TransitionEnum.None)
        {
            Debug.LogError(GetType() + "/DeleteTransition()/ transition is None");
            throw new ArgumentException();
        }
        if (m_keyValuePairs.ContainsKey(transition))
        {
            return m_keyValuePairs[transition];
        }
        return StateEnum.None;
    }

    // 状态切换进入前的回调
    public virtual void DoBeforeEntering() { }
    // 状态切换退出的状态的回调
    public virtual void DoAfterLeaving() { }
    // 固定帧速率的回调，用于物理相关的逻辑
    public virtual void FixedAct(GameObject npc) { }
    // 固定帧速率的条件
    public virtual void FixedCondition(GameObject npc) { }
    // 执行的回调
    public abstract void Act(GameObject npc);
    // 转换条件
    public abstract void Condition(GameObject npc);
}

