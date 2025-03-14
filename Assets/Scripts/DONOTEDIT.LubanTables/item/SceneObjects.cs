
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using SimpleJSON;


namespace cfg.item
{
public sealed partial class SceneObjects : Luban.BeanBase
{
    public SceneObjects(JSONNode _buf) 
    {
        { if(!_buf["ID"].IsNumber) { throw new SerializationException(); }  ID = _buf["ID"]; }
        { if(!_buf["NAME"].IsNumber) { throw new SerializationException(); }  NAME = _buf["NAME"]; }
        { if(!_buf["DESCRIBE"].IsNumber) { throw new SerializationException(); }  DESCRIBE = _buf["DESCRIBE"]; }
        { if(!_buf["interactEffectID"].IsNumber) { throw new SerializationException(); }  InteractEffectID = _buf["interactEffectID"]; }
        { if(!_buf["isDestructible"].IsBoolean) { throw new SerializationException(); }  IsDestructible = _buf["isDestructible"]; }
        { if(!_buf["durability"].IsNumber) { throw new SerializationException(); }  Durability = _buf["durability"]; }
        { if(!_buf["buffID"].IsNumber) { throw new SerializationException(); }  BuffID = _buf["buffID"]; }
        { if(!_buf["buffTime"].IsNumber) { throw new SerializationException(); }  BuffTime = _buf["buffTime"]; }
        { if(!_buf["attack"].IsNumber) { throw new SerializationException(); }  Attack = _buf["attack"]; }
        { if(!_buf["IconName"].IsString) { throw new SerializationException(); }  IconName = _buf["IconName"]; }
        { if(!_buf["SpriteName"].IsString) { throw new SerializationException(); }  SpriteName = _buf["SpriteName"]; }
        { if(!_buf["PrefabName"].IsString) { throw new SerializationException(); }  PrefabName = _buf["PrefabName"]; }
    }

    public static SceneObjects DeserializeSceneObjects(JSONNode _buf)
    {
        return new item.SceneObjects(_buf);
    }

    /// <summary>
    /// 序号
    /// </summary>
    public readonly int ID;
    /// <summary>
    /// 名称
    /// </summary>
    public readonly int NAME;
    /// <summary>
    /// 描述
    /// </summary>
    public readonly int DESCRIBE;
    /// <summary>
    /// 玩家与其的交互是哪一种<br/>也就是说，在这个表里添加东西的话，要去func_InteractEffect.xlsx表里查询或添加交互显示
    /// </summary>
    public readonly int InteractEffectID;
    /// <summary>
    /// 是否可被武器攻击所破坏
    /// </summary>
    public readonly bool IsDestructible;
    /// <summary>
    /// 这里的攻击和耐久更像是一种可以被调用的数值
    /// </summary>
    public readonly int Durability;
    /// <summary>
    /// 提供哪一种buff（不一定是交互后，比如靠近某个物体一定范围后等）
    /// </summary>
    public readonly int BuffID;
    /// <summary>
    /// 赋予的buff有多长时间
    /// </summary>
    public readonly float BuffTime;
    /// <summary>
    /// 注意，更详细的功能效果并未配进该表中
    /// </summary>
    public readonly int Attack;
    /// <summary>
    /// 图标
    /// </summary>
    public readonly string IconName;
    /// <summary>
    /// 图片渲染
    /// </summary>
    public readonly string SpriteName;
    public readonly string PrefabName;
   
    public const int __ID__ = -337794557;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "ID:" + ID + ","
        + "NAME:" + NAME + ","
        + "DESCRIBE:" + DESCRIBE + ","
        + "interactEffectID:" + InteractEffectID + ","
        + "isDestructible:" + IsDestructible + ","
        + "durability:" + Durability + ","
        + "buffID:" + BuffID + ","
        + "buffTime:" + BuffTime + ","
        + "attack:" + Attack + ","
        + "IconName:" + IconName + ","
        + "SpriteName:" + SpriteName + ","
        + "PrefabName:" + PrefabName + ","
        + "}";
    }
}

}

