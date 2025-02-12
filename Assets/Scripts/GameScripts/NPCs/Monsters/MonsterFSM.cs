using UnityEngine;
using UnityEngine.AI;

public class MonsterFSM : MonoBehaviour
{
    private FiniteStateMachine m_fsm;
    // 怪物数据
    public NPCDataBase m_NPCDatas;
    // 头顶信息
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
        // 构建有限状态机
        m_fsm = new ();
        var playerObj = GameObject.Find("Player000");
        Debug.Log("obj:" + playerObj.ToString());
        // 初始化各个状态
        PatrolState patrolState = new (m_fsm, gameObject, playerObj.transform);
        // 看见玩家，转换到看着玩家的状态
        patrolState.AddTransition(TransitionEnum.SeePlayer, StateEnum.LookAt);

        LookAtState lookAtState = new (m_fsm, playerObj.transform);
        // 追逐玩家，转换到追逐
        lookAtState.AddTransition(TransitionEnum.ChasePlayer, StateEnum.Chase);
        // 丢失玩家，转换到巡逻
        lookAtState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);

        ChaseState chaseState = new(m_fsm, gameObject, playerObj.transform);
        // 看见玩家，转换到看着
        chaseState.AddTransition(TransitionEnum.SeePlayer, StateEnum.LookAt);
        // 攻击玩家
        chaseState.AddTransition(TransitionEnum.AttackPlayer, StateEnum.Attack);
        // 丢失玩家
        chaseState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);

        AttackState attackState = new(m_fsm, playerObj.transform);
        // 追逐玩家
        attackState.AddTransition(TransitionEnum.ChasePlayer, StateEnum.Chase);
        // 丢失玩家
        attackState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);

        m_fsm.AddState(patrolState);
        m_fsm.AddState(lookAtState);
        m_fsm.AddState(chaseState);
        m_fsm.AddState(attackState);
    }
}
