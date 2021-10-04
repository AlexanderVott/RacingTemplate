namespace RedDev.Kernel.DB
{
	public interface IBasePrefsModel
	{
		void Load();
		void PostLoad();
		void Save();

		void Reset();
	}
}