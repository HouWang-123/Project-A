
    using UnityEngine;

public class CaveMono : MonoBehaviour,IInteractHandler
{
    public GameInteractTip InteractTipPosition;
    
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
        GameControl.Instance.PlayerDropFromSky();
    }

    public enum EDoorLock
    {
        UnLock = 1,
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
