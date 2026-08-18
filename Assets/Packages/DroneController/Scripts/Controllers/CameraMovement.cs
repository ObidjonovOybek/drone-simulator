using DroneController;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Project References:")]
    [SerializeField] private CameraMovementData _cameraMovementData = default;

    [Header("Scene References:")]
    [SerializeField] private Transform _objecToFollow = default;

    private Vector3 _positionVelocity;
    private float _cameraTiltRotation;
    private float _previousFrameCameraPosition;

    private void OnEnable()
    {
        _positionVelocity = Vector3.zero;
        _cameraTiltRotation = 0f;
        _previousFrameCameraPosition = transform.position.y;
    }

    public void SetFollowTarget(Transform target)
    {
        _objecToFollow = target;
        _positionVelocity = Vector3.zero;
        _cameraTiltRotation = 0f;
        _previousFrameCameraPosition = transform.position.y;
    }

    private void FixedUpdate()
    {
        if (_cameraMovementData == null || _objecToFollow == null)
            return;

        FollowDroneMethod();
        TiltCameraUpDown();
        ApplyCameraRotation();
    }

    private void FollowDroneMethod()
    {
        transform.position = Vector3.SmoothDamp(
            transform.position,
            _objecToFollow.TransformPoint(_cameraMovementData.Offset),
            ref _positionVelocity,
            _cameraMovementData.FollowSpeed);
    }

    private void TiltCameraUpDown()
    {
        _cameraTiltRotation = Mathf.Lerp(
            _cameraTiltRotation,
            (transform.position.y - _previousFrameCameraPosition) * -_cameraMovementData.YFollowStrength,
            Time.deltaTime * 10f);

        _previousFrameCameraPosition = transform.position.y;
    }

    private void ApplyCameraRotation()
    {
        transform.rotation = Quaternion.Euler(
            14f + _cameraTiltRotation,
            _objecToFollow.rotation.eulerAngles.y,
            0f);
    }
}