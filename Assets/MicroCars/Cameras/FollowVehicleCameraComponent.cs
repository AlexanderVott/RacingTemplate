using UnityEngine;

namespace MicroRace.Cameras {
    public class FollowVehicleCameraComponent : MonoBehaviour {
        [SerializeField] private bool _shouldRotate = true;

        [SerializeField] private float _distanceForward = 0.0f;
        [SerializeField] private float _distance = 10.0f;
        [SerializeField] private float _height = 5.0f;
        [SerializeField] private float _heightDamping = 2.0f;
        [SerializeField] private float _rotationDamping = 3.0f;
        [SerializeField] private float _targetDamping = 3.0f;
        [SerializeField] private float _heightOffset = 0.0f;

        private float _wantedRotationAngle;
        private float _wantedHeight;
        private float _currentRotationAngle;
        private float _currentHeight;

        private Vector3 _currentTargetPosition;
        private Vector3 _wantedTargetPosition;

        private Quaternion _currentRotation;

        private CameraController _vehicleCamera;

        private void Start() {
            _vehicleCamera = GetComponent<CameraController>();
        }

        private void Update() { 
            if (_vehicleCamera == null || _vehicleCamera.Target == null)
                return;

            var target = _vehicleCamera.Target;
            var targetTransform = target.transform;

            _wantedRotationAngle = targetTransform.eulerAngles.y;
            _wantedHeight = targetTransform.position.y + _height;
            _currentRotationAngle = transform.eulerAngles.y;
            _currentHeight = transform.position.y;

            _currentRotationAngle = Mathf.LerpAngle(_currentRotationAngle, _wantedRotationAngle, _rotationDamping * Time.deltaTime);
            _currentHeight = Mathf.Lerp(_currentHeight, _wantedHeight, _heightDamping * Time.deltaTime);
            _currentRotation = Quaternion.Euler(0, _currentRotationAngle, 0);

            transform.position = targetTransform.position;
            transform.position -= _currentRotation * Vector3.forward * _distance;

            transform.position = new Vector3(transform.position.x, _currentHeight, transform.position.z);

            if (_shouldRotate) {
                _wantedTargetPosition = targetTransform.position +
                                        (Vector3.up * _heightOffset) +
                                        (targetTransform.forward * _distanceForward);
                _currentTargetPosition = Vector3.Lerp(_currentTargetPosition, _wantedTargetPosition, _targetDamping * Time.deltaTime);
                transform.LookAt(_currentTargetPosition);
            }
        }
    }
}