using UnityEngine;

public class DeathState : BaseState
{
    private readonly float m_destoryTime = 10f;
    private float m_timer = 0f;
    private float m_animTime = float.MaxValue;
    public DeathState(FiniteStateMachine finiteStateMachine, GameObject gameObject) : base(finiteStateMachine, gameObject)
    {
        m_stateEnum = StateEnum.Death;
    }
    public override void Act(GameObject npc)
    {

    }
    public override void Condition(GameObject npc)
    {
        // 死亡后不再切换状态，销毁自己
        m_timer += Time.deltaTime;
        // 去除碰撞体
        if (m_timer >= m_animTime)
        {
            m_monsterBaseFSM.Collider.SetActive(false);
        }
        if (m_timer >= m_destoryTime)
        {
            GameObject.Destroy(m_gameObject);
        }
    }

    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
        // 死亡动画
        MonsterAnimationController.PlayAnim(m_gameObject, StateEnum.Death, 0, false, m_timeScale);
        m_animTime = MonsterAnimationController.AnimationTotalTime(m_monsterBaseFSM.SkeletonAnim);
    }
}

