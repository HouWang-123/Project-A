using cfg.mon;
using UnityEngine;
using UnityEngine.AI;

public class MonsterFSM : MonoBehaviour
{
    private FiniteStateMachine m_fsm;
    // 怪物数据
    private Monster m_NPCDatas;
    public Monster NPCDatas { get { return m_NPCDatas; } }
    // 怪物NavMeshAgent
    [SerializeField]
    private NavMeshAgent m_NavMeshAgent;
    public NavMeshAgent NavMeshAgent { get { return m_NavMeshAgent; } }
    // 怪物的Sprite
    [SerializeField]
    private SpriteRenderer m_spriteRenderer;
    public SpriteRenderer SpriteRenderer {  get { return m_spriteRenderer; } }
    // 头顶信息
    [SerializeField]
    private SpriteRenderer m_infoRenderer;
    public SpriteRenderer InfoRenderer {  get { return m_infoRenderer; } }
    // 发射物位置
    [SerializeField]
    private Transform m_projectileTransform;
    public Transform ProjectileTransform { get { return m_projectileTransform; } }
    // 光源位置
    [SerializeField]
    private Transform m_lightTransform;
    public Transform LightTransform { get { return m_lightTransform; } }
    private void Start()
    {
        foreach (var monster in GameTableDataAgent.MonsterTable.DataList)
        {
            // 根据预制体名称获取不同的怪物数据
            if (gameObject.name == monster.PrefabName)
            {
                m_NPCDatas = monster;
                break;
            }
        }

        if (m_NPCDatas == null)
        {
            Debug.LogError(GetType() + " /Start() => 请检查怪物的预制体名称是否和表格一致！");
        }

        if (m_infoRenderer == null)
        {
            m_infoRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        }

        if (m_spriteRenderer == null)
        {
            m_spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        }

        if (m_NavMeshAgent == null)
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
        }
        m_NavMeshAgent.speed = m_NPCDatas.Speed;

        if (m_projectileTransform == null)
        {
            m_projectileTransform = transform.Find("Projectile");
        }

        if (m_lightTransform == null)
        {
            Light light = FindAnyObjectByType<Light>();
            if (light != null)
            {
                m_lightTransform = light.transform;
            }
        }

        InitFSM();
    }

    private void Update()
    {
        m_fsm.DoUpdate(gameObject);
        // 检测附近是否存在光源
        Light light = FindAnyObjectByType<Light>();
        if (light != null)
        {
            m_lightTransform = light.transform;
            
        }
        
    }

    private void InitFSM()
    {
        // 构建有限状态机
        m_fsm = new ();
        var playerObj = GameObject.Find("Player000");
        Debug.Log("obj:" + playerObj.ToString());
        // 初始化各个状态
        IdleState idleState = new(m_fsm, gameObject, playerObj.transform);
        // 转向巡逻状态
        idleState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);
        // 转向看向玩家
        idleState.AddTransition(TransitionEnum.SeePlayer, StateEnum.LookAt);
        // 转为逃跑状态
        idleState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
        m_fsm.AddState(idleState);

        PatrolState patrolState = new (m_fsm, gameObject, playerObj.transform);
        // 转向待机状态
        patrolState.AddTransition(TransitionEnum.ToIdle, StateEnum.Idle);
        // 看见玩家，转换到看着玩家的状态
        patrolState.AddTransition(TransitionEnum.SeePlayer, StateEnum.LookAt);
        // 转为逃跑状态
        patrolState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
        m_fsm.AddState(patrolState);

        LookAtState lookAtState = new (m_fsm, gameObject, playerObj.transform);
        // 追逐玩家，转换到追逐
        lookAtState.AddTransition(TransitionEnum.ChasePlayer, StateEnum.Chase);
        // 丢失玩家，转换到巡逻
        lookAtState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);
        // 转为逃跑状态
        lookAtState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
        m_fsm.AddState(lookAtState);

        ChaseState chaseState = new(m_fsm, gameObject, playerObj.transform);
        // 看见玩家，转换到看着
        chaseState.AddTransition(TransitionEnum.SeePlayer, StateEnum.LookAt);
        // 丢失玩家
        chaseState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);
        // 转为逃跑状态
        chaseState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);

        // 根据远程或者近战构造不同的状态转换图

        // 近战和远程
        if (m_NPCDatas.ShootRange != -1 && m_NPCDatas.HitRange != -1)
        {
            // 近战攻击玩家
            chaseState.AddTransition(TransitionEnum.MeleeAttackPlayer, StateEnum.MeleeAttack);
            // 远程攻击玩家
            chaseState.AddTransition(TransitionEnum.RangedAttackPlayer, StateEnum.RangedAttack);

            MeleeAttackState meleeAttackState = new(m_fsm, gameObject, playerObj.transform);
            // 远程攻击玩家
            meleeAttackState.AddTransition(TransitionEnum.RangedAttackPlayer, StateEnum.RangedAttack);
            // 追逐玩家
            meleeAttackState.AddTransition(TransitionEnum.ChasePlayer, StateEnum.Chase);
            // 丢失玩家
            meleeAttackState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);
            // 转为逃跑状态
            meleeAttackState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
            m_fsm.AddState(meleeAttackState);

            RangedAttackState rangedAttackState = new(m_fsm, gameObject, playerObj.transform);
            // 近战玩家
            rangedAttackState.AddTransition(TransitionEnum.MeleeAttackPlayer, StateEnum.MeleeAttack);
            // 追逐玩家
            rangedAttackState.AddTransition(TransitionEnum.ChasePlayer, StateEnum.Chase);
            // 转为逃跑状态
            rangedAttackState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
            m_fsm.AddState(rangedAttackState);
        }
        // 仅远程
        if (m_NPCDatas.ShootRange != -1 && m_NPCDatas.HitRange == -1)
        {
            // 远程攻击玩家
            chaseState.AddTransition(TransitionEnum.RangedAttackPlayer, StateEnum.RangedAttack);
            RangedAttackState rangedAttackState = new(m_fsm, gameObject, playerObj.transform);
            // 追逐玩家
            rangedAttackState.AddTransition(TransitionEnum.ChasePlayer, StateEnum.Chase);
            // 丢失玩家
            rangedAttackState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);
            // 转为逃跑状态
            rangedAttackState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
            m_fsm.AddState(rangedAttackState);
        }
        // 仅近战
        if (m_NPCDatas.HitRange != -1 && m_NPCDatas.ShootRange == -1)
        {
            // 近战攻击玩家
            chaseState.AddTransition(TransitionEnum.MeleeAttackPlayer, StateEnum.MeleeAttack);
            MeleeAttackState meleeAttackState = new(m_fsm, gameObject, playerObj.transform);
            // 追逐玩家
            meleeAttackState.AddTransition(TransitionEnum.ChasePlayer, StateEnum.Chase);
            // 丢失玩家
            meleeAttackState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);
            // 转为逃跑状态
            meleeAttackState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
            m_fsm.AddState(meleeAttackState);
        }
        m_fsm.AddState(chaseState);

        FleeState fleeState = new(m_fsm, gameObject);
        // 转为Idle
        fleeState.AddTransition(TransitionEnum.ToIdle, StateEnum.Idle);
        m_fsm.AddState(fleeState);
    }
}
