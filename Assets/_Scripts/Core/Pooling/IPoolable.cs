namespace Core.Pooling
{
    public interface IPoolable
    {
        void OnSpawn();
        void OnDespawn();
        void SetPool<T>(ObjectPool<T> pool) where T : UnityEngine.MonoBehaviour;
    }
}