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
        if (playerPoint != null)
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
        if (doorParent != null)
        {
            if (roomData.DoorList.Count != doorParent.childCount)
            {
                Debug.LogWarning("room数据所含door数量与实际预制体的door不同，请检查问题！！！");
            }
            SetUpCamCollider();
            if (doorInited) return;
            for (int i = 0; i < roomData.DoorList.Count && i < doorParent.childCount; i++)
            {
                DoorMono mono = doorParent.GetChild(i).GetComponent<DoorMono>();
                if (mono == null)
                {
                    mono = doorParent.GetChild(i).gameObject.AddComponent<DoorMono>();
                }

                mono.SetData(roomData.DoorList[i]);
                doorDic.Add(roomData.DoorList[i], mono);
            }

            doorInited = true;
        }
    }

    private void SetUpCamCollider()
    {
        Collider camCollider = GameRoot.Instance.camCollider;
        float roomDataRoomX = roomData.RoomX;
        float roomDataRoomY = roomData.RoomY + 3;
        float camSizeZ = 0f;
// 计算摄像机的Y轴大小
        if (roomDataRoomY > 8)
        {
            camSizeZ = (roomDataRoomY - 3) / 2f;
        }
        else
        {
            camSizeZ = 0;
        }
        

// 计算摄像机的X轴大小，根据房间X的大小来调整
        float camSizeX = 0f;
        if (roomDataRoomX < 12)
        {
            camSizeX = 0f; // 小于12米，摄像机的X轴大小为0
        }
        else if (roomDataRoomX >= 12 && roomDataRoomX < 18)
        {
            camSizeX = (roomDataRoomX - 12) / 2f; // 大于等于12米且小于18米，按该公式计算
        }
        else
        {
            camSizeX = roomDataRoomX / 2f + 3f; // 大于等于18米，按该公式计算
        }

// 如果Collider是BoxCollider，直接设置其大小
        if (camCollider is BoxCollider boxCollider)
        {
            boxCollider.size = new Vector3(camSizeX, boxCollider.size.y, camSizeZ);
        }
    }

    public void SetPlayPoint(int doorId)
    {
        playerPoint = doorDic[doorId].GetPlayerPoint();
        GameControl.Instance.GetGamePlayer().transform.position = playerPoint.position;
    }
}