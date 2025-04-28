using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using YooAsset;

public class GameRoot : MonoBehaviour
{
     public static GameRoot Instance;
    public Collider camCollider;
    [GUIColor(0,1,1)]
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
    
    ResourcePackage package;
    
    public LayerMask FloorLayer;
    
    [HorizontalGroup("Columns")]
    [GUIColor(1f, 0.5f, 1f)]
    [BoxGroup("Columns/传送", showLabel: true)]
    [LabelText("传送ID")]

    public int TPRoomID;

    [BoxGroup("Columns/传送")]
    [Button("点击传送")]
    [GUIColor(1f, 0.5f, 1f)]
    private void TeleportPlayer()
    {
        GameControl.Instance.TeleportPlayer(TPRoomID);
    }

    [HorizontalGroup("Columns")]
    
    [BoxGroup("Columns/物品", showLabel: true)]
    [LabelText("物品ID")]
    [GUIColor(1f, 1f, 0)]
    public int GiveMeItemId;

    [BoxGroup("Columns/物品")]
    [GUIColor(1f, 1f, 0)]
    [Button("获取物品")]
    public void GiveMeItem()
    {
        GameControl.Instance.GivePlayerItem(GiveMeItemId);
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
        
        InitInteractions();
        
        UIManager.Instance.GetPanel(UIPanelConfig.MainPanel);
        UIManager.Instance.GetPanel(UIPanelConfig.LoadingPanel).GetComponent<UIBase>().Hide();
    }

    private void InitInteractions()
    {
        foreach (var effect in GameTableDataAgent.InteractEffectTable.DataList)
        {
            GameItemInteractionHub.InitInteraction(effect);
        }
    }
}
