using System.Collections.Generic;
using RedDev.Kernel.DB;

namespace Game.DB
{
	[MetaModel("DB/Vehicles/Tuning/Axles/")]
	public class AxlesDBMeta : BaseTuningDBMeta
	{
		public List<AxleModelData> dataAxles;
	}
}