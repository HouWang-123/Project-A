using System.Collections.Generic;
using UnityEngine;
using YooAsset;
/// <summary>
/// 远程攻击状态
/// </summary>
public class RangedAttackState : BaseState
{
    // 玩家的位置
    private readonly Transform m_playerTransform;
    // 近战攻击范围
    private readonly float m_MeleeDistance = -1f;
    // 远程攻击范围
    private readonly float m_RangedDistance = -1f;
    // 动画播放的时候攻击了一次
    private bool m_enterCD = false;
    // 攻击了没有
    private bool m_attacked = false;
    // 动画总时间
    private float m_animTotalTime = 0f;
    // 攻击CD
    private readonly float m_attackCD = 5f;
    // 发射物的初始位置
    private readonly Transform m_projectileTransform;
    // 发射物内存池
    private readonly Queue<GameObject> m_projectileQueue;
    // 发射物内存池容量
    private readonly int m_projectileListCapacity = 16;
    // 记录发射物总共内存块的个数
    private int m_projectileCount = 0;
    // 记录距离开始的时间
    private float m_timer = 0f;
    // 子弹预制体的引用
    private readonly AssetHandle m_assetHandle;
    // 转为逃跑的光源距离
    private readonly float m_fleeDistance = -1f;
    // 弹道修正向量
    private readonly Vector3 m_projectileFixVector3;
    public RangedAttackState(FiniteStateMachine finiteStateMachine, GameObject npcObj, Transform projectileTrans, Transform playerTransform)
        : base(finiteStateMachine, npcObj)
    {
        // 设置状态
        m_stateEnum = StateEnum.RangedAttack;
        m_projectileTransform = projectileTrans;
        
        if (playerTransform != null)
        {
            m_playerTransform = playerTransform;
        }
        else
        {
            m_playerTransform = GameObject.Find("Player000").transform;
        }
        var monsterBaseFSM = npcObj.GetComponent<MonsterBaseFSM>();
        m_RangedDistance = monsterBaseFSM.MonsterDatas.ShootRange; // 应该获取表中的远程范围
        m_MeleeDistance = monsterBaseFSM.MonsterDatas.HitRange;
        m_projectileFixVector3 = new Vector3(0, 1, 0);
        m_fleeDistance = 3f;

        // 提前生成发射物
        m_projectileQueue = new(m_projectileListCapacity);
        // 读取发射物预制体
        m_assetHandle = YooAssets.LoadAssetSync<GameObject>("BlueMucus");

        for (int i = 0; i < m_projectileListCapacity; ++i)
        {
            GameObject go = Object.Instantiate(m_assetHandle.AssetObject) as GameObject;
            go.name = "发射物_" + i;
            go.transform.SetParent(m_projectileTransform);
            go.transform.SetPositionAndRotation(go.transform.parent.position, go.transform.parent.rotation);
            go.GetComponent<ProjectileControl>().ReturnFunc = ReturnProjectile;
            go.GetComponent<ProjectileControl>().MonsterBaseFSM = monsterBaseFSM;
            go.SetActive(false);
            m_projectileQueue.Enqueue(go);
            ++m_projectileCount;
        }
    }
    public GameObject GetProjectile()
    {
        if (m_projectileQueue.Count > 0)
        {
            var projectile = m_projectileQueue.Dequeue();
            projectile.SetActive(true);
            return projectile;
        }
        else
        {
            GameObject go = Object.Instantiate(m_assetHandle.AssetObject) as GameObject;
            go.name = "发射物_" + m_projectileCount;
            go.transform.SetParent(m_projectileTransform);
            go.transform.SetPositionAndRotation(go.transform.parent.position, go.transform.parent.rotation);
            go.GetComponent<ProjectileControl>().ReturnFunc = ReturnProjectile;
            go.SetActive(false);
            ++m_projectileCount;
            return go;
        }
    }

    public void ReturnProjectile(GameObject projectile)
    {
        projectile.SetActive(false);
        projectile.transform.SetPositionAndRotation(projectile.transform.parent.position, projectile.transform.parent.rotation);
        m_projectileQueue.Enqueue(projectile);
    }

    public override void Act(GameObject npc)
    {
        // 模拟播放攻击动画
        m_timer += Time.deltaTime * m_timeScale;
        if (!m_enterCD)
        {
            var monsterFSM = m_gameObject.GetComponent<MonsterBaseFSM>();
            AnimationController.PlayAnim(m_gameObject, StateEnum.RangedAttack, 0, false, m_timeScale);
            m_animTotalTime = AnimationController.AnimationTotalTime(monsterFSM.SkeletonAnim);
            m_enterCD = true;
        }
        if (m_timer >= m_animTotalTime && !m_attacked)
        {
            Debug.Log(GetType() + " /Act() => 动画播放完成，并生成发射物");
            GameObject projectileGo = GetProjectile();
            if (projectileGo != null)
            {
                var com = projectileGo.GetComponent<ProjectileControl>();
                // m_projectileFixVector3为弹道修正，往上打一点
                com.Direction = (m_playerTransform.position + m_projectileFixVector3 - m_projectileTransform.position).normalized;
            }
            m_attacked = true;
            AnimationController.PlayAnim(m_gameObject, StateEnum.Idle, 0, false, m_timeScale);
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
        float distance = Vector3.Distance(npc.transform.position, m_playerTransform.position);
        // 大于m_RangedDistance米就转为追逐玩家
        if (distance > m_RangedDistance)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.ChasePlayer);
        }
        // 转为近战
        if (m_gameObject.GetComponent<MonsterBaseFSM>().MonsterDatas.HitRange != -1f && distance <= m_MeleeDistance)
        {
            m_finiteStateMachine.PerformTransition(TransitionEnum.MeleeAttackPlayer);
        }
        // 发现光源直接逃跑
        var lightTransform = m_gameObject.GetComponent<MonsterBaseFSM>().LightTransform;
        if (lightTransform != null)
        {
            if (Vector3.Distance(lightTransform.position, m_gameObject.transform.position) <= m_fleeDistance)
            {
                m_finiteStateMachine.PerformTransition(TransitionEnum.FleeAction);
            }
        }
    }

    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
        AnimationController.PlayAnim(m_gameObject, StateEnum.RangedAttack, 0, false, m_timeScale);
    }
}

