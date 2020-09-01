using Libvaxy.Attributes;
using Libvaxy.Debug;
using Libvaxy.Debug.REPL;
using Libvaxy.Extensions;
using Libvaxy.GameHelpers;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

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
		private static UserInterface REPLInterface;
		private static ReplUI REPLUI;

		internal static new ILog Logger => instance.Logger;

		internal void PreLoad() => instance = this;

		public override void Load()
		{
			Reflection.InitializeCaches();
			FallingTileTextures = new Dictionary<int, Texture2D>();
			fallingTileAlphaMask = GetTexture("GameHelpers/FallingTileAlphaMask");
			disposeList = new List<IDisposable>();
			TerrariaAssembly = typeof(Main).Assembly;
			ModAssemblies = ModLoader.Mods.Skip(1).ToDictionary(m => m.Name, m => m.Code); // initialize on load so libvaxy-dependent mods function when using this
			StackInspectHandler.Initialize();
			HookHandler.ApplyHooks();

			if (!Main.dedServ)
			{
				REPLInterface = new UserInterface();
				REPLUI = new ReplUI();
				REPLUI.Activate();
				ShowREPL();
			}
		}

		public void PostLoad()
		{
			ModAssemblies = ModLoader.Mods.Skip(1).ToDictionary(m => m.Name, m => m.Code); // add the rest of the loaded mods after Load()
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

			StackInspectHandler.Unload();

			REPLInterface = null;
			REPLUI = null;
		}

		private GameTime lastGameTime;

		public override void UpdateUI(GameTime gameTime)
		{
			lastGameTime = gameTime;

			if (REPLInterface?.CurrentState != null)
				REPLInterface.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseTextIndex != -1)
			{
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
					"LibvaxyMod: REPL UI",
					delegate
					{
						if (lastGameTime != null && REPLInterface?.CurrentState != null)
						{
							REPLInterface.Draw(Main.spriteBatch, lastGameTime);
						}
						return true;
					},
					   InterfaceScaleType.UI));
			}
		}

		public static void ShowREPL() => REPLInterface?.SetState(REPLUI);

		public static void HideREPL() => REPLInterface?.SetState(null);

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

		public override void PostAddRecipes() => PostLoad();
	}
}