using System;
using System.Collections.Generic;
using System.IO;
using Game.DB;
using Game.Vehicles;
using RedDev.Helpers.Extensions;
using RedDev.Kernel.Managers;
using UnityEngine;

namespace Game.Managers
{
	public enum TuningType
	{
		None = -1,
		Engine = 0,
		Steering = 1,
		Transmission = 2,
		Brakes = 3,
		Axles = 4,
		Wheels = 5
	}

	public class VehiclesManager : BaseManager
	{
		private DBManager _dbManager;
		private VehicleVisualManager _vehicleVisual;
		//private TrackConfigManager _trackConfig;
		private PrefsManager _prefs;

		private readonly string m_vehiclesPrefabsPath = "Prefabs" + Path.AltDirectorySeparatorChar +
														"Vehicles" + Path.AltDirectorySeparatorChar;

		private int _maxPlayerId = 0;
		public readonly List<VehicleGameController> vehicles = new List<VehicleGameController>();

        public Action<VehicleGameController> onRegisteredVehicle;

        public override void Attach()
		{
			base.Attach();

            //_dbManager = Core.Get<DBManager>();
            //_vehicleVisual = Core.Get<VehicleVisualManager>();
            //_prefs = Core.Get<PrefsManager>();
        }

        public void ClearVehicles()
		{
			foreach (var vehicle in vehicles)
			{
				//if (veh.gameObject.layer == LOCAL_CAR)
				Destroy(vehicle.gameObject);
			}
			vehicles.Clear();
			_maxPlayerId = 0;
		}

		public VehicleGameController InstantiateVehicle(int carId, VehicleVisual overrideVisual = null, int layer = -1)
		{
			var vehicleMeta = _dbManager.Get<VehiclesDBMeta>(carId);
			var result = InstantiateVehiclePrefab(vehicleMeta, layer);

			ApplyVisual(result, overrideVisual);
			//TODO: загрузка всегда дефолтного тюнинга, необходимо пробросить префсы и предусмотреть провал применения и взятие дефолтного тюнинга
			ApplyCustomTuning(result, vehicleMeta.engines.defaultValue, TuningType.Engine);
			ApplyCustomTuning(result, vehicleMeta.steering.defaultValue, TuningType.Steering);
			ApplyCustomTuning(result, vehicleMeta.transmissions.defaultValue, TuningType.Transmission);
			ApplyCustomTuning(result, vehicleMeta.brakes.defaultValue, TuningType.Brakes);
			ApplyCustomTuning(result, vehicleMeta.axles.defaultValue, TuningType.Axles);
			ApplyCustomTuning(result, vehicleMeta.wheels.defaultValue, TuningType.Wheels);
			result.onVisualPartsChanged.SafeCall();

			Dev.Log($"[{nameof(VehiclesManager)}] Vehicle {_maxPlayerId} is spawned");

			RegisterVehiclePlayer(result);
			
			return result;
		}



        #region SpawnVehicle
        public VehicleGameController SpawnVehicle(int vehicleId, int layer = -1) {
            var vehicleMeta = _dbManager.Get<VehiclesDBMeta>(vehicleId);
            var result = InstantiateVehiclePrefab(vehicleMeta, layer);

            Dev.Log($"[{nameof(VehiclesManager)}] Vehicle {_maxPlayerId} is spawned");

            RegisterVehiclePlayer(result);

            return result;
        }

        public void RegisterVehiclePlayer(VehicleGameController vehicle) {
            if (vehicle.PlayerData.idPlayer >= 0)
                return;
            vehicle.PlayerData.idPlayer = _maxPlayerId;
            _maxPlayerId++;
            vehicles.Add(vehicle);
            onRegisteredVehicle.SafeCall(vehicle);
        }
        #endregion

        private VehicleGameController InstantiateVehiclePrefab(VehiclesDBMeta meta, int layer = -1)
		{
            VehicleGameController result = null;
			if (!String.IsNullOrEmpty(meta.prefabName))
			{
				var prefab = Resources.Load(m_vehiclesPrefabsPath + meta.prefabName);
				var gobj = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
				result = gobj.GetComponent<VehicleGameController>();
				if (result != null)
				{
					result.Data.Meta = meta;
					result.PlayerData.vehicleId = meta.Id;
				}
				if (layer != -1)
					gobj.layer = layer;
			}
			else
                Prod.LogError($"[VehiclesManager] Path to prefab for {meta.Id} is empty");

			return result;
		}

		public VehicleGameController GetVehicle(int idPlayer)
		{
			foreach (var vehicle in vehicles)
				if (vehicle.PlayerData.idPlayer == idPlayer)
					return vehicle;
			return null;
		}

		#region Visual
		public void ApplyVisual(VehicleGameController vehicle, VehicleVisual overrideVisual = null, int idChassis = -1)
		{
			var vehId = vehicle.PlayerData.vehicleId;
			var visual = overrideVisual == null ? _dbManager.Get<CustomizeDBMeta>(vehId)?.root : overrideVisual;
			
			_vehicleVisual.ApplyVisual(vehicle, visual);
		}

		public void ChangeVisual(VehicleGameController vehicle, int idChassis = -1)
		{
			var vehId = vehicle.PlayerData.vehicleId;
			var visual = _dbManager.Get<CustomizeDBMeta>(vehId)?.root;

			_vehicleVisual.ChangeChassis(vehicle, visual, idChassis);
			vehicle.onVisualPartsChanged.SafeCall();
		}
		#endregion

