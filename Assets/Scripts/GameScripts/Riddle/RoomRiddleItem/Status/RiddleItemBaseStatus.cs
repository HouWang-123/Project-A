using System;

[Serializable]
public abstract class RiddleItemBaseStatus
{
    public abstract bool GetResult();
}
[Serializable]
public class SwitchStatus : RiddleItemBaseStatus
{
    public bool is_on;

    public SwitchStatus(bool isOn)
    {
        is_on = isOn;
    }

    public override bool GetResult()
    {
        return is_on;
    }
}

