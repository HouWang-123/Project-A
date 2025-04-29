using System;
using cfg.interact;
using FEVM.Timmer;
using UnityEngine;

public class SoundSWMono : RiddleItemBase
{
    public float SoundSustainTime;
    public SwitchStatus isCovered;
    public bool Active;
    public SpriteRenderer SoundSwitchRenderer;
    public SpriteRenderer MyCoverSpRenderer;
    public Sprite SoundSwitchRendere;
    public Sprite OnSoundReceive;

    
    [ContextMenu("TestSoundHearing")]
    public void OnSoundHearing()
    {
        if (isCovered.is_on)
        {
            Debug.Log("======Covered=======");
            return;
        }
        Active = true;
        SoundSwitchRenderer.sprite = OnSoundReceive;
        TimeMgr.Instance.AddTask(SoundSustainTime,false, () => { SoundHearingStop(); });
    }

    public void OnDestroy()
    {
        TimeMgr.Instance.RemoveTask(SoundHearingStop);
    }

    public void SoundHearingStop()
    {
        Active = false;
        if (isCovered.is_on)
        {
            
        }
        else
        {
            SoundSwitchRenderer.sprite = SoundSwitchRendere;
        }
    }
    public void Cover()
    {
        isCovered.is_on = true;
        itemId = 220016;
        SceneObjectsData = GameTableDataAgent.SceneObjectsTable.Get(220016);
        MyCoverSpRenderer.gameObject.SetActive(true);
    }
    public void UnCover()
    {
        isCovered.is_on = false;
        itemId = 220015;
        SceneObjectsData = GameTableDataAgent.SceneObjectsTable.Get(220015);
        MyCoverSpRenderer.gameObject.SetActive(false);
    }
    
    public void Start()
    {
        base.Start();
        if (isCovered == null)
        {
            isCovered = new SwitchStatus(false);
        }
        if (isCovered.is_on)
        {
            Cover();
        }
        else
        {
            UnCover();
        }
    }
    public override void OnPlayerInteract() { }
    public override RiddleItemBaseStatus GetRiddleStatus() { return isCovered; }

    public override void SetRiddleItemStatus(RiddleItemBaseStatus BaseStatus)
    {
        isCovered = BaseStatus as SwitchStatus;
    }
    public override void OnPlayerStartInteract(int itemid)
    {
        if (isCovered.is_on)
        {
            int findInteractEffectById = GameItemInteractionHub.FindInteractEffectById(itemid, itemId);
            InteractEffect interactEffect = GameTableDataAgent.InteractEffectTable.Get(findInteractEffectById);
            GameInteractSystemExtendedCode.ExecuteInteraction(interactEffect.CodeExecuteID, gameObject);
            UnCover();
        }
        else
        {
            if (GameRunTimeData.Instance.characterBasicStatManager.GetStat().LiftedItem != null)
            {
                if (GameRunTimeData.Instance.characterBasicStatManager.GetStat().LiftedItem.ItemID == 230001 )
                {
                    int findInteractEffectById = GameItemInteractionHub.FindInteractEffectById(itemid, itemId);
                    InteractEffect interactEffect = GameTableDataAgent.InteractEffectTable.Get(findInteractEffectById);
                    GameInteractSystemExtendedCode.ExecuteInteraction(interactEffect.CodeExecuteID, gameObject);
                    Cover();
                }
            }
        }
    }

    public override bool GetRiddleItemResult()
    {
        if (isCovered.is_on)
        {
            return false;
        }
        return Active;
    }
}