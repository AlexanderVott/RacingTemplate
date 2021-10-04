using System;
using RedDev.Kernel.DB;
using UnityEngine;

namespace MicroRace.DB
{
	[MetaModel("DB/FX/")]
	public class VehicleSurfaceDB: BaseMetaDB
	{
		public GameObject smokePrefab;
		public GameObject dustPrefab;

		public SurfaceData[] surfacesData = {};

		public SurfaceData this[int index]
		{
			get
			{
				if (index >= 0 && index < surfacesData.Length)
					return surfacesData[index];
				return null;
			}
		}

		public SurfaceData this[string nameSurface]
		{
			get
			{
				foreach (var surface in surfacesData)
					if (surface.surfaceName == nameSurface)
						return surface;
				return null;
			}
		}

		[Serializable]
		public class BaseSurfaceData
		{
			public string surfaceName;
			[Header("Surface")]
			public int[] terrainTextureIndices = { };
			public string[] tags = { };
		}
		
		[Serializable]
		public class SurfaceData: BaseSurfaceData
		{
			//public WheelController.FrictionPreset.FrictionPresetEnum frictionPresetEnum;

			[Header("Skidmarks")]
			public Material skidmarkMaterial;
			public bool slipBasedSkidIntensity = false;

			[Header("Particle effects")]
			[Range(0, 50)] public float smokeIntensity = 30f;
			[Range(0, 50)] public float dustIntensity = 0f;
			public Color dustColor = Color.yellow;

			[Header("Sound")]
			public bool slipSensitiveSurfaceSound;
			public string surfaceSoundEvent;
			public string skidSoundEvent;
			public string rollSoundEvent;
		}
	}
}