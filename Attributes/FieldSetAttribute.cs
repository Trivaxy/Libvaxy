using System;

namespace Libvaxy.Attributes
{
	/// <summary>
	/// An attribute that lets you set non-public class fields without a runtime reflection penalty.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class FieldSetAttribute : Attribute
	{
		internal string fieldName;

		public FieldSetAttribute(string fieldName) => this.fieldName = fieldName;
	}
}
