using System.Collections;
using UnityEngine;

/// <summary>
/// Класс упрощающий возврат результата работы курутины.
/// </summary>
/// <typeparam name="T">Тип возвращаемого объекта.</typeparam>
public class Coroutine<T>
{
	private MonoBehaviour m_owner;
	private IEnumerator m_target;

	/// <summary>
	/// Возвращаемый результат.
	/// </summary>
	public T result { get; private set; }

	/// <summary>
	/// Выполняемая курутина.
	/// </summary>
	public Coroutine coroutine { get; private set; }

	/// <summary>
	/// Конструктор.
	/// </summary>
	/// <param name="owner">Объект, вызывающий курутину.</param>
	/// <param name="target">Ключевая курутина, результат которой необходимо вернуть.</param>
	public Coroutine(MonoBehaviour owner, IEnumerator target)
	{
		m_owner = owner;
		m_target = target;

		coroutine = m_owner.StartCoroutine(Run());
	}

	private IEnumerator Run()
	{
		while (m_target.MoveNext())
		{
			if (m_target.Current is WWW)
			{
				continue;
			}
			result = (T) m_target.Current;
			yield return result;
		}
	}
}
