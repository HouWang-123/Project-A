using UnityEngine;
/// <summary>
/// 近战攻击状态
/// </summary>
public class MeleeAttackState : BaseState
{
    // 玩家的位置
    private readonly Transform m_playerTransform;
    // 伤害帧
    private readonly float m_2DamageFrame = 1f;
    // 近战攻击范围
    private readonly float m_MeleeDistance = -1f;
    // 动画播放的时候攻击了一次
    private bool m_attacked = false;
    // 攻击CD
    private readonly float m_attackOnceTimes = 1f;
    // 记录距离开始的时间
    private float m_timer = 0f;
    // 转为逃跑的光源距离
    private readonly float m_fleeDistance = -1f;
    public MeleeAttackState(FiniteStateMachine finiteStateMachine, GameObject npcObj, Transform playerTransform = null)
        : base(finiteStateMachine, npcObj)
    {
        // 设置状态
        m_stateEnum = StateEnum.MeleeAttack;
        if (playerTransform != null)
        {
            m_playerTransform = playerTransform;
        }
        else
        {
            m_playerTransform = GameObject.Find("Player000").transform;
        }
        m_2DamageFrame = 0.5f;
        m_MeleeDistance = npcObj.GetComponent<MonsterFSM>().NPCDatas.HitRange; // 应该获取表中的近战范围
        m_attackOnceTimes = 1.5f;
        m_fleeDistance = 3f;
    }

    public override void Act(GameObject npc)
    {
        // 模拟播放攻击动画
        m_timer += Time.deltaTime;
        if (m_timer >= m_2DamageFrame && !m_attacked)
        {
            if (Vector3.Distance(npc.transform.position, m_playerTransform.position) <= m_MeleeDistance)
            {
                Debug.Log(GetType() + " /Act() => 动画播放完成，并对玩家造成了伤害");
                m_attacked = true;
            }
        }
        if (m_timer >= m_attackOnceTimes + m_2DamageFrame)
        {
            // 重置CD
            m_timer = 0f;
            m_attacked = false;
        }
    }

    public override void Condition(GameObject npc)
    {
        // 大于m_MeleeDistance米
        if (Vector3.Distance(npc.transform.position, m_playerTransform.position) > m_MeleeDistance)
        {
            // 具有远程攻击，转为远程攻击状态
            if (m_gameObject.GetComponent<MonsterFSM>().NPCDatas.ShootRange != -1f)
            {
                m_finiteStateMachine.PerformTransition(TransitionEnum.RangedAttackPlayer);
            }
            else // 否则转为追逐
            {
                m_finiteStateMachine.PerformTransition(TransitionEnum.ChasePlayer);
            }
        }
        // 发现光源直接逃跑
        var lightTransform = m_gameObject.GetComponent<MonsterFSM>().LightTransform;
        if (lightTransform != null)
        {
            if (Vector3.Distance(lightTransform.position, m_gameObject.transform.position) <= m_fleeDistance)
            {
                m_finiteStateMachine.PerformTransition(TransitionEnum.FleeAction);
            }
        }
    }
}

