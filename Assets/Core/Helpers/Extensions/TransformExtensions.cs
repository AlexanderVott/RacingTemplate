using System.Collections.Generic;
using UnityEngine;

namespace RedDev.Helpers.Extensions
{
	public static class TransformExtensions
	{
		public static float Distance(this Transform self, Transform other) 
            => Vector3.Distance(self.position, other.position);

        public static float Distance(this Transform self, Vector3 other) 
            => Vector3.Distance(self.position, other);

        public static Transform FindChildWithTag(this Transform parent, string tag)
		{
			if (parent == null)
				throw new System.ArgumentNullException();
			if (string.IsNullOrEmpty(tag))
				throw new System.ArgumentNullException();

			for (int i = 0; i < parent.transform.childCount; i++)
			{
				var child = parent.transform.GetChild(i);
				if (child.gameObject.CompareTag(tag))
					return child;
			}

			return null;
		}

		public static Transform[] FindChildsWithTag(this Transform parent, string tag)
		{
			if (parent == null)
				throw new System.ArgumentNullException();
			if (string.IsNullOrEmpty(tag))
				throw new System.ArgumentNullException();

			List<Transform> result = new List<Transform>();
			for (int i = 0; i < parent.transform.childCount; i++)
			{
				var child = parent.transform.GetChild(i);
				if (child.gameObject.CompareTag(tag))
					result.Add(child);
				result.AddRange(child.FindChildsWithTag(tag));
			}

			return result.ToArray();
		}
	}
}
