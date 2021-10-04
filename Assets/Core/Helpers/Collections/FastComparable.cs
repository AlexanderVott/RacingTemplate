using System.Collections.Generic;

namespace RedDev.Helpers.Collections
{
	public class FastComparable : IEqualityComparer<int>
	{
		public bool Equals(int x, int y)
		{
			return x == y;
		}

		public int GetHashCode(int obj)
		{
			return obj.GetHashCode();
		}
	}
}