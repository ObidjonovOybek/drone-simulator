using UnityEngine;

namespace DroneController
{
    [RequireComponent(typeof(Rigidbody))]
    public class DroneMovement : MonoBehaviour
    {
        [Header("Project References")]
        [SerializeField] private DroneMovementData _droneMovementData = default;
        [SerializeField] private Transform _droneObject = default;

        [Header("PS4 Input Names")]
        [SerializeField] private string throttleAxis = "Lift";
        [SerializeField] private string yawAxis = "Yaw";
        [SerializeField] private string pitchAxis = "Pitch";
        [SerializeField] private string rollAxis = "Roll";

        [Header("Keyboard Input")]
        [SerializeField] private bool enableKeyboardInput = true;
        [SerializeField] private float keyboardThrottlePower = 1f;
        [SerializeField] private float keyboardYawPower = 1f;
        [SerializeField] private float keyboardPitchPower = 1f;
        [SerializeField] private float keyboardRollPower = 1f;

        [Header("Axis Fix")]
        [SerializeField] private bool invertPitch = true;
        [SerializeField] private bool invertRoll = false;
        [SerializeField] private float inputDeadZone = 0.35f;

        [Header("Inspector Tuning")]
        [SerializeField] private float upwardForceMultiplier = 1f;
        [SerializeField] private float downwardForceMultiplier = 1f;
        [SerializeField] private float forwardForceMultiplier = 1f;
        [SerializeField] private float sideForceMultiplier = 1f;
        [SerializeField] private float yawSpeedMultiplier = 1f;

        [Header("Visual Tuning")]
        [SerializeField] private float pitchTiltMultiplier = 1f;
        [SerializeField] private float rollTiltMultiplier = 1f;
        [SerializeField] private float tiltSmoothTime = 0.2f;

        [Header("Physics Tuning")]
        [SerializeField] private float maxVelocity = 10f;
        [SerializeField] private float idleSlowDownTime = 0.3f;
        [SerializeField] private float rotationSmoothTime = 0.25f;
        [SerializeField] private float linearDamping = 0.5f;
        [SerializeField] private float angularDamping = 2f;

        private Rigidbody _rigidbody;

        private Vector3 _smoothDampToStopVelocity = default;
        private float _currentRollAmount = default;
        private float _currentRollAmountVelocity = default;
        private float _currentPitchAmount = default;
        private float _currentPitchAmountVelocity = default;
        private float _currentYRotation = default;
        private float _targetYRotation = default;
        private float _targetYRotationVelocity = default;
        private float _currentUpForce = default;

