using Cinemachine;
using MicroRace.Vehicles;
using UnityEngine;

namespace MicroRace.Cameras {
    [RequireComponent(typeof(CameraController), typeof(CinemachineVirtualCamera))]
    public class VehicleLookAtCameraComponent : MonoBehaviour {
        private CameraController cameraController;
        private CinemachineVirtualCamera virtualCam;

        private void Start() {
            cameraController = GetComponent<CameraController>();
            virtualCam = GetComponent<CinemachineVirtualCamera>();
            cameraController.onChangedTarget += OnChangedTargetCamera;
            virtualCam.LookAt = cameraController.Target != null ? cameraController.Target.transform : null;
            OnChangedTargetCamera(null, cameraController.Target);
        }

        private void OnChangedTargetCamera(VehicleGameController oldTarget, VehicleGameController newTarget) {
            virtualCam.LookAt = newTarget != null ? newTarget.transform : null;
            if (oldTarget != null
                && newTarget == null
                && oldTarget.Data != null
                && oldTarget.Data.LookAtCamera == this.cameraController)
                oldTarget.Data.LookAtCamera = null;
            if (newTarget != null
                && newTarget.Data.LookAtCamera != this.cameraController)
                newTarget.Data.LookAtCamera = this.cameraController;
        }
    }
}