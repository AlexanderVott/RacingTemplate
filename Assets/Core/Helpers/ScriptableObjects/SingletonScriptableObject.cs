using UnityEngine;

namespace RedDev.Helpers
{
	public abstract class SingletonScriptableObject<T> : ScriptableObject where T: ScriptableObject
	{
		private static T _instance = null;

		public static T instance
		{
			get
			{
				if (_instance == null)
					_instance = Resources.Load($"Assets/Resources/ScriptableObjects/{typeof(T).Name}.asset") as T;
				return _instance;
			}
		}
	}
}