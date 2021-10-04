using UnityEngine;

namespace RedDev.Helpers {
	public class GizmosForward : MonoBehaviour {

		public Color ColorElement = Color.cyan;
		public float DistanceVector = 1.0f;

        private void OnDrawGizmos() {	
			Gizmos.color = new Color(ColorElement.r, ColorElement.g, ColorElement.b, 0.25f);
			Gizmos.DrawLine(transform.position, transform.position + (transform.forward * DistanceVector));
			Gizmos.color = Color.green;
		}

        private void OnDrawGizmosSelected() {
			Gizmos.color = ColorElement;
			Gizmos.DrawLine(transform.position, transform.position + (transform.forward * DistanceVector));
			Gizmos.color = Color.green;
		}
	}
}