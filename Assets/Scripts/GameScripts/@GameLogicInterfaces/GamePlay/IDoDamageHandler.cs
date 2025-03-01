/// <summary>
/// 可造成伤害的物体
/// </summary>
public interface IDoDamageHandler
{
    public int GetDamage();
    public void DoDamage(IDamageable obj);
}
/// <summary>
/// 伤害接收
/// </summary>
public interface IDamageable
{
    public void DamageReceive();
}