        public float CurrentYRotation => _currentYRotation;
        public Vector3 Velocity => _rigidbody.linearVelocity;

        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        protected virtual void Start()
        {
            SetStartingRotation();

            _rigidbody.useGravity = true;
            _rigidbody.linearDamping = linearDamping;
            _rigidbody.angularDamping = angularDamping;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        protected virtual void FixedUpdate()
        {
            float throttleInput = ApplyDeadZone(Input.GetAxis(throttleAxis));
            float yawInput = ApplyDeadZone(Input.GetAxis(yawAxis));
            float pitchInput = ApplyDeadZone(Input.GetAxis(pitchAxis));
            float rollInput = ApplyDeadZone(Input.GetAxis(rollAxis));

            if (enableKeyboardInput)
            {
                float keyboardThrottle = 0f;
                float keyboardYaw = 0f;
                float keyboardPitch = 0f;
                float keyboardRoll = 0f;

                // W / S = yuqoriga / pastga
                if (Input.GetKey(KeyCode.W)) keyboardThrottle = keyboardThrottlePower;
                else if (Input.GetKey(KeyCode.S)) keyboardThrottle = -keyboardThrottlePower;

                // A / D = yaw chap / yaw o'ng
                if (Input.GetKey(KeyCode.A)) keyboardYaw = -keyboardYawPower;
                else if (Input.GetKey(KeyCode.D)) keyboardYaw = keyboardYawPower;

                // I / K = oldinga / orqaga
                if (Input.GetKey(KeyCode.K)) keyboardPitch = keyboardPitchPower;
                else if (Input.GetKey(KeyCode.I)) keyboardPitch = -keyboardPitchPower;

                // J / L = chap / o'ng
                if (Input.GetKey(KeyCode.J)) keyboardRoll = -keyboardRollPower;
                else if (Input.GetKey(KeyCode.L)) keyboardRoll = keyboardRollPower;

                // Keyboard bosilsa, controller input ustidan yozadi
                if (keyboardThrottle != 0f) throttleInput = keyboardThrottle;
                if (keyboardYaw != 0f) yawInput = keyboardYaw;
                if (keyboardPitch != 0f) pitchInput = keyboardPitch;
                if (keyboardRoll != 0f) rollInput = keyboardRoll;
            }

            if (Mathf.Abs(pitchInput) < 0.2f) pitchInput = 0f;
            if (Mathf.Abs(rollInput) < 0.2f) rollInput = 0f;

            if (invertPitch) pitchInput *= -1f;
            if (invertRoll) rollInput *= -1f;

            ClampingSpeedValues(throttleInput, yawInput, pitchInput, rollInput);

            ThrottleForce(throttleInput);
            RollForce(rollInput);
            YawForce(yawInput);
            PitchForce(pitchInput);

            ApplyForces();
        }

        private float ApplyDeadZone(float value)
        {
            return Mathf.Abs(value) < inputDeadZone ? 0f : value;
        }

        private void SetStartingRotation()
        {
            _targetYRotation = transform.eulerAngles.y;
            _currentYRotation = transform.eulerAngles.y;
        }

        public void ApplyForces()
        {
            _rigidbody.AddRelativeForce(Vector3.up * _currentUpForce, ForceMode.Force);
            _rigidbody.MoveRotation(Quaternion.Euler(0f, _currentYRotation, 0f));

            if (_droneObject != null)
            {
                _droneObject.localRotation = Quaternion.Euler(
                    _currentPitchAmount,
                    0f,
                    -_currentRollAmount
                );
            }
        }

        public void ClampingSpeedValues(float throttleInput, float yawInput, float pitchInput, float rollInput)
        {
            _rigidbody.linearVelocity = Vector3.ClampMagnitude(
                _rigidbody.linearVelocity,
                maxVelocity
            );

            bool isIdle =
                Mathf.Abs(throttleInput) < 0.01f &&
                Mathf.Abs(yawInput) < 0.01f &&
                Mathf.Abs(pitchInput) < 0.01f &&
                Mathf.Abs(rollInput) < 0.01f;

            if (isIdle)
            {
                _rigidbody.linearVelocity = Vector3.SmoothDamp(
                    _rigidbody.linearVelocity,
                    Vector3.zero,
                    ref _smoothDampToStopVelocity,
                    idleSlowDownTime
                );
            }
        }

        public void ThrottleForce(float throttleInput)
        {
            float upwardForce = _droneMovementData.UpwardMovementForce * upwardForceMultiplier;
            float downwardForce = _droneMovementData.DownwardMovementForce * downwardForceMultiplier;

            float forceValue =
                (throttleInput > 0f) ? upwardForce :
                (throttleInput < 0f) ? downwardForce :
                0f;

            _currentUpForce = _rigidbody.mass * 9.81f + throttleInput * forceValue;
        }

        public void RollForce(float rollInput)
        {
            float sideForce = _droneMovementData.SidewardMovementForce * sideForceMultiplier;

            _rigidbody.AddRelativeForce(
                Vector3.right * rollInput * sideForce,
                ForceMode.Force
            );

            _currentRollAmount = Mathf.SmoothDamp(
                _currentRollAmount,
                (_droneMovementData.MaximumRollAmount * rollTiltMultiplier) * rollInput,
                ref _currentRollAmountVelocity,
                tiltSmoothTime
            );
        }

        public void YawForce(float yawInput)
        {
            float yawSpeed = _droneMovementData.MaximumYawSpeed * yawSpeedMultiplier;

            _targetYRotation += yawInput * yawSpeed * Time.fixedDeltaTime * 100f;

            _currentYRotation = Mathf.SmoothDamp(
                _currentYRotation,
                _targetYRotation,
                ref _targetYRotationVelocity,
                rotationSmoothTime
            );
        }

        public void PitchForce(float pitchInput)
        {
            float forwardForce = _droneMovementData.ForwardMovementForce * forwardForceMultiplier;

            _rigidbody.AddRelativeForce(
                Vector3.forward * pitchInput * forwardForce,
                ForceMode.Force
            );

            _currentPitchAmount = Mathf.SmoothDamp(
                _currentPitchAmount,
                (_droneMovementData.MaximumPitchAmount * pitchTiltMultiplier) * pitchInput,
                ref _currentPitchAmountVelocity,
                tiltSmoothTime
            );
        }
    }
}