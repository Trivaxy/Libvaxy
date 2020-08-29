using System;

namespace Libvaxy.Debug
{
	/// <summary>
	/// Represents a target MonoMod/TerrariaHooks method that can have its stack inspected.
	/// </summary>
	public readonly struct StackInspectTarget
	{
		/// <summary>
		/// The type of the class containing the method event.
		/// </summary>
		public readonly Type HookType;

		/// <summary>
		/// The name of the method/event.
		/// </summary>
		public readonly string HookName;

		/// <summary>
		/// The instruction index to begin the stack inspection for.
		/// </summary>
		public readonly int? BeginIndex;

		/// <summary>
		/// The instruction index where the stack inspection stops.
		/// </summary>
		public readonly int? EndIndex;

		public StackInspectTarget(Type hookType, string hookName, int beginIndex, int endIndex)
		{
			HookType = hookType;
			HookName = hookName;
			BeginIndex = beginIndex;
			EndIndex = endIndex;
		}

		public StackInspectTarget(Type hookType, string hookName, int beginIndex)
		{
			HookType = hookType;
			HookName = hookName;
			BeginIndex = beginIndex;
			EndIndex = null;
		}

		public StackInspectTarget(Type hookType, string hookName)
		{
			HookType = hookType;
			HookName = hookName;
			BeginIndex = null;
			EndIndex = null;
		}
	}
}
