using UnityEngine;
using UnityEngine.Serialization;

public class RiddleSwitch : RiddleItemBase
{
    [FormerlySerializedAs("onSpriteRenderer")] public SpriteRenderer SwithchSpriteRenderer;
    public Sprite OnStatusSprite;
    public Sprite OffStatusSprite;
    public bool DefaultValue;
    public SwitchStatus SwitchStatus
    {
        set
        {
            _switchStatus = value;
            RiddleManager.OnRiddleItemStatusChange(this); // 通知Manager状态发生改变
        }
        get
        {
            return _switchStatus;
        }
    }

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
            SetRiddleItemStatus(new SwitchStatus(ison));
        }
        else
        {
            SwithchSpriteRenderer.sprite = OffStatusSprite;
            SetRiddleItemStatus(new SwitchStatus(ison));
        }
    }

    public override void OnPlayerInteract()
    {

    }

    public override RiddleItemBaseStatus GetRiddleStatus()
    {
        return SwitchStatus;
    }
    public override void SetRiddleItemStatus(RiddleItemBaseStatus BaseStatus)
    {
        SwitchStatus = BaseStatus as SwitchStatus;
    }
    
    public override bool hasInteraction(int itemid)
    {
        return GameItemInteractionHub.HasInteract(itemid, itemId);
    }

    public override void OnPlayerStartInteract(int itemid)
    {
        ChangeSwitch(!SwitchStatus.is_on);
    }

    public override void OnPlayerFocus(int itemid)
    {
        OnPlayerFocus();
    }

    public bool GetSwitchValue()
    {
        return SwitchStatus.is_on;
    }
}

