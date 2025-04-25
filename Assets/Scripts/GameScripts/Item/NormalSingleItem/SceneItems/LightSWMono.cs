using System;
using UnityEngine;

public class LightSWMono : BaseSWMono
{
    public override void ChangeState()
    {
        base.ChangeState();
    }

    public override void OnPlayerFocus()
    {
        base.OnPlayerFocus();
    }

    public override void OnPlayerDefocus()
    {
        base.OnPlayerDefocus();
    }

    public override void OnPlayerStartInteract(int id)
    {
        base.OnPlayerStartInteract();
    }

    public override void OnPlayerInteractCancel()
    {
        base.OnPlayerInteractCancel();
    }

    private void OnTriggerStay(Collider other)
    {
        //todo 射线触发开关
        IsOn = true;
        ChangeState();
    }

    private void OnTriggerExit(Collider other)
    {
        IsOn = false;
        ChangeState();
    }
}