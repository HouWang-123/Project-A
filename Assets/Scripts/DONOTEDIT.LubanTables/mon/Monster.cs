
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using SimpleJSON;


namespace cfg.mon
{
public sealed partial class Monster : Luban.BeanBase
{
    public Monster(JSONNode _buf) 
    {
        { if(!_buf["ID"].IsNumber) { throw new SerializationException(); }  ID = _buf["ID"]; }
        { if(!_buf["NAME"].IsString) { throw new SerializationException(); }  NAME = _buf["NAME"]; }
        { if(!_buf["DESCRIBE"].IsString) { throw new SerializationException(); }  DESCRIBE = _buf["DESCRIBE"]; }
        { if(!_buf["prefabName"].IsString) { throw new SerializationException(); }  PrefabName = _buf["prefabName"]; }
        { if(!_buf["maxHP"].IsNumber) { throw new SerializationException(); }  MaxHP = _buf["maxHP"]; }
        { if(!_buf["attack"].IsNumber) { throw new SerializationException(); }  Attack = _buf["attack"]; }
        { if(!_buf["speed"].IsNumber) { throw new SerializationException(); }  Speed = _buf["speed"]; }
        { if(!_buf["warnRange"].IsNumber) { throw new SerializationException(); }  WarnRange = _buf["warnRange"]; }
        { if(!_buf["hitRange"].IsNumber) { throw new SerializationException(); }  HitRange = _buf["hitRange"]; }
        { if(!_buf["shootRange"].IsNumber) { throw new SerializationException(); }  ShootRange = _buf["shootRange"]; }
        { if(!_buf["hitDegree"].IsNumber) { throw new SerializationException(); }  HitDegree = _buf["hitDegree"]; }
    }

    public static Monster DeserializeMonster(JSONNode _buf)
    {
        return new mon.Monster(_buf);
    }

    /// <summary>
    /// 序号
    /// </summary>
    public readonly int ID;
    /// <summary>
    /// 名称
    /// </summary>
    public readonly string NAME;
    /// <summary>
    /// 描述
    /// </summary>
    public readonly string DESCRIBE;
    /// <summary>
    /// 预制体名称
    /// </summary>
    public readonly string PrefabName;
    /// <summary>
    /// 生命值
    /// </summary>
    public readonly int MaxHP;
    /// <summary>
    /// 攻击力
    /// </summary>
    public readonly int Attack;
    /// <summary>
    /// 移动速度
    /// </summary>
    public readonly float Speed;
    /// <summary>
    /// 警戒范围（m）
    /// </summary>
    public readonly float WarnRange;
    /// <summary>
    /// 近战攻击范围(m)
    /// </summary>
    public readonly float HitRange;
    /// <summary>
    /// 远程攻击范围(m)
    /// </summary>
    public readonly float ShootRange;
    /// <summary>
    /// 近战角度
    /// </summary>
    public readonly int HitDegree;
   
    public const int __ID__ = 1577306840;
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
        + "prefabName:" + PrefabName + ","
        + "maxHP:" + MaxHP + ","
        + "attack:" + Attack + ","
        + "speed:" + Speed + ","
        + "warnRange:" + WarnRange + ","
        + "hitRange:" + HitRange + ","
        + "shootRange:" + ShootRange + ","
        + "hitDegree:" + HitDegree + ","
        + "}";
    }
}

}

