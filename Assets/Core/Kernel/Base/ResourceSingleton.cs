using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;

#endif

namespace RedDev.Kernel.Base {
    public abstract class ResourceSingleton<T> : ScriptableObject where T : ScriptableObject {
        private static T _instance;

        public static T instance {
            get {
                LoadAsset();

                if (_instance == null)
                    throw new System.ArgumentNullException(
                        $"Could't load asset from ResourceSingleton {typeof(T).Name}");

                return _instance;
            }
        }

        private static void LoadAsset() {
            if (Application.isPlaying)
                if (!_instance)
                    _instance = Resources.Load(typeof(T).Name) as T;

#if UNITY_EDITOR
            if (!_instance) {
                ResourceSingletonBuilder.BuildResourceSingletonsIfDirty(); // ensure that singletons were built

                var temp = CreateInstance<T>();
                var monoscript = MonoScript.FromScriptableObject(temp);
                DestroyImmediate(temp);
                var scriptPath = AssetDatabase.GetAssetPath(monoscript);
                //TODO: ввести атрибут который может указать кастомный путь
                var assetDir = Path.GetDirectoryName(scriptPath) + "/Resources/";
                var assetPath = assetDir + Path.GetFileNameWithoutExtension(scriptPath) + ".asset";
                _instance = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
#endif
        }
    }

    #region internal

#if UNITY_EDITOR
    public class ResourceSingletonBuilder // : UnityEditor.AssetPostprocessor
    {
        private static bool _hasRun = false;

        /*
        prvate static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) 
        {
            if(importedAssets.Concat(movedAssets).Any(x=> x.EndsWith(".cs") || x.EndsWith(".js")))
            {
                BuildResourceSingletonsIfDirty();
            }
        }
        */
        [UnityEditor.Callbacks.DidReloadScripts]
        public static void BuildResourceSingletonsIfDirty() {
            if (_hasRun)
                return;

            BuildResourceSingletons();
        }

        public static void BuildResourceSingletons() {
            var result = System.Reflection.Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => !t.IsAbstract && GetBaseType(t, typeof(ResourceSingleton<>)));

            var method = typeof(ResourceSingletonBuilder).GetMethod("BuildOrMoveAsset",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (method == null) {
                EditorApplication.delayCall += BuildResourceSingletons;
                return;
            }

            foreach (var i in result) {
                var generic = method.MakeGenericMethod(new System.Type[] {i});
                generic.Invoke(null, new object[0]);
            }

            _hasRun = true;
        }

        private static bool GetBaseType(System.Type type, System.Type baseType) {
            if (type == null || baseType == null || type == baseType)
                return false;

            if (baseType.IsGenericType == false) {
                if (type.IsGenericType == false)
                    return type.IsSubclassOf(baseType);
            }
            else {
                baseType = baseType.GetGenericTypeDefinition();
            }

            type = type.BaseType;
            var objectType = typeof(object);
            while (type != objectType && type != null) {
                var curentType = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (curentType == baseType)
                    return true;

                type = type.BaseType;
            }

            return false;
        }

        private static void BuildOrMoveAsset<T>() where T : ScriptableObject {
            var instance = Resources.Load(typeof(T).Name) as T;

            var temp = ScriptableObject.CreateInstance<T>();
            var monoscript = MonoScript.FromScriptableObject(temp);
            Object.DestroyImmediate(temp);
            if (monoscript == null) {
                Debug.LogError($"Couldn't find script named {typeof(T).Name}.cs");
                return;
            }

            var scriptPath = AssetDatabase.GetAssetPath(monoscript);

            var assetDir = Path.GetDirectoryName(scriptPath) + "/Resources/";
            var assetPath = assetDir + Path.GetFileNameWithoutExtension(scriptPath) + ".asset";
            Directory.CreateDirectory(assetDir);
            var assetPathInstance = AssetDatabase.GetAssetPath(instance);
            assetPath = assetPath.Replace("\\", "/");

            if (instance && assetPathInstance != assetPath) {
                Debug.Log($"ResourceSingleton: Moving asset: {typeof(T).Name} from {assetPathInstance} to {assetPath}");
                FileUtil.MoveFileOrDirectory(assetPathInstance, assetPath);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }

            if (!instance && !File.Exists(assetPath)) {
                Debug.Log($"ResourceSingleton: Creating asset: {typeof(T).Name} at {assetPath}");
                instance = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(instance, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }
    }
#endif

    #endregion
}