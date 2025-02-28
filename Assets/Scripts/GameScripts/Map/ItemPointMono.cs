using System;
using cfg.item;
using cfg.func;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
using cfg;
using Random = UnityEngine.Random;

public class ItemPointMono : MonoBehaviour
{
    //[Header("生成的物品")]
    //public List<ItemTip> ItemTips;
    //[Header("生成的次数，即生成几个物品")]
    //public int Number = 1;
    [Header("是否直接生成")]
    [Tooltip("false为直接生成，true为需要交互后生成")]
    private bool IsInFlood = false;

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
            transform.GetChild(0).gameObject.SetActive(true);
        }
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
            GameTool.GenerateItemAtTransform(itemIDs[i], transform);
        }
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