using Cinemachine;
using MicroRace.Vehicles;
using UnityEngine;

namespace MicroRace.Cameras {
    [RequireComponent(typeof(CameraController), typeof(CinemachineVirtualCamera))]
    public class VehicleFollowCameraComponent : MonoBehaviour {
        private CameraController cameraController;
        private CinemachineVirtualCamera virtualCamera;

        private void Start() {
            cameraController = GetComponent<CameraController>();
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            cameraController.onChangedTarget += OnChangedTargetCamera;
            OnChangedTargetCamera(null, cameraController.Target);
        }

        private void OnChangedTargetCamera(VehicleGameController oldTarget, VehicleGameController newTarget) {
            virtualCamera.Follow = newTarget != null ? newTarget.transform : null;
        }
    }
}