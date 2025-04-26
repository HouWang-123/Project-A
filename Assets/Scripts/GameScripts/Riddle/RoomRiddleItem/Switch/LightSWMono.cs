using System;
using UnityEngine;

public class LightSWMono : RiddleSwitch
{
    public override void OnPlayerFocus()
    {
        base.OnPlayerFocus();
    }

    public override void OnPlayerDefocus()
    {
        base.OnPlayerDefocus();
    }

    public override void OnPlayerStartInteract()
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
        // IsOn = true;
        // ChangeState();
    }

    private void OnTriggerExit(Collider other)
    {
        // IsOn = false;
        // ChangeState();
    }
}