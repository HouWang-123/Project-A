using UnityEngine;
using YooAsset;

public class GameControl
{
    public readonly static GameControl Instance;
    private GameObject SceneItemNode;            //场景物品节点

    public void SetSceneItemList(GameObject gameObject)
    {
        SceneItemNode = gameObject;
    }
    public GameObject GetSceneItemList()
    {
        return SceneItemNode;
    }
    
    private GameObject playerObj;

    private GameObject mapObj;

    static GameControl()
    {
        Instance = new GameControl();
    }
    private GameControl() { }


    public void GameStart()
    {
        AssetHandle handle = YooAssets.LoadAssetSync<GameObject>("Map0000");
        mapObj = Object.Instantiate(handle.AssetObject) as GameObject;

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
