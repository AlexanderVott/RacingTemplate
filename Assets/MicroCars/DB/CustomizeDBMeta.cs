using System;
using System.Collections.Generic;
using RedDev.Kernel.DB;
using RotaryHeart.Lib.SerializableDictionary;
using SimpleXML;
using UnityEngine;

namespace MicroRace.DB
{
	[MetaModel("DB/Vehicles/Customize/")]
	public class CustomizeDBMeta : BaseMetaDB, IMetaXML
	{
		[SerializeField]
		private VehicleVisual _root = new VehicleVisual();
		public VehicleVisual root => _root;

		public void Parse(string source)
		{
			var doc = new XMLDoc(source, false);

			_id = doc.GetAttribDef("carId", -1);
			_root.carId = _id;
			_root.path = doc.GetAttribDef("path", "");

			foreach (var childSlot in doc.childs)
				ParseSlot(childSlot);

			Validator();
		}

		private void ParseSlot(XMLDoc node)
		{
			var separator = new[] { ' ' };
			var slot = new VehiclePart
			{
				nativeId = node.GetAttribDef("id", -1),
				type = node.nodeName,
				prefab = node.GetAttribDef("prefab", ""),
				dlc = node.GetAttribDef("dlc", ""),
				price = node.GetAttribDef("price", 0),
			};

			var enableSourceArr = node.GetAttribDef("enable", "").Split(separator, StringSplitOptions.RemoveEmptyEntries);
			foreach (var enableSourceItem in enableSourceArr)
			{
				var pair = enableSourceItem.Split('#');
				var idPart = -1;
				if (pair.Length > 1)
					int.TryParse(pair[1], out idPart);
				var slotPart = new SlotPart
				{
					slot = pair[0],
					partId = idPart
				};
				slot.enable.Add(slotPart);
			}

			var slotsSourceArr = node.GetAttribDef("take", "").Split(separator, StringSplitOptions.RemoveEmptyEntries);
			foreach (var slotSourceItem in slotsSourceArr)
				slot.slots.Add(new SlotPart
				{
					slot = slotSourceItem.ToLower(),
					partId = -1
				});

			if (!_root.dbById.ContainsKey(slot.nativeId))
				_root.dbById.Add(slot.nativeId, slot);
			else
				Debug.LogError($"Для кастомизации авто {_root.carId} уже существует слот с id = {slot.nativeId}");

			if (!_root.dbByType.ContainsKey(slot.type))
				_root.dbByType.Add(slot.type, new PartsList { slot });
			else
				_root.dbByType[slot.type].Add(slot);
		}

		private void Validator()
		{
			if (_root.carId < 0)
				Debug.Log($"У профиля кастомизации некорректный Id. Префаб: {_root.path}");
			if (String.IsNullOrWhiteSpace(_root.path))
				Debug.Log($"Не указан путь до префабов кастомизации для {_root.carId}");
			if (_root.core == null)
				Debug.Log($"Не задан core модуль кастомизации для {_root.carId}");

			foreach (var partKey in _root.dbById.Keys)
				ValidatePart(_root.dbById[partKey]);
		}

		private void ValidatePart(VehiclePart part)
		{
			//TODO: Check nativeId, type, DLC

			if (part.type.ToLower() != "core" && part.nativeId < 0)
				Debug.Log($"Некорректно указанный id элемента у {_root.carId}");

			foreach (var enable in part.enable)
			{
				if (String.IsNullOrWhiteSpace(enable.slot))
					Debug.Log($"У {_root.carId} пустой слот в разделе enable");
				if (!_root.dbByType.ContainsKey(enable.slot))
					Debug.LogError($"Слот {enable.slot} для {_root.carId} - {part.type} не найден");
				if (enable.partId < 0 || !_root.dbById.ContainsKey(enable.partId))
					Debug.LogError($"Слот с Id {enable.partId} для {_root.carId} - {part.type} не найден");
			}
			foreach (var slot in part.slots)
				if (!String.IsNullOrWhiteSpace(slot.slot) && !_root.dbByType.ContainsKey(slot.slot))
					Debug.Log($"У {_root.carId} не найден {slot.slot}");
		}
	}

	[Serializable]
	public class VehicleVisual
	{
		public ulong packetId;

		public int carId;
		public string path;

		public VehiclePart core;

		public VehiclePartDictionary dbById = new VehiclePartDictionary();
		public VehiclePartsCollectionsByTypeDictionary dbByType = new VehiclePartsCollectionsByTypeDictionary();

		public List<SlotMeta> slots = new List<SlotMeta>();

		public VehiclePart this[int id] => dbById[id];

		public List<VehiclePart> this[string take]
		{
			get
			{
				// кэш для реалтайма в билде
				if (dbByType[take].Count == 0)
				{
					foreach (var part in dbById.Values)
						if (part.type.Equals(take))
							dbByType[take].Add(part);
				}
				return dbByType[take];
			}
		}
	}

	[Serializable]
	public class VehiclePartDictionary: SerializableDictionaryBase<int, VehiclePart> { }

	[Serializable]
	public class VehiclePartsCollectionsByTypeDictionary: SerializableDictionaryBase<string, PartsList> { }
	
	/// <summary>
	/// Слоты по названиям категорий.
	/// </summary>
	[Serializable]
	public class SlotsByCategory: SerializableDictionaryBase<string, SlotMeta> { }

	[Serializable]
	public class PartsList: List<VehiclePart> { }

	[Serializable]
	public class VehiclePart
	{
		public int nativeId;
		public string type;

		public string prefab;
		public string dlc = String.Empty;
		//public string take;
		public int price;

		public List<SlotPart> enable = new List<SlotPart>();
		public List<SlotPart> disable = new List<SlotPart>();
		public List<int> force = new List<int>();
		public List<SlotPart> slots = new List<SlotPart>();
	}

	[Serializable]
	public class SlotPart
	{
		public string slot;
		public int partId;
	}

	[Serializable]
	public class SlotsMap
	{
		public int vehicleId;
		public SlotsByCategory slots = new SlotsByCategory();
	}

	[Serializable]
	public class SlotMeta
	{
		public int defaultValue;
		public int value;
		public bool activated;

		public int GetValue()
		{
			return value < 0 ? defaultValue : value;
		}
	}
}