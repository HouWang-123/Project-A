using UnityEngine;

public class GameConstData
{
    public const string GameStart = "GameStart";
    //默认角度，45
    public static readonly Vector3 DefAngles = new(45, 0, 0);
    // 正向
    public static readonly Vector3 NormalScale = new(1f, 1f, 1f);
    //反向
    internal static readonly Vector3 ReverseScale = new(-1,1,1);
    public static readonly Vector3 ReversedRotation = new(0, -180, 0);
    
    public static readonly int DEFAULT_SLOTCOUNT = 6;
    public static readonly int DEFAULT_CHARACTER_ID = 110001;
    
    // 渲染顺序
    public static readonly int OverPlayerOrder = 1;
    public static readonly int PlayerOrder = 0;
    public static readonly int BelowPlayerOrder = 0;

    // 物品垂直丢弃速度
    public static readonly Vector3 VthrowSpeed = new (0, 1.8f, 0);
}
