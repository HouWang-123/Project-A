using System;
using System.Diagnostics;
using FEVM.Timmer;
using UnityEngine;

public class CaveMono : RiddleItemBase,IInteractHandler
{
    public GameInteractTip InteractTipPosition;
    private SwitchStatus isOpen;
    [Header("通向的RoomID")]
    public int ToRoomID;
    private bool playerinside;
    private bool targeted;
    private void Awake()
    {
        InteractTipPosition = transform.Find("P_UI_WorldUI_InteractTip").gameObject.GetComponent<GameInteractTip>();
    }

    private void Start()
    {
        InteractTipPosition.gameObject.transform.localScale = Vector3.zero;
        if (isOpen == null)
        {
            isOpen = new SwitchStatus(false);
        }

        if (isOpen.is_on)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public override RiddleItemBaseStatus GetRiddleStatus()
    {
        return isOpen;
    }

    public override void SetRiddleItemStatus(RiddleItemBaseStatus BaseStatus)
    {
        isOpen = BaseStatus as SwitchStatus;
    }

    public void OpenCave()
    {
        isOpen.is_on = true;
        gameObject.SetActive(true);
        RiddleManager.OnRiddleItemStatusChange(this);
    }
    public override void OnPlayerStartInteract(int itemid)
    {
        OnPlayerStartInteract();
    }

    public override bool GetRiddleItemResult()
    {
        return isOpen.is_on;
    }


    private void OnTriggerEnter(Collider other)
    {
        playerinside = true;
    }
    
    private void OnTriggerExit(Collider other)
    {
        playerinside = false;
    }
    

    private void Enter()
    {
        EventManager.Instance.RunEvent<IInteractHandler>(EventConstName.OnInteractiveDestory,this);
        GameControl.Instance.ChangeRoom(ToRoomID);
        TimeMgr.Instance.AddTask(0.02f,false,()=>GameControl.Instance.PlayerDropFromSky());
    }
    

    public void OnPlayerFocus()
    {
        targeted = true;
        if (playerinside)
        {
            InteractTipPosition.PlayInitAnimation();
        }

    }

    public void OnPlayerDefocus()
    {
        targeted = false;
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
        if (playerinside)
        {
            Enter();
        }
    }

    public override void OnPlayerInteract()
    {
    }

    public void OnPlayerInteractCancel()
    {

    }
    public void UpdateInteractHandler(ItemBase updated)
    {
    }
}
