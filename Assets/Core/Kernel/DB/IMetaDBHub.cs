using System;

namespace RedDev.Kernel.DB
{
	public interface IMetaDBHub
	{
		void Load(string path);
	}

	public interface IMetaDBHub<out T> : IMetaDBHub where T: class, IMetaDB
	{
		T[] metaData { get; }

		/// <summary>
		/// Возвращает данные по id.
		/// </summary>
		/// <param name="id">Идентификатор ресурса.</param>
		/// <returns>Возвращает экземпляр хранилища данных если такой найден. В противном случае null.</returns>
		T Get(int id);
		
		/// <summary>
		/// Возвращает данные по identifier, если такой поиск поддерживается.
		/// </summary>
		/// <param name="identifier">Текстовый идентификатор данных.</param>
		/// <returns>Возвращает экземпляр хранилища данных, если такой найден. В случае несовпадения или отсутствия поддержки поиска по данному полю возвращает null.</returns>
		T Get(string identifier);

		/// <summary>
		/// Возвращает массив данных используя предиктор для выбора нужного экземпляра.
		/// </summary>
		/// <param name="match">Предиктор для выборки.</param>
		/// <returns>Возвращает массив данных, состоящий из экземпляров данных удовлетворяющих условиям предиктора.</returns>
		T[] Get(Predicate<T> match);

		/// <summary>
		/// Возвращает весь массив данных.
		/// </summary>
		/// <returns></returns>
		T[] Get();

		T this[int index] { get; }
		T this[string identifier] { get; }
		T[] this[Predicate<T> match] { get; }
	}
}