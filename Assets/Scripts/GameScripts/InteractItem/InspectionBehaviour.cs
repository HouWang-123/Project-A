using System;
using cfg.func;
using FEVM.Timmer;
using UnityEngine;

public class InspectionBehaviour : MonoBehaviour, IInteractHandler
{
    public int InspectionID;
    public float InteractRequiredTime;
    public Inspection InspectionData;
    public SpriteRenderer MyRenderer;

    public Shader DefaultShader;
    public Shader SelectedShader;

    private void Awake()
    {

    }

    private void Start()
    {
        InspectionData = GameTableDataAgent.InspectionTable.Get(InspectionID);
        MyRenderer = gameObject.transform.GetComponentInChildren<SpriteRenderer>();
        
        MyRenderer.transform.localEulerAngles = GameConstData.DefAngles;
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

    public MonoBehaviour getMonoBehaviour()
    {
        return this;
    }

    public void OnPlayerStartInteract(Action interactCallback)   // 交互开始
    {
        GameHUD.Instance.ResizeCursor(2f,InteractRequiredTime);
        Debug.Log("========InteractStart========");
        TimeMgr.Instance.AddTask(InteractRequiredTime, false, OnPlayerInteract);
    }

    public void OnPlayerInteract( )
    {
        GameHUD.Instance.ResizeCursor(1f,0.2f);
        Debug.Log("=======FinishInterAction=======");    // 交互完成
        Debug.Log(InspectionData.ToString());
        EventManager.Instance.RunEvent(EventConstName.PlayerFinishInteraction);
    }
    public void OnPlayerInteractCancel(Action cancleCallback)   // 交互取消
    {
        GameHUD.Instance.ResizeCursor(1f,0.2f);
        Debug.Log("=== PlayerCanceledInteract ===");
        cancleCallback?.Invoke();
        TimeMgr.Instance.RemoveTask(OnPlayerInteract);
    }
}
