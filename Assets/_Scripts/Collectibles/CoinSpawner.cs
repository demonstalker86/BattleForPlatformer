using UnityEngine;
using Core.Pooling;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CoinSpawner : Spawner<Coin>
{
    [SerializeField] private float _spawnInterval = 1.5f;
    [SerializeField] private float _spawnRadius = 5f;
    [SerializeField] private float _spawnHeight = 2f;
    [SerializeField] private bool _autoSpawnOnStart = true;

    private CancellationTokenSource _cancellationTokenSource;

    private void Start()
    {
        if (_autoSpawnOnStart)
        {
            StartAutoSpawn();
        }
    }

    public void StartAutoSpawn()
    {
        StopAutoSpawn();
        _cancellationTokenSource = new CancellationTokenSource();
        AutoSpawnLoop(_cancellationTokenSource.Token).Forget();
    }

    public void StopAutoSpawn()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }

    private async UniTask AutoSpawnLoop(CancellationToken token)
    {
        while (token.IsCancellationRequested == false)
        {
            Vector2 randomCircle = Random.insideUnitCircle * _spawnRadius;
            Vector3 spawnPos = new Vector3(randomCircle.x, _spawnHeight, randomCircle.y);

            Spawn(spawnPos, Quaternion.identity);

            await UniTask.Delay(System.TimeSpan.FromSeconds(_spawnInterval), cancellationToken: token);
        }
    }

    private void OnDestroy()
    {
        StopAutoSpawn();
    }
}