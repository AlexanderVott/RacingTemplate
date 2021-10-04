using System;
using UnityEngine;

namespace RedDev.Helpers.Extensions
{
	public static class ActionExtensions
	{
		/// <summary>
		/// Внутренний метод вывода исключения вызова делегата в лог.
		/// </summary>
		/// <param name="except">Объект исключения.</param>
		private static void LogException(Exception except)
		{
			Prod.LogError($"[System] {except.Message}");
		}

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
			try
            {
                action?.Invoke();
            }
			catch (Exception except)
			{
				LogException(except);
			}
		}

		/// <summary>
		/// Безопасный вызов делегата с возвратом информации об успешности его выполнения. В случае исключения, оно обрабатывается и выводится в лог.
		/// </summary>
		/// <param name="action">Делегат.</param>
		/// <returns>Возвращает True, если выполнение делегата прошло успешно. В противном случае исключение выводится в лог и возвращается False.</returns>
		public static bool TrySafeCall(this Action action)
		{
			try
			{
				if (action != null)
				{
					action();
					return true;
				}
			}
			catch (Exception except)
			{
				LogException(except);
			}
			return false;
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
			try
            {
                action?.Invoke(param);
            }
			catch (Exception except)
			{
				LogException(except);
			}
		}

		/// <summary>
		/// Безопасный вызов делегата с возвратом информации об успешности его выполнения. В случае исключения, оно обрабатывается и выводится в лог.
		/// </summary>
		/// <param name="action">Делегат.</param>
		/// <param name="param">Параметр делегата.</param>
		/// <returns>Возвращает True, если выполнение делегата прошло успешно. В противном случае исключение выводится в лог и возвращается False.</returns>
		public static bool TrySafeCall<T>(this Action<T> action, T param)
		{
			try
			{
				if (action != null)
				{
					action(param);
					return true;
				}
			}
			catch (Exception except)
			{
				LogException(except);
			}
			return false;
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
			try
            {
                action?.Invoke(param1, param2);
            }
			catch (Exception except)
			{
				LogException(except);
			}
		}

		/// <summary>
		/// Безопасный вызов делегата с возвратом информации об успешности его выполнения. В случае исключения, оно обрабатывается и выводится в лог.
		/// </summary>
		/// <param name="action">Делегат.</param>
		/// <param name="param1">Параметр делегата.</param>
		/// <param name="param2">Параметр делегата.</param>
		/// <returns>Возвращает True, если выполнение делегата прошло успешно. В противном случае исключение выводится в лог и возвращается False.</returns>
		public static bool TrySafeCall<T1, T2>(this Action<T1, T2> action, T1 param1, T2 param2)
		{
			try
			{
				if (action != null)
				{
					action(param1, param2);
					return true;
				}
			}
			catch (Exception except)
			{
				LogException(except);
			}
			return false;
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
			try
            {
                action?.Invoke(param1, param2, param3);
            }
			catch (Exception except)
			{
				LogException(except);
			}
		}

		/// <summary>
		/// Безопасный вызов делегата с возвратом информации об успешности его выполнения. В случае исключения, оно обрабатывается и выводится в лог.
		/// </summary>
		/// <param name="action">Делегат.</param>
		/// <param name="param1">Параметр делегата.</param>
		/// <param name="param2">Параметр делегата.</param>
		/// <param name="param3">Параметр делегата.</param>
		/// <returns>Возвращает True, если выполнение делегата прошло успешно. В противном случае исключение выводится в лог и возвращается False.</returns>
		public static bool TrySafeCall<T1, T2, T3>(this Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
		{
			try
			{
				if (action != null)
				{
					action(param1, param2, param3);
					return true;
				}
			}
			catch (Exception except)
			{
				LogException(except);
			}
			return false;
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
			try
            {
                action?.Invoke(param1, param2, param3, param4);
            }
			catch (Exception except)
			{
				LogException(except);
			}
		}

		/// <summary>
		/// Безопасный вызов делегата с возвратом информации об успешности его выполнения. В случае исключения, оно обрабатывается и выводится в лог.
		/// </summary>
		/// <param name="action">Делегат.</param>
		/// <param name="param1">Параметр делегата.</param>
		/// <param name="param2">Параметр делегата.</param>
		/// <param name="param3">Параметр делегата.</param>
		/// <param name="param4">Параметр делегата.</param>
		/// <returns>Возвращает True, если выполнение делегата прошло успешно. В противном случае исключение выводится в лог и возвращается False.</returns>
		public static bool TrySafeCall<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			try
			{
				if (action != null)
				{
					action(param1, param2, param3, param4);
					return true;
				}
			}
			catch (Exception except)
			{
				LogException(except);
			}
			return false;
		}
		#endregion
	}
}