using System;
using System.Reflection;
using UnityEngine;

public static class BehaviorExtension {

	public static void BuildUpDI(this MonoBehaviour component)
	{
		Type type = component.GetType();

		MemberInfo[] members = type.FindMembers(MemberTypes.Property | MemberTypes.Field,
			BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null);

		foreach (MemberInfo member in members)
		{
			var attrs = member.GetCustomAttributes(typeof(DependencyAttribute), true);

			if (attrs.Length == 0)
				continue;

			if (member.MemberType == MemberTypes.Property)
			{
				if (((PropertyInfo)member).GetValue(component, null) != null)
					continue;
			}
			else if (member.MemberType == MemberTypes.Field)
			{
				if (((FieldInfo)member).GetValue(component) != null)
					continue;
			}

			Transform transformObj = null;

			var attrib = attrs[0] as DependencyAttribute;
			// * - поиск во всей сцене.
			if (String.IsNullOrEmpty(attrib.name) || attrib.name[0].Equals('*'))
			{
				var nameObj = "*";
				var obj = GameObject.Find(nameObj);
				if (obj == null)
				{
                    Dev.LogWarning($"DI Object for {component.gameObject.name} not found {attrib.name}");
					continue;
				}
				else
					transformObj = obj.transform;
			}
			else if (attrib.name.Equals("$"))
			{
				var property = member as PropertyInfo;
				var propType = property != null ? property.PropertyType : (member as FieldInfo).FieldType;
				var comp = component.GetComponent(propType);
				if (comp != null)
					transformObj = comp.transform;
			}
			else if (attrib.name.Equals("#"))
			{
				var property = member as PropertyInfo;
				var propType = property != null ? property.PropertyType : (member as FieldInfo).FieldType;
				var comp = component.GetComponent(propType);
				var parent = component.transform.parent;
				while (parent != null)
				{
					comp = parent.GetComponent(propType);
					if (comp == null)
						parent = parent.parent;
					else
						break;
				}
				if (comp != null)
					transformObj = comp.transform;
			}
			else
			{
				transformObj = component.transform.Find(attrib.name);
				if (transformObj == null)
				{
					Dev.LogWarning($"DI Element for {component.gameObject.name} not found {attrib.name}");
					continue;
				}
			}

			if (transformObj != null)
			{
				if (member.MemberType == MemberTypes.Property)
				{
					var property = (PropertyInfo) member;
					property.SetValue(component, transformObj.GetComponent(property.PropertyType));
				}
				else if (member.MemberType == MemberTypes.Field)
				{
					var field = (FieldInfo) member;
					field.SetValue(component, transformObj.GetComponent(field.FieldType));
				}
			}
		}
	}
}
