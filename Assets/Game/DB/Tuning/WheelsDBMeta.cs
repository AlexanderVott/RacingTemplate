using System.Collections.Generic;
using RedDev.Kernel.DB;

namespace Game.DB
{
	[MetaModel("DB/Vehicles/Tuning/Wheels/")]
	public class WheelsDBMeta: BaseTuningDBMeta
	{
		public List<WheelModelData> dataWheels;
	}
}