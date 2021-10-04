namespace RedDev.Kernel.Events
{
	public interface IReceiverEvent { }

	public interface IReceiverEvent<in T> : IReceiverEvent
	{
		void HandleEvent(T arg);
	}
}