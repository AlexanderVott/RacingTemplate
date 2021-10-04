using System;
using RedDev.Helpers.Extensions;
using RedDev.Helpers.ToolsEditor;
using UnityEngine;

namespace RedDev.Kernel.DB
{
	public class MetaDbHub<T> : IMetaDBHub<T> where T : ScriptableObject, IMetaDB
	{
		public T[] metaData { get; protected set; }
		
		/// <summary>
		/// Загружает данные по указанному пути.
		/// </summary>
		/// <param name="path">Путь указывающий на место хранения данных.</param>
		public virtual void Load(string path)
		{
			var type = typeof(T);
#if UNITY_EDITOR
			if (type.IsAssignableFrom(typeof(IMetaJSON)))
			{
				var assets = Resources.LoadAll<TextAsset>(path);
				metaData = new T[assets.Length];
				for (int i = 0; i < assets.Length; i++)
				{
					var asset = assets[i];
					var meta = ScriptableObjectsFactory.CreateAsset<T>(path + asset.name + ".asset");
					JsonUtility.FromJsonOverwrite(asset.text, meta);
					metaData[i] = meta;
				}
				//TODO: грузить и SO с проверкой на уже наличествующий ID в списке
			} else if (type.IsAssignableFrom(typeof(IMetaXML)))
			{

			}
			else
#endif
			{
				metaData = Resources.LoadAll<T>(path);
				foreach (var meta in metaData)
					meta.PreLoad();
			}

		}

		/// <inheritdoc />
		public virtual T Get(int id)
		{
			foreach (var meta in metaData)
				if (meta.Id == id)
					return meta;
			return null;
		}

		/// <inheritdoc />
		public virtual T Get(string identifier)
		{
			foreach (var meta in metaData)
				if (meta.Identifier.Equals(identifier, StringComparison.OrdinalIgnoreCase))
					return meta;
			return null;
		}

		/// <inheritdoc />
		public virtual T[] Get(Predicate<T> match)
		{
			return metaData.Filter(match);
		}

		/// <inheritdoc/>
		public virtual T[] Get()
		{
			return metaData;
		}

		/// <inheritdoc />
		public T this[int index] => Get(index);

		/// <inheritdoc />
		public T this[string identifier] => Get(identifier);

		/// <inheritdoc />
		public T[] this[Predicate<T> match] => Get(match);
	}
}