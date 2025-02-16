using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// 逃走状态
/// </summary>
public class FleeState : BaseState
{
    // 逃跑速度
    private readonly float m_fleeSpeed;
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
        var monsterFSM = gameObject.GetComponent<MonsterFSM>();
        m_fleeSpeed = monsterFSM.NPCDatas.Speed;
        agent = gameObject.GetComponent<MonsterFSM>().NavMeshAgent;
    }

    public override void Act(GameObject npc)
    {
        var lightTransfrom = npc.GetComponent<MonsterFSM>().LightTransform;
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
        if (npc.GetComponent<MonsterFSM>().LightTransform)
        {
            m_timer += Time.deltaTime;
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
    }

    public override void DoAfterLeaving()
    {
        base.DoAfterLeaving();
        // 停止移动
        agent.isStopped = true;
        agent.destination = agent.transform.position;
    }
}

