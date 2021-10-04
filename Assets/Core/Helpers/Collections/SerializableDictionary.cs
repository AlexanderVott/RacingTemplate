using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedDev.Helpers.Collections
{
	/// <inheritdoc />
	/// <summary>
	/// Класс словаря с возможностью сериализации. Не сериализует пары, в которой присутствует пустое значение.
	/// </summary>
	/// <typeparam name="TK"></typeparam>
	/// <typeparam name="TV"></typeparam>
	[Serializable]
	public class SerializableDictionary<TK, TV>: Dictionary<TK, TV>
	{
		[SerializeField]
		private List<TK> _keysList = new List<TK>();
		[SerializeField]
		private List<TV> _valuesList = new List<TV>();

		public void OnBeforeSerialize()
		{
			_keysList.Clear();
			_valuesList.Clear();

			foreach (var kvp in this)
			{
				if (kvp.Value == null)
					continue;
				_keysList.Add(kvp.Key);
				_valuesList.Add(kvp.Value);
			}
		}

		public void OnAfterDeserialize()
		{
			Clear();
			for (var i = 0; i != Math.Min(_keysList.Count, _valuesList.Count); i++)
				Add(_keysList[i], _valuesList[i]);
		}
	}
}