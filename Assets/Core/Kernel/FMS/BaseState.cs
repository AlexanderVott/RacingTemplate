using System;
using RedDev.Helpers.Extensions;
using UnityEngine;

namespace RedDev.Kernel.FMS
{
	public interface IDataState { }

	public class NullData : IDataState { }

	public class BaseState : MonoBehaviour, IState
	{
		protected static NullData @nullData = new NullData();

		public IDataState rawData { get; private set; }
		private bool _needUpdateData = false;
		protected bool isDataApplied => rawData != null && !_needUpdateData;

		public EStateMode mode { get; private set; }
		
		#region Base event methods state
		public virtual void OnInitialize()
		{
			if (rawData == null) 
				SetDefaultData();
			if (rawData != null)
			{
				if (_needUpdateData)
				{
					ConstructData();
					_needUpdateData = false;
				}
			}

			mode = EStateMode.idle;
		}

		public void OnPreEnter(Action callback = null)
		{
			mode = EStateMode.entering;
			try
			{
				callback.SafeCall();
			}
			catch (Exception e)
			{
                Prod.LogError($"[States] Pre-enter callback fail: {e.Message}");
			}
		}

		public virtual void OnEnter()
		{
			mode = EStateMode.running;
		}

		public virtual void OnExit()
		{
			mode = EStateMode.stopped;
		}

		public virtual void OnPause()
		{
			mode = EStateMode.pause;
		}

		public virtual void OnResume()
		{
			mode = EStateMode.running;
		}
		#endregion

		#region Data methods
		public void SetData(IDataState data)
		{
			if (isDataApplied)
			{
				DestroyData();
			}

			rawData = data;
			_needUpdateData = data != null;
		}

		protected virtual void SetDefaultData() { }

		protected virtual void ConstructData()
		{
			if (isDataApplied)
				return;
		}

		protected virtual void DestroyData()
		{
			rawData = null;
		}
		#endregion
	}
}