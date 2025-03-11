/// <summary>
/// 转换过渡
/// </summary>
public enum TransitionEnum
{
    None = 0,
    // 变为待机
    ToIdle,
    // 看见玩家
    SeePlayer,
    // 追逐玩家
    ChasePlayer,
    // 丢失玩家
    LostPlayer,
    // 近战攻击玩家
    MeleeAttackPlayer,
    // 远程攻击玩家
    RangedAttackPlayer,
    // 该逃避了
    FleeAction,
    // 死亡
    ToDeath
}

