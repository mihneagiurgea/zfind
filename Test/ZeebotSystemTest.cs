using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using csUnit;
using ZFind.ZeebotModule;
using ZFind.MapModule;
using ZFind.CoreModule;

namespace ZFind.Test
{
	[TestFixture]
	public class ZeebotSystemTest
	{
		ZeebotSystem zeebotSystem;
		bool logging = false;

		[SetUp]
		public void SetUp()
		{
		}

		delegate MovingPoint CalculatedDelegate(Zeebot z);

		MovingPoint Real(Zeebot z) { return zeebotSystem.MapContext.Zeebots[z]; }
		MovingPoint CalculatedNoSimetry(Zeebot z)
		{
			return new MovingPoint(zeebotSystem.CoordinateSystem.Zeebots[z]);
		}
		MovingPoint CalculatedOXSimetry(Zeebot z)
		{
			MovingPoint toCopy = zeebotSystem.CoordinateSystem.Zeebots[z];
			return new MovingPoint(toCopy.X, -toCopy.Y, - toCopy.Facing);
		}
		double Angle(Point pt) { return Math.Atan2(pt.Y, pt.X); }

		bool Overlap(CalculatedDelegate Calculated)
		{
			List<Zeebot> zeebots = zeebotSystem.Zeebots;
			Zeebot z0, z1;

			// Test: pick a random zeebot, and translate + rotate all so that
			// its calculated position coincides with its real one.
			int i = Util.Random.Next(zeebots.Count);
			i = 3;
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

				if (position != Real(zb))
					return false;
			}
			return true;
		}
		bool Overlaps()
		{
			bool no, ox;
			no = Overlap(CalculatedNoSimetry);
			ox = Overlap(CalculatedOXSimetry);
			if (logging)
				Console.WriteLine("Overlaps: {0} || {1} = {2}", no, ox, no | ox);
			return no | ox;
		}

		void System_Test(int nrZeebots, int testSize)
		{
			zeebotSystem = new ZeebotSystem(new MapContext(MapBuilder.Build("Fixtures/map.in")), nrZeebots);

			Assert.Equals(true, Overlaps());
			for (int i = 0; i < testSize; i++)
			{
				zeebotSystem.Run();
				Assert.Equals(true, Overlaps());
			}
		}

		[Test]
		public void System_SmallTest()
		{
			Edge edge = new Edge(0.91, -5.50, 3.51, -2.55);
			Point h1 = edge.IntersectWithPerpendicular(new Point(3.43, -2.64));
			// System_Test(5, 50);
			List<Edge> edges = zeebotSystem.CoordinateSystem.PartialMap.Edges;
			List<double> angles = new List<double>();
			foreach (Edge e in edges)
				angles.Add(e.Angle);
			angles.Sort();
			
		}

		[Test]
		public void System_LargeTest()
		{
			System_Test(25, 150);
		}
	}
}
