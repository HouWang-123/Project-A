using System;
using UnityEngine;

public class HandleSWOnMono : BaseSWMono
{
    public bool IsTrue;
    public override void OnPlayerFocus()
    {
        base.OnPlayerFocus();
    }

    public override void OnPlayerStartInteract(int itemid)
    {
        Debug.Log("关闭开关");
        ChangeState();
    }

    public override void ChangeState()
    {
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