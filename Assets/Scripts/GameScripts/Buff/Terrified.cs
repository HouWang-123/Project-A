public class Terrified : BuffComponet
{
    public override void Execute()
    {
        base.Execute();
        playerData.Strength += 1;
    }
}