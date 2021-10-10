using System;
using System.Collections.Generic;
using RedDev.Helpers;
using RedDev.Helpers.Extensions;
using RedDev.Kernel.Interfaces;
using UnityEngine;

namespace RedDev.Kernel.Managers
{
	/// <summary>
	/// Ядро игрового движка.
	/// </summary>
	public class Core: Singleton<Core>
	{
		[SerializeField]
		private Dictionary<int, BaseManager> _managers = new Dictionary<int, BaseManager>();

        public bool IsApplicationQuitting { private set; get; } = false;
        public Action OnApplicationQuitEvent;

        private void Awake()
		{
			var handMakedManagers = GetComponentsInChildren<BaseManager>();
			foreach (var manager in handMakedManagers)
				Add(manager);
		}

        private void OnApplicationQuit() {
            Prod.Log("Handle OnApplicationQuit", this);
            IsApplicationQuitting = true;
            OnApplicationQuitEvent?.Invoke();
        }

        #region Add
        /// <summary>
        /// Добавляет в ядро менеджер указанного типа.
        /// </summary>
        /// <typeparam name="T">Тип добавляемого менеджера</typeparam>
        /// <returns>Возвращает экземпляр добавленного менеджера, если удалось его создать.</returns>
        public static T Add<T>() where T : BaseManager
		{
			var innerType = typeof(T);
			var hash = innerType.GetHashCode();
			var inst = Instance;
			if (!inst._managers.TryGetValue(hash, out var obj))
			{
				var gobj = new GameObject(innerType.Name);
				gobj.AddComponent(innerType);
				gobj.transform.parent = inst.transform;
				var result = gobj.GetComponent<T>();
				inst._managers.Add(hash, result);
				//result?.Attach();
				return result;
			} else
				return obj as T;
		}

		/// <summary>
		/// Добавляет созданный экземпляр менеджера в ядро.
		/// </summary>
		/// <param name="obj">Объект содержащий класс менеджера.</param>
		/// <returns>Возвращает объект содержащий класс менеджера.</returns>
		public static object Add(object obj)
		{
			var innerType = obj.GetType();
			var hash = innerType.GetHashCode();
			var inst = Instance;

			if (inst._managers.TryGetValue(hash, out var result))
				return result;

			inst._managers.Add(hash, obj as BaseManager);
			return obj;
		}
		#endregion

		public static void AttachManagers()
		{
			foreach (var key in Instance._managers.Keys)
			{
				var man = Instance._managers[key];
				if (man != null && !man.Initialized)
					man.Attach();
			}
		}

		#region Remove
		/// <summary>
		/// Удаление менеджера из ядра по указанному типу в качестве параметра или только дженерика.
		/// </summary>
		/// <typeparam name="T">Тип удаляемого менеджера.</typeparam>
		/// <param name="type">Вариант указания удаляемого менеджера через получаемый тип.</param>
		/// <returns>Возвращает true, если удалось удалить менеджер из ядра.</returns>
		public static bool Remove<T>(Type type = null) where T : BaseManager
		{
			var innerType = type == null ? typeof(T) : type;
			var hash = innerType.GetHashCode();
			return Instance._managers.Remove(hash);
		}

		/// <summary>
		/// Удаляет менеджер из ядра используя ссылку на экземпляр менеджера.
		/// </summary>
		/// <param name="obj">Объект содержащий в себе класс менеджера.</param>
		/// <returns>Возвращает true, если удалось удалить менеджер из ядра.</returns>
		public static bool Remove(object obj)
		{
			return Instance._managers.Remove(obj.GetType().GetHashCode());
		}
		#endregion

		/// <summary>
		/// Возвращает менеджер по указанному типу.
		/// </summary>
		/// <typeparam name="T">Тип искомого менеджера.</typeparam>
		/// <param name="type">Вариант получения менеджера через получаемый тип.</param>
		/// <returns>Возвращает менеджер в случае, если его удалось найти. В противном случае будет возвращён null.</returns>
		public static T Get<T>(Type type = null) where T : BaseManager
		{
			var innerType = type == null ? typeof(T) : type;
			var hash = innerType.GetHashCode();
			Instance._managers.TryGetValue(hash, out var result);
			return result as T;
		}

		/// <summary>
		/// Осуществляет проверку, присутствует ли указанный тип менеджера в ядре.
		/// </summary>
		/// <typeparam name="T">Тип искомого менеджера.</typeparam>
		/// <param name="type">Вариант проверки наличия менеджера через получаемый тип.</param>
		/// <returns>Возвращает true, если менеджер указанного типа присутствует в ядре.</returns>
		public static bool Contains<T>(Type type = null) where T : BaseManager
		{
			if (type == null)
				type = typeof(T);
			return Instance._managers.ContainsKey(type.GetHashCode());
		}

		/// <summary>
		/// Очищает полностью ядро от менеджеров.
		/// </summary>
		public static void Clear() => Instance._managers.Clear();

        /// <summary>
		/// Удаляет игровые сессионные менеджеры из ядра.
		/// </summary>
		public static void ClearSession()
		{
			var toWipe = new List<int>();
			var inst = Instance;

			foreach (var man in Instance._managers)
			{
				if (man.Value is IMustBeWipedOut)
				{
					(man.Value.gameObject).Destroy();
					toWipe.Add(man.Key);
				}

				var needToBeCleaned = man.Value as IDisposable;
				needToBeCleaned?.Dispose();
			}

			foreach (int wipeItem in toWipe)
				inst._managers.Remove(wipeItem);
		}
	}
}