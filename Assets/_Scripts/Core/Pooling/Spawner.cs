using UnityEngine;

namespace Core.Pooling
{
    public abstract class Spawner<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private T _prefab;
        [SerializeField] private int _prewarmCount = 10;
        [SerializeField] private Transform _parentForPool;

        private ObjectPool<T> _pool;

        protected virtual void Awake()
        {
            if (_parentForPool == null)
            {
                _parentForPool = transform;
            }

            _pool = new ObjectPool<T>(_prefab, _parentForPool, _prewarmCount);
        }

        public T Spawn(Vector3 position, Quaternion rotation)
        {
            T obj = _pool.Spawn(position, rotation);
            if (obj is IPoolable poolable)
            {
                poolable.SetPool(_pool);
            }
            return obj;
        }

        public void Despawn(T obj)
        {
            _pool.Despawn(obj);
        }

        public void DespawnAll()
        {
            _pool.DespawnAll();
        }
    }
}
