using System;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using YooAsset;
using Vector3 = UnityEngine.Vector3;

public static class GameTool
{
    private static string GetPrefabName(int id)
    {
        string uri = "";
        if (id.ToString().StartsWith("20")) //FOOD
        {
            var weapon = GameTableDataAgent.FoodTable.Get(id);
            uri = weapon.PrefabName;
        }
        if (id.ToString().StartsWith("22")) //SceneObject
        {
            var weapon = GameTableDataAgent.SceneObjectsTable.Get(id);
            uri = weapon.PrefabName;
        }
        if (id.ToString().StartsWith("23")) //ThrowObject
        {
            var weapon = GameTableDataAgent.ThrowObjectsTable.Get(id);
            uri = weapon.PrefabName;
        }
        if (id.ToString().StartsWith("24")) //TinyObject
        {
            var weapon = GameTableDataAgent.TinyObjectsTable.Get(id);
            uri = weapon.PrefabName;
        }
        if (id.ToString().StartsWith("25")) //Tool
        {
            var weapon = GameTableDataAgent.ToolsTable.Get(id);
            uri = weapon.PrefabName;
        }
        if (id.ToString().StartsWith("26")) //Weapon
        {
            var weapon = GameTableDataAgent.WeaponTable.Get(id);
            uri = weapon.PrefabName;
        }
        return uri;
    }
    public static void GenerateItemAtTransform(int id, Transform parent, Action<ItemBase> res = null)
    {
        string uri = GetPrefabName(id);
        if (uri != "")
        {
            ItemBase ib;
            if (YooAssets.CheckLocationValid(uri))
            {
                AssetHandle loadAssetAsync = YooAssets.LoadAssetAsync<GameObject>(uri);
                loadAssetAsync.Completed += handle =>
                {
                    GameObject instantiate = GameObject.Instantiate(loadAssetAsync.AssetObject, parent) as GameObject;
                    instantiate.transform.SetParent(GameControl.Instance.GetSceneItemList().transform);
                    ib = instantiate.GetComponent<ItemBase>(); ib.OnItemDrop(false); ib.SetItemId(id); res?.Invoke(ib);
                };
            }
            else
            {
                Debug.LogWarning("预制体 " + uri + " 没有找到");
            }
        }
    }
    public static void GenerateItemAtTransform(int id, Vector3 worldPos, Action<ItemBase> res = null)
    {
        string uri = GetPrefabName(id);
        if (uri != "")
        {
            ItemBase ib;
            if (YooAssets.CheckLocationValid(uri))
            {
                AssetHandle loadAssetAsync = YooAssets.LoadAssetAsync<GameObject>(uri);
                loadAssetAsync.Completed += handle =>
                {
                    GameObject instantiate = GameObject.Instantiate(loadAssetAsync.AssetObject) as GameObject;
                    instantiate.transform.SetParent(GameControl.Instance.GetSceneItemList().transform);
                    instantiate.transform.position = worldPos; ib = instantiate.GetComponent<ItemBase>();
                    ib.OnItemDrop(false); ib.SetItemId(id); res?.Invoke(ib);
                };
            }
            else
            {
                Debug.LogWarning("预制体 " + uri + " 没有找到");
            }
        }
    }
}