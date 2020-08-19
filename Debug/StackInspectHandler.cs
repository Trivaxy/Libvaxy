using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Collections.Generic;
using System.Reflection;

namespace Libvaxy.Debug
{
	internal static class StackInspectHandler
	{
		internal static Dictionary<EventInfo, int> inspectedMethodsRegisterCount;
		private static MethodInfo logStackAt;

		internal static void Initialize()
		{
			inspectedMethodsRegisterCount = new Dictionary<EventInfo, int>();
			logStackAt = typeof(StackInspectHandler).GetMethod(nameof(LogStackAt), Reflection.AnyStatic);
		}

		internal static void ApplyStackInspection(StackInspectTarget target)
		{
			EventInfo hook = target.HookType.GetEvent(target.HookName, Reflection.AnyStatic);

			if (hook == null)
				throw new LibvaxyException($"Unknown method to apply stack inspection to: {target.HookType}.{target.HookName}");

			var stackInspectionHandler = new ILContext.Manipulator(context =>
			{
				if (inspectedMethodsRegisterCount.TryGetValue(hook, out int registerCount) && registerCount + 1 >= 2)
				{
					LibvaxyMod.Logger.Warn($"Tried to inspect {hook.FullMemberName()} more than once. Ignoring...");
					return;
				}

				ILCursor il = new ILCursor(context);

				int startIndex = target.BeginIndex ?? 0;
				int endIndex = target.EndIndex ?? context.Instrs.Count;

				il.Emit(OpCodes.Ldnull);

				while (il.Index < endIndex + ((endIndex - startIndex) * 4))
				{
					il.Emit(OpCodes.Dup);
					il.Emit(OpCodes.Ldstr, il.Instrs[il.Index].OpCode.Name);
					il.Emit(OpCodes.Ldstr, target.HookName);
					il.Emit(OpCodes.Call, logStackAt);
					il.Index++;
				}

				il.Index++;
				il.Emit(OpCodes.Pop);

				inspectedMethodsRegisterCount[hook] = 1;
			});

			hook.AddEventHandler(null, stackInspectionHandler);
		}

		internal static void Unload()
		{
			inspectedMethodsRegisterCount = null;
			logStackAt = null;
		}

#pragma warning disable IDE0051
		private static void LogStackAt(object obj, string instruction, string method)
#pragma warning restore IDE0051
		{

			LibvaxyMod.Logger.Debug($"Topmost value in stack of {method} BEFORE [{instruction}] is run: {obj ?? "None"}");
		}
	}
}
