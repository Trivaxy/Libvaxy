using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using static Terraria.WorldGen;

namespace Libvaxy.WorldGen
{
	public class GenChain
	{
		private List<Point> points;

		public GenChain(Point startingPoint)
		{
			points = new List<Point>();
			points.Add(startingPoint);
		}

		private int RandomPointIndex => _genRand.Next(points.Count);

		public GenChain TakeRandomPoint(Action<Point> action)
		{
			int randIndex = RandomPointIndex;
			action.Invoke(points[randIndex]);
			points.RemoveAt(randIndex);

			return this;
		}

		public GenChain PickRandomPoint(Action<Point> action)
		{
			int randIndex = RandomPointIndex;
			action.Invoke(points[randIndex]);

			return this;
		}

		public GenChain TakeRandomPoints(int count, Action<IEnumerable<Point>> action)
		{
			action.Invoke(GetAndRemoveRandomPoints(count));

			return this;
		}

		public GenChain PickRandomPoints(int count, Action<IEnumerable<Point>> action)
		{
			action.Invoke(GetRandomPoints(count));

			return this;
		}

		public GenChain PickUniqueRandomPoints(int count, Action<IEnumerable<Point>> action)
		{
			action.Invoke(GetUniqueRandomPoints(count));

			return this;
		}

		public GenChain PickRandomPointAndReturnPoint(Func<Point, Point> func)
		{
			Point result = func.Invoke(points[RandomPointIndex]);
			points.Add(result);

			return this;
		}

		public GenChain TakeRandomPointAndReturnPoint(Func<Point, Point> func)
		{
			int randIndex = RandomPointIndex;
			Point result = func.Invoke(points[randIndex]);
			points.RemoveAt(randIndex);
			points.Add(result);

			return this;
		}

		public GenChain PickRandomPointsAndReturnPoint(int count, Func<IEnumerable<Point>, Point> func)
		{
			Point result = func.Invoke(GetRandomPoints(count));
			points.Add(result);

			return this;
		}

		public GenChain TakeRandomPointsAndReturnPoint(int count, Func<IEnumerable<Point>, Point> func)
		{
			Point result = func.Invoke(GetAndRemoveRandomPoints(count));
			points.Add(result);

			return this;
		}

		public GenChain PickUniqueRandomPointsAndReturnPoint(int count, Func<IEnumerable<Point>, Point> func)
		{
			Point result = func.Invoke(GetUniqueRandomPoints(count));
			points.Add(result);

			return this;
		}

		public GenChain PickRandomPointAndReturnPoints(Func<Point, IEnumerable<Point>> func)
		{
			IEnumerable<Point> result = func.Invoke(points[RandomPointIndex]);
			foreach (Point point in result)
				points.Add(point);

			return this;
		}

		public GenChain TakeRandomPointAndReturnPoints(Func<Point, IEnumerable<Point>> func)
		{
			int randIndex = RandomPointIndex;
			IEnumerable<Point> result = func.Invoke(points[randIndex]);
			points.RemoveAt(randIndex);

			foreach (Point point in result)
				points.Add(point);

			return this;
		}

		public GenChain PickRandomPointsAndReturnPoints(int count, Func<IEnumerable<Point>, IEnumerable<Point>> func)
		{
			IEnumerable<Point> result = func.Invoke(GetRandomPoints(count));
			foreach (Point point in result)
				points.Add(point);

			return this;
		}

		public GenChain TakeRandomPointsAndReturnPoints(int count, Func<IEnumerable<Point>, IEnumerable<Point>> func)
		{
			IEnumerable<Point> result = func.Invoke(GetAndRemoveRandomPoints(count));
			foreach (Point point in result)
				points.Add(point);

			return this;
		}

		public GenChain PickUniqueRandomPointsAndReturnPoints(int count, Func<IEnumerable<Point>, IEnumerable<Point>> func)
		{
			IEnumerable<Point> result = func.Invoke(GetUniqueRandomPoints(count));
			foreach (Point point in result)
				points.Add(point);

			return this;
		}

		public GenChain PickAllPoints(Action<IEnumerable<Point>> action)
		{
			action.Invoke(points);
			return this;
		}

		public GenChain TakeAllPoints(Action<IEnumerable<Point>> action)
		{
			action.Invoke(points);
			points.Clear();

			return this;
		}

		public GenChain PickAllPointsAndReturnPoint(Func<IEnumerable<Point>, Point> func)
		{
			Point result = func.Invoke(points);
			points.Add(result);

			return this;
		}

		public GenChain TakeAllPointsAndReturnPoint(Func<IEnumerable<Point>, Point> func)
		{
			Point result = func.Invoke(points);
			points.Clear();
			points.Add(result);

			return this;
		}

		public GenChain PickAllPointsAndReturnPoints(Func<IEnumerable<Point>, IEnumerable<Point>> func)
		{
			IEnumerable<Point> result = func.Invoke(points);
			foreach (Point point in result)
				points.Add(point);

			return this;
		}

		public GenChain TakeAllPointsAndReturnPoints(Func<IEnumerable<Point>, IEnumerable<Point>> func)
		{
			IEnumerable<Point> result = func.Invoke(points);
			points.Clear();

			foreach (Point point in result)
				points.Add(point);

			return this;
		}

		public GenChain DiscardAllPoints()
		{
			points.Clear();
			return this;
		}

		private IEnumerable<Point> GetRandomPoints(int count)
		{
			Point[] randPoints = new Point[count];

			for (int i = 0; i < count; i++)
				randPoints[i] = points[RandomPointIndex];

			return randPoints;
		}

		private IEnumerable<Point> GetAndRemoveRandomPoints(int count)
		{
			Point[] randPoints = new Point[count];

			for (int i = 0; i < count; i++)
			{
				int randIndex = RandomPointIndex;
				randPoints[i] = points[randIndex];
				points.RemoveAt(randIndex);
			}

			return randPoints;
		}

		private IEnumerable<Point> GetUniqueRandomPoints(int count)
		{
			Point[] randPoints = new Point[count];
			HashSet<int> pickedIndices = new HashSet<int>();

			int tries = 0;
			while (pickedIndices.Count < count && tries < 5000)
			{
				int randIndex = RandomPointIndex;

				if (pickedIndices.Contains(randIndex))
				{
					tries++;
					continue;
				}

				randPoints[pickedIndices.Count] = points[randIndex];
				pickedIndices.Add(randIndex);
			}

			Array.Resize(ref randPoints, pickedIndices.Count);

			return randPoints;
		}
	}
}
