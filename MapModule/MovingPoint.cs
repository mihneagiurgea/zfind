using System;
using ZFind.CoreModule;

namespace ZFind.MapModule
{
	public class MovingPoint
	{
		public Point Location { get; set; }
		public double X { get { return Location.X; } }
		public double Y { get { return Location.Y; } }

		double _facing;
		public double Facing
		{
			get { return _facing; }
			set
			{
				_facing = Util.ModPI(value);
			}
		}

		public MovingPoint(double x, double y, double facing)
			: this( new Point(x, y), facing) {}
		public MovingPoint(Point location, double facing)
		{
			Location = location;
			Facing = facing;
		}
		public MovingPoint(MovingPoint toCopy)
		{
			Location = toCopy.Location;
			Facing = toCopy.Facing;
		}

		public MovingPoint Rotate(double angle)
		{
			return new MovingPoint(Location.Rotate(angle), Facing + angle);
		}
		public MovingPoint Rotate(double angle, Point around)
		{
			return new MovingPoint(Location.Rotate(angle, around), Facing + angle);
		}
		public static MovingPoint operator +(MovingPoint p, Point translate)
		{
			return new MovingPoint(p.Location + translate, p.Facing);
		}
		public static Point operator -(MovingPoint p1, MovingPoint p2)
		{
			return p1.Location - p2.Location;
		}

		public static bool operator ==(MovingPoint p1, MovingPoint p2)
		{
			return (p1.Location == p2.Location) && Util.IsEqual(p1.Facing, p2.Facing);
		}
		public static bool operator !=(MovingPoint p1, MovingPoint p2)
		{
			return !(p1 == p2);
		}

		public override string ToString()
		{
			return string.Format("({0}) facing {1:0.00}JI", Location, Facing / Math.PI);
		}
	}
}

