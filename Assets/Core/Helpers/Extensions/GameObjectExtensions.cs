using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace RedDev.Helpers.Extensions
{
	public static class GameObjectExtensions
	{
		public static T GetOrAddComponent<T>(this GameObject self) where T : Component
		{
			var result = self.GetComponent<T>();
			if (!result)
				result = self.AddComponent<T>();
			return result;
		}

		public static Component GetOrAddComponent(this GameObject self, Type componentType)
		{
			var result = self.GetComponent(componentType);
			if (!result)
				result = self.AddComponent(componentType);
			return result;
		}

		public static void ChangeActive(this GameObject self, bool state)
		{
			if (self.activeSelf != state)
				self.SetActive(state);
		}

		public static void SwitchActive(this GameObject self)
		{
			self.SetActive(!self.activeSelf);
		}

		public static void ToggleActiveWithEvent(this GameObject self, bool value, UnityAction callback)
		{
			var activeSelf = self.activeSelf;

			if (value)
			{
				if (!activeSelf)
				{
					self.SetActive(true);
					callback();
				}
			}
			else
			{
				if (activeSelf)
					self.SetActive(false);
			}
		}

		public static void Destroy(this GameObject self)
		{
#if UNITY_EDITOR
			Object.DestroyImmediate(self);
#else
			Object.Destroy(self);
#endif
		}

		public static GameObject FindChildWithTag(this GameObject parent, string tag)
		{
			if (parent == null)
				throw new System.ArgumentNullException();
			if (string.IsNullOrEmpty(tag))
				throw new System.ArgumentNullException();

			for (int i = 0; i < parent.transform.childCount; i++)
			{
				var child = parent.transform.GetChild(i);
				if (child.gameObject.CompareTag(tag))
					return child.gameObject;
			}

			return null;
		}

		public static GameObject[] FindChildsWithTag(this GameObject parent, string tag)
		{
			if (parent == null)
				throw new System.ArgumentNullException();
			if (string.IsNullOrEmpty(tag))
				throw new System.ArgumentNullException();

			List<GameObject> result = new List<GameObject>();
			for (int i = 0; i < parent.transform.childCount; i++)
			{
				var child = parent.transform.GetChild(i);
				if (child.gameObject.CompareTag(tag))
					result.Add(child.gameObject);
				result.AddRange(child.gameObject.FindChildsWithTag(tag));
			}

			return result.ToArray();
		}

		public static T[] FindComponentsInChildrenWithTag<T>(this GameObject parent, string tag, bool forceActive = false) where T : Component
		{
			if (parent == null) { throw new System.ArgumentNullException(); }
			if (string.IsNullOrEmpty(tag) == true) { throw new System.ArgumentNullException(); }
			List<T> list = new List<T>(parent.GetComponentsInChildren<T>(forceActive));
			if (list.Count == 0) { return null; }

			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (list[i].CompareTag(tag) == false)
				{
					list.RemoveAt(i);
				}
			}
			return list.ToArray();
		}

		public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag, bool forceActive = false) where T : Component
		{
			if (parent == null) { throw new System.ArgumentNullException(); }
			if (string.IsNullOrEmpty(tag) == true) { throw new System.ArgumentNullException(); }

			var list = parent.GetComponentsInChildren<T>(forceActive);
			foreach (T t in list)
			{
				if (t.CompareTag(tag) == true)
				{
					return t;
				}
			}
			return null;
		}
	}
}