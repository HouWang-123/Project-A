using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����״̬��
/// </summary>
public class FiniteStateMachine
{
    // ����״̬
    private readonly Dictionary<StateEnum, BaseState> m_keyValuePairs = new ();
    // ��ǰ״̬
    private (StateEnum, BaseState) m_currentState;

    /// <summary>
    /// ���и���
    /// </summary>
    /// <param name="gameObject">��Ϸ����</param>
    public void DoUpdate(GameObject gameObject)
    {
        m_currentState.Item2.Act(gameObject);
        m_currentState.Item2.Condition(gameObject);
    }
    /// <summary>
    /// ���й̶�����
    /// </summary>
    /// <param name="gameObject">��Ϸ����</param>
    public void DoFixedUpdate(GameObject gameObject)
    {
        m_currentState.Item2.FixedAct(gameObject);
        m_currentState.Item2.FixedCondition(gameObject);
    }

    /// <summary>
    /// ���״̬���ֵ���
    /// </summary>
    /// <param name="fsmState">״̬</param>
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

        // ����Ĭ��״̬
        if (m_currentState.Item2 == null)
        {
            m_currentState.Item2 = fsmState;
            m_currentState.Item1 = fsmState.StateEnum;
        }

        m_keyValuePairs.Add(fsmState.StateEnum, fsmState);
    }

    /// <summary>
    /// ɾ��״̬
    /// </summary>
    /// <param name="StateEnum">״̬ö��</param>
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
    /// ִ��ת����ĳ��״̬
    /// </summary>
    /// <param name="transition">״̬����</param>
    public void PerformTransition(TransitionEnum transition)
    {
        if (transition == TransitionEnum.None)
        {
            Debug.LogError(GetType() + "/PerformTransition()/ transition is None");
            throw new ArgumentException();
        }

        // ��ȡ��ǰ״̬�¶�Ӧת����״̬
        StateEnum stateEnum = m_currentState.Item2.GetStateWithTransition(transition);
        if (stateEnum == StateEnum.None)
        {
            Debug.LogWarning(GetType() + "/PerformTransition()/ stateEnum form GetOutputState(transition) is None");
            return;
        }
        if (!m_keyValuePairs.ContainsKey(stateEnum))
        {
            Debug.LogError(GetType() + "/PerformTransition()/ m_keyValuePairs have not stateEnum��stateEnum = " + stateEnum);
            throw new ArgumentException();
        }

        // �˳���ǰ״̬ǰ�Ļص�
        m_currentState.Item2?.DoAfterLeaving();
        // ����״̬
        BaseState state = m_keyValuePairs[stateEnum];
        m_currentState.Item1 = stateEnum;
        m_currentState.Item2 = state;
        Debug.Log(GetType() + "/PerformTransition()/ ��ǰ״̬Ϊ��" + stateEnum.ToString());
        // �л���ǰ״̬�Ľ���ǰ�Ļص�
        m_currentState.Item2?.DoBeforeEntering();
    }

    public void TimeScale(float scale)
    {
        m_currentState.Item2.TimeScale = scale;
    }
}
