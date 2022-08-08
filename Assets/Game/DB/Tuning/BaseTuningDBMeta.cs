using RedDev.Kernel.DB;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.DB
{
	public class BaseTuningDBMeta : BaseMetaDB
	{
		public virtual void Apply(/*VehicleController controller*/)
		{
		}

		public void Save()
		{
#if UNITY_EDITOR
			EditorUtility.SetDirty(this);
#endif
		}
	}
}
