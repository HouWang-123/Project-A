using cfg.scene;
using System;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using YooAsset;
/// <summary>
/// 追逐状态
/// </summary>
public class ChaseState : BaseState
{
    // 判断NPC是否卡住的阈值时间
    public float stuckThreshold = 2f;
    // 射线检测的层级，设置为“Obstacle”层后，射线只会检测该层物体
    public LayerMask obstacleLayer;
    // 射线检测前方障碍物的最大距离
    public float raycastDistance = 5f;

    // NPC游戏对象
    private readonly GameObject NPC;
    // NPC的NavMeshAgent组件
    private readonly NavMeshAgent agent;
    // 记录NPC卡住的时间，用于触发强制重新寻路
    private float stuckTimer;
    // 玩家的位置
    private readonly Transform m_playerTransform;
    // 调整检测距离
    public float checkDistance = 0.1f;
    // 转换为看向玩家状态的最小距离
    private readonly float m_2LookAtTime = 10f;
    // 转换为攻击玩家的距离
    private readonly float m_2Attack = 1.5f;
    public ChaseState(FiniteStateMachine finiteStateMachine, GameObject NPCObj, Transform playerTransform = null) : base(finiteStateMachine)
    {
        // 设置当前的状态
        m_stateEnum = StateEnum.Chase;
        if (NPCObj == null)
        {
            Debug.LogError(GetType() + "/PatrolState/ gameObject can not be null!");
            throw new ArgumentException();
        }
        NPC = NPCObj;
        agent = NPCObj.GetComponent<NavMeshAgent>();
        // lastPosition = gameObject.transform.position;
        // 初始位置强制校正
        if (NavMesh.SamplePosition(NPC.transform.position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position); // 确保Agent位置正确
        }
        else
        {
            Debug.LogError("NPC初始位置无效！请检查NavMesh烘焙。");
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
    /// 执行追逐动作
    /// </summary>
    /// <param name="npc">游戏对象</param>
    public override void Act(GameObject npc)
    {
        // npc.transform.LookAt(m_playerTransform);
        float npcX = npc.transform.position.x;
        float playerX = m_playerTransform.position.x;
        // 玩家在NPC左边，看向左边
        SpriteRenderer spriteRenderer = npc.GetComponent<MonsterFSM>().m_spriteRenderer;
        if (playerX - npcX > 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
        // 跟着玩家
        if (IsPathValid(m_playerTransform.position))
        {
            agent.SetDestination(m_playerTransform.position);
        }
        else
        {
            Debug.Log(GetType() + " /Act()=> 玩家位置无效，退化为直线行走");
            var direction = (m_playerTransform.position - npc.transform.position).normalized;
            float speed = npc.GetComponent<MonsterFSM>().m_NPCDatas.Speed;
            Vector3 newPos = npc.transform.position + speed * Time.deltaTime * direction;
            npc.transform.position = newPos;
        }
        
        // 卡顿检测
        if (agent.velocity.sqrMagnitude < 0.1f)// 取平方比取模快
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
                    Debug.Log(GetType() + " /Act()=> 玩家位置无效，退化为直线行走");
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

        // 实时射线检测动态障碍物
        CheckObstacle();
    }

    /// <summary>
    /// 判断是否切换看向玩家的条件监听
    /// </summary>
    /// <param name="npc">游戏对象</param>
    public override void Condition(GameObject npc)
    {
        // 大于10米就转为看向玩家状态
        if (Vector3.Distance(npc.transform.position, m_playerTransform.position) > m_2LookAtTime)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.SeePlayer);
        }
        // 小于1.5米就转为攻击玩家状态
        if (Vector3.Distance(npc.transform.position, m_playerTransform.position) <= m_2Attack)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.AttackPlayer);
        }
        // 玩家出了巡逻范围，就进行游荡
        // 在玩家位置周围检测NavMesh
        if (m_playerTransform.gameObject.TryGetComponent<NavMeshAgent>(out var navAgentPlayer))
        {
            bool isInside = navAgentPlayer.isOnNavMesh;
            if (!isInside)
            {
                Debug.Log("玩家已走出NavMesh范围！");
                m_finiteStateMachine.PerformTransition(TransitionEnum.LostPlayer);
            }
        }
    }
    /// <summary>
    /// 检查目标点是否可达
    /// </summary>
    /// <param name="target">目标点</param>
    /// <returns></returns>
    private bool IsPathValid(Vector3 target)
    {
        NavMeshPath path = new();
        if (agent.CalculatePath(target, path))
        {
            if (path.status != NavMeshPathStatus.PathComplete)
            {
                Debug.DrawLine(NPC.transform.position, target, Color.red, 2f); // 绘制不可达路径
                return false;
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// 实时射线检测前方障碍物
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
        // 停止移动
        agent.isStopped = true;
        agent.destination = agent.transform.position;
    }
}
