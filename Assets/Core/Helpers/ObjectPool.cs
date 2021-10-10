using RedDev.ManagersEngine;
using UnityEngine;

namespace RedDev.Helpers
{
    /// <summary>
    /// Класс реализует пул объектов в рамках Unity.
    /// </summary>
    public class ObjectPool : MonoBehaviour, IPoolObject<string>
    {
        /// <summary>
        /// Группа пула.
        /// </summary>
        public virtual string Group { get { return name; } }
        
        /// <summary>
        /// Конструктор объекта пула.
        /// </summary>
        public virtual void Create()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Деструктор объекта пула.
        /// </summary>
        public void OnPush()
        {
            gameObject.SetActive(false);
        }

        public virtual void Push()
        {
            PoolManager.Instance.Push(Group, this);
        }

        /// <summary>
        /// Метод реализует событие, вызываемое при отсутствии свободного места в пуле.
        /// </summary>
        public void OnFailedPush()
        {
            Debug.Log("Failed push to pool for: " + gameObject.name);
            Destroy(gameObject);
        }
    }
}