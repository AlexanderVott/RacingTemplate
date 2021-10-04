namespace RedDev.Helpers.Extensions
{
	public static class FloatExtensions
	{
		public static float CurveFactor(float factor) 
            => 1 - (1 - factor) * (1 - factor);

        public static float ULerp(float from, float to, float value) 
            => (1.0f - value) * @from + value * to;

        public static bool IsInRangeInc(this float self, float min, float max) 
            => (self >= min) && (self <= max);

        public static bool IsInRangeExc(this float self, float min, float max) 
            => (self > min) && (self < max);

        public static void SwitchValues(this float self, float target)
		{
			var tmp = self;
			self = target;
			target = tmp;
		}
	}
}
