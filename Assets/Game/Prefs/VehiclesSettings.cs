using System;
using System.Collections.Generic;
using Game.DB;
using RedDev.Kernel.DB;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

namespace Game.Prefs
{
	[Serializable]
	public class VehicleVisualSettingsDictionary: SerializableDictionaryBase<int, VehicleVisualSettings> { }

	[Serializable]
	public class VehiclesSettings : BasePrefsModel
	{
		[SerializeField]
		private VehicleVisualSettingsDictionary _visualSettings = new VehicleVisualSettingsDictionary();
		public VehicleVisualSettingsDictionary visualSettings => _visualSettings;
	}

	[Serializable]
	public class VehicleVisualSettings
	{
		public int currentChassis = -1;
		public List<int> PurchasedParts = new List<int>();
		public List<int> InstalledParts = new List<int>();
		public SlotsByCategory slots = new SlotsByCategory();
	}
}