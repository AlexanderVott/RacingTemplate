#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace RVP {
    [CustomEditor(typeof(DetachablePart))]
    [CanEditMultipleObjects]
    public class DetachablePartEditor : Editor {
        private static bool showHandles = true;

        public override void OnInspectorGUI() {
            showHandles = EditorGUILayout.Toggle("Show Handles", showHandles);
            SceneView.RepaintAll();

            DrawDefaultInspector();
        }

        public void OnSceneGUI() {
            var targetScript = (DetachablePart) target;
            Undo.RecordObject(targetScript, "Detachable Part Change");

            if (showHandles && targetScript.gameObject.activeInHierarchy)
                if (targetScript.joints != null)
                    foreach (var curJoint in targetScript.joints) {
                        if (Tools.current == Tool.Move)
                            curJoint.hingeAnchor = targetScript.transform.InverseTransformPoint(
                                Handles.PositionHandle(targetScript.transform.TransformPoint(curJoint.hingeAnchor),
                                    Tools.pivotRotation == PivotRotation.Local
                                        ? targetScript.transform.rotation
                                        : Quaternion.identity));
                        else if (Tools.current == Tool.Rotate)
                            curJoint.hingeAxis = targetScript.transform.InverseTransformDirection(
                                Handles.RotationHandle(
                                    Quaternion.LookRotation(
                                        targetScript.transform.TransformDirection(curJoint.hingeAxis),
                                        new Vector3(-targetScript.transform.TransformDirection(curJoint.hingeAxis).y,
                                            targetScript.transform.TransformDirection(curJoint.hingeAxis).x, 0)),
                                    targetScript.transform.TransformPoint(curJoint.hingeAnchor)) * Vector3.forward);
                    }

            if (GUI.changed)
                EditorUtility.SetDirty(targetScript);
        }
    }
}
#endif