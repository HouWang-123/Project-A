using UnityEngine;

public class SaveDataPanel : UIBase
{


    protected override void Awake()
    {
        base.Awake();
        SetSelect("GameSaveData1_N");
    }

    protected override void AddListen()
    {
        AddButtonListen("LoadButton_N", () =>
        {
            UIManager.Instance.GetPanel(UIPanelConfig.MainPanel).GetComponent<UIBase>().Hide();
            Hide();
            UIManager.Instance.GetPanel(UIPanelConfig.HUD_MainGameHUD).GetComponent<UIBase>().Show();
            GameControl.Instance.GameStart();
        });
        AddButtonListen("CancelButton_N", () =>
        {
            //UIManager.Instance.GetGameObject("MainPanel", "GameStartButton_N").GetComponent<UIBehaviour>().SetSelect();
            UIManager.Instance.GetPanel(UIPanelConfig.MainPanel).GetComponent<UIBase>().SetSelect("GameStartButton_N");
            Hide();
        });
    }
}
