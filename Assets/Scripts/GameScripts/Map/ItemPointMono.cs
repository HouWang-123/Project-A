using System;
using cfg.item;
using cfg.func;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
using cfg;
using Random = UnityEngine.Random;

public class ItemPointMono : MonoBehaviour , IInteractHandler , ITrackable
{
    //[Header("生成的物品")]
    //public List<ItemTip> ItemTips;
    //[Header("生成的次数，即生成几个物品")]
    //public int Number = 1;
    [Header("是否直接生成")]
    [Tooltip("false为直接生成，true为需要交互后生成")]
    private bool IsInFlood = false;

    private TrackerData _trackerData;
    private SphereCollider colliderThis;

    private List<int> itemIDs = new List<int>();

    private void Awake()
    {
        colliderThis = GetComponent<SphereCollider>();
        colliderThis.enabled = IsInFlood;
    }

    private void Start()
    {
        if(!IsInFlood)
        {
            CreateItems();
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            RegisterTracker();
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        UnRegisterTracker();
    }

    public void SetData(int ID, bool isInFlood)
    {
        string idStr = ID.ToString();
        if(idStr.StartsWith("91"))
        {
            DropRule drop = GameTableDataAgent.DropRuleTable.Get(ID);
            itemIDs = drop.GetID();
        }
        else
        {
            itemIDs.Add(ID);
        }
        IsInFlood = isInFlood;
    }


    private void CreateItems()
    {
        for(int i = 0; i < itemIDs.Count; i++)
        {
            GameItemTool.GenerateItemAtTransform(itemIDs[i], transform);
        }
    }
    // 交互操作
    public void OnPlayerFocus()
    {

    }

    public void OnPlayerDefocus()
    {

    }

    public MonoBehaviour getMonoBehaviour()
    {
        return this;
    }

    public void OnPlayerStartInteract()
    {
        OnPlayerInteract();
    }

    public void OnPlayerInteract()
    {
        CreateItems();
    }

    public void OnPlayerInteractCancel()
    {
        
    }
    // 场景追踪
    public void RegisterTracker()
    {
        GameRunTimeData.Instance.MapTrackDataManager.RegisterTracker(this);
    }
    public void UnRegisterTracker()
    {
        GameRunTimeData.Instance.MapTrackDataManager.UnRegisterTracker(this);
    }

    public TrackerData CollectTrackedData()
    {
        return _trackerData;
    }
}

//[System.Serializable]
//public class ItemTip
//{
//    public int ID;
//    [Header("生成的数量")]
//    public int Number;
//    [Range(0f, 100f)]
//    [Header("生成的概率")]
//    public int Odds;
//}