
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using SimpleJSON;


namespace cfg.scene
{
public partial class DoorsTable
{
    private readonly System.Collections.Generic.Dictionary<int, scene.Doors> _dataMap;
    private readonly System.Collections.Generic.List<scene.Doors> _dataList;
    
    public DoorsTable(JSONNode _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<int, scene.Doors>();
        _dataList = new System.Collections.Generic.List<scene.Doors>();
        
        foreach(JSONNode _ele in _buf.Children)
        {
            scene.Doors _v;
            { if(!_ele.IsObject) { throw new SerializationException(); }  _v = scene.Doors.DeserializeDoors(_ele);  }
            _dataList.Add(_v);
            _dataMap.Add(_v.ID, _v);
        }
    }

    public System.Collections.Generic.Dictionary<int, scene.Doors> DataMap => _dataMap;
    public System.Collections.Generic.List<scene.Doors> DataList => _dataList;

    public scene.Doors GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public scene.Doors Get(int key) => _dataMap[key];
    public scene.Doors this[int key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}

}

