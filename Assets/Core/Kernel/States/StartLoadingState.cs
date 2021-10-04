using RedDev.Kernel.Contexts;
using RedDev.Kernel.FMS.Globals;
using RedDev.Kernel.Managers;
using UnityEngine;

namespace RedDev.Kernel.States
{
	public class StartLoadingState : GlobalBaseState
	{
		[SerializeField]
		private GlobalBaseState _nextState = null;

		private KernelStartEvent _kernelStartEvent = new KernelStartEvent();

		public override void OnInitialize()
		{
			base.OnInitialize();
			contextManager?.Preload<UIStartLoadingContext>();
		}

		public override void OnEnter()
		{
			base.OnEnter();
			contextManager?.Push<UIStartLoadingContext>();
		}

		public override void OnExit()
		{
			contextManager?.Pop<UIStartLoadingContext>();
			base.OnExit();
		}

		public override void OnPause()
		{
			contextManager?.Pop<UIStartLoadingContext>();
			base.OnPause();
		}

		public override void OnResume()
		{
			base.OnResume();
			contextManager?.Push<UIStartLoadingContext>();
		}

		public void NextState()
		{
			var events = Core.Get<SystemEventsManager>();
			events.AddAutoCallArg(_kernelStartEvent);
			events.KernelStart();
			if (_nextState != null)
				globalStates?.ChangeState(_nextState.GetType());
			else
                Prod.LogError("[States] Next state after loading is null!");
		}
	}
}