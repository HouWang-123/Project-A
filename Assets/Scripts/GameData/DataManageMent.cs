using System;
using cfg;
using Luban;
using SimpleJSON;
using UnityEngine;
using YooAsset;

public class DataManagement
{
    public static Tables tb;
    public static void LoadAllTable()
    {
            Tables tables = new cfg.Tables(LoadByteBuf);
            tb = tables;
            ColorfulDebugger.Debug(" All Game Data loaded",ColorfulDebugger.Data);
    }
    
    public static ITable AdressTable(Type type)
    {
        return tb.ITables[type];
    }
    public static IData AdressTableDataById(Type type, int Id)
    {
        return tb.ITables[type].FindById(Id);
    }
    
    private static JSONNode LoadByteBuf(string file)
    {
        AssetHandle loadAssetSync = YooAssets.LoadAssetSync<TextAsset>(file);
        TextAsset assetObject = loadAssetSync.GetAssetObject<TextAsset>();
        return JSON.Parse(assetObject.text);
    }
}
