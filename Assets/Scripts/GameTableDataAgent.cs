using System;
using System.Collections.Generic;
using cfg;
using cfg.buff;
using cfg.cha;
using cfg.interact;
using cfg.item;
using cfg.skill;
using cfg.scene;
using SimpleJSON;
using UnityEngine;
using YooAsset;

/// <summary>
///  Update: 修改类为 static
/// </summary>
public class GameTableDataAgent
{
    private static Tables _Instance; // 所有表的集合

    // in case of CLASSNAME CHANGES, introduce the table alas variable, and bridge it to real table instance
    //public static RewardTable RewardTable;
    public static TinyObjectsTable TinyObjectsTable;
    public static WeaponTable WeaponTable;
    public static FoodTable FoodTable;
    public static ThrowObjectsTable ThrowObjectsTable;
    public static SceneObjectsTable SceneObjectsTable;
    public static ToolsTable ToolsTable;
    public static JewelryTable JewelryTable;
    public static CharacterTable CharacterTable;
    public static ConditionsTable ConditionsTable;
    public static PassiveSkillsTable PassiveSkillsTable;
    public static ActiveSkillsTable ActiveSkillsTable;
    public static InteractEffectTable InteractEffectTable;
    public static RoomsTable RoomsTable;
    public static DoorsTable DoorsTable;


    public static void LoadAllTable()
    {
        Tables tables = new(LoadByteBuf);
        _Instance = tables;

        // bridge to shorter table usage, in case of CLASSNAME CHANGES
        //RewardTable = _Instance.RewardTable;
        TinyObjectsTable = _Instance.TinyObjectsTable;
        WeaponTable = _Instance.WeaponTable;
        FoodTable = _Instance.FoodTable;
        ThrowObjectsTable = _Instance.ThrowObjectsTable;
        SceneObjectsTable = _Instance.SceneObjectsTable;
        ToolsTable = _Instance.ToolsTable;
        JewelryTable = _Instance.JewelryTable;
        CharacterTable = _Instance.CharacterTable;
        ConditionsTable = _Instance.ConditionsTable;
        PassiveSkillsTable = _Instance.PassiveSkillsTable;
        ActiveSkillsTable = _Instance.ActiveSkillsTable;
        InteractEffectTable = _Instance.InteractEffectTable;
        RoomsTable = _Instance.RoomsTable;
        DoorsTable = _Instance.DoorsTable;

        ColorfulDebugger.Debug(" All Game Data loaded", ColorfulDebugger.Instance.Data);
    }

    private static JSONNode LoadByteBuf(string file)
    {
        AssetHandle loadAssetSync = YooAssets.LoadAssetSync<TextAsset>(file);
        TextAsset assetObject = loadAssetSync.GetAssetObject<TextAsset>();
        return JSON.Parse(assetObject.text);
    }
}