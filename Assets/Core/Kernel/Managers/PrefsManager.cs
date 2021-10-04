using System;
using System.Collections.Generic;
using RedDev.Kernel.DB;
using UnityEngine;

namespace RedDev.Kernel.Managers
{
	public class PrefsManager : BaseManager
	{
		private Dictionary<int, IBasePrefsModel> _cached = new Dictionary<int, IBasePrefsModel>();

		public T Get<T>() where T: BasePrefsModel
		{
			var hash = typeof(T).GetHashCode();
			if (_cached.ContainsKey(hash))
				return _cached[hash] as T;
			else
				return Load<T>();
		}
		
		public T Load<T>() where T: BasePrefsModel
		{
			var type = typeof(T);
			var hash = type.GetHashCode();
			if (_cached.ContainsKey(hash))
				return _cached[hash] as T;
			else
			{
				var instance = Activator.CreateInstance(type) as T;
				if (instance != null)
				{
					instance.Load();
					_cached.Add(hash, instance);
				}
				else
                    Prod.LogError($"[Prefs] Prefs can't create instance of {type.Name}");
				return instance;
			}
		}

		public bool Save<T>() where T: class
		{
			var hash = typeof(T).GetHashCode();
			if (!_cached.ContainsKey(hash))
			{
                Prod.LogError($"[Prod] Not found {typeof(T).Name} for save");
				return false;
			}
			_cached[hash].Save();
			return true;
		}

		public void SaveAll()
		{
			foreach (var key in _cached.Keys)
				_cached[key].Save();
		}
	}
}