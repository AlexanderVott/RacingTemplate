namespace RedDev.Helpers.Extensions
{
	public static class IntExtensions
	{
		public static bool IsInRangeInc(this int self, int min, int max) 
            => (self >= min) && (self <= max);

        public static bool IsInRangeExc(this int self, int min, int max) 
            => (self > min) && (self < max);

        public static void SwitchValues(this int self, int target)
		{
			var tmp = self;
			self = target;
			target = tmp;
		}
	}
}