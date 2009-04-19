using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using csUnit;
using ZFind.ZeebotModule;
using ZFind.MapModule;

namespace ZFind.Test
{
	[TestFixture]
	public class CoordinateSystemTest
	{
		CoordinateSystem coordinateSystem;
		MapContext mapContext;
		Random random = new Random();

		[SetUp]
		public void SetUp()
		{
			coordinateSystem = new CoordinateSystem();
			mapContext = new MapContext(MapBuilder.Build("Fixtures/map.in"));
		}

		void Expect(Point p, Point p1, double r1, Point p2, double r2, Point p3, double r3)
		{
			Assert.Equals(p, coordinateSystem.IntersectCircles(p1, r1, p2, r2, p3, r3));
		}

		void Rotate(double x, double y, double x1, double y1, double r1,
										double x2, double y2, double r2,
										double x3, double y3, double r3)
		{
			Point p1 = new Point(x1, y1), p2 = new Point(x2, y2), p3 = new Point(x3, y3);
			Point p = new Point(x, y);
			// 1 2 3
			Expect(p, p1, r1, p2, r2, p3, r3);
			// 2 1 3
			Expect(p, p2, r2, p1, r1, p3, r3);
			// 3 2 1
			Expect(p, p3, r3, p2, r2, p1, r1);

			// Heavy random translation + rotations
			for (int i = 0; i < 50; i++)
			{
				double O = random.NextDouble() * 2 * Math.PI;
				Point T = new Point(random.NextDouble() * 1000 - 500, random.NextDouble() * 2000 - 1000);
				Expect((p+T).Rotate(O), (p1+T).Rotate(O), r1, (p2+T).Rotate(O), r2, (p3+T).Rotate(O), r3);
				Expect(p.Rotate(O), p1.Rotate(O), r1, p2.Rotate(O), r2, p3.Rotate(O), r3);
			}
		}

		[Test]
		public void IntersectCircles()
		{
			// Each test scrambles the points around + 1 random rotation, just to make sure :)

			// Simple points on coordinat axes
			Rotate(0, 0, 5, 0, 5, 0, 3, 3, 0, -7, 7);

			// Circle centers are colinear + one circle inside the other
			Rotate(10, 2, 2, 2, 8, 5, 2, 5, 12, 2, 2);
		}

		delegate MovingPoint CalculatedDelegate(Zeebot z);

		MovingPoint Real(Zeebot z) { return mapContext.Zeebots[z];  }
		MovingPoint Calculated(Zeebot z) 
		{
			return new MovingPoint(coordinateSystem.Zeebots[z]);
		}
		MovingPoint CalculatedWithSimetry(Zeebot z)
		{
			MovingPoint toCopy = coordinateSystem.Zeebots[z];
			return new MovingPoint(toCopy.X, -toCopy.Y, 2 * Math.PI - toCopy.Facing);
		}
		double Angle(Point pt) { return Math.Atan2(pt.Y, pt.X); }

		bool Overlap(List<Zeebot> zeebots, CalculatedDelegate Calculated)
		{
			Zeebot z0, z1;

			// Test: pick a random zeebot, and translate + rotate all so that
			// its calculated position coincides with its real one.
			int i = random.Next(zeebots.Count);
			z0 = zeebots[i];
			z1 = zeebots[(i + 1) % zeebots.Count];

			Point translate = Real(z0) - Calculated(z0);
			Point center = Calculated(z0).Location;
			double O = Angle(Real(z1) - Real(z0)) - Angle(Calculated(z1) - Calculated(z0));

			foreach (Zeebot zb in zeebots)
			{
				MovingPoint position = Calculated(zb);

				position = position.Rotate(O, center);
				position += translate;

				//if (position.Location != Real(zb).Location)
				if (position != Real(zb))
					return false;
			}
			return true;
		}

		[Test, Ignore("Obsolete")]
		public void AddZeebot_SmallTest()
		{
			List<MovingPoint> positions = new List<MovingPoint>();
			positions.Add(new MovingPoint(1.5, 3.5, Math.PI / 2));
			positions.Add(new MovingPoint(4.5, 6.5, Math.PI));
			positions.Add(new MovingPoint(3.5, 1.5, 0));
			positions.Add(new MovingPoint(5.5, 4.5, Math.PI / 3));
			positions.Add(new MovingPoint(2.5, 8.5, Math.PI));
							
			List<Zeebot> zeebots = new List<Zeebot>();

			// Add the zeebots in the mapcontext.
			for (int i = 0; i < positions.Count; i++)
			{
				Zeebot z = new Zeebot();
				z.MapContext = mapContext;
				z.CoordinateSystem = coordinateSystem;
				mapContext.AddZeebotAt(z, positions[i]);
				zeebots.Add(z);
			}

			// Run the CoordinateSystem for the same zeebots.
			for (int i = 0; i < positions.Count; i++)
				coordinateSystem.AddZeebot(zeebots[i]);

			bool overlap = Overlap(zeebots, Calculated);
			bool overlapWithSimetry = Overlap(zeebots, CalculatedWithSimetry);

			Console.WriteLine("Overlap: {0}", overlap);
			Console.WriteLine("Overlap with oX simetry: {0}", overlapWithSimetry);
			
			Assert.Equals(true, overlap || overlapWithSimetry);
		}

	}
}