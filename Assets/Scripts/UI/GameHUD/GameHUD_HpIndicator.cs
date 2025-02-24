using UnityEngine.UI;

public class GameHUD_HpIndicator : UIBase
{
    public Image HpBase;
    protected override void AddListen()
    {
    }

    public void UpdateHp()
    {
        float ratiao = GameHUD.CharacterStatHUDStat.CurrentHp / GameHUD.CharacterStatHUDStat.MaxHP;
        HpBase.fillAmount = ratiao;
    }
}
