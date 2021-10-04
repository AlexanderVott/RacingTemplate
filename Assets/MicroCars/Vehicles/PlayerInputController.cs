using MicroCars.Inputs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MicroRace.Vehicles {
    [RequireComponent(typeof(VehicleGameController))]
    public class PlayerInputController : MonoBehaviour {
        [ShowInInspector, ReadOnly] private int playerInputId = 0;
        public int PlayerInputId => playerInputId;

        private VehicleGameController controller;
        private MicroCarsInputsActions actions;
        private MicroCarsInputsActions.GameplayActions gameplay;

        private void Awake() {
            controller = GetComponent<VehicleGameController>();
            actions = new @MicroCarsInputsActions();
            gameplay = actions.Gameplay;
        }

        private void OnEnable() {
            gameplay.Enable();
        }

        private void OnDisable() {
            gameplay.Disable();
        }

        private void Update() {
            controller.inputData.accel = gameplay.Accel.ReadValue<float>();
            controller.inputData.brake = gameplay.Brake.ReadValue<float>();
            controller.inputData.steer = gameplay.Steer.ReadValue<float>();
            controller.inputData.handbrake = gameplay.HandBrake.ReadValue<float>();
            controller.inputData.boost = gameplay.Boost.ReadValue<float>() > 0.5f;
        }
    }
}