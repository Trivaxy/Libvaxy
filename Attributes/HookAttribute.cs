using System;

namespace Libvaxy.Attributes
{
	/// <summary>
	/// An attribute that subscribes a method to an event in the On.Terraria and IL.Terraria namespaces.
	/// </summary>
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
