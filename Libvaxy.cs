using Libvaxy.Attributes;
using Libvaxy.GameHelpers;
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
		public static Dictionary<string, Assembly> ModAssemblies;
		public static List<DustEmitter> DustEmitters;

		internal static new ILog Logger => instance.Logger;

		internal void PreLoad() => instance = this;

		public override void Load()
		{
			Reflection.InitializeCaches();
			FallingTileTextures = new Dictionary<int, Texture2D>();
			fallingTileAlphaMask = GetTexture("GameHelpers/FallingTileAlphaMask");
			disposeList = new List<IDisposable>();
			DustEmitters = new List<DustEmitter>();
		}

		public void PostLoad()
		{
			TerrariaAssembly = typeof(Main).Assembly;
			ModAssemblies = ModLoader.Mods.Skip(1).ToDictionary(m => m.Name, m => m.Code);
			HookHandler.ApplyHooks();
			DetourHandler.ApplyDetours();
		}

		public override void Unload()
		{
			instance = null;

			Reflection.UnloadCaches();

			FallingTileTextures = null;
			fallingTileAlphaMask = null;

			foreach (IDisposable disposable in disposeList)
				disposable.Dispose();
			disposeList = null;

			TerrariaAssembly = null;
			ModAssemblies = null;

			DustEmitters.Clear();
			DustEmitters = null;
		}

		public override void MidUpdateDustTime()
		{
			// cOlLeCtIon ModIfIeD, eNuMeRaTIOn OpErAtIoN MAy not ExECuTe
			for (int i = DustEmitters.Count - 1; i >= 0; i--)
				DustEmitters[i].Update();
		}

		public override void PreSaveAndQuit()
		{
			DustEmitters.Clear();
		}

		public override void PostDrawInterface(SpriteBatch spriteBatch)
		{
			for (int i = DustEmitters.Count - 1; i >= 0; i--)
				DustEmitters[i].DebugDrawRect(spriteBatch);
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