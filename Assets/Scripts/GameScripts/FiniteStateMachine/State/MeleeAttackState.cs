using UnityEngine;
/// <summary>
/// 近战攻击状态
/// </summary>
public class MeleeAttackState : BaseState
{
    // 玩家的位置
    private readonly Transform m_playerTransform;
    // 近战攻击位置
    private readonly Transform m_meleeTransform;
    // 近战攻击范围
    private readonly float m_MeleeDistance = -1f;
    // 近战攻击角度
    private readonly float m_MeleeDegree = 1f;
    // 动画播放的时候攻击了一次
    private bool m_enterCD = false;
    // 攻击了没有
    private bool m_attacked = false;
    // 动画播放时间
    private float m_animTotalTime = 0f;
    // 攻击CD
    private readonly float m_attackCD = 1f;
    // 记录距离开始的时间
    private float m_timer = 0f;
    // 转为逃跑的光源距离
    private readonly float m_fleeDistance = -1f;
    // 玩家灯光组件
    private readonly FlashLightBehaviour lightCom;
    public MeleeAttackState(FiniteStateMachine finiteStateMachine, GameObject npcObj, Transform meleeTransform, Transform playerTransform)
        : base(finiteStateMachine, npcObj)
    {
        // 设置状态
        m_stateEnum = StateEnum.MeleeAttack;
        m_meleeTransform = meleeTransform;
        m_playerTransform = playerTransform;
        if (playerTransform == null)
        {
            m_playerTransform = GameObject.Find("Player000").transform;
        }
        m_MeleeDistance = m_monsterBaseFSM.MonsterDatas.HitRange; // 应该获取表中的近战范围
        m_MeleeDegree = m_monsterBaseFSM.MonsterDatas.HitDegree / 2.0f;
        m_fleeDistance = 3f;
        lightCom = m_monsterBaseFSM.LightComponent;
    }

    public override void Act(GameObject npc)
    {
        m_timer += Time.deltaTime * m_timeScale;
        if (!m_enterCD)
        {
            // 播放动画前朝向玩家
            float direction = m_playerTransform.position.x - m_monsterBaseFSM.transform.position.x;
            // 玩家在NPC左边，看向左边
            Vector3 scale;
            if (direction > 0f)
            {
                scale = GameConstData.XNormalScale;
            }
            else
            {
                scale = GameConstData.XReverseScale;
            }
            m_monsterBaseFSM.transform.localScale = scale;
            MonsterAnimationController.PlayAnim(m_gameObject, StateEnum.MeleeAttack, 0, false, m_timeScale);
            m_animTotalTime = MonsterAnimationController.AnimationTotalTime(m_monsterBaseFSM.SkeletonAnim);
            m_enterCD = true;
        }
        
        if (m_timer >= m_animTotalTime && !m_attacked)
        {
            Debug.Log(GetType() + " /Act() => 动画播放完成，检测玩家是否在范围内");
            // 进行射线检测
            int rayCount = 16; // 射线的数量
            Vector3 origin = m_meleeTransform.position;
            // 射线从怪物正前方发射
            float forwardf = m_playerTransform.position.x - m_monsterBaseFSM.transform.position.x;

            Vector3 forward = forwardf > 0f ? Vector3.right : Vector3.left;

            float angleStep = (m_MeleeDegree - -m_MeleeDegree) / (rayCount - 1);

            for (int i = 0; i < rayCount; ++i)
            {
                float angle = -m_MeleeDegree + i * angleStep;
                Vector3 direction = Quaternion.Euler(0, angle, 0) * forward;

                Ray ray = new(origin, direction);

                if (Physics.Raycast(ray, out RaycastHit hit, m_MeleeDistance))
                {
                    // 处理检测到的物体
                    Debug.DrawLine(ray.origin, hit.point, Color.red, 2.0f);
                    if (hit.collider != null)
                    {
                        Debug.Log(GetType() + " /Act() => 命中碰撞体: " + hit.collider.gameObject.name + " 距离: " + hit.distance);
                        if (hit.collider.gameObject.TryGetComponent<PlayerControl>(out _))
                        {
                            if (m_monsterBaseFSM is DrownedOnesFSM drownedOnesFSM)
                            {
                                EventManager.Instance.RunEvent(drownedOnesFSM.MeleeAttackHurtEventName);
                            }
                            else if (m_monsterBaseFSM is HoundTindalosFSM houndTindalosFSM)
                            {
                                EventManager.Instance.RunEvent(houndTindalosFSM.MeleeAttackHurtEventName);
                            }
                            break;
                        }
                    }
                    if (hit.rigidbody != null)
                    {
                        Debug.Log(GetType() + " /Act() => 命中刚体: " + hit.rigidbody.gameObject.name + " 距离: " + hit.distance);
                        if (hit.rigidbody.gameObject.TryGetComponent<PlayerControl>(out _))
                        {
                            if (m_monsterBaseFSM is DrownedOnesFSM drownedOnesFSM)
                            {
                                EventManager.Instance.RunEvent(drownedOnesFSM.RangedAttackHurtEventName);
                            }
                            else if (m_monsterBaseFSM is HoundTindalosFSM houndTindalosFSM)
                            {
                                EventManager.Instance.RunEvent(houndTindalosFSM.MeleeAttackHurtEventName);
                            }
                            break;
                        }
                    }
                }
            }

            m_attacked = true;
            MonsterAnimationController.PlayAnim(m_gameObject, StateEnum.Idle, 0, false, m_timeScale);
        }
        if (m_timer >= m_attackCD + m_animTotalTime)
        {
            // 重置CD
            m_timer = 0f;
            m_enterCD = false;
            m_attacked = false;
        }
    }

    public override void Condition(GameObject npc)
    {
        // 不处于攻击CD，大于m_MeleeDistance米
        if (!m_enterCD && Vector3.Distance(npc.transform.position, m_playerTransform.position) > m_MeleeDistance)
        {
            // 具有远程攻击，转为远程攻击状态
            if (m_monsterBaseFSM.MonsterDatas.ShootRange != -1f)
            {
                m_finiteStateMachine.PerformTransition(TransitionEnum.RangedAttackPlayer);
            }
            else // 否则转为追逐
            {
                m_finiteStateMachine.PerformTransition(TransitionEnum.ChasePlayer);
            }
        }
        // 发现光源直接逃跑
        /*if (lightCom != null && lightCom.isOn)
        {
            Transform lightTransform = lightCom.transform;
            if (Vector3.Distance(lightTransform.position, m_gameObject.transform.position) <= m_fleeDistance)
            {
                m_finiteStateMachine.PerformTransition(TransitionEnum.FleeAction);
            }
        }*/
        if (m_monsterBaseFSM.IsLightOnMonster)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.FleeAction);
        }
    }
    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
        MonsterAnimationController.PlayAnim(m_gameObject, StateEnum.MeleeAttack, 0, false, m_timeScale);
    }



}

