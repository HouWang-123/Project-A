using cfg.scene;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
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
    [Header("脚本谜题，如果房间没有谜题就设置为空")]
    public RoomRiddleMonoBase riddleGameObject; // 谜题脚本

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


    public void SetDataAndGenerateItemAndEnemy(Rooms room)
    {
        GameControl.Instance.SetSceneItemList(SceneItemNode);
        roomData = room;
        SetUpCamCollider();
        
        bool generated = GameRunTimeData.Instance.MapTrackDataManager.RoomGenerated(room.ID); 
        // 场景中可追踪的游戏物体数据
        GameRunTimeData.Instance.MapTrackDataManager.OnRoomDataLoadComplete(room.ID);
        // 如果游戏存档生命周期生成过一次则不进行物品，怪物初始化工作
        
        
        
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
                DoorMono doorMono = doorParent.GetChild(i).GetComponent<DoorMono>();
                if(doorMono == null)
                {
                    doorMono = doorParent.GetChild(i).gameObject.AddComponent<DoorMono>();
                }

                doorMono.SetData(roomData.DoorList[i]);
                doorDic.Add(roomData.DoorList[i], doorMono);
            }
        }
        
        //生成物品的初始化
        if (!generated)
        {
            itemsParent = transform.Find("ItemList");
            if(itemsParent != null)
            {
                if(roomData.DropRuleIDList.Count != itemsParent.childCount || roomData.DropRuleIDList.Count != roomData.DropType.Count)
                {
                    Debug.LogWarning("room数据所含Drop数量与实际预制体的itemPoint不同，请检查问题！！！ID == " + roomData.ID + " name == " + roomData.PrefabName);
                }
                for(int i = 0; i < roomData.DropRuleIDList.Count && i < roomData.DropType.Count && i < itemsParent.childCount; i++)
                {
                    ItemPointMono itemPoint = itemsParent.GetChild(i).GetComponent<ItemPointMono>();
                    if(itemPoint == null)
                    {
                        itemPoint = itemsParent.GetChild(i).gameObject.AddComponent<ItemPointMono>();
                    }
                    itemPoint.SetData(roomData.DropRuleIDList[i], roomData.DropType[i]);
                }
            }
        }
        else
        {
            GameRunTimeData.Instance.MapTrackDataManager.RecoverItemPoint(room.ID);
        }

        
        
        //生成的怪物数据初始化
        monstersParent = transform.Find("Monsters");
        if(monstersParent != null)
        {
            // 挂载NavMesh Surface
            GameObject navGo = new("DynamicNavMesh");
            navGo.transform.parent = transform;
            // 创建
            var navMeshSurface = navGo.AddComponent<NavMeshSurface>();
            navMeshSurface.agentTypeID = 0;
            navMeshSurface.collectObjects = CollectObjects.Children;
            // 动态生成一个平台并加入导航网格
            GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platform.transform.position = new Vector3(0, 0, 0);
            platform.transform.localScale = new Vector3(roomData.RoomX, 0.2f, roomData.RoomY);
            platform.transform.parent = navMeshSurface.transform; // 设为NavMeshSurface的子物体
            navMeshSurface.BuildNavMesh();
            platform.SetActive(false);
            if (roomData.MonstersIDList.Count != monstersParent.childCount)
            {
                Debug.LogWarning("room数据所含Monsters数量与实际预制体的monsterPoint不同，请检查问题！！！ID == " + roomData.ID + " name == " + roomData.PrefabName);
            }
            for(int i = 0;i<roomData.MonstersIDList.Count && i < monstersParent.childCount; i++)
            {
                MonsterPointMono enemyMono = monstersParent.GetChild(i).GetComponent<MonsterPointMono>();
                if(enemyMono == null)
                {
                    enemyMono = monstersParent.GetChild(i).gameObject.AddComponent<MonsterPointMono>();
                }

                enemyMono.SetData(roomData.MonstersIDList[i]);
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


    [ContextMenu("ResizeDoor")]
    public void ResizeDoor()
    {
        //门的数据初始化
        doorParent = transform.Find("Door");
        if(doorParent != null)
        {
            for(int i = 0; i < doorParent.childCount; i++)
            {
                DoorMono doorMono = doorParent.GetChild(i).GetComponent<DoorMono>();
                doorMono.gameObject.transform.localScale = Vector3.one;
            }
        }
    }
    [ContextMenu("SetDoorLayer")]
    public void SetDoorLayer()
    {
        //门的数据初始化
        doorParent = transform.Find("Door");
        
        if(doorParent != null)
        {
            for(int i = 0; i < doorParent.childCount; i++)
            {
                Transform doorMono = doorParent.GetChild(i);
                doorMono.gameObject.layer = 11;
            }
        }
    }
    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.cyan;
        Handles.Label(transform.position + Vector3.up * 3.2f, roomData.NAME , style);
        GUIStyle style2 = new GUIStyle();
        style2.fontStyle = FontStyle.Bold;
        style2.normal.textColor = Color.white;
        Handles.Label(transform.position + Vector3.up * 3.4f, roomData.ID.ToString() , style2);
    }
}