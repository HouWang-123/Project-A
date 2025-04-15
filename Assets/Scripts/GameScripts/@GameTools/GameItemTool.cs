using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using YooAsset;
using Vector3 = UnityEngine.Vector3;

public static class GameItemTool
{
    private static Dictionary<int, ItemResourceData> dataCache = new Dictionary<int, ItemResourceData>();
    private static ItemResourceData GetItemResourceData(int id)
    {
        if (dataCache.ContainsKey(id))
        {
            return dataCache[id];
        }
        ItemResourceData resourceData = new ItemResourceData();
        string uri = "";
        if (id.ToString().StartsWith("20")) //FOOD
        {
            var Item = GameTableDataAgent.FoodTable.Get(id);
            resourceData.PrefabName = Item.PrefabName;
            resourceData.IconName = Item.IconName;
            resourceData.RendererName = Item.SpriteName;
        }
        if (id.ToString().StartsWith("22")) //SceneObject
        {
            var Item = GameTableDataAgent.SceneObjectsTable.Get(id);
            resourceData.PrefabName = Item.PrefabName;
            resourceData.IconName = Item.IconName;
            resourceData.RendererName = Item.SpriteName;
        }
        if (id.ToString().StartsWith("23")) //ThrowObject
        {
            var Item = GameTableDataAgent.ThrowObjectsTable.Get(id);
            resourceData.PrefabName = Item.PrefabName;
            resourceData.IconName = Item.IconName;
            resourceData.RendererName = Item.SpriteName;
        }
        if (id.ToString().StartsWith("24")) //TinyObject
        {
            var Item = GameTableDataAgent.TinyObjectsTable.Get(id);
            resourceData.PrefabName = Item.PrefabName;
            resourceData.IconName = Item.IconName;
            resourceData.RendererName = Item.SpriteName;
        }
        if (id.ToString().StartsWith("25")) //Tool
        {
            var Item = GameTableDataAgent.ToolsTable.Get(id);
            resourceData.PrefabName = Item.PrefabName;
            resourceData.IconName = Item.IconName;
            resourceData.RendererName = Item.SpriteName;
        }
        if (id.ToString().StartsWith("26")) //Weapon
        {
            var Item = GameTableDataAgent.WeaponTable.Get(id);
            resourceData.PrefabName = Item.PrefabName;
            resourceData.IconName = Item.IconName;
            resourceData.RendererName = Item.SpriteName;
        }
        dataCache.Add(id,resourceData);
        return resourceData;
    }
    /// <summary>
    /// 物品生成
    /// </summary>
    /// <param name="id">物品ID</param>
    /// <param name="parent">父物体</param>
    /// <param name="ignoreAngleCorrect">是否忽略物品角度修正（45°朝向摄像机）</param>
    /// <param name="res">回调</param>
    public static void GenerateItemAtTransform(int id, Transform parent, bool ignoreAngleCorrect = false, Action<ItemBase> res = null)
    {
        string uri = GetItemResourceData(id).PrefabName;
        if (uri != "")
        {
            ItemBase ib;
            if (YooAssets.CheckLocationValid(uri))
            {
                AssetHandle loadAssetAsync = YooAssets.LoadAssetAsync<GameObject>(uri);
                loadAssetAsync.Completed += handle =>
                {
                    GameObject instantiate = GameObject.Instantiate(loadAssetAsync.AssetObject, parent) as GameObject;
                    ib = instantiate.GetComponent<ItemBase>();
                    ib.SetItemId(id); 
                    ib.SetIgnoreAngle(ignoreAngleCorrect);
                    res?.Invoke(ib);
                    loadAssetAsync.Release();
                };
            }
            else
            {
                Debug.LogWarning("预制体 " + uri + " 没有找到");
            }
        }
    }
    /// <summary>
    /// 物品生成
    /// </summary>
    /// <param name="id">物品ID</param>
    /// <param name="worldPos">世界坐标</param>
    /// <param name="ignoreAngleCorrect">是否忽略物品角度修正（45°朝向摄像机）</param>
    /// <param name="res">回调</param>
    public static void GenerateItemAtTransform(int id, Vector3 worldPos,bool ignoreAngleCorrect = false, Action<ItemBase> res = null)
    {
        string uri = GetItemResourceData(id).PrefabName;
        if (uri != "")
        {
            ItemBase ib;
            if (YooAssets.CheckLocationValid(uri))
            {
                AssetHandle loadAssetAsync = YooAssets.LoadAssetAsync<GameObject>(uri);
                loadAssetAsync.Completed += handle =>
                {
                    GameObject instantiate = GameObject.Instantiate(loadAssetAsync.AssetObject) as GameObject;
                    instantiate.transform.position = worldPos; ib = instantiate.GetComponent<ItemBase>();
                    ib.SetItemId(id); 
                    ib.SetIgnoreAngle(ignoreAngleCorrect);
                    res?.Invoke(ib);
                    loadAssetAsync.Release();
                };
            }
            else
            {
                Debug.LogWarning("预制体 " + uri + " 没有找到");
            }
        }
    }
    /// <summary>
    /// 生成可堆叠物品
    /// </summary>
    /// <param name="id">物品id</param>
    /// <param name="count">数量</param>
    /// <param name="parent">父物体</param>
    /// <param name="ignoreAngleCorrect">同上</param>
    /// <param name="res">回调</param>
    public static void GenerateStackableItemAtTransform(int id,int count, Transform parent,bool ignoreAngleCorrect = false, Action<ItemBase> res = null)
    {
        string uri = GetItemResourceData(id).PrefabName;
        if (uri != "")
        {
            ItemBase ib;
            if (YooAssets.CheckLocationValid(uri))
            {
                AssetHandle loadAssetAsync = YooAssets.LoadAssetAsync<GameObject>(uri);
                loadAssetAsync.Completed += handle =>
                {
                    GameObject instantiate = GameObject.Instantiate(loadAssetAsync.AssetObject, parent) as GameObject;
                    ib = instantiate.GetComponent<ItemBase>();
                    ib.SetItemId(id);
                    ib.SetIgnoreAngle(ignoreAngleCorrect);
                    IStackable stackable = ib as IStackable;
                    stackable.ChangeStackCount(count);
                    res?.Invoke(ib);
                    loadAssetAsync.Release();
                };
            }
            else
            {
                Debug.LogWarning("预制体 " + uri + " 没有找到");
            }
        }
    }
    /// <summary>
    /// 批量生成可堆叠物品
    /// </summary>
    /// <param name="id">物品id</param>
    /// <param name="count">数量</param>
    /// <param name="worldPos">世界坐标</param>
    /// <param name="ignoreAngleCorrect">是否忽略角度修正</param>
    /// <param name="res">回调</param>
    public static void GenerateStackableItemAtTransform(int id,int count, Vector3 worldPos, bool ignoreAngleCorrect = false, Action<ItemBase> res = null)
    {
        string uri = GetItemResourceData(id).PrefabName;
        if (uri != "")
        {
            ItemBase ib;
            if (YooAssets.CheckLocationValid(uri))
            {
                AssetHandle loadAssetAsync = YooAssets.LoadAssetAsync<GameObject>(uri);
                loadAssetAsync.Completed += handle =>
                {
                    GameObject instantiate = GameObject.Instantiate(loadAssetAsync.AssetObject) as GameObject;
                    instantiate.transform.position = worldPos;
                    ib = instantiate.GetComponent<ItemBase>();
                    ib.SetIgnoreAngle(ignoreAngleCorrect);
                    IStackable stackable = ib as IStackable;
                    stackable.ChangeStackCount(count);
                    ib.SetItemId(id);
                    res?.Invoke(ib);
                };
            }
            else
            {
                Debug.LogWarning("预制体 " + uri + " 没有找到");
            }
        }
    }
    /// <summary>
    /// 获取物品Icon的Sprite
    /// </summary>
    /// <param name="id"></param>
    /// <param name="finishCallback"></param>
    public static void GetItemIcon(int id, Action<Sprite> finishCallback)
    {
        string uri = GetItemResourceData(id).IconName;
        if (uri != "")
        {
            GameSpriteTool.LoadImageAsync(uri,finishCallback);
        }
    }
    /// <summary>
    /// 获取渲染器
    /// </summary>
    /// <param name="id"></param>
    /// <param name="finishCallback"></param>
    public static void GetItemRendererImage(int id, Action<Sprite> finishCallback)
    {
        string uri = GetItemResourceData(id).RendererName;
        if (uri != "")
        {
            GameSpriteTool.LoadImageAsync(uri,finishCallback);
        }
    }
}