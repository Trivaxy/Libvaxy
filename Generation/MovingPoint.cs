using Microsoft.Xna.Framework;

namespace Libvaxy.Generation
{
	public struct MovingPoint
	{
		public float X;
		public float Y;
		public float HorizontalSpeed;
		public float VerticalSpeed;

		public Point Position => new Point((int)X, (int)Y);

		public MovingPoint(float x, float y, float horizontalSpeed, float verticalSpeed)
		{
			X = x;
			Y = y;
			HorizontalSpeed = horizontalSpeed;
			VerticalSpeed = verticalSpeed;
		}

		public Point Move()
		{
			X += HorizontalSpeed;
			Y += VerticalSpeed;
			return new Point((int)X, (int)Y);
		}
	}
}