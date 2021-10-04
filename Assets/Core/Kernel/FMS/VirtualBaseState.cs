using System;

namespace RedDev.Kernel.FMS
{
	/// <summary>
	/// Описывает базовый функционал состояния.
	/// </summary>
	public class VirtualBaseState : IState
	{
		public EStateMode mode { get; private set; }

		public VirtualBaseState()
		{
			mode = EStateMode.idle;
		}
		
		public void OnInitialize() { }

		public void OnPreEnter(Action callback = null) 
            => mode = EStateMode.entering;

        public void OnEnter() 
            => mode = EStateMode.running;

        public void OnExit() 
            => mode = EStateMode.stopped;

        public void OnPause() 
            => mode = EStateMode.pause;

        public void OnResume() 
            => mode = EStateMode.running;
    }
}