using System;
using FEVM.Timmer;
using PixelCrushers.DialogueSystem.ChatMapper;
using UnityEngine;
using YooAsset;

public class Throwable : ItemBase, ILiftable, IThrowable
{
    public cfg.item.ThrowObjects data;
    public Rigidbody ThrowableRigidbody;
    public Collider _collider;
    private float originalDamp;
    private bool ObjectStoped;

    private ThrowableStatus _throwableStatus;

    // 物品初始化
    public override void SetItemStatus(ItemStatus itemStatus)
    {
        base.SetItemStatus(itemStatus);
        _throwableStatus = MyItemStatus as ThrowableStatus;
    }

    public override void InitItem(int id, TrackerData trackerData = null)
    {
        base.InitItem(id, trackerData);
        ItemType = GameItemType.Throwable;

        try
        {
            ItemData = GameTableDataAgent.ThrowObjectsTable.Get(id);
            data = ItemData as cfg.item.ThrowObjects;
            ItemID = data.ID;
        }
        catch (Exception e)
        {
            Debug.LogError("可投掷物品ID" + id + "不存在，物品名称" + gameObject.name);
        }

        ItemSpriteName = data.SpriteName;
        ThrowableRigidbody = GetComponent<Rigidbody>();
        IgnoreDefaultItemDrop = true;
        originalDamp = ThrowableRigidbody.linearDamping;
    }

    public override void OnItemPickUp()
    {
        ThrowableRigidbody.linearDamping = 0;
        IsholdByPlayer = true;
        base.OnItemPickUp();
        ThrowableRigidbody.isKinematic = true;
        ThrowableRigidbody.Sleep();
        _collider.enabled = false;
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
        base.OnRightInteract();
    }

    public override void OnLeftInteract()
    {
        if (startactionTime < 0.1f)
        {
            return;
        }
        OnThrow();
    }

    public void OnThrow()
    {
        ThrowableRigidbody.linearDamping = originalDamp;
        DropState = true;
        ObjectStoped = false;
        ThrowableRigidbody.isKinematic = false;
        ThrowableRigidbody.AddForce(PlayerControl.Instance.PlayerLookatDirection * 3 + new Vector3(0f, 4f, 0f),
            ForceMode.VelocityChange);
        ThrowableRigidbody.WakeUp();

        PlayerControl.Instance.DropItem(false); // ------------ 这里会调用 OnItemDrop();
    }

    private void EnableColiders()
    {
        IsholdByPlayer = false;
        _collider.enabled = true;
    }
    public override void OnItemDrop(bool fastDrop, bool IgnoreBias = false, bool Playerreversed = false)
    {
        RegisterTracker();
        ThrowableRigidbody.isKinematic = false;
        IgnoreDefaultItemDrop = true;
        DropState = true;

        ThrowableRigidbody.WakeUp();
        ObjectStoped = false;
        TimeMgr.Instance.AddTask(0.1f, false, EnableColiders);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        TimeMgr.Instance.RemoveTask(EnableColiders);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (IsholdByPlayer) return;
        if (ObjectStoped) return;
        if (DropState)
        {
            if (ThrowableRigidbody.linearVelocity == Vector3.zero)
            {
                ObjectStoped = true;
                DropState = false;
                OnDropCallback?.Invoke();
            }
        }
    }

    protected override void OnCollisionEnter(Collision other)
    {
    }
}