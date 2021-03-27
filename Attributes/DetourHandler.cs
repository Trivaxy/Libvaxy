using MonoMod.RuntimeDetour;
using System;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader;

namespace Libvaxy.Attributes
{
	/// <summary>
	/// Internal class used by Libvaxy to apply mod detours.
	/// </summary>
	internal class DetourHandler
	{
		// TODO: Make it possible to call original method easily + pass in an instance of the caller
		internal static void ApplyDetours()
		{
			foreach (MethodInfo method in Reflection.GetMethodsWithAttribute<DetourAttribute>())
			{
				DetourAttribute attribute = method.GetCustomAttribute<DetourAttribute>();
				string modName = attribute.typeName.Split('.')[0];

				if (!LibvaxyMod.ModAssemblies.ContainsKey(modName))
				{
					LibvaxyMod.Logger.Warn("Attempted to detour an unknown / unloaded mod, ignoring and moving on...");
					continue;
				}

				Type targetMethodType = LibvaxyMod.ModAssemblies[modName].GetType(attribute.typeName);

				if (targetMethodType == null)
					throw new LibvaxyException("Could not find the target type to perform detour");

				MethodInfo targetMethod = Reflection.GetMethodInfo(targetMethodType, attribute.methodName, attribute.parameterTypes ?? new Type[0]);

				// TODO: move these checks elsewhere as a utility
				if (targetMethod == null)
					throw new LibvaxyException("Could not find the target method to perform detour");

				if (!Reflection.GetParameterTypes(method).SequenceEqual(Reflection.GetParameterTypes(targetMethod)))
					throw new LibvaxyException("The target method and detour method do not have matching parameter types");

				if (method.ReturnType != targetMethod.ReturnType)
					throw new LibvaxyException("The target method and detour method do not have matching return types");

				LibvaxyMod.DisposeOnUnload(new Detour(targetMethod, method));
				LibvaxyMod.Logger.Info($"Registered detour from {targetMethod.FullMemberName()}\nTo: {method.FullMemberName()}");
			}
		}
	}
}
