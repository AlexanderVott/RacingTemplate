using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using RedDev.Helpers.Extensions;
using RedDev.Helpers.ToolsEditor;
using RedDev.Kernel.DB;
using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif

namespace RedDev.Kernel.Managers
{
	/// <summary>
	/// Менеджер БД.
	/// </summary>
	public class DBManager : BaseManager
	{
		private Dictionary<int, IMetaDBHub> _hubs = new Dictionary<int, IMetaDBHub>();

		#region Load
		/// <summary>
		/// Загружает данные по типу хаба БД.
		/// </summary>
		/// <typeparam name="T">Тип хаба БД.</typeparam>
		/// <param name="path">Путь загрузки данных.</param>
		/// <returns>Возвращает экземпляр хаба БД.</returns>
		public T LoadHub<T>(string path) where T: class, IMetaDBHub, new()
		{
			var hash = typeof(T).GetHashCode();
			if (_hubs.ContainsKey(hash))
				return _hubs[hash] as T;
			else
			{
				var result = new T();
				result.Load($"DB/{path}");
				_hubs.Add(hash, result);
				return result;
			}
		}

		/// <summary>
		/// Загружает хаб БД по типу данных этого хаба. В качестве хаба используется базовый класс <see cref="MetaDbHub{T}"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="path"></param>
		/// <returns></returns>
		public MetaDbHub<T> Load<T>() where T : BaseMetaDB
		{
			var type = typeof(T);
			var attr = type.GetCustomAttribute<MetaModelAttribute>();
			if (attr != null)
			{
				return LoadHub<MetaDbHub<T>>(attr.path);
			}
			else
			{
                Prod.LogError($"[DB] Not found path attribute for db model {type.Name}");
				return null;
			}
		}

		#endregion

		/// <summary>
		/// Возвращает хаб по типу данных.
		/// </summary>
		/// <typeparam name="T">Тип хаба.</typeparam>
		/// <returns>Экземпляр хаба, если такой найден. В пртивном случае будет возвращён null.</returns>
		public T GetHub<T>() where T : class, IMetaDBHub, new()
		{
			var hash = typeof(T).GetHashCode();
			return _hubs.TryGetValue(hash, out var result) ? result as T : null;
		}

		/// <summary>
		/// Возвращает хаб данных по типу содержимого хаба.
		/// </summary>
		/// <typeparam name="T">Тип данных содержимого хаба.</typeparam>
		/// <returns>Возвращает экземпляр хаба.</returns>
		public MetaDbHub<T> Get<T>() where T : BaseMetaDB
		{
			return GetHub<MetaDbHub<T>>();
		}

		/// <summary>
		/// Возвращает данные по типу содержимого и цифрового идентификатора экземпляра.
		/// </summary>
		/// <typeparam name="T">Тип запрашиваемых данных.</typeparam>
		/// <param name="id">Цифровой идентификатор экземпляра данных.</param>
		/// <returns>Возвращает экземпляр запрашиваемых данные, условия поиска были выполнены успешно. В противном случае null.</returns>
		public T Get<T>(int id) where T : BaseMetaDB
		{
			var hub = GetHub<MetaDbHub<T>>();
			if (hub == null)
			{
                Prod.LogError($"[DB] Not found hub for type {typeof(T).Name}");
				return null;
			}
			return hub.Get(id);
		}

		/// <summary>
		/// Возвращает данные по типу содержимого и идентификатора экземпляра.
		/// </summary>
		/// <typeparam name="T">Тип запрашиваемых данных.</typeparam>
		/// <param name="identifier">Текстовый идентификатор экземпляра данных.</param>
		/// <returns>Возвращает экземпляр запрашиваемых данные, условия поиска были выполнены успешно. В противном случае null.</returns>
		public T Get<T>(string identifier) where T : BaseMetaDB
		{
			var hub = GetHub<MetaDbHub<T>>();
			return hub.Get(identifier);
		}

