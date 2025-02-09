using System.Collections;
using System.Collections.Generic;
using cfg;
using cfg.item;
using SimpleJSON;
using UnityEngine;
using YooAsset;

public class GameRoot : MonoBehaviour
{
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
    ResourcePackage package;

    private void Awake()
    {
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
        ColorfulDebugger.Debug("LoadingData", ColorfulDebugger.Instance.Data);
        //Data
        GameTableDataAgent.LoadAllTable();
        
        Invoke("ReadTableTest", 1f);
        
        //AssetHandle handle2 = YooAssets.LoadAssetSync<GameObject>("GameObject");
        //GameObject @object = Instantiate(handle2.AssetObject) as GameObject;
        UIManager.Instance.GetPanel("MainPanel");
        UIManager.Instance.GetPanel("LoadingPanel").GetComponent<UIBase>().Hide();
    }

    private void ReadTableTest()
    {
        
        Reward tableDataById = GameTableDataAgent.RewardTable.Get(1001);
        
        
        List<Reward> tableDataByIdtable = GameTableDataAgent.RewardTable.DataList;
        
        ColorfulDebugger.Debug(tableDataById.ToString(),ColorfulDebugger.Instance.Data);
        
    }
}
