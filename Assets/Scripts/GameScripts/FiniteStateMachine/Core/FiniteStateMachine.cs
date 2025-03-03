using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// 有限状态机
/// </summary>
public class FiniteStateMachine
{
    // 所有状态
    private readonly Dictionary<StateEnum, BaseState> m_keyValuePairs = new ();
    // 当前状态
    private (StateEnum, BaseState) m_currentState;

    /// <summary>
    /// 进行更新
    /// </summary>
    /// <param name="gameObject">游戏物体</param>
    public void DoUpdate(GameObject gameObject)
    {
        m_currentState.Item2.Act(gameObject);
        m_currentState.Item2.Condition(gameObject);
    }
    /// <summary>
    /// 进行固定更新
    /// </summary>
    /// <param name="gameObject">游戏物体</param>
    public void DoFixedUpdate(GameObject gameObject)
    {
        m_currentState.Item2.FixedAct(gameObject);
        m_currentState.Item2.FixedCondition(gameObject);
    }

    /// <summary>
    /// 添加状态到字典中
    /// </summary>
    /// <param name="fsmState">状态</param>
    public void AddState(BaseState fsmState)
    {
        if (fsmState == null)
        {
            Debug.LogError(GetType() + "/AddState()/ fsmState is null");
            throw new ArgumentException();
        }


        if (m_keyValuePairs.ContainsKey(fsmState.StateEnum))
        {
            Debug.LogError(GetType() + "/AddState()/ m_keyValuePairs had already existed, fsmState.StateEnum = " + fsmState.StateEnum);
            throw new ArgumentException();
        }

        // 设置默认状态
        if (m_currentState.Item2 == null)
        {
            m_currentState.Item2 = fsmState;
            m_currentState.Item1 = fsmState.StateEnum;
        }

        m_keyValuePairs.Add(fsmState.StateEnum, fsmState);
    }

    /// <summary>
    /// 删除状态
    /// </summary>
    /// <param name="StateEnum">状态枚举</param>
    public void DeleteState(StateEnum StateEnum)
    {
        if (StateEnum == StateEnum.None)
        {
            Debug.LogError(GetType() + "/DeleteState()/ StateEnum is None");
            throw new ArgumentException();
        }


        if (m_keyValuePairs.ContainsKey(StateEnum) == false)
        {
            Debug.LogError(GetType() + "/DeleteState()/ m_keyValuePairs had not existed, StateEnum = " + StateEnum);
            throw new ArgumentException();
        }

        m_keyValuePairs.Remove(StateEnum);
    }

    /// <summary>
    /// 执行转换到某个状态
    /// </summary>
    /// <param name="transition">状态过渡</param>
    public void PerformTransition(TransitionEnum transition)
    {
        if (transition == TransitionEnum.None)
        {
            Debug.LogError(GetType() + "/PerformTransition()/ transition is None");
            throw new ArgumentException();
        }

        // 获取当前状态下对应转换的状态
        StateEnum stateEnum = m_currentState.Item2.GetStateWithTransition(transition);
        if (stateEnum == StateEnum.None)
        {
            Debug.LogWarning(GetType() + "/PerformTransition()/ stateEnum form GetOutputState(transition) is None");
            return;
        }
        if (!m_keyValuePairs.ContainsKey(stateEnum))
        {
            Debug.LogError(GetType() + "/PerformTransition()/ m_keyValuePairs have not stateEnum，stateEnum = " + stateEnum);
            throw new ArgumentException();
        }

        // 退出当前状态前的回调
        m_currentState.Item2?.DoAfterLeaving();
        // 更新状态
        BaseState state = m_keyValuePairs[stateEnum];
        m_currentState.Item1 = stateEnum;
        m_currentState.Item2 = state;
        Debug.Log(GetType() + "/PerformTransition()/ 当前状态为：" + stateEnum.ToString());
        // 切换后当前状态的进入前的回调
        m_currentState.Item2?.DoBeforeEntering();
    }

    public void TimeScale(float scale)
    {
        m_currentState.Item2.TimeScale = scale;
    }
}
