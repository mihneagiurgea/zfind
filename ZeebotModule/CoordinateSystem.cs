using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using ZFind.MapModule;
using ZFind.CoreModule;

namespace ZFind.ZeebotModule
{
	public class CoordinateSystem
	{
		// Computes the coordinates and facings for each Zeebot, by using 3 Zeebots
		// (and thus 2 vectors that form a base). Then will compute the starting
		// facing direction for each zeebot.
		
		public Dictionary<Zeebot, MovingPoint> Zeebots {get; private set;}
		public PartialMap PartialMap { get; set; }
		public int Sign { get; set; }
		Zeebot z1, z2, z3;

		public CoordinateSystem()
		{
			Zeebots = new Dictionary<Zeebot, MovingPoint>();
			PartialMap = new PartialMap();
			z1 = z2 = z3 = null;
			Sign = 0;
		}

		public void ComputeMinMaxBox(out Point lowerCorner, out Point upperCorner)
		{
			Point min, max;
			PartialMap.ComputeMinMaxBox(out min, out max);
			foreach (MovingPoint position in Zeebots.Values)
			{
				Point p = position.Location;
				if (p.X < min.X) min.X = p.X;
				if (p.Y < min.Y) min.Y = p.Y;
				if (p.X > max.X) max.X = p.X;
				if (p.Y > max.Y) max.Y = p.Y;
			}
			lowerCorner = min;
			upperCorner = max;
		}

		public Point IntersectCircles(Point c1, double r1, Point c2, double r2, Point c3, double r3)
		{
			// To compute the intersection of the 3 circle, we intersect two of them, get 2
			// solutions, and see which one is on the third circle.

			// Step 1: Translate all circle centers so that c1 = (0, 0).
			c2 = c2 - c1;
			c3 = c3 - c1;
			
			// Step 2: Rotate all around (0, 0) so that c2 is now on Ox axis.
			double O = Math.Atan2(c2.Y, c2.X);
			c2 = c2.Rotate(-O);
			c3 = c3.Rotate(-O);

			// Step 3: Intersect circles (0, 0, r1) with (c2.X, 0, r2)
			Point p1 = new Point(), p2 = new Point(), sol;
			p1.X = p2.X = (r1 * r1 - r2 * r2 + c2.X * c2.X) / (2 * c2.X);
			// sqrt( negative zero ) = NaN
			if (r1 > p1.X) p1.Y = Math.Sqrt(r1 * r1 - p1.X * p1.X);
			else p1.Y = 0;
			p2.Y = - p1.Y;

			if (Util.IsEqual(p2.DistanceTo(c3), r3))
				sol = p2;
			else
				sol = p1;
			
			// Unapply step 2
			sol = sol.Rotate(O);

			// Unapply step 1
			sol = sol + c1;

			return sol;
		}

		void ComputeFacingDirection(Zeebot zeebot)
		{
			double success = 0, moved = 0;
			double oldFacing = zeebot.Facing;
			Point oldLocation = new Point();

			// Turn the zeebot around, because he's probably stuck.
			zeebot.ChangeFacing(Math.PI);
			moved = Math.PI;
			for (int tries = 0; tries < 20 && success < Util.Epsilon; tries++)
			{
				zeebot.ChangeFacing(Math.PI / 7);
				moved += Math.PI / 7;
				oldLocation = ComputeLocation(zeebot);
				zeebot.TryMove(1.0, out success);
			}
			if (success < Util.Epsilon)
			{
				// Zeebot is "boxed-in"
				throw new Exception("Not implemented: boxed-in zeebot");
			}
			Point current = ComputeLocation(zeebot);
			Point diff = current - oldLocation;
			double O = Math.Atan2(diff.Y, diff.X);

			// Rewrite the correct Location & Facing for zeebot.
			zeebot.Location = current;
			zeebot.Facing = O;

			this.Sign = (Util.IsEqual(Util.ModPI(oldFacing + moved), O)) ? +1 : -1;
		}

		void ComputeFacing(Zeebot zeebot)
		{
			double success;

			// Find 2 Zeebot references - z1 and (z2 or z3) - non-colinear with zeebot
			Zeebot zref1, zref2;
			if (zeebot == z1)
			{
				zref1 = z2;
				zref2 = z3;
			}
			else
			{
				zref1 = z1;
				if (zeebot.Location.OnLine(z1.Location, z2.Location)) zref2 = z3;
				else zref2 = z2;
			}

			zeebot.TryMove(1.0, out success);
			for (int tries = 0; tries < 20 && success < Util.Epsilon; tries++)
			{
				zeebot.ChangeFacing(Math.PI / 7);
				zeebot.TryMove(1.0, out success);
			}
			if (success < Util.Epsilon)
			{
				// Zeebot is "boxed-in"
				throw new Exception("Not implemented: boxed-in zeebot");
			}
			Point current = IntersectCircles(zeebot.Location, success,
											 zref1.Location, zref1.DistanceTo(zeebot),
											 zref2.Location, zref2.DistanceTo(zeebot));
			Point diff = current - zeebot.Location;
			double O = Math.Atan2(diff.Y, diff.X);

			// Found the zeebot's facing, updating it and the new location.
			zeebot.Location = current;
			zeebot.Facing = O;
			zeebot.Initialized = true;
		}

		void ComputeZ3(out double x, out double y)
		{
			double r1 = z3.DistanceTo(z1);
			double r2 = z3.DistanceTo(z2);
			double d = Zeebots[z2].Location.X;
			x = (r1 * r1 - r2 * r2 + d * d) / (2 * d);
			// sqrt( negative zero ) = NaN
			if (r1 > x) y = Math.Sqrt(r1 * r1 - x * x);
			else y = 0;
		}
		Point ComputeLocation(Zeebot zeebot)
		{
			return IntersectCircles(z1.Location, z1.DistanceTo(zeebot),
									z2.Location, z2.DistanceTo(zeebot),
									z3.Location, z3.DistanceTo(zeebot));
		}
		public void AddZeebot(Zeebot zeebot)
		{
			if (z1 == null)
			{
				// First zeebot, assume (0, 0, ?).
				z1 = zeebot;
				Zeebots[zeebot] = new MovingPoint(0, 0, double.NaN);
			}
			else
				if (z2 == null)
				{
					// Second zeebot, assume (d, 0, ?). Later compute ?.
					z2 = zeebot;
					double d = z1.DistanceTo(z2);
					Zeebots[zeebot] = new MovingPoint(d, 0, double.NaN);
				}
				else
					if (z3 == null)
					{
						// Third zeebot, compute (x, y, ?). After compute all ? facings.
						z3 = zeebot;
						double x, y, success;
						ComputeZ3(out x, out y);
						// If y = 0, they are colinear, so I'll move it a bit before.
						if (Util.IsZero(y))
						{
							z3.TryMove(1.0, out success);
							ComputeZ3(out x, out y);
							// If y = 0, change its facing, then move it again.
							if (Util.IsZero(y))
							{
								z3.ChangeFacing(Math.PI / 2);
								z3.ChangeFacing(1.0);
								ComputeZ3(out x, out y);
							}
						}

						Zeebots[zeebot] = new MovingPoint(x, y, double.NaN);

						// Can now compute all facings: move one zeebot in a direction, 
						// then intersect the 3 circles to find its exact position -> determine facing.
						ComputeFacing(z3);
						ComputeFacing(z2);
						ComputeFacing(z1);
					}
					else
					{
						// We already have the base in place, now use that to compute this
						// zeebot's exact coordinates.
						Point location = ComputeLocation(zeebot);
						Zeebots[zeebot] = new MovingPoint(location, double.NaN);
						ComputeFacing(zeebot);
						
						// Now we determing the general facing direction (+/-) of the Coordinate System.
						if (Sign == 0)	
							ComputeFacingDirection(zeebot);
					}
		}
		
	}  
}
