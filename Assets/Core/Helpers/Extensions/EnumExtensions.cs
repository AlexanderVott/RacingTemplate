using System;
using System.ComponentModel;

namespace RedDev.Helpers.Extensions
{
    public static class EnumHelpers {
        public static T Next<T>(this T value) where T : unmanaged, Enum => EnumTools<T>.Next(value);
        public static T Prev<T>(this T value) where T : unmanaged, Enum => EnumTools<T>.Prev(value);

        public static bool HasFlagNoAlloc<T>(this T value, T flag) where T : unmanaged, Enum {
            var src = EnumTools<T>.To<ulong>(value);
            var flg = EnumTools<T>.To<ulong>(flag);
            return (src & flg) == flg;
        }
    }

    public static class EnumTools<T> where T: unmanaged, Enum
	{
        private static class EnumValuesCache<TS> where TS : Enum {
            public static TS[] values { private set; get; } = (TS[])Enum.GetValues(typeof(TS));
            public static int count { private set; get; } = values.Length;
        }

        public static int count => EnumValuesCache<T>.count;

        public static T[] values => EnumValuesCache<T>.values;

        public static T From<S>(S value) where S : unmanaged {
            unsafe {
                if (sizeof(T) > sizeof(S)) {
                    // We might be spilling in the stack by taking more bytes than value provides,
                    // alloc the largest data-type and 'cast' that instead.
                    T o = default;
                    *((S*) &o) = value;
                    return o;
                }
                else {
                    return *(T*) &value;
                }
            }
        }

        public static TResult To<TResult>(T value) where TResult : unmanaged {
            unsafe {
                if (sizeof(TResult) > sizeof(T)) {
                    // We might be spilling in the stack by taking more bytes than value provides,
                    // alloc the largest data-type and 'cast' that instead.
                    TResult o = default;
                    *((T*)&o) = value;
                    return o;
                }
                else {
                    return *(TResult*)&value;
                }
            }
        }

        public static string GetDescription(T enumerationValue) {
            var type = enumerationValue.GetType();

            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0) {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0) {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return enumerationValue.ToString();
        }

        public static string GetName(T value) => Enum.GetName(typeof(T), value);

        public static string[] GetNames() => Enum.GetNames(typeof(T));

        public static T Next(T value) => values[(values.FindFirstIndex(value) + 1) % values.Length];
        public static T Prev(T value) => values[(values.FindFirstIndex(value) - 1) % values.Length];
    }
}