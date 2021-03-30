using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.WorldGen;

namespace Libvaxy.Extensions
{
	public static class PointExtensions
	{
		public static Point GetRandomPointWithinRadius(this Point point, float radius)
			=> (_genRand.NextVector2Unit() * _genRand.NextFloat(radius)).ToPoint();

		public static Point GetRandomPointWithinRing(this Point point, float minRadius, float maxRadius)
			=> (_genRand.NextVector2Unit() * _genRand.NextFloat(minRadius, maxRadius)).ToPoint();

		public static Point GetRandomPointOnCircleAround(this Point point, float radius)
			=> (_genRand.NextVector2Unit() * radius).ToPoint();
	}
}
