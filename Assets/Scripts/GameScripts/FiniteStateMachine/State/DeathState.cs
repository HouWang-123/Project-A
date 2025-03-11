using UnityEngine;

public class DeathState : BaseState
{
    private readonly float m_destoryTime = 10f;
    private float m_timer = 0f;
    public DeathState(FiniteStateMachine finiteStateMachine, GameObject gameObject) : base(finiteStateMachine, gameObject)
    {
        m_stateEnum = StateEnum.Death;
    }
    public override void Act(GameObject npc)
    {
        // 死亡动画
        AnimationController.PlayAnim(m_gameObject, StateEnum.Death, 0, false, m_timeScale);
    }
    public override void Condition(GameObject npc)
    {
        // 死亡后不再切换状态，销毁自己
        m_timer += Time.deltaTime;
        if (m_timer >= m_destoryTime)
        {
            GameObject.Destroy(m_gameObject);
        }
    }
}

