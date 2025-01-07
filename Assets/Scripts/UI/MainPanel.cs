using UnityEngine;

public class MainPanel : UIBase
{
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void AddListen()
    {
        AddButtonListen("GameStartButton_N", () =>
        {
            UIManager.Instance.GetPanel("SaveDataPanel").GetComponent<UIBase>().Show();
        });
    }

}
