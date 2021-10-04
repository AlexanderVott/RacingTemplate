using System;
using System.Collections.Generic;
using RedDev.Helpers.Extensions;
using UnityEngine;

namespace RedDev.Kernel.FMS
{
	/// <summary>
	/// Описывает функционал машины состояний.
	/// </summary>
	public class StateMachine
	{
		public IState currentState { get; private set; }

		private List<IState> _statesStack = new List<IState>();
		public int stackSize => _statesStack.Count;

		private Dictionary<int, IState> _states = new Dictionary<int, IState>();
		private Dictionary<int, GameObject> _statesObjects = new Dictionary<int, GameObject>();

		private Transform _root;

		public bool switchInProgress { get; private set; } = false;

		public StateMachine()
		{
		}

		public StateMachine(Transform root)
		{
			_root = root;
			var statesInContainer = root.GetComponentsInChildren<Transform>(true);
			foreach (var transformState in statesInContainer)
			{
				var stateInContainer = transformState.GetComponent<IState>();
				if (stateInContainer == null)
					continue;
				var type = stateInContainer.GetType();
				int hash = type.GetHashCode();
				if (_states.ContainsKey(hash))
				{
#if UNITY_EDITOR
					Debug.LogError($"[{GetType().Name}] Dublicates states container with Type: {type.Name}");
#endif
				}
				_states.Add(hash, stateInContainer);
				_statesObjects.Add(hash, transformState.gameObject);
				transformState.gameObject.SetActive(false);
				stateInContainer.OnInitialize();
			}
		}

		#region Stack methods
		public void ClearStack() 
            => _statesStack.Clear();

        private void Push(IState state)
		{
			RemoveLoopIfFound(state);

			_statesStack.Add(state);
		}

		private IState Peek() 
            => _statesStack.Count > 0 ? _statesStack[_statesStack.Count - 1] : null;

        private void RemoveLast()
		{
			if (_statesStack.Count == 0)
				return;

			_statesStack.RemoveAt(_statesStack.Count - 1);
		}

		private void RemoveLoopIfFound(IState state)
		{
			if (!_statesStack.Contains(state))
				return;

			var index = _statesStack.IndexOf(state);
			while (index < _statesStack.Count)
				_statesStack.RemoveAt(index);
		}
		#endregion

		#region Add
		public T GetOrAdd<T>(Type type = null) where T : class, IState
		{
			var innerType = type == null ? typeof(T) : type;
			var hash = innerType.GetHashCode();
			if (_states.TryGetValue(hash, out var state))
			{
				return state as T;
			}
			else
			{
				var result = default(T);
				if (_root != null)
				{
					var gobj = new GameObject(innerType.Name, innerType);
					gobj.transform.parent = _root;
					result = gobj.GetComponent<T>();
					_statesObjects.Add(hash, gobj);
					_states.Add(hash, result);
				}
				else
					result = Activator.CreateInstance<T>();

				result.OnInitialize();

				return result;
			}
		}

		public IState GetOrAdd(IState state)
		{
			var innerType = state.GetType();
			var hash = innerType.GetHashCode();

			if (_states.TryGetValue(hash, out var result))
				return result;

			_states.Add(hash, state);
			return state;
		}
		#endregion

		#region Remove
		public bool Remove<T>(Type type = null) where T : class, IState
		{
			var innerType = type == null ? typeof(T) : type;
			var hash = innerType.GetHashCode();
			return _states.Remove(hash);
		}

		public bool Remove(IState state) => _states.Remove(state.GetType().GetHashCode());
		#endregion

		#region Get and GetAll
		public T Get<T>() 
            => (T)_states[typeof(T).GetHashCode()];

		public IState[] GetAll()
		{
			var statesTmp = _states.Values;
			IState[] result = new IState[_states.Count];
			int counter = 0;
			foreach (var state in statesTmp)
				result[counter++] = state;

			return result;
		}
		#endregion

		#region ChangeState
		public void ChangeState(Type type, bool addToStack = true, Action<IState> callback = null)
		{
			//var curState = currentState;
			var state = GetOrAdd<IState>(type);
			if (state != null)
				SwitchInternal(state, addToStack, callback);
			else
                Prod.LogError($"[System] Global states not found state {type} for change state");
		}

		public void ChangeState<T>(bool addToStack = true, Action<T> callback = null) where T : class, IState => ChangeState(typeof(T), addToStack, (x)=> callback.SafeCall(x as T));

		public void SwitchToPreviousState(Action callback = null)
		{
			if (stackSize > 1 && !switchInProgress) 
				SwitchToPreviousState((x) => callback.SafeCall());
		}

		#endregion

		#region Internal switches
		private void SwitchToPreviousState(Action<IState> callback)
		{
			var lastState = Peek();
			if (currentState == lastState)
			{
				RemoveLast();
				lastState = Peek();
			}
			ChangeStateBegin(lastState, callback);
		}

		private void SwitchInternal(IState newState, bool addToStack = true, Action<IState> callback = null)
		{
			if (currentState != newState && newState != null)
			{
				if (addToStack)
					Push(newState);
				ChangeStateBegin(newState, callback);
			}
		}

		private void ChangeStateBegin(IState newState, Action<IState> callback)
		{
			switchInProgress = true;
			currentState?.OnPause();
			newState.OnPreEnter(() => SwitchStateEnd(newState, callback));
		}

		private void SwitchStateEnd(IState newState, Action<IState> callback)
		{
			switchInProgress = false;
			var oldState = currentState;
			currentState = newState;

			callback.SafeCall(newState);
			currentState.OnEnter();

			Type innerType;
			int hash;
			if (oldState != null)
			{
				innerType = oldState.GetType();
				hash = innerType.GetHashCode();
				_statesObjects[hash].gameObject.SetActive(false);
			}

			if (currentState != null)
			{
				innerType = currentState.GetType();
				hash = innerType.GetHashCode();
				_statesObjects[hash].gameObject.SetActive(true);
			}
		}
		#endregion
	}
}