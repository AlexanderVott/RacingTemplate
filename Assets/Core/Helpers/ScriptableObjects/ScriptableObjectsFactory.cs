using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif

namespace RedDev.Helpers.ToolsEditor
{
	public static class ScriptableObjectsFactory
	{
		public static T CreateAsset<T>(string path) where T : ScriptableObject
		{
#if UNITY_EDITOR
			var asset = ScriptableObject.CreateInstance<T>();

			if (String.IsNullOrEmpty(path))
				path = "Assets";

			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();
			return asset;
#else
			return null;
#endif
		}

		public static string[] GetAssetsPaths<T>() where T : ScriptableObject
		{
			var resources = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
			return resources.Select(AssetDatabase.GUIDToAssetPath).ToArray();
		}

		public static string[] GetAssets<T>() where T : ScriptableObject
		{
			return GetAssetsPaths<T>().Select(x =>
			{
				var match = Regex.Match(x, @"(\w+).asset");
				return match.Groups.Count > 1 ? match.Groups[1].Value : x;
			}).ToArray();
		}

		public static T LoadAsset<T>(string path) where T: ScriptableObject 
            => Resources.Load<T>(path);

        public static void RemoveAllAssets<T>() where T: ScriptableObject
		{
#if UNITY_EDITOR
			var resources = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
			foreach (var asset in resources)
				AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(asset));
#endif
		}

		public static ScriptableObject CreateAsset(string path, Type type)
		{
			var asset = ScriptableObject.CreateInstance(type);

#if UNITY_EDITOR
			if (String.IsNullOrEmpty(path))
				path = "Assets";

			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();
#endif
			return asset;
		}

		public static void SaveAsset(ScriptableObject target, string path, Type type)
		{
#if UNITY_EDITOR
			if (String.IsNullOrEmpty(path))
				path = "Assets";

			AssetDatabase.CreateAsset(target, path);
			AssetDatabase.SaveAssets();
#endif
		}

#if UNITY_EDITOR
		[MenuItem("RedDev/ScriptableObjects/Create")]
		[MenuItem("Assets/Create/ScriptableObject")]
#endif
		public static void Create()
		{
#if UNITY_EDITOR
			var assembly = GetAssembly();

			// Get all classes derived from ScriptableObject
			var allScriptableObjects = (from t in assembly.GetTypes()
				where t.IsSubclassOf(typeof(ScriptableObject))
				select t).ToArray();

			// Show the selection window.
			var window = EditorWindow.GetWindow<ScriptableObjectsFactoryWindow>(true, "Create a new ScriptableObject", true);
			window.ShowPopup();

			window.Types = allScriptableObjects;
#endif
		}

		/// <summary>
		/// Returns the assembly that contains the script code for this project (currently hard coded)
		/// </summary>
		private static Assembly GetAssembly() 
            => Assembly.Load(new AssemblyName("Assembly-CSharp"));
    }
}