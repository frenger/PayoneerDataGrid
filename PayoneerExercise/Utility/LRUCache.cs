using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PayoneerExercise.Utility
{
    public class LRUCache<K, V>
    {
        private int Capacity;
        private Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>> CacheMap = new Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>>();
        private LinkedList<LRUCacheItem<K, V>> LruList = new LinkedList<LRUCacheItem<K, V>>();

        public LRUCache(int capacity)
        {
            this.Capacity = capacity;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public V Get(K key)
        {
            LinkedListNode<LRUCacheItem<K, V>> node;
            if (CacheMap.TryGetValue(key, out node))
            {
                V value = node.Value.Value;
                LruList.Remove(node);
                LruList.AddLast(node);
                return value;
            }
            return default(V);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Add(K key, V val)
        {
            if (CacheMap.Count >= Capacity)
            {
                RemoveFirst();
            }

            LRUCacheItem<K, V> cacheItem = new LRUCacheItem<K, V>(key, val);
            LinkedListNode<LRUCacheItem<K, V>> node = new LinkedListNode<LRUCacheItem<K, V>>(cacheItem);
            LruList.AddLast(node);
            CacheMap.Add(key, node);
        }

        private void RemoveFirst()
        {
            // Remove from LRUPriority
            LinkedListNode<LRUCacheItem<K, V>> node = LruList.First;
            LruList.RemoveFirst();

            // Remove from cache
            CacheMap.Remove(node.Value.Key);
        }
    }

    class LRUCacheItem<K, V>
    {
        public LRUCacheItem(K k, V v)
        {
            this.Key = k;
            this.Value = v;
        }
        public K Key;
        public V Value;
    }
}