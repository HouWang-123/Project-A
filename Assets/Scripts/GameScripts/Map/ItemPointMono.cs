using cfg.item;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public class ItemPointMono : MonoBehaviour
{
    [Header("生成的物品")]
    public List<ItemTip> ItemTips;
    [Header("生成的次数，即生成几个物品")]
    public int Number = 1;
    [Header("是否直接生成")]
    [Tooltip("true为直接生成，false为需要交互后生成")]
    public bool IsInFlood = true;

    private SphereCollider colliderThis;

    private void Awake()
    {
        colliderThis = GetComponent<SphereCollider>();
        colliderThis.enabled = !IsInFlood;
    }

    private void Start()
    {
        if(IsInFlood)
        {
            CreateItems();
        }
    }


    private void CreateItems()
    {
        int i = 0, j = 0;
        do
        {
            int num = Random.Range(0, 100);
            int tmp = 0;
            ItemTip tip = null;
            while(ItemTips.Count > 0 && tmp < num)
            {
                tip = ItemTips[j++];
                j %= ItemTips.Count;
                tmp += tip.Odds;
            }
            if(tip != null)
            {
                string objName = "";
                string ID = tip.ID.ToString();
                if(ID.StartsWith("23"))
                {
                    ThrowObjects table = GameTableDataAgent.ThrowObjectsTable.Get(tip.ID);
                    objName = table.PrefabName;
                }
                else if(ID.StartsWith("26"))
                {
                    var table = GameTableDataAgent.WeaponTable.Get(tip.ID);
                    objName = table.PrefabName;
                }
                //........
                if(YooAssets.CheckLocationValid(objName))
                {
                    AssetHandle handle = YooAssets.LoadAssetAsync(objName);
                    handle.Completed += (h) =>
                    {
                        GameObject obj = Instantiate(h.AssetObject, transform) as GameObject;
                        obj.name = objName;
                        obj.transform.position = transform.position;
                        obj.GetComponent<IStackable>()?.ChangeStackCount(tip.Number);
                    };
                }
            }
            i++;
        } while(i < Number);

    }
}

[System.Serializable]
public class ItemTip
{
    public int ID;
    [Header("生成的数量")]
    public int Number;
    [Range(0f, 100f)]
    [Header("生成的概率")]
    public int Odds;
}