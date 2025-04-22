using System;
using cfg.item;
using cfg.func;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
using cfg;
using Random = UnityEngine.Random;
public class ItemPointStatus : TrackableBaseData
{
    public bool Interact2Create;
    public bool Created;
    public List<int> itemIDs = new List<int>();
    public int interactSprite;
}

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
    private ItemPointStatus _itemPointStatus;


    private void Awake()
    {
        colliderThis = GetComponent<SphereCollider>();
        colliderThis.enabled = IsInFlood;
    }

    private void Start()
    {

    }

    private void OnDestroy()
    {
        UnRegisterTracker();
    }

    public void SetData(int ID, bool isInFlood)
    {
        _itemPointStatus = new ItemPointStatus();
        string idStr = ID.ToString();
        if(idStr.StartsWith("91"))
        {
            DropRule drop = GameTableDataAgent.DropRuleTable.Get(ID);
            _itemPointStatus.itemIDs = drop.GetID();
        }
        else
        {
            _itemPointStatus.itemIDs.Add(ID);
        }
        IsInFlood = isInFlood;
        
        _itemPointStatus.Interact2Create = isInFlood;
        _itemPointStatus.Created = false;

        _trackerData = new TrackerData(
            ID,
            TrackType.ItemPoint,
            transform.position,
            transform.eulerAngles,
            transform.localScale,
            _itemPointStatus);
        
        if(!_itemPointStatus.Interact2Create)
        {
            if (_itemPointStatus.Created) return;
            CreateItems();
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            RegisterTracker();
            transform.GetChild(0).gameObject.SetActive(true);
        }
        
    }
    public void SetData(TrackerData trackerData)
    {
        _itemPointStatus = trackerData.TrackableBaseData as ItemPointStatus;
        _trackerData = new TrackerData(
            trackerData.ID,
            TrackType.ItemPoint,
            transform.position,
            transform.eulerAngles,
            transform.localScale,
            _itemPointStatus);
    }

    private void CreateItems()
    {
        for(int i = 0; i < _itemPointStatus.itemIDs.Count; i++)
        {
            GameItemTool.GenerateItemAtPosition(_itemPointStatus.itemIDs[i], transform);
        }
        _itemPointStatus.Created = true;
        UnRegisterTracker();
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
        if (_itemPointStatus.Created) return;
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