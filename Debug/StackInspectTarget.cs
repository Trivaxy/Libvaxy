using System;

namespace Libvaxy.Debug
{
	public readonly struct StackInspectTarget
	{
		public readonly Type HookType;
		public readonly string HookName;
		public readonly int? BeginIndex;
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
