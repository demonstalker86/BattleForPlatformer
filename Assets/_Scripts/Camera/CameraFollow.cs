using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _followSpeed = 15f;
    [SerializeField] private float _verticalOffset = 2f;
    [SerializeField] private float _cameraDepth = -10f;
    [SerializeField] private float _zeroOffset = 0f;

    private Vector3 _currentOffset;

    private void Start()
    {
        if (_target == null)
        {
            return;
        }

        _currentOffset = new Vector3(_zeroOffset, _verticalOffset, _cameraDepth);
    }

    private void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }

        Vector3 targetPosition = _target.position + _currentOffset;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, _followSpeed * Time.deltaTime);
    }
}