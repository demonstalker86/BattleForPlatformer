using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class BanditPatrol : MonoBehaviour
{
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _stopDistance = 0.1f;
    [SerializeField] private float _minPause = 4f;
    [SerializeField] private float _maxPause = 5f;

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private int _currentIndex;
    private CancellationTokenSource _cts;

    private void Awake()
    {
        if (TryGetComponent(out _rigidbody) == false)
        {
            Debug.LogError("BanditPatrol: Rigidbody2D not found.", this);
            return;
        }

        _animator = GetComponent<Animator>();
        if (_waypoints == null || _waypoints.Length == 0)
        {
            Debug.LogWarning("BanditPatrol: no waypoints, patrol disabled.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        _cts = new CancellationTokenSource();
        PatrolLoop(_cts.Token).Forget();
    }

    private void OnDisable()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private async UniTaskVoid PatrolLoop(CancellationToken token)
    {
        while (token.IsCancellationRequested == false)
        {
            if (_waypoints.Length == 0)
            {
                return;
            }

            Transform target = _waypoints[_currentIndex];
            await MoveToTarget(target, token);

            if (token.IsCancellationRequested)
            {
                return;
            }

            float pauseDuration = Random.Range(_minPause, _maxPause);
            SetAnimatorSpeed(0f);
            _rigidbody.linearVelocity = Vector2.zero;
            await UniTask.Delay((int)(pauseDuration * 1000), cancellationToken: token);

            if (token.IsCancellationRequested)
            {
                return;
            }

            _currentIndex = (_currentIndex + 1) % _waypoints.Length;
        }
    }

    private async UniTask MoveToTarget(Transform target, CancellationToken token)
    {
        Vector2 targetPosition = target.position;
        float direction = Mathf.Sign(targetPosition.x - transform.position.x);

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;

        SetAnimatorSpeed(_moveSpeed);

        while (token.IsCancellationRequested == false)
        {
            Vector2 currentPos = transform.position;
            float distance = Vector2.Distance(currentPos, targetPosition);

            if (distance <= _stopDistance)
            {
                _rigidbody.linearVelocity = Vector2.zero;
                return;
            }

            Vector2 newVelocity = (targetPosition - currentPos).normalized * _moveSpeed;
            newVelocity.y = _rigidbody.linearVelocity.y;
            _rigidbody.linearVelocity = newVelocity;

            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
        }
    }

    private void SetAnimatorSpeed(float speed)
    {
        if (_animator != null)
        {
            _animator.SetFloat("Speed", speed);
        }
    }
}