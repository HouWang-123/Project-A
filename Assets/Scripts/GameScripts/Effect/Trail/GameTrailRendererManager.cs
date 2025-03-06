using System.Collections.Generic;
using FEVM.ObjectPool;
using UnityEngine;

public class GameTrailRendererManager : Singleton<GameTrailRendererManager>
{
    private Dictionary<int, GameObject> PrefabList = new Dictionary<int, GameObject>();
    public void PreLoadBulletTrails(int buketId,int Count, GameObject prefab)
    {
        GameObjectPool.Instance.Preload(prefab, buketId, Count);
        PrefabList.TryAdd(buketId,prefab);
    }

    public PoolableTrail GetATrail(int WeaponId,Transform position)
    {
        GameObject o = GameObjectPool.Instance.GetObject(PrefabList[WeaponId], WeaponId,position);
        return o.GetComponent<PoolableTrail>();
    }

    public void RecycleTrail(int weaponId,GameObject trail)
    {
        GameObjectPool.Instance.RecycleGameObject(trail,weaponId);
    }
}
