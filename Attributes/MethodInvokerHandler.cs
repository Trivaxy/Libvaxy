using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Libvaxy.Attributes
{
	internal class MethodInvokerHandler
	{
		internal static void ApplyMethodInvokers()
		{
			foreach (MethodInfo method in Reflection.GetMethodsWithAttribute<MethodInvokerAttribute>())
			{
				if (method.GetCustomAttribute<ExtensionAttribute>() == null)
					throw new LibvaxyException($"The MethodInvoker method '{method.FullMemberName()}' must be an extension method targetting the type with the specified target method");

				MethodInvokerAttribute attribute = method.GetCustomAttribute<MethodInvokerAttribute>();
				Type targetType = method.GetParameters()[0].ParameterType;

				if (!Reflection.HasMethod(targetType, attribute.methodName))
					throw new LibvaxyException($"The MethodInvoker method '{method.FullMemberName()}' targets method '{attribute.methodName}' which does not exist in type '{targetType.FullName}'");

				MethodInfo targetMethodInfo = Reflection.GetMethodInfo(targetType, attribute.methodName, Reflection.GetParameterTypes(method).Skip(1).ToArray());

				if (method.ReturnType != targetMethodInfo.ReturnType && targetMethodInfo.ReturnType.IsSubclassOf(method.ReturnType))
					throw new LibvaxyException($"The MethodInvoker method '{method.FullMemberName()}' does not have a return type that matches or is a parent of the return type of the targetted method '{attribute.methodName}'");

				Type[] targetMethodParameterTypes = Reflection.GetParameterTypes(targetMethodInfo);
				int parameterCount = targetMethodParameterTypes.Length;

				if (!Reflection.GetParameterTypes(method).Skip(1).SequenceEqual(targetMethodParameterTypes))
					throw new LibvaxyException($"The MethodInvoker method '{method.FullMemberName()}' does not have parameter types that match the targetted method '{attribute.methodName}'");

				ILHook hook = new ILHook(method, il =>
				{
					il.Body.Instructions.Clear();
					ILCursor cursor = new ILCursor(il);

					if (!targetMethodInfo.IsStatic)
						cursor.Emit(OpCodes.Ldarg_0);

					for (int i = 1; i <= parameterCount; i++)
					{
						switch (i)
						{
							case 1:
								cursor.Emit(OpCodes.Ldarg_1);
								break;

							case 2:
								cursor.Emit(OpCodes.Ldarg_2);
								break;

							case 3:
								cursor.Emit(OpCodes.Ldarg_3);
								break;

							default:
								cursor.Emit(OpCodes.Ldarg_S, (byte)i);
								break;
						}
					}

					cursor.Emit(OpCodes.Call, targetMethodInfo);
					cursor.Emit(OpCodes.Ret);
				});

				LibvaxyMod.DisposeOnUnload(hook);
			}
		}
	}
}
