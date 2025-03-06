using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using System.Threading;
using JetBrains.Annotations;
using NUnit.Framework;

namespace FEVM.ObjectPool
{
    public interface FT_IPoolable
    {
        void OnRecycle();
        void OnReEnable();
        void SetBucketId(int ID);
    }

    public class GameObjectPool : Singleton<GameObjectPool>
    {
        // 使用线程安全的ConcurrentDictionary替代Dictionary
        private readonly Dictionary<int, Queue<GameObject>> ObjectPool = new Dictionary<int, Queue<GameObject>>();
        // 对象池容量限制
        private const int MaxPoolSize = 200;
        
        public GameObject GetObject([NotNull] GameObject prefab, int BucketId,Transform target = null)
        {
            // 确保bucket存在
            ObjectPool.TryAdd(BucketId, new Queue<GameObject>());

            GameObject obj = null;
            if (ObjectPool[BucketId].TryDequeue(out obj))
            {
                if (obj != null)
                {
                    if (target != null)
                    {
                        obj.transform.position = target.position;
                    }
                    obj.SetActive(true);
                    foreach (var VARIABLE in obj.GetComponents<FT_IPoolable>())
                    {
                        VARIABLE.OnReEnable();
                    }
                    return obj;
                }
            }

            GameObject instantiate = Instantiate(prefab);
            instantiate.transform.SetParent(transform);
            foreach (var VARIABLE in instantiate.GetComponents<FT_IPoolable>())
            {
                VARIABLE.SetBucketId(BucketId);
            }
            return instantiate;
        }
        public void RecycleGameObject(GameObject gameObject, int BucketId)
        {
            if (gameObject == null) return;
            
            // 确保bucket存在
            ObjectPool.TryAdd(BucketId, new Queue<GameObject>());

            var queue = ObjectPool[BucketId];
            if (queue.Count >= MaxPoolSize)
            {
                Destroy(gameObject);
                return;
            }

            gameObject.SetActive(false);
            
            Transform t = gameObject.transform;
            t.SetParent(this.transform);
            t.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            t.localScale = Vector3.one;
            
            foreach (var poolable in gameObject.GetComponents<FT_IPoolable>())
            {
                poolable.OnRecycle();
            }
            
            queue.Enqueue(gameObject);
        }
        public List<GameObject> Preload([NotNull] GameObject prefab,int BucketId,int count)
        {
            if (ObjectPool.ContainsKey(BucketId)) return null; // 防止多次预加载
            if (count <= 0) return null;
            // 确保bucket存在
            ObjectPool.TryAdd(BucketId, new Queue<GameObject>());
            
            var queue = ObjectPool[BucketId];
            int currentCount = queue.Count;
            int targetCount = Math.Min(count, MaxPoolSize);
            List<GameObject> preloaded = new List<GameObject>();
            for (int i = currentCount; i < targetCount; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                obj.transform.SetParent(transform);
                preloaded.Add(obj);
                queue.Enqueue(obj);
                foreach (var VARIABLE in obj.GetComponents<FT_IPoolable>())
                {
                    VARIABLE.SetBucketId(BucketId);
                }
            }
            return preloaded;
        }
        
    }
}