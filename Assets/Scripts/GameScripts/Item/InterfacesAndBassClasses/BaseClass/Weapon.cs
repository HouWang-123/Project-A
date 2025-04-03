using System;
using System.Runtime.Serialization;
using DG.Tweening;
using Spine.Unity.Examples;
using UnityEngine;
using UnityEngine.Serialization;
using YooAsset;

public class Weapon : ItemBase, ISlotable
{
    public cfg.item.Weapon data;
    protected BaseWeaponBehavior _weaponBeahaviour;
    protected ShotBehaviour _shotBehaviour;
    protected SlashBehaviour _slashBehaviour;
    private float currentCd;
    
    // 动态生成物品
    public override void InitItem( int ID )
    {
        ItemType = GameItemType.Weapon;
        try
        {
            ItemData = GameTableDataAgent.WeaponTable.Get(ID);
            data = ItemData as cfg.item.Weapon;
            ItemID = data.ID;
        }
        catch (Exception e)
        {
            ColorfulDebugger.DebugError("武器物品ID" + ID +"不存在，物品名称" + gameObject.name,ColorfulDebugger.Instance.Data);
        }
        ItemSpriteName = data.SpriteName;
        GetWeaponBehaviour();
    }
    
    public void GetWeaponBehaviour()
    {
        _weaponBeahaviour = GetComponent<BaseWeaponBehavior>();
        if (_weaponBeahaviour is ShotBehaviour)
        {
            _shotBehaviour = _weaponBeahaviour as ShotBehaviour;
            _shotBehaviour.WeaponID = ItemID;
            _shotBehaviour.SetBulletPrefabURI();
        }

        if (_weaponBeahaviour is SlashBehaviour)
        {
            _slashBehaviour = _weaponBeahaviour as SlashBehaviour;
            _slashBehaviour.WeaponID = ItemID;
        }
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
    public override void OnRightInteract( )
    {
        
    }
    
    public override void OnLeftInteract( )
    {
        OnWeaponAttack();
    }

    protected override void F_UpdateWeaponCDRecover()
    {
        currentCd -= Time.deltaTime;
    }

    public void OnWeaponAttack()
    {
        if (currentCd > 0) return;      // WEAPON CD
        if (_shotBehaviour != null)
        {
            if (GameRunTimeData.Instance.InventoryManger.HasItem(data.AmmoId)) ///  Requeired Ammos
            {
                GameRunTimeData.Instance.InventoryManger.UseItem(data.AmmoId, 1);
                _shotBehaviour.OnWeaponAttack(CalculateShotDamage());
            }
            // TEST 
            _shotBehaviour.OnWeaponAttack(CalculateShotDamage());
        }

        if (_slashBehaviour != null)
        {
            CalculateSlashDamage();
            _slashBehaviour.SetInitialDamage(CalculateSlashDamage());
        }
        currentCd = data.AttackCD;
    }
    
    public float CalculateSlashDamage()
    {
        return data.Attack;
    }

    public float CalculateShotDamage()
    {
        return data.Attack;
    }
    public int GetItemId()
    {
        return ItemID;
    }
}
