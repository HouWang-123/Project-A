using System;
using cfg.func;
using UnityEngine;

public class InspectionBehaviour : MonoBehaviour, IInteractHandler
{
    public int InspectionID;
    public Inspection InspectionData;
    public SpriteRenderer MyRenderer;

    public Shader DefaultShader;
    public Shader SelectedShader;
    
    private void Start()
    {
        InspectionData = GameTableDataAgent.InspectionTable.Get(InspectionID);
        MyRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void OnInspetionSelect()
    {
        MyRenderer.material.shader = SelectedShader;
    }

    public void OnInspectionDeselect()
    {
        MyRenderer.material.shader = DefaultShader;
    }

    public void OnPlayerFocus()
    {
        OnInspetionSelect();
    }

    public void OnPlayerDefocus()
    {
        OnInspectionDeselect();
    }

    public void OnPlayerInteract()
    {
        
    }
}
