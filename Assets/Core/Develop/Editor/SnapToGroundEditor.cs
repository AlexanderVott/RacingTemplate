using UnityEditor;
using UnityEngine;

namespace RedDev.Editor
{
	public class SnapToGroundEditor : EditorWindow
	{
		[SerializeField] private float _distanceRayCast = 100f;

		[SerializeField] private LayerMask _ignoreRaycast;

		[MenuItem("RedDev/Snap to ground", false, 1)]
		public static void ShowWindow()
		{
			var myWindow = GetWindow<SnapToGroundEditor>();
			myWindow.titleContent = new GUIContent("SnapToGround");
			myWindow.minSize = new Vector2(300, 400);
			myWindow.maxSize = myWindow.minSize;
			myWindow.position = new Rect(500, 500, 300, 400);
			myWindow.Show();
		}

		void OnGUI()
		{
			GUILayout.Label("Snap to ground");
			EditorGUILayout.Space();
			GUILayout.Label("Distance for raycast");
			EditorGUILayout.BeginVertical();
			_distanceRayCast = EditorGUILayout.FloatField(_distanceRayCast);
			EditorGUILayout.EndVertical();

			GUILayout.Label("Ignore raycast mask");
			EditorGUILayout.BeginVertical();
			_ignoreRaycast.value = EditorGUILayout.MaskField(_ignoreRaycast.value, UnityEditorInternal.InternalEditorUtility.layers);
			EditorGUILayout.EndVertical();

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			if (GUILayout.Button("Snap"))
				SnapOnGround(_distanceRayCast, _ignoreRaycast);
		}

		public static void SnapOnGround(float distance, LayerMask mask)
		{
			var selected = Selection.transforms;
			foreach (var item in selected)
			{
				var ray = new Ray(item.position, Vector3.down);
				if (Physics.Raycast(ray, out var hit, distance, ~mask))
				{
					Undo.RecordObject(item, "Snap to ground");
					item.position = hit.point;
					EditorUtility.SetDirty(item);
				}
			}
		}
	}
}