using System;

namespace Libvaxy.Attributes
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class HookAttribute : Attribute
	{
		internal Type eventParentType;
		internal string hookName;

		public HookAttribute(Type eventParentType, string hookName)
		{
			this.eventParentType = eventParentType;
			this.hookName = hookName;
		}
	}
}
