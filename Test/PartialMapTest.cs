using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using csUnit;
using ZFind.MapModule;

namespace ZFind.Test
{
	[TestFixture]
	public class PartialMapTest
	{
		// All these tests are designed for PartialMap.Epsilon = 0.1
		PartialMap pmap;

		[SetUp]
		public void SetUp()
		{
			pmap = new PartialMap();
		}

		void Expect(int expectedEdges, int expectedPoints)
		{
			Assert.Equals(expectedEdges, pmap.Edges.Count);
			Assert.Equals(expectedPoints, pmap.Points.Count);
		}

		void Contains(double X1, double Y1, double X2, double Y2)
		{
			Assert.Equals(true, pmap.Edges.Contains(new Edge(X1, Y1, X2, Y2)));
		}

		[Test]
		public void AddPoint()
		{
			pmap.AddPoint(new Point(0, 0));
			Expect(0, 1);
			pmap.AddPoint(new Point(0, 0.1));
			Expect(1, 0);
			pmap.AddPoint(new Point(0, 0.2));
			Expect(1, 0);
			pmap.AddPoint(new Point(0, 0.3));
			Expect(1, 0);
			pmap.AddPoint(new Point(1.0, 0.1));
			Expect(1, 1);
			pmap.AddPoint(new Point(2.0, 0.1));
			Expect(1, 2);
			pmap.AddPoint(new Point(2.0, 0.2));
			Expect(2, 1);

			pmap.AddPoint(new Point(5.0, 0.1));
			Expect(2, 2);
			pmap.AddPoint(new Point(5.0, 0.2));
			Expect(3, 1);
			pmap.AddPoint(new Point(5.0, 0.6));
			Expect(3, 2);
			pmap.AddPoint(new Point(5.0, 0.7));
			Expect(4, 1);
			pmap.AddPoint(new Point(5.0, 0.4));
			Expect(3, 1);

			pmap.AddPoint(new Point(7.0, 7.0));
			pmap.AddPoint(new Point(7.4, 7.0));
			pmap.AddPoint(new Point(7.2, 7.4));
			Expect(3, 4);
			pmap.AddPoint(new Point(7.2, 7.2));
			Expect(6, 1);

			Assert.Equals(true, pmap.Edges.Contains(new Edge(7.2, 7.4, 7.2, 7.2)));
			Assert.Equals(true, pmap.Edges.Contains(new Edge(7.0, 7.0, 7.2, 7.2)));
			Assert.Equals(true, pmap.Edges.Contains(new Edge(7.4, 7.0, 7.2, 7.2)));
		}

		[Test, Ignore("Fails, needs improvements")]
		public void CreateEdgesFromEdges()
		{
			pmap.AddPoint(new Point(0.0, 0.0));
			pmap.AddPoint(new Point(0.0, 0.2));
			pmap.AddPoint(new Point(0.0, 0.4));
			Expect(1, 0);
			pmap.AddPoint(new Point(0.1, 0.3));
			Expect(2, 0);
			pmap.AddPoint(new Point(0.4, 0.5));
			Expect(2, 1);
			pmap.AddPoint(new Point(-0.1, 0.6));
			Expect(3, 1);
			Contains(-0.1, 0.6, 0.0, 0.4);
		}

		[Test]
		public void AddPoints_Test()
		{
			Point[] path = {new Point(6,2), 
							new Point(6,3), 
							new Point(5,3), 
							new Point(5,2)};

			for (int i = 1; i < 4; i++)
			{
				int splits = 20;
				Point diff = path[i] - path[i-1];
				diff.X /= splits;
				diff.Y /= splits;
				Point p = path[i-1];
				for (int j = 0; j < splits; j++)
				{
					p = p + diff;
					pmap.AddPoint(p);
				}
				Console.WriteLine("Edges: {0} | Points: {1}", pmap.Edges.Count, pmap.Points.Count);
			}

			Expect(3, 0);
		}

	}
}
