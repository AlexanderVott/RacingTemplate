using RedDev.Kernel.DB.Editor;
using UnityEditor;

namespace MicroRace.DB
{
	public static class VehicleSurfaceDBEditorMenus 
	{
		[MenuItem("RedDev/DB/FX/Create Vehicle Surface DB")]
		public static void CreateTestMeta()
		{
			DBEditorMenus.CreateMetaBase<VehicleSurfaceDB>();
		}
	}
}