using cfg.scene;
using System.Collections.Generic;
using UnityEngine;

public class RoomMono : MonoBehaviour
{
    private Rooms roomData;
    private Transform playerPoint;
    public GameObject SceneItemNode; // 场景道具物品节点
    public GameObject monsterList;

    private Transform doorParent;
    private Dictionary<int, DoorMono> doorDic = new Dictionary<int, DoorMono>();

    //public Transform monsterPoint;    // 怪物生成点（测试用）

    private void Awake()
    {

    }

    private void Start()
    {
        if(playerPoint != null)
        {
            GameControl.Instance.GetGamePlayer().transform.position = playerPoint.position;
        }
        else
        {
            GameControl.Instance.GetGamePlayer().transform.position = Vector3.zero;
        }

        GameControl.Instance.SetSceneItemList(SceneItemNode);
    }

    private bool doorInited;
    public void SetData(Rooms room)
    {
        GameControl.Instance.SetSceneItemList(SceneItemNode);
        roomData = room;
        doorParent = transform.Find("Door");
        if(doorParent != null)
        {
            if(roomData.DoorList.Count != doorParent.childCount)
            {
                Debug.LogWarning("room数据所含door数量与实际预制体的door不同，请检查问题！！！");
            }
            
            if (doorInited) return;
            for(int i = 0; i < roomData.DoorList.Count && i < doorParent.childCount; i++)
            {
                DoorMono mono = doorParent.GetChild(i).GetComponent<DoorMono>();
                if(mono == null)
                {
                    mono = doorParent.GetChild(i).gameObject.AddComponent<DoorMono>();
                }
                mono.SetData(roomData.DoorList[i]);
                doorDic.Add(roomData.DoorList[i], mono);
            }
            doorInited = true;
        }
    }


    public void SetPlayPoint(int doorId)
    {
        playerPoint = doorDic[doorId].GetPlayerPoint();
        GameControl.Instance.GetGamePlayer().transform.position = playerPoint.position;
    }
}
