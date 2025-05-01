using System;
using UnityEngine;
using cfg.scene;
using FEVM.Timmer;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DoorMono : MonoBehaviour, IInteractHandler
{
    public GameInteractTip InteractTipPosition;
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
        InteractTipPosition = transform.Find("P_UI_WorldUI_InteractTip").gameObject.GetComponent<GameInteractTip>();
    }

    private void Start()
    {
        InteractTipPosition.gameObject.transform.localScale = Vector3.zero;
    }

    public void SetData(int v)
    {
        SetData(GameTableDataAgent.DoorsTable.Get(v));
    }

    public void SetData(Doors data)
    {
        doorData = data;

    }

    private void OnDrawGizmos()
    {
        if(doorData ==null) return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position,0.2f);
        Gizmos.color = Color.white;
        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
#if UNITY_EDITOR

        Handles.Label(transform.position + Vector3.up * 2f, doorData.ID.ToString() , style);
#endif
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
                InteractTipPosition.gameObject.SetActive(true);
                InteractTipPosition.PlayInitAnimation();
            }
            // 测试房间解密专用
            // SpawnRiddleTestRoom();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            doorEnabled = true;
            playerinside = false;
            TimeMgr.Instance.RemoveTask(EnterDoor);
            InteractTipPosition.OnDetargeted();
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
            InteractTipPosition.PlayInitAnimation();
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
        InteractTipPosition.OnDetargeted();

    }

    public MonoBehaviour getMonoBehaviour()
    {
        return this;
    }

    public void OnPlayerStartInteract()
    {
        if (!doorEnabled) return;
        if (GameRunTimeData.Instance.gameState.IsPlayerInRiddle)
        {
            return;
        }
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

    public void UpdateInteractHandler(ItemBase updated)
    {
    }
}
