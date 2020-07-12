using MonoMod.Utils;
using System;
using System.Reflection;
using System.Reflection.Emit;
using Terraria.ModLoader.IO;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace Libvaxy.GameHelpers.IO
{
	public class AutoSerializer<T>
	{
		private static Func<T, TagCompound> serializer;
		private static Func<TagCompound, T> deserializer;

		public TagCompound Serialize(T obj)
		{
			if (serializer == null)
				serializer = CreateSerializer<T>();
			return serializer.Invoke(obj);
		}

		public T Deserialize(TagCompound tag)
		{
			if (deserializer == null)
				deserializer = CreateDeserializer<T>();
			return deserializer.Invoke(tag);
		}

		public void Deserialize(TagCompound tag, ref T obj) => obj = Deserialize(tag);

#pragma warning disable CS0693
		private static Func<T, TagCompound> CreateSerializer<T>()
#pragma warning restore CS0693
		{
			Type type = typeof(T);
			Type tagCompound = typeof(TagCompound);

			DynamicMethod serializer = new DynamicMethod(
				"AutoSerialize_" + type.Name,
				tagCompound,
				new Type[] { type },
				typeof(Libvaxy).Module,
				true
				);

			ILGenerator il = serializer.GetILGenerator(1024);

			il.Emit(OpCodes.Newobj, Reflection.GetConstructorInfo(tagCompound, Type.EmptyTypes));

			foreach (FieldInfo field in type.GetFields(Reflection.AnyInstance))
			{
				il.Emit(OpCodes.Dup);
				il.Emit(OpCodes.Ldstr, field.Name);

				if (field.FieldType.IsPrimitive)
				{
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldfld, Reflection.GetFieldInfo(type, field.Name, Reflection.AnyInstance));
					il.Emit(OpCodes.Box, field.FieldType);
				}
				else
				{
					Type innerAutoSerializer = typeof(AutoSerializer<>).MakeGenericType(field.FieldType);
					il.Emit(OpCodes.Newobj, Reflection.GetConstructorInfo(innerAutoSerializer, Type.EmptyTypes));
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldfld, Reflection.GetFieldInfo(type, field.Name, Reflection.AnyInstance));
					il.Emit(OpCodes.Callvirt, Reflection.GetMethodInfo(
						innerAutoSerializer,
						nameof(Serialize),
						new Type[] { field.FieldType })
						);
				}

				il.Emit(OpCodes.Callvirt, Reflection.GetMethodInfo(
						tagCompound,
						"Add",
						new Type[] { typeof(string), typeof(object) }
						));
			}

			il.Emit(OpCodes.Ret);

			return (Func<T, TagCompound>)serializer.CreateDelegate<Func<T, TagCompound>>();
		}

#pragma warning disable CS0693
		private static Func<TagCompound, T> CreateDeserializer<T>()
#pragma warning restore CS0693
		{
			Type type = typeof(T);
			Type tagCompound = typeof(TagCompound);

			DynamicMethod deserializer = new DynamicMethod(
				"AutoDeserialize_" + type.Name,
				type,
				new Type[] { tagCompound },
				typeof(Libvaxy).Module,
				true
				);

			ILGenerator il = deserializer.GetILGenerator(1024);

			il.DeclareLocal(type);

			Action pushObj;

			if (!type.IsValueType)
			{
				il.Emit(OpCodes.Newobj, Reflection.GetConstructorInfo(type, Type.EmptyTypes));
				il.Emit(OpCodes.Stloc_0);
				pushObj = () => il.Emit(OpCodes.Ldloc_0);
			}
			else
			{
				il.Emit(OpCodes.Ldloca_S, (byte)0);
				il.Emit(OpCodes.Initobj, type);
				pushObj = () => il.Emit(OpCodes.Ldloca_S, (byte)0);
			}

			foreach (FieldInfo field in type.GetFields(Reflection.AnyInstance))
			{
				pushObj();

				if (field.FieldType.IsPrimitive)
				{
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldstr, field.Name);
					il.Emit(OpCodes.Callvirt, Reflection.GetMethodInfo(
						tagCompound,
						"Get",
						new Type[] { typeof(string) })
						.MakeGenericMethod(new Type[] { field.FieldType }));
				}
				else
				{
					Type innerAutoSerializer = typeof(AutoSerializer<>).MakeGenericType(field.FieldType);
					il.Emit(OpCodes.Newobj, Reflection.GetConstructorInfo(innerAutoSerializer, Type.EmptyTypes));
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldstr, field.Name);
					il.Emit(OpCodes.Callvirt, Reflection.GetMethodInfo(
						tagCompound,
						"GetCompound",
						new Type[] { typeof(string) }));

					il.Emit(OpCodes.Call, Reflection.GetMethodInfo(
						innerAutoSerializer,
						nameof(Deserialize),
						new Type[] { tagCompound }
						));
				}

				il.Emit(OpCodes.Stfld, field);
			}

			il.Emit(OpCodes.Ldloc_0);
			il.Emit(OpCodes.Ret);

			return (Func<TagCompound, T>)deserializer.CreateDelegate<Func<TagCompound, T>>();
		}
	}
}