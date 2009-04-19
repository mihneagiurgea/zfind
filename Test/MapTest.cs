using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using csUnit;
using ZFind.MapModule;
using ZFind.CoreModule;

namespace ZFind.Test
{
	[TestFixture]
	public class MapTest
	{
		[Test]
		public void TestEdgeContains()
		{
			Edge edge;

			edge = new Edge(1, 8, 4, 8);
			Assert.Equals(edge.Contains(new Point(2, 8)), true);
			Assert.Equals(edge.Contains(new Point(4, 8)), true);
			Assert.Equals(edge.Contains(new Point(6, 8)), false);
			Assert.Equals(edge.Contains(new Point(3, 7)), false);

			edge = new Edge(3, 7, 7, 9);
			Assert.Equals(edge.Contains(new Point(5, 8)), true);
			Assert.Equals(edge.Contains(new Point(5, 9)), false);
			Assert.Equals(edge.Contains(new Point(9, 10)), false);
			Assert.Equals(edge.Contains(new Point(3, -7)), false);
		}

		[Test]
		public void Test_WallwalkAngle()
		{
			Point[] tests = new Point[4];
			tests[0] = new Point(0, 2);
			tests[1] = new Point(2, 6);
			tests[2] = new Point(4, 0);
			tests[3] = new Point(6, 4);

			foreach (Point start in tests)
			{
				Console.WriteLine("{0} -> {1}", start.ToString(), Map.ComputeWallwalkAngle(start, new Point(3, 3),
					new Edge(2, 0, 4, 6)) / Math.PI);
			}
		}

		[Test]
		public void TestEdgeIntersection()
		{
			Edge edge;
			Point p1, p2, intersection;
			bool result;
			object[,] tests = { {4.0, 4.0, 6.0, 2.0, false, 0.0, 0.0},
								{0.0, 2.0, 6.0, -1.0, true, 2.0, 1.0},
								{0.0, 2.0, 6.0, 2.0, true, 4.0, 2.0},
								{6.0, -1.0, 2.0, -3.0, false, 0.0, 0.0} };

			edge = new Edge(0, 0, 4, 2);
			for (int i = 0; i < tests.GetLength(0); i++)
			{
				p1 = new Point((double)tests[i, 0], (double)tests[i, 1]);
				p2 = new Point((double)tests[i, 2], (double)tests[i, 3]);
				
				result = edge.ComputeIntersection(new Edge(p1, p2), out intersection);
				
				Assert.Equals(result, tests[i, 4]);
				if (result)
				{
					p1 = new Point((double)tests[i, 5], (double)tests[i, 6]);
					Assert.Equals(p1.X, intersection.X);
					Assert.Equals(p1.Y, intersection.Y);
				}
			}
			
		}

		[Test]
		public void TestTryMove()
		{
			Map map = MapBuilder.Build("Fixtures/map.in");
			double success;
			bool result;

			result = map.TryMove(new Point(3.5, 7.5), new Point(1.5, 7.5), out success);
			Assert.Equals(result, true);

			result = map.TryMove(new Point(3.5, 6.5), new Point(0, 6.5), out success);
			Assert.Equals(result, false);
			Assert.Less(success, 0.5, Util.Epsilon);

			result = map.TryMove(new Point(4.5, 4.5), new Point(1.5, 6.5), out success);
			Assert.Equals(result, false);
			Assert.Greater(success, 0.50, Util.Epsilon);
			Assert.Greater(0.65, success, Util.Epsilon);
		}

	}
}

