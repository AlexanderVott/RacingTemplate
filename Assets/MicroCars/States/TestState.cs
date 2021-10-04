using System;
using MicroRace.DB;
using MicroRace.Managers;
using MicroRace.Vehicles;
using RedDev.Kernel.FMS.Globals;
using RedDev.Kernel.Managers;
using UnityEngine;

namespace MicroRace.States {
    public class TestState : GlobalBaseState {
        private DBManager _dbManager;
        private CoreScenesManager _scenes;

        private VehiclesManager _vehicleManager;

        [SerializeField] private string _vehicleIdentifier = "TestCar";

        private VehicleGameController vehicle;

        public override void OnInitialize() {
            base.OnInitialize();

            _dbManager = Core.Get<DBManager>();
            _scenes = Core.Get<CoreScenesManager>();
        }

        public override void OnEnter() {
            base.OnEnter();

            /*if (!Application.isEditor)
                _scenes.To("CarTester");*/

            GetManagers();
            InitializeVehicle();
            InitializeCamera();
        }

        private void GetManagers() {
            _vehicleManager = Core.Get<VehiclesManager>();
        }

        private void InitializeVehicle() {
            vehicle = FindVehicleInScene();

            if (vehicle == null && !String.IsNullOrEmpty(_vehicleIdentifier)) {
                LoadVehicleFromDB();
            }
            else {
                _vehicleManager.RegisterVehiclePlayer(vehicle);
            }
        }

        private void InitializeCamera() {
            if (vehicle != null) {
                var camerasMan = Core.Get<CamerasManager>();
                camerasMan.SpawnOnCameraVehicle(vehicle);
            }
        }

        private VehicleGameController FindVehicleInScene() {
            return FindObjectOfType<VehicleGameController>();
        }

        private void LoadVehicleFromDB() {
            var vehDB = _dbManager.Load<VehiclesDBMeta>();
            var vehRecordDB = vehDB.Get(_vehicleIdentifier);
            if (vehRecordDB == null) {
                Prod.LogError($"[{nameof(TestState)}] Not found record DB with identifier {_vehicleIdentifier}");
                return;
            }
            vehicle = _vehicleManager.SpawnVehicle(vehRecordDB.Id);
        }

        public override void OnExit() {
            base.OnExit();
        }

        public override void OnPause() {
            base.OnPause();
        }

        public override void OnResume() {
            base.OnResume();
        }
    }
}