using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Libvaxy.WorldGen
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

		public static Point GetRandomPointInArea(int x, int y, int width, int height) => new Point(Terraria.WorldGen.genRand.Next(x, x + width), Terraria.WorldGen.genRand.Next(y, y + height));

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
	}

	public enum LiquidType
	{
		Water,
		Lava,
		Honey
	}
}