		#region Tuning
		public T GetTuning<T>(int idDB) where T : BaseTuningDBMeta
		{
			return _dbManager.Get<T>(idDB);
		}

		public void ApplyCustomTuning(VehicleGameController vehicle, int idDB, TuningType tuningType)
		{
			/*var controller = vehicle.controller;
			if (controller == null)
			{
                Prod.LogError($"[VehiclesManager] Not found controller for vehicle {vehicle.gameObject.name}");
				return;
			}
			bool isNotFound = false;
			switch (tuningType)
			{
				case TuningType.Engine:
					var engine = _dbManager.Get<EnginesDBMeta>(idDB);
					if (engine != null)
					{
						if (vehicle.centerOfMass != null)
							vehicle.centerOfMass.centerOfMassOffset = engine.centerOfMass;
						engine.dataEngine.Apply(controller);
					}
					else
						isNotFound = true;
					break;
				case TuningType.Steering:
					var steering = _dbManager.Get<SteeringDBMeta>(idDB);
					if (steering != null)
						steering.dataSteering.Apply(controller);
					else
						isNotFound = true;
					break;
				case TuningType.Transmission:
					var transmission = _dbManager.Get<TransmissionsDBMeta>(idDB);
					if (transmission != null)
						transmission.dataTransmission.Apply(controller);
					else
						isNotFound = true;
					break;
				case TuningType.Brakes:
					var brakes = _dbManager.Get<BrakesDBMeta>(idDB);
					if (brakes != null)
						brakes.dataBrakes.Apply(controller);
					else
						isNotFound = true;
					break;
				case TuningType.Axles:
					var axles = _dbManager.Get<AxlesDBMeta>(idDB);
					if (axles != null)
						axles.dataAxles.Apply(controller);
					else
						isNotFound = true;
					break;
				case TuningType.Wheels:
					var wheels = _dbManager.Get<WheelsDBMeta>(idDB);
					if (wheels != null)
						wheels.dataWheels.Apply(controller);
					else
						isNotFound = true;
					break;
				default:
					LogSystem.Print(AlertLevel.Notify, "VehiclesManager", $"Tuning type is not defined for {tuningType}");
					break;
			}
			if (isNotFound)
				LogSystem.Print(AlertLevel.Error, "VehiclesManager", $"Not found {tuningType} tuning in db with id = {idDB}");
#if UNITY_EDITOR
			else
				if (tuningType != TuningType.None)
					vehicle.idsTuning[(int)tuningType] = idDB;
#endif*/
		}

		public void ApplyCustomTuning(VehicleGameController vehicle, string identifier, TuningType tuningType)
		{
			/*var controller = vehicle.controller;
			if (controller == null)
			{
				LogSystem.Print(AlertLevel.Error, "VehiclesManager", $"Not found controller for vehicle {vehicle.gameObject.name}");
				return;
			}
			bool isNotFound = false;
			var idDB = -1;
			switch (tuningType)
			{
				case TuningType.Engine:
					var engine = _dbManager.Get<EnginesDBMeta>(identifier);
					if (engine != null)
					{
						engine.dataEngine.Apply(controller);
						idDB = engine.id;
					}
					else
						isNotFound = true;
					break;
				case TuningType.Steering:
					var steering = _dbManager.Get<SteeringDBMeta>(identifier);
					if (steering != null)
					{
						steering.dataSteering.Apply(controller);
						idDB = steering.id;
					}
					else
						isNotFound = true;
					break;
				case TuningType.Transmission:
					var transmission = _dbManager.Get<TransmissionsDBMeta>(identifier);
					if (transmission != null)
					{
						transmission.dataTransmission.Apply(controller);
						idDB = transmission.id;
					}
					else
						isNotFound = true;
					break;
				case TuningType.Brakes:
					var brakes = _dbManager.Get<BrakesDBMeta>(identifier);
					if (brakes != null)
					{
						brakes.dataBrakes.Apply(controller);
						idDB = brakes.id;
					}
					else
						isNotFound = true;
					break;
				case TuningType.Axles:
					var axles = _dbManager.Get<AxlesDBMeta>(identifier);
					if (axles != null)
					{
						axles.dataAxles.Apply(controller);
						idDB = axles.id;
					}
					else
						isNotFound = true;
					break;
				case TuningType.Wheels:
					var wheels = _dbManager.Get<WheelsDBMeta>(identifier);
					if (wheels != null)
					{
						wheels.dataWheels.Apply(controller);
						idDB = wheels.id;
					}
					else
						isNotFound = true;
					break;
				default:
					LogSystem.Print(AlertLevel.Notify, "VehiclesManager", $"Tuning type is not defined for {tuningType}");
					break;
			}
			if (isNotFound)
				LogSystem.Print(AlertLevel.Error, "VehiclesManager", $"Not found {tuningType} tuning in db with identifier = {identifier}");
#if UNITY_EDITOR
			else
				if (tuningType != TuningType.None)
					vehicle.idsTuning[(int)tuningType] = idDB;
#endif*/
		}

		public static TuningType StringToTuningType(string tunningTypeName)
		{
			switch (tunningTypeName.ToLower())
			{
				case "engine":
					return TuningType.Engine;
				case "steering":
					return TuningType.Steering;
				case "transmission":
					return TuningType.Transmission;
				case "brakes":
					return TuningType.Brakes;
				case "axles":
					return TuningType.Axles;
				case "wheels":
					return TuningType.Wheels;
			}
			return TuningType.None;
		}
		#endregion
	}
}