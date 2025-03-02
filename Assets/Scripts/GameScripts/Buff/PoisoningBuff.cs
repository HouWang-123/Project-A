using System.Threading.Tasks;
using UnityEngine;
using cfg.buff;
using FEVM.Timmer;
/// <summary>
/// 中毒
/// </summary>
public class PoisoningBuff : BuffComponet
{
    public override void Execute()
    {
        base.Execute();
        AsyncTimer.Instance.StartTask(BuffConfig.PoisoningBuff,2f,BuffTime, () =>
        {
            GameRunTimeData.Instance.CharacterBasicStat.HurtPlayer(2f);
        });
    }
}