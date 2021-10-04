namespace RedDev.Helpers.Extensions
{
	public static class ObjectExtensions
	{
		public static bool TryCatch<T>(this object self, out T result)
		{
			result = default(T);
			if (self is T)
			{
				result = (T) self;
				return true;
			}
			return false;
		}
	}
}