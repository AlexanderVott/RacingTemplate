using RedDev.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RedDev.ManagersEngine
{
    /// <summary>
    /// Класс реализует менеджер пула.
    /// </summary>
    public class PoolManager : Singleton<PoolManager>
    {
        /// <summary>
        /// Максимальный размер пула объектов.
        /// </summary>
        public readonly int MaxInstanceCount = 24;

        private Pool<string, ObjectPool> _pool;
        /// <summary>
        /// Пул объектов.
        /// </summary>
        public Pool<string, ObjectPool> Pool { get { return _pool; } }

        protected override void Awake()
        {
            base.Awake();
            _pool = new Pool<string, ObjectPool>(MaxInstanceCount);
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                Clear();
            };
        }

        /// <summary>
        /// Метод обеспечивает проверку на возможность добавить объект в пул.
        /// </summary>
        /// <returns>Возвращает true, если пул имеет свободные места.</returns>
        public virtual bool CanPush()
        {
            return _pool.CanPush();
        }

        /// <summary>
        /// Метод добавляет объект в пул.
        /// </summary>
        /// <param name="groupKey">Группа пула.</param>
        /// <param name="objectPool">Объект пула.</param>
        /// <returns>Возвращает true в случае, если объект был добавлен.</returns>
        public virtual bool Push(string groupKey, ObjectPool objectPool)
        {
            return _pool.Push(groupKey, objectPool);
        }

        /// <summary>
        /// Метод возвращает объект из пула или создаёт новый, если в пуле объект отсутствовал.
        /// </summary>
        /// <typeparam name="T">Объект пула.</typeparam>
        /// <param name="prefab">Префаб объекта.</param>
        /// <returns>Возвращает объект пула.</returns>
        public virtual T PopOrCreate<T>(T prefab) where T : ObjectPool
        {
            return PopOrCreate<T>(prefab, Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// Метод возвращает объект из пула или создаёт новый, если в пуле объект отсутствовал.
        /// </summary>
        /// <typeparam name="T">Объект пула.</typeparam>
        /// <param name="prefab">Префаб объекта.</param>
        /// <param name="position">Позиция объекта.</param>
        /// <param name="rotation">Вращение объекта.</param>
        /// <returns>Возвращает объект пула с указанными параметрами.</returns>
        public virtual T PopOrCreate<T>(T prefab, Vector3 position, Quaternion rotation) where T : ObjectPool
        {
            T result = _pool.Pop<T>(prefab.Group);
            if (result == null)
                result = CreateObject<T>(prefab, position, rotation);
            else
            {
                result.transform.position = position;
                result.transform.rotation = rotation;
            }
            return result;
        }

        /// <summary>
        /// Возвращает объект из группы пула.
        /// </summary>
        /// <param name="groupKey">Группа пула</param>
        /// <returns>Возвращает объект пула.</returns>
        public virtual ObjectPool Pop(string groupKey)
        {
            return _pool.Pop<ObjectPool>(groupKey);
        }

        /// <summary>
        /// Возвращает объект из пула.
        /// </summary>
        /// <typeparam name="T">Объект пула.</typeparam>
        /// <returns>Возвращает объект пула.</returns>
        public virtual T Pop<T>() where T : ObjectPool
        {
            return _pool.Pop<T>();
        }

        /// <summary>
        /// Возвращает объект из пула, если условия метода сравнения выполняются.
        /// </summary>
        /// <typeparam name="T">Объект пула.</typeparam>
        /// <param name="comparer">Метод обеспечивающий выполнение сравнения.</param>
        /// <returns>Возвращает объект пула.</returns>
        public virtual T Pop<T>(Pool<string, ObjectPool>.Compare<T> comparer) where T : ObjectPool
        {
            return _pool.Pop<T>(comparer);
        }

        /// <summary>
        /// /// Возвращает объект из пула по названию группы.
        /// </summary>
        /// <typeparam name="T">Объект пула.</typeparam>
        /// <param name="groupKey">Название группы пула.</param>
        /// <returns>Возвращает объект из группы пула.</returns>
        public virtual T Pop<T>(string groupKey) where T : ObjectPool
        {
            return _pool.Pop<T>(groupKey);
        }

        /// <summary>
        /// Обеспечивает проку на наличие в пуля указанной группы.
        /// </summary>
        /// <param name="groupKey">Группа пула.</param>
        /// <returns>Возвращает true, если такая группа в пуле существует.</returns>
        public virtual bool Contains(string groupKey)
        {
            return _pool.Contains(groupKey);
        }

        /// <summary>
        /// Очищает пул.
        /// </summary>
        public virtual void Clear()
        {
            _pool.Clear();
        }

        /// <summary>
        /// Создаёт объект пула по указанным параметрам.
        /// </summary>
        /// <typeparam name="T">Объект группы пула.</typeparam>
        /// <param name="prefab">Префаб объекта.</param>
        /// <param name="position">Позиция.</param>
        /// <param name="rotation">Вращение.</param>
        /// <returns>Возвращает объект пула.</returns>
        protected virtual T CreateObject<T>(T prefab, Vector3 position, Quaternion rotation) where T : ObjectPool
        {
            GameObject gObj = Instantiate(prefab.gameObject, position, rotation);
            T result = gObj.GetComponent<T>();
            result.name = prefab.name;
            result.Create();
            return result;
        }


    }
}