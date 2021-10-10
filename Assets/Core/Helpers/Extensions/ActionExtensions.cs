using System;

namespace RedDev.Helpers.Extensions
{
	public static class ActionExtensions
	{
#region Calls
		/// <summary>
		/// Вызывает делегат, если он инициализирован.
		/// </summary>
		/// <param name="action">Делегат.</param>
		public static void Call(this Action action)
        {
            action?.Invoke();
        }

		/// <summary>
		/// Безопасный вызов делегата, если он инициализирован. В случае исключения, оно обрабатывается и выводится в лог.
		/// </summary>
		/// <param name="action">Делегат.</param>
		public static void SafeCall(this Action action)
		{
#if !UNITY_EDITOR
			try
            {
#endif
                action?.Invoke();
#if !UNITY_EDITOR
            }
			catch (Exception except)
			{
				LogException(except);
			}
#endif
		}
#endregion

#region Calls<T>
		/// <summary>
		/// Вызывает делегат, если он инициализирован.
		/// </summary>
		/// <param name="action">Делегат.</param>
		/// <param name="param">Параметр делегата.</param>
		public static void Call<T>(this Action<T> action, T param)
        {
            action?.Invoke(param);
        }

		/// <summary>
		/// Безопасный вызов делегата, если он инициализирован. В случае исключения, оно обрабатывается и выводится в лог.
		/// </summary>
		/// <param name="action">Делегат.</param>
		/// <param name="param">Параметр делегата.</param>
		public static void SafeCall<T>(this Action<T> action, T param)
		{
#if !UNITY_EDITOR
			try
            {
#endif
                action?.Invoke(param);
#if !UNITY_EDITOR
            }
			catch (Exception except)
			{
				LogException(except);
			}
#endif
		}
#endregion

#region Calls<T1, T2>
		/// <summary>
		/// Вызывает делегат, если он инициализирован.
		/// </summary>
		/// <param name="action">Делегат.</param>
		/// <param name="param1">Параметр делегата.</param>
		/// <param name="param2">Параметр делегата.</param>
		public static void Call<T1, T2>(this Action<T1, T2> action, T1 param1, T2 param2)
        {
            action?.Invoke(param1, param2);
        }

		/// <summary>
		/// Безопасный вызов делегата, если он инициализирован. В случае исключения, оно обрабатывается и выводится в лог.
		/// </summary>
		/// <param name="action">Делегат.</param>
		/// <param name="param1">Параметр делегата.</param>
		/// <param name="param2">Параметр делегата.</param>
		public static void SafeCall<T1, T2>(this Action<T1, T2> action, T1 param1, T2 param2)
		{
#if !UNITY_EDITOR
			try
            {
#endif
                action?.Invoke(param1, param2);
#if !UNITY_EDITOR
            }
			catch (Exception except)
			{
				LogException(except);
			}
#endif
		}
#endregion

#region Calls<T1, T2, T3>
		/// <summary>
		/// Вызывает делегат, если он инициализирован.
		/// </summary>
		/// <param name="action">Делегат.</param>
		/// <param name="param1">Параметр делегата.</param>
		/// <param name="param2">Параметр делегата.</param>
		/// <param name="param3">Параметр делегата.</param>
		public static void Call<T1, T2, T3>(this Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
        {
            action?.Invoke(param1, param2, param3);
        }

		/// <summary>
		/// Безопасный вызов делегата, если он инициализирован. В случае исключения, оно обрабатывается и выводится в лог.
		/// </summary>
		/// <param name="action">Делегат.</param>
		/// <param name="param1">Параметр делегата.</param>
		/// <param name="param2">Параметр делегата.</param>
		/// <param name="param3">Параметр делегата.</param>
		public static void SafeCall<T1, T2, T3>(this Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
		{
#if !UNITY_EDITOR
			try
            {
#endif
                action?.Invoke(param1, param2, param3);
#if !UNITY_EDITOR
            }
			catch (Exception except)
			{
				LogException(except);
			}
#endif
		}
#endregion

#region Calls<T1, T2, T3, T4>
		/// <summary>
		/// Вызывает делегат, если он инициализирован.
		/// </summary>
		/// <param name="action">Делегат.</param>
		/// <param name="param1">Параметр делегата.</param>
		/// <param name="param2">Параметр делегата.</param>
		/// <param name="param3">Параметр делегата.</param>
		/// <param name="param4">Параметр делегата.</param>
		public static void Call<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            action?.Invoke(param1, param2, param3, param4);
        }

		/// <summary>
		/// Безопасный вызов делегата, если он инициализирован. В случае исключения, оно обрабатывается и выводится в лог.
		/// </summary>
		/// <param name="action">Делегат.</param>
		/// <param name="param1">Параметр делегата.</param>
		/// <param name="param2">Параметр делегата.</param>
		/// <param name="param3">Параметр делегата.</param>
		/// <param name="param4">Параметр делегата.</param>
		public static void SafeCall<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
		{
#if !UNITY_EDITOR
			try
            {
#endif
                action?.Invoke(param1, param2, param3, param4);
#if !UNITY_EDITOR
            }
			catch (Exception except)
			{
				LogException(except);
			}
#endif
		}
#endregion
	}
}