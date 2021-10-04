#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace RVP {
    [CustomEditor(typeof(Wheel))]
    [CanEditMultipleObjects]
    public class WheelEditor : Editor {
        private bool isPrefab = false;
        private static bool showButtons = true;
        private static float radiusMargin = 0;
        private static float widthMargin = 0;

        public override void OnInspectorGUI() {
            var boldFoldout = new GUIStyle(EditorStyles.foldout);
            boldFoldout.fontStyle = FontStyle.Bold;
            var targetScript = (Wheel) target;
            var allTargets = new Wheel[targets.Length];
            isPrefab = F.IsPrefab(targetScript);

            for (var i = 0; i < targets.Length; i++) {
                Undo.RecordObject(targets[i], "Wheel Change");
                allTargets[i] = targets[i] as Wheel;
            }

            DrawDefaultInspector();

            if (!isPrefab && targetScript.gameObject.activeInHierarchy) {
                showButtons = EditorGUILayout.Foldout(showButtons, "Quick Actions", boldFoldout);
                EditorGUI.indentLevel++;
                if (showButtons) {
                    if (GUILayout.Button("Get Wheel Dimensions"))
                        foreach (var curTarget in allTargets)
                            curTarget.GetWheelDimensions(radiusMargin, widthMargin);

                    EditorGUI.indentLevel++;
                    radiusMargin = EditorGUILayout.FloatField("Radius Margin", radiusMargin);
                    widthMargin = EditorGUILayout.FloatField("Width Margin", widthMargin);
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }

            if (GUI.changed)
                EditorUtility.SetDirty(targetScript);
        }
    }
}
#endif