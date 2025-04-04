using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

[Serializable]
public class MapTrackDataManager : SerializedMonoBehaviour
{
    public Dictionary<int, List<TrackerData>> AllTrackedData = new ();
    public HashSet<ITrackable> CurrentTrackerList = new ();
    public HashSet<int> RoomIdList = new();
    
    
    public void RegisterTracker(ITrackable trackerData)
    {
        CurrentTrackerList.Add(trackerData);
    }

    public void UnRegisterTracker(ITrackable trackerData)
    {
        CurrentTrackerList.Remove(trackerData);
    }
    
    
    public void OnRoomDataLoadComplete(int roomid)
    {
        RoomIdList.Add(roomid);
        AllTrackedData.Add(roomid,new List<TrackerData>());
    }
    /// <summary>
    /// 判断某个场景是否生成过
    /// </summary>
    /// <returns></returns>
    public bool RoomGenerated(int roomid)
    {
        if (RoomIdList.Contains(roomid))
        {
            return true;
        }
        return false;
    }

    public void SaveTrackerData(int roomid)
    {
        List<TrackerData> recorded = new List<TrackerData>();
        foreach (var VARIABLE in CurrentTrackerList)
        {
            TrackerData collectTrackedData = VARIABLE.CollectTrackedData();
            recorded.Add(collectTrackedData);
        }
        AllTrackedData[roomid] = recorded;
    }
    /// <summary>
    /// 根据追踪到的场景数据重新生成对应实例，并赋状态
    /// </summary>
    public void RecoverItem(int roomid)
    {
        foreach (var data in AllTrackedData[roomid])
        {
            if (data.TrackType == TrackType.Item)
            {
                ItemStatus itemStatus = data.TrackableBaseData as ItemStatus;
                if (itemStatus.StackCount > 1)
                {
                    GameItemTool.GenerateItemAtTransform(data.id,data.postion);
                }
                else
                {
                    GameItemTool.GenerateStackableItemAtTransform(data.id,itemStatus.StackCount,data.postion);
                }
            }
        }
    }
    public void RecoverEnemy(int roomid)
    {
        foreach (var data in AllTrackedData[roomid])
        {
            if (data.TrackType == TrackType.Enemy)
            {
                
            }
        }
    }
}
