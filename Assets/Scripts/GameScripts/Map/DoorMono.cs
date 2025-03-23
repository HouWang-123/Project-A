using System;
using UnityEngine;
using cfg.scene;

public class DoorMono : MonoBehaviour
{
    [Header("通向的门ID")]
    public int ToDoorID;
    [Header("通向的RoomID")]
    public int ToRoomID;
    [Header("门的锁定状态")]
    [Tooltip("UnLock 未锁定;.....")]
    public EDoorLock DoorState;


    private Doors doorData;
    
    private bool doorEnabled = true;

    public void SetData(int v)
    {
        SetData(GameTableDataAgent.DoorsTable.Get(v));
    }

    public void SetData(Doors data)
    {
        doorData = data;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(doorEnabled && other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Comming!!!!");
            GameControl.Instance.ChangeRoom(doorData.ToRoomID,doorData.ToDoorID);
            // 测试房间解密专用
            // SpawnRiddleTestRoom();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            doorEnabled = true;
        }
    }

    public Transform GetPlayerPoint()
    {
        doorEnabled = false;
        return transform;
    }

    private void SpawnRiddleTestRoom()
    {
        GameControl.Instance.ChangeRoom(300016, 310040);
    }


    public enum EDoorLock
    {
        UnLock = 1,

    }
}
