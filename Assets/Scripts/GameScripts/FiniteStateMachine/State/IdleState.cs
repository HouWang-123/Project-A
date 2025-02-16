using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 待机状态
/// </summary>
public class IdleState : BaseState
{
    // 动画计时器
    private float m_Timer = 0f;
    // 等待计时器
    private float m_TimeToWait = 0f;
    // 模拟动画序列
    public List<string> m_animations;
    // 动画播放中
    private bool m_animPlaying = false;
    // 动画播放完成
    private bool m_animationDone = false;
    // 玩家位置
    private Transform m_playerTransform;
    // 转换为看向玩家状态的最小距离
    private readonly float m_warnDistance = -1f;
    // 转为逃跑的光源距离
    private readonly float m_fleeDistance = -1f;

    public IdleState(FiniteStateMachine finiteStateMachine, GameObject gameObject, Transform playerTransform)
        : base(finiteStateMachine, gameObject)
    {
        // 状态设置
        m_stateEnum = StateEnum.Idle;
        // 模拟添加动画
        m_animations = new()
        {
            "抬头战吼",
            "低头撒娇"
        };
        m_playerTransform = playerTransform;
        m_warnDistance = gameObject.GetComponent<MonsterFSM>().NPCDatas.WarnRange + 5f;
        m_fleeDistance = 3f;
    }

    public override void Act(GameObject npc)
    {
        m_Timer += Time.deltaTime;
        // 模拟播放动画
        if (!m_animPlaying)
        {
            m_animPlaying = true;
            int randInt = Random.Range(0, m_animations.Count);
            Debug.Log(GetType() + " /Act() => 播放动画: " + m_animations[randInt]);
        }
        // 假设每个动画播放完需要3秒
        if (m_Timer > 3f && !m_animationDone)
        {
            m_animationDone = true;
            Debug.Log(GetType() + " /Act() => 动画播放完成");
            m_Timer = 0f;
        }
    }

    public override void Condition(GameObject npc)
    {
        m_TimeToWait += Time.deltaTime;
        // 动画播放完成，等待5秒
        if (m_animationDone && m_TimeToWait >= 5f)
        {
            // 计时器清零
            m_TimeToWait = 0f;
            m_animPlaying = false;
            m_animationDone = false;
            Debug.Log(GetType() + " /Condition() => 按概率转换状态为待机或者巡逻");
            // 按概率切换状态，0.7的概率切换到巡逻
            if (Random.value > 0.3f)
            {
                // 切换到巡逻状态
                m_finiteStateMachine.PerformTransition(TransitionEnum.LostPlayer);
            }
        }
        // 转为看向玩家
        if (Vector3.Distance(m_playerTransform.position, m_gameObject.transform.position) <= m_warnDistance)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.SeePlayer);
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
