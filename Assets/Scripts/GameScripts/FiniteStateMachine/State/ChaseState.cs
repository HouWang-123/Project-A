using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
/// <summary>
/// ׷��״̬
/// </summary>
public class ChaseState : BaseState
{
    // ��ʱ��
    private float m_timer = 0f;
    // �ж�NPC�Ƿ�ס����ֵʱ��
    public float stuckThreshold = 4f;
    // NPC��NavMeshAgent���
    private readonly NavMeshAgent agent;
    // ��¼NPC��ס��ʱ�䣬���ڴ���ǿ������Ѱ·
    private float stuckTimer;
    // ��ҵ�λ��
    private readonly Transform m_playerTransform;
    // ת��Ϊ�������״̬����С����
    private readonly float m_2LookAtDistance = -1f;
    // Զ�̹�������
    private readonly float m_2RangedAttack = -1f;
    // ת��Ϊ������ҵľ���
    private readonly float m_2MeleeAttack = -1f;
    // תΪ���ܵĹ�Դ����
    private readonly float m_fleeDistance = -1f;
    // ��������ʱ��
    private float m_fuckingTime = 0f;
    // �����CDʱ��
    private readonly float m_fuckingCD = 10f;
    // ������
    private bool m_fucking = false;
    // �����˳���
    private bool m_fuckedAnimPlayed = false;
    public ChaseState(FiniteStateMachine finiteStateMachine, GameObject NPCObj, Transform playerTransform = null)
        : base(finiteStateMachine, NPCObj)
    {
        // ���õ�ǰ��״̬
        m_stateEnum = StateEnum.Chase;
        if (NPCObj == null)
        {
            Debug.LogError(GetType() + "/PatrolState/ gameObject can not be null!");
            throw new ArgumentException();
        }
        m_gameObject = NPCObj;
        agent = NPCObj.GetComponent<MonsterBaseFSM>().NavMeshAgent;
        m_2LookAtDistance = m_gameObject.GetComponent<MonsterBaseFSM>().MonsterDatas.WarnRange;
        m_2RangedAttack = m_gameObject.GetComponent<MonsterBaseFSM>().MonsterDatas.ShootRange;
        m_2MeleeAttack = m_gameObject.GetComponent<MonsterBaseFSM>().MonsterDatas.HitRange;
        // lastPosition = gameObject.transform.position;
        // ��ʼλ��ǿ��У��
        if (NavMesh.SamplePosition(NPCObj.transform.position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position); // ȷ��Agentλ����ȷ
        }
        else
        {
            Debug.LogError("NPC��ʼλ����Ч������NavMesh�決��");
        }

        if (playerTransform != null)
        {
            m_playerTransform = playerTransform;
        }
        else
        {
            m_playerTransform = GameObject.Find("Player000").transform;
        }
        m_fleeDistance = 3f;
    }

