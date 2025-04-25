using System;
using UnityEngine;


public class HideAreaMono : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerControl>(out PlayerControl _))
        {
            GameRunTimeData.Instance.CharacterExtendedStatManager.GetStat().IsHidding = true;
            Debug.Log(GetType() + "OnTriggerEnter() => 玩家进入躲藏区域。。。");
        }
    }

    // 如果另一个碰撞器停止接触触发器，则调用 OnTriggerExit
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerControl>(out PlayerControl _))
        {
            GameRunTimeData.Instance.CharacterExtendedStatManager.GetStat().IsHidding = false;
            Debug.Log(GetType() + "OnTriggerEnter() => 玩家离开躲藏区域。。。");
        }
    }
    public GameObject GetGO()
    {
        return gameObject;
    }
}
