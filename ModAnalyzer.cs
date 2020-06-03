using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader;
using Terraria.ModLoader.Audio;

namespace Libvaxy
{
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
		/// Returns all resources inside a mod as objects.
		/// </summary>
		/// <returns>A 2-dimensional array containing all resources. Each array contains one type of resource.</returns>
		public object[][] GetAllResources()
		{
			// oh god
			object[][] resources = new object[][]
			{
				GetResources<Texture2D>(),
				GetResources<SoundEffect>(),
				GetResources<Music>(),
				GetResources<DynamicSpriteFont>(),
				GetResources<Effect>(),
				GetResources<ModItem>(),
				GetResources<GlobalItem>(),
				GetResources<EquipTexture>(),
				GetResources<ModPrefix>(),
				GetResources<ModDust>(),
				GetResources<ModTile>(),
				GetResources<GlobalTile>(),
				GetResources<ModTileEntity>(),
				GetResources<ModWall>(),
				GetResources<GlobalWall>(),
				GetResources<ModProjectile>(),
				GetResources<GlobalProjectile>(),
				GetResources<ModNPC>(),
				GetResources<GlobalNPC>(),
				GetResources<ModPlayer>(),
				GetResources<ModMountData>(),
				GetResources<ModBuff>(),
				GetResources<GlobalBuff>(),
				GetResources<ModWorld>(),
				GetResources<ModUgBgStyle>(),
				GetResources<ModSurfaceBgStyle>(),
				GetResources<GlobalBgStyle>(),
				GetResources<ModWaterStyle>(),
				GetResources<ModWaterfallStyle>(),
				GetResources<ModRecipe>(),
				GetResources<GlobalRecipe>(),
				GetResources<ModTranslation>(),
				GetResources<ModGore>()
			};

			return resources;
		}
	}
}