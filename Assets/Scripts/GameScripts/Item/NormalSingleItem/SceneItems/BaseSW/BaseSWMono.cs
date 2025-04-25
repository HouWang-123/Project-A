using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseSWMono : SceneObjects
{
    public bool IsOn;
    public List<ItemBase> RelationObject = new();
    public virtual void OnPlayerFocus()
    {
        SetTargerted(true);
    }

    public virtual void OnPlayerDefocus()
    {
        SetTargerted(false);
    }

    public override void SetTargerted(bool v)
    {
        PickUpTargeted = v;
        if (PickUpTargeted)
        {
            ItemRenderer.material.shader = oulineShader;
        }
        else
        {
            ItemRenderer.material.shader = DefaultSpriteShader;
        }
    }

    public virtual void ChangeState()
    {
        
    }
    public virtual MonoBehaviour getMonoBehaviour()
    {
        return null;
    }

    public void OnPlayerStartInteract()
    {
    }

    public virtual void OnPlayerInteract()
    {
        
    }

    public virtual void OnPlayerInteractCancel()
    {
        
    }
}