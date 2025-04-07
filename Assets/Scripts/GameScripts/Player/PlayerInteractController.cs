using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = System.Random;

public class PlayerInteractController : MonoBehaviour
{
    private List<IInteractHandler> InteractHandlerList = new ();
    public LayerMask InteractiveLayer;
    public IInteractHandler CurrentFocusedInteractHandler;
    public bool interactLock;

    private void Start()
    {
        EventManager.Instance.RegistEvent(EventConstName.PlayerFinishInteraction, PlayerCancleInteract);
        EventManager.Instance.RegistEvent<IInteractHandler>(EventConstName.OnInteractiveDestory, ClearInteractHandler);
    }

    public void FixedUpdate()
    {
        Debug.Log(InteractHandlerList.Count);
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        bool hit = Physics.Raycast(ray.origin, ray.direction, out var hitinfo, 20000, InteractiveLayer);
        if (hit)
        {
            IInteractHandler interactHandler = hitinfo.transform.GetComponentInChildren<IInteractHandler>();
            if (interactHandler == null) return;
            if (interactHandler == CurrentFocusedInteractHandler) return;
            if (InteractHandlerList.Contains(interactHandler))
            {
                ChangeToTargetItem(interactHandler);
            }
        }
    }

    private void ChangeToTargetItem(IInteractHandler interactHandler)
    {
        InteractHandlerList.Remove(InteractHandlerList.Find(x => x.getMonoBehaviour() == null));
        foreach (var handler in InteractHandlerList)
        {
            handler.OnPlayerDefocus();
        }
        CurrentFocusedInteractHandler = interactHandler;
        interactHandler.OnPlayerFocus();
    }

    private void ClearInteractHandler(IInteractHandler handler)
    {
        InteractHandlerList.Remove(handler);
    }
    public void InteractItem()
    {
        if (interactLock) return; //交互锁
        interactLock = true;
        if (CurrentFocusedInteractHandler != null)
        {
            CurrentFocusedInteractHandler.OnPlayerStartInteract();
        }
        
    }
    public void PlayerCancleInteract()
    {
        if (!interactLock) return;
        if (CurrentFocusedInteractHandler != null)
        {
            CurrentFocusedInteractHandler.OnPlayerInteractCancel();
        }
        interactLock = false;
    }
    public void OnTriggerEnter(Collider other)
    {
        IInteractHandler InteractHandler = other.gameObject.GetComponent<IInteractHandler>();
        if (InteractHandler != null)
        {
            InteractHandlerList.Add(InteractHandler);
            ChangeToTargetItem(InteractHandler);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        IInteractHandler InteractHandler = other.gameObject.GetComponent<IInteractHandler>();
        if (InteractHandler != null)
        {
            InteractHandlerList.Contains(InteractHandler);
            InteractHandler.OnPlayerDefocus();
            if (interactLock)
            {
                InteractHandler.OnPlayerInteractCancel();
                interactLock = false;
            }
            InteractHandlerList.Remove(InteractHandler);
            if (InteractHandlerList.Count > 0)
            {
                int count = InteractHandlerList.Count - 1;
                Random r = new Random();
                int newIndex = r.Next(0,count);
                ChangeToTargetItem(InteractHandlerList[newIndex]);
            }
        }
    }
}
