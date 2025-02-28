using cfg.mon;
using System;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public class MonsterPointMono : MonoBehaviour
{

    private int MonsterID;

    private Monster data;

    private void Start()
    {
        if (MonsterID != 0)
        {
            try
            {
                data = GameTableDataAgent.MonsterTable.Get(MonsterID);
                AssetHandle handle = YooAssets.LoadAssetAsync(data.PrefabName);
                handle.Completed += (obj) =>
                {
                    GameObject monster = Instantiate(obj.AssetObject, transform) as GameObject;
                    monster.name = data.PrefabName;
                    monster.transform.position = transform.position;
                    monster.transform.parent = transform;
                };
            }
            catch(System.Exception)
            {

            }
        }
    }

    public void SetData(int id)
    {
        MonsterID = id;
    }
}
