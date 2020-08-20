using Microsoft.Xna.Framework;
using System;
using Terraria;
using static Terraria.WorldGen;

namespace Libvaxy.GameHelpers.WorldGen
{
	public static class WorldGenUtils
	{
		public const int SmallWorldWidth = 4200;
		public const int SmallWorldHeight = 1200;
		public const int MediumWorldWidth = (int)(SmallWorldWidth * 1.5f);
		public const int MediumWorldHeight = (int)(SmallWorldHeight * 1.5f);
		public const int LargeWorldWidth = SmallWorldWidth * 2;
		public const int LargeWorldHeight = SmallWorldHeight * 2;

		public static void Replace(int x, int y, int width, int height, ushort type, ushort newType)
		{
			for (int i = x; i < x + width; i++)
			{
				for (int j = y; j < y + height; j++)
				{
					if (i < Main.maxTilesX && j < Main.maxTilesY && Main.tile[i, j].type == type)
					{
						Main.tile[i, j].active(true);
						Main.tile[i, j].type = newType;
					}
				}
			}
		}

		public static void Fill(int x, int y, int width, int height, ushort type)
		{
			for (int i = x; i < x + width; i++)
			{
				for (int j = y; j < y + height; j++)
				{
					Main.tile[i, j].active(true);
					Main.tile[i, j].type = type;
				}
			}
		}

		public static void FillAir(int x, int y, int width, int height)
		{
			for (int i = x; i < x + width; i++)
				for (int j = y; j < y + height; j++)
					Main.tile[i, j].active(false);
		}

		public static void FillLiquid(int x, int y, int width, int height, LiquidType liquidType)
		{
			for (int i = x; i < x + width; i++)
			{
				for (int j = y; j < y + height; j++)
				{
					Main.tile[i, j].liquidType((int)liquidType);
					Main.tile[i, j].liquid = 255;
				}
			}
					
		}

		public static int[] GetPerlinDisplacements(int displacementCount, float frequency, int maxLimit, float multiplier, int seed)
		{
			FastNoise noise = new FastNoise(seed);
			noise.SetNoiseType(FastNoise.NoiseType.Perlin);
			noise.SetFrequency(frequency);

			int[] displacements = new int[displacementCount];

			for (int x = 0; x < displacementCount; x++)
				displacements[x] = (int)Math.Floor(noise.GetNoise(x, x) * maxLimit * multiplier);

			return displacements;
		}

		public static Point GetRandomPointInArea(int x, int y, int width, int height) => new Point(genRand.Next(x, x + width), genRand.Next(y, y + height));

		public static Point[] GetRandomPointsInArea(int x, int y, int width, int height, int points)
		{
			Point[] randPoints = new Point[points];

			for (int i = 0; i < points; i++)
				randPoints[i] = GetRandomPointInArea(x, y, width, height);

			return randPoints;
		}

		public static Point[] GetRandomTilePointsInArea(int x, int y, int width, int height, int points, ushort type)
		{
			Point[] randPoints = GetRandomPointsInArea(x, y, width, height, points);

			for (int i = 0; i < points; i++)
				while (Main.tile[randPoints[i].X, randPoints[i].Y].type != type)
					randPoints[i] = GetRandomPointInArea(x, y, width, height);

			return randPoints;
		}

		public static Point[] GetRandomAirPointsInArea(int x, int y, int width, int height, int points)
		{
			Point[] randPoints = GetRandomPointsInArea(x, y, width, height, points);

			for (int i = 0; i < points; i++)
				while (Main.tile[randPoints[i].X, randPoints[i].Y].active())
					randPoints[i] = GetRandomPointInArea(x, y, width, height);

			return randPoints;
		}

		public static Point FindLowestTile(int x, int y)
		{
			while (y < Main.maxTilesY && y > -1 && x < Main.maxTilesX && x > -1)
			{
				if (Main.tile[x, y].active())
					return new Point(x, y);
				y++;
			}
			return Point.Zero;
		}

		public static bool SafeCoordinates(int i, int j) => i > 0 && i < Main.maxTilesX && j > 0 && j < Main.maxTilesY;

		/// <summary>
		/// An alternative version of WorldGen.TileRunner. It uses noise to fill out a shape for you, which typically looks much more smooth than TileRunner.
		/// </summary>
		/// <param name="i">The X tile coordinate.</param>
		/// <param name="j">The Y tile coordinate.</param>
		/// <param name="type">The type of tile to fill. If this is -1, NoiseRunner will fill air instead.</param>
		/// <param name="radius">The base radius to try and fill.</param>
		/// <param name="frequency">How erratic the resulting fill will be. A higher frequency means more intense deformation.
		/// It is recommended to keep the frequency anywhere between 0.005 - 0.6, but you can use any you wish.</param>
		public static void NoiseRunner(int i, int j, int type, float radius, float frequency)
		{
			int[] circleDisplacements = GetPerlinDisplacements((int)(2 * Math.PI * radius * 1.5f), frequency, (int)radius, 1f, Main.rand.Next(int.MaxValue));
			float angle = (float)(2 * Math.PI / circleDisplacements.Length);

			for (int x = 0; x < circleDisplacements.Length; x++)
			{
				MovingPoint point = new MovingPoint(i, j, (float)Math.Sin(angle * x), (float)Math.Cos(angle * x));

				float distance = radius + circleDisplacements[x];

				for (int z = 0; z < distance; z++)
				{
					if (!SafeCoordinates(point.Position.X, point.Position.Y))
						break;

					Tile tile = Main.tile[point.Position.X, point.Position.Y];

					if (type == -1)
					{
						tile.active(false);
						tile.type = 0;
					}
					else
					{
						tile.active(true);
						tile.type = (ushort)type;
					}

					point.Move();
				}	
			}
		}
	}

	public enum LiquidType
	{
		Water,
		Lava,
		Honey
	}
}