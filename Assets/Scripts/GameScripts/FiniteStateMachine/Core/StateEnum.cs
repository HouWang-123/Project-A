/// <summary>
/// 状态
/// </summary>
public enum StateEnum
{
    None = 0,
    // 待机
    Idle,
    // 游荡
    Patrol,
    // 看向玩家
    LookAt,
    // 追逐
    Chase,
    // 近战攻击
    MeleeAttack,
    // 远程攻击
    RangedAttack,
    // 逃避
    Flee,
    // 死亡
    Death
}

