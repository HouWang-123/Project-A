using UnityEngine;

public class SaveDataPanel : UIBase
{


    protected override void Awake()
    {
        base.Awake();

    }

    protected override void AddListen()
    {
        AddButtonListen("LoadButton_N", () =>
        {
            UIManager.Instance.GetPanel("MainPanel").GetComponent<UIBase>().Hide();
            Hide();
            UIManager.Instance.GetPanel("GamePanel").GetComponent<UIBase>().Show();
        });
        AddButtonListen("CancelButton_N", () =>
        {
            Hide();
        });
    }
}
