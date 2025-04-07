using System;
using UnityEngine;
using cfg.scene;
using FEVM.Timmer;

public class DoorMono : MonoBehaviour, IInteractHandler
{
    public GameObject InteractTipPosition;
    [Header("通向的门ID")]
    public int ToDoorID;
    [Header("通向的RoomID")]
    public int ToRoomID;
    [Header("门的锁定状态")]
    [Tooltip("UnLock 未锁定;.....")]
    public EDoorLock DoorState;

    public bool playerinside;

    private Doors doorData;


    private void Start()
    {
        InteractTipPosition.SetActive(false);
    }

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
        if(other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Comming!!!!");
            EventManager.Instance.RunEvent("OnGameRoomChanges");
            playerinside = true;
                InteractTipPosition.SetActive(true);
            // 测试房间解密专用
            // SpawnRiddleTestRoom();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            playerinside = false;
            TimeMgr.Instance.RemoveTask(EnterDoor);
            InteractTipPosition.SetActive(false);
        }
    }

    public Transform GetPlayerPoint()
    {
        return transform;
    }

    private void SpawnRiddleTestRoom()
    {
        GameControl.Instance.ChangeRoom(300016, 310040);
    }

    private void EnterDoor()
    {
        GameControl.Instance.ChangeRoom(doorData.ToRoomID,doorData.ToDoorID);
    }

    public enum EDoorLock
    {
        UnLock = 1,

    }

    public void OnPlayerFocus()
    {
        if (playerinside)
        {
            InteractTipPosition.SetActive(true);
        }
    }

    public void OnPlayerDefocus()
    {
        InteractTipPosition.SetActive(false);
    }

    public MonoBehaviour getMonoBehaviour()
    {
        return this;
    }

    public void OnPlayerStartInteract()
    {
        if (playerinside)
        {
            TimeMgr.Instance.AddTask(0.3f,false,EnterDoor);
        }
    }

    public void OnPlayerInteract()
    {
        
    }

    public void OnPlayerInteractCancel()
    {
        TimeMgr.Instance.RemoveTask(EnterDoor);
    }
}
