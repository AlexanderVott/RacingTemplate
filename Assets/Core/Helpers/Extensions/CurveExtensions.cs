using UnityEngine;

namespace RedDev.Helpers.Extensions
{
	public static class CurveExtensions
	{
		public static void Copy(this AnimationCurve self, out AnimationCurve to)
		{
			to = new AnimationCurve(self.keys);
			to.preWrapMode = self.preWrapMode;
			to.postWrapMode = self.postWrapMode;
		}
	}
}