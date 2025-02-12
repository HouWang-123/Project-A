using UnityEngine;
/// <summary>
/// ׷��״̬
/// </summary>
public class ChaseState : BaseState
{
    // ��ҵ�λ��
    private readonly Transform m_playerTransform;
    // ת��Ϊ�������״̬����С����
    private readonly float m_2LookAtTime = 10f;
    // ת��Ϊ������ҵľ���
    private readonly float m_2Attack = 1.5f;
    public ChaseState(FiniteStateMachine finiteStateMachine, Transform playerTransform = null) : base(finiteStateMachine)
    {
        // ���õ�ǰ��״̬
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
    /// ִ��׷����
    /// </summary>
    /// <param name="npc">��Ϸ����</param>
    public override void Act(GameObject npc)
    {
        // npc.transform.LookAt(m_playerTransform);
        float npcX = npc.transform.position.x;
        float playerX = m_playerTransform.position.x;
        // �����NPC��ߣ��������
        SpriteRenderer spriteRenderer = npc.transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (playerX - npcX > 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
        // ������Һ�npc֮��ķ�������
        var direction = (m_playerTransform.position - npc.transform.position).normalized;
        float velocity = npc.GetComponent<MonsterFSM>().NPCDatas.velocity;
        Vector3 newPos = npc.transform.position + velocity * Time.deltaTime * direction;
        npc.transform.position = newPos;
        // npc.transform.Translate(2 * Time.deltaTime * direction);
    }

    /// <summary>
    /// �ж��Ƿ��л�������ҵ���������
    /// </summary>
    /// <param name="npc">��Ϸ����</param>
    public override void Condition(GameObject npc)
    {
        // ����10�׾�תΪ�������״̬
        if (Vector3.Distance(npc.transform.position, m_playerTransform.position) > m_2LookAtTime)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.SeePlayer);
        }
        // С��1.5�׾�תΪ�������״̬
        if (Vector3.Distance(npc.transform.position, m_playerTransform.position) <= m_2Attack)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.AttackPlayer);
        }
        
    }
}
