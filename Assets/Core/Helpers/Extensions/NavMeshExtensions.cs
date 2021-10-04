using UnityEngine;
using UnityEngine.AI;

namespace RedDev.Helpers.Extensions
{
	public static class NavMeshExtensions
	{
		public static void GetPointInDirection(Vector3 position, Vector3 direction, float maxDistance, int areaMask, out Vector3? result)
		{
			NavMeshHit hit;
			if (NavMesh.SamplePosition(position + direction, out hit, maxDistance, areaMask))
			{
				result = hit.position;
				return;
			}
			result = null;
		}

		public static void GetRandomInSphereRadius(Vector3 position, float radius, int areaMask, out Vector3? result)
		{
			Vector3 randomDirection = Random.insideUnitSphere * radius;
			GetPointInDirection(position, randomDirection, radius, areaMask, out result);
		}

		public static Vector3 GetRandomInSphereRadius(Vector3 position, float radius, int areaMask)
		{
			Vector3? hit;
			GetRandomInSphereRadius(position, radius, areaMask, out hit);
			return hit ?? Vector3.zero;
		}

		public static void GetRandomInCircleRadius(Vector3 position, float radius, int areaMask, out Vector3? result)
		{
			var circleRandom = Random.insideUnitCircle * radius;
			Vector3 randomDirection = new Vector3(circleRandom.x, 0.0f, circleRandom.y);
			GetPointInDirection(position, randomDirection, radius, areaMask, out result);
		}

		public static Vector3 GetRandomInCircleRadius(Vector3 position, float radius, int areaMask)
		{
			Vector3? hit;
			GetRandomInCircleRadius(position, radius, areaMask, out hit);
			return hit ?? Vector3.zero;
		}
	}
}