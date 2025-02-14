using cfg.scene;
using System;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using YooAsset;
/// <summary>
/// ׷��״̬
/// </summary>
public class ChaseState : BaseState
{
    // �ж�NPC�Ƿ�ס����ֵʱ��
    public float stuckThreshold = 2f;
    // ���߼��Ĳ㼶������Ϊ��Obstacle���������ֻ����ò�����
    public LayerMask obstacleLayer;
    // ���߼��ǰ���ϰ����������
    public float raycastDistance = 5f;

    // NPC��Ϸ����
    private readonly GameObject NPC;
    // NPC��NavMeshAgent���
    private readonly NavMeshAgent agent;
    // ��¼NPC��ס��ʱ�䣬���ڴ���ǿ������Ѱ·
    private float stuckTimer;
    // ��ҵ�λ��
    private readonly Transform m_playerTransform;
    // ����������
    public float checkDistance = 0.1f;
    // ת��Ϊ�������״̬����С����
    private readonly float m_2LookAtTime = 10f;
    // ת��Ϊ������ҵľ���
    private readonly float m_2Attack = 1.5f;
    public ChaseState(FiniteStateMachine finiteStateMachine, GameObject NPCObj, Transform playerTransform = null) : base(finiteStateMachine)
    {
        // ���õ�ǰ��״̬
        m_stateEnum = StateEnum.Chase;
        if (NPCObj == null)
        {
            Debug.LogError(GetType() + "/PatrolState/ gameObject can not be null!");
            throw new ArgumentException();
        }
        NPC = NPCObj;
        agent = NPCObj.GetComponent<NavMeshAgent>();
        // lastPosition = gameObject.transform.position;
        // ��ʼλ��ǿ��У��
        if (NavMesh.SamplePosition(NPC.transform.position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
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
        SpriteRenderer spriteRenderer = npc.GetComponent<MonsterFSM>().m_spriteRenderer;
        if (playerX - npcX > 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
        // �������
        if (IsPathValid(m_playerTransform.position))
        {
            agent.SetDestination(m_playerTransform.position);
        }
        else
        {
            Debug.Log(GetType() + " /Act()=> ���λ����Ч���˻�Ϊֱ������");
            var direction = (m_playerTransform.position - npc.transform.position).normalized;
            float speed = npc.GetComponent<MonsterFSM>().m_NPCDatas.Speed;
            Vector3 newPos = npc.transform.position + speed * Time.deltaTime * direction;
            npc.transform.position = newPos;
        }
        
        // ���ټ��
        if (agent.velocity.sqrMagnitude < 0.1f)// ȡƽ����ȡģ��
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckThreshold)
            {
                if (IsPathValid(m_playerTransform.position))
                {
                    agent.SetDestination(m_playerTransform.position);
                }
                else
                {
                    Debug.Log(GetType() + " /Act()=> ���λ����Ч���˻�Ϊֱ������");
                    var direction = (m_playerTransform.position - npc.transform.position).normalized;
                    float speed = npc.GetComponent<MonsterFSM>().m_NPCDatas.Speed;
                    Vector3 newPos = npc.transform.position + speed * Time.deltaTime * direction;
                    npc.transform.position = newPos;
                }
                stuckTimer = 0;
            }
        }
        else
        {
            stuckTimer = 0;
        }

        // ʵʱ���߼�⶯̬�ϰ���
        CheckObstacle();
    }

    /// <summary>
    /// �ж��Ƿ��л�������ҵ���������
    /// </summary>
    /// <param name="npc">��Ϸ����</param>
    public override void Condition(GameObject npc)
    {
        // ����10�׾�תΪ�������״̬
        if (Vector3.Distance(npc.transform.position, m_playerTransform.position) > m_2LookAtTime)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.SeePlayer);
        }
        // С��1.5�׾�תΪ�������״̬
        if (Vector3.Distance(npc.transform.position, m_playerTransform.position) <= m_2Attack)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.AttackPlayer);
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
                Debug.DrawLine(NPC.transform.position, target, Color.red, 2f); // ���Ʋ��ɴ�·��
                return false;
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// ʵʱ���߼��ǰ���ϰ���
    /// </summary>
    private void CheckObstacle()
    {
        if (Physics.Raycast(NPC.transform.position, agent.velocity.normalized, raycastDistance, obstacleLayer))
        {
            if (IsPathValid(m_playerTransform.position))
            {
                agent.SetDestination(m_playerTransform.position);
            }
        }
    }
    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
        agent.isStopped = false;
    }

    public override void DoAfterLeaving()
    {
        base.DoAfterLeaving();
        // ֹͣ�ƶ�
        agent.isStopped = true;
        agent.destination = agent.transform.position;
    }
}
