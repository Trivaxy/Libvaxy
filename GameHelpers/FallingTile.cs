using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Libvaxy.GameHelpers
{
	/// <summary>
	/// Represents any falling tile in projectile form. This is internally used by Libvaxy. You should use LibvaxyMod.SpawnFallingTile to create falling tiles.
	/// </summary>
	public class FallingTile : ModProjectile
	{
		public override void SetStaticDefaults() =>	DisplayName.SetDefault("Falling Tile");

		public override void SetDefaults()
		{
			projectile.width = 10;
			projectile.height = 10;
			projectile.aiStyle = -1;
			projectile.friendly = true;
			projectile.hostile = true;
			projectile.tileCollide = true;
			projectile.penetrate = -1;
		}

		public override void AI()
		{
			projectile.rotation += 0.1f; // rotate projectile
			projectile.velocity.Y += 0.41f; // accelerate projectile downwards
			projectile.velocity.Y = MathHelper.Clamp(projectile.velocity.Y, 0f, 10f); // max out projectile speed at 10f or 37.5 tiles per second
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			int tileType = (int)projectile.ai[0];

			if (!LibvaxyMod.FallingTileTextures.ContainsKey(tileType))
				LibvaxyMod.FallingTileTextures[tileType] = LibvaxyMod.CreateFallingTileTexture(tileType);

			spriteBatch.Draw(LibvaxyMod.FallingTileTextures[tileType], projectile.position - Main.screenPosition, null, lightColor, projectile.rotation, new Vector2(projectile.width / 2, projectile.height / 2), 1f, SpriteEffects.None, 0f);
			return false;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Terraria.WorldGen.PlaceTile((int)projectile.Center.X / 16, (int)(projectile.Center.Y / 16), (int)projectile.ai[0]);
			return true;
		}
	}
}