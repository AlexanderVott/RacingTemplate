using System;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RedDev.Kernel.Scenes
{
	[Serializable]
	public class SceneField 
	{
		[SerializeField]
		private Object _sceneAsset;
		[SerializeField]
		private string _sceneName = null;

		public string sceneName => _sceneName;

		public static implicit operator string(SceneField sceneField) 
            => sceneField.sceneName;
    }

#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(SceneField))]
	public class SceneFieldPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, GUIContent.none, property);

			var sceneAsset = property.FindPropertyRelative("_sceneAsset");
			var sceneName = property.FindPropertyRelative("_sceneName");

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			if (sceneAsset != null)
			{
				sceneAsset.objectReferenceValue = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
				if (sceneAsset.objectReferenceValue != null)
					sceneName.stringValue = (sceneAsset.objectReferenceValue as SceneAsset).name;
			}

			EditorGUI.EndProperty();
		}
	}
#endif
}