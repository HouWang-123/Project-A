using UnityEngine;


public class HideAreaMono : MonoBehaviour, IRoomRiddleItem
{
    private bool isDone;
    // 如果另一个碰撞器进入了触发器，则调用 OnTriggerEnter
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerControl>(out PlayerControl _))
        {
            isDone = true;
            Debug.Log(GetType() + "OnTriggerEnter() => 玩家进入躲藏区域。。。");
        }
    }

    // 如果另一个碰撞器停止接触触发器，则调用 OnTriggerExit
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerControl>(out PlayerControl _))
        {
            isDone = false;
            Debug.Log(GetType() + "OnTriggerEnter() => 玩家离开躲藏区域。。。");
        }
    }

    public bool isItemDone()
    {
        return isDone;
    }

    public GameObject GetGO()
    {
        return gameObject;
    }
}
