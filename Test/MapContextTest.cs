using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using csUnit;
using ZFind.MapModule;
using ZFind.ZeebotModule;
using ZFind.CoreModule;

namespace ZFind.Test
{
	[TestFixture]
	public class MapContextTest
	{
		[Test]
		public void Test100Adds()
		{
			MapContext mapContext = new MapContext(MapBuilder.Build("Fixtures/map.in"));
			
			DateTime start = DateTime.Now;

			for (int i = 0; i < 100; i++)
				mapContext.AddZeebot(new Zeebot());

			TimeSpan span = DateTime.Now.Subtract(start);
			double ms = span.TotalMilliseconds;
			Assert.Less(ms, 100.0, 0.0);
		}

		[Test]
		public void TestTryMove()
		{
			MapContext mapContext = new MapContext(MapBuilder.Build("Fixtures/map.in"));

			Zeebot zeebot = new Zeebot();
			mapContext.AddZeebot(zeebot);
			mapContext.Zeebots[zeebot].Facing = Math.PI;
			mapContext.Zeebots[zeebot].Location = new Point(3.5, 6.5);

			double success;
			bool result = mapContext.TryMove(zeebot, 2.0, out success);

			Assert.Equals(false, result);
			Assert.Less(success, 0.5, Util.Epsilon);

			zeebot = new Zeebot();
			mapContext.AddZeebot(zeebot);
			mapContext.Zeebots[zeebot].Facing = Math.PI / 2;
			mapContext.Zeebots[zeebot].Location = new Point(6.5, 3.5);

			result = mapContext.TryMove(zeebot, 4.0, out success);

			Assert.Equals(true, result);
			Assert.Less(success, 4.01, Util.Epsilon);

			zeebot = new Zeebot();
			mapContext.AddZeebot(zeebot);
			mapContext.Zeebots[zeebot].Facing = 1.50;
			mapContext.Zeebots[zeebot].Location = new Point(1.60, 9.96);

			result = mapContext.TryMove(zeebot, 0.7, out success);

			Assert.Equals(false, result);
			Assert.Less(success, 0.04, 0.00);
		}

		[Test]
		public void TestQueryLaser()
		{
			MapContext mapContext = new MapContext(MapBuilder.Build("Fixtures/map.in"));
			double result;

			Zeebot zeebot = new Zeebot();
			mapContext.AddZeebot(zeebot);
			mapContext.Zeebots[zeebot].Facing = Math.PI;
			mapContext.Zeebots[zeebot].Location = new Point(3.5, 6.5);
			result = mapContext.QueryLaser(zeebot);
			Assert.Equals(result, 0.5, Util.Epsilon);

			zeebot = new Zeebot();
			mapContext.AddZeebot(zeebot);
			mapContext.Zeebots[zeebot].Facing = Math.PI / 2;
			mapContext.Zeebots[zeebot].Location = new Point(6.5, 4.0);
			result = mapContext.QueryLaser(zeebot);
			Assert.Equals(result, 6.00, Util.Epsilon);

			zeebot = new Zeebot();
			mapContext.AddZeebot(zeebot);
			mapContext.Zeebots[zeebot].Facing = Math.PI / 2;
			mapContext.Zeebots[zeebot].Location = new Point(9.5, 4.0);
			result = mapContext.QueryLaser(zeebot);
			Assert.Equals(double.NaN, result);
		}

		/*
		[Test]
		public void TestQueryTarget()
		{
			MapContext mapContext = new MapContext(MapBuilder.Build("Fixtures/map.in"));
			double result;

			Zeebot zeebot = new Zeebot();
			mapContext.AddZeebot(zeebot);
			mapContext.Zeebots[zeebot].Facing = Math.PI;
			mapContext.Zeebots[zeebot].Location = new Point(3.5, 6.5);
			result = mapContext.QueryTarget(zeebot);
			Assert.Equals(true, double.IsNaN(result));

			zeebot = new Zeebot();
			mapContext.AddZeebot(zeebot);
			mapContext.Zeebots[zeebot].Facing = Math.PI / 2;
			mapContext.Zeebots[zeebot].Location = new Point(2.5, 5.5);
			result = mapContext.QueryTarget(zeebot);
			Assert.Equals(false, double.IsNaN(result));
			Assert.Equals(1.0, result);

			zeebot = new Zeebot();
			mapContext.AddZeebot(zeebot);
			mapContext.Zeebots[zeebot].Facing = Math.PI / 2;
			mapContext.Zeebots[zeebot].Location = new Point(3.2, 6.5);
			result = mapContext.QueryTarget(zeebot);
			Assert.Equals(true, double.IsNaN(result));
		}
		 */
	}
}
