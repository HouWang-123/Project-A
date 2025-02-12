using UnityEngine;
/// <summary>
/// 追逐状态
/// </summary>
public class ChaseState : BaseState
{
    // 玩家的位置
    private readonly Transform m_playerTransform;
    // 转换为看向玩家状态的最小距离
    private readonly float m_2LookAtTime = 10f;
    // 转换为攻击玩家的距离
    private readonly float m_2Attack = 1.5f;
    public ChaseState(FiniteStateMachine finiteStateMachine, Transform playerTransform = null) : base(finiteStateMachine)
    {
        // 设置当前的状态
        m_stateEnum = StateEnum.Chase;
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
    /// 执行追逐动作
    /// </summary>
    /// <param name="npc">游戏对象</param>
    public override void Act(GameObject npc)
    {
        // npc.transform.LookAt(m_playerTransform);
        float npcX = npc.transform.position.x;
        float playerX = m_playerTransform.position.x;
        // 玩家在NPC左边，看向左边
        SpriteRenderer spriteRenderer = npc.transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (playerX - npcX > 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
        // 计算玩家和npc之间的方向向量
        var direction = (m_playerTransform.position - npc.transform.position).normalized;
        float velocity = npc.GetComponent<MonsterFSM>().NPCDatas.velocity;
        Vector3 newPos = npc.transform.position + velocity * Time.deltaTime * direction;
        npc.transform.position = newPos;
        // npc.transform.Translate(2 * Time.deltaTime * direction);
    }

    /// <summary>
    /// 判断是否切换看向玩家的条件监听
    /// </summary>
    /// <param name="npc">游戏对象</param>
    public override void Condition(GameObject npc)
    {
        // 大于10米就转为看向玩家状态
        if (Vector3.Distance(npc.transform.position, m_playerTransform.position) > m_2LookAtTime)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.SeePlayer);
        }
        // 小于1.5米就转为攻击玩家状态
        if (Vector3.Distance(npc.transform.position, m_playerTransform.position) <= m_2Attack)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.AttackPlayer);
        }
        
    }
}
