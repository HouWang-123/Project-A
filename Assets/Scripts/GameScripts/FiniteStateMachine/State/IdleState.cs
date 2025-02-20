using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����״̬
/// </summary>
public class IdleState : BaseState
{
    // �ȴ���ʱ��
    private float m_TimeToWait = 0f;
    // ���λ��
    private readonly Transform m_playerTransform;
    // ת��Ϊ�������״̬����С����
    private readonly float m_warnDistance = -1f;
    // תΪ���ܵĹ�Դ����
    private readonly float m_fleeDistance = -1f;

    public IdleState(FiniteStateMachine finiteStateMachine, GameObject gameObject, Transform playerTransform)
        : base(finiteStateMachine, gameObject)
    {
        // ״̬����
        m_stateEnum = StateEnum.Idle;
        m_playerTransform = playerTransform;
        m_warnDistance = gameObject.GetComponent<MonsterBaseFSM>().MonsterDatas.WarnRange + 5f;
        m_fleeDistance = 3f;
    }

    public override void Act(GameObject npc)
    {
        
    }

    public override void Condition(GameObject npc)
    {
        m_TimeToWait += Time.deltaTime;
        // �ȴ�5��
        if (m_TimeToWait >= 5f)
        {
            // ��ʱ������
            m_TimeToWait = 0f;
            Debug.Log(GetType() + " /Condition() => ������ת��״̬Ϊ��������Ѳ��");
            // �������л�״̬��0.7�ĸ����л���Ѳ��
            if (Random.value > 0.3f)
            {
                // �л���Ѳ��״̬
                m_finiteStateMachine.PerformTransition(TransitionEnum.LostPlayer);
            }
        }
        // תΪ�������
        if (Vector3.Distance(m_playerTransform.position, m_gameObject.transform.position) <= m_warnDistance)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.SeePlayer);
        }
        // ���ֹ�Դֱ������
        var lightTransform = m_gameObject.GetComponent<MonsterBaseFSM>().LightTransform;
        if (lightTransform != null)
        {
            if (Vector3.Distance(lightTransform.position, m_gameObject.transform.position) <= m_fleeDistance)
            {
                m_finiteStateMachine.PerformTransition(TransitionEnum.FleeAction);
            }
        }
    }

    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
        AnimationController.PlayAnim(m_gameObject, StateEnum.Idle, 0, true);
    }
}
