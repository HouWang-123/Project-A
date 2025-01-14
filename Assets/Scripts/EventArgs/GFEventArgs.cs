using GameFramework.Event;

public enum GFEventType
{
    ResourceInitialized = 0, //游戏资源初始化完成后
    RewardAdLoaded = 1, // 激励广告已加载
    LevelLoaded = 2 // 关卡已加载
}
public class GFEventArgs : GameEventArgs
{
    public static readonly int EventId = typeof(GFEventArgs).GetHashCode();
    public override int Id => EventId;
    public GFEventType EventType { get; private set; }
    public object UserData { get; private set; }
    public override void Clear()
    {
        UserData = null;
    }
    public GFEventArgs Fill(GFEventType eventType, object userDt = null)
    {
        this.EventType = eventType;
        this.UserData = userDt;
        return this;
    }
}
