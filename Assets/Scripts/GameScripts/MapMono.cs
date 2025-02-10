using UnityEngine;

public class MapMono : MonoBehaviour
{
    public Transform PlayerPoint;
    public GameObject SceneItemNode; // 场景道具物品节点
    private void Awake()
    {
        
    }

    private void Start()
    {
        GameControl.Instance.GetGamePlayer().transform.position = PlayerPoint.position;
        GameControl.Instance.SetSceneItemList(SceneItemNode);
    }

}
