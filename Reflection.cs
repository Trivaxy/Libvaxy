using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Terraria.ModLoader;

namespace Libvaxy
{
	/// <summary>
	/// Provides many useful utilities to make reflection easy. Does automatic caching when needed.
	/// </summary>
	public static class Reflection
	{
		public const BindingFlags AllFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
		public const BindingFlags AnyPublic = BindingFlags.Public | BindingFlags.NonPublic;
		public const BindingFlags AnyInstance = AnyPublic | BindingFlags.Instance;
		public const BindingFlags AnyStatic = AnyPublic | BindingFlags.Static;

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

		/// <summary>
		/// Checks if a given type has a field.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="name">The field name.</param>
		/// <returns>Whether the type has the field or not.</returns>
		public static bool HasField(Type type, string name, BindingFlags flags = AllFlags)
			=> GetFieldInfo(type, name, flags) != null;

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

		/// <summary>
		/// Checks whether the given type has a property.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="name">The name of the property.</param>
		/// <returns>Whether the type has the property or not.</returns>
		public static bool HasProperty(Type type, string name, BindingFlags flags = AllFlags)
			=> GetPropertyInfo(type, name, flags) != null;

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

		/// <summary>
		/// Checks whether the specified type has a method with the specified name and parameter types.
		/// </summary>
		/// <param name="type">The type to search in</param>
		/// <param name="name">The name of the method</param>
		/// <param name="flags">Any optional flags you want to pass in</param>
		/// <param name="paramTypes">The types of the parameters in the method, in order</param>
		/// <returns>Whether or not the type has a matching method</returns>
		public static bool HasMethod(Type type, string name, BindingFlags flags = AllFlags, bool ignoreParameters = true, params Type[] paramTypes)
		{
			if (!ignoreParameters)
				return GetMethodInfo(type, name, paramTypes, flags) != null;

			foreach (MethodInfo method in type.GetMethods(flags))
				if (method.Name == name)
					return true;

			return false;
		}

		/// <summary>
		/// Searches for a constructor in the passed instance's type matching the specified parameter types and invokes it on the object.
		/// </summary>
		/// <param name="obj">The instance to search in and to also pass to the constructor</param>
		/// <param name="ctorParamTypes">The parameter types of the constructor, in order</param>
		/// <param name="parameters">The values to pass to the constructor that is found</param>
		public static void InvokeConstructor(object obj, Type[] ctorParamTypes, params object[] parameters)
		{
			string key = FullMethodName(obj.GetType(), ".ctor", GetObjectTypes(parameters));
			if (!constructorCache.ContainsKey(key))
				CacheInfo(GetConstructorInfo(obj.GetType(), ctorParamTypes));
			constructorCache[key].Invoke(obj, parameters);
		}

		/// <summary>
		/// Checks whether the specified type has a constructor with the specified parameter types.
		/// </summary>
		/// <param name="type">The type to search in</param>
		/// <param name="flags">Any optional flags you want to pass in</param>
		/// <param name="paramTypes">The types of the constructor parameters, in order</param>
		/// <returns>Whether or not the type has a matching constructor</returns>
		public static bool HasConstructor(Type type, BindingFlags flags = AllFlags, params Type[] paramTypes)
			=> GetConstructorInfo(type, paramTypes, flags) != null;

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
		public static MethodInfo GetMethodInfo(Type type, string name, Type[] paramTypes = null, BindingFlags flags = AllFlags) => type.GetMethod(name, flags, null, paramTypes ?? Type.EmptyTypes, null);

		/// <summary>
		/// Gets a ConstructorInfo without internally caching it.
		/// </summary>
		/// <param name="type">The type containing the property</param>
		/// <param name="paramTypes">The parameter types of the constructor, in order</param>
		/// <returns>The acquired ConstructorInfo</returns>
		public static ConstructorInfo GetConstructorInfo(Type type, Type[] paramTypes, BindingFlags flags = AllFlags) => type.GetConstructor(flags, Type.DefaultBinder, paramTypes, null);

		/// <summary>
		/// Gets the type of each parameter passed in.
		/// </summary>
		/// <param name="parameters">The parameters</param>
		/// <returns>An array containing the type of each parameter, in order</returns>
		public static Type[] GetObjectTypes(params object[] parameters) => parameters.Select(p => p.GetType()).ToArray();

		/// <summary>
		/// Returns a corresponding array of types representing the parameter types of the method. 
		/// </summary>
		/// <param name="info">The method with the parameters</param>
		/// <returns>An array of the parameter types</returns>
		public static Type[] GetParameterTypes(MethodInfo info) => info.GetParameters().Select(p => p.ParameterType).ToArray();

		/// <summary>
		/// Creates an uninitalized instance of the type T, completely disregarding constructors.
		/// </summary>
		/// <typeparam name="T">The type to create an instance for</typeparam>
		/// <returns>An instance of the type T</returns>
		public static T CreateInstance<T>() => (T)CreateInstance(typeof(T));

