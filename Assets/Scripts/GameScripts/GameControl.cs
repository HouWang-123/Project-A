using UnityEngine;
using YooAsset;
using cfg.scene;
using System.Collections.Generic;

public class GameControl
{
    public readonly static GameControl Instance;

    //data
    private Rooms room;

    //mono
    private GameObject sceneItemNode;            //场景物品节点
    private GameObject playerObj;
    private GameObject roomObj;                  // 当前房间Go
    private Dictionary<int, GameObject> roomCache;

    private List<GameObject> monsterList;
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
        roomCache = new Dictionary<int, GameObject>();
        roomList = new GameObject();
        roomList.name = "===RoomList===";
        
        room = GameTableDataAgent.RoomsTable.Get(300006); // 初始房间
        AssetHandle handle = YooAssets.LoadAssetSync<GameObject>(room.PrefabName);
        roomObj = Object.Instantiate(handle.AssetObject) as GameObject;
        RoomMono mono = roomObj.GetComponent<RoomMono>();
        if(mono == null)
        {
            mono = roomObj.AddComponent<RoomMono>();
        }
        mono.SetData(room);
        mono.transform.SetParent(roomList.transform);
        roomCache.Add(300006,roomObj);
    }

    public void ChangeRoom(int RoomId, int DoorId)
    {
        Rooms r = GameTableDataAgent.RoomsTable.Get(RoomId);
        if(!YooAssets.CheckLocationValid(r.PrefabName))
        {
            return;
        }
        GameObject o;
        if (!roomCache.ContainsKey(RoomId))
        {
            AssetHandle handle = YooAssets.LoadAssetSync<GameObject>(r.PrefabName);
            o = Object.Instantiate(handle.AssetObject) as GameObject;
            o.transform.SetParent(roomList.transform);
        }
        else
        {
            o = roomCache[RoomId];
            o.SetActive(true);
        }
        
        RoomMono mono = o.GetComponent<RoomMono>();
        if(mono == null)
        {
            mono = o.AddComponent<RoomMono>();
        }
        mono.SetData(r);
        mono.SetPlayPoint(DoorId);
        
        
        if(!roomCache.ContainsKey(RoomId))
        {
            roomCache.Add(RoomId,o); // 加载的房间进入缓存
        }
        
        roomObj.SetActive(false);
        //Object.Destroy(roomObj);
        room = r;
        roomObj = o;
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


    public GameObject GetGamePlayer()
    {
        if(playerObj == null)
        {
            AssetHandle handle = YooAssets.LoadAssetSync<GameObject>("Player000");
            playerObj = Object.Instantiate(handle.AssetObject) as GameObject;
            playerObj.name = "Player000";
        }
        return playerObj;
    }
    // 怪物测试代码
    public GameObject GetGameMonster(int i)
    {
        int monsterCount = 1; // 怪物数量，根据数据表调整
        if (monsterList == null)
        {
            monsterList = new ();
            for (int j = 0; j < monsterCount; ++j)
            {
                AssetHandle handle = YooAssets.LoadAssetSync<GameObject>("Monster" + j.ToString("D3"));
                var monsterObj = Object.Instantiate(handle.AssetObject) as GameObject;
                monsterObj.name = "Monster" + j.ToString("D3");
                monsterList.Add(monsterObj);
            }
        }
        return monsterList[i];
    }
}
