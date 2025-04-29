public class RiddleClock : RiddleItemBase
{
    public RiddleTriggerNode Mytrigger;
    public override void OnPlayerInteract()
    {
        Mytrigger.OnTrigger();
    }

    public override RiddleItemBaseStatus GetRiddleStatus()
    {
        return null;
    }

    public override void SetRiddleItemStatus(RiddleItemBaseStatus BaseStatus)
    {
        
    }

    public override void OnPlayerStartInteract(int itemid)
    {
        OnPlayerInteract();
    }

    public override bool GetRiddleItemResult()
    {
        return true;
    }
}
