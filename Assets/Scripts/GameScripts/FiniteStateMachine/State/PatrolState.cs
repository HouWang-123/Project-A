using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using Transform = UnityEngine.Transform;
/// <summary>
/// 游荡状态
/// </summary>
public class PatrolState : BaseState
{
    // NPC随机游荡的最大半径
    public float wanderRadius = 10f;
    // NPC重新选择目标点的时间间隔
    public float wanderInterval = 8f;
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
    // 记录距离上一次目标点生成的时间
    private float timer = 0f;
    // 记录NPC卡住的时间，用于触发强制重新寻路
    private float stuckTimer;
    // NPC上一帧的位置，用于计算是否卡住
    // private Vector3 lastPosition;
    // 转换为看向玩家状态的最小距离
    private readonly float m_2LookAtTime = 15f;
    // 玩家的位置
    private readonly Transform m_playerTransform;
    public PatrolState(FiniteStateMachine finiteStateMachine, GameObject NPCObj, Transform playerTransform = null) : base(finiteStateMachine)
    {
        // 设置状态
        m_stateEnum = StateEnum.Patrol;
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
    /// 执行游荡动作
    /// </summary>
    /// <param name="npc">游戏对象</param>
    public override void Act(GameObject npc)
    {
        // 随机游荡逻辑
        timer += Time.deltaTime;
        if (timer >= wanderInterval)
        {
            TryFindNewDestination();
            timer = 0;
        }

        // 卡顿检测
        if (agent.velocity.sqrMagnitude < 0.1f)// 取平方比取模快
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckThreshold)
            {
                TryFindNewDestination();
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
    /// 尝试生成新的目标点
    /// </summary>
    private void TryFindNewDestination()
    {
        Vector3 newPos = RandomNavSphere(NPC.transform.position, wanderRadius, -1);
        if (IsPathValid(newPos))
        {
            agent.SetDestination(newPos);
            float newPosX = newPos.x;
            float npcPosX = NPC.transform.position.x;
            // 点在右边，看向右边
            SpriteRenderer spriteRenderer = NPC.GetComponent<MonsterFSM>().m_spriteRenderer;
            if (newPosX - npcPosX > 0)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }
        else
        {
            Debug.Log($"目标点 {newPos} 不可达，已忽略。");
        }
    }
    /// <summary>
    /// 检查目标点是否可达
    /// </summary>
    /// <param name="target">目标点</param>
    /// <returns></returns>
    private bool IsPathValid(Vector3 target)
    {
        NavMeshPath path = new ();
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
    /// 在导航网格上生成随机点
    /// </summary>
    /// <param name="origin">位置</param>
    /// <param name="radius">半径</param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    private Vector3 RandomNavSphere(Vector3 origin, float radius, int layerMask)
    {
        Vector3 randDir = Random.insideUnitSphere * radius;
        randDir += origin;
        // 将随机点投影到导航网格上，确保该点可行走
        if (NavMesh.SamplePosition(randDir, out NavMeshHit navHit, radius, layerMask))
        {
            return navHit.position;
        }
        else
        {
            Debug.LogWarning("随机点生成失败，使用当前位置。");
            return NPC.transform.position;
        }
    }
    /// <summary>
    /// 实时射线检测前方障碍物
    /// </summary>
    private void CheckObstacle()
    {
        if (Physics.Raycast(NPC.transform.position, agent.velocity.normalized, raycastDistance, obstacleLayer))
        {
            TryFindNewDestination();
        }
    }
    /// <summary>
    /// 判断状态是否转换为看向玩家
    /// </summary>
    /// <param name="npc">游戏对象</param>
    public override void Condition(GameObject npc)
    {
        // 如果距离小于等于15，就看向玩家
        if (Vector3.Distance(npc.transform.position, m_playerTransform.position) <= m_2LookAtTime)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.SeePlayer);
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

