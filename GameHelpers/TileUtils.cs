using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ObjectData;

namespace Libvaxy.GameHelpers
{
	public static class TileUtils
	{
		// i actually dont know where this code came from, but i just yoinked it anyways
		public static Point GetTileTopLeft(int i, int j)
		{
			if (ValidCoordinates(i, j))
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

		public static bool ValidCoordinates(int i, int j) => i > 0 && i < Main.maxTilesX && j > 0 && j < Main.maxTilesY;
	}
}
