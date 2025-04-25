using System;
using UnityEngine;

public class HandleSWOnMono : BaseSWMono
{
    public bool IsTrue;

    private void Start()
    {
        
    }

    public override void OnPlayerFocus()
    {
        base.OnPlayerFocus();
    }

    public override void OnPlayerStartInteract()
    {
        base.OnPlayerStartInteract();
        ChangeState();
    }

    public override void ChangeState()
    {
        base.ChangeState();
        ChangeToItem(220013);
    }
    
    public override void OnPlayerInteract()
    {
        base.OnPlayerInteract();
        if (!IsTrue)
        {
            GameRunTimeData.Instance.CharacterBasicStat.GetStat().CurrentHp -= 9999;
        }
    }

    public override MonoBehaviour getMonoBehaviour()
    {
        return this;
    }
}