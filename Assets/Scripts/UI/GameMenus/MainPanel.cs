using UnityEngine;

public class MainPanel : UIBase
{
    protected override void Awake()
    {
        base.Awake();
        SetSelect("GameStartButton_N");
    }
    protected override void AddListen()
    {
        AddButtonListen("GameStartButton_N", () =>
        {
            UIManager.Instance.GetPanel(UIPanelConfig.SaveDataPanel).GetComponent<UIBase>().Show();
        });
    }

}
