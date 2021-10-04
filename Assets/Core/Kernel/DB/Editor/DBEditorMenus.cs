using System;
using System.IO;
using System.Reflection;
using RedDev.Helpers.ToolsEditor;
using UnityEditor;
using UnityEngine;

namespace RedDev.Kernel.DB.Editor
{
	public static class DBEditorMenus
	{
		private static string CreateAndGetPathMeta(Type type)
		{
			var attr = type.GetCustomAttribute<MetaModelAttribute>();
			if (attr != null)
			{
				var path = "Assets/Resources/DB/" + attr.path;
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);
				return path;
			}
			else
			{
				Debug.LogError("Not found attribute MetaModel with path for meta data");
				return null;
			}
		}

		private static string GetResourcesPathMeta(Type type)
		{
			var attr = type.GetCustomAttribute<MetaModelAttribute>();
			if (attr != null)
				return $"DB/{attr.path}";
			else
			{
				Debug.LogError("Not found attribute MetaModel with path for meta data");
				return null;
			}
		}

		public static T CreateMetaBase<T>(string nameAsset = null) where T : BaseMetaDB, new()
		{
			var type = typeof(T);
			var asset = nameAsset ?? type.Name;
			var path = CreateAndGetPathMeta(type);
			if (!String.IsNullOrEmpty(path))
			{
				var result = ScriptableObjectsFactory.CreateAsset<T>(path + asset + ".asset");
				EditorGUIUtility.PingObject(result);
				return result;
			}

			return null;
		}

		public static T LoadMetaBase<T>(string nameAsset = null) where T : BaseMetaDB, new()
		{
			var type = typeof(T);
			var asset = nameAsset ?? type.Name;
			var path = GetResourcesPathMeta(type);
			if (!String.IsNullOrEmpty(path))
				return ScriptableObjectsFactory.LoadAsset<T>(path + asset);
			return null;
		}

		public static void RemoveMetaBase<T>() where T : BaseMetaDB, new()
		{
			var type = typeof(T);
			var path = CreateAndGetPathMeta(type);
			if (!String.IsNullOrEmpty(path))
				ScriptableObjectsFactory.RemoveAllAssets<T>();
		}

		public static void RemoveMeta<T>(string nameAsset)
		{
			var type = typeof(T);
			var path = CreateAndGetPathMeta(type);
			if (!String.IsNullOrEmpty(path) && !String.IsNullOrEmpty(nameAsset))
			{
				AssetDatabase.DeleteAsset(path + nameAsset);
			}
		}
	}
}