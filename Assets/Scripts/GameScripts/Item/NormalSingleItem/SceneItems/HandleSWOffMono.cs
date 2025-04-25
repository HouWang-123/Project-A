using System;
using UnityEngine;

public class HandleSWOffMono : BaseSWMono
{
    public bool IsTrue;
    public override void OnPlayerStartInteract(int itemid)
    {
        if (startactionTime < 0.2f) return;
        Debug.Log("开启开关");
        ChangeState();
    }
    
    public override void ChangeState()
    {
        base.ChangeState();
        ChangeToItem(220014,(newItem => {UpdateInteractHandler(newItem,this);}));
    }

    public override MonoBehaviour getMonoBehaviour()
    {
        return this;
    }
}