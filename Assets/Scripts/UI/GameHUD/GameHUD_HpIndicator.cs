using System;
using DG.Tweening;
using UnityEngine.UI;

public class GameHUD_HpIndicator : UIBase
{
    public Image HpBase;
    

    protected override void AddListen()
    {
    }

    public void UpdateHp()
    {
        HpBase.DOKill();
        float ratiao = GameHUD.CharacterStatHUDStat.CurrentHp / GameHUD.CharacterStatHUDStat.MaxHP;
        HpBase.DOFillAmount(ratiao,0.3f);
    }
}
