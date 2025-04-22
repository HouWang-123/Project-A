using System;
using cfg.func;
using FEVM.Timmer;
using UnityEngine;

public class InspectionBehaviour : MonoBehaviour, IInteractHandler
{
    public int InspectionID;
    private float InteractRequiredTime;
    public Inspection InspectionData;
    public SpriteRenderer MyRenderer;

    public Shader DefaultShader;
    public Shader SelectedShader;

    private void Awake()
    {
        InspectionData = GameTableDataAgent.InspectionTable.Get(InspectionID);
        InteractRequiredTime = InspectionData.InteracteTime;
        MyRenderer = gameObject.transform.GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
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

    public void OnPlayerFocus()                      // 获得交互焦点
    {
        OnInspetionSelect();
    }

    public void OnPlayerDefocus()                    // 失去交互焦点
    {
        OnInspectionDeselect();
    }

    public MonoBehaviour getMonoBehaviour()          // 必须实现，否则功能无法使用
    {
        return this;
    }

    public void OnPlayerStartInteract()   // 交互开始
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
        
        
        // todo 打开检视UI界面
        
        
        EventManager.Instance.RunEvent(EventConstName.PlayerFinishInteraction);
    }
    public void OnPlayerInteractCancel()   // 交互取消
    {
        GameHUD.Instance.ResizeCursor(1f,0.2f);
        Debug.Log("=== PlayerCanceledInteract ===");
        TimeMgr.Instance.RemoveTask(OnPlayerInteract);
    }
}
