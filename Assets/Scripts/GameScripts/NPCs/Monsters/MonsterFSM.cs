using UnityEngine;
using UnityEngine.AI;

public class MonsterFSM : MonoBehaviour
{
    private FiniteStateMachine m_fsm;
    // ��������
    public NPCDataBase m_NPCDatas;
    // ͷ����Ϣ
    public SpriteRenderer m_infoRenderer;
    private void Start()
    {
        InitFSM();
        m_NPCDatas = new MonsterData
        {
            speed = GetComponent<NavMeshAgent>().speed
        };
        if (m_infoRenderer == null)
        {
            m_infoRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        }
    }

    private void Update()
    {
        m_fsm.DoUpdate(gameObject);
    }

    private void InitFSM()
    {
        // ��������״̬��
        m_fsm = new ();
        var playerObj = GameObject.Find("Player000");
        Debug.Log("obj:" + playerObj.ToString());
        // ��ʼ������״̬
        PatrolState patrolState = new (m_fsm, gameObject, playerObj.transform);
        // ������ң�ת����������ҵ�״̬
        patrolState.AddTransition(TransitionEnum.SeePlayer, StateEnum.LookAt);

        LookAtState lookAtState = new (m_fsm, playerObj.transform);
        // ׷����ң�ת����׷��
        lookAtState.AddTransition(TransitionEnum.ChasePlayer, StateEnum.Chase);
        // ��ʧ��ң�ת����Ѳ��
        lookAtState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);

        ChaseState chaseState = new(m_fsm, gameObject, playerObj.transform);
        // ������ң�ת��������
        chaseState.AddTransition(TransitionEnum.SeePlayer, StateEnum.LookAt);
        // �������
        chaseState.AddTransition(TransitionEnum.AttackPlayer, StateEnum.Attack);
        // ��ʧ���
        chaseState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);

        AttackState attackState = new(m_fsm, playerObj.transform);
        // ׷�����
        attackState.AddTransition(TransitionEnum.ChasePlayer, StateEnum.Chase);
        // ��ʧ���
        attackState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);

        m_fsm.AddState(patrolState);
        m_fsm.AddState(lookAtState);
        m_fsm.AddState(chaseState);
        m_fsm.AddState(attackState);
    }
}
