using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
/// <summary>
/// 追逐状态
/// </summary>
public class ChaseState : BaseState
{
    // 计时器
    private float m_timer = 0f;
    // 判断NPC是否卡住的阈值时间
    public float stuckThreshold = 4f;
    // NPC的NavMeshAgent组件
    private readonly NavMeshAgent agent;
    // 记录NPC卡住的时间，用于触发强制重新寻路
    private float stuckTimer;
    // 玩家的位置
    private readonly Transform m_playerTransform;
    // 转换为看向玩家状态的最小距离
    private readonly float m_2LookAtDistance = -1f;
    // 远程攻击距离
    private readonly float m_2RangedAttack = -1f;
    // 转换为攻击玩家的距离
    private readonly float m_2MeleeAttack = -1f;
    // 转为逃跑的光源距离
    private readonly float m_fleeDistance = -1f;
    // 嘲讽动画的时间
    private float m_fuckingTime = 0f;
    // 嘲讽的CD时间
    private readonly float m_fuckingCD = 10f;
    // 嘲讽中
    private bool m_fucking = false;
    // 触发了嘲讽
    private bool m_fuckedAnimPlayed = false;
    public ChaseState(FiniteStateMachine finiteStateMachine, GameObject NPCObj, Transform playerTransform = null)
        : base(finiteStateMachine, NPCObj)
    {
        // 设置当前的状态
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
        // 点在右边，看向右边
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
        // 跟着玩家
        if (IsPathValid(m_playerTransform.position))
        {
            agent.SetDestination(m_playerTransform.position);
        }
        // 10%概率触发嘲讽
        if (!m_fucking && Random.value <= 0.1f)
        {
            m_fucking = true;
            m_fuckedAnimPlayed = true;
            agent.isStopped = true;
            Debug.Log(GetType() + " /Act() => 触发了嘲讽动画");
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
        // 十秒CD
        if (m_timer > m_fuckingTime + m_fuckingCD)
        {
            m_timer = 0f;
            m_fucking = false;
        }
        /*else
        {
            Debug.Log(GetType() + " /Act()=> 玩家位置无效，退化为直线行走");
            var direction = (m_playerTransform.position - npc.transform.position).normalized;
            float speed = npc.GetComponent<MonsterFSM>().NPCDatas.Speed;
            Vector3 newPos = npc.transform.position + speed * Time.deltaTime * direction;
            npc.transform.position = newPos;
        }*/

        // 卡顿检测
        if (agent.velocity.sqrMagnitude < 0.05f)// 取平方比取模快
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
                    Debug.Log(GetType() + " /Act()=> 玩家位置无效，退化为直线行走");
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
    /// 判断是否切换看向玩家的条件监听
    /// </summary>
    /// <param name="npc">游戏对象</param>
    public override void Condition(GameObject npc)
    {
        float distance = Vector3.Distance(npc.transform.position, m_playerTransform.position);
        // 大于m_2LookAtDistance米就转为看向玩家状态
        if (distance > m_2LookAtDistance)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.SeePlayer);
        }
        // 小于m_2Attack米就转为近战攻击玩家状态
        if (m_2MeleeAttack != -1 && distance <= m_2MeleeAttack)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.MeleeAttackPlayer);
        }
        // 小于m_2Attack米就转为远程攻击玩家状态
        if (m_2RangedAttack != -1 && distance <= m_2RangedAttack)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.RangedAttackPlayer);
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
        // 发现光源直接逃跑
        var lightComponent = npc.GetComponent<MonsterBaseFSM>().LightComponent;
        if (lightComponent != null)
        {
            // 光源打开着
            if (lightComponent.isOn)
            {
                Transform lightTransform = lightComponent.transform;
                if (Vector3.Distance(lightTransform.position, m_gameObject.transform.position) <= m_fleeDistance)
                {
                    m_finiteStateMachine.PerformTransition(TransitionEnum.FleeAction);
                }
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
                Debug.DrawLine(m_gameObject.transform.position, target, Color.red, 2f); // 绘制不可达路径
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
        // 停止移动
        agent.isStopped = true;
        agent.destination = agent.transform.position;
    }
}
