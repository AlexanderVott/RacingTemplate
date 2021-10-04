using System;
using System.Collections.Generic;

namespace RedDev.Helpers
{
    /// <summary>
    /// Интерфейс описывает объект пула.
    /// </summary>
    /// <typeparam name="T">Объект пула.</typeparam>
    public interface IPoolObject<T>
    {
        /// <summary>
        /// Идентификатор группы.
        /// </summary>
        T Group { get; }

        /// <summary>
        /// Метод инициализирует объект.
        /// </summary>
        void Create();

        /// <summary>
        /// Метод обеспечивает отключение объекта при попадании в пул.
        /// </summary>
        void OnPush();

        /// <summary>
        /// Метод вызывается в случае невозможности попадания в пул.
        /// </summary>
        void OnFailedPush();
    }

    /// <summary>
    /// Класс реализует механизм пула.
    /// </summary>
    public class Pool<K, V> where V : IPoolObject<K>
    {
        /// <summary>
        /// Максимальное количество пул-объектов.
        /// </summary>
        public virtual int MaxInstances { get; protected set; }

        /// <summary>
        /// Текущее количество пул-объектов.
        /// </summary>
        public virtual int InstanceCount { get { return _objects.Count; } }

        /// <summary>
        /// Размер кэша.
        /// </summary>
        public virtual int CacheCount { get { return _cache.Count; } }

        /// <summary>
        /// Делегат сравнения.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public delegate bool Compare<T>(T value) where T : V;

        /// <summary>
        /// Сгруппированные списки пул-объектов.
        /// </summary>
        protected readonly Dictionary<K, List<V>> _objects = new Dictionary<K, List<V>>();

        /// <summary>
        /// Кэш объектов по типу.
        /// </summary>
        protected readonly Dictionary<Type, List<V>> _cache = new Dictionary<Type, List<V>>();

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="maxInstances">Максимальный размер пула.</param>
        public Pool(int maxInstances)
        {
            MaxInstances = maxInstances;
        }

        /// <summary>
        /// Метод обеспечивает проверку на возможность добавить объект в пул.
        /// </summary>
        /// <returns>Возвращает true, если пул имеет свободные места.</returns>
        public virtual bool CanPush()
        {
            return InstanceCount + 1 < MaxInstances;
        }

        /// <summary>
        /// Метод добавляет объект в пул.
        /// </summary>
        /// <param name="groupKey">Группа пула.</param>
        /// <param name="value">Объект пула.</param>
        /// <returns>Возвращает true в случае, если объект был добавлен.</returns>
        public virtual bool Push(K groupKey, V value)
        {
            bool result = false;

            if (CanPush())
            {
                value.OnPush();

                if (!_objects.ContainsKey(groupKey))
                    _objects.Add(groupKey, new List<V>());
                _objects[groupKey].Add(value);

                Type type = value.GetType();
                if (!_cache.ContainsKey(type))
                    _cache.Add(type, new List<V>());
                _cache[type].Add(value);
                result = true;
            }
            else
                value.OnFailedPush();

            return result;
        }

        /// <summary>
        /// Метод обеспечивает выдачу объекта из пула.
        /// </summary>
        /// <typeparam name="T">Объект пула.</typeparam>
        /// <returns>Возвращает объект пула.</returns>
        public virtual T Pop<T>() where T : V
        {
            T result = default(T);
            Type type = typeof(T);
            if (ValidateForPop(type))
            {
                for (int i = 0; i < _cache[type].Count; i++)
                {
                    result = (T)_cache[type][i];
                    if (result != null && _objects.ContainsKey(result.Group))
                    {
                        _objects[result.Group].Remove(result);
                        RemoveFromCache(result, type);
                        result.Create();
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Метод обеспечивает выдачу объекта из пула.
        /// </summary>
        /// <typeparam name="T">Объект пула.</typeparam>
        /// <param name="groupKey">Группа пула.</param>
        /// <returns>Возвращает объект пула.</returns>
        public virtual T Pop<T>(K groupKey) where T : V
        {
            T result = default(T);

            if (Contains(groupKey) && GroupCount(groupKey) > 0)
            {
                for (int i = 0; i < _objects[groupKey].Count; i++)
                {
                    if (_objects[groupKey][i] is T)
                    {
                        result = (T)_objects[groupKey][i];
                        Type type = result.GetType();
                        RemoveObject(groupKey, i);
                        RemoveFromCache(result, type);
                        result.Create();
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Метод обеспечивает выдачу объекта из пула по условию.
        /// </summary>
        /// <typeparam name="T">Объект пула.</typeparam>
        /// <param name="comparer">Метод сравнения.</param>
        /// <returns>Возвращает объект из пула, если условия выполняются.</returns>
        public virtual T Pop<T>(Compare<T> comparer) where T : V
        {
            T result = default(T);
            Type type = typeof(T);
            if (ValidateForPop(type))
            {
                for (int i = 0; i < _cache[type].Count; i++)
                {
                    T value = (T)_cache[type][i];
                    if (comparer(value))
                    {
                        _objects[value.Group].Remove(value);
                        RemoveFromCache(result, type);
                        result = value;
                        result.Create();
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Возвращает размер группы пула.
        /// </summary>
        /// <param name="groupKey">Группа пула.</param>
        /// <returns>Возвращает количество объектов в группе пула.</returns>
        public virtual int GroupCount(K groupKey)
        {
            return _objects[groupKey].Count;
        }

        /// <summary>
        /// Обеспечивает проку на наличие в пуля указанной группы.
        /// </summary>
        /// <param name="groupKey">Группа пула.</param>
        /// <returns>Возвращает true, если такая группа в пуле существует.</returns>
        public virtual bool Contains(K groupKey)
        {
            return _objects.ContainsKey(groupKey);
        }

        /// <summary>
        /// Очищает пул.
        /// </summary>
        public virtual void Clear()
        {
            _objects.Clear();
        }

        /// <summary>
        /// Обеспечивает проверку на выдачу из кэша пула.
        /// </summary>
        /// <param name="type">Тип объекта пула.</param>
        /// <returns>Возвращает true, если указанный тип существует в кэше пула и этот кэш не пустой.</returns>
        protected virtual bool ValidateForPop(Type type)
        {
            return _cache.ContainsKey(type) && _cache[type].Count > 0;
        }

        /// <summary>
        /// Удаляет объект из пула по индексу.
        /// </summary>
        /// <param name="groupKey">Группа пула.</param>
        /// <param name="index">Индекс в списке пула.</param>
        protected virtual void RemoveObject(K groupKey, int index)
        {
            if (index >= 0 && index < _objects[groupKey].Count)
            {
                _objects[groupKey].RemoveAt(index);
                if (_objects[groupKey].Count == 0)
                    _objects.Remove(groupKey);
            }
        }

        /// <summary>
        /// Удаляет объект из кэша пула.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type">Тип объекта пула.</param>
        protected void RemoveFromCache(V value, Type type)
        {
            if (_cache.ContainsKey(type))
            {
                _cache[type].Remove(value);
                if (_cache[type].Count == 0)
                    _cache.Remove(type);
            }
        }
    }
}