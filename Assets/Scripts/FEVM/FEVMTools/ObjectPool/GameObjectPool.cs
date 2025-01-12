using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using System.Threading;
using JetBrains.Annotations;

namespace FEVM.ObjectPool
{
    public interface FT_IPoolable
    {
        void OnRecycle();
    }

    public class GameObjectPool : Singleton<GameObjectPool>
    {
        // 使用线程安全的ConcurrentDictionary替代Dictionary
        private readonly ConcurrentDictionary<int, ConcurrentQueue<GameObject>> ObjectPool = new ConcurrentDictionary<int, ConcurrentQueue<GameObject>>();
        // 对象池容量限制
        private const int MaxPoolSize = 200;
        private float lastCleanupTime;
        
        public GameObject GetObject([NotNull] GameObject prefab, int BucketId)
        {
            // 确保bucket存在
            ObjectPool.TryAdd(BucketId, new ConcurrentQueue<GameObject>());

            GameObject obj = null;
            if (ObjectPool[BucketId].TryDequeue(out obj))
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                    return obj;
                }
            }
            // 在主线程中实例化
            if (Thread.CurrentThread.ManagedThreadId != 1)
            {
                return null; // 不在主线程则返回null
            }
            return Instantiate(prefab);
        }
        public void RecycleGameObject(GameObject gameObject, int BucketId)
        {
            if (gameObject == null) return;
            
            // 确保bucket存在
            ObjectPool.TryAdd(BucketId, new ConcurrentQueue<GameObject>());

            var queue = ObjectPool[BucketId];
            if (queue.Count >= MaxPoolSize)
            {
                Destroy(gameObject);
                return;
            }

            gameObject.SetActive(false);

            // Reset transform
            Transform t = gameObject.transform;
            t.SetParent(this.transform);
            t.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            t.localScale = Vector3.one;

            // Call reset for poolable components
            foreach (var poolable in gameObject.GetComponents<FT_IPoolable>())
            {
                poolable.OnRecycle();
            }

            queue.Enqueue(gameObject);
        }
        public List<GameObject> Preload([NotNull] GameObject prefab,int BucketId,int count)
        {
            
            if (count <= 0) return null;
            // 确保bucket存在
            ObjectPool.TryAdd(BucketId, new ConcurrentQueue<GameObject>());
            
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
            }
            return preloaded;
        }
        
    }
}