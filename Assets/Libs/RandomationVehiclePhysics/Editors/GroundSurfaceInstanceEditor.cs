#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace RVP {
    [CustomEditor(typeof(GroundSurfaceInstance))]
    [CanEditMultipleObjects]
    public class GroundSurfaceInstanceEditor : Editor {
        public override void OnInspectorGUI() {
            var surfaceMaster = FindObjectOfType<GroundSurfaceMaster>();
            var targetScript = (GroundSurfaceInstance) target;
            var allTargets = new GroundSurfaceInstance[targets.Length];

            for (var i = 0; i < targets.Length; i++) {
                Undo.RecordObject(targets[i], "Ground Surface Change");
                allTargets[i] = targets[i] as GroundSurfaceInstance;
            }

            var surfaceNames = new string[surfaceMaster.surfaceTypes.Length];

            for (var i = 0; i < surfaceNames.Length; i++)
                surfaceNames[i] = surfaceMaster.surfaceTypes[i].name;

            foreach (var curTarget in allTargets)
                curTarget.surfaceType = EditorGUILayout.Popup("Surface Type", curTarget.surfaceType, surfaceNames);

            if (GUI.changed)
                EditorUtility.SetDirty(targetScript);
        }
    }
}
#endif