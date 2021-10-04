using System;
using UnityEngine;

namespace RedDev.Helpers
{
	public enum HandType
	{
		Box = 0,
		Sphere = 1
	}

	[ExecuteInEditMode]
	public class GizmosElement : MonoBehaviour
	{
		public HandType Type = HandType.Box;
		public Color ColorElement = Color.cyan;

		public Vector3 size = Vector3.one;

        private void OnDrawGizmosSelected()
		{
			Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

			Gizmos.matrix = matrix;

			Gizmos.color = new Color(ColorElement.r, ColorElement.g, ColorElement.b, 0.25f);

			switch (Type)
			{
				case HandType.Box:
					Gizmos.DrawWireCube(Vector3.zero, size);
					break;
				case HandType.Sphere:
					Gizmos.DrawWireSphere(Vector3.zero, size.x);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			Gizmos.color = Color.green;

			Gizmos.matrix = Matrix4x4.identity;
			OnDrawGizmos();
		}

        private void OnDrawGizmos()
		{
			Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

			Gizmos.matrix = matrix;

			Gizmos.color = ColorElement;

			switch (Type)
			{
				case HandType.Box:
					Gizmos.DrawWireCube(Vector3.zero, size);
					break;
				case HandType.Sphere:
					Gizmos.DrawWireSphere(Vector3.zero, size.x);
					break;
			}

			Gizmos.color = new Color(ColorElement.r, ColorElement.g, ColorElement.b, ColorElement.a);

			switch (Type)
			{
				case HandType.Box:
					Gizmos.DrawCube(Vector3.zero, size);
					break;
				case HandType.Sphere:
					Gizmos.DrawSphere(Vector3.zero, size.x);
					break;
			}

			Gizmos.color = Color.green;

			Gizmos.matrix = Matrix4x4.identity;
		}
	}
}