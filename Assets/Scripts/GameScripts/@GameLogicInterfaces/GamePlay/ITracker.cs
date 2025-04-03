using System;
using SimpleJSON;
using UnityEngine;
[Serializable]
public abstract class TrackableData
{
    
}

[Serializable]
public class TrackerData
{
    public int id;
    public TrackType TrackType;
    public Vector3 postion;
    public TrackableData TrackableData;

    public TrackerData(int id,TrackType trackType,Vector3 postion, TrackableData trackableData)
    {
        this.id = id;                        // 对象id
        TrackType = trackType;               // 追踪类型
        this.postion = postion;              // 位置
        this.TrackableData = trackableData;  // 其他可追踪数据
    }
}
[Serializable]
public enum TrackType
{
    Item,
    Enemy
}
public interface IMapObjectTracker
{
    public void RegistTracker(TrackerData trackerData);
    public void UnRegistTracker();
}
