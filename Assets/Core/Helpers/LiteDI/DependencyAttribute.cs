using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class DependencyAttribute : Attribute {

	public string name { get; private set; }

	public DependencyAttribute() { }

	public DependencyAttribute(string name) 
        => this.name = name;
}
