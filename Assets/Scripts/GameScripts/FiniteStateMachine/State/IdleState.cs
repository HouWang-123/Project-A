using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����״̬
/// </summary>
public class IdleState : BaseState
{
    // ������ʱ��
    private float m_Timer = 0f;
    // �ȴ���ʱ��
    private float m_TimeToWait = 0f;
    // ģ�⶯������
    public List<string> m_animations;
    // ����������
    private bool m_animPlaying = false;
    // �����������
    private bool m_animationDone = false;
    // ���λ��
    private Transform m_playerTransform;
    // ת��Ϊ�������״̬����С����
    private readonly float m_warnDistance = -1f;
    // תΪ���ܵĹ�Դ����
    private readonly float m_fleeDistance = -1f;

    public IdleState(FiniteStateMachine finiteStateMachine, GameObject gameObject, Transform playerTransform)
        : base(finiteStateMachine, gameObject)
    {
        // ״̬����
        m_stateEnum = StateEnum.Idle;
        // ģ����Ӷ���
        m_animations = new()
        {
            "̧ͷս��",
            "��ͷ����"
        };
        m_playerTransform = playerTransform;
        m_warnDistance = gameObject.GetComponent<MonsterFSM>().NPCDatas.WarnRange + 5f;
        m_fleeDistance = 3f;
    }

    public override void Act(GameObject npc)
    {
        m_Timer += Time.deltaTime;
        // ģ�ⲥ�Ŷ���
        if (!m_animPlaying)
        {
            m_animPlaying = true;
            int randInt = Random.Range(0, m_animations.Count);
            Debug.Log(GetType() + " /Act() => ���Ŷ���: " + m_animations[randInt]);
        }
        // ����ÿ��������������Ҫ3��
        if (m_Timer > 3f && !m_animationDone)
        {
            m_animationDone = true;
            Debug.Log(GetType() + " /Act() => �����������");
            m_Timer = 0f;
        }
    }

    public override void Condition(GameObject npc)
    {
        m_TimeToWait += Time.deltaTime;
        // ����������ɣ��ȴ�5��
        if (m_animationDone && m_TimeToWait >= 5f)
        {
            // ��ʱ������
            m_TimeToWait = 0f;
            m_animPlaying = false;
            m_animationDone = false;
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
        var lightTransform = m_gameObject.GetComponent<MonsterFSM>().LightTransform;
        if (lightTransform != null)
        {
            if (Vector3.Distance(lightTransform.position, m_gameObject.transform.position) <= m_fleeDistance)
            {
                m_finiteStateMachine.PerformTransition(TransitionEnum.FleeAction);
            }
        }
    }
}
