using System;
using DG.Tweening;
using Spine.Unity.Examples;
using UnityEngine;
using UnityEngine.Serialization;
using YooAsset;

public class Weapon : ItemBase, ISlotable
{
    public cfg.item.Weapon data;
    
    [FormerlySerializedAs("BaseWeaponBehavior")] public BaseWeaponBehavior _weaponBeahaviour;
    
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
        
        GameRunTimeData.Instance.ItemManager.RegistItem(this);
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
        
    }
    
    public void OnWeaponAttack()
    {
        if (_weaponBeahaviour is ShotBehaviour)
        {
            if (GameRunTimeData.Instance.InventoryManger.HasItem(data.AmmoId))
            {
                GameRunTimeData.Instance.InventoryManger.UseItem(data.AmmoId, 1);
                _weaponBeahaviour.OnWeaponAttack(CalculateShotDamage());
            }
        }

        if (_weaponBeahaviour is SlashBehaviour)
        {
            CalculateSlashDamage();
            _weaponBeahaviour.OnWeaponAttack(CalculateSlashDamage());
        }
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