    /// <summary>
    /// ִ��׷����
    /// </summary>
    /// <param name="npc">��Ϸ����</param>
    public override void Act(GameObject npc)
    {
        // npc.transform.LookAt(m_playerTransform);
        float npcX = npc.transform.position.x;
        float playerX = m_playerTransform.position.x;
        // �����NPC��ߣ��������
        // �����ұߣ������ұ�
        Vector3 scale;
        if (playerX - npcX > 0f)
        {
            scale = GameConstData.NormalScale;
        }
        else
        {
            scale = GameConstData.ReverseScale;
        }
        m_gameObject.transform.localScale = scale;
        // �������
        if (IsPathValid(m_playerTransform.position))
        {
            agent.SetDestination(m_playerTransform.position);
        }
        // 10%���ʴ�������
        if (!m_fucking && Random.value <= 0.1f)
        {
            m_fucking = true;
            m_fuckedAnimPlayed = true;
            agent.isStopped = true;
            Debug.Log(GetType() + " /Act() => �����˳�����");
            AnimationController.PlayAnim(m_gameObject, StateEnum.Idle, 0, false);
            var monsterFSM = m_gameObject.GetComponent<MonsterBaseFSM>();
            m_fuckingTime = AnimationController.AnimationTotalTime(monsterFSM.SkeletonAnim);
        }
        if (m_fucking)
        {
            m_timer += Time.deltaTime * m_timeScale;
        }
        if (m_fuckedAnimPlayed && m_timer > m_fuckingTime)
        {
            m_fuckedAnimPlayed = false;
            agent.isStopped = false;
            AnimationController.PlayAnim(m_gameObject, StateEnum.Chase, 0, true, m_timeScale);
        }
        // ʮ��CD
        if (m_timer > m_fuckingTime + m_fuckingCD)
        {
            m_timer = 0f;
            m_fucking = false;
        }
        /*else
        {
            Debug.Log(GetType() + " /Act()=> ���λ����Ч���˻�Ϊֱ������");
            var direction = (m_playerTransform.position - npc.transform.position).normalized;
            float speed = npc.GetComponent<MonsterFSM>().NPCDatas.Speed;
            Vector3 newPos = npc.transform.position + speed * Time.deltaTime * direction;
            npc.transform.position = newPos;
        }*/

        // ���ټ��
        if (agent.velocity.sqrMagnitude < 0.05f)// ȡƽ����ȡģ��
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckThreshold)
            {
                if (IsPathValid(m_playerTransform.position))
                {
                    agent.SetDestination(m_playerTransform.position);
                }
                /*else
                {
                    Debug.Log(GetType() + " /Act()=> ���λ����Ч���˻�Ϊֱ������");
                    var direction = (m_playerTransform.position - npc.transform.position).normalized;
                    float speed = npc.GetComponent<MonsterFSM>().NPCDatas.Speed;
                    Vector3 newPos = npc.transform.position + speed * Time.deltaTime * direction;
                    npc.transform.position = newPos;
                }*/
                stuckTimer = 0;
            }
        }
        else
        {
            stuckTimer = 0;
        }
    }

    /// <summary>
    /// �ж��Ƿ��л�������ҵ���������
    /// </summary>
    /// <param name="npc">��Ϸ����</param>
    public override void Condition(GameObject npc)
    {
        float distance = Vector3.Distance(npc.transform.position, m_playerTransform.position);
        // ����m_2LookAtDistance�׾�תΪ�������״̬
        if (distance > m_2LookAtDistance)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.SeePlayer);
        }
        // С��m_2Attack�׾�תΪ��ս�������״̬
        if (m_2MeleeAttack != -1 && distance <= m_2MeleeAttack)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.MeleeAttackPlayer);
        }
        // С��m_2Attack�׾�תΪԶ�̹������״̬
        if (m_2RangedAttack != -1 && distance <= m_2RangedAttack)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.RangedAttackPlayer);
        }
        // ��ҳ���Ѳ�߷�Χ���ͽ����ε�
        // �����λ����Χ���NavMesh
        if (m_playerTransform.gameObject.TryGetComponent<NavMeshAgent>(out var navAgentPlayer))
        {
            bool isInside = navAgentPlayer.isOnNavMesh;
            if (!isInside)
            {
                Debug.Log("������߳�NavMesh��Χ��");
                m_finiteStateMachine.PerformTransition(TransitionEnum.LostPlayer);
            }
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
    /// <summary>
    /// ���Ŀ����Ƿ�ɴ�
    /// </summary>
    /// <param name="target">Ŀ���</param>
    /// <returns></returns>
    private bool IsPathValid(Vector3 target)
    {
        NavMeshPath path = new();
        if (agent.CalculatePath(target, path))
        {
            if (path.status != NavMeshPathStatus.PathComplete)
            {
                Debug.DrawLine(m_gameObject.transform.position, target, Color.red, 2f); // ���Ʋ��ɴ�·��
                return false;
            }
            return true;
        }
        return false;
    }

    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
        agent.isStopped = false;
        AnimationController.PlayAnim(m_gameObject, StateEnum.Chase, 0, true, m_timeScale);
    }

    public override void DoAfterLeaving()
    {
        base.DoAfterLeaving();
        // ֹͣ�ƶ�
        agent.isStopped = true;
        agent.destination = agent.transform.position;
    }
}
