using RedDev.Kernel.Contexts;
using RedDev.Kernel.Managers;
using UnityEngine;

namespace RedDev.Kernel.FMS.Globals
{
	public class GlobalBaseState : BaseState
	{
		private ContextManager _contextManager;
		protected ContextManager contextManager
		{
			get
			{
				if (_contextManager == null)
				{
					_contextManager = Managers.Core.Get<ContextManager>();
					if (_contextManager == null)
                        Prod.LogError($"[States] Not found context manager for {name}");
				}
				return _contextManager;
			}
		}


		private GlobalStatesManager _globalStates;
		public GlobalStatesManager globalStates
		{
			get
			{
				if (_globalStates == null)
				{
					_globalStates = Managers.Core.Get<GlobalStatesManager>();
					if (_globalStates == null)
                        Prod.LogError($"[States] Not found global states manager for {name}");
				}
				return _globalStates;
			}
		}

		public virtual void ShowContext<T>() where T: BaseContext { }

		public virtual void HideContext(BaseContext context) { }
	}
}