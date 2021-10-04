using RedDev.Kernel.Bindings.Components.Base;
using UnityEngine;

namespace RedDev.Kernel.Contexts
{
	public abstract class BaseContext : MonoBehaviour, IBndTarget
	{
		public string GetName() => gameObject.name;
    }
}