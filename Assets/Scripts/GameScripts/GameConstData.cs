using UnityEngine;

public class GameConstData
{
    public const string GameStart = "GameStart";
    //默认角度，45
    public static readonly Vector3 DefAngles = new(45, 0, 0);

    public static readonly Vector3 ReversedDefAngles = new (135, -180, 0);
    // X正向
    public static readonly Vector3 XNormalScale = new(1f, 1f, 1f);
    // X反向
    internal static readonly Vector3 XReverseScale = new(-1f, 1f, 1f);
    public static readonly Vector3 ReversedRotation = new(0, -180, 0);
    
    public static readonly int DEFAULT_SLOTCOUNT = 6;
    public static readonly int DEFAULT_CHARACTER_ID = 110001;
    
    // 渲染顺序
    public static readonly int OverPlayerOrder = 1;
    public static readonly int PlayerOrder = 0;
    public static readonly int BelowPlayerOrder = 0;

    // 物品垂直丢弃速度
    public static readonly Vector3 VthrowSpeed = new (0, 1.8f, 0);

    // 谜题物体标签
    public static readonly string RIDDLE_TAG = "Riddle";
}
