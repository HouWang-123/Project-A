using System;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 存储场景后需要追踪的数据
/// </summary>
[Serializable]
public abstract class TrackableBaseData
{
    
}

[Serializable]
public class TrackerData
{
    public int ID;
    public TrackType TrackType;
    
    public Vector3 Position;
    public Vector3 EulerAngle;
    public Vector3 Scale;
    
    public TrackableBaseData TrackableBaseData;

    public TrackerData(int id,TrackType trackType,Vector3 position,Vector3 eulerAngle, Vector3 scale, TrackableBaseData trackableBaseData)
    {
        ID = id;                        // 对象id
        TrackType = trackType;               // 追踪类型
        Position = position;              // 位置
        EulerAngle = eulerAngle;
        Scale = scale;
        TrackableBaseData = trackableBaseData;  // 其他可追踪数据
        
    }
}
[Serializable]
public enum TrackType
{
    Item,
    ItemPoint,
    Enemy
}

public interface ITrackable
{
    public void RegisterTracker();
    public void UnRegisterTracker();
    public TrackerData CollectTrackedData();
}
