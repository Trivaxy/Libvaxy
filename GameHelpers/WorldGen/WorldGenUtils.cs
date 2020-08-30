using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using static Terraria.WorldGen;

namespace Libvaxy.GameHelpers.WorldGen
{
	/// <summary>
	/// Provides many utilities to make worldgen-related things more viable (some methods can be used outside worldgen as well).
	/// </summary>
	public static class WorldGenUtils
	{
		public const int SmallWorldWidth = 4200;
		public const int SmallWorldHeight = 1200;
		public const int MediumWorldWidth = (int)(SmallWorldWidth * 1.5f);
		public const int MediumWorldHeight = (int)(SmallWorldHeight * 1.5f);
		public const int LargeWorldWidth = SmallWorldWidth * 2;
		public const int LargeWorldHeight = SmallWorldHeight * 2;

		/// <summary>
		/// Replaces all tiles of a certain type in a rectangle to another type.
		/// </summary>
		/// <param name="x">The X coordinate of the rectangle.</param>
		/// <param name="y">The Y coordinate of the rectangle.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		/// <param name="type">The type of tile to replace.</param>
		/// <param name="newType">The type to set replaced tiles to.</param>
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

		/// <summary>
		/// Sets all tiles in a rectangle to a specified type.
		/// </summary>
		/// <param name="x">The X coordinate of the rectangle.</param>
		/// <param name="y">The Y coordinate of the rectangle.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		/// <param name="type">The type to set the tiles to within the rectangle.</param>
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

		/// <summary>
		/// Sets all tiles within a rectangle to air.
		/// </summary>
		/// <param name="x">The X coordinate of the rectangle.</param>
		/// <param name="y">The Y coordinate of the rectangle.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		public static void FillAir(int x, int y, int width, int height)
		{
			for (int i = x; i < x + width; i++)
				for (int j = y; j < y + height; j++)
					Main.tile[i, j].active(false);
		}

		/// <summary>
		/// Sets all tiles within a rectangle to a specified vanilla liquid.
		/// </summary>
		/// <param name="x">The X coordinate of the rectangle.</param>
		/// <param name="y">The Y coordinate of the rectangle.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		/// <param name="liquidType">The type of liquid to fill.</param>
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

		/// <summary>
		/// Gets you an array of displacements generated through perlin noise.
		/// </summary>
		/// <param name="displacementCount">How many displacements to generate.</param>
		/// <param name="frequency">The frequency of the displacements.</param>
		/// <param name="maxLimit">The maximum limit the displacements can be, both positive and negative. Can be any number.</param>
		/// <param name="multiplier">The multiplier to affect the maxLimit by.</param>
		/// <param name="seed">The seed to use for the perlin generator.</param>
		/// <returns>An array of perlin displacements.</returns>
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

		/// <summary>
		/// Gets a random point within a rectangle.
		/// </summary>
		/// <param name="x">The X coordinate of the rectangle.</param>
		/// <param name="y">The Y coordinate of the rectangle.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		/// <returns>A random point within the rectangle.</returns>
		public static Point GetRandomPointInArea(int x, int y, int width, int height) => new Point(genRand.Next(x, x + width), genRand.Next(y, y + height));

		/// <summary>
		/// Gets multiple random points within a rectangle.
		/// </summary>
		/// <param name="x">The X coordinate of the rectangle.</param>
		/// <param name="y">The Y coordinate of the rectangle.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		/// <param name="points">The number of random points to get.</param>
		/// <returns>Random points within the rectangle.</returns>
		public static Point[] GetRandomPointsInArea(int x, int y, int width, int height, int points)
		{
			Point[] randPoints = new Point[points];

			for (int i = 0; i < points; i++)
				randPoints[i] = GetRandomPointInArea(x, y, width, height);

			return randPoints;
		}

		/// <summary>
		/// Gets random points within a rectangle that have a specific tile type.
		/// </summary>
		/// <param name="x">The X coordinate of the rectangle.</param>
		/// <param name="y">The Y coordinate of the rectangle.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		/// <param name="points">The number of random points to get.</param>
		/// <param name="type">The tile type to search for.</param>
		/// <returns>Random points within the rectangle that have the specified type.</returns>
		public static Point[] GetRandomTilePointsInArea(int x, int y, int width, int height, int points, ushort type)
		{
			Point[] randPoints = GetRandomPointsInArea(x, y, width, height, points);

			for (int i = 0; i < points; i++)
				while (Main.tile[randPoints[i].X, randPoints[i].Y].type != type)
					randPoints[i] = GetRandomPointInArea(x, y, width, height);

			return randPoints;
		}

		/// <summary>
		/// Gets random points within a rectangle that are all air.
		/// </summary>
		/// <param name="x">The X coordinate of the rectangle.</param>
		/// <param name="y">The Y coordinate of the rectangle.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		/// <param name="points">The number of random points to get.</param>
		/// <returns></returns>
		public static Point[] GetRandomAirPointsInArea(int x, int y, int width, int height, int points)
		{
			Point[] randPoints = GetRandomPointsInArea(x, y, width, height, points);

			for (int i = 0; i < points; i++)
				while (Main.tile[randPoints[i].X, randPoints[i].Y].active())
					randPoints[i] = GetRandomPointInArea(x, y, width, height);

			return randPoints;
		}

