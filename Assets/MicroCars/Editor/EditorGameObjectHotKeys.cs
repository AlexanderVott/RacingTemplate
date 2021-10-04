using UnityEditor;
using UnityEngine;
using TMPro;
using RedDev.Helpers.Extensions;

namespace System.EditorExtensions.Internal {
    public static class EditorGameObjectHotKeys {
        private static Component[] _copiedComponents;

        [MenuItem("GameObject/Replace text components %&T")]
        private static void ReplaceText() {
            DoReplaceText(Selection.activeGameObject);
        }

        private static void DoReplaceText(GameObject go) {
            ReplaceText(go);
            ReplaceInput(go);
            foreach (Transform itr in go.transform) {
                DoReplaceText(itr.gameObject);
            }
        }

        private static void ReplaceInput(GameObject go) {
            var input = go.GetComponent<TMP_InputField>(); 
            if (input == null) {
                return;
            }

            input.textViewport = input.textComponent?.transform as RectTransform;
            EditorUtility.SetDirty(go);
        }

        private static void ReplaceText(GameObject go) {
            var textComponent = go.GetComponent<UnityEngine.UI.Text>();
            if (textComponent == null) {
                return;
            }

            var text = textComponent.text;
            var size = textComponent.fontSize;
            var lineSpacing = textComponent.lineSpacing;
            var alignment = textComponent.alignment;
            GameObject.DestroyImmediate(textComponent);

            var uguiComponent = go.GetOrAddComponent<TextMeshProUGUI>();
            uguiComponent.text = text;
            uguiComponent.fontSize = size;
            uguiComponent.lineSpacing = lineSpacing;

            switch (alignment) {
                case TextAnchor.UpperLeft: uguiComponent.horizontalAlignment = HorizontalAlignmentOptions.Left; uguiComponent.verticalAlignment = VerticalAlignmentOptions.Top; break;
                case TextAnchor.UpperCenter: uguiComponent.horizontalAlignment = HorizontalAlignmentOptions.Center; uguiComponent.verticalAlignment = VerticalAlignmentOptions.Top; break;
                case TextAnchor.UpperRight: uguiComponent.horizontalAlignment = HorizontalAlignmentOptions.Right; uguiComponent.verticalAlignment = VerticalAlignmentOptions.Top; break;

                case TextAnchor.MiddleLeft: uguiComponent.horizontalAlignment = HorizontalAlignmentOptions.Left; uguiComponent.verticalAlignment = VerticalAlignmentOptions.Middle; break;
                case TextAnchor.MiddleCenter: uguiComponent.horizontalAlignment = HorizontalAlignmentOptions.Center; uguiComponent.verticalAlignment = VerticalAlignmentOptions.Middle; break;
                case TextAnchor.MiddleRight: uguiComponent.horizontalAlignment = HorizontalAlignmentOptions.Right; uguiComponent.verticalAlignment = VerticalAlignmentOptions.Middle; break;

                case TextAnchor.LowerLeft: uguiComponent.horizontalAlignment = HorizontalAlignmentOptions.Left; uguiComponent.verticalAlignment = VerticalAlignmentOptions.Bottom; break;
                case TextAnchor.LowerCenter: uguiComponent.horizontalAlignment = HorizontalAlignmentOptions.Center; uguiComponent.verticalAlignment = VerticalAlignmentOptions.Bottom; break;
                case TextAnchor.LowerRight: uguiComponent.horizontalAlignment = HorizontalAlignmentOptions.Right; uguiComponent.verticalAlignment = VerticalAlignmentOptions.Bottom; break;
            }
            
        }

        [MenuItem("GameObject/Unpack prefab %&X")]
        public static void Unpack() {
            PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
        }

        [MenuItem("GameObject/Copy all components %&C")]
        public static void Copy() {
            _copiedComponents = Selection.activeGameObject.GetComponents<Component>();
        }

        [MenuItem("GameObject/Paste all components %&V")]
        public static void Paste() {
            if (_copiedComponents == null) {
                return;
            }

            foreach (var targetGameObject in Selection.gameObjects) {
                if (!targetGameObject)
                    continue;

                foreach (var copiedComponent in _copiedComponents) {
                    if (!copiedComponent)
                        continue;

                    UnityEditorInternal.ComponentUtility.CopyComponent(copiedComponent);
                    var exist = targetGameObject.GetComponent(copiedComponent.GetType());
                    if (exist) {
                        UnityEditorInternal.ComponentUtility.PasteComponentValues(exist);
                    }
                    else {
                        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetGameObject);
                    }
                }
            }
        }
    }
}