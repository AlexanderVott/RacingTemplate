#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace RVP {
    [CustomEditor(typeof(Suspension))]
    [CanEditMultipleObjects]
    public class SuspensionEditor : Editor {
        private bool isPrefab = false;
        private static bool showButtons = true;

        public override void OnInspectorGUI() {
            var boldFoldout = new GUIStyle(EditorStyles.foldout);
            boldFoldout.fontStyle = FontStyle.Bold;
            var targetScript = (Suspension) target;
            var allTargets = new Suspension[targets.Length];
            isPrefab = F.IsPrefab(targetScript);

            for (var i = 0; i < targets.Length; i++) {
                Undo.RecordObject(targets[i], "Suspension Change");
                allTargets[i] = targets[i] as Suspension;
            }

            if (!targetScript.wheel)
                EditorGUILayout.HelpBox("Wheel must be assigned.", MessageType.Error);

            DrawDefaultInspector();

            if (!isPrefab && targetScript.gameObject.activeInHierarchy) {
                showButtons = EditorGUILayout.Foldout(showButtons, "Quick Actions", boldFoldout);
                EditorGUI.indentLevel++;
                if (showButtons) {
                    if (GUILayout.Button("Get Wheel"))
                        foreach (var curTarget in allTargets)
                            curTarget.wheel = curTarget.transform.GetComponentInChildren<Wheel>();

                    if (GUILayout.Button("Get Opposite Wheel"))
                        foreach (var curTarget in allTargets) {
                            var vp = curTarget.transform.GetTopmostParentComponent<VehicleParent>();
                            Suspension closestOne = null;
                            var closeDist = Mathf.Infinity;

                            foreach (var curWheel in vp.wheels) {
                                var curDist = Vector2.Distance(
                                    new Vector2(curTarget.transform.localPosition.y,
                                        curTarget.transform.localPosition.z),
                                    new Vector2(curWheel.transform.parent.localPosition.y,
                                        curWheel.transform.parent.localPosition.z));
                                if (Mathf.Sign(curTarget.transform.localPosition.x) !=
                                    Mathf.Sign(curWheel.transform.parent.localPosition.x) && curDist < closeDist) {
                                    closeDist = curDist;
                                    closestOne = curWheel.transform.parent.GetComponent<Suspension>();
                                }
                            }

                            curTarget.oppositeWheel = closestOne;
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