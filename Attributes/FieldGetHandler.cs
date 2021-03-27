using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terraria;

namespace Libvaxy.Attributes
{
	/// <summary>
	/// Internal class used by Libvaxy to prepare FieldGet methods.
	/// </summary>
	internal static class FieldGetHandler
	{
		internal static void ApplyFieldGets()
		{
			foreach (MethodInfo method in Reflection.GetMethodsWithAttribute<FieldGetAttribute>())
			{
				if (method.ReturnType == typeof(void))
					throw new LibvaxyException($"The FieldGet method '{method.FullMemberName()}' cannot have a return type of void");

				if (method.GetCustomAttribute<ExtensionAttribute>() == null)
					throw new LibvaxyException($"The FieldGet method '{method.FullMemberName()} must be an extension method targetting the type with the specified field");

				Type targetType = method.GetParameters()[0].ParameterType;
				FieldGetAttribute attribute = method.GetCustomAttribute<FieldGetAttribute>();

				if (!Reflection.HasField(targetType, attribute.fieldName))
					throw new LibvaxyException($"The FieldGet method '{method.FullMemberName()}' targets field '{attribute.fieldName}' which does not exist in type '{targetType.FullName}'");

				ILHook hook = new ILHook(method, il =>
				{
					il.Body.Instructions.Clear();
					ILCursor cursor = new ILCursor(il);

					FieldInfo typeInfo = Reflection.GetFieldInfo(targetType, attribute.fieldName);

					if (typeInfo.IsStatic)
						cursor.Emit(OpCodes.Ldsfld, typeInfo);
					else
					{
						cursor.Emit(OpCodes.Ldarg_0);
						cursor.Emit(OpCodes.Ldfld, typeInfo);
					}

					cursor.Emit(OpCodes.Ret);
				});

				LibvaxyMod.DisposeOnUnload(hook);
			}
		}
	}
}