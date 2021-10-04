using UnityEditor;

namespace System.EditorExtensions.Internal {
    [InitializeOnLoad]
    public static class UnityEditorHotkeys {
        static UnityEditorHotkeys() {
            EditorApplication.playModeStateChanged += RefreshAndRun;
        }

        private static void RefreshAndRun(PlayModeStateChange state) {
            if (state == PlayModeStateChange.ExitingEditMode) {
                if (!EditorPrefs.GetBool("kAutoRefresh")) {
                    RefreshImplementation();
                }
            }
        }

        [MenuItem("Tools/Editor Extensions/Step _F8")]
        private static void StepImplementation() {
            EditorApplication.ExecuteMenuItem("Edit/Step");
        }

        [MenuItem("Tools/Editor Extensions/Refresh and Play _F9")]
        private static void RefreshAndPlayImplementation() {
            RefreshImplementation();
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }

        [MenuItem("Tools/Editor Extensions/Pause _F11")]
        private static void PauseImplementation() {
            EditorApplication.ExecuteMenuItem("Edit/Pause");
        }

        private static void RefreshImplementation() {
            AssetDatabase.Refresh();
        }
    }
}