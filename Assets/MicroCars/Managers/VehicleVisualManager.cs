using System.Collections.Generic;
using MicroRace.DB;
using MicroRace.Prefs;
using MicroRace.Vehicles;
using RedDev.Kernel.Managers;
using UnityEngine;

namespace MicroRace.Managers
{
	public class VehicleVisualManager : BaseManager
	{
		private PrefsManager _prefs;

		public override void Attach()
		{
			base.Attach();
			_prefs = Core.Get<PrefsManager>();
		}

		public void ApplyVisual(VehicleGameController actor, VehicleVisual visual)
		{
			if (actor == null || visual == null)
			{
                Prod.LogError($"[VehicleVisualManager] No actor or visual settings");
				return;
			}

			var vehSettings = _prefs.Get<VehiclesSettings>();
			if (!vehSettings.visualSettings.TryGetValue(actor.PlayerData.vehicleId, out var visSettings))
			{
				visSettings = new VehicleVisualSettings();
				vehSettings.visualSettings.Add(actor.PlayerData.vehicleId, visSettings);
			}

			SpawnBodyKits(actor, visSettings, visual);

			vehSettings.Save();
		}

		public void ChangeChassis(VehicleGameController actor, VehicleVisual visual, int idChassis = -1)
		{
			if (actor == null || visual == null)
			{
                Prod.LogError("[VehicleVisualManager] No actor or visual settings");
				return;
			}

			var vehSettings = _prefs.Get<VehiclesSettings>();
			if (!vehSettings.visualSettings.TryGetValue(actor.PlayerData.vehicleId, out var visSettings))
			{
				visSettings = new VehicleVisualSettings();
				vehSettings.visualSettings.Add(actor.PlayerData.vehicleId, visSettings);
			}
			visSettings.currentChassis = idChassis;
			visSettings.slots.Clear();

			SpawnBodyKits(actor, visSettings, visual);

			vehSettings.Save();
		}

		private void ClearBodyKits(VehicleGameController actor, Transform visualContainer)
		{
			if (visualContainer)
				for (int i = visualContainer.childCount - 1; i >= 0; i--)
					Destroy(visualContainer.GetChild(i).gameObject);
		}

		private void SpawnBodyKits(VehicleGameController actor, VehicleVisualSettings visualSettings, VehicleVisual visual)
		{
			var visualContainer = actor.transform.Find("[VisualBody]");
			if (!visualContainer)
			{
				visualContainer = (new GameObject("[VisualBody]")).transform;
				visualContainer.parent = actor.transform;
			}
			if (visualContainer.childCount > 0)
				ClearBodyKits(actor, visualContainer);

			visualSettings.PurchasedParts.Clear();
			visualSettings.InstalledParts.Clear();
			if (visualSettings.currentChassis < 0 || visualSettings.slots.Count == 0)
			{
				//var core = visualSettings.currentChassis < 0 ? visual["core"][0] : visual[visualSettings.currentChassis];
				VehiclePart core;
				if (visualSettings.currentChassis < 0)
				{
					var coreList = visual["core"];
					core = coreList[0];
				}
				else
					core = visual[visualSettings.currentChassis];
				if (core != null)
					BuildChassis(visualContainer, visualSettings, visual, core);
				else
                    Prod.LogError($"[VehicleVisualManager] Core for {visual.carId} not found");
			}
			else
				BuildFromSlots(visualContainer, visualSettings, visual);
			/*var damageComponent = actor.Get<DamageComponent>();
			if (damageComponent != null)
				damageComponent.Build();*/
		}

		private void BuildChassis(Transform visualContainer, VehicleVisualSettings visualSettings, VehicleVisual visual, VehiclePart vehiclePart)
		{
			Stack<VehiclePart> parts = new Stack<VehiclePart>();

			ChangeOrCreateSlotMeta(visualSettings, vehiclePart.type, vehiclePart.nativeId);
			PrepareEnableSection(ref parts, visualSettings, visual, vehiclePart);

			PrepareDisableSection(visualSettings, vehiclePart);
			foreach (var part in parts)
				PrepareDisableSection(visualSettings, part);

			BuildFromSlots(visualContainer, visualSettings, visual);
		}

		private void BuildFromSlots(Transform visualContainer, VehicleVisualSettings visualSettings, VehicleVisual visual)
		{
			foreach (var slot in visualSettings.slots.Values)
			{
				var partId = slot.GetValue();
				if (visualSettings.InstalledParts.Contains(partId) || !slot.activated)
					continue;
				visualSettings.InstalledParts.Add(partId);
				var part = visual[partId];
				InternalSpawn(visual.path + part.prefab, visualContainer);
			}
		}

		private void PrepareEnableSection(ref Stack<VehiclePart> stack, VehicleVisualSettings settings, VehicleVisual visual, VehiclePart part)
		{
			foreach (var item in part.enable)
			{
				ChangeOrCreateSlotMeta(settings, item.slot, item.partId);
				if (item.partId < 0)
					continue;
				var partItem = visual[item.partId];
				stack.Push(partItem);
				PrepareEnableSection(ref stack, settings, visual, partItem);
			}
		}

		private void ChangeOrCreateSlotMeta(VehicleVisualSettings settings, string slot, int partId)
		{
			if (!settings.slots.TryGetValue(slot, out var slotout))
			{
				settings.slots.Add(slot, new SlotMeta
				{
					activated = true,
					defaultValue = partId,
					value = partId
				});
			}
			else
			{
				slotout.activated = true;
				slotout.defaultValue = partId;
				if (slotout.value < 0)
					slotout.value = partId;
			}
		}

		private void PrepareDisableSection(VehicleVisualSettings settings, VehiclePart part)
		{
			foreach (var item in part.disable)
				if (item.partId < 0)
					if (settings.slots.TryGetValue(item.slot, out var slotout))
						slotout.activated = false;
		}

		private void InternalSpawn(string pathPrefab, Transform parent = null)
		{
			var resource = Resources.Load<GameObject>(pathPrefab);
			if (resource != null)
			{
				var obj = Instantiate(resource, Vector3.zero, Quaternion.identity, parent);
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localRotation = Quaternion.identity;
			}
			else
                Prod.LogError($"[VehicleVisualManager] Prefab {pathPrefab} not found");
		}
	}
}