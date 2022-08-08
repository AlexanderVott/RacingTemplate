using System.Collections.Generic;
using Game.Cameras;
using Game.Vehicles;
using RedDev.Kernel.Managers;
using UnityEngine;

namespace Game.States {
    public class CamerasManager : BaseManager {
        private readonly string CamerasPrefabsPath = $"Prefabs/Cameras/";

        private readonly List<CameraController> vehiclesCameras = new List<CameraController>();

        public override void Attach() {
            base.Attach();
        }

        public void ClearVehicleCameras() {
            foreach (var itr in vehiclesCameras)
                Destroy(itr.gameObject);
            vehiclesCameras.Clear();
        }

        public CameraController SpawnOnCameraVehicle(VehicleGameController vehicleTarget) {
            var result = FindFreeCamera();
            if (result != null)
                return result;

            result = InstantiateCamera("VehicleCamera");
            if (result != null) {
                result.SetupCamera(new Rect(Vector2.zero, Vector2.one));
                result.Target = vehicleTarget;
            }
            vehiclesCameras.Add(result);
            return result;
        }

        private CameraController FindFreeCamera() {
            if (vehiclesCameras.Count <= 0)
                return null;

            foreach (var itr in vehiclesCameras) {
                if (itr.Target == null)
                    return itr;
            }

            return null;
        }

        private CameraController InstantiateCamera(string namePrefab) {
            CameraController result = null;
            if (!string.IsNullOrEmpty(namePrefab)) {
                var prefab = Resources.Load(CamerasPrefabsPath + namePrefab);
                var gobj = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
                result = gobj.GetComponent<CameraController>();
            }
            else {
                Prod.LogError($"[{nameof(CamerasManager)}] Name prefab is empty");
            }

            return result;
        }
    }
}