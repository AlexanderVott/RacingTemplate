using System;
using System.Collections;
using System.Collections.Generic;

namespace RedDev.Helpers.Extensions
{
	public static class CollectionExtensions
	{
		public static bool IsNullOrEmpty(this ICollection collection)
		{
			return collection == null || collection.Count < 1;
		}

		public static bool IsNullOrEmpty<T>(this T[] array)
		{
			return array == null || array.Length < 1;
		}

		public static List<TR> Map<T, TR>(this ICollection<T> collection, Func<T, TR> provider)
		{
			if (provider == null)
				throw new ArgumentException("Provider for Map is empty");

			var result = new List<TR>(collection.Count);

			foreach (T item in collection)
				result.Add(provider(item));

			return result;
		}

		public static TR Reduce<T, TR>(this ICollection<T> collection, TR initialValue, Func<T, TR, TR> func)
		{
			if (func == null)
				throw new ArgumentException("Func for Reduce is empty");

			TR accum = initialValue;
			foreach (T item in collection)
				accum = func(item, accum);

			return accum;
		}

		public static bool HasAny<T>(this ICollection<T> collection, Func<T, bool> func)
		{
			if (func == null)
				throw new ArgumentException("Func for HasAny is empty");

			foreach (T item in collection)
				if (func(item))
					return true;

			return false;
		}

		public static bool IsOneOf<T>(this T valueToFind, params T[] valuesToCheck)
		{
			foreach (var value in valuesToCheck)
				if (value.Equals(valueToFind))
					return true;
			return false;
		}

		public static bool IsNoneOf<T>(this T valueToFind, params T[] valuesToCheck)
		{
			return !IsOneOf(valueToFind, valuesToCheck);
		}

		public static T PopLast<T>(this IList<T> list)
		{
			var index = list.Count - 1;
			if (index < 0)
				return default(T);

			var popped = list[index];
			list.RemoveAt(index);
			return popped;
		}

		public static T Last<T>(this IList<T> list)
		{
			var index = list.Count - 1;
			if (index < 0)
				return default(T);

			return list[index];
		}

		public static int CountOf<T>(this IEnumerable<T> self, T itemToFind)
		{
			var result = 0;
			foreach (var item in self)
				if (itemToFind.Equals(item))
					result++;
			return result;
		}

		public static bool Contains<T>(this IEnumerable<T> self, T itemToFind)
		{
			foreach (var item in self)
				if (item.Equals(itemToFind))
					return true;
			return false;
		}

		public static bool Contains(this Hashtable self, string key)
		{
			foreach (var item in self)
				if (item.ToString() == key)
					return true;
			return false;
		}

		public static void AddFromEnumOfSameType<T>(this List<T> self)
		{
			foreach (T enumElement in Enum.GetValues(typeof(T)))
			{
				self.Add(enumElement);
			}
		}
	}
}