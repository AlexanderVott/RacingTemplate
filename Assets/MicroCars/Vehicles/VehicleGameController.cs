using System;
using MicroRace.Cameras;
using MicroRace.DB;
using MicroRace.Prefs;
using RVP;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MicroRace.Vehicles {
    [Serializable]
    public class InputData {
        public float accel;
        public float brake;

        public float steer;

        public float clutch;
        public float handbrake;

        public float shiftUp;
        public float shiftDown;

        public bool boost;

        public bool horn;

        public bool activate;
        public bool flipOver;
    }

    [Serializable]
    public class PlayerData {
        [ReadOnly] public int idPlayer = 0;
        [ReadOnly] public int vehicleId = 0;
    }

    [Serializable]
    public class VehicleData {
        [ShowInInspector, ReadOnly] private VehiclesDBMeta meta;
        public VehiclesDBMeta Meta {
            get => meta;
            set => meta = value;
        }
        
        [ShowInInspector, ReadOnly] private VehicleVisualSettings visualSettings;
        public VehicleVisualSettings VisualSettings {
            get => visualSettings;
            set => visualSettings = value;
        }

        [ShowInInspector, ReadOnly] private CameraController lookAtCamera;
        public CameraController LookAtCamera {
            get => lookAtCamera;
            set => lookAtCamera = value;
        }
    }

    [RequireComponent(typeof(VehicleParent))]
    public class VehicleGameController : BaseVehicleController {
        
        [FoldoutGroup("Data", expanded:false), ShowInInspector, ReadOnly] private VehicleData data = new VehicleData();
        public VehicleData Data => data;

        [FoldoutGroup("Data", expanded:false), ShowInInspector, ReadOnly] private VehicleParent controller;
        public VehicleParent Controller => controller;

        [FoldoutGroup("Player data", expanded:false), ShowInInspector, ReadOnly] private PlayerData playerData = new PlayerData();
        public PlayerData PlayerData => playerData;

        [FoldoutGroup("Input data", expanded: false), ShowInInspector, ReadOnly] public InputData inputData = new InputData();
        public InputData InputData => inputData;

        public Rigidbody Body { get; private set; }

        public Action onVisualPartsChanged;

        private void Awake() {
            controller = GetComponent<VehicleParent>();
            Body = GetComponent<Rigidbody>();
        }

        private void Start() { }
    }
}