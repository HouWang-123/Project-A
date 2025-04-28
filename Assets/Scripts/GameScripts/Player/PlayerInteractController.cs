using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
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
        EventManager.Instance.RegistEvent(EventConstName.OnPlayerHandItemChanges_Item, OnPlayerItemChanges);
    }

    private void OnPlayerItemChanges()
    {
        ChangeToTargetItem(CurrentFocusedInteractHandler);
    }
    public void FixedUpdate()
    {
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

    public void UpdateInteractController(ItemBase itemBase,IInteractHandler old)
    {
        InteractHandlerList.Remove(old);
        if (itemBase is IInteractHandler)
        {
            if (itemBase.gameObject.GetComponent<IInteractHandler>() is { } interactHandler)
            {
                InteractHandlerList.Add(interactHandler);
                ChangeToTargetItem(interactHandler);
            }
        }
    }
    public void UpdateInteractController(ItemBase itemBase)
    {
        if (itemBase is IInteractHandler)
        {
            if (itemBase.gameObject.GetComponent<IInteractHandler>() is { } interactHandler)
            {
                InteractHandlerList.Add(interactHandler);
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
        if (CurrentFocusedInteractHandler == null) return;
        if (CurrentFocusedInteractHandler is IInteractableItemReceiver receiver)
        {
            int itemonhandid = GetOnHandItem();
            if (receiver.hasInteraction(itemonhandid))
            {
                receiver.OnPlayerFocus(itemonhandid);
            }
        }
        CurrentFocusedInteractHandler = interactHandler;
        CurrentFocusedInteractHandler.OnPlayerFocus();
    }

    private void ClearInteractHandler(IInteractHandler handler)
    {
        if (CurrentFocusedInteractHandler == handler)
        {
            CurrentFocusedInteractHandler = null;
        }
        InteractHandlerList.Remove(handler);
        
    }
    public void InteractItem()
    {
        if (interactLock) return; //交互锁
        interactLock = true;
        if (CurrentFocusedInteractHandler == null)        // 是存在可交互的对象
        {
            return;
        }
        if (CurrentFocusedInteractHandler is IInteractableItemReceiver receiver) // 需要玩家持有物品进行的特殊交互
        {
            // 获取手中物品ID
            int InteractItemId = GetOnHandItem();
            // 查找交互表
            bool hasInteraction = receiver.hasInteraction(InteractItemId);
            
            if (hasInteraction)
            {
                if (InteractItemId == -1)
                {
                    receiver.OnPlayerStartInteract(-1);
                }
                else // 物品交互
                {
                    receiver.OnPlayerStartInteract(InteractItemId);
                }
            }
        }
        else                                                                 // 不需要物品即可直接交互
        {
            Debug.Log("不需要物品交互");
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
        if (other.gameObject.GetComponent<IInteractHandler>() is { } interactHandler)
        {
            if (interactHandler == CurrentFocusedInteractHandler) return;
            InteractHandlerList.Add(interactHandler);
            ChangeToTargetItem(interactHandler);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        IInteractHandler interactHandler = other.gameObject.GetComponent<IInteractHandler>();
        if (interactHandler != null)
        {
            interactHandler.OnPlayerDefocus();
            if (interactLock)
            {
                interactHandler.OnPlayerInteractCancel();
                interactLock = false;
            }
            InteractHandlerList.Remove(InteractHandlerList.Find(x => x.getMonoBehaviour() == null));
            InteractHandlerList.Remove(interactHandler);
            if (InteractHandlerList.Count > 0)
            {
                int count = InteractHandlerList.Count - 1;
                Random r = new Random();
                int newIndex = r.Next(0,count);
                ChangeToTargetItem(InteractHandlerList[newIndex]);
            }
            else
            {
                CurrentFocusedInteractHandler = null;
            }
        }
    }

    private int GetOnHandItem()
    {
        int interactItemId = -1;
        if (GameRunTimeData.Instance.characterBasicStatManager.GetStat().ItemOnHand != null)
        {
            interactItemId = GameRunTimeData.Instance.characterBasicStatManager.GetStat().ItemOnHand.ItemID;
        }
        if (GameRunTimeData.Instance.characterBasicStatManager.GetStat().LiftedItem != null)
        {
            interactItemId = GameRunTimeData.Instance.characterBasicStatManager.GetStat().LiftedItem.ItemID;
        }
        return interactItemId;
    }
}
