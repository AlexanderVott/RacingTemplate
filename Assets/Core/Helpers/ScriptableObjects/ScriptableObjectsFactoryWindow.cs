using System;
using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
#endif

namespace RedDev.Helpers.ToolsEditor
{
#if UNITY_EDITOR
	internal class EndNameEdit : EndNameEditAction
    {
        #region implemented abstract members of EndNameEditAction

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            AssetDatabase.CreateAsset(EditorUtility.InstanceIDToObject(instanceId),
                AssetDatabase.GenerateUniqueAssetPath(pathName));
        }

        #endregion
    }

    /// <summary>
    /// Scriptable object window.
    /// </summary>
    public class ScriptableObjectsFactoryWindow : EditorWindow
    {
        private int selectedIndex;
        private string[] names;

        private Type[] types;

        public Type[] Types
        {
            get => types;
	        set
            {
                types = value;
                names = types.Select(t => t.FullName).ToArray();
            }
        }

        public void OnGUI()
        {
            GUILayout.Label("ScriptableObject Class");
            selectedIndex = EditorGUILayout.Popup(selectedIndex, names);

            if (GUILayout.Button("Create"))
            {
	            var type = types[selectedIndex];
				var path = (Selection.activeObject != null
		            ? Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID()))
		            : "") + "/" + type.Name + ".asset";
				var asset = ScriptableObject.CreateInstance(type);
	            var instanceId = asset.GetInstanceID();
				ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
					instanceId,
					ScriptableObject.CreateInstance<EndNameEdit>(),
                    path,
                    AssetPreview.GetMiniThumbnail(asset),
                    null);

                Close();
            }
        }
    }
#endif
}