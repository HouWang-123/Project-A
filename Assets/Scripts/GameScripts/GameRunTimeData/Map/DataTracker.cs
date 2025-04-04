using System;
using SimpleJSON;
using UnityEngine;
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
    public int id;
    public TrackType TrackType;
    public Vector3 postion;
    public TrackableBaseData TrackableBaseData;

    public TrackerData(int id,TrackType trackType,Vector3 postion,TrackableBaseData trackableBaseData)
    {
        this.id = id;                        // 对象id
        TrackType = trackType;               // 追踪类型
        this.postion = postion;              // 位置
        TrackableBaseData = trackableBaseData;  // 其他可追踪数据
    }
}
[Serializable]
public enum TrackType
{
    Item,
    Enemy
}

public interface ITrackable
{
    public void RegisterTracker();
    public void UnRegisterTracker();
    public TrackerData CollectTrackedData();
}
