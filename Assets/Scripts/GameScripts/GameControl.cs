using UnityEngine;
using YooAsset;
using cfg.scene;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine.LightTransport.PostProcessing;
using UnityEngine.Rendering.Universal;

public class GameControl
{
    public readonly static GameControl Instance;

    public readonly static int DEFAULTROOM = 300007;
    //data
    private Rooms room;

    public Rooms GetRoomData()
    {
        return room;
    }
    private CinemachineCamera mainCam;
    //mono
    private GameObject sceneItemNode;            //场景物品节点
    private GameObject playerObj;
    private GameObject roomObj;                  // 当前房间Go
    
    public  PlayerControl PlayerControl;
    private Dictionary<int, List<GameObject>> roomWithMonsterList; // 房间对应的怪物
    private GameObject roomList;

    static GameControl()
    {
        Instance = new GameControl();
    }
    private GameControl() { }

    public void SetSceneItemList(GameObject gameObject)
    {
        sceneItemNode = gameObject;
    }
    public GameObject GetSceneItemList()
    {
        return sceneItemNode;
    }

    public void GameStart()
    {
        //roomCache = new Dictionary<int, GameObject>();
        roomList = new GameObject();
        roomList.name = "===RoomList===";
                
        room = GameTableDataAgent.RoomsTable.Get(DEFAULTROOM); // 初始房间
        GameHUD.Instance.OnAreaNotificaiton(room.NAME);
        AssetHandle handle = YooAssets.LoadAssetSync<GameObject>(room.PrefabName);
        mainCam = GameObject.Find("CinemachineCamera").GetComponent<CinemachineCamera>();
        roomObj = Object.Instantiate(handle.AssetObject) as GameObject;
        RoomMono mono = roomObj.GetComponent<RoomMono>();
        if(mono == null)
        {
            mono = roomObj.AddComponent<RoomMono>();
        }
        mono.SetDataAndGenerateItemAndEnemy(room);
        mono.transform.SetParent(roomList.transform);
        //roomCache.Add(300006,roomObj);

        // 添加全局光
        CreateGlobalLight();
    }

    public void ChangeRoom(int RoomId, int DoorId)
    {
        RoomMono mono = InitRoom(RoomId);
        if (mono == null) return;
        
        GameRunTimeData.Instance.MapTrackDataManager.RecoverItem(room.ID,sceneItemNode.transform);
        EventManager.Instance.RunEvent(EventConstName.OnChangeRoom);
        mono.SetPlayPoint(DoorId);
    }

    private RoomMono InitRoom(int RoomId)
    {
        (GameObject,Rooms) roomTurple = GetRoomObject(RoomId);
        GameObject RoomObject = roomTurple.Item1;
        Rooms roomData = roomTurple.Item2;
        if(RoomObject == null || roomData == null ) return null;
        RoomMono mono = RoomObject.GetComponent<RoomMono>();
        if(mono == null)
        {
            mono = RoomObject.AddComponent<RoomMono>();
        }
        mono.SetDataAndGenerateItemAndEnemy(roomData);
        RiddleByRoom(mono);
        Object.Destroy(roomObj);
        room = roomData;
        roomObj = RoomObject;
        return mono;
    }
    private (GameObject,Rooms) GetRoomObject(int RoomId)
    {
        Rooms roomData = GameTableDataAgent.RoomsTable.Get(RoomId);
        if (roomData == null)
        {
            Debug.LogWarning("场景数据" + RoomId + "为空");
            return (null,null);
        }
        if(!YooAssets.CheckLocationValid(roomData.PrefabName))
        {
            Debug.LogWarning("没有对应的预制体 " + roomData.PrefabName);
            return (null,null);
        }
        GameRunTimeData.Instance.MapTrackDataManager.SaveTrackerData(room.ID);
        GameHUD.Instance.OnAreaNotificaiton(roomData.NAME);
        GameObject RoomObject;
        AssetHandle handle = YooAssets.LoadAssetSync<GameObject>(roomData.PrefabName);
        RoomObject = Object.Instantiate(handle.AssetObject) as GameObject;
        RoomObject.transform.SetParent(roomList.transform);
        return (RoomObject,roomData);
    }
    
    // 根据房间设置谜题
    private void RiddleByRoom(RoomMono roomMono)
    {
        var riddleObj = roomMono.riddleGameObject;
        if (riddleObj != null)
        {
            riddleObj.SetRiddle();
        }
    }
    
    public void GameSave()
    {

    }

    public void GameOver()
    {

    }

    public void GamePause()
    {

    }

    // 传入 PlayerID 控制角色切换
    public GameObject GetGamePlayer(int PlayerId = 0)
    {
        if(playerObj == null)
        {
            
            // 角色数据设置
            GameRunTimeData.Instance.CharacterBasicStat.InitCharacter(PlayerId);
            
            
            AssetHandle handle = YooAssets.LoadAssetSync<GameObject>("Player000");
            playerObj = Object.Instantiate(handle.AssetObject) as GameObject;
            playerObj.name = "Player000";
            mainCam.Follow = playerObj.transform;
            mainCam.LookAt = playerObj.transform;
        }
        PlayerControl = playerObj.GetComponent<PlayerControl>();
        return playerObj;
        
    }

    private void CreateGlobalLight()
    {
        GameObject globalLight2D = new() { name = "GlobalLight_2D" };
        globalLight2D.AddComponent<Light2D>();
        globalLight2D.AddComponent<GlobalLightController>();
        //Object.DontDestroyOnLoad(globalLight2D);
    }


    // 怪物测试代码
    public List<GameObject> GetGameMonsterList(int roomId)
    {
        if (roomWithMonsterList == null)
        {
            var monsterTable = GameTableDataAgent.MonsterTable;
            int monsterCount = monsterTable.DataList.Count; // 怪物数量，根据房间对应的怪物数量调整
            roomWithMonsterList = new();
            List<GameObject> monsterList = new();
            for (int j = 0; j < monsterCount; ++j)
            {
                AssetHandle handle = YooAssets.LoadAssetSync<GameObject>(monsterTable.DataList[j].PrefabName);
                var monsterObj = Object.Instantiate(handle.AssetObject) as GameObject;
                monsterObj.name = monsterTable.DataList[j].PrefabName;
                monsterList.Add(monsterObj);
            }
            roomWithMonsterList.Add(roomId, monsterList);
        }
        return roomWithMonsterList[roomId];
    }
    public GameObject GetGameMonster(int roomId, int i)
    {
        if (roomWithMonsterList == null)
        {
            var monsterTable = GameTableDataAgent.MonsterTable;
            int monsterCount = monsterTable.DataList.Count; // 怪物数量，根据房间对应的怪物数量调整
            roomWithMonsterList = new();
            List<GameObject> monsterList = new();
            for (int j = 0; j < monsterCount; ++j)
            {
                AssetHandle handle = YooAssets.LoadAssetSync<GameObject>(monsterTable.DataList[j].PrefabName);
                var monsterObj = Object.Instantiate(handle.AssetObject) as GameObject;
                monsterObj.name = monsterTable.DataList[j].PrefabName;
                monsterList.Add(monsterObj);
            }
            roomWithMonsterList.Add(roomId, monsterList);
        }
        return roomWithMonsterList[roomId][i];
    }
    
    
    public void TeleportPlayer(int RoomId)
    {
        RoomMono mono = InitRoom(RoomId);
        GameRunTimeData.Instance.MapTrackDataManager.RecoverItem(room.ID,sceneItemNode.transform);
        EventManager.Instance.RunEvent(EventConstName.OnChangeRoom);
        playerObj.transform.position = Vector3.zero;
    }
}
