public class Basketball : Throwable
{
    protected override void Throw()
    {
        throw new System.NotImplementedException();
    }

    public override void OnInteract()
    {
        base.OnInteract();
        // Throw away
    }
}
