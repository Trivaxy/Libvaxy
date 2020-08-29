using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.Audio;

namespace Libvaxy
{
	/// <summary>
	/// Represents a wrapper around Mod objects that can retrieve its internal tModLoader-supported resources (things such as ModItem, Texture2D, etc).
	/// </summary>
	public class ModAnalyzer
	{
		private Mod mod;

		public ModAnalyzer(Mod mod) => this.mod = mod;

		private static readonly Dictionary<Type, string> typeToName = new Dictionary<Type, string>()
		{
			{ typeof(Texture2D), "textures" },
			{ typeof(SoundEffect), "sounds" },
			{ typeof(Music), "musics" },
			{ typeof(DynamicSpriteFont), "fonts" },
			{ typeof(Effect), "effects" },
			{ typeof(ModItem), "items" },
			{ typeof(GlobalItem), "globalItems" },
			{ typeof(EquipTexture), "equipTextures" },
			{ typeof(ModPrefix), "prefixes" },
			{ typeof(ModDust), "dusts" },
			{ typeof(ModTile), "tiles" },
			{ typeof(GlobalTile), "globalTiles" },
			{ typeof(ModTileEntity), "tileEntities" },
			{ typeof(ModWall), "walls" },
			{ typeof(GlobalWall), "globalWalls" },
			{ typeof(ModProjectile), "projectiles" },
			{ typeof(GlobalProjectile), "globalProjectiles" },
			{ typeof(ModNPC), "npcs" },
			{ typeof(GlobalNPC), "globalNPCs" },
			{ typeof(ModPlayer), "players" },
			{ typeof(ModMountData), "mountDatas" },
			{ typeof(ModBuff), "buffs" },
			{ typeof(GlobalBuff), "globalBuffs" },
			{ typeof(ModWorld), "worlds" },
			{ typeof(ModUgBgStyle), "ugBgStyles" },
			{ typeof(ModSurfaceBgStyle), "surfaceBgStyles" },
			{ typeof(GlobalBgStyle), "globalBgStyles" },
			{ typeof(ModWaterStyle), "waterStyles" },
			{ typeof(ModWaterfallStyle), "waterfallStyles" },
			{ typeof(ModRecipe), "recipes" },
			{ typeof(GlobalRecipe), "globalRecipes" },
			{ typeof(ModTranslation), "translations" },
			{ typeof(ModGore), "gores (Non-existent field)" } // this is here so that it still counts as a resource type
		};

		/// <summary>
		/// Retrieves the resources within a mod. All possible types can be found in Libvaxy's documentation
		/// </summary>
		/// <typeparam name="T">The type of resource to get</typeparam>
		/// <returns>An array of resources of the specified type</returns>
		public T[] GetResources<T>()
		{
			Type resourceType = typeof(T);

			if (!typeToName.ContainsKey(resourceType))
				throw new LibvaxyException("Tried to get an unknown mod resource type");

			if (resourceType == typeof(ModGore)) // ModGore is an extreme edge case
				return Reflection.GetStaticField<IDictionary<int, ModGore>>(typeof(ModGore), "modGores")
					.Where(kvp => kvp.Value.GetType().Assembly == mod.Code)
					.Select(kvp => kvp.Value)
					.ToArray() as T[];

			string fieldName = typeToName[resourceType];
			object value = Reflection.GetInstanceField<object>(mod, fieldName);

			if (value == null) // should never be null, this check is redundant
				return null;

			if (resourceType == typeof(ModRecipe))
				return (value as IList<ModRecipe>).ToArray() as T[];

			if (resourceType == typeof(EquipTexture))
				return (value as IDictionary<Tuple<string, EquipType>, EquipTexture>).Values.ToArray() as T[];

			return (value as IDictionary<string, T>).Values.ToArray();
		}

		/// <summary>
		/// Gets all the resources inside a mod as a Dictionary whose keys are the Type of the resource and the values are the collection of the resource.
		/// </summary>
		/// <returns>Returns all resources inside a mod as a Dictionary</returns>
		public Dictionary<Type, object> GetAllResources()
		{
			// i hate this.
			return new Dictionary<Type, object>()
			{
				{ typeof(Texture2D), GetResources<Texture2D>() },
				{ typeof(SoundEffect), GetResources<SoundEffect>() },
				{ typeof(Music), GetResources<Music>() },
				{ typeof(DynamicSpriteFont), GetResources<DynamicSpriteFont>() },
				{ typeof(Effect), GetResources<Effect>() },
				{ typeof(ModItem), GetResources<ModItem>() },
				{ typeof(GlobalItem), GetResources<GlobalItem>() },
				{ typeof(EquipTexture), GetResources<EquipTexture>() },
				{ typeof(ModPrefix), GetResources<ModPrefix>() },
				{ typeof(ModDust), GetResources<ModDust>() },
				{ typeof(ModTile), GetResources<ModTile>() },
				{ typeof(GlobalTile), GetResources<GlobalTile>() },
				{ typeof(ModTileEntity), GetResources<ModTileEntity>() },
				{ typeof(ModWall), GetResources<ModWall>() },
				{ typeof(GlobalWall), GetResources<GlobalWall>() },
				{ typeof(ModProjectile), GetResources<ModProjectile>() },
				{ typeof(GlobalProjectile), GetResources<GlobalProjectile>() },
				{ typeof(ModNPC), GetResources<ModNPC>() },
				{ typeof(GlobalNPC), GetResources<GlobalNPC>() },
				{ typeof(ModPlayer), GetResources<ModPlayer>() },
				{ typeof(ModMountData), GetResources<ModMountData>() },
				{ typeof(ModBuff), GetResources<ModBuff>() },
				{ typeof(GlobalBuff), GetResources<GlobalBuff>() },
				{ typeof(ModWorld), GetResources<ModWorld>() },
				{ typeof(ModUgBgStyle), GetResources<ModUgBgStyle>() },
				{ typeof(ModSurfaceBgStyle), GetResources<ModSurfaceBgStyle>() },
				{ typeof(GlobalBgStyle), GetResources<GlobalBgStyle>() },
				{ typeof(ModWaterStyle), GetResources<ModWaterStyle>() },
				{ typeof(ModWaterfallStyle), GetResources<ModWaterfallStyle>() },
				{ typeof(ModRecipe), GetResources<ModRecipe>() },
				{ typeof(GlobalRecipe), GetResources<GlobalRecipe>() },
				{ typeof(ModTranslation), GetResources<ModTranslation>() },
				{ typeof(ModGore), GetResources<ModGore>() }
			};
		}
	}
}