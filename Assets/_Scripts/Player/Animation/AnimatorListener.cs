using UnityEngine;

public class AnimatorListener : MonoBehaviour
{
    private Animator _playerAnimator;
    private PlayerMovement _targetMovement;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");

    private void Awake()
    {
        if (TryGetComponent(out Animator playerAnimator) == false)
        {
            Debug.LogError("Компонент Animator отсутствует на объекте AnimatorListener.", this);

            return;
        }

        _playerAnimator = playerAnimator;
    }

    private void OnEnable()
    {
        if (TryGetComponent(out PlayerMovement targetMovement) == false)
        {
            Debug.LogError("Компонент PlayerMovement отсутствует на объекте AnimatorListener. Анимация не будет обновляться.", this);

            return;
        }

        _targetMovement = targetMovement;

        _targetMovement.HorizontalSpeedChanged += HandleHorizontalSpeedChanged;

        _targetMovement.IsGroundedStateChanged += HandleGroundedStateChanged;
    }

    private void OnDisable()
    {
        if (_targetMovement != null)
        {
            _targetMovement.HorizontalSpeedChanged -= HandleHorizontalSpeedChanged;

            _targetMovement.IsGroundedStateChanged -= HandleGroundedStateChanged;
        }
    }

    private void HandleHorizontalSpeedChanged(float currentSpeed)
    {
        _playerAnimator.SetFloat(SpeedHash, currentSpeed);
    }

    private void HandleGroundedStateChanged(bool isGrounded)
    {
        _playerAnimator.SetBool(IsGroundedHash, isGrounded);
    }
}