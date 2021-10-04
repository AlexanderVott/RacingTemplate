using System;
using RedDev.Kernel.FMS;
using RedDev.Kernel.FMS.Globals;
using RedDev.Kernel.States;
using UnityEngine;

namespace RedDev.Kernel.Managers
{
	public class GlobalStatesManager : BaseManager
	{
		#region Fields and properties
		private Transform _root;

		private StateMachine _stateMachine;

		public GlobalBaseState currentState => _stateMachine.currentState as GlobalBaseState;

		public GlobalBaseState startingState;
		#endregion

		void Start()
		{
			InitializeStates();
			if (startingState == null)
				ChangeState<StartLoadingState>();
			else
			{
				startingState.gameObject.SetActive(true);
				ChangeState(startingState.GetType());
			}
		}

		private void InitializeStates()
		{
			_root = transform.Find("Root") ?? (new GameObject("Root")).transform;
			_root.parent = transform;
			_stateMachine = new StateMachine(_root);
		}
        
		#region Add
		public T GetOrAdd<T>(Type type = null) where T: GlobalBaseState => _stateMachine.GetOrAdd<T>(type);

		public GlobalBaseState GetOrAdd(GlobalBaseState state) => _stateMachine.GetOrAdd(state) as GlobalBaseState;
		#endregion

		#region Remove
		public bool Remove<T>(Type type = null) where T : GlobalBaseState => _stateMachine.Remove<T>(type);

		public bool Remove(GlobalBaseState state) => _stateMachine.Remove(state);
		#endregion

		#region Get and GetAll
		public T Get<T>() => _stateMachine.Get<T>();

		public GlobalBaseState[] GetAll() => _stateMachine.GetAll() as GlobalBaseState[];
		#endregion

		#region ChangeState
		public void ChangeState(Type type, Action<IState> callback = null) => _stateMachine.ChangeState(type, true, callback);
        
		public void ChangeState<T>(Action<T> callback = null) where T : GlobalBaseState => _stateMachine.ChangeState(true, callback);
        #endregion
	}
}