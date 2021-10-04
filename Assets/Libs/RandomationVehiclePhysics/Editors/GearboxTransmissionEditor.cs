#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace RVP {
    [CustomEditor(typeof(GearboxTransmission))]
    [CanEditMultipleObjects]
    public class GearboxTransmissionEditor : Editor {
        private bool isPrefab = false;
        private static bool showButtons = true;

        public override void OnInspectorGUI() {
            var boldFoldout = new GUIStyle(EditorStyles.foldout);
            boldFoldout.fontStyle = FontStyle.Bold;
            var targetScript = (GearboxTransmission) target;
            var allTargets = new GearboxTransmission[targets.Length];
            isPrefab = F.IsPrefab(targetScript);

            for (var i = 0; i < targets.Length; i++) {
                Undo.RecordObject(targets[i], "Transmission Change");
                allTargets[i] = targets[i] as GearboxTransmission;
            }

            DrawDefaultInspector();

            if (!isPrefab && targetScript.gameObject.activeInHierarchy) {
                showButtons = EditorGUILayout.Foldout(showButtons, "Quick Actions", boldFoldout);
                EditorGUI.indentLevel++;
                if (showButtons)
                    if (GUILayout.Button("Calculate RPM Ranges"))
                        foreach (var curTarget in allTargets)
                            curTarget.CalculateRpmRanges();
                EditorGUI.indentLevel--;
            }

            if (GUI.changed)
                EditorUtility.SetDirty(targetScript);
        }
    }
}
#endif