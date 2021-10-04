using System;
using System.Collections.Generic;

namespace RedDev.Kernel.Events
{
	public interface IReceiverHub
	{
		void Subscribe(IReceiverEvent receiver, Type type, SubscribeType subscribeType);
		void Unsubscribe(IReceiverEvent receiver);
		void Clear();
	}

	public class ReceiverHub<T> : IReceiverHub where T : BaseEventParam
	{
		private List<IReceiverEvent> _receivers = new List<IReceiverEvent>();
		private List<IReceiverEvent> _receiversOneCall = new List<IReceiverEvent>();
		private List<T> _autoCallArguments = new List<T>();

		#region AutoCallArguments
		public void AddAutoCallArg(T arg) 
            => _autoCallArguments.Add(arg);

		public void RemoveAutoCallArg(T arg) 
            => _autoCallArguments.Remove(arg);

		public void ClearAutoCallArgs() 
            => _autoCallArguments.Clear();
		#endregion

		public void Subscribe(IReceiverEvent receiver, Type type, SubscribeType subscribeType)
		{
			if ((subscribeType & SubscribeType.OneCall) != 0)
				_receiversOneCall.Add(receiver);
			else
				_receivers.Add(receiver);

			if (((subscribeType & SubscribeType.AutoCall) != 0) && (_autoCallArguments.Count > 0))
				foreach (var arg in _autoCallArguments)
					Call(arg);
		}

		public void Unsubscribe(IReceiverEvent receiver)
		{
			_receivers.Remove(receiver);
			_receiversOneCall.Remove(receiver);
		}

		public void Clear()
		{
			_receivers.Clear();
			_receiversOneCall.Clear();
			_autoCallArguments.Clear();
		}

		public void Call(T arg)
		{
			foreach (var item in _receivers)
			{
				var receiver = item as IReceiverEvent<T>;
				receiver?.HandleEvent(arg);
			}
			foreach (var item in _receiversOneCall)
			{
				var receiver = item as IReceiverEvent<T>;
				receiver?.HandleEvent(arg);
			}
			_receiversOneCall.Clear();
		}
	}
}