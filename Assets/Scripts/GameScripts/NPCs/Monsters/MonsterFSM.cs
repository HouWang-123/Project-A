using cfg.mon;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;
using Animation = Spine.Animation;

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
    // ����Ķ���������
    [SerializeField]
    private SkeletonAnimation m_animRenderer;
    public SkeletonAnimation SpriteRenderer {  get { return m_animRenderer; } }
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

        if (m_animRenderer == null)
        {
            m_animRenderer = transform.GetChild(0).GetComponent<SkeletonAnimation>();
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
        m_animRenderer.Initialize(true);
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
    /// <summary>
    /// ���Ŷ���
    /// </summary>
    /// <param name="trackIndex">���</param>
    /// <param name="animName">��������</param>
    /// <param name="loop">�����Ƿ�ѭ��</param>
    public void PlayAnimation(int trackIndex, string animName, bool loop)
    {
        if (!m_animRenderer.skeleton.Data.Animations.Exists(a => a.Name == animName))
        {
            Debug.LogError($"{GetType()} /PlayAnimation() => �Ҳ�������: {animName}");
            return;
        }
        var trackEntry = m_animRenderer.state.SetAnimation(trackIndex, animName, loop);
        Debug.Log($"{GetType()} /PlayAnimation() => ���Ŷ���: {trackEntry.Animation.Name}");
        trackEntry.MixDuration = 0.2f; // ������Ϸ�ֹͻ��
        trackEntry.Complete += TrackEntry_Complete;
        
    }

    private void TrackEntry_Complete(TrackEntry trackEntry)
    {
        Debug.Log($"{GetType()} /TrackEntry_Complete() => ����: {trackEntry.Animation.Name} �������");
    }

    public float AnimationTotalTime(int trackIndex = 0)
    {
        TrackEntry trackEntry = m_animRenderer.state.GetCurrent(trackIndex);
        if (trackEntry != null)
        {
            Animation currentAnimation = trackEntry.Animation;
            float animationDuration = currentAnimation.Duration;
            return animationDuration;
        }
        return 0f;
    }
}
