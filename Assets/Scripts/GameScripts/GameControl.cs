using UnityEngine;
using YooAsset;
using cfg.scene;

public class GameControl
{
    public readonly static GameControl Instance;
    private GameObject sceneItemNode;            //场景物品节点


    public void SetSceneItemList(GameObject gameObject)
    {
        sceneItemNode = gameObject;
    }
    public GameObject GetSceneItemList()
    {
        return sceneItemNode;
    }

    private GameObject playerObj;

    private Rooms room;
    private GameObject roomObj;

    static GameControl()
    {
        Instance = new GameControl();
    }
    private GameControl() { }


    public void GameStart()
    {
        room = GameTableDataAgent.RoomsTable.Get(300006);
        AssetHandle handle = YooAssets.LoadAssetSync<GameObject>(room.PrefabName);
        roomObj = Object.Instantiate(handle.AssetObject) as GameObject;
        RoomMono mono = roomObj.GetComponent<RoomMono>();
        if(mono == null)
        {
            mono = roomObj.AddComponent<RoomMono>();
        }
        mono.SetData(room);
    }

    public void ChangeRoom(int RoomId, int DoorId)
    {
        Rooms r = GameTableDataAgent.RoomsTable.Get(RoomId);
        if(!YooAssets.CheckLocationValid(r.PrefabName))
        {
            return;
        }
        AssetHandle handle = YooAssets.LoadAssetSync<GameObject>(r.PrefabName);
        GameObject o = Object.Instantiate(handle.AssetObject) as GameObject;
        RoomMono mono = o.GetComponent<RoomMono>();
        if(mono == null)
        {
            mono = o.AddComponent<RoomMono>();
        }
        mono.SetData(r);
        mono.SetPlayPoint(DoorId);
        roomObj.SetActive(false);
        Object.Destroy(roomObj);
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
        }
        return playerObj;
    }

}
