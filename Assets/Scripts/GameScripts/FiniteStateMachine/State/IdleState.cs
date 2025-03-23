using UnityEngine;
/// <summary>
/// 待机状态
/// </summary>
public class IdleState : BaseState
{
    // 等待计时器
    private float m_TimeToWait = 0f;
    // 玩家位置
    private readonly Transform m_playerTransform;
    // 转换为看向玩家状态的最小距离
    private readonly float m_warnDistance = -1f;
    // 转为逃跑的光源距离
    private readonly float m_fleeDistance = -1f;
    // 玩家灯光组件
    private readonly FlashLightBehaviour lightCom;

    public IdleState(FiniteStateMachine finiteStateMachine, GameObject gameObject, Transform playerTransform)
        : base(finiteStateMachine, gameObject)
    {
        // 状态设置
        m_stateEnum = StateEnum.Idle;
        m_playerTransform = playerTransform;
        m_warnDistance = m_monsterBaseFSM.MonsterDatas.WarnRange + 5f;
        m_fleeDistance = 3f;
        lightCom = m_monsterBaseFSM.LightComponent;
    }

    public override void Act(GameObject npc)
    {
        
    }

    public override void Condition(GameObject npc)
    {
        m_TimeToWait += Time.deltaTime * m_timeScale;
        // 等待5秒
        if (m_TimeToWait >= 5f)
        {
            // 计时器清零
            m_TimeToWait = 0f;
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
        MonsterAnimationController.PlayAnim(m_gameObject, StateEnum.Idle, 0, true, m_timeScale);
    }
}
