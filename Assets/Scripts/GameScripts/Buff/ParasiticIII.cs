/// <summary>
/// 寄生III
/// </summary>

public class ParasiticIII : BuffComponet
{
    public override void Execute()
    {
        base.Execute();
        GameRunTimeData.Instance.CharacterBasicStat.HurtPlayer(20f);
    }
}