using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Libvaxy.Attributes
{
	/// <summary>
	/// Internal class used by Libvaxy to prepare FieldGet methods.
	/// </summary>
	internal static class FieldSetHandler
	{
		internal static void ApplyFieldSets()
		{
			foreach (MethodInfo method in Reflection.GetMethodsWithAttribute<FieldSetAttribute>())
			{
				if (method.ReturnType != typeof(void))
					throw new LibvaxyException($"The FieldSet method '{method.FullMemberName()}' must have a return type of void");

				if (method.GetCustomAttribute<ExtensionAttribute>() == null)
					throw new LibvaxyException($"The FieldSet method '{method.FullMemberName()} must be an extension method targetting the type with the specified field");

				ParameterInfo[] parameters = method.GetParameters();

				if (parameters.Length != 2)
					throw new LibvaxyException($"The FieldSet method '{method.FullMemberName()}' must have a second parameter whose type matches the specified field's type");

				Type targetType = parameters[0].ParameterType;
				FieldSetAttribute attribute = method.GetCustomAttribute<FieldSetAttribute>();

				if (!Reflection.HasField(targetType, attribute.fieldName))
					throw new LibvaxyException($"The FieldSet method '{method.FullMemberName()}' targets field '{attribute.fieldName}' which does not exist in type '{targetType.FullName}'");

				FieldInfo targetFieldInfo = Reflection.GetFieldInfo(targetType, attribute.fieldName);
				ParameterInfo valueParameter = parameters[1];

				if (valueParameter.ParameterType != targetFieldInfo.FieldType)
					throw new LibvaxyException($"The setter parameter '{valueParameter.Name}' in the FieldSet method '{method.FullMemberName()}' does not have a type matching the target field '{attribute.fieldName}'");

				ILHook hook = new ILHook(method, il =>
				{
					il.Body.Instructions.Clear();
					ILCursor cursor = new ILCursor(il);

					cursor.Emit(OpCodes.Ldarg_1);

					if (targetFieldInfo.IsStatic)
						cursor.Emit(OpCodes.Stsfld, targetFieldInfo);
					else
					{
						cursor.Emit(OpCodes.Ldarg_0);
						cursor.Emit(OpCodes.Stfld, targetFieldInfo);
					}

					cursor.Emit(OpCodes.Ret);
				});

				LibvaxyMod.DisposeOnUnload(hook);
			}
		}
	}
}