using System;
using RedDev.Kernel.DB;
using RedDev.Kernel.DB.Editor;
using UnityEditor;

namespace MicroRace.DB.Editor
{
	public static class TuningMetaEditorMenus
	{
		[MenuItem("RedDev/DB/Vehicle/Tuning/Create EnginesDBMeta")]
		public static void CreateEngineMeta()
		{
			DBEditorMenus.CreateMetaBase<EnginesDBMeta>();
		}

		[MenuItem("RedDev/DB/Vehicle/Tuning/Create SteeringDBMeta")]
		public static void CreateSteeringMeta()
		{
			DBEditorMenus.CreateMetaBase<SteeringDBMeta>();
		}

		[MenuItem("RedDev/DB/Vehicle/Tuning/Create TransmissionDBMeta")]
		public static void CreateTransmissionMeta()
		{
			DBEditorMenus.CreateMetaBase<TransmissionsDBMeta>();
		}

		[MenuItem("RedDev/DB/Vehicle/Tuning/Create BrakesDBMeta")]
		public static void CreateBrakesMeta()
		{
			DBEditorMenus.CreateMetaBase<BrakesDBMeta>();
		}

		[MenuItem("RedDev/DB/Vehicle/Tuning/Create AxlesDBMeta")]
		public static void CreateAxlesMeta()
		{
			DBEditorMenus.CreateMetaBase<AxlesDBMeta>();
		}

		[MenuItem("RedDev/DB/Vehicle/Tuning/Create WheelsDBMeta")]
		public static void CreateWheelsMeta()
		{
			DBEditorMenus.CreateMetaBase<WheelsDBMeta>();
		}

		public static bool ValidateCreateMetaBase(this BaseMetaDB self)
		{
			/*if (Selection.activeObject == null)
				return false;*/
			if (String.IsNullOrEmpty(AssetDatabase.GetAssetPath(Selection.activeObject)))
				return false;
			/*if (Selection.activeObject is T == false)
				return false;*/
			return true;
		}

		
	}
}