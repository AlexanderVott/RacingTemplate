using System;
using UnityEngine;

namespace RedDev.Helpers
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
	public class SingletonSerializableAttribute : Attribute
	{
		public string path { get; private set; }

		public SingletonSerializableAttribute()
		{
		}

		public SingletonSerializableAttribute(string path)
		{
			this.path = path;
		}
	}

	public class SerializableSingleton<T, TD> : MonoBehaviour where T : MonoBehaviour where TD : class
	{
		protected static TD _instance;

		protected static T baseObject;

		private static object _lock;

		private static bool _appIsQuitting = false;

		public static bool saveOnExit { get; set; }

		public static TD Instance
		{
			get
			{
				if (_appIsQuitting)
				{
                    Prod.LogWarning($"[Singleton] Instance '{typeof(TD)}' already destroyed on application quit. Won't create again - returning null.");
					return null;
				}
				if (_instance == null)
				{
					Type type = typeof(TD);
					Type baseType = typeof(T);
					var path = "";

					var attrs = baseType.GetCustomAttributes(typeof(SingletonSerializableAttribute), true);

					if (attrs.Length > 0)
					{
						var attrib = attrs[0] as SingletonSerializableAttribute;
						if (!String.IsNullOrEmpty(attrib.path))
						{
							path = attrib.path;
						}
					}
					/*path = "Assets/Resources/" + path;
					if (!path.EndsWith("/"))
						path += "/";*/

					var file = path + baseType.Name;// + ".json";
					/*if (!File.Exists(file))
						throw new Exception($"File SerializableSingleton '{file}' not found");

					_instance = JSON.Parse<TD>(File.ReadAllText(file));*/
					var source = Resources.Load<TextAsset>(file);
					if (source == null)
					{
#if UNITY_EDITOR
#else
#endif
					}
					else
					{
						_instance = JsonUtility.FromJson<TD>(source?.text);
					}

					var obj = new GameObject(type.Name);
					baseObject = obj.AddComponent<T>();
					DontDestroyOnLoad(obj);
				}

				return _instance;
			}
		}

		void OnDestroy()
		{
			_appIsQuitting = true;
		}
	}
}