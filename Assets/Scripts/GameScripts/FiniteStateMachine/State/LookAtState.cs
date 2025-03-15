using UnityEngine;
/// <summary>
/// 看向玩家的状态
/// </summary>
public class LookAtState : BaseState
{
    // 玩家的位置
    private readonly Transform m_playerTransform;
    // 转换到追逐玩家的距离
    private readonly float m_2ChaseDistance = -1f;
    // 转换到丢失玩家的距离
    private readonly float m_2LostDistance = -1f;
    // 转为逃跑的光源距离
    private readonly float m_fleeDistance = -1f;
    // 玩家灯光组件
    private readonly LightBehaviour lightCom;
    public LookAtState(FiniteStateMachine finiteStateMachine, GameObject npcObj, Transform playerTransform = null)
        : base(finiteStateMachine, npcObj)
    {
        // 设置当前的状态
        m_stateEnum = StateEnum.LookAt;
        if (playerTransform != null)
        {
            m_playerTransform = playerTransform;
        }
        else
        {
            m_playerTransform = GameObject.Find("Player000").transform;
        }
        m_2ChaseDistance = m_monsterBaseFSM.MonsterDatas.WarnRange;
        m_2LostDistance = m_monsterBaseFSM.MonsterDatas.WarnRange + 5f;
        m_fleeDistance = 3f;
        lightCom = m_monsterBaseFSM.LightComponent;
    }
    /// <summary>
    /// 执行看向玩家的动作
    /// </summary>
    /// <param name="npc">游戏对象</param>
    public override void Act(GameObject npc)
    {
        // npc.transform.LookAt(m_playerTransform);
        float direction = m_playerTransform.position.x - m_monsterBaseFSM.transform.position.x;
        // 玩家在NPC左边，看向左边
        Vector3 scale;
        if (direction > 0f)
        {
            scale = GameConstData.XNormalScale;
        }
        else
        {
            scale = GameConstData.XReverseScale;
        }
        m_monsterBaseFSM.Renderer.transform.localScale = scale;
    }
    /// <summary>
    /// 判断是否切换到追逐玩家或者丢失玩家的状态
    /// </summary>
    /// <param name="npc">游戏对象</param>
    public override void Condition(GameObject npc)
    {
        float distance = Vector3.Distance(npc.transform.position, m_playerTransform.position);
        // 小于m_2ChaseDistance米就追逐玩家
        if (distance <= m_2ChaseDistance)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.ChasePlayer);
        }
        // 大于m_2LostDistance米就丢失玩家
        if (distance > m_2LostDistance)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.LostPlayer);
        }
        // 发现光源直接逃跑
        if (lightCom != null && lightCom.isOn)
        {
            Transform lightTransform = lightCom.transform;
            if (Vector3.Distance(lightTransform.position, m_gameObject.transform.position) <= m_fleeDistance)
            {
                m_finiteStateMachine.PerformTransition(TransitionEnum.FleeAction);
            }
        }
    }
    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
        AnimationController.PlayAnim(m_gameObject, StateEnum.LookAt, 0, true, m_timeScale);
    }
}

