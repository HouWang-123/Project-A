using UnityEngine;

public class HoundTindalosFSM : MonsterBaseFSM
{
    // 近战攻击位置
    [Header("近战攻击位置")]
    [SerializeField]
    private Transform m_meleeTransform;
    // 事件编码
    private string thisClassHashCode;
    public string MeleeAttackHurtEventName { get { return thisClassHashCode; } }
    protected override void Init()
    {
        base.Init();
        thisClassHashCode = GetHashCode().ToString();
        if (m_meleeTransform == null)
        {
            m_meleeTransform = transform.Find("Melee");
        }
        m_animationEnumWithName.Add(StateEnum.Patrol, new() { "Walk" });
        m_animationEnumWithName.Add(StateEnum.LookAt, new() { "Idle" });
        m_animationEnumWithName.Add(StateEnum.Chase, new() { "Walk" });
        m_animationEnumWithName.Add(StateEnum.MeleeAttack, new() { "Attack_1", "Attack_2" });
        m_animationEnumWithName.Add(StateEnum.Flee, new() { "Walk" });
        InitFSM();
        // 注册玩家受伤的事件
        EventManager.Instance.RegistEvent(thisClassHashCode, HurtPlayer);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveEvent(thisClassHashCode, HurtPlayer);
    }

    private void Start()
    {
        Init();
    }

    protected override void DoUpdate()
    {
        base.DoUpdate();
    }

    protected void Update()
    {
        DoUpdate();
    }

    protected override void InitFSM()
    {
        base.InitFSM();
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
        // 转为死亡状态
        idleState.AddTransition(TransitionEnum.ToDeath, StateEnum.Death);
        m_fsm.AddState(idleState);

        PatrolState patrolState = new(m_fsm, gameObject, playerObj.transform);
        // 转向待机状态
        patrolState.AddTransition(TransitionEnum.ToIdle, StateEnum.Idle);
        // 看见玩家，转换到看着玩家的状态
        patrolState.AddTransition(TransitionEnum.SeePlayer, StateEnum.LookAt);
        // 转为逃跑状态
        patrolState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
        // 转为死亡状态
        patrolState.AddTransition(TransitionEnum.ToDeath, StateEnum.Death);
        m_fsm.AddState(patrolState);

        LookAtState lookAtState = new(m_fsm, gameObject, playerObj.transform);
        // 追逐玩家，转换到追逐
        lookAtState.AddTransition(TransitionEnum.ChasePlayer, StateEnum.Chase);
        // 丢失玩家，转换到巡逻
        lookAtState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);
        // 转为逃跑状态
        lookAtState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
        // 转为死亡状态
        lookAtState.AddTransition(TransitionEnum.ToDeath, StateEnum.Death);
        m_fsm.AddState(lookAtState);

        ChaseState chaseState = new(m_fsm, gameObject, playerObj.transform);
        // 看见玩家，转换到看着
        chaseState.AddTransition(TransitionEnum.SeePlayer, StateEnum.LookAt);
        // 丢失玩家
        chaseState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);
        // 转为逃跑状态
        chaseState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
        // 仅近战
        chaseState.AddTransition(TransitionEnum.MeleeAttackPlayer, StateEnum.MeleeAttack);
        // 转为死亡状态
        chaseState.AddTransition(TransitionEnum.ToDeath, StateEnum.Death);
        m_fsm.AddState(chaseState);

        // 仅远程
        /*if (m_monsterDatas.ShootRange != -1 && m_monsterDatas.HitRange == -1)
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
        }*/
        MeleeAttackState meleeAttackState = new(m_fsm, gameObject, m_meleeTransform, playerObj.transform);
        // 追逐玩家
        meleeAttackState.AddTransition(TransitionEnum.ChasePlayer, StateEnum.Chase);
        // 丢失玩家
        meleeAttackState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);
        // 转为逃跑状态
        meleeAttackState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
        // 转为死亡状态
        meleeAttackState.AddTransition(TransitionEnum.ToDeath, StateEnum.Death);
        m_fsm.AddState(meleeAttackState);

        FleeState fleeState = new(m_fsm, gameObject);
        // 转为Idle
        fleeState.AddTransition(TransitionEnum.ToIdle, StateEnum.Idle);
        // 转为死亡状态
        fleeState.AddTransition(TransitionEnum.ToDeath, StateEnum.Death);
        m_fsm.AddState(fleeState);

        // 死亡状态
        DeathState deathState = new(m_fsm, gameObject);
        m_fsm.AddState(deathState);
    }
    protected override void HurtPlayer()
    {
        base.HurtPlayer();
        GameRunTimeData.Instance.CharacterBasicStat.HurtPlayer(m_monsterDatas.Attack);
        return;
    }
}

