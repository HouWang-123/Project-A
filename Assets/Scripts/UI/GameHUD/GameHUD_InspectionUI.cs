using cfg.func;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD_InspectionUI : UIBase
{
    public Image BackGroundImage;
    public Image BigImage;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Description;
    public override void Show()
    {
        base.Show();
        BigImage.color = new Color(0, 0, 0, 0);
        BigImage.DOColor(new Color(1, 1, 1, 1), 0.3f).onComplete = () =>
        {
        };
        
    }
    public override void Hide()
    {
        
        base.Hide();
    }
    public void InitInspectionUI(Inspection inspection)
    {
        string describe = inspection.DESCRIBE;
        string title = inspection.NAME;
        string imagename = inspection.ImageSpriteName;
        
        SetDescription(describe);
        SetTitle(title);
        SetBigImage(imagename);
    }


    protected override void AddListen()
    {
        
    }

    private void SetBigImage(string imageUri)
    {
        GameSpriteTool.LoadImageAsync(imageUri, sprite =>
        {
            BigImage.sprite = sprite;
        });
    }

    private void SetTitle(string text)
    {
        Title.text = text;
    }

    private void SetDescription(string text)
    {
        Description.text = text;
    }
}
