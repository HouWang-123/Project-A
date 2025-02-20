using UnityEngine;

public class DrownedOnesFSM : MonsterBaseFSM
{
    // ������λ��
    [Header("������λ��")]
    [SerializeField]
    private Transform m_projectileTransform;
    // ��ս����λ��
    [Header("��ս����λ��")]
    [SerializeField]
    private Transform m_meleeTransform;
    protected override void Init()
    {
        base.Init();

        if (m_projectileTransform == null)
        {
            m_projectileTransform = transform.Find("Projectile");
        }
        if (m_meleeTransform == null)
        {
            m_meleeTransform = transform.Find("Melee");
        }    
        m_animationEnumWithName.Add(StateEnum.Patrol, new() { "Walk2" });
        m_animationEnumWithName.Add(StateEnum.LookAt, new() { "Idle" });
        m_animationEnumWithName.Add(StateEnum.Chase, new() { "Walk2" });
        m_animationEnumWithName.Add(StateEnum.MeleeAttack, new() { "Attack_1", "Attack_3" });
        m_animationEnumWithName.Add(StateEnum.RangedAttack, new() { "Attack_2" });
        m_animationEnumWithName.Add(StateEnum.Flee, new() { "Walk2" });
        InitFSM();

        // ע��������˵��¼�
        EventManager.Instance.RegistEvent(EventConstName.PLAYER_HURTED_BY_DROWNED_ONES_MELEE, HurtPlayer);
        EventManager.Instance.RegistEvent(EventConstName.PLAYER_HURTED_BY_DROWNED_ONES_RANGED, HurtPlayer);
    }

    private void OnDestroy()
    {

        // ȡ��������˵��¼�
        EventManager.Instance.RemoveEvent(EventConstName.PLAYER_HURTED_BY_DROWNED_ONES_MELEE, HurtPlayer);
        EventManager.Instance.RemoveEvent(EventConstName.PLAYER_HURTED_BY_DROWNED_ONES_RANGED, HurtPlayer);
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
        // ��ʼ������״̬
        IdleState idleState = new(m_fsm, gameObject, playerObj.transform);
        // ת��Ѳ��״̬
        idleState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);
        // ת�������
        idleState.AddTransition(TransitionEnum.SeePlayer, StateEnum.LookAt);
        // תΪ����״̬
        idleState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
        m_fsm.AddState(idleState);

        PatrolState patrolState = new(m_fsm, gameObject, playerObj.transform);
        // ת�����״̬
        patrolState.AddTransition(TransitionEnum.ToIdle, StateEnum.Idle);
        // ������ң�ת����������ҵ�״̬
        patrolState.AddTransition(TransitionEnum.SeePlayer, StateEnum.LookAt);
        // תΪ����״̬
        patrolState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
        m_fsm.AddState(patrolState);

        LookAtState lookAtState = new(m_fsm, gameObject, playerObj.transform);
        // ׷����ң�ת����׷��
        lookAtState.AddTransition(TransitionEnum.ChasePlayer, StateEnum.Chase);
        // ��ʧ��ң�ת����Ѳ��
        lookAtState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);
        // תΪ����״̬
        lookAtState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
        m_fsm.AddState(lookAtState);

        ChaseState chaseState = new(m_fsm, gameObject, playerObj.transform);
        // ������ң�ת��������
        chaseState.AddTransition(TransitionEnum.SeePlayer, StateEnum.LookAt);
        // ��ʧ���
        chaseState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);
        // תΪ����״̬
        chaseState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
        // ��ս�������
        chaseState.AddTransition(TransitionEnum.MeleeAttackPlayer, StateEnum.MeleeAttack);
        // Զ�̹������
        chaseState.AddTransition(TransitionEnum.RangedAttackPlayer, StateEnum.RangedAttack);
        m_fsm.AddState(chaseState);

        MeleeAttackState meleeAttackState = new(m_fsm, gameObject, m_meleeTransform, playerObj.transform);
        // Զ�̹������
        meleeAttackState.AddTransition(TransitionEnum.RangedAttackPlayer, StateEnum.RangedAttack);
        // ׷�����
        meleeAttackState.AddTransition(TransitionEnum.ChasePlayer, StateEnum.Chase);
        // ��ʧ���
        meleeAttackState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);
        // תΪ����״̬
        meleeAttackState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
        m_fsm.AddState(meleeAttackState);

        RangedAttackState rangedAttackState = new(m_fsm, gameObject, m_projectileTransform, playerObj.transform);
        // ��ս���
        rangedAttackState.AddTransition(TransitionEnum.MeleeAttackPlayer, StateEnum.MeleeAttack);
        // ׷�����
        rangedAttackState.AddTransition(TransitionEnum.ChasePlayer, StateEnum.Chase);
        // תΪ����״̬
        rangedAttackState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
        m_fsm.AddState(rangedAttackState);

        FleeState fleeState = new(m_fsm, gameObject);
        // תΪIdle
        fleeState.AddTransition(TransitionEnum.ToIdle, StateEnum.Idle);
        m_fsm.AddState(fleeState);
    }
    protected override void HurtPlayer()
    {
        base.HurtPlayer();

        var playerObj = GameObject.Find("Player000");
        var playerData = playerObj.GetComponent<PlayerControl>().PlayerData;
        playerData.CurrentHP -= m_monsterDatas.Attack;
        Debug.Log(GetType() + " /HurtPlayer() => ����ܵ� " + m_monsterDatas.Attack + " ���˺�");
    }
}