		/// <summary>
		/// Keeps searching downwards until a non-air tile is found.
		/// </summary>
		/// <param name="x">The X coordinate to start searching from.</param>
		/// <param name="y">The Y coordinate to start searching from.</param>
		/// <returns>The first non-air tile found after searching downwards.</returns>
		public static Point FindDownwardsTile(int x, int y)
		{
			while (y < Main.maxTilesY && y > -1 && x < Main.maxTilesX && x > -1)
			{
				if (Main.tile[x, y].active())
					return new Point(x, y);
				y++;
			}
			return new Point(x, Main.maxTilesY - 1);
		}

		/// <summary>
		/// Checks if the specified coordinates are within the world boundaries.
		/// </summary>
		/// <param name="i">The X coordinate of the tile.</param>
		/// <param name="j">The Y coordinate of the tile.</param>
		/// <returns>Whether the coordinates are within the world boundaries or not.</returns>
		public static bool SafeCoordinates(int i, int j) => i >= 0 && i < Main.maxTilesX && j >= 0 && j < Main.maxTilesY;

		/// <summary>
		/// An alternative version of WorldGen.TileRunner. It uses noise to fill out a shape for you, which typically looks much more smooth than TileRunner.
		/// </summary>
		/// <param name="i">The X tile coordinate.</param>
		/// <param name="j">The Y tile coordinate.</param>
		/// <param name="type">The type of tile to fill. If this is -1, NoiseRunner will fill air instead.</param>
		/// <param name="radius">The base radius to try and fill.</param>
		/// <param name="frequency">How erratic the resulting fill will be. A higher frequency means more intense deformation.
		/// It is recommended to keep the frequency anywhere between 0.005 - 0.6, but you can use any you wish.
		/// For good results, remember that smaller radii will require higher frequencies in order to deform better.</param>
		/// <param name="ignoredTiles">A list of tile IDs the NoiseRunner should not affect.</param>
		public static void NoiseRunner(int i, int j, int type, float radius, float frequency, params int[] ignoredTiles)
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

					for (int g = 0; g < ignoredTiles.Length; g++)
						if (tile.type == ignoredTiles[g])
							goto BadTile; // sometimes, goto is less evil than the alternatives

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

					BadTile:
					point.Move();
				}
			}
		}
		/// <summary>
		/// Checks whether a point is in the specified oval.
		/// </summary>
		/// <param name="midX">X coordinate of the middle of the specified oval.</param>
		/// <param name="midY">Y coordinate of the middle of the specified oval.</param>
		/// <param name="x">X coordinate of the point you want to check</param>
		/// <param name="y">Y coordinate of the point you want to check</param>
		/// <param name="sizeX">How wide the oval will be</param>
		/// <param name="sizeY">How tall the oval will be</param>
		public static bool OvalCheck(int midX, int midY, int x, int y, int sizeX, int sizeY)
		{
			double p = Math.Pow(x - midX, 2) / Math.Pow(sizeX, 2)
					+ Math.Pow(y - midY, 2) / Math.Pow(sizeY, 2);

			return p < 1 ? true : false;
		}
		/// <summary>
		/// Fills oval with a specified tile type.
		/// </summary>
		/// <param name="width">Width of Oval</param>
		/// <param name="height">Height of Oval</param>
		/// <param name="startingPoint">Starting point of Oval</param>
		/// <param name="type">Specified tile you want the oval to be filled with</param>
		/// <param name="forced">Wether or not this will override existing blocks</param>
		public static void MakeOval(int width, int height, Vector2 startingPoint, int type, bool forced)
		{
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (OvalCheck((int)(startingPoint.X + width / 2), (int)(startingPoint.Y + height / 2), i + (int)startingPoint.X, j + (int)startingPoint.Y, (int)(width * .5f), (int)(height * .5f)))
					{
						Tile tile = Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y);
						if(!(forced && tile.active()))
						tile.type = (ushort)type;
					}
				}
			}
		}
		/// <summary>
		/// Fills circle.
		/// </summary>
		/// <param name="size">Radius of the circle</param>
		/// <param name="startingPoint">Position of circle</param>
		/// <param name="type">Specified tile you want the circle to be filled with</param>
		/// <param name="forced">Wether or not this will override existing blocks</param>
		public static void MakeCircle(int size, Vector2 startingPoint, int type, bool forced)
		{
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					float f = size * 0.5f;
					if (Vector2.DistanceSquared(new Vector2(i + (int)startingPoint.X, j + (int)startingPoint.Y), startingPoint + new Vector2(size * 0.5f, size * 0.5f)) < f * f)
					{
						Tile tile = Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y);
						if (!(forced && tile.active()))
							tile.type = (ushort)type;
					}
				}
			}
		}
	}

	/// <summary>
	/// Represents a vanilla liquid.
	/// </summary>
	public enum LiquidType
	{
		Water,
		Lava,
		Honey
	}
}