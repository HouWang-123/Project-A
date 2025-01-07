using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public class UIManager : MonoBehaviour
{
    //单例
    public static UIManager Instance;
    //建立一个集合，将UIBehaviour放入集合中
    //二维数组
    Dictionary<string, Dictionary<string, GameObject>> allWedgate;
    //将子控件放入集合中
    public void RegistGameObject(string PanleName,string wedgateName,GameObject obj)
    {
        if (!allWedgate.ContainsKey(PanleName))
        {
            allWedgate[PanleName] = new Dictionary<string, GameObject>();
        }
        //将wedgateName放入数组中
        allWedgate[PanleName].Add(wedgateName, obj);
    }

    public GameObject GetPanel(string panelName)
    {
        if(allWedgate.ContainsKey(panelName))
        {
            return allWedgate[panelName][panelName];
        }
        else
        {
            AssetHandle handle = YooAssets.LoadAssetSync<GameObject>(panelName);
            return Instantiate(handle.AssetObject, transform) as GameObject;
        }
    }

    //获取某一个Panle下面的子控件
    public GameObject GetGameObject(string panleName, string wedgateName)
    {
        if (allWedgate.ContainsKey(panleName))
        {
            return allWedgate[panleName][wedgateName];
            Debug.Log("执行");
        }
        return null;
    }
    public void UnRegistGameObject(string panelName, string widegaName)
    {   //将子控件从内存中删除掉
        if (allWedgate.ContainsKey(panelName))
        {
            if (allWedgate[panelName].ContainsKey(widegaName))
            {
                allWedgate[panelName].Remove(widegaName);
            }
        }
    }
    //将panel从内存中删除
    public void UnRegistPanel(string panelNmae)
    {
        if (allWedgate.ContainsKey(panelNmae))
        {
            allWedgate[panelNmae].Clear();
            allWedgate[panelNmae] = null;
        }
    }

    void Awake()
    {
        Instance = this;
        allWedgate = new Dictionary<string, Dictionary<string, GameObject>>();
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {

    }
    void Update()
    {

    }
}

