using RedDev.Kernel.Bindings.Properties.Base;

namespace RedDev.Kernel.Bindings.Properties
{
	public class FloatProperty : Property<float>, INumericProperty
	{
		public float GetFloatValue()
		{
			return Value;
		}

		public void SetFromFloat(float value)
		{
			Value = value;
		}
	}
}
