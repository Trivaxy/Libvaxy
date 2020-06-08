using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Libvaxy
{
	public static class Reflection
	{
		public const BindingFlags AllFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
		public const BindingFlags AnyPublic = BindingFlags.Public | BindingFlags.NonPublic;

		private static Dictionary<string, FieldInfo> fieldCache;
		private static Dictionary<string, PropertyInfo> propertyCache;
		private static Dictionary<string, MethodInfo> methodCache;
		private static Dictionary<string, ConstructorInfo> constructorCache;

		internal static void InitializeCaches()
		{
			fieldCache = new Dictionary<string, FieldInfo>();
			propertyCache = new Dictionary<string, PropertyInfo>();
			methodCache = new Dictionary<string, MethodInfo>();
			constructorCache = new Dictionary<string, ConstructorInfo>();
		}

		internal static void UnloadCaches()
		{
			fieldCache = null;
			propertyCache = null;
			methodCache = null;
			constructorCache = null;
		}

		/// <summary>
		/// Gets the value of a field inside an instance.
		/// </summary>
		/// <typeparam name="T">The type that the field value should be automatically cast to</typeparam>
		/// <param name="obj">The instance containing the field</param>
		/// <param name="name">The name of the field</param>
		/// <param name="flags">Any optional flags you want to pass in. BindingFlags.Instance is always automatically included</param>
		/// <returns>The value of the field in the instance</returns>
		public static T GetInstanceField<T>(object obj, string name, BindingFlags flags = AnyPublic)
		{
			string key = $"{obj.GetType().FullName}.{name}";
			if (!fieldCache.ContainsKey(key))
				CacheInfo(GetFieldInfo(obj.GetType(), name, flags | BindingFlags.Instance));
			return (T)fieldCache[key].GetValue(obj);
		}

		/// <summary>
		/// Gets the value of a static field in a type.
		/// </summary>
		/// <typeparam name="T">The type that the retrieved field value should be automatically cast to</typeparam>
		/// <param name="type">The type containing the field</param>
		/// <param name="name">The name of the field</param>
		/// <param name="flags">Any optional flags you want to pass in. BindingFlags.Static is always automatically included</param>
		/// <returns>The value of the static field in the type</returns>
		public static T GetStaticField<T>(Type type, string name, BindingFlags flags = AnyPublic)
		{
			string key = $"{type.FullName}.{name}";
			if (!fieldCache.ContainsKey(key))
				CacheInfo(GetFieldInfo(type, name, flags | BindingFlags.Static));
			return (T)fieldCache[key].GetValue(null);
		}

		/// <summary>
		/// Sets the value of a field in an instance.
		/// </summary>
		/// <param name="obj">The instance containing the field</param>
		/// <param name="name">The name of the field</param>
		/// <param name="value">The value to set the field to</param>
		/// <param name="flags">Any optional flags you want to pass in. BindingFlags.Instance is always automatically included</param>
		public static void SetInstanceField(object obj, string name, object value, BindingFlags flags = AnyPublic)
		{
			string key = $"{obj.GetType().FullName}.{name}";
			if (!fieldCache.ContainsKey(key))
				CacheInfo(GetFieldInfo(obj.GetType(), name, flags | BindingFlags.Instance));
			fieldCache[key].SetValue(obj, value);
		}

		/// <summary>
		/// Sets the value of a static field in a type.
		/// </summary>
		/// <param name="type">The type containing the field</param>
		/// <param name="name">The name of the field</param>
		/// <param name="value">The value to set the field to</param>
		/// <param name="flags">Any optional flags you want to pass in. BindingFlags.Static is always automatically included</param>
		public static void SetStaticField(Type type, string name, object value, BindingFlags flags = AnyPublic)
		{
			string key = $"{type.FullName}.{name}";
			if (!fieldCache.ContainsKey(key))
				CacheInfo(GetFieldInfo(type, name, flags | BindingFlags.Static));
			fieldCache[key].SetValue(null, value);
		}

		public static bool HasField(Type type, string name)
			=> GetFieldInfo(type, name) != null;

		/// <summary>
		/// Gets the value of a property inside an instance.
		/// </summary>
		/// <typeparam name="T">The type that the property value should be automatically cast to</typeparam>
		/// <param name="obj">The instance containing the property</param>
		/// <param name="name">The name of the property</param>
		/// <param name="flags">Any optional flags you want to pass in. BindingFlags.Instance is always automatically included</param>
		/// <returns>The value of the property in the instance</returns>
		public static T GetInstanceProperty<T>(object obj, string name, BindingFlags flags = AnyPublic)
		{
			string key = $"{obj.GetType().FullName}.{name}";
			if (!propertyCache.ContainsKey(key))
				CacheInfo(GetPropertyInfo(obj.GetType(), name, flags | BindingFlags.Instance));
			return (T)propertyCache[key].GetValue(obj);
		}

		/// <summary>
		/// Gets the value of a static property in a type.
		/// </summary>
		/// <typeparam name="T">The type that the property value should be automatically cast to</typeparam>
		/// <param name="type">The type containing the property</param>
		/// <param name="name">The name of the property</param>
		/// <param name="flags">Any optional flags you want to pass in. BindingFlags.Static is always automatically included</param>
		/// <returns>The value of the property in the type</returns>
		public static T GetStaticProperty<T>(Type type, string name, BindingFlags flags = AnyPublic)
		{
			string key = $"{type.FullName}.{name}";
			if (!propertyCache.ContainsKey(key))
				CacheInfo(GetPropertyInfo(type, name, flags | BindingFlags.Static));
			return (T)propertyCache[key].GetValue(null);
		}

		/// <summary>
		/// Sets a property in an instance to the specified value.
		/// </summary>
		/// <param name="obj">The instance containing the property</param>
		/// <param name="name">The name of the property</param>
		/// <param name="value">The value to set the property to</param>
		/// <param name="flags">Any optional flags you want to pass in. BindingFlags.Instance is always automatically included</param>
		public static void SetInstanceProperty(object obj, string name, object value, BindingFlags flags = AnyPublic)
		{
			string key = $"{obj.GetType().FullName}.{name}";
			if (!propertyCache.ContainsKey(key))
				CacheInfo(GetPropertyInfo(obj.GetType(), name, flags | BindingFlags.Instance));
			propertyCache[key].SetValue(obj, value);
		}

		/// <summary>
		/// Sets a static property in a type to the specified value.
		/// </summary>
		/// <param name="type">The type containing the property</param>
		/// <param name="name">The name of the property</param>
		/// <param name="value">The value to set the property to</param>
		/// <param name="flags">Any optional flags you want to pass in. BindingFlags.Static is always automatically included</param>
		public static void SetStaticProperty(Type type, string name, object value, BindingFlags flags = AnyPublic)
		{
			string key = $"{type.FullName}.{name}";
			if (!propertyCache.ContainsKey(key))
				CacheInfo(GetPropertyInfo(type, name, flags | BindingFlags.Static));
			propertyCache[key].SetValue(null, value);
		}

		public static bool HasProperty(Type type, string name)
			=> GetPropertyInfo(type, name) != null;

		/// <summary>
		/// Invokes an instance method and gets its return value.
		/// </summary>
		/// <typeparam name="T">The type that the return value of the method should be automatically cast to</typeparam>
		/// <param name="obj">The instance containing the method</param>
		/// <param name="name">The name of the method</param>
		/// <param name="flags">Any optional flags you want to pass in. BindingFlags.Instance is always automatically included</param>
		/// <param name="parameters">The parameters to pass to the method</param>
		/// <returns>The invoked method's return value</returns>
		public static T InvokeInstanceMethod<T>(object obj, string name, BindingFlags flags = AnyPublic, params object[] parameters)
		{
			Type[] paramTypes = GetObjectTypes(parameters);
			string key = FullMethodName(obj.GetType(), name, paramTypes);
			if (!methodCache.ContainsKey(key))
				CacheInfo(GetMethodInfo(obj.GetType(), name, paramTypes, flags | BindingFlags.Instance));
			return (T)methodCache[key].Invoke(obj, parameters);
		}

		/// <summary>
		/// Invokes a static method and gets its return value.
		/// </summary>
		/// <typeparam name="T">The type that the return value of the method should be automatically cast to</typeparam>
		/// <param name="type">The type containing the method</param>
		/// <param name="name">The name of the method</param>
		/// <param name="flags">Any optional flags you want to pass in. BindingFlags.Static is always automatically included</param>
		/// <param name="parameters">The parameters to pass to the method</param>
		/// <returns>The invoked method's return value</returns>
		public static T InvokeStaticMethod<T>(Type type, string name, BindingFlags flags = AnyPublic, params object[] parameters)
		{
			Type[] paramTypes = GetObjectTypes(parameters);
			string key = FullMethodName(type, name, paramTypes);
			if (!methodCache.ContainsKey(key))
				CacheInfo(GetMethodInfo(type, name, paramTypes, flags | BindingFlags.Static));
			return (T)methodCache[key].Invoke(null, parameters);
		}

		public static bool HasMethod(Type type, string name, params Type[] paramTypes)
			=> GetMethodInfo(type, name, paramTypes) != null;

		/// <summary>
		/// Invokes a constructor on an object.
		/// </summary>
		/// <param name="obj">The instance containing the method</param>
		/// <param name="name">The name of the method</param>
		/// <param name="flags">Any optional flags you want to pass in. BindingFlags.Instance is always automatically included</param>
		/// <param name="parameters">The parameters to pass to the method</param>
		/// <returns>The invoked method's return value</returns>
		public static void InvokeConstructor(object obj, Type[] ctorParamTypes, params object[] parameters)
		{
			string key = FullMethodName(obj.GetType(), ".ctor", GetObjectTypes(parameters));
			if (!constructorCache.ContainsKey(key))
				CacheInfo(GetConstructorInfo(obj.GetType(), ctorParamTypes));
			constructorCache[key].Invoke(obj, parameters);
		}

		public static bool HasConstructor(Type type, params Type[] ctorParamTypes)
			=> GetConstructorInfo(type, ctorParamTypes) != null;

		/// <summary>
		/// Gets a FieldInfo without internally caching it.
		/// </summary>
		/// <param name="type">The type containing the field</param>
		/// <param name="name">The name of the field</param>
		/// <param name="flags">Any optional flags you want to pass in. By default, most flags are automatically included</param>
		/// <returns>The acquired FieldInfo</returns>
		public static FieldInfo GetFieldInfo(Type type, string name, BindingFlags flags = AllFlags) => type.GetField(name, flags);

		/// <summary>
		/// Gets a PropertyInfo without internally caching it.
		/// </summary>
		/// <param name="type">The type containing the property</param>
		/// <param name="name">The name of the property</param>
		/// <param name="flags">Any optional flags you want to pass in. By default, most flags are automatically included</param>
		/// <returns>The acquired PropertyInfo</returns>
		public static PropertyInfo GetPropertyInfo(Type type, string name, BindingFlags flags = AllFlags) => type.GetProperty(name, flags);

		/// <summary>
		/// Gets a MethodInfo without internally caching it.
		/// </summary>
		/// <param name="type">The type containing the method</param>
		/// <param name="name">The name of the method</param>
		/// <param name="paramTypes">The parameter types of the method, in order</param>
		/// <param name="flags">Any optional flags you want to pass in. By default, most flags are automatically included</param>
		/// <returns>The acquired MethodInfo</returns>
		public static MethodInfo GetMethodInfo(Type type, string name, Type[] paramTypes = null, BindingFlags flags = AllFlags) => type.GetMethod(name, flags, null, paramTypes, null);

		/// <summary>
		/// Gets a ConstructorInfo without internally caching it.
		/// </summary>
		/// <param name="type">The type containing the property</param>
		/// <param name="paramTypes">The parameter types of the constructor, in order</param>
		/// <returns>The acquired ConstructorInfo</returns>
		public static ConstructorInfo GetConstructorInfo(Type type, Type[] paramTypes) => type.GetConstructor(paramTypes);

		/// <summary>
		/// Gets the type of each parameter passed in.
		/// </summary>
		/// <param name="parameters">The parameters</param>
		/// <returns>An array containing the type of each parameter, in order</returns>
		public static Type[] GetObjectTypes(params object[] parameters) => parameters.Select(p => p.GetType()).ToArray();

		public static Type[] GetParameterTypes(MethodInfo info) => info.GetParameters().Select(p => p.ParameterType).ToArray();

		/// <summary>
		/// Searches the given assembly for types holding the specified attribute.
		/// </summary>
		/// <typeparam name="T">The type of the attribute</typeparam>
		/// <param name="assembly">The assembly to search in</param>
		/// <returns>An array of all types in the assembly holding the specified attribute</returns>
		public static Type[] GetTypesWithAttribute<T>(Assembly assembly = null, bool inherited = false)
			where T: Attribute
		{
			Type[] types = assembly?.GetTypes() ?? Libvaxy.ModAssemblies.Values.SelectMany(asm => asm.GetTypes()).ToArray();

			return types
				.Where(t => t.GetCustomAttributes(typeof(T), inherited).Length > 0)
				.ToArray();
		}

		public static MethodInfo[] GetMethodsWithAttribute<T>(Assembly assembly = null, bool inherited = false)
			where T: Attribute
		{
			Type[] types = assembly?.GetTypes() ?? Libvaxy.ModAssemblies.Values.SelectMany(asm => asm.GetTypes()).ToArray();

			return types
				.SelectMany(t => t.GetMethods(AllFlags))
				.Where(m => m.GetCustomAttributes(typeof(T), inherited).Length > 0)
				.ToArray();
		}

		// will cache the provided MemberInfo into its respective cache dictionary
		internal static void CacheInfo(MemberInfo info)
		{
			string key = FullMemberName(info);

			if (info is FieldInfo)
				fieldCache[key] = (FieldInfo)info;
			else if (info is PropertyInfo)
				propertyCache[key] = (PropertyInfo)info;
			else if (info is MethodInfo)
				methodCache[key] = (MethodInfo)info;
			else if (info is ConstructorInfo)
				constructorCache[key] = (ConstructorInfo)info;
		}

		internal static string FullMemberName(this MemberInfo info)
		{
			string name = $"{info.ReflectedType.FullName}.{info.Name}";

			if (info is MethodBase methodBase)
			{
				Type returnType = info is MethodInfo ? ((MethodInfo)info).ReturnType : typeof(void);
				name = FullMethodName(info.ReflectedType, info.Name, methodBase.GetParameters().Select(p => p.ParameterType).ToArray());
			}

			return name;
		}

		// format: AssemblyName.TypeName.MethodName[ParameterType1, ParameterType2, ParameterType3, ...]
		// e.g the full name of System.Random.Next(int, int) is System.Random.Next[Int32, Int32]
		// the name of every constructor is .ctor so e.g System.Random..ctor[Int32] stands for new Random(int)
		// this method is ugly
		internal static string FullMethodName(Type reflectedType, string methodName, params Type[] parameters)
			=> $"{reflectedType.FullName}.{methodName}[{string.Join(", ", parameters.Select(t => t.Name).ToArray())}]";
	}
}