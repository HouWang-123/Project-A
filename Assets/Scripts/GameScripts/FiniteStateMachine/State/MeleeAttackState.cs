using Spine;
using UnityEngine;
/// <summary>
/// 近战攻击状态
/// </summary>
public class MeleeAttackState : BaseState
{
    // 玩家的位置
    private readonly Transform m_playerTransform;
    // 近战攻击范围
    private readonly float m_MeleeDistance = -1f;
    // 动画播放的时候攻击了一次
    private bool m_enterCD = false;
    // 攻击了没有
    private bool m_attacked = false;
    // 动画播放时间
    private float m_animTotalTime = 0f;
    // 攻击CD
    private readonly float m_attackCD = 1f;
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
        m_MeleeDistance = npcObj.GetComponent<MonsterFSM>().NPCDatas.HitRange; // 应该获取表中的近战范围
        m_fleeDistance = 3f;
    }

    public override void Act(GameObject npc)
    {
        m_timer += Time.deltaTime;
        if (!m_enterCD)
        {
            // 等概率出现两种近战攻击动画
            if (Random.value <= 0.5f)
            {
                m_gameObject.GetComponent<MonsterFSM>().PlayAnimation(0, "Attack_1", false);
                m_animTotalTime = m_gameObject.GetComponent<MonsterFSM>().AnimationTotalTime();
            }
            else
            {
                var monsterFSM = m_gameObject.GetComponent<MonsterFSM>();
                // 状态对应动画名称，根据怪物调整
                switch (monsterFSM.NPCDatas.PrefabName)
                {
                    case "DrownedOnes":
                        monsterFSM.PlayAnimation(0, "Attack_3", false);
                        m_animTotalTime = monsterFSM.AnimationTotalTime();
                        break;
                    case "HoundTindalos":
                        monsterFSM.PlayAnimation(0, "Attack_2", false);
                        m_animTotalTime = monsterFSM.AnimationTotalTime();
                        break;
                }
                m_animTotalTime = m_gameObject.GetComponent<MonsterFSM>().AnimationTotalTime();
            }
            m_enterCD = true;
        }
        
        if (m_timer >= m_animTotalTime && !m_attacked)
        {
            if (Vector3.Distance(npc.transform.position, m_playerTransform.position) <= m_MeleeDistance)
            {
                Debug.Log(GetType() + " /Act() => 动画播放完成，并对玩家造成了伤害");
            }
            m_attacked = true;
            m_gameObject.GetComponent<MonsterFSM>().PlayAnimation(0, "Idle", false);
        }
        if (m_timer >= m_attackCD + m_animTotalTime)
        {
            // 重置CD
            m_timer = 0f;
            m_enterCD = false;
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
    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
        var monsterFSM = m_gameObject.GetComponent<MonsterFSM>();
        // 等概率出现两种近战攻击动画
        if (Random.value <= 0.5f)
        {
            monsterFSM.PlayAnimation(0, "Attack_1", false);
            m_animTotalTime = monsterFSM.AnimationTotalTime();
        }
        else
        {
            // 状态对应动画名称，根据怪物调整
            switch (monsterFSM.NPCDatas.PrefabName)
            {
                case "DrownedOnes":
                    monsterFSM.PlayAnimation(0, "Attack_3", false);
                    m_animTotalTime = monsterFSM.AnimationTotalTime();
                    break;
                case "HoundTindalos":
                    monsterFSM.PlayAnimation(0, "Attack_2", false);
                    m_animTotalTime = monsterFSM.AnimationTotalTime();
                    break;
            }
        }
    }
}

