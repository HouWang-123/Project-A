// 远程攻击

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;


public class ShotBehaviour : BaseWeaponBehavior
{
    public Transform BulletPostion; // 子弹生成坐标
    public Queue<BulletBehaviour> Pool;
    private AssetHandle loadAssetAsync;
    private string bulletAssetURL;

    public void Start()
    {
        Pool = new Queue<BulletBehaviour>();
        EventManager.Instance.RegistEvent(EventConstName.OnChangeRoom,ClearBulletPool);
    }

    public void ClearBulletPool()
    {
        Pool = new Queue<BulletBehaviour>();
    }
    public void SetBulletPrefabURI(string bulletURI = "ZiDan")
    {
        bulletAssetURL = bulletURI;
        if (loadAssetAsync == null)
        {
            loadAssetAsync = YooAssets.LoadAssetAsync(bulletAssetURL);
            AssetHandle bulletTrailAssetHandler = YooAssets.LoadAssetAsync(bulletAssetURL + "_Trail");

            bulletTrailAssetHandler.Completed += handle =>
            {
                GameObject BulletTrailObject = handle.AssetObject as GameObject;
                GameTrailRendererManager.Instance.PreLoadBulletTrails(WeaponID,6,BulletTrailObject);// 预加载子弹轨迹
            };
        }
        
    }
    public void RecycleBullet(BulletBehaviour bulletBehaviour)
    {
        bulletBehaviour.gameObject.transform.position = Vector3.zero;
        bulletBehaviour.gameObject.SetActive(false);
        bulletBehaviour.RecycleTrail();
        Pool.Enqueue(bulletBehaviour);
    }
    public override void OnWeaponAttack(float damageAmount)
    {
        if (Pool.Count == 0)
        {
            GameObject assetObject = loadAssetAsync.AssetObject as GameObject;
            GameObject bulletInstance = Instantiate(assetObject);
            
            BulletBehaviour bulletBehaviour = bulletInstance.GetComponent<BulletBehaviour>();
            bulletBehaviour.WeaponId = WeaponID;
            bulletBehaviour.SetShotParent(this);
            Process(bulletBehaviour);
        }
        else
        {
            BulletBehaviour bulletBehaviour = Pool.Dequeue();
            Process(bulletBehaviour);
        }
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
    public void OnDestroy()
    {
        foreach (var bullet in Pool)
        {
            Destroy(bullet.gameObject);
        }
        Pool = null;
        loadAssetAsync = null;
        EventManager.Instance.RemoveEvent(EventConstName.OnChangeRoom,ClearBulletPool);
    }
}
