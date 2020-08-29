using System;

namespace Libvaxy.Attributes
{
	/// <summary>
	/// An attribute that lets you detour any method completely (including mod methods).
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class DetourAttribute : Attribute
	{
		internal string typeName;
		internal string methodName;
		internal Type[] parameterTypes;

		public DetourAttribute(string typeName, string methodName, Type[] parameterTypes = null)
		{
			this.typeName = typeName;
			this.methodName = methodName;
			this.parameterTypes = parameterTypes;
		}
	}
}
