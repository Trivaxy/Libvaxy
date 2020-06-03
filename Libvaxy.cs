using Libvaxy.Attributes;
using Libvaxy.ContentHelpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Pdb;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace Libvaxy
{
	public class Libvaxy : Mod
	{
		public Libvaxy() => PreLoad();

		internal static Texture2D fallingTileAlphaMask;
		internal static readonly Rectangle fallingTileFrame = new Rectangle(180, 0, 14, 14);
		public static Dictionary<int, Texture2D> FallingTileTextures;
		private static List<IDisposable> disposeList;

		internal void PreLoad()
		{
			disposeList = new List<IDisposable>(); // initialize dispose list
		}

		public override void Load()
		{
			Reflection.InitializeCaches(); // initialize all reflection caches
			FallingTileTextures = new Dictionary<int, Texture2D>(); // initialize the dictionary
			fallingTileAlphaMask = GetTexture("ContentHelpers/FallingTileAlphaMask"); // load falling tile alpha mask
			disposeList = new List<IDisposable>(); // initialize the disposeList
		}

		public override void Unload()
		{
			Reflection.UnloadCaches();
			FallingTileTextures = null;
			fallingTileAlphaMask = null;

			foreach (IDisposable disposable in disposeList)
				disposable.Dispose();
			disposeList = null;
		}

		internal static void DisposeOnUnload(IDisposable disposable)
			=> disposeList.Add(disposable);

		// given the id of a tile, this will create an automatic falling tile texture for it
		internal static Texture2D CreateFallingTileTexture(int tileType)
		{
			Terraria.Projectile.NewProjectile()
			Main.instance.LoadTiles(tileType); // load the tile texture if it hasn't been yet
			Texture2D newTexture = Main.tileTexture[tileType].CloneRectangle(fallingTileFrame); // rip out a specific frame in the tilesheet
			newTexture.MaskAlpha(fallingTileAlphaMask); // lay over it the falling tile alpha mask, this will circularize the edges
			newTexture.MaskTexture(Main.projectileTexture[ModContent.ProjectileType<FallingTile>()]); // we lay over a dark border on the texture
			return newTexture;
		}

		[Detour("Terraria.Projectile.NewProjectile[Vector2, Vector2, Int32, Int32, Single, Int32, Single, Single]")]
		public static int SpawnFallingTile(int x, int y, int tileType, int damage, float knockback = 6f)
			=> Projectile.NewProjectile(x, y, 0f, 0f, ModContent.ProjectileType<FallingTile>(), damage, knockback, ai0: tileType);

		public static int SpawnFallingTile(Point position, int tileType, int damage, float knockback = 6f)
			=> SpawnFallingTile(position.X, position.Y, tileType, damage, knockback);
	}
}