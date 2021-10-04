namespace RedDev.Helpers.Collections
{
	public class DoublyNode<T>
	{
		public T data { get; set; }

		public DoublyNode<T> prev { get; set; }
		public DoublyNode<T> next { get; set; }

		public DoublyNode(T data)
		{
			this.data = data;
		}
	}
}