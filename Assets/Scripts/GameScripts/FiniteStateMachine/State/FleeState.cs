using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// 逃走状态
/// </summary>
public class FleeState : BaseState
{
    // 逃跑时间
    private readonly float m_fleeTime = 3f;
    private float m_timer = 0f;
    // NPC的NavMeshAgent组件
    private readonly NavMeshAgent agent;
    public FleeState(FiniteStateMachine finiteStateMachine, GameObject gameObject)
        : base(finiteStateMachine, gameObject)
    {
        // 设置状态
        m_stateEnum = StateEnum.Flee;
        m_fleeTime = 3f;
        agent = gameObject.GetComponent<MonsterBaseFSM>().NavMeshAgent;
    }

    public override void Act(GameObject npc)
    {
        var lightTransfrom = npc.GetComponent<MonsterBaseFSM>().LightTransform;
        if (lightTransfrom)
        {
            Vector3 direction = 2f * (npc.transform.position - lightTransfrom.position);
            // 在该方向
            if (IsPathValid(npc.transform.position + direction))
            {
                agent.SetDestination(npc.transform.position + direction);
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
        if (npc.GetComponent<MonsterBaseFSM>().LightTransform)
        {
            m_timer += Time.deltaTime * m_timeScale;
            // 超过逃跑时间
            if (m_timer > m_fleeTime)
            {
                m_finiteStateMachine.PerformTransition(TransitionEnum.ToIdle);
                m_timer = 0f;
            }
        }
    }
    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
        agent.isStopped = false;
        var monsterFSM = m_gameObject.GetComponent<MonsterBaseFSM>();
        agent.speed = monsterFSM.MonsterDatas.Speed * m_timeScale;
        // 状态对应动画名称
        AnimationController.PlayAnim(m_gameObject, StateEnum.Flee, 0, false, m_timeScale);
    }

    public override void DoAfterLeaving()
    {
        base.DoAfterLeaving();
        // 停止移动
        agent.isStopped = true;
        var monsterFSM = m_gameObject.GetComponent<MonsterBaseFSM>();
        agent.speed = monsterFSM.MonsterDatas.Speed * m_timeScale;
        agent.destination = agent.transform.position;
    }
}

