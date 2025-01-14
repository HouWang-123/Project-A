using System.Collections;
using UnityEngine;
using YooAsset;

public class GameRoot : MonoBehaviour
{
    [SerializeField]
    private GameObject gfDataTableObj;
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
    ResourcePackage package;

    private void Awake()
    {
        Instantiate(Resources.Load<GameObject>("LoadingPanel"), UIManager.Instance.transform);
        YooAssets.Initialize();
        package = YooAssets.CreatePackage("DefaultPackage");
        YooAssets.SetDefaultPackage(package);
        switch(PlayMode)
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
    private void Start()
    {
        CreateGFExtension();
    }
    private async void LoadDataTables()
    {
        var appConfig = await AppConfigs.GetInstanceSync();
        foreach (var item in appConfig.DataTables)
        {
            LoadDataTable(item);
        }
    }
    private void CreateGFExtension()
    {
        GF.Resource.LoadAsset(UtilityBuiltin.AssetsPath.GetPrefab("Core/GFExtension"), typeof(GameObject), new GameFramework.Resource.LoadAssetCallbacks(OnLoadGFExtensionSuccess));
    }
    private void OnLoadGFExtensionSuccess(string assetName, object asset, float duration, object userData)
    {
        var gfExtPfb = asset as GameObject;
        if (null != GameObject.Instantiate(gfExtPfb, Vector3.zero, Quaternion.identity, GF.Base.transform))
        {
            GF.LogInfo("GF框架扩展成功!");
            LoadDataTables();
        }
    }
    /// <summary>
    /// 加载数据表
    /// </summary>
    /// <param name="name"></param>
    private void LoadDataTable(string name)
    {
        GF.DataTable.LoadDataTable(name, this);
    }
    private IEnumerator EditorInitPackage()
    {
        var initParameters = new EditorSimulateModeParameters();
        var simulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.BuiltinBuildPipeline, "DefaultPackage");
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

        //Data
        //AssetHandle handle2 = YooAssets.LoadAssetSync<GameObject>("GameObject");
        //GameObject @object = Instantiate(handle2.AssetObject) as GameObject;
        UIManager.Instance.GetPanel("MainPanel");
        var data = GF.DataTable.GetDataTable<ver1>();
        UIManager.Instance.GetPanel("LoadingPanel").GetComponent<UIBase>().Hide();
    }
}
