using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Libvaxy.Attributes
{
	internal static class DetourHandler
	{
		internal static void HandleDetours()
		{
			MethodInfo[] methods = Reflection.GetMethodsWithAttribute<DetourAttribute>(Assembly.GetExecutingAssembly());

			foreach (MethodInfo method in methods)
				foreach (DetourAttribute attribute in method.GetCustomAttributes(typeof(DetourAttribute)))
					Libvaxy.DisposeOnUnload(new Detour(method, attribute.detourMethod));
		}
	}
}