		/// <summary>
		/// Сборщик данных и преобразователь их в ScriptableObject ресурсы для загрузки в realtime.
		/// </summary>
#if UNITY_EDITOR
		[MenuItem("RedDev/DB/CollectDB")]
		[UnityEditor.Callbacks.DidReloadScripts]
		public static void CollectDB()
		{
			var metaType = typeof(IMetaJSON);
			var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
												.Where(p => metaType.IsAssignableFrom(p));
			ParseJSON(ref types);

			metaType = typeof(IMetaXML);
			types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
											.Where(p => metaType.IsAssignableFrom(p));
			ParseXML(ref types);
			AssetDatabase.SaveAssets();
		}
#endif

		#region Parsers
#if UNITY_EDITOR
		private static void ParseJSON(ref IEnumerable<Type> types)
		{
			foreach (var type in types)
			{
				var attrs = type.GetCustomAttributes(typeof(MetaModelAttribute), true);
				if (attrs.Length <= 0)
					continue;
				var metaModelAttr = attrs[0] as MetaModelAttribute;
				var path = $"DB/{metaModelAttr?.path}";
				var assetPath = $"Assets/Resources/{path}";
				if (String.IsNullOrEmpty(path))
				{
					Debug.Log($"В мета БД {type.Name} не указан атрибут сохранения данных");
					continue;
				}
				if (!Directory.Exists(assetPath))
				{
					Debug.Log($"Данных для обработки {type.Name} не найдено");
					continue;
				}
				var assets = Resources.LoadAll<TextAsset>(path);
				foreach (var asset in assets)
				{
					if (!File.Exists(assetPath + asset.name + ".json"))
						continue;
					var pathFile = $"{assetPath}{asset.name}.asset";
					var so = File.Exists(assetPath)
						? Resources.Load(path, type)
						: ScriptableObjectsFactory.CreateAsset(pathFile, type);
					if (so == null || !so.TryCatch(out object dbMeta))
						continue;
					try
					{
						JsonUtility.FromJsonOverwrite(asset.text, dbMeta);
					}
					catch (Exception except)
					{
						Debug.LogError($"Json парсинг для {asset.name} [{type.Name}] вызвал исключение: {except.Message}");
						continue;
					}
					EditorUtility.SetDirty(so);
				}
			}
		}

		private static void ParseXML(ref IEnumerable<Type> types)
		{
			foreach (var type in types)
			{
				var attrs = type.GetCustomAttributes(typeof(MetaModelAttribute), true);
				if (attrs.Length <= 0)
					continue;
				var metaModelAttr = attrs[0] as MetaModelAttribute;
				var path = $"DB/{metaModelAttr?.path}";
				var assetPath = $"Assets/Resources/{path}";
				if (String.IsNullOrEmpty(path))
				{
					Debug.Log($"В мета БД {type.Name} не указан атрибут сохранения данных");
					continue;
				}
				if (!Directory.Exists(assetPath))
				{
					Debug.Log($"Данных для обработки {type.Name} не найдено");
					continue;
				}
				var assets = Resources.LoadAll<TextAsset>(path);
				foreach (var asset in assets)
				{
					if (!File.Exists(assetPath + asset.name + ".xml"))
						continue;
					var pathFile = $"{assetPath}{asset.name}.asset";
					var so = File.Exists(assetPath)
						? Resources.Load(path, type)
						: ScriptableObjectsFactory.CreateAsset(pathFile, type);
					if (so == null || !so.TryCatch(out object dbMeta))
						continue;
					try
					{
						if (dbMeta is IMetaXML parser)
						{
							parser.Parse(asset.text);
							AssetDatabase.SaveAssets();
						}
						else
						{
							Debug.Log($"Типу {type.Name} не удалось вызвать парсер");
						}
					}
					catch (Exception except)
					{
						Debug.LogError($"XML парсинг {type.Name} вызвал исключение: {except.Message} - {except.StackTrace}");
						continue;
					}
					EditorUtility.SetDirty(so);
				}
			}
		}
#endif
#endregion
	}
}