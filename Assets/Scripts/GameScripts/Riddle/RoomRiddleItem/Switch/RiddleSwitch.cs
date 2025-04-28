using UnityEngine;
using UnityEngine.Serialization;

public class RiddleSwitch : RiddleItemBase
{
    [FormerlySerializedAs("onSpriteRenderer")] public SpriteRenderer SwithchSpriteRenderer;
    public Sprite OnStatusSprite;
    public Sprite OffStatusSprite;
    public bool DefaultValue;


    private SwitchStatus _switchStatus;
    public void Start()
    {
        base.Start();
        _switchStatus = new SwitchStatus(DefaultValue);
    }
    
    public void ChangeSwitch(bool ison)
    {
        if (ison)
        {
            SwithchSpriteRenderer.sprite = OnStatusSprite;
            _switchStatus.is_on = ison;
            RiddleManager.OnRiddleItemStatusChange(this); // 通知Manager状态发生改变
        }
        else
        {
            SwithchSpriteRenderer.sprite = OffStatusSprite;
            _switchStatus.is_on = ison;
            RiddleManager.OnRiddleItemStatusChange(this); // 通知Manager状态发生改变
        }
    }

    public override void OnPlayerInteract()
    {

    }

    public override RiddleItemBaseStatus GetRiddleStatus()
    {
        return _switchStatus;
    }
    public override void SetRiddleItemStatus(RiddleItemBaseStatus BaseStatus)
    {
        _switchStatus = BaseStatus as SwitchStatus;
        ChangeSwitch(_switchStatus.is_on);
    }
    

    public override void OnPlayerStartInteract(int itemid)
    {
        ChangeSwitch(!_switchStatus.is_on);
    }

    public override bool GetRiddleItemResult()
    {
        return _switchStatus.is_on;
    }
}

