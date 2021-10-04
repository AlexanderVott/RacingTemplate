#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace RVP {
    [CustomEditor(typeof(VehicleParent))]
    [CanEditMultipleObjects]
    public class VehicleParentEditor : Editor {
        private bool isPrefab = false;
        private static bool showButtons = true;
        private bool wheelMissing = false;

        public override void OnInspectorGUI() {
            var boldFoldout = new GUIStyle(EditorStyles.foldout);
            boldFoldout.fontStyle = FontStyle.Bold;
            var targetScript = (VehicleParent) target;
            var allTargets = new VehicleParent[targets.Length];
            isPrefab = F.IsPrefab(targetScript);

            for (var i = 0; i < targets.Length; i++) {
                Undo.RecordObject(targets[i], "Vehicle Parent Change");
                allTargets[i] = targets[i] as VehicleParent;
            }

            wheelMissing = false;
            if (targetScript.wheelGroups != null && targetScript.wheelGroups.Length > 0) {
                if (targetScript.hover)
                    foreach (var curWheel in targetScript.hoverWheels) {
                        var wheelfound = false;
                        foreach (var curGroup in targetScript.wheelGroups) {
                            foreach (var curWheelInstance in curGroup.hoverWheels) {
                                if (curWheel == curWheelInstance)
                                    wheelfound = true;
                            }
                        }

                        if (!wheelfound) {
                            wheelMissing = true;
                            break;
                        }
                    }
                else
                    foreach (var curWheel in targetScript.wheels) {
                        var wheelfound = false;
                        foreach (var curGroup in targetScript.wheelGroups) {
                            foreach (var curWheelInstance in curGroup.wheels) {
                                if (curWheel == curWheelInstance)
                                    wheelfound = true;
                            }
                        }

                        if (!wheelfound) {
                            wheelMissing = true;
                            break;
                        }
                    }
            }

            if (wheelMissing)
                EditorGUILayout.HelpBox("If there is at least one wheel group, all wheels must be part of a group.",
                    MessageType.Error);

            DrawDefaultInspector();

            if (!isPrefab && targetScript.gameObject.activeInHierarchy) {
                showButtons = EditorGUILayout.Foldout(showButtons, "Quick Actions", boldFoldout);
                EditorGUI.indentLevel++;
                if (showButtons) {
                    if (GUILayout.Button("Get Engine"))
                        foreach (var curTarget in allTargets)
                            curTarget.engine = curTarget.transform.GetComponentInChildren<Motor>();

                    if (GUILayout.Button("Get Wheels"))
                        foreach (var curTarget in allTargets) {
                            if (curTarget.hover)
                                curTarget.hoverWheels = curTarget.transform.GetComponentsInChildren<HoverWheel>();
                            else
                                curTarget.wheels = curTarget.transform.GetComponentsInChildren<Wheel>();
                        }
                }

                EditorGUI.indentLevel--;
            }

            if (GUI.changed)
                EditorUtility.SetDirty(targetScript);
        }
    }
}
#endif