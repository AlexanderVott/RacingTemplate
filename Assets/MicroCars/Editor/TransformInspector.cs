//-------------------------------------------------
//			  NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace System.EditorExtensions.Internal {
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Transform), true)]
    public class TransformInspector : Editor {
        public static TransformInspector Instance;

        private SerializedProperty _pos;
        private SerializedProperty _rot;
        private SerializedProperty _scale;

        private void OnEnable() {
            Instance = this;

            if (this) {
                try {
                    var so = serializedObject;
                    _pos = so.FindProperty("m_LocalPosition");
                    _rot = so.FindProperty("m_LocalRotation");
                    _scale = so.FindProperty("m_LocalScale");
                }
                catch { }
            }
        }

        private void OnDestroy() {
            Instance = null;
        }

        /// <summary>
        /// Draw the inspector widget.
        /// </summary>
        public override void OnInspectorGUI() {
            EditorGUIUtility.labelWidth = 15f;
            serializedObject.Update();

            DrawPosition();
            DrawRotation();
            DrawScale();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPosition() {
            GUILayout.BeginHorizontal();
            var reset = GUILayout.Button("P", GUILayout.Width(20f));
            EditorGUILayout.PropertyField(_pos.FindPropertyRelative("x"));
            EditorGUILayout.PropertyField(_pos.FindPropertyRelative("y"));
            EditorGUILayout.PropertyField(_pos.FindPropertyRelative("z"));
            if (reset)
                _pos.vector3Value = Vector3.zero;
            GUILayout.EndHorizontal();
        }

        private void DrawScale() {
            GUILayout.BeginHorizontal();
            var reset = GUILayout.Button("S", GUILayout.Width(20f));
            EditorGUILayout.PropertyField(_scale.FindPropertyRelative("x"));
            EditorGUILayout.PropertyField(_scale.FindPropertyRelative("y"));
            EditorGUILayout.PropertyField(_scale.FindPropertyRelative("z"));
            if (reset)
                _scale.vector3Value = Vector3.one;
            GUILayout.EndHorizontal();
        }

        private enum Axes : int {
            None = 0,
            X = 1,
            Y = 2,
            Z = 4,
            All = 7,
        }

        private Axes CheckDifference(Transform t, Vector3 original) {
            var next = t.localEulerAngles;

            var axes = Axes.None;

            if (Differs(next.x, original.x))
                axes |= Axes.X;
            if (Differs(next.y, original.y))
                axes |= Axes.Y;
            if (Differs(next.z, original.z))
                axes |= Axes.Z;

            return axes;
        }

        private Axes CheckDifference(SerializedProperty property) {
            var axes = Axes.None;

            if (property.hasMultipleDifferentValues) {
                var original = property.quaternionValue.eulerAngles;

                foreach (var obj in serializedObject.targetObjects) {
                    axes |= CheckDifference(obj as Transform, original);
                    if (axes == Axes.All)
                        break;
                }
            }

            return axes;
        }

        /// <summary>
        /// Draw an editable float field.
        /// </summary>
        /// <param name="hidden">Whether to replace the value with a dash</param>
        /// <param name="greyedOut">Whether the value should be greyed out or not</param>
        private static bool FloatField(string name, ref float value, bool hidden, bool greyedOut, GUILayoutOption opt) {
            var newValue = value;
            GUI.changed = false;

            if (!hidden) {
                if (greyedOut) {
                    GUI.color = new Color(0.7f, 0.7f, 0.7f);
                    newValue = EditorGUILayout.FloatField(name, newValue, opt);
                    GUI.color = Color.white;
                }
                else {
                    newValue = EditorGUILayout.FloatField(name, newValue, opt);
                }
            }
            else if (greyedOut) {
                GUI.color = new Color(0.7f, 0.7f, 0.7f);
                float.TryParse(EditorGUILayout.TextField(name, "--", opt), out newValue);
                GUI.color = Color.white;
            }
            else {
                float.TryParse(EditorGUILayout.TextField(name, "--", opt), out newValue);
            }

            if (GUI.changed && Differs(newValue, value)) {
                value = newValue;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Because Mathf.Approximately is too sensitive.
        /// </summary>
        private static bool Differs(float a, float b) {
            return Mathf.Abs(a - b) > 0.0001f;
        }

        private void DrawRotation() {
            GUILayout.BeginHorizontal();
            {
                var reset = GUILayout.Button("R", GUILayout.Width(20f));

                var visible = (serializedObject.targetObject as Transform).localEulerAngles;

                visible.x = WrapAngle(visible.x);
                visible.y = WrapAngle(visible.y);
                visible.z = WrapAngle(visible.z);

                var changed = CheckDifference(_rot);
                var altered = Axes.None;

                var opt = GUILayout.MinWidth(30f);

                if (FloatField("X", ref visible.x, (changed & Axes.X) != 0, false, opt))
                    altered |= Axes.X;
                if (FloatField("Y", ref visible.y, (changed & Axes.Y) != 0, false, opt))
                    altered |= Axes.Y;
                if (FloatField("Z", ref visible.z, (changed & Axes.Z) != 0, false, opt))
                    altered |= Axes.Z;

                if (reset) {
                    _rot.quaternionValue = Quaternion.identity;
                }
                else if (altered != Axes.None) {
                    RegisterUndo("Change Rotation", serializedObject.targetObjects);

                    foreach (var obj in serializedObject.targetObjects) {
                        var t = obj as Transform;
                        var v = t.localEulerAngles;

                        if ((altered & Axes.X) != 0)
                            v.x = visible.x;
                        if ((altered & Axes.Y) != 0)
                            v.y = visible.y;
                        if ((altered & Axes.Z) != 0)
                            v.z = visible.z;

                        t.localEulerAngles = v;
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        private float WrapAngle(float angle) {
            while (angle > 180f)
                angle -= 360f;
            while (angle < -180f)
                angle += 360f;
            return angle;
        }

        private void RegisterUndo(string name, params UnityEngine.Object[] objects) {
            if (objects != null && objects.Length > 0) {
                Undo.RecordObjects(objects, name);

                foreach (var obj in objects) {
                    if (obj == null)
                        continue;

                    EditorUtility.SetDirty(obj);
                }
            }
        }
    }
}