using System;
using System.Collections.Generic;
using cfg.interact;
using UnityEngine;
using YooAsset;

public class SceneObjects : ItemBase , IInteractableItemReceiver
{
    public cfg.item.SceneObjects data;
    private GameInteractTip _interactTip;
    public override void InitItem(int id, TrackerData trackerData = null)
    {
        base.InitItem(id,trackerData);
        ItemType = GameItemType.SceneObject;
        try
        {
            ItemData = GameTableDataAgent.SceneObjectsTable.Get(id);
            data = ItemData as cfg.item.SceneObjects;
            ItemID = data.ID;
        }
        catch (Exception e)
        {
            Debug.LogError("场景物品ID" + ItemID +"不存在，物品名称" + gameObject.name);
        }
        ItemSpriteName = data.SpriteName;
        
    }
    public override Sprite GetItemIcon()
    {
        AssetHandle loadAssetSync = YooAssets.LoadAssetSync<Sprite>(data.IconName);
        if (loadAssetSync.AssetObject == null)
        {
            loadAssetSync = YooAssets.LoadAssetSync<Sprite>("SpriteNotFound_Default");
        }
        return Instantiate(loadAssetSync.AssetObject, transform) as Sprite;
    }
    public override string GetPrefabName() { return data.PrefabName; }
    // Interact
    public void OnPlayerDefocus()
    {
        if(_interactTip == null) return;
        _interactTip.OnDetargeted();
    }
    public bool hasInteraction(int itemid) // interacted Item Id;
    {
        return GameItemInteractionHub.HasInteract(itemid,ItemID);
    }
    public virtual void OnPlayerStartInteract(int itemid)
    {
        Debug.Log("交互成功，交互物品ID：" + itemid);
    }

    public virtual void OnPlayerFocus(int itemid)
    {
        if (_interactTip == null)
        {
            AssetHandle loadAssetAsync = YooAssets.LoadAssetAsync<GameObject>("P_UI_WorldUI_InteractTip");
            loadAssetAsync.Completed += (loadAssetAsync) =>
            {
                GameObject objAssetObject = loadAssetAsync.AssetObject as GameObject;
                GameObject instantiate = Instantiate(objAssetObject);
                instantiate.transform.position = transform.position;
                instantiate.transform.position += new Vector3(0, 2f, 0);
                instantiate.transform.localEulerAngles = GameConstData.DefAngles;
                instantiate.transform.SetParent(transform);
                _interactTip = instantiate.GetComponent<GameInteractTip>();
                _interactTip.PlayInitAnimation();
            };
        }
        else
        {
            _interactTip.PlayInitAnimation();
        }
    }
    
    
    // notused implements
    public virtual void OnPlayerStartInteract(){}
    public virtual void OnPlayerInteract(){}
    public virtual void OnPlayerInteractCancel(){}
    public MonoBehaviour getMonoBehaviour() { return this; }
    public void OnPlayerFocus(){}
}
