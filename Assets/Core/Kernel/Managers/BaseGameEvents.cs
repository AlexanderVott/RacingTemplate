using System;
using RedDev.Kernel.Events;

namespace RedDev.Kernel.Managers
{
	/// <summary>
	/// Менеджер событий.
	/// </summary>
	public class BaseGameEvents : BaseManager
	{
		public CoreEvents events { get; } = new CoreEvents();
		
		#region Receivers hubs
		/// <summary>
		/// Добавляет экземпляр хаба прослушивателей по типу события.
		/// </summary>
		/// <typeparam name="T">Тип события.</typeparam>
		/// <param name="receiverHub">Экземпляр хаба события.</param>
		/// <returns>Возвращает менеджер событий.</returns>
		public BaseGameEvents AddReceiverType<T>(ReceiverHub<T> receiverHub) where T : BaseEventParam
		{
			events.AddReceiverType(receiverHub);
			return this;
		}

		/// <summary>
		/// Добавляет хаб прослушивателей по типу события.
		/// </summary>
		/// <typeparam name="T">Тип события.</typeparam>
		/// <returns>Возвращает менеджер событий.</returns>
		public BaseGameEvents AddReceiverType<T>() where T : BaseEventParam
		{
			events.AddReceiverType<T>();
			return this;
		}

		/// <summary>
		/// Удаляет хаб прослушивателей по типу события.
		/// </summary>
		/// <typeparam name="T">Тип события.</typeparam>
		/// <returns>Возвращает менеджер событий.</returns>
		public BaseGameEvents RemoveReceiverType<T>() where T : BaseEventParam
		{
			events.RemoveReceiverType<T>();
			return this;
		}
		#endregion

		#region Events
		/// <summary>
		/// Подписка на событие.
		/// </summary>
		/// <typeparam name="T">Тип прослушиваемого события.</typeparam>
		/// <param name="receiver"></param>
		/// <param name="type"></param>
		/// <param name="subscribeType"></param>
		/// <returns></returns>
		public BaseGameEvents Subscribe<T>(IReceiverEvent<T> receiver, Type type, SubscribeType subscribeType)
		{
			events.Subscribe(receiver as IReceiverEvent<BaseEventParam>, type, subscribeType);
			return this;
		}

		public BaseGameEvents Subscribe(object obj, SubscribeType subscribeType)
		{
			events.Subscribe(obj, subscribeType);
			return this;
		}

		public BaseGameEvents Unsubscribe(IReceiverEvent receiver, Type type)
		{
			events.Unsubscribe(receiver, type);
			return this;
		}

		public BaseGameEvents Unsubscribe(object obj)
		{
			events.Unsubscribe(obj);
			return this;
		}

		public void Clear<T>() where T: BaseEventParam 
            => events.Clear<T>();

        public void Clear() 
            => events.Clear();

        public BaseGameEvents Call<T>(T arg) where T: BaseEventParam
		{
			events.Call(arg);
			return this;
		}
		#endregion

		#region AutoCallArgs
		public BaseGameEvents AddAutoCallArg<T>(T arg) where T: BaseEventParam
		{
			events.AddAutoCallArg(arg);
			return this;
		}

		public BaseGameEvents RemoveAutoCallArg<T>(T arg) where T : BaseEventParam
		{
			events.RemoveAutoCallArg(arg);
			return this;
		}

		public void ClearAutoCallsArgs<T>() where T : BaseEventParam 
            => events.ClearAutoCallsArgs<T>();

        #endregion
	}
}