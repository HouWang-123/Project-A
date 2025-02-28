using cfg.scene;
using System.Collections.Generic;
using UnityEngine;

public class RoomMono : MonoBehaviour
{
    private Rooms roomData;
    private Transform playerPoint;
    private GameObject SceneItemNode;

    private Transform doorParent;
    private Dictionary<int, DoorMono> doorDic = new Dictionary<int, DoorMono>();

    private Transform itemsParent;
    private List<ItemBase> items = new List<ItemBase>();

    private Transform monstersParent;
    private List<MonsterBaseFSM> monsters = new List<MonsterBaseFSM>();

    //public Transform monsterPoint;    // 怪物生成点（测试用）

    private void Awake()
    {
        SceneItemNode = new GameObject("ItemNode");
        SceneItemNode.transform.SetParent(transform);
        SceneItemNode.transform.localPosition = Vector3.zero;
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


    public void SetData(Rooms room)
    {
        GameControl.Instance.SetSceneItemList(SceneItemNode);
        roomData = room;
        SetUpCamCollider();
        //门的数据初始化
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

                mono.SetData(roomData.DoorList[i]);
                doorDic.Add(roomData.DoorList[i], mono);
            }
        }
        //生成物品的初始化
        itemsParent = transform.Find("ItemList");
        if(itemsParent != null)
        {
            if(roomData.DropRuleIDList.Count != itemsParent.childCount && roomData.DropRuleIDList.Count != roomData.DropType.Count)
            {
                Debug.LogWarning("room数据所含Drop数量与实际预制体的itemPoint不同，请检查问题！！！");
            }
            for(int i = 0; i < roomData.DropRuleIDList.Count && i < roomData.DropType.Count && i < itemsParent.childCount; i++)
            {
                ItemPointMono mono = itemsParent.GetChild(i).GetComponent<ItemPointMono>();
                if(mono == null)
                {
                    mono = itemsParent.GetChild(i).gameObject.AddComponent<ItemPointMono>();
                }

                mono.SetData(roomData.DropRuleIDList[i], roomData.DropType[i]);
            }
        }
        //生成的怪物数据初始化
        monstersParent = transform.Find("Monsters");
        if(monstersParent != null)
        {
            if(roomData.MonstersIDList.Count != monstersParent.childCount)
            {
                Debug.LogWarning("room数据所含Monsters数量与实际预制体的monsterPoint不同，请检查问题！！！");
            }
            for(int i = 0;i<roomData.MonstersIDList.Count && i < monstersParent.childCount; i++)
            {
                MonsterPointMono mono = monstersParent.GetChild(i).GetComponent<MonsterPointMono>();
                if(mono == null)
                {
                    mono = monstersParent.GetChild(i).gameObject.AddComponent<MonsterPointMono>();
                }

                mono.SetData(roomData.MonstersIDList[i]);
            }
        }

    }

    private void SetUpCamCollider()
    {
        Collider camCollider = GameRoot.Instance.camCollider;
        float roomDataRoomX = roomData.RoomX;
        float roomDataRoomY = roomData.RoomY + 3;
        float camSizeZ = 0f;
        // 计算摄像机的Y轴大小
        if(roomDataRoomY > 8)
        {
            camSizeZ = (roomDataRoomY - 3) / 2f;
        }
        else
        {
            camSizeZ = 0;
        }


        // 计算摄像机的X轴大小，根据房间X的大小来调整
        float camSizeX = 0f;
        if(roomDataRoomX < 12)
        {
            camSizeX = 0f; // 小于12米，摄像机的X轴大小为0
        }
        else if(roomDataRoomX >= 12 && roomDataRoomX < 18)
        {
            camSizeX = (roomDataRoomX - 12) / 2f; // 大于等于12米且小于18米，按该公式计算
        }
        else
        {
            camSizeX = roomDataRoomX / 2f + 3f; // 大于等于18米，按该公式计算
        }

        // 如果Collider是BoxCollider，直接设置其大小
        if(camCollider is BoxCollider boxCollider)
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