using Libvaxy.GameHelpers.WorldGen;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ObjectData;

namespace Libvaxy.GameHelpers
{
	/// <summary>
	/// Provides methods that makes it easier to work with tiles.
	/// </summary>
	public static class TileUtils
	{
		// i actually dont know where this code came from, but i just yoinked it anyways
		/// <summary>
		/// Gets the top left of a multitile.
		/// </summary>
		/// <param name="i">The X coordinate of any multitile part.</param>
		/// <param name="j">The Y coordinate of any multitile part.</param>
		/// <returns>The top left coordinate of the multitile.</returns>
		public static Point GetTileTopLeft(int i, int j)
		{
			if (WorldGenUtils.SafeCoordinates(i, j))
			{
				Tile tile = Main.tile[i, j];

				int fX = 0;
				int fY = 0;

				if (tile != null)
				{
					TileObjectData data = TileObjectData.GetTileData(tile.type, 0);

					if (data != null)
					{
						fX = tile.frameX % (18 * data.Width) / 18;
						fY = tile.frameY % (18 * data.Height) / 18;
					}
				}

				return new Point(i - fX, j - fY);
			}

			return new Point(-1, -1);
		}
	}
}
