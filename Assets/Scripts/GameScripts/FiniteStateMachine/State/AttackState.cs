using UnityEngine;

public class AttackState : BaseState
{
    // 玩家的位置
    private readonly Transform m_playerTransform;
    // 伤害帧
    private readonly float m_2DamageFrame = 1f;
    // 攻击范围
    private readonly float m_2ChaseDistance = 1.5f;
    // 动画播放的时候攻击了一次
    private bool m_attacked = false;
    // 动画总时间
    private readonly float m_animTimes = 2f;
    // 记录距离开始的时间
    private float m_timer = 0f;
    public AttackState(FiniteStateMachine finiteStateMachine, Transform playerTransform) : base(finiteStateMachine)
    {
        m_stateEnum = StateEnum.Attack;
        if (playerTransform != null)
        {
            m_playerTransform = playerTransform;
        }
        else
        {
            m_playerTransform = GameObject.Find("Player000").transform;
        }
    }

    public override void Act(GameObject npc)
    {
        // 模拟播放攻击动画
        m_timer += Time.deltaTime;
        if (m_timer >= m_2DamageFrame && !m_attacked)
        {
            if (Vector3.Distance(npc.transform.position, m_playerTransform.position) <= m_2ChaseDistance)
            {
                Debug.Log(GetType() + " /Act() => 动画播放到伤害帧，对玩家造成了伤害");
                m_attacked = true;
            }
        }
        if (m_timer >= m_animTimes)
        {
            m_timer = 0f;
            m_attacked = false;
        }
    }

    public override void Condition(GameObject npc)
    {
        // 大于1.5米就转为追逐玩家
        if (Vector3.Distance(npc.transform.position, m_playerTransform.position) > m_2ChaseDistance)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.ChasePlayer);
        }
    }
}

