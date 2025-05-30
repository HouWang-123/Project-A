
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using SimpleJSON;


namespace cfg.text
{
public partial class LocalizationTable
{
    private readonly System.Collections.Generic.Dictionary<int, text.Localization> _dataMap;
    private readonly System.Collections.Generic.List<text.Localization> _dataList;
    
    public LocalizationTable(JSONNode _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<int, text.Localization>();
        _dataList = new System.Collections.Generic.List<text.Localization>();
        
        foreach(JSONNode _ele in _buf.Children)
        {
            text.Localization _v;
            { if(!_ele.IsObject) { throw new SerializationException(); }  _v = text.Localization.DeserializeLocalization(_ele);  }
            _dataList.Add(_v);
            _dataMap.Add(_v.ID, _v);
        }
    }

    public System.Collections.Generic.Dictionary<int, text.Localization> DataMap => _dataMap;
    public System.Collections.Generic.List<text.Localization> DataList => _dataList;

    public text.Localization GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public text.Localization Get(int key) => _dataMap[key];
    public text.Localization this[int key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}

}

