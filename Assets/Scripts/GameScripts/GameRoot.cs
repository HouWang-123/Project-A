using System;
using System.Collections;
using System.Collections.Generic;
using cfg;
using cfg.item;
using SimpleJSON;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using YooAsset;

public class GameRoot : MonoBehaviour
{
    [HideInInspector] public static GameRoot Instance;
    public Collider camCollider;
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
    ResourcePackage package;
    public LayerMask FloorLayer;
    
    [LabelText("传送ID")]
    public int TPRoomID;


    [Button("点击传送")]
    private void TeleportPlayer()
    {
        GameControl.Instance.TeleportPlayer(TPRoomID);
    }
    
    
    private void Awake()
    {
        Instance = this;
        Instantiate(Resources.Load<GameObject>("LoadingPanel"), UIManager.Instance.transform);
        YooAssets.Initialize();
        package = YooAssets.CreatePackage("DefaultPackage");
        YooAssets.SetDefaultPackage(package);
        
        switch (PlayMode)
        {
            case EPlayMode.EditorSimulateMode:
                StartCoroutine(EditorInitPackage());
                break;
            case EPlayMode.OfflinePlayMode:
                StartCoroutine(OfflineInitPackage());
                break;
            case EPlayMode.HostPlayMode:
                break;
            case EPlayMode.WebPlayMode:
                break;
        }
        // 默认语言
        LocalizationTool.Instance.InitLocal(Localizations.Zh);
    }

    private IEnumerator EditorInitPackage()
    {
        var initParameters = new EditorSimulateModeParameters();
        var simulateManifestFilePath =
            EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.BuiltinBuildPipeline, "DefaultPackage");
        initParameters.SimulateManifestFilePath = simulateManifestFilePath;
        yield return package.InitializeAsync(initParameters);
        yield return null;
        GameStart();
    }

    private IEnumerator OfflineInitPackage()
    {
        var initParameters = new OfflinePlayModeParameters();
        yield return package.InitializeAsync(initParameters);
        yield return null;
        GameStart();
    }


    private void GameStart()
    {
        GameRunTimeData.Instance.CharacterItemSlotData.SetMaxSlotCount(GameConstData.DEFAULT_SLOTCOUNT);
        
        //Data
        GameTableDataAgent.LoadAllTable();
        
        Invoke("ReadTableTest", 1f);
        
        UIManager.Instance.GetPanel(UIPanelConfig.MainPanel);
        UIManager.Instance.GetPanel(UIPanelConfig.LoadingPanel).GetComponent<UIBase>().Hide();
    }
    
}
