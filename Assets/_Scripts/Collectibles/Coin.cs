using UnityEngine;
using Core.Pooling;
using Core.Utilities;
using Cysharp.Threading.Tasks;

public class Coin : MonoBehaviour, IPoolable
{
    [SerializeField] private float _lifeTime = 3f;
    private Rigidbody2D _rb;
    private ObjectPool<Coin> _pool;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void SetPool<T>(ObjectPool<T> pool) where T : MonoBehaviour
    {
        _pool = pool as ObjectPool<Coin>;
    }

    public void OnSpawn()
    {
        _rb.linearVelocity = Random.insideUnitCircle * 2f;
        _rb.angularVelocity = Random.Range(-10f, 10f);
        this.DespawnAfter(_lifeTime, _pool).Forget();
    }

    public void OnDespawn()
    {
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;
        transform.rotation = Quaternion.identity;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        if (_pool != null)
        {
            _pool.Despawn(this);
        }
        else
        {
            Debug.LogError("Coin: pool not set, destroying.");
            Destroy(gameObject);
        }
    }
}