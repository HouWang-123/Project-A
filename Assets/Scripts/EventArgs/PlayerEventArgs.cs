using GameFramework.Event;

public enum PlayerEventType
{
    TriggerShopPanel = 1,
    BuyFollower = 2,
    UpgradeItem = 3,
    TriggerUpgradePanel = 4,
    BuyAIPlayer = 5, 
    OnUnlocked = 6,
    ExitGame = 7,   // 退出游戏时通知
    ClaimMoney = 8,
    NotifyFreshUpgradeView = 9, //通知刷新升级界面信息
    InterstitialAdClose = 10,
    UpgradeByAd = 11,
    BannerClicked = 12, // Banner点击
    RewardAdClicked = 13, // 激励广告点击
    RewardAdPlayedSucceed = 14, // 激励广告播放成功
    RewardAdPlayedFailure = 15,
    BuyNoAds = 16, // 购买广告
    BuyNoAdsSucceed = 17,
    BuyNoAdsFailure = 18
}
public class PlayerEventArgs : GameEventArgs
{
    public static readonly int EventId = typeof(PlayerEventArgs).GetHashCode();
    public override int Id { get { return EventId; } }
    public PlayerEventType EventType { get; private set; }
    public object EventData { get; private set; }
    public override void Clear()
    {
        this.EventData = null;
    }
    public PlayerEventArgs Fill(PlayerEventType eventType, object eventData = null)
    {
        this.EventType = eventType;
        this.EventData = eventData;
        return this;
    }
}
