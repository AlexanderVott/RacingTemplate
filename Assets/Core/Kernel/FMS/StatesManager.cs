using System;
using UnityEngine;

namespace RedDev.Kernel.FMS
{
	public class StatesManager
	{
		#region Fields and properties
		private StateMachine _stateMachine;

		public Transform container { get; private set; }
		public BaseState startingState { get; private set; }

		public BaseState currentState => _stateMachine.currentState as BaseState;
		#endregion

		public StatesManager(Transform containerParam)
		{
			if (containerParam == null)
			{
                Prod.LogError("[States] Container for statesManager is null");
				return;
			}
			container = containerParam;

			_stateMachine = new StateMachine(container);

			ChangeState(startingState.GetType());
		}

		#region Add
		public T Add<T>(Type type = null) where T : BaseState => _stateMachine.GetOrAdd<T>(type);

		public BaseState Add(BaseState state) => _stateMachine.GetOrAdd(state) as BaseState;
		#endregion

		#region Remove
		public bool Remove(BaseState state) => _stateMachine.Remove(state);
		public bool Remove<T>(Type type) where T : BaseState => _stateMachine.Remove<T>(type);
		#endregion

		#region Get and GetAll
		public T Get<T>() where T : BaseState => _stateMachine.Get<T>();

		public BaseState[] GetAll() => _stateMachine.GetAll() as BaseState[];
		#endregion

		#region ChangeState
		public void ChangeState(Type type, bool addToStack = true, Action<IState> callback = null) => _stateMachine.ChangeState(type, addToStack, callback);

		public void ChangeState<T>(bool addToStack = true, Action<T> callback = null) where T : BaseState => _stateMachine.ChangeState(addToStack, callback);
		#endregion
	}
}