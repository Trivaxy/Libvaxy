using System;
using System.Reflection;

namespace Libvaxy.Attributes
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class DetourAttribute : Attribute
	{
		internal string detourMethodName;

		public DetourAttribute(string detourMethodName) => this.detourMethodName = detourMethodName;
	}
}