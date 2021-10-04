using System.Collections.Generic;
using RedDev.Kernel.DB;

namespace MicroRace.DB
{
	[MetaModel("DB/Vehicles/Tuning/Wheels/")]
	public class WheelsDBMeta: BaseTuningDBMeta
	{
		public List<WheelModelData> dataWheels;
	}
}