		/// <summary>
		/// Creates an uninitalized instance of the specified type and casts it to T, completely disregarding constructors.
		/// </summary>
		/// <typeparam name="T">The type to cast the created instance to</typeparam>
		/// <returns>An instance of the specified type</returns>
		public static T CreateInstance<T>(Type type) => (T)CreateInstance(type);

		/// <summary>
		/// Creates an uninitialized instance of the specified type, completely disregarding constructors.
		/// </summary>
		/// <param name="type">The type to create an instance for</param>
		/// <returns>An instance of the specified type</returns>
		public static object CreateInstance(Type type) => FormatterServices.GetUninitializedObject(type);

		/// <summary>
		/// Searches the given assembly (or all mod assemblies if no assembly is specified) for a type matching the specified <b>full</b> name.
		/// </summary>
		/// <param name="name">The <b>full</b> name of the type</param>
		/// <param name="assembly">The assembly to search in. If this is null, all mod assemblies are searched</param>
		/// <returns>A type matching the specified name</returns>
		public static Type GetType(string name, Assembly assembly = null)
		{
			Type[] types = assembly?.GetTypes() ?? LibvaxyMod.ModAssemblies.Values.SelectMany(asm => asm.GetTypes()).ToArray();

			return types
                .First(type => type.FullName == name);
		}

		/// <summary>
		/// Searches the given assembly (or all mod assemblies if no assembly is specified) for types holding the specified attribute.
		/// </summary>
		/// <typeparam name="T">The type of the attribute to search for</typeparam>
		/// <param name="assembly">The assembly to search in. If this is null, all mod assemblies are searched</param>
		/// <param name="inherited">Whether or not to also search the inheritance tree of types for the attribute</param>
		/// <returns>An array of all types holding the specified attribute</returns>
		public static Type[] GetTypesWithAttribute<T>(Assembly assembly = null, bool inherited = false)
			where T: Attribute
		{
			Type[] types = assembly?.GetTypes() ?? LibvaxyMod.ModAssemblies.Values.SelectMany(asm => asm.GetTypes()).ToArray();

			return types
				.Where(t => t.GetCustomAttributes(typeof(T), inherited).Length > 0)
				.ToArray();
		}

		/// <summary>
		/// Searches the given assembly (or all mod assemblies if no assembly is specified) for methods holding the specified attribute.
		/// </summary>
		/// <typeparam name="T">The type of the attribute to search for</typeparam>
		/// <param name="assembly">The assembly to search in. If this is null, all mod assemblies are searched</param>
		/// <param name="inherited">Whether or not to also search the inheritance tree of the type containing the method for the attribute</param>
		/// <returns>An array of all methods holding the specified attribute</returns>
		public static MethodInfo[] GetMethodsWithAttribute<T>(Assembly assembly = null, bool inherited = false)
			where T: Attribute
		{
			Type[] types = assembly?.GetTypes() ?? LibvaxyMod.ModAssemblies.Values.SelectMany(asm => asm.GetTypes()).ToArray();

			return types
				.SelectMany(t => t.GetMethods(AllFlags))
				.Where(m => m.GetCustomAttributes(typeof(T), inherited).Length > 0)
				.ToArray();
		}

		/// <summary>
		/// Searches the given assembly (or all mod assemblies if no assembly is specified) for types extending the specified type.
		/// </summary>
		/// <param name="type">The parent type to find the extending types of</param>
		/// <param name="assembly">The assembly to search in. If this is null, all mod assemblies are searched</param>
		/// <returns>An array of all the types extending from the specified type</returns>
		public static Type[] GetSubtypes(Type type, Assembly assembly = null)
		{
			Type[] types = assembly?.GetTypes() ?? LibvaxyMod.ModAssemblies.Values.SelectMany(asm => asm.GetTypes()).ToArray();

			return types
				.Where(t => t.IsSubclassOf(type))
				.ToArray();
		}

		/// <summary>
		/// Searches the given assembly (or all mod assemblies if no assembly is specified) for non-abstract types extending the specified type.
		/// </summary>
		/// <param name="type">The parent type to find the non-abstract extending types of</param>
		/// <param name="assembly">The assembly to search in. If this is null, all mod assemblies are searched</param>
		/// <returns>An array of all the non-abstract types extending from the specified type</returns>
		public static Type[] GetNonAbstractSubtypes(Type type, Assembly assembly = null)
		{
			Type[] types = assembly?.GetTypes() ?? LibvaxyMod.ModAssemblies.Values.SelectMany(asm => asm.GetTypes()).ToArray();

			return types
				.Where(t => t.IsSubclassOf(type) && !t.IsAbstract)
				.ToArray();
		}

		// will cache the provided MemberInfo into its respective cache dictionary
		internal static void CacheInfo(MemberInfo info)
		{
			string key = FullMemberName(info);

			if (info is FieldInfo fieldInfo)
				fieldCache[key] = fieldInfo;
			else if (info is PropertyInfo propertyInfo)
				propertyCache[key] = propertyInfo;
			else if (info is MethodInfo methodInfo)
				methodCache[key] = methodInfo;
			else if (info is ConstructorInfo constructorInfo)
				constructorCache[key] = constructorInfo;
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