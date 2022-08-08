using RVP;
using UnityEngine;

namespace Game.Vehicles {
    [RequireComponent(typeof(VehicleGameController), typeof(VehicleParent))]
    public class VehicleInputController : MonoBehaviour {
        private VehicleGameController vehicleController;
        private VehicleParent vehicle;

        private void Awake() {
            vehicleController = GetComponent<VehicleGameController>();
            vehicle = GetComponent<VehicleParent>();
        }

        private void Update() {
            var input = vehicleController.inputData;
            vehicle.SetAccel(input.accel);
            vehicle.SetBrake(input.brake);
            vehicle.SetSteer(input.steer);
            vehicle.SetEbrake(input.handbrake);
            vehicle.SetBoost(input.boost);
            vehicle.SetUpshift(input.shiftUp);
            vehicle.SetDownshift(input.shiftDown);
        }
    }
}