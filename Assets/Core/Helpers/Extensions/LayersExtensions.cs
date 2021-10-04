using UnityEngine;

namespace RedDev.Helpers.Extensions
{
	public static class LayersExtensions
	{
		public static bool Contains(this LayerMask mask, int layer) 
            => mask == (mask | (1 << layer));
    }
}