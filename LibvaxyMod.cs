using Libvaxy.Attributes;
using Libvaxy.Debug;
using Libvaxy.Extensions;
using Libvaxy.GameHelpers;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Libvaxy
{
	// i want the class name to be Libvaxy so bad, but the namespace already reserved it :(
	/// <summary>
	/// The main class of Libvaxy that makes everything work. Contains certain useful methods regarding FallingTile and StackInspect.
	/// </summary>
	public class LibvaxyMod : Mod
	{
		public LibvaxyMod() => PreLoad();

		internal static Mod instance;
		internal static Texture2D fallingTileAlphaMask;
		internal static readonly Rectangle fallingTileFrame = new Rectangle(180, 0, 14, 14);
		public static Dictionary<int, Texture2D> FallingTileTextures;
		private static List<IDisposable> disposeList;
		public static Assembly TerrariaAssembly;
		public static Dictionary<string, Assembly> ModAssemblies;

        internal new static ILog Logger => instance.Logger;

		internal void PreLoad() => instance = this;

		public override void Load()
        {
            Reflection.InitializeCaches();
			MonoModHooks.RequestNativeAccess();

			FallingTileTextures = new Dictionary<int, Texture2D>();
			fallingTileAlphaMask = GetTexture("GameHelpers/FallingTileAlphaMask");

			disposeList = new List<IDisposable>();

			TerrariaAssembly = typeof(Main).Assembly;
			ModAssemblies = ModLoader.Mods.Skip(1).ToDictionary(m => m.Name, m => m.Code); // initialize on load so libvaxy-dependent mods function when using this

			StackInspectHandler.Initialize();
			HookHandler.ApplyHooks();
        }

		public void PostLoad()
		{
            ModAssemblies = ModLoader.Mods.Skip(1).ToDictionary(m => m.Name, m => m.Code); // add the rest of the loaded mods after Load()
			FieldGetHandler.ApplyFieldGets();
			DetourHandler.ApplyDetours();
		}

		public override void Unload()
		{
			instance = null;

			Reflection.UnloadCaches();
            FallingTileTextures = null;
			fallingTileAlphaMask = null;

			if (disposeList != null)
			{
				foreach (IDisposable disposable in disposeList)
					disposable.Dispose();
				disposeList = null;
			}

			TerrariaAssembly = null;
			ModAssemblies = null;

			StackInspectHandler.Unload();
        }

		internal static void DisposeOnUnload(IDisposable disposable)
			=> disposeList.Add(disposable);

		// given the id of a tile, this will create an automatic falling tile texture for it
		internal static Texture2D CreateFallingTileTexture(int tileType)
		{
			Main.instance.LoadTiles(tileType); // load the tile texture if it hasn't been yet
			Texture2D newTexture = Main.tileTexture[tileType].CloneRectangle(fallingTileFrame); // rip out a specific frame in the tilesheet
			newTexture.MaskAlpha(fallingTileAlphaMask); // lay over it the falling tile alpha mask, this will circularize the edges
			newTexture.MergeTexture(Main.projectileTexture[ModContent.ProjectileType<FallingTile>()]); // we lay over a dark border on the texture
			return newTexture;
		}

		public static int SpawnFallingTile(int x, int y, int tileType, int damage, float knockback = 6f)
			=> Projectile.NewProjectile(x, y, 0f, 0f, ModContent.ProjectileType<FallingTile>(), damage, knockback, ai0: tileType);

		public static int SpawnFallingTile(Point position, int tileType, int damage, float knockback = 6f)
			=> SpawnFallingTile(position.X, position.Y, tileType, damage, knockback);

		public static void InspectStack(StackInspectTarget target)
			=> StackInspectHandler.ApplyStackInspection(target);

		public static void SendMessage(string message, Color color, int excludedPlayer = -1)
		{
			if (Main.dedServ)
				NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(message), color, excludedPlayer);
			else
				Main.NewText(message, color);
		}

		public static double Benchmark(Action action, int iterations)
		{
			GC.Collect();
			action.Invoke(); // run once outside of loop to avoid initialization costs

			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < iterations; i++)
				action.Invoke();

			stopwatch.Stop();

			return stopwatch.ElapsedTicks;
		}

		public override void PostAddRecipes() => PostLoad();
	}

	public static class FieldGetters
	{
		[FieldGet("adjWater")]
		public static bool IsAdjacentToWater(this Player player) => false;

		[FieldGet("defaultItemGrabRange")]
		public static int GetDefaultItemGrabRange(this Player player) => 0;
	}
}