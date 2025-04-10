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
    private bool targeted;
    private Doors doorData;
    public bool doorEnabled;
    private void Awake()
    {
        doorEnabled = true;
        InteractTipPosition = transform.Find("P_UI_WorldUI_InteractTip").gameObject;
    }

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

    private void OnDestroy()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!doorEnabled) return;
        if(other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Comming!!!!");
            EventManager.Instance.RunEvent("OnGameRoomChanges");
            playerinside = true;
            if (targeted)
            {
                InteractTipPosition.SetActive(true);
            }
            // 测试房间解密专用
            // SpawnRiddleTestRoom();
        }
    }

    private void FixedUpdate()
    {
        if (targeted && playerinside)
        {
            InteractTipPosition.SetActive(true);
        }
        else
        {
            InteractTipPosition.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            doorEnabled = true;
            playerinside = false;
            TimeMgr.Instance.RemoveTask(EnterDoor);
            InteractTipPosition.SetActive(false);
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

    private void EnterDoor()
    {
        EventManager.Instance.RunEvent<IInteractHandler>(EventConstName.OnInteractiveDestory,this);
        GameControl.Instance.ChangeRoom(doorData.ToRoomID,doorData.ToDoorID);
    }

    public enum EDoorLock
    {
        UnLock = 1,
    }

    public void OnPlayerFocus()
    {
        targeted = true;
        if (!doorEnabled) return;
        if (playerinside)
        {
            InteractTipPosition.SetActive(true);
        }

    }

    public void OnPlayerDefocus()
    {
        targeted = false;
        if (!doorEnabled) return;
        if (InteractTipPosition == null)
        {
            return;
        }
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
            EnterDoor();
        }
    }

    public void OnPlayerInteract()
    {
        
    }

    public void OnPlayerInteractCancel()
    {

    }
}
