using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class RiddleItemBase : MonoBehaviour, IInteractHandler,IRiddleItem
{
    public List<ItemBase> RelationObject;
    public RiddleManager RiddleManager;
    public GameInteractTip myInteractTip;
    public virtual void OnPlayerFocus()
    {
        if (myInteractTip == null)
        {
            return;
        }
        myInteractTip.PlayInitAnimation();
    }
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
        return null;
    }
    public virtual void OnPlayerStartInteract()
    {
        OnPlayerInteract();
    }
    public abstract void OnPlayerInteract();
    public virtual void OnPlayerInteractCancel()
    {
    }


    public int RiddleItemKey;
    public int GetRiddleKey()
    {
        return RiddleItemKey;
    }

    public void SetRiddleKey(int key)
    {
        RiddleItemKey = key;
    }

    public void Start()
    {
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

    public void SetRiddleManager(RiddleManager manager)
    {
        RiddleManager = manager;
    }
}