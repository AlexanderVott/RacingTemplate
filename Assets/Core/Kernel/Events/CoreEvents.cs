using System;
using System.Collections.Generic;

namespace RedDev.Kernel.Events
{
	[Flags]
	public enum SubscribeType
	{
		Simple = 0,
		AutoCall = 1 << 0,
		OneCall = 1 << 1,

		SimpleAutoCall = Simple | AutoCall,
		AutoOneCall = OneCall | AutoCall,
	}
	
	public class CoreEvents : IDisposable
	{
		private Dictionary<int, IReceiverHub> _receiversHubs = new Dictionary<int, IReceiverHub>();

		#region Methods
		#region Receivers hubs
		public void AddReceiverType<T>(ReceiverHub<T> receiverHub) where T : BaseEventParam
		{
			_receiversHubs.Add(typeof(T).GetHashCode(), receiverHub);
		}

		public CoreEvents AddReceiverType<T>() where T : BaseEventParam
		{
			var receiverHub = Activator.CreateInstance<ReceiverHub<T>>();
			AddReceiverType(receiverHub);
			return this;
		}

		public void RemoveReceiverType<T>() where T : BaseEventParam
		{
			var hash = typeof(T).GetHashCode();
			if (_receiversHubs.ContainsKey(hash))
			{
				_receiversHubs.Clear();
				_receiversHubs.Remove(hash);
			}
		}
		#endregion

		#region Subscribe
		public void Subscribe(IReceiverEvent receiver, Type type, SubscribeType subscribeType)
		{
			var hash = type.GetHashCode();
			if (!_receiversHubs.ContainsKey(hash))
				return;
			var receiverHub = _receiversHubs[hash];
			receiverHub.Subscribe(receiver, type, subscribeType);
		}

		public void Subscribe(object obj, SubscribeType subscribeType)
		{
			var interfaces = obj.GetType().GetInterfaces();
			var receiver = obj as IReceiverEvent;
			foreach (var intType in interfaces)
				if (intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IReceiverEvent<>))
					Subscribe(receiver, intType.GetGenericArguments()[0], subscribeType);
		}
		#endregion

		#region Unsubscribe
		public void Unsubscribe(IReceiverEvent receiver, Type type)
		{
			var hash = type.GetHashCode();
			if (!_receiversHubs.ContainsKey(hash))
				return;
			var receiverHub = _receiversHubs[hash];
			receiverHub.Unsubscribe(receiver);
		}

		public void Unsubscribe(object obj)
		{
			var interfaces = obj.GetType().GetInterfaces();
			var receiver = obj as IReceiverEvent;
			foreach (var intType in interfaces)
				if (intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IReceiverEvent<>))
					Unsubscribe(receiver, intType.GetGenericArguments()[0]);
		}
		#endregion
		
		#region Clear
		public void Clear()
		{
			foreach (var hub in _receiversHubs.Keys)
				_receiversHubs[hub].Clear();
		}

		public void Clear<T>() where T: BaseEventParam
		{
			var hash = typeof(T).GetHashCode();
			if (_receiversHubs.ContainsKey(hash))
				_receiversHubs[hash].Clear();
		}
		#endregion

		#region AutoCallArgs
		public void AddAutoCallArg<T>(T arg) where T: BaseEventParam
		{
			var hash = arg.GetType().GetHashCode();
			if (_receiversHubs.ContainsKey(hash))
				(_receiversHubs[hash] as ReceiverHub<T>)?.AddAutoCallArg(arg);
		}

		public void RemoveAutoCallArg<T>(T arg) where T : BaseEventParam
		{
			var hash = arg.GetType().GetHashCode();
			if (_receiversHubs.ContainsKey(hash))
				(_receiversHubs[hash] as ReceiverHub<T>)?.RemoveAutoCallArg(arg);
		}

		public void ClearAutoCallsArgs<T>() where T : BaseEventParam
		{
			var hash = typeof(T).GetHashCode();
			if (_receiversHubs.ContainsKey(hash))
				(_receiversHubs[hash] as ReceiverHub<T>)?.ClearAutoCallArgs();
		}
		#endregion

		#region Calls
		private void Call<T>(int hash, T arg) where T: BaseEventParam
		{
			if (_receiversHubs.ContainsKey(hash))
				(_receiversHubs[hash] as ReceiverHub<T>)?.Call(arg);
		}

		public void Call<T>(T arg) where T: BaseEventParam
		{
			Call(arg.GetType().GetHashCode(), arg);
		}
		#endregion
		#endregion

		#region Implementation
		public void Dispose() 
            => Clear();
        #endregion
	}
}