using System;

namespace Libvaxy.Attributes
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class MethodInvokerAttribute : Attribute
	{
		internal string methodName;

		public MethodInvokerAttribute(string methodName) => this.methodName = methodName;
	}
}
