using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class RiddleItemBase : MonoBehaviour, IInteractableItemReceiver,IRiddleItem
{
    public int itemId;
    public cfg.item.SceneObjects SceneObjectsData;
    public RiddleManager RiddleManager;
    public GameInteractTip myInteractTip;
    public string RiddleItemKey;
    public virtual void OnPlayerFocus() { }

    public virtual void OnPlayerDefocus()
    {
        if (myInteractTip == null)
        {
            return;
        } 
        myInteractTip.OnDetargeted();
    }

    public virtual MonoBehaviour getMonoBehaviour()
    {
        return this;
    }

    public virtual void OnPlayerStartInteract()
    {
        OnPlayerInteract();
    }
    public abstract void OnPlayerInteract();
    public virtual void OnPlayerInteractCancel() { }

    public string GetRiddleKey()
    {
        return RiddleItemKey;
    }

    public void SetRiddleKey(string key)
    {
        RiddleItemKey = key;
    }
    public void Start()
    {
        
        SceneObjectsData = GameTableDataAgent.SceneObjectsTable.Get(itemId);
        if (myInteractTip == null)
        {
            Debug.LogWarning(name+"缺少交互提示");
        }
        else
        {
            myInteractTip.transform.localScale = Vector3.zero;
        }
    }
    public abstract RiddleItemBaseStatus GetRiddleStatus();
    public abstract void SetRiddleItemStatus(RiddleItemBaseStatus BaseStatus);
    public void SetRiddleManager(RiddleManager manager) { RiddleManager = manager; }

    public virtual bool hasInteraction(int itemid)
    {
        return GameItemInteractionHub.HasInteract(itemid, itemId);
    }
    public abstract void OnPlayerStartInteract(int itemid);
    public abstract bool GetRiddleItemResult();
    public virtual void OnPlayerFocus(int itemid)
    {
        if (hasInteraction(itemid))
        {
            if (myInteractTip == null)
            {
                return;
            }
            myInteractTip.PlayInitAnimation();
        }
    }
}