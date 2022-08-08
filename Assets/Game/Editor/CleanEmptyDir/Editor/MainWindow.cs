using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AltProg.CleanEmptyDir {
    public class MainWindow : EditorWindow {
        private List<DirectoryInfo> _emptyDirs;
        private Vector2 _scrollPosition;
        private bool _lastCleanOnSave;
        private string _delayedNotiMsg;
        private GUIStyle _updateMsgStyle;

        private bool _hasNoEmptyDir {
            get { return _emptyDirs == null || _emptyDirs.Count == 0; }
        }

        private const float DirLabelHeight = 21;

        [MenuItem("Tools/Editor Extensions/Clean Empty Dir")]
        public static void ShowWindow() {
            var w = GetWindow<MainWindow>();
#if UNITY_5
            w.titleContent = new GUIContent("Clean");
#else
            w.titleContent.text = "Clean";
#endif
        }

        private void OnEnable() {
            _lastCleanOnSave = Core.CleanOnSave;
            Core.OnAutoClean += Core_OnAutoClean;
            _delayedNotiMsg = "Click 'Find Empty Dirs' Button.";
        }

        private void OnDisable() {
            Core.CleanOnSave = _lastCleanOnSave;
            Core.OnAutoClean -= Core_OnAutoClean;
        }

        private void Core_OnAutoClean() {
            _delayedNotiMsg = "Cleaned on Save";
        }

        private void OnGUI() {
            if (_delayedNotiMsg != null) {
                ShowNotification(new GUIContent(_delayedNotiMsg));
                _delayedNotiMsg = null;
            }

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Find Empty Dirs")) {
                        Core.FillEmptyDirList(out _emptyDirs);

                        if (_hasNoEmptyDir) {
                            ShowNotification(new GUIContent("No Empty Directory"));
                        }
                        else {
                            RemoveNotification();
                        }
                    }

                    if (ColorButton("Delete All", !_hasNoEmptyDir, Color.red)) {
                        Core.DeleteAllEmptyDirAndMeta(ref _emptyDirs);
                        ShowNotification(new GUIContent("Deleted All"));
                    }
                }
                EditorGUILayout.EndHorizontal();

                bool cleanOnSave = GUILayout.Toggle(_lastCleanOnSave, " Clean Empty Dirs Automatically On Save");
                if (cleanOnSave != _lastCleanOnSave) {
                    _lastCleanOnSave = cleanOnSave;
                    Core.CleanOnSave = cleanOnSave;
                }

                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

                if (!_hasNoEmptyDir) {
                    _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandWidth(true));
                    {
                        EditorGUILayout.BeginVertical();
                        {
#if UNITY_4_6 || UNITY_4_7 // and higher
                            GUIContent folderContent = EditorGUIUtility.IconContent("Folder Icon");
#else
                            GUIContent folderContent = new GUIContent();
#endif

                            foreach (var dirInfo in _emptyDirs) {
                                UnityEngine.Object assetObj = AssetDatabase.LoadAssetAtPath("Assets", typeof(UnityEngine.Object));
                                if (null != assetObj) {
                                    folderContent.text = Core.GetRelativePath(dirInfo.FullName, Application.dataPath);
                                    GUILayout.Label(folderContent, GUILayout.Height(DirLabelHeight));
                                }
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndScrollView();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void ColorLabel(string title, Color color) {
            Color oldColor = GUI.color;
            //GUI.color = color;
            GUI.enabled = false;
            GUILayout.Label(title);
            GUI.enabled = true;
            ;
            GUI.color = oldColor;
        }

        private bool ColorButton(string title, bool enabled, Color color) {
            bool oldEnabled = GUI.enabled;
            Color oldColor = GUI.color;

            GUI.enabled = enabled;
            GUI.color = color;

            bool ret = GUILayout.Button(title);

            GUI.enabled = oldEnabled;
            GUI.color = oldColor;

            return ret;
        }
    }
}