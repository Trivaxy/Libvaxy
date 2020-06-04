using Libvaxy.Attributes;
using Libvaxy.ContentHelpers;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace Libvaxy
{
	public class Libvaxy : Mod
	{
		public Libvaxy() => PreLoad();

		internal static Mod instance;
		internal static Texture2D fallingTileAlphaMask;
		internal static readonly Rectangle fallingTileFrame = new Rectangle(180, 0, 14, 14);
		public static Dictionary<int, Texture2D> FallingTileTextures;
		private static List<IDisposable> disposeList;
		public static Assembly TerrariaAssembly;
		public static Assembly[] ModAssemblies;

		internal static new ILog Logger => instance.Logger;


		internal void PreLoad()
		{
			instance = this;
		}

		public override void Load()
		{
			Reflection.InitializeCaches();
			FallingTileTextures = new Dictionary<int, Texture2D>();
			fallingTileAlphaMask = GetTexture("ContentHelpers/FallingTileAlphaMask");
			disposeList = new List<IDisposable>();
		}

		public void PostLoad()
		{
			TerrariaAssembly = typeof(Main).Assembly;
			ModAssemblies = ModLoader.Mods.Select(mod => mod.Code).Skip(1).ToArray(); // index 0 is always null for some reason
			HookHandler.ApplyHooks();
		}

		public override void Unload()
		{
			Reflection.UnloadCaches();
			FallingTileTextures = null;
			fallingTileAlphaMask = null;

			foreach (IDisposable disposable in disposeList)
				disposable.Dispose();
			disposeList = null;

			TerrariaAssembly = null;
			ModAssemblies = null;
		}

		internal static void DisposeOnUnload(IDisposable disposable)
			=> disposeList.Add(disposable);

		// given the id of a tile, this will create an automatic falling tile texture for it
		internal static Texture2D CreateFallingTileTexture(int tileType)
		{
			Main.instance.LoadTiles(tileType); // load the tile texture if it hasn't been yet
			Texture2D newTexture = Main.tileTexture[tileType].CloneRectangle(fallingTileFrame); // rip out a specific frame in the tilesheet
			newTexture.MaskAlpha(fallingTileAlphaMask); // lay over it the falling tile alpha mask, this will circularize the edges
			newTexture.MaskTexture(Main.projectileTexture[ModContent.ProjectileType<FallingTile>()]); // we lay over a dark border on the texture
			return newTexture;
		}

		public static int SpawnFallingTile(int x, int y, int tileType, int damage, float knockback = 6f)
			=> Projectile.NewProjectile(x, y, 0f, 0f, ModContent.ProjectileType<FallingTile>(), damage, knockback, ai0: tileType);

		public static int SpawnFallingTile(Point position, int tileType, int damage, float knockback = 6f)
			=> SpawnFallingTile(position.X, position.Y, tileType, damage, knockback);

		public override void PostAddRecipes() => PostLoad();
	}
}