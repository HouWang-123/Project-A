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
    // 判断NPC是否卡住的阈值时间
    public float stuckThreshold = 4f;
    // 是否可以重新移动
    private bool m_canMove = true;
    // 等待时间
    private float m_waitTime = 0f;
    // NPC的NavMeshAgent组件
    private readonly NavMeshAgent agent;
    // 记录距离上一次目标点生成的时间
    // private float timer = 0f;
    // 记录NPC卡住的时间，用于触发强制重新寻路
    private float stuckTimer;
    // NPC上一帧的位置，用于计算是否卡住
    // private Vector3 lastPosition;
    // 转换为看向玩家状态的最小距离
    private readonly float m_warnDistance = -1f;
    // 玩家的位置
    private readonly Transform m_playerTransform;
    // 转为逃跑的光源距离
    private readonly float m_fleeDistance = -1f;
    // 玩家灯光组件
    private readonly FlashLightBehaviour lightCom;
    public PatrolState(FiniteStateMachine finiteStateMachine, GameObject NPCObj, Transform playerTransform = null)
        : base(finiteStateMachine, NPCObj)
    {
        // 设置状态
        m_stateEnum = StateEnum.Patrol;
        if (NPCObj == null)
        {
            Debug.LogError(GetType() + "/PatrolState/ gameObject 不能为空!");
            throw new ArgumentException();
        }
        agent = m_monsterBaseFSM.NavMeshAgent;
        m_warnDistance = m_monsterBaseFSM.MonsterDatas.WarnRange + 5f;
        // lastPosition = gameObject.transform.position;
        // 初始位置强制校正
        if (NavMesh.SamplePosition(NPCObj.transform.position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
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
        m_fleeDistance = 3f;
        lightCom = m_monsterBaseFSM.LightComponent;
    }

    /// <summary>
    /// 执行游荡动作
    /// </summary>
    /// <param name="npc">游戏对象</param>
    public override void Act(GameObject npc)
    {
        // 开始移动
        if (m_canMove)
        {
            TryFindNewDestination();
        }

        // 卡顿检测
        if (!m_canMove && agent.velocity.sqrMagnitude < 0.05f)// 取平方比取模快
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckThreshold)
            {
                TryFindNewDestination();
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }
    }
    /// <summary>
    /// 尝试生成新的目标点
    /// </summary>
    private void TryFindNewDestination()
    {
        Vector3 newPos = RandomNavSphere(m_gameObject.transform.position, wanderRadius, -1);
        if (IsPathValid(newPos))
        {
            agent.SetDestination(newPos);
            float newPosX = newPos.x;
            float npcPosX = m_gameObject.transform.position.x;
            // 点在右边，看向右边
            Vector3 scale;
            if (newPosX - npcPosX > 0f)
            {
                scale = GameConstData.XNormalScale;
            }
            else
            {
                scale = GameConstData.XReverseScale;
            }
            
            m_gameObject.transform.localScale = scale;
            m_canMove = false;
        }
        else
        {
            Debug.Log($"目标点 {newPos} 不可达，已忽略。");
            m_canMove = true;
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
                Debug.DrawLine(m_gameObject.transform.position, target, Color.red, 2f); // 绘制不可达路径
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
            return m_gameObject.transform.position;
        }
    }

    /// <summary>
    /// 判断状态是否转换为看向玩家
    /// </summary>
    /// <param name="npc">游戏对象</param>
    public override void Condition(GameObject npc)
    {        
        // 如果NPC到达目的地
        if (agent.transform.position == agent.destination)
        {
            m_canMove = false;
            AnimationController.PlayAnim(m_gameObject, StateEnum.Idle, 0, true, m_timeScale);
            m_waitTime += Time.deltaTime * m_timeScale;
            // 等待了两秒
            if (m_waitTime >= 2f)
            {
                Debug.Log(GetType() + " /Act() => 等待了两秒，判断是否需要跳转状态到idle");
                m_canMove = true;
                // 0.3的概率转换为待机
                if (Random.value <= 0.3f)
                {
                    m_finiteStateMachine.PerformTransition(TransitionEnum.ToIdle);
                }
                else
                {
                    AnimationController.PlayAnim(m_gameObject, StateEnum.Patrol, 0, true, m_timeScale);
                }
            }
        }
        // 如果距离小于等于m_2LookAtDistance，就看向玩家
        if (Vector3.Distance(npc.transform.position, m_playerTransform.position) <= m_warnDistance)
        {
            // agent.SetDestination(agent.transform.position);
            m_finiteStateMachine.PerformTransition(TransitionEnum.SeePlayer);
        }
        // 发现光源直接逃跑
        /*if (lightCom != null && lightCom.isOn)
        {
            Transform lightTransform = lightCom.transform;
            if (Vector3.Distance(lightTransform.position, m_gameObject.transform.position) <= m_fleeDistance)
            {
                m_finiteStateMachine.PerformTransition(TransitionEnum.FleeAction);
            }
        }*/
        if (m_monsterBaseFSM.IsLightOnMonster)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.FleeAction);
        }
    }

    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
        agent.isStopped = false;

        // 巡逻速度是0.5倍的移动速度
        agent.speed = 0.5f * m_monsterBaseFSM.MonsterDatas.Speed * m_timeScale;
        // 动画播放速度为0.5倍
        AnimationController.PlayAnim(m_gameObject, StateEnum.Patrol, 0, true, 0.5f * m_timeScale);
    }

    public override void DoAfterLeaving()
    {
        base.DoAfterLeaving();
        // 停止移动
        agent.isStopped = true;
        agent.SetDestination(agent.transform.position);

        // 恢复正常移动速度
        agent.speed = m_monsterBaseFSM.MonsterDatas.Speed * m_timeScale;
    }
}

