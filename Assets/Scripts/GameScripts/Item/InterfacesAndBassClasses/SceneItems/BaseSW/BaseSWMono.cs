using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseSWMono : MonoBehaviour,IInteractHandler
{
    public bool IsOn;
    public List<ItemBase> RelationObject = new();
    public Transform ItemNode;

    private void Awake()
    {
        ItemNode = GameObject.Find("ItemNode").transform;
    }

    public virtual void OnPlayerFocus()
    {
        
    }

    public virtual void OnPlayerDefocus()
    {
        
    }

    public virtual MonoBehaviour getMonoBehaviour()
    {
        return null;
    }

    public virtual void OnPlayerStartInteract()
    {
        
    }

    public virtual void OnPlayerInteract()
    {
        
    }

    public virtual void OnPlayerInteractCancel()
    {
        
    }
}