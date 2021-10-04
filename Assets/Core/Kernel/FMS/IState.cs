using System;

namespace RedDev.Kernel.FMS
{
	/// <summary>
	/// Описывает текущий режим работы состояния.
	/// </summary>
	public enum EStateMode
	{
		idle = 0,
		entering = 1,
		running = 2,
		pause,
		stopped
	}

	/// <summary>
	/// Интерфейс описывает состояние FMS.
	/// </summary>
	public interface IState
	{
		/// <summary>
		/// Текущий режим работы состояния.
		/// </summary>
		EStateMode mode { get; }

		/// <summary>
		/// Выполняется при инициализации объекта.
		/// </summary>
		void OnInitialize();

		void OnPreEnter(Action callback = null);
		
		/// <summary>
		/// Выполняется при первичном входе в состояние.
		/// </summary>
		void OnEnter();

		/// <summary>
		/// Выполняется при выходе из состояния.
		/// </summary>
		void OnExit();

		/// <summary>
		/// Выполняется при остановке состояния.
		/// </summary>
		void OnPause();

		/// <summary>
		/// Выполняется при возвращении состояния в активное положение.
		/// </summary>
		void OnResume();
	}
}