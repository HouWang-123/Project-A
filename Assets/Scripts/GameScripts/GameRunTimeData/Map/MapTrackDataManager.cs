using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class MapTrackDataManager
{
    public Dictionary<int, List<TrackerData>> AllTrackedData = new();
    public HashSet<ITrackable> CurrentTrackerList = new();
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
        if (AllTrackedData.ContainsKey(roomid)) return;
        AllTrackedData.Add(roomid, new List<TrackerData>());
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
            if (collectTrackedData != null)
            {
                recorded.Add(collectTrackedData);
            }
        }

        AllTrackedData[roomid] = recorded;
    }

    /// <summary>
    /// 根据追踪到的场景数据重新生成对应实例，并赋状态
    /// </summary>
    public void RecoverItem(int roomid, Transform itemNode)
    {
        foreach (var data in AllTrackedData[roomid])
        {
            if (data.TrackType == TrackType.Item)
            {
                ItemStatus itemStatus = data.TrackableBaseData as ItemStatus;
                if (itemStatus.StackCount > 1)
                {
                    GameItemTool.GenerateItemAtTransform(data.id, data.postion, false,
                        itembase => { itembase.transform.SetParent(itemNode); });
                }
                else
                {
                    GameItemTool.GenerateStackableItemAtTransform(data.id, itemStatus.StackCount, data.postion, false,
                        itembase => { itembase.transform.SetParent(itemNode); });
                }
            }
        }
    }

    // 恢复可交互物品生成点
    public void RecoverItemPoint(int roomid)
    {
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