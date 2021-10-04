using System;
using RedDev.Kernel.Bindings.Properties.Base;

namespace RedDev.Kernel.Bindings.Properties
{
	public class IntProperty : Property<int>, INumericProperty
	{
		public float GetFloatValue()
		{
			return Convert.ToSingle(Value);
		}

		public void SetFromFloat(float value)
		{
			Value = Convert.ToInt32(value);
		}
	}
}