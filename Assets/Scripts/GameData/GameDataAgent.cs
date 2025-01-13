using System;
using System.Collections.Generic;
using cfg;
using SimpleJSON;
using UnityEngine;
using YooAsset;
/// <summary>
///  Update: 修改类为 static
/// </summary>
public static class GameDataAgent
{
    public static Tables tb;
    public static void LoadAllTable()
    {
        Tables tables = new cfg.Tables(LoadByteBuf);
        tb = tables;
        ColorfulDebugger.Debug(" All Game Data loaded", ColorfulDebugger.Instance.Data);
    }
    /// <summary>
    /// 通过类型获取数据表
    /// update: 添加泛型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static T GetTableByType<T>() where T: ITable
    {
        return (T)tb.ITables[typeof(T)];
    }
    /// <summary>
    /// 通过传入Type和Id得到数据
    /// update: 添加泛型
    /// </summary>
    /// <param name="type"></param>
    /// <param name="Id"></param>
    /// <returns></returns>
    public static T GetTableDataById<T>(int Id) where T: IData
    {
        return (T)tb.ITables[typeof(T)].FindById(Id);
    }
    private static JSONNode LoadByteBuf(string file)
    {
        AssetHandle loadAssetSync = YooAssets.LoadAssetSync<TextAsset>(file);
        TextAsset assetObject = loadAssetSync.GetAssetObject<TextAsset>();
        return JSON.Parse(assetObject.text);
    }
}
/// <summary>
/// 万用数据（可以组合任意类型数据）
/// 不推荐使用，这样会极大增加维护成本
/// 相当于json中的可塞入任何数据的对象
/// 不到万不得已千万不要使用，不然出现问题非常难排查
/// </summary>
public class DataPackageBuilder
{
    private DataPackage dp = new ();
    public void BuildData(IData data)
    {
        dp.AddData(data);
    }
    public DataPackage Build()
    {
        return dp;
    }
    public class DataPackage
    {
        public List<Dictionary<Type, IData>> Datas = new();
        public List<IData> GetDataByType(Type t)
        {
            List<IData> datas = new List<IData>();
            foreach (var d in Datas)
            {
                if (d.ContainsKey(t)) { datas.Add(d[t]); }
            }
            return datas;
        }
        public void AddData(IData data)
        {
            Type t = data.GetType();
            foreach (var VARIABLE in Datas)
            {
                if (VARIABLE.ContainsKey(t))
                {
                    continue;
                }
                else
                {
                    VARIABLE.Add(t,data);
                    return;
                }
            }
            Dictionary<Type, IData> AddOne = new Dictionary<Type, IData>();
            AddOne.Add(t,data);
            Datas.Add(AddOne);
        }
    }
}

