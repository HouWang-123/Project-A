using System;
using FEVM.Timmer;
using PixelCrushers.DialogueSystem.ChatMapper;
using UnityEngine;
using YooAsset;

public class Throwable : ItemBase, ILiftable, IThrowable
{
    public cfg.item.ThrowObjects data;
    public Rigidbody ThrowableRigidbody;
    
    private bool ObjectStoped;
    // 物品初始化
    public override void InitItem(int id,TrackerData trackerData = null)
    {
        base.InitItem(id,trackerData);
        ItemType = GameItemType.Throwable;

        try
        {
            ItemData = GameTableDataAgent.ThrowObjectsTable.Get(id);
            data = ItemData as cfg.item.ThrowObjects;
            ItemID = data.ID;
        }
        catch (Exception e)
        {
            ColorfulDebugger.DebugError("可投掷物品ID" + id +"不存在，物品名称" + gameObject.name,ColorfulDebugger.Instance.Data);
        }
        ItemSpriteName = data.SpriteName;
        ThrowableRigidbody = GetComponent<Rigidbody>();
        IgnoreDefaultItemDrop = true;
    }
    public override void OnItemPickUp()
    {
        IsholdByPlayer = true;
        base.OnItemPickUp();
        ThrowableRigidbody.isKinematic = true;
        ThrowableRigidbody.Sleep();
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
    public override string GetPrefabName()
    {
        return data.PrefabName;
    }
    
    
    public override void OnRightInteract()
    {

    }
    
    public override void OnLeftInteract()
    {
        OnThrow();
    }

    public void OnThrow()
    {
        DropState = true;
        ObjectStoped = false;
        ThrowableRigidbody.isKinematic = false;
        ThrowableRigidbody.AddForce( PlayerControl.Instance.PlayerLookatDirection * 3 + new Vector3(0f,4f,0f) ,ForceMode.VelocityChange);
        ThrowableRigidbody.WakeUp();
        PlayerControl.Instance.DropItem(false);
        TimeMgr.Instance.AddTask(0.1f,false, () =>
        {
            IsholdByPlayer = false;
        });
    }

    public override void OnItemDrop(bool fastDrop, bool IgnoreBias = false, bool Playerreversed = false)
    {
        base.OnItemDrop(fastDrop, IgnoreBias, Playerreversed);
        IgnoreDefaultItemDrop = true;
        DropState = true;
        ThrowableRigidbody.isKinematic = false;
        ThrowableRigidbody.WakeUp();
        ObjectStoped = false;
        TimeMgr.Instance.AddTask(0.1f,false, () =>
        {
            IsholdByPlayer = false;
        });
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (IsholdByPlayer) return;
        if (ObjectStoped) return;
        if (ThrowableRigidbody.linearVelocity == Vector3.zero)
        {
            ObjectStoped = true;
            ThrowableRigidbody.Sleep();
            DropState = false;
            OnDropCallback?.Invoke();
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    protected override void OnCollisionEnter(Collision other)
    {

    }
}
