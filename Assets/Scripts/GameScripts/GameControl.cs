﻿using UnityEngine;
using YooAsset;
using cfg.scene;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine.Rendering.Universal;

public class GameControl
{
    public readonly static GameControl Instance;

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
    //private Dictionary<int, GameObject> roomCache;
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
                
        room = GameTableDataAgent.RoomsTable.Get(300006); // 初始房间
        GameHUD.Instance.OnAreaNotificaiton(room.NAME);
        AssetHandle handle = YooAssets.LoadAssetSync<GameObject>(room.PrefabName);
        mainCam = GameObject.Find("CinemachineCamera").GetComponent<CinemachineCamera>();
        roomObj = Object.Instantiate(handle.AssetObject) as GameObject;
        RoomMono mono = roomObj.GetComponent<RoomMono>();
        if(mono == null)
        {
            mono = roomObj.AddComponent<RoomMono>();
        }
        mono.SetData(room);
        mono.transform.SetParent(roomList.transform);
        //roomCache.Add(300006,roomObj);

        // 添加全局光
        CreateGlobalLight();
    }

    public void ChangeRoom(int RoomId, int DoorId)
    {
        Rooms r = GameTableDataAgent.RoomsTable.Get(RoomId);
        GameHUD.Instance.OnAreaNotificaiton(r.NAME);
        if(!YooAssets.CheckLocationValid(r.PrefabName))
        {
            return;
        }
        GameObject o;
        //if (!roomCache.ContainsKey(RoomId))
        //{
            AssetHandle handle = YooAssets.LoadAssetSync<GameObject>(r.PrefabName);
            o = Object.Instantiate(handle.AssetObject) as GameObject;
            o.transform.SetParent(roomList.transform);
        //}
        //else
        //{
        //    o = roomCache[RoomId];
        //    o.SetActive(true);
        //}
        
        RoomMono mono = o.GetComponent<RoomMono>();
        if(mono == null)
        {
            mono = o.AddComponent<RoomMono>();
        }
        mono.SetData(r);
        mono.SetPlayPoint(DoorId);

        // 不为安全屋，时间系统不走
        switch (r.PrefabName)
        {
            case "SafetyRoom": // 安全屋
                TimeSystemManager.Instance.TimeSpeed = 1f;
                EventManager.Instance.RunEvent(EventConstName.PlayerEnterSafeHouseEvent);
                break;
            default:
                TimeSystemManager.Instance.TimeSpeed = 0f;
                EventManager.Instance.RunEvent(EventConstName.PlayerLeaveSafeHouseEvent);
                break;
        }
        RiddleByRoom(mono, r);

        //if(!roomCache.ContainsKey(RoomId))
        //{
        //    roomCache.Add(RoomId,o); // 加载的房间进入缓存
        //}

        roomObj.SetActive(false);
        Object.Destroy(roomObj);
        room = r;
        roomObj = o;

        // 房间切换后，检查怪物生成
        //if (mono.monsterList != null)
        //{
        //    foreach (var monster in GetGameMonsterList(r.ID))
        //    {
        //        monster.transform.SetParent(mono.monsterList.transform);
        //        monster.transform.position = mono.monsterList.transform.position;
        //    }
        //}
        
        EventManager.Instance.RunEvent(EventConstName.OnChangeRoom);
    }

    // 根据房间设置谜题
    private void RiddleByRoom(RoomMono roomMono, Rooms roomTableData)
    {
        switch (roomTableData.PrefabName)
        {
            case "Corridor1":
                var corridor1RiddleMono = roomMono.riddleGameObject;
                if (corridor1RiddleMono == null)
                {
                    corridor1RiddleMono = roomMono.gameObject.AddComponent<Corridor1RiddleMono>();
                    roomMono.riddleGameObject = corridor1RiddleMono;
                }
                corridor1RiddleMono.DoRiddle();
                break;
            default:
                
                break;
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
            //if (roomCache.ContainsKey(roomId))
            {
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
            //else
            //{
            //    Debug.LogError(GetType() + " /GetGameMonsterList => 没有找到对应的房间！");
            //}
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
            //if (roomCache.ContainsKey(roomId))
            {
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
            //else
            //{
            //    Debug.LogError(GetType() + " /GetGameMonsterList => 没有找到对应的房间！");
            //}
        }
        return roomWithMonsterList[roomId][i];
    }
}
