using System;
using Cinemachine;
using Game.Vehicles;
using RedDev.Helpers.Extensions;
using UnityEngine;

namespace Game.Cameras {
    public class CameraController : MonoBehaviour {
        public Camera CameraLink { get; private set; }
        public CinemachineBrain CineBrain { get; private set; }
        public CinemachineVirtualCamera CineCamera { get; private set; }

        public Action<VehicleGameController, VehicleGameController> onChangedTarget;

        [SerializeField] private VehicleGameController target = null;
        public VehicleGameController Target {
            get => target;
            set {
                onChangedTarget.SafeCall(target, value);
                target = value;
            }
        }

        private Rect rect = new Rect(Vector2.zero, Vector2.one);

        void Awake() {
            CameraLink = GetComponent<Camera>();
            CineBrain = GetComponent<CinemachineBrain>();
            CineCamera = GetComponent<CinemachineVirtualCamera>();
        }

        public void SetupCamera(Rect rect) {
            this.rect = rect;
            if (CameraLink != null)
                CameraLink.rect = rect;
        }
    }
}