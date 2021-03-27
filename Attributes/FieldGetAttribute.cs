using System;

namespace Libvaxy.Attributes
{
	/// <summary>
	/// An attribute that lets you fetch non-public class fields without a runtime reflection penalty.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class FieldGetAttribute : Attribute
	{
		internal string fieldName;

		public FieldGetAttribute(string fieldName) => this.fieldName = fieldName;
	}
}
