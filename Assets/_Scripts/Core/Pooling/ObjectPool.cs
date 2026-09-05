using System.Collections.Generic;
using UnityEngine;

namespace Core.Pooling
{
    public class ObjectPool<T> where T : MonoBehaviour
    {
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly Queue<T> _pool = new Queue<T>();
        private readonly List<T> _active = new List<T>();

        public ObjectPool(T prefab, Transform parent = null, int prewarmCount = 0)
        {
            _prefab = prefab;
            _parent = parent ?? new GameObject($"{typeof(T).Name}Pool").transform;

            for (int i = 0; i < prewarmCount; i++)
            {
                T obj = CreateNew();
                obj.gameObject.SetActive(false);
                _pool.Enqueue(obj);
            }
        }

        private T CreateNew()
        {
            T obj = Object.Instantiate(_prefab, _parent);
            obj.gameObject.SetActive(false);
            return obj;
        }

        public T Spawn(Vector3 position, Quaternion rotation)
        {
            T obj = _pool.Count > 0 ? _pool.Dequeue() : CreateNew();
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.gameObject.SetActive(true);
            _active.Add(obj);

            if (obj is IPoolable poolable)
            {
                poolable.OnSpawn();
            }

            return obj;
        }

        public void Despawn(T obj)
        {
            if (_active.Contains(obj) == false)
            {
                return;
            }

            obj.gameObject.SetActive(false);
            _active.Remove(obj);
            _pool.Enqueue(obj);

            if (obj is IPoolable poolable)
            {
                poolable.OnDespawn();
            }
        }

        public void DespawnAll()
        {
            List<T> activeCopy = new List<T>(_active);
            foreach (T obj in activeCopy)
            {
                Despawn(obj);
            }
        }
    }
}