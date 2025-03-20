using System;
using System.Collections.Generic;
using cfg.func;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerInteractController : MonoBehaviour
{
    [FormerlySerializedAs("InspectionList")] public List<IInteractHandler> InteractHandlerList;
    public LayerMask InteractiveLayer;
    public IInteractHandler CurrentFocusedInteractHandler;
    
    public void FixedUpdate()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        bool hit = Physics.Raycast(ray.origin, ray.direction, out var hitinfo, 20000, InteractiveLayer);
        if (hit)
        {
            Debug.Log(hitinfo.transform.name);
            IInteractHandler interactHandler = hitinfo.transform.GetComponentInChildren<IInteractHandler>();
            if (interactHandler == null) return;
            if (interactHandler == CurrentFocusedInteractHandler) return;
            EventManager.Instance.RunEvent(EventConstName.OnMouseFocusItemChanges, interactHandler);
            
            if (InteractHandlerList.Contains(interactHandler))
            {
                ChangeToTargetItem(interactHandler);
            }
        }
        else
        {
            EventManager.Instance.RunEvent<ItemBase>(EventConstName.OnMouseFocusItemChanges, null);
        }
    }

    private void ChangeToTargetItem(IInteractHandler interactHandler)
    {
        foreach (var handler in InteractHandlerList)
        {
            handler.OnPlayerDefocus();
        }
        CurrentFocusedInteractHandler = interactHandler;
        interactHandler.OnPlayerDefocus();
    }
    public void InteractItem()
    {
        CurrentFocusedInteractHandler.OnPlayerInteract();
    }
    
    public void OnTriggerEnter(Collider other)
    {
        IInteractHandler InteractHandler = other.gameObject.GetComponent<IInteractHandler>();
        if (InteractHandler != null)
        {
            InteractHandlerList.Add(InteractHandler);
            CurrentFocusedInteractHandler = InteractHandler;
            InteractHandler.OnPlayerFocus();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        IInteractHandler InteractHandler = other.gameObject.GetComponent<IInteractHandler>();
        if (InteractHandler != null)
        {
            
            InteractHandlerList.Contains(InteractHandler);
            InteractHandler.OnPlayerDefocus();
            InteractHandlerList.Remove(InteractHandler);
        }
    }
}
