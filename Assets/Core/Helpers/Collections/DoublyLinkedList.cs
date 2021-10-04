using System.Collections;
using System.Collections.Generic;

namespace RedDev.Helpers.Collections
{
	public class DoublyLinkedList<T> : IEnumerable<T>
	{
		private DoublyNode<T> head;
		private DoublyNode<T> tail;

		public int count { get; private set; }
		public bool isEmpty => count == 0;

		#region Managment methods
		/// <summary>
		/// Добавляет новый элемент по порядку.
		/// </summary>
		/// <param name="data"></param>
		public void Add(T data)
		{
			var node = new DoublyNode<T>(data);

			if (head == null)
				head = node;
			else
			{
				tail.next = node;
				node.prev = tail;
			}
			tail = node;
			count++;
		}

		/// <summary>
		/// Добавляет новый элемент в начало списка.
		/// </summary>
		/// <param name="data"></param>
		public void AddFirst(T data)
		{
			var node = new DoublyNode<T>(data);
			var tmp = head;
			node.next = tmp;
			head = node;
			if (count == 0)
				tail = head;
			else
				tmp.prev = node;
			count++;
		}

		/// <summary>
		/// Вставляет ноду до указанного элемента.
		/// </summary>
		/// <param name="targetData">Информация целевого элемента, до которого надо ставить новый элемент.</param>
		/// <param name="data">Информация нового элемента.</param>
		/// <returns>Возвращает true в случае успешного добавления, false в случае, если целевой элемент не удалось найти.</returns>
		public bool InsertAfter(T targetData, T data)
		{
			var targetNode = GetNode(targetData);
			if (targetNode == null)
				return false;
			else
			{
				var node = new DoublyNode<T>(data);
				var nextNode = targetNode.next;
				targetNode.next = node;
				nextNode.prev = node;
				node.next = nextNode;
				node.prev = targetNode;
				count++;
			}
			return true;
		}

		/// <summary>
		/// Вставляет ноду после указанного элемента.
		/// </summary>
		/// <param name="targetData">Информация целевого элемента, после которого надо ставить новый элемент.</param>
		/// <param name="data">Информация нового элемента.</param>
		/// <returns>Возвращает true в случае успешного добавления, false в случае, если целевой элемент не удалось найти.</returns>
		public bool InsertBefore(T targetData, T data)
		{
			var targetNode = GetNode(targetData);
			if (targetNode == null)
				return false;
			else
			{
				var node = new DoublyNode<T>(data);
				var prevNode = targetNode.prev;
				targetNode.prev = node;
				prevNode.next = node;
				node.next = targetNode;
				node.prev = prevNode;
				count++;
			}
			return true;
		}

		/// <summary>
		/// Возвращает элемент соответствующий содержимому.
		/// </summary>
		/// <param name="data"></param>
		/// <returns>Возвращает null если ничего не найдено, или найденный элемент в противном случае.</returns>
		public DoublyNode<T> GetNode(T data)
		{
			var current = head;
			while (current != null)
			{
				if (current.data.Equals(data))
					break;
				current = current.next;
			}

			return current;
		}

		/// <summary>
		/// Удаляет ноду по содержимому.
		/// </summary>
		/// <param name="data"></param>
		/// <returns>Возвращает true если удалось найти и удалить элемент, в противном случае false.</returns>
		public bool Remove(T data)
		{
			var targetNode = GetNode(data);

			if (targetNode != null)
			{
				// если узел не последний
				if (targetNode.next != null)
				{
					targetNode.next.prev = targetNode.prev;
				}
				else
				{
					// если последний изменяем хвостовой элемент
					tail = targetNode.prev;
				}

				// если узел не первый
				if (targetNode.prev != null)
					targetNode.prev.next = targetNode.next;
				else
				{
					// если первый изменяем головной элемент
					head = targetNode.next;
				}
				count--;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Сбрасывает ссылки на головной и конечный элементы, сбрасывает счётчик количества.
		/// </summary>
		public void Clear()
		{
			head = null;
			tail = null;
			count = 0;
		}

		public bool Contains(T data)
		{
			return GetNode(data) != null;
		}
		#endregion

		#region Enumerators
		public IEnumerable<T> GetBackEnumerator()
		{
			var current = tail;
			while (current != null)
			{
				yield return current.data;
				current = current.prev;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			var current = head;
			while (current != null)
			{
				yield return current.data;
				current = current.next;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this).GetEnumerator();
		}
		#endregion
	}
}