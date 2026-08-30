using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 8f;
    [SerializeField] private float _acceleration = 30f;
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private float _inputDeadZone = 0f;
    [SerializeField] private float _speedEventThreshold = 0.01f;
    [SerializeField] private float _coyoteTime = 0.1f;

    private float _coyoteTimer;
    private Rigidbody2D _targetRigidbody2D;
    private bool _isGrounded;
    private bool _wasGrounded;
    private float _currentHorizontalInput;
    private float _previousHorizontalSpeed;

    public event Action<float> HorizontalSpeedChanged;
    public event Action<bool> IsGroundedStateChanged;

    private void Start()
    {
        if (TryGetComponent(out Rigidbody2D targetRigidbody2D) == false)
        {
            Debug.LogError("Компонент Rigidbody2D отсутствует на объекте PlayerMovement. Игрок не сможет двигаться.", this);

            return;
        }

        _targetRigidbody2D = targetRigidbody2D;
    }

    private void Update()
    {
        HandleJumpInput();

        CheckGroundState();        

        PublishSpeedData();
    }

    private void FixedUpdate()
    {
        ApplyHorizontalMovement();
    }

    private void CheckGroundState()
    {
        _isGrounded = Physics2D.OverlapCircle(_groundCheckPoint.position, _groundCheckRadius, _groundLayerMask);

        if (_isGrounded)
        {
            _coyoteTimer = _coyoteTime;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;
        }

        if (_isGrounded != _wasGrounded && _isGrounded == false && _coyoteTimer <= 0f)
        {
            IsGroundedStateChanged?.Invoke(false);

            _wasGrounded = false;
        }
        else if (_isGrounded != _wasGrounded && _isGrounded == true)
        {
            IsGroundedStateChanged?.Invoke(true);

            _wasGrounded = true;
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            Vector2 currentVelocity = _targetRigidbody2D.linearVelocity;
            _targetRigidbody2D.linearVelocity = new Vector2(currentVelocity.x, _jumpForce);

            _isGrounded = false;
            _wasGrounded = true;

            IsGroundedStateChanged?.Invoke(false);
        }
    }

    private void ApplyHorizontalMovement()
    {
        _currentHorizontalInput = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(_currentHorizontalInput) > _inputDeadZone)
        {
            float direction = Mathf.Sign(_currentHorizontalInput);

            Vector3 targetScale = transform.localScale;

            targetScale.x = Mathf.Abs(targetScale.x) * direction;

            transform.localScale = targetScale;
        }

        float targetSpeed = _currentHorizontalInput * _moveSpeed;
        Vector2 currentVelocity = _targetRigidbody2D.linearVelocity;

        float newHorizontalSpeed = Mathf.MoveTowards(currentVelocity.x, targetSpeed, _acceleration * Time.fixedDeltaTime);

        _targetRigidbody2D.linearVelocity = new Vector2(newHorizontalSpeed, currentVelocity.y);
    }

    private void PublishSpeedData()
    {
        float currentHorizontalSpeed = Mathf.Abs(_currentHorizontalInput * _moveSpeed);

        if (Mathf.Abs(currentHorizontalSpeed - _previousHorizontalSpeed) > _speedEventThreshold)
        {
            HorizontalSpeedChanged?.Invoke(currentHorizontalSpeed);

            _previousHorizontalSpeed = currentHorizontalSpeed;
        }
    }
}