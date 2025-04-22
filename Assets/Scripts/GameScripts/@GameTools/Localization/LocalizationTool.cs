using System;
using cfg.text;
public enum Localizations
{
    Zh,
    En,
    Jp
}

public class LocalizationTool : Singleton<LocalizationTool>
{
    private Localizations locals;
    // 默认语言环境初始化
    public void InitLocal(Localizations local)
    {
        locals = local;
    }
    // 通过 game settings 修改语言首选项
    public void ChangeLocals(Localizations local)
    {
        locals = local;
        EventManager.Instance.RunEvent(EventConstName.GAME_SETTINGS_TRANSLATION_CHANGES); // 通知所有 TEXT 更改文本
    }

    // 传入文本ID获取当前语言环境的值
    public string GetTranslation(int id)
    {
        string s = "";
        switch (locals)
        {
            case Localizations.Zh:
                s = GameTableDataAgent.LocalizationTable.Get(id).Zh;
                if (String.Equals(s,""))
                {
                    return "当前语言环境下翻译缺失,文本id:" + id;
                }
                return s;
            case Localizations.En:
                s = GameTableDataAgent.LocalizationTable.Get(id).En;
                if (String.Equals(s,""))
                {
                    return "Translation for text id: " + id + " is not found";
                }
                return s;
            case Localizations.Jp:
                s = GameTableDataAgent.LocalizationTable.Get(id).Zh;
                if (String.Equals(s,""))
                {
                    return "現在のロケール、テキスト ID に翻訳がありません:" + id;
                }
                return s;
            default:
                return "语言环境出错";
        }
    }
}
