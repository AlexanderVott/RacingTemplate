using UnityEngine;

namespace RedDev.Kernel.DB
{
	public interface IMetaXML
	{
		void Parse(string source);
	}
	public interface IMetaJSON { }

	public interface IMetaDB
	{
		int Id { get; }
		string Identifier { get; }
		void PreLoad();
	}

	public class BaseMetaDB : ScriptableObject, IMetaDB
	{
		[SerializeField]
		protected int _id;
		public int Id
		{
			get => _id;
#if UNITY_EDITOR
			set => _id = value;
#endif
		}

		[SerializeField]
		protected string _identifier;
		public string Identifier
		{
			get => _identifier;
#if UNITY_EDITOR
			set => _identifier = value;
#endif
		}

		public void PreLoad()
		{
			
		}
	}
}