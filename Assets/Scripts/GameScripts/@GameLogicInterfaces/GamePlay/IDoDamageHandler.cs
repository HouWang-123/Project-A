/// <summary>
/// 可造成伤害的物体
/// </summary>
public interface IDoDamageHandler
{
    public void SetInitialDamage(float damage);
}
/// <summary>
/// 伤害接收
/// </summary>
public interface IDamageable
{
    public void DamageReceive(float DamageAmount);
}