using UnityEngine;
/// <summary>
/// 看向玩家的状态
/// </summary>
public class LookAtState : BaseState
{
    // 玩家的位置
    private readonly Transform m_playerTransform;
    // 转换到追逐玩家的距离
    private readonly float m_2ChaseDistance = 10f;
    // 转换到丢失玩家的距离
    private readonly float m_2LostDistance = 15f;
    public LookAtState(FiniteStateMachine finiteStateMachine, Transform playerTransform = null) : base(finiteStateMachine)
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
    }
    /// <summary>
    /// 执行看向玩家的动作
    /// </summary>
    /// <param name="npc">游戏对象</param>
    public override void Act(GameObject npc)
    {
        // npc.transform.LookAt(m_playerTransform);
        float npcX = npc.transform.position.x;
        float playerX = m_playerTransform.position.x;
        // 玩家在NPC左边，看向左边
        SpriteRenderer spriteRenderer = npc.GetComponent<MonsterFSM>().m_spriteRenderer;
        if (playerX - npcX > 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }
    /// <summary>
    /// 判断是否切换到追逐玩家或者丢失玩家的状态
    /// </summary>
    /// <param name="npc">游戏对象</param>
    public override void Condition(GameObject npc)
    {
        // 小于10米就追逐玩家
        if (Vector3.Distance(npc.transform.position, m_playerTransform.position) <= m_2ChaseDistance)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.ChasePlayer);
        }
        // 大于15米就丢失玩家
        if (Vector3.Distance(npc.transform.position, m_playerTransform.position) > m_2LostDistance)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.LostPlayer);
        }
    }
}

