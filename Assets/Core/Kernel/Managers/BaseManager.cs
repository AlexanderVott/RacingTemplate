using UnityEngine;

namespace RedDev.Kernel.Managers
{
	public abstract class BaseManager : MonoBehaviour
	{
		public bool Initialized { get; protected set; } = false;

		public virtual void Attach() 
            => Initialized = true;
    }
}