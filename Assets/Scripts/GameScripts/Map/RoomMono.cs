using cfg.scene;
using System.Collections.Generic;
using UnityEngine;

public class RoomMono : MonoBehaviour
{
    private Rooms roomData;
    public Transform PlayerPoint;
    public GameObject SceneItemNode; // 场景道具物品节点

    private Transform doorParent;

    private Dictionary<int, DoorMono> doorDic = new Dictionary<int, DoorMono>();

    private void Awake()
    {

    }

    private void Start()
    {
        if(PlayerPoint != null)
        {
            GameControl.Instance.GetGamePlayer().transform.position = PlayerPoint.position;
        }
        GameControl.Instance.SetSceneItemList(SceneItemNode);
    }


    public void SetData(Rooms room)
    {
        roomData = room;
        doorParent = transform.Find("Door");
        if(doorParent != null)
        {
            if(roomData.DoorList.Count != doorParent.childCount)
            {
                Debug.LogWarning("room数据所含door数量与实际预制体的door不同，请检查问题！！！");
            }
            for(int i = 0; i < roomData.DoorList.Count && i < doorParent.childCount; i++)
            {
                DoorMono mono = doorParent.GetChild(i).GetComponent<DoorMono>();
                if(mono == null)
                {
                    mono = doorParent.GetChild(i).gameObject.AddComponent<DoorMono>();
                }
                mono.SetDate(roomData.DoorList[i]);
                doorDic.Add(roomData.DoorList[i], mono);
            }
        }
    }


    public void SetPlayPoint(int doorId)
    {
        PlayerPoint = doorDic[doorId].GetPlayerPoint();
    }
}
