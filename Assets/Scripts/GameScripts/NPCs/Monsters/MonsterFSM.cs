using cfg.mon;
using UnityEngine;
using UnityEngine.AI;

public class MonsterFSM : MonoBehaviour
{
    private FiniteStateMachine m_fsm;
    // ��������
    private Monster m_NPCDatas;
    public Monster NPCDatas { get { return m_NPCDatas; } }
    // ����NavMeshAgent
    [SerializeField]
    private NavMeshAgent m_NavMeshAgent;
    public NavMeshAgent NavMeshAgent { get { return m_NavMeshAgent; } }
    // �����Sprite
    [SerializeField]
    private SpriteRenderer m_spriteRenderer;
    public SpriteRenderer SpriteRenderer {  get { return m_spriteRenderer; } }
    // ͷ����Ϣ
    [SerializeField]
    private SpriteRenderer m_infoRenderer;
    public SpriteRenderer InfoRenderer {  get { return m_infoRenderer; } }
    // ������λ��
    [SerializeField]
    private Transform m_projectileTransform;
    public Transform ProjectileTransform { get { return m_projectileTransform; } }
    // ��Դλ��
    [SerializeField]
    private Transform m_lightTransform;
    public Transform LightTransform { get { return m_lightTransform; } }
    private void Start()
    {
        foreach (var monster in GameTableDataAgent.MonsterTable.DataList)
        {
            // ����Ԥ�������ƻ�ȡ��ͬ�Ĺ�������
            if (gameObject.name == monster.PrefabName)
            {
                m_NPCDatas = monster;
                break;
            }
        }

        if (m_NPCDatas == null)
        {
            Debug.LogError(GetType() + " /Start() => ��������Ԥ���������Ƿ�ͱ��һ�£�");
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
        // ��⸽���Ƿ���ڹ�Դ
        Light light = FindAnyObjectByType<Light>();
        if (light != null)
        {
            m_lightTransform = light.transform;
            
        }
        
    }

    private void InitFSM()
    {
        // ��������״̬��
        m_fsm = new ();
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

        PatrolState patrolState = new (m_fsm, gameObject, playerObj.transform);
        // ת�����״̬
        patrolState.AddTransition(TransitionEnum.ToIdle, StateEnum.Idle);
        // ������ң�ת����������ҵ�״̬
        patrolState.AddTransition(TransitionEnum.SeePlayer, StateEnum.LookAt);
        // תΪ����״̬
        patrolState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
        m_fsm.AddState(patrolState);

        LookAtState lookAtState = new (m_fsm, gameObject, playerObj.transform);
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

        // ����Զ�̻��߽�ս���첻ͬ��״̬ת��ͼ

        // ��ս��Զ��
        if (m_NPCDatas.ShootRange != -1 && m_NPCDatas.HitRange != -1)
        {
            // ��ս�������
            chaseState.AddTransition(TransitionEnum.MeleeAttackPlayer, StateEnum.MeleeAttack);
            // Զ�̹������
            chaseState.AddTransition(TransitionEnum.RangedAttackPlayer, StateEnum.RangedAttack);

            MeleeAttackState meleeAttackState = new(m_fsm, gameObject, playerObj.transform);
            // Զ�̹������
            meleeAttackState.AddTransition(TransitionEnum.RangedAttackPlayer, StateEnum.RangedAttack);
            // ׷�����
            meleeAttackState.AddTransition(TransitionEnum.ChasePlayer, StateEnum.Chase);
            // ��ʧ���
            meleeAttackState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);
            // תΪ����״̬
            meleeAttackState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
            m_fsm.AddState(meleeAttackState);

            RangedAttackState rangedAttackState = new(m_fsm, gameObject, playerObj.transform);
            // ��ս���
            rangedAttackState.AddTransition(TransitionEnum.MeleeAttackPlayer, StateEnum.MeleeAttack);
            // ׷�����
            rangedAttackState.AddTransition(TransitionEnum.ChasePlayer, StateEnum.Chase);
            // תΪ����״̬
            rangedAttackState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
            m_fsm.AddState(rangedAttackState);
        }
        // ��Զ��
        if (m_NPCDatas.ShootRange != -1 && m_NPCDatas.HitRange == -1)
        {
            // Զ�̹������
            chaseState.AddTransition(TransitionEnum.RangedAttackPlayer, StateEnum.RangedAttack);
            RangedAttackState rangedAttackState = new(m_fsm, gameObject, playerObj.transform);
            // ׷�����
            rangedAttackState.AddTransition(TransitionEnum.ChasePlayer, StateEnum.Chase);
            // ��ʧ���
            rangedAttackState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);
            // תΪ����״̬
            rangedAttackState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
            m_fsm.AddState(rangedAttackState);
        }
        // ����ս
        if (m_NPCDatas.HitRange != -1 && m_NPCDatas.ShootRange == -1)
        {
            // ��ս�������
            chaseState.AddTransition(TransitionEnum.MeleeAttackPlayer, StateEnum.MeleeAttack);
            MeleeAttackState meleeAttackState = new(m_fsm, gameObject, playerObj.transform);
            // ׷�����
            meleeAttackState.AddTransition(TransitionEnum.ChasePlayer, StateEnum.Chase);
            // ��ʧ���
            meleeAttackState.AddTransition(TransitionEnum.LostPlayer, StateEnum.Patrol);
            // תΪ����״̬
            meleeAttackState.AddTransition(TransitionEnum.FleeAction, StateEnum.Flee);
            m_fsm.AddState(meleeAttackState);
        }
        m_fsm.AddState(chaseState);

        FleeState fleeState = new(m_fsm, gameObject);
        // תΪIdle
        fleeState.AddTransition(TransitionEnum.ToIdle, StateEnum.Idle);
        m_fsm.AddState(fleeState);
    }
}
