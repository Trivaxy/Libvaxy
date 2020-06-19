namespace Libvaxy.WorldGen
{
	public struct MovingPoint
	{
		public int X;
		public int Y;
		public int HorizontalSpeed;
		public int VerticalSpeed;

		public MovingPoint(int x, int y, int horizontalSpeed, int verticalSpeed)
		{
			X = x;
			Y = y;
			HorizontalSpeed = horizontalSpeed;
			VerticalSpeed = verticalSpeed;
		}

		public void Move()
		{
			X += HorizontalSpeed;
			Y += VerticalSpeed;
		}
	}
}