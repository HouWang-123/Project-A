// 远程攻击

using System;
using System.Collections.Generic;
using FEVM.ObjectPool;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;


public class ShotBehaviour : BaseWeaponBehavior
{
    public Transform BulletPostion; // 子弹生成坐标

    private AssetHandle loadAssetAsync;
    private GameObject BulletPrefab;
    private string bulletAssetURL;
    
    public void SetBulletPrefabURI(string bulletURI = "ZiDan")
    {
        bulletAssetURL = bulletURI;
        if (loadAssetAsync == null)
        {
            loadAssetAsync = YooAssets.LoadAssetAsync(bulletAssetURL);
            loadAssetAsync.Completed += (handler) =>
            {
                BulletPrefab = handler.AssetObject as GameObject;
            };
            AssetHandle bulletTrailAssetHandler = YooAssets.LoadAssetAsync(bulletAssetURL + "_Trail");

            bulletTrailAssetHandler.Completed += handle =>
            {
                GameObject BulletTrailObject = handle.AssetObject as GameObject;
                GameTrailRendererManager.Instance.PreLoadBulletTrails(WeaponID,6,BulletTrailObject);// 预加载子弹轨迹
            };
        }
    }
    
    public override void OnWeaponAttack(float damageAmount)
    {
        GameObject bullet = GameObjectPool.Instance.GetObject(BulletPrefab, -WeaponID);
        Process(bullet.GetComponent<BulletBehaviour>());
        
        Debug.Log("Damage!" + damageAmount);
        void Process(BulletBehaviour bulletBehaviour)
        {
            bulletBehaviour.SetInitialDamage(damageAmount);
            bulletBehaviour.transform.SetParent(transform);
            bulletBehaviour.transform.localEulerAngles = Vector3.zero;
            bulletBehaviour.gameObject.transform.position = BulletPostion.position;
            bulletBehaviour.gameObject.SetActive(true);
            bulletBehaviour.transform.SetParent(GameControl.Instance.GetSceneItemList().transform);
            bulletBehaviour.GetBulletTrail();
        }
    }
    
}
