using cfg.mon;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MonsterBaseFSM : MonoBehaviour,IDamageable
{
    protected FiniteStateMachine m_fsm;
    protected float CurrentHp;
    [Header("将要生成的怪物的ID")]
    [SerializeField]
    protected int m_monsterID;
    // 怪物数据
    protected Monster m_monsterDatas;
    public Monster MonsterDatas { get { return m_monsterDatas; } }
    // 怪物的动画
    [Header("怪物不同状态对应的动画")]
    [SerializeField]
    protected Dictionary<StateEnum, List<string>> m_animationEnumWithName;
    public Dictionary<StateEnum, List<string>> AnimationEnumWithName { get { return m_animationEnumWithName; } }
    // 怪物NavMeshAgent
    [Header("怪物的NavMeshAgent")]
    [SerializeField]
    protected NavMeshAgent m_NavMeshAgent;
    public NavMeshAgent NavMeshAgent { get { return m_NavMeshAgent; } }
    // 怪物的骨骼动画
    [Header("怪物的骨骼动画")]
    [SerializeField]
    protected SkeletonAnimation m_skeletonAnim;
    public SkeletonAnimation SkeletonAnim { get { return m_skeletonAnim; } }
    // 头顶信息
    [Header("怪物的头顶信息")]
    [SerializeField]
    protected GameObject m_infoRenderer;
    public GameObject InfoRenderer { get { return m_infoRenderer; } }
    // 光源位置
    private Transform m_lightTransform;
    public Transform LightTransform { get { return m_lightTransform; } }

    protected virtual void Init()
    {
        m_monsterDatas = GameTableDataAgent.MonsterTable.Get(m_monsterID);

        if (m_monsterDatas == null)
        {
            Debug.LogError(GetType() + " /Start() => 请检查怪物的ID是否和表格一致！");
        }

        if (m_infoRenderer == null)
        {
            m_infoRenderer = transform.GetChild(1).gameObject;
        }
        if (m_NavMeshAgent == null)
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
        }
        m_NavMeshAgent.speed = m_monsterDatas.Speed;

        if (m_lightTransform == null)
        {
            Light light = FindAnyObjectByType<Light>();
            if (light != null)
            {
                m_lightTransform = light.transform;
            }
        }

        if (m_skeletonAnim == null)
        {
            m_skeletonAnim = transform.GetChild(0).GetComponent<SkeletonAnimation>();
        }
        m_skeletonAnim.Initialize(true);

        m_animationEnumWithName = new()
        {
            { StateEnum.Idle, new() { "Idle" } }
        };
        CurrentHp = m_monsterDatas.MaxHP;
    }

    protected virtual void DoUpdate()
    {
        m_fsm.DoUpdate(gameObject);

        // 检测附近是否存在光源
        Light light = FindAnyObjectByType<Light>();
        if (light != null)
        {
            m_lightTransform = light.transform;

        }
    }

    protected virtual void InitFSM()
    {
        // 构建有限状态机
        m_fsm = new();
    }
    protected virtual void HurtPlayer()
    {
    }
    /// <summary>
    ///  添加伤害接收接口
    /// </summary>
    /// <param name="DamageAmount"></param>
    public void DamageReceive(float DamageAmount)
    {
        CurrentHp -= DamageAmount;
        CheckDeath();
    }

    private void CheckDeath()
    {
        if (CurrentHp < 0)
        {
            Destroy(gameObject);
        }
    }
}

