using GameFramework.Event;
public enum UserDataType
{
    MONEY = 1,
    ADD_EFFECT = 2,
    GAME_LEVEL = 3,
    AD2MONEY_LV = 4,
    FOLLOWER_NUM_CHANGED = 5,
    Removed_ADS = 6,
    IS_WIN = 7,
    LANGUAGE_CHANGED = 8,
    LEVEL_USED_TIME = 9,
    LEVEL_FILL_NUM = 10,
    FREE_AD_TIMES = 11,
    NMN_LEVEL_ID = 12
}
public class UserDataChangedEventArgs : GameEventArgs
{
    public static readonly int EventId = typeof(UserDataChangedEventArgs).GetHashCode();
    public override int Id { get { return EventId; } }
    public UserDataType Type { get; private set; }
    public object OldValue { get; private set; }
    public object Value { get; private set; }
    public override void Clear()
    {
        Type = default;
        Value = null;
        OldValue = null;
    }
    public UserDataChangedEventArgs Fill(UserDataType type,object oldV, object newV)
    {
        Type = type;
        OldValue = oldV;
        Value = newV;
        return this;
    }
    public override string ToString()
    {
        return $"class:{GetType()},type:{Type}, Value:{Value}, OldValue:{OldValue}\n";
    }
}
