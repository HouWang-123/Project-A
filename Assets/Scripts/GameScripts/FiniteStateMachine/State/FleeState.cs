using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// 逃走状态
/// </summary>
public class FleeState : BaseState
{
    // 逃跑时间
    private readonly float m_fleeTime = 3f;
    // 单次逃跑距离
    private readonly float m_runDistance = 3f;
    private float m_timer = 0f;
    private bool m_isMoving = false;
    // NPC的NavMeshAgent组件
    private readonly NavMeshAgent agent;
    public FleeState(FiniteStateMachine finiteStateMachine, GameObject gameObject)
        : base(finiteStateMachine, gameObject)
    {
        // 设置状态
        m_stateEnum = StateEnum.Flee;
        m_fleeTime = 3f;
        m_isMoving = false;
        agent = m_monsterBaseFSM.NavMeshAgent;
    }

    public override void Act(GameObject npc)
    {
        if (!m_isMoving)
        {
            var light = m_monsterBaseFSM.LightComponent;
            if (light)
            {
                Vector3 temp = (npc.transform.position - light.transform.position);
                Vector3 nextPos = npc.transform.position + m_runDistance * temp.normalized;
                // 在该方向不可达，就往反方向走
                if (!IsPathValid(nextPos))
                {
                    Vector3 curDirection = temp.normalized;
                    float angle = 180f;
                    Vector3 newDirection = Quaternion.Euler(0, angle, 0) * curDirection;
                    nextPos = npc.transform.position + m_runDistance * newDirection;
                }
                // 调整朝向
                float direction = nextPos.x - m_monsterBaseFSM.transform.position.x;
                Vector3 scale;
                if (direction > 0f)
                {
                    scale = GameConstData.XNormalScale;
                }
                else
                {
                    scale = GameConstData.XReverseScale;
                }
                m_monsterBaseFSM.transform.localScale = scale;
                agent.SetDestination(nextPos);
                m_isMoving = true;
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

    public override void Condition(GameObject npc)
    {
        if (m_monsterBaseFSM.LightComponent)
        {
            m_timer += Time.deltaTime * m_timeScale;
            // 超过逃跑时间
            if (m_timer > m_fleeTime)
            {
                m_isMoving = false;
                m_finiteStateMachine.PerformTransition(TransitionEnum.ToIdle);
                m_timer = 0f;
            }
        }
        else
        {
            m_isMoving = false;
            m_finiteStateMachine.PerformTransition(TransitionEnum.ToIdle);
            m_timer = 0f;
        }
    }
    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
        agent.isStopped = false;
        agent.speed = m_monsterBaseFSM.MonsterDatas.Speed * m_timeScale;
        // 状态对应动画名称
        MonsterAnimationController.PlayAnim(m_gameObject, StateEnum.Flee, 0, true, m_timeScale);
    }

    public override void DoAfterLeaving()
    {
        base.DoAfterLeaving();
        // 停止移动
        agent.isStopped = true;
        agent.speed = m_monsterBaseFSM.MonsterDatas.Speed * m_timeScale;
        agent.destination = agent.transform.position;
    }
}

