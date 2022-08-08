using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AltProg.CleanEmptyDir {
    [InitializeOnLoad]
    public class Core : UnityEditor.AssetModificationProcessor {
        private const string CleanOnSaveKey = "k1";

        public static event Action OnAutoClean;

        // UnityEditor.AssetModificationProcessor
        public static string[] OnWillSaveAssets(string[] paths) {
            if (CleanOnSave) {
                List<DirectoryInfo> emptyDirs;
                FillEmptyDirList(out emptyDirs);
                if (emptyDirs != null && emptyDirs.Count > 0) {
                    DeleteAllEmptyDirAndMeta(ref emptyDirs);

                    Debug.Log("[Clean] Cleaned Empty Directories on Save");

                    if (OnAutoClean != null)
                        OnAutoClean();
                }
            }

            return paths;
        }

        public static bool CleanOnSave {
            get { return EditorPrefs.GetBool(CleanOnSaveKey, false); }
            set { EditorPrefs.SetBool(CleanOnSaveKey, value); }
        }

        public static void DeleteAllEmptyDirAndMeta(ref List<DirectoryInfo> emptyDirs) {
            foreach (var dirInfo in emptyDirs) {
                AssetDatabase.MoveAssetToTrash(GetRelativePathFromCd(dirInfo.FullName));
            }

            emptyDirs = null;
        }

        public static void FillEmptyDirList(out List<DirectoryInfo> emptyDirs) {
            var newEmptyDirs = new List<DirectoryInfo>();
            emptyDirs = newEmptyDirs;

            var assetDir = new DirectoryInfo(Application.dataPath);

            WalkDirectoryTree(assetDir, (dirInfo, areSubDirsEmpty) => {
                bool isDirEmpty = areSubDirsEmpty && DirHasNoFile(dirInfo);
                if (isDirEmpty)
                    newEmptyDirs.Add(dirInfo);
                return isDirEmpty;
            });
        }

        // return: Is this directory empty?
        private delegate bool IsEmptyDirectory(DirectoryInfo dirInfo, bool areSubDirsEmpty);

        // return: Is this directory empty?
        private static bool WalkDirectoryTree(DirectoryInfo root, IsEmptyDirectory pred) {
            DirectoryInfo[] subDirs = root.GetDirectories();

            bool areSubDirsEmpty = true;
            foreach (DirectoryInfo dirInfo in subDirs) {
                if (false == WalkDirectoryTree(dirInfo, pred))
                    areSubDirsEmpty = false;
            }

            bool isRootEmpty = pred(root, areSubDirsEmpty);
            return isRootEmpty;
        }

        private static bool DirHasNoFile(DirectoryInfo dirInfo) {
            FileInfo[] files = null;

            try {
                files = dirInfo.GetFiles("*.*");
                files = files.Where(x => !IsMetaFile(x.Name) && !IsSystemFile(x.Name)).ToArray();
            }
            catch (Exception) { }

            return files == null || files.Length == 0;
        }

        private static string GetRelativePathFromCd(string filespec) {
            return GetRelativePath(filespec, Directory.GetCurrentDirectory());
        }

        public static string GetRelativePath(string filespec, string folder) {
            Uri pathUri = new Uri(filespec);
            // Folders must end in a slash
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString())) {
                folder += Path.DirectorySeparatorChar;
            }

            Uri folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        private static string GetMetaFilePath(string dirPath) {
            return dirPath.TrimEnd('/', '\\') + ".meta";
        }

        private static bool IsMetaFile(string path) {
            return path.EndsWith(".meta");
        }

        private static bool IsSystemFile(string path) {
            return path.StartsWith(".");
        }
    }
}