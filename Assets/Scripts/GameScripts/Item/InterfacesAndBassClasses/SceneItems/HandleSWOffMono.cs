using System;
using UnityEngine;

public class HandleSWOffMono : BaseSWMono
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
    }

    public override void OnPlayerInteract()
    {
        base.OnPlayerInteract();
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