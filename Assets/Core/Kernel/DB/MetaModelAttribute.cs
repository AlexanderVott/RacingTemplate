using System;

namespace RedDev.Kernel.DB
{
	[AttributeUsage(AttributeTargets.Class)]
	public class MetaModelAttribute : Attribute
	{
		public string path { get; private set; }

		public MetaModelAttribute(string path)
		{
			this.path = path;
		}
	}
}