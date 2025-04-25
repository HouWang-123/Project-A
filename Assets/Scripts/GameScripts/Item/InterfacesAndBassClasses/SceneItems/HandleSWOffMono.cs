using System;
using UnityEngine;

public class HandleSWOffMono : BaseSWMono
{
    public bool IsTrue;

    public override void OnPlayerFocus()
    {
        base.OnPlayerFocus();
    }

    public override void OnPlayerStartInteract(int itemid)
    {
        Debug.Log("开启开关");
        ChangeState();
    }
    
    public override void ChangeState()
    {
        base.ChangeState();
        ChangeToItem(220014);
    }

    public override MonoBehaviour getMonoBehaviour()
    {
        return this;
    }
}