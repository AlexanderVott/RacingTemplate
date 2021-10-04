using System;
using System.Collections.Generic;
using System.Linq;
using RedDev.Helpers.Extensions;
using RedDev.Kernel.Contexts;
using UnityEngine;

namespace RedDev.Kernel.Managers
{
	/// <summary>
	/// Класс описывает менеджер контекстов.
	/// При инициализации собираются базовые данные о контекстах и их префабах.
	/// Пути префабов берутся из массива констант и scriptableobject хранящего названия подкатегорий для поиска.
	/// </summary>
	public class ContextManager : BaseManager
	{
		private const string BASECONTEXT_CHECKER = "RedDev.Kernel.Contexts.BaseContext";
		private static readonly string[] RESOURCES_PATH_TO_UI_PREFAB =
		{
#if UNITY_STANDALONE
			"Prefabs/UI/",
#elif UNITY_ANDROID
			"Prefabs/UI/",
			//"Prefabs/UI/Android/",
#else
			"Prefabs/UI/",
#endif
		};

		public Action<BaseContext> onContextActivateChanged;

		private Canvas _uiRoot;
		private Dictionary<Type, BaseContext> _contexts = new Dictionary<Type, BaseContext>();
		
		public BaseContext this[Type type]
		{
			get
			{
				_contexts.TryGetValue(type, out var tmpContext);
				return tmpContext;
			}
		}

		public BaseContext this[string typeName]
		{
			get
			{
				if (String.IsNullOrEmpty(typeName))
					return null;

				var type = FindTypeByName(typeName);

				return type != null ? this[type] : null;
			}
		}

		protected void Awake()
		{
			var canvases = Resources.FindObjectsOfTypeAll<Canvas>();
			if (canvases == null || canvases.Length == 0)
			{
                Prod.LogError("[Contexts] Not found root canvas");
				return;
			}
			foreach (var canvas in canvases)
				if (canvas.name.Equals("root", StringComparison.InvariantCultureIgnoreCase))
				{
					_uiRoot = canvas;
					break;
				}
			if (_uiRoot != null)
			{
				if (_uiRoot.transform.root != null)
					_uiRoot.transform.root.gameObject.SetActive(true);
				_uiRoot.gameObject.SetActive(true);
			}
			var tmp = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
				.Where(p => typeof(BaseContext).IsAssignableFrom(p) && p.Namespace != null && !p.FullName.Equals(BASECONTEXT_CHECKER) && p.GetCustomAttributes(typeof(IgnorePreinitializeContext), true).Length == 0).ToList();
			foreach (var ctx in tmp)
				InitializeContext(ctx);
		}

		private GameObject LoadPrefab(string prefabName)
		{
			foreach (var uiPath in RESOURCES_PATH_TO_UI_PREFAB)
			{
				var path = uiPath + prefabName;
				var prefab = Resources.Load<GameObject>(path);
				if (prefab != null)
					return prefab;

                path = $"{uiPath}/{prefabName}";
                prefab = Resources.Load<GameObject>(path);
                if (prefab != null)
                    return prefab;
            }
			return null;
		}

		private GameObject InstantiateByType(Type contextType)
		{
			const string TypeNamePostfix = "Context";
			var prefabName = contextType.Name;

			if (prefabName.EndsWith(TypeNamePostfix))
				prefabName = prefabName.Substring(0, prefabName.Length - TypeNamePostfix.Length);

			var prefab = LoadPrefab(prefabName);
			if (prefab == null)
				throw new Exception("Could not find UI prefab: " + prefabName);

			var result = Instantiate(prefab, _uiRoot.transform);
			if (result == null)
				throw new Exception("Could not instantiate prefab: " + prefabName);

			result.transform.localPosition = Vector3.zero;
			result.transform.localScale = Vector3.one;

			return result;
		}

		/// <summary>
		/// Инициализирует контекст по указанному типу, создаёт префаб, добавляет в словарь и выключает объект.
		/// </summary>
		/// <param name="type">Тип контекста, который необходимо инициализировать.</param>
		/// <returns>Возвращает инициализированный объект контекста.</returns>
		private BaseContext InitializeContext(Type type)
		{
			var go = InstantiateByType(type);
			var context = go.GetComponent<BaseContext>();
			if (context == null)
				context = go.AddComponent(type) as BaseContext;
			if (context.GetType() != type)
				throw new Exception($"Prefab for {type.Name} contains wrong context of type {context.GetType().Name}");

			_contexts.Add(type, context);
			context.gameObject.SetActive(false);
			return context;
		}

		public Type FindTypeByName(string name)
		{
			foreach (var type in _contexts.Keys)
				if (type.Name.Equals(name))
					return type;
			return null;
		}

		private void ActivateContext(BaseContext context)
		{
			if (context.gameObject.activeSelf)
				throw new InvalidOperationException($"Context {context.GetType().Name} already activated");

			context.gameObject.SetActive(true);
			onContextActivateChanged.SafeCall(context);
		}

		private void DeactivateContext(BaseContext context)
		{
			if (!context.gameObject.activeSelf)
				return;

			context.gameObject.SetActive(false);
			onContextActivateChanged.SafeCall(context);
		}

		private BaseContext GetOrInitContext(Type type) => this[type] ?? InitializeContext(type);

        private void DisableContext(BaseContext context)
		{
			if (context != null)
				DeactivateContext(context);
		}

		public bool IsActive(Type type)
		{
			var context = this[type];
			return context != null && context.gameObject.activeSelf;
		}

		public bool IsActive<T>() where T : BaseContext => IsActive(typeof(T));

        public BaseContext Preload<T>() where T : BaseContext
		{
			var type = typeof(T);
			if (!_contexts.ContainsKey(type))
				return InitializeContext(type);

			Dev.LogWarning($"Context {type.Name} already loaded");
			return this[type];
		}

		public T Get<T>() where T : BaseContext => this[typeof(T)] as T;

        public BaseContext Push(Type type)
		{
			var context = GetOrInitContext(type);
			if (context != null)
				ActivateContext(context);
			return context;
		}

		public T Push<T>() where T : BaseContext => Push(typeof(T)) as T;

        private void PopContext(BaseContext context) => DisableContext(context);

        public void Pop<T>() where T : BaseContext
		{
			var context = this[typeof(T)];
			if (context != null)
				PopContext(context);
		}

		public void ClearAll()
		{
			foreach (var context in _contexts.Values)
			{
				DeactivateContext(context);
				Destroy(context.gameObject);
			}
			_contexts.Clear();
		}
	}
}