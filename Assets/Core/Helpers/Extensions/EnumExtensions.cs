using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace RedDev.Helpers.Extensions
{
	public static class EnumExt<T> where T: struct, IConvertible
	{
		public static int Count
		{
			get
			{
				if (!typeof(T).IsEnum)
					throw new ArgumentException("T must be an enumerated type");
				return Enum.GetNames(typeof(T)).Length;
			}
		}

		public static string[] GetNames()
		{
			return Enum.GetNames(typeof(T));
		}

		public static string GetName(T value)
		{
			return Enum.GetName(typeof(T), value);
		}

		public static string GetDescription(T enumerationValue)
		{
			Type type = enumerationValue.GetType();
			if (!type.IsEnum)
			{
				throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
			}

			//Tries to find a DescriptionAttribute for a potential friendly name
			//for the enum
			MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString(CultureInfo.InvariantCulture));
			if (memberInfo.Length > 0)
			{
				object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

				if (attrs != null && attrs.Length > 0)
				{
					//Pull out the description value
					return ((DescriptionAttribute)attrs[0]).Description;
				}
			}
			//If we have no description attribute, just return the ToString of the enum
			return enumerationValue.ToString(CultureInfo.InvariantCulture);
		}
	}
}