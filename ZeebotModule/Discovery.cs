using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZFind.MapModule;
using ZFind.CoreModule;

namespace ZFind.ZeebotModule
{
	public interface IDiscovery
	{
		void Run();
	}

	public class Discovery : IDiscovery
	{
		Zeebot zeebot;
		const int LookAroundDensity = 16;

		public Discovery(Zeebot zeebot)
		{
			this.zeebot = zeebot;
		}

		void LookAround()
		{
			double dist;
			double angle = 2 * Math.PI / LookAroundDensity;
			double randomOffset = Util.Random.NextDouble() * angle;

			// First, change the current facing of the Zeebot by a random offset,
			// so that every point has the same chance to be "hit".
			zeebot.ChangeFacing(randomOffset);

			// Then make some laser queries and add the new points.
			for (int i = 0; i < LookAroundDensity; i++)
			{
				dist = zeebot.QueryLaser();
				if (!double.IsNaN(dist))
				{
					Point pt = new Point();
					pt.X = zeebot.Location.X + dist * Math.Cos(zeebot.Facing);
					pt.Y = zeebot.Location.Y + dist * Math.Sin(zeebot.Facing);
					zeebot.CoordinateSystem.PartialMap.AddPoint(pt);
				}
				zeebot.ChangeFacing(angle);
			}

			// Reorient to mantain the same facing.
			zeebot.ChangeFacing(-randomOffset);
		}

		void Move()
		{
			double success;
			if (!zeebot.TryMove(0.7, out success))
			{
				// Hit an edge, turn back.
				zeebot.ChangeFacing(Util.Random.NextDouble() * 2 * Math.PI);
			}
		}

		public void Run()
		{
			LookAround();
			Move();
		}
	}
}
