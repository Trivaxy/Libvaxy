using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.WorldGen;

namespace Libvaxy.Extensions
{
	public static class PointExtensions
	{
		/// <summary>
		/// Gets a random point within a radius around an origin.
		/// </summary>
		/// <param name="point">The origin.</param>
		/// <param name="radius">The maximum radius away from the origin.</param>
		public static Point GetRandomPointWithinRadius(this Point point, float radius)
			=> (_genRand.NextVector2Unit() * _genRand.NextFloat(radius)).ToPoint();

		/// <summary>
		/// Gets a random point that lies inside a ring around an origin.
		/// </summary>
		/// <param name="point">The origin.</param>
		/// <param name="minRadius">The inner radius of the ring away from the point.</param>
		/// <param name="maxRadius">The outer radius of the ring away from the point.</param>
		public static Point GetRandomPointWithinRing(this Point point, float minRadius, float maxRadius)
			=> (_genRand.NextVector2Unit() * _genRand.NextFloat(minRadius, maxRadius)).ToPoint();

		/// <summary>
		/// Gets a random point that lies on the circumference of a circle, around an origin.
		/// </summary>
		/// <param name="point">The origin.</param>
		/// <param name="radius">The radius of the circle.</param>
		public static Point GetRandomPointOnCircumference(this Point point, float radius)
			=> (_genRand.NextVector2Unit() * radius).ToPoint();
	}
}
