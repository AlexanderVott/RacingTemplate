using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class IgnorePreinitializeContext : Attribute
{
	public IgnorePreinitializeContext()
	{
	}
}
