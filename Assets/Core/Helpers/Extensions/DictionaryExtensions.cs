using System;
using System.Collections.Generic;

#if !BASE_GLOBAL_EXTENSIONS
namespace RedDev.Helpers.Extensions {
#endif

    public static class DictionaryExtensions {
        public static V GetOrCreateDefault<K, V>(this Dictionary<K, V> dictionary, K key) {
            if (dictionary.TryGetValue(key, out var v)) {
                return v;
            }

            v = Activator.CreateInstance<V>();
            dictionary.Add(key, v);
            return v;
        }

        public static V Pop<K, V>(this Dictionary<K, V> dictionary, K key) {
            if (dictionary.TryGetValue(key, out var value)) {
                dictionary.Remove(key);
                return value;
            }

            return default(V);
        }

        public static void Push<K, V>(this Dictionary<K, V> dictionary, K key, V value) {
            dictionary[key] = value;
        }

        public static bool TryGetAndRemove<K, V>(this Dictionary<K, V> dictionary, K key, out V value) {
            if (dictionary.TryGetValue(key, out value)) {
                dictionary.Remove(key);
                return true;
            }

            return false;
        }
    }

#if !BASE_GLOBAL_EXTENSIONS
}
#endif