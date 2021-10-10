using System;
using System.Collections.Generic;
using System.Reflection;
using RedDev.Helpers.Extensions;

namespace RedDev.Helpers.DynamicTypes {
    using FieldDictionary = System.Collections.Generic.Dictionary<int /*Type.GetHashCode*/, System.Reflection.FieldInfo[]>;
    using MethodsDictionary = System.Collections.Generic.Dictionary<int /*Type.GetHashCode*/, System.Reflection.MethodInfo[]>;
    using PropertyDictionary = System.Collections.Generic.Dictionary<int /*Type.GetHashCode*/, System.Reflection.PropertyInfo[]>;

    public static class DynamicTypesManager {
        private static Dictionary<string, List<Type>> _dynamicTypesByName = new Dictionary<string, List<Type>>(64);
        private static Dictionary<Type, List<Type>> _typesByInterface = new Dictionary<Type, List<Type>>(64);

        private static List<Type> _cachedTypesResult = new List<Type>(64); // for return result

        private static Dictionary<BindingFlags, FieldDictionary> _fieldsCache = new Dictionary<BindingFlags, FieldDictionary>(64);
        private static Dictionary<BindingFlags, PropertyDictionary> _propertiesCache = new Dictionary<BindingFlags, PropertyDictionary>(64);
        private static Dictionary<BindingFlags, MethodsDictionary> _methodsCache = new Dictionary<BindingFlags, MethodsDictionary>(64);
        private static Dictionary<long /*MetadataToken*/, int> _propertiyIndexParamsCache = new Dictionary<long, int>(64);
        private static Dictionary<Int64 /*attributeType.Hash<<32|type.Get*/, bool> _isDefinedAttrCache = new Dictionary<Int64, bool>(256);

        private static object _lockedObject = new object();

        static DynamicTypesManager() {
            lock (_lockedObject) {
                InitiateDictionary();
            }
        }

        public static Type GetFirstDerivedType<T>(string name) {
            lock (_lockedObject) {
                List<Type> list;
                if (_dynamicTypesByName.TryGetValue(name, out list)) {
                    foreach (var t in list) {
                        if (t.IsSubclassOf(typeof(T))) {
                            return t;
                        }
                    }
                }

                return null;
            }
        }

        public static IList<Type> GetTypes(string name) {
            lock (_lockedObject) {
                if (_dynamicTypesByName.TryGetValue(name, out var types)) {
                    return types;
                }

                return null;
            }
        }

        public static IList<Type> GetInterfacedTypes<T>() {
            lock (_lockedObject) {
                List<Type> list;
                if (_typesByInterface.TryGetValue(typeof(T), out list)) {
                    return list;
                }

                return new List<Type>();
            }
        }

        public static FieldInfo[] GetCachedFields(this Type type, BindingFlags bindFlags) {
            lock (_lockedObject) {
                var fields = _fieldsCache.GetOrCreateDefault(bindFlags);

                FieldInfo[] result;
                int hash = type.GetHashCode();
                if (fields.TryGetValue(hash, out result)) {
                    return result;
                }

                result = type.GetFields(bindFlags);
                fields[hash] = result;
                return result;
            }
        }

        public static PropertyInfo[] GetCachedProperties(this Type type, BindingFlags bindFlags) {
            lock (_lockedObject) {
                var properties = _propertiesCache.GetOrCreateDefault(bindFlags);

                PropertyInfo[] result;
                int hash = type.GetHashCode();
                if (properties.TryGetValue(hash, out result)) {
                    return result;
                }

                result = type.GetProperties(bindFlags);
                properties[hash] = result;
                return result;
            }
        }

        public static bool GetCachedIsDefinedInherit(this MemberInfo type, Type attributeType) {
            lock (_lockedObject) {
                bool isDefined;

                int attributeHash = attributeType.GetHashCode();
                int typeHash = type.GetHashCode();
                long hash = ((long) (uint) attributeHash << 32) | (long) (uint) typeHash;
                if (!_isDefinedAttrCache.TryGetValue(hash, out isDefined)) {
                    isDefined = type.IsDefined(attributeType, true);
                    _isDefinedAttrCache[hash] = isDefined;
                    return isDefined;
                }

                return isDefined;
            }
        }

        public static int GetCachedPropertyIndexParamsCount(PropertyInfo property) {
            lock (_lockedObject) {
                // metadataToken для property уникальный только в рамках своего module, кроме того MetadataToken почему-то давало дубликаты для разных пропертей
                var key = MakeLong(property.Module.GetHashCode(), property.GetHashCode());
                if (_propertiyIndexParamsCache.TryGetValue(key, out var result)) {
                    return result;
                }

                result = property.GetIndexParameters().Length;
                _propertiyIndexParamsCache[key] = result;
                return result;
            }
        }

        public static MethodInfo[] GetCachedMethods(this Type type, BindingFlags bindFlags) {
            lock (_lockedObject) {
                var methods = _methodsCache.GetOrCreateDefault(bindFlags);

                MethodInfo[] result;
                int hash = type.GetHashCode();
                if (methods.TryGetValue(hash, out result)) {
                    return result;
                }

                result = type.GetMethods(bindFlags);
                methods[hash] = result;
                return result;
            }
        }

        private static Type[] GetAllDomainTypes() {
            List<Type> result = new List<Type>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
                result.AddRange(a.GetTypes());
            }

            return result.ToArray();
        }

        private static long MakeLong(int left, int right) {
            long res = left;
            res = (res << 32);
            res = res | (long) (uint) right;

            return res;
        }

        private static bool FastStringCompare(string a, string b) {
            // null-strings never occur in this case, so no null-string-checks here

            var aLength = a.Length;
            var bLength = b.Length;

            if (aLength != bLength) {
                return false;
            }

            for (int i = 0; i < aLength; i++) {
                if (a[i] != b[i]) {
                    return false;
                }
            }

            return true;
        }

        private static void InitiateDictionary() {
            lock (_lockedObject) {
                var types = GetAllDomainTypes();

                foreach (var t in types) {
                    if (t.IsAbstract) {
                        continue;
                    }

                    string typeName = t.Name;

                    _dynamicTypesByName.GetOrCreateDefault(typeName).Add(t);

                    foreach (Type i in t.GetInterfaces()) {
                        _typesByInterface.GetOrCreateDefault(i).Add(t);
                    }
                }
            }
        }

        private static bool CheckBaseType(this List<Type> set, Type type) {
            foreach (var l in set) {
                if (type.IsSubclassOf(l)) {
                    return true;
                }
            }

            return false;
        }
    }
}