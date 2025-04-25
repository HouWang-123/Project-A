using System;
using UnityEngine;

public class HandleSWOnMono : BaseSWMono
{
    public bool IsTrue;
    public override void OnPlayerStartInteract(int itemid)
    {
        if (startactionTime < 0.2f) return;
        Debug.Log("关闭开关");
        ChangeState();
    }

    public override void ChangeState()
    {
        ChangeToItem(220013,
            newItem =>
            {
                UpdateInteractHandler(newItem,this);
            }
        );
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