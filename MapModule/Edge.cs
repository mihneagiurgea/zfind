using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZFind.CoreModule;

namespace ZFind.MapModule
{
	public struct Point
	{
		public double X { get; set; }
		public double Y { get; set; }

		public Point(double x, double y)
			: this()
		{
			X = x;
			Y = y;
		}

		public double DistanceTo(Point other)
		{
			return Math.Sqrt((other.X - X) * (other.X - X) + (other.Y - Y) * (other.Y - Y));
		}
		public double DistanceTo(Edge edge)
		{
			double nom = Math.Abs((edge.X2 - edge.X1) * (edge.Y1 - Y) - (edge.X1 - X) * (edge.Y2 - edge.Y1));
			double denom = Math.Sqrt((edge.X2 - edge.X1) * (edge.X2 - edge.X1) + (edge.Y2 - edge.Y1) * (edge.Y2 - edge.Y1));
			return nom / denom;
		}

		public bool OnLine(Point P1, Point P2)
		{
			return Util.IsZero(P1.X * (P2.Y - Y) + P2.X * (Y - P1.Y) + X * (P1.Y - P2.Y));
		}
		public bool Between(Point P1, Point P2, bool strict)
		{
			if (!strict)
			{
				if (Util.IsEqual(P1.X, P2.X))
					return Util.IsPositiveOrZero((Y - P1.Y) * (P2.Y - Y));
				else
					return Util.IsPositiveOrZero((X - P1.X) * (P2.X - X));
			}
			else
			{
				if (Util.IsEqual(P1.X, P2.X))
					return Util.IsPositive((Y - P1.Y) * (P2.Y - Y));
				else
					return Util.IsPositive((X - P1.X) * (P2.X - X));
			}
		}
		public bool Between(Point P1, Point P2)
		{
			return Between(P1, P2, false);
		}
		/// <summary>
		/// Returns where the point is situated on P1, P2 segment, assuming P is on P1, P2 line:
		/// 0 if between P1 and P2, 1 if "on" P1's side, 2 if "on" P2's side.
		/// </summary>
		public int Where(Point P1, Point P2)
		{
			if (Between(P1, P2)) return 0;
			if (P1.Between(this, P2)) return 1;
			else return 2;
		}

		/// <summary>
		/// Rotates a point by an angle around (0,0) and returns the result.
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		public Point Rotate(double angle)
		{
			double cos = Math.Cos(angle);
			double sin = Math.Sin(angle);
			return new Point(X * cos - Y * sin, 
							 X * sin + Y * cos);
		}
		/// <summary>
		/// Rotates a point by an angle around another point and returns the result.
		/// </summary>
		/// <param name="angle"></param>
		/// <param name="around"></param>
		/// <returns></returns>
		public Point Rotate(double angle, Point around)
		{
			return around + (this - around).Rotate(angle);
		}
		public static Point operator +(Point p1, Point p2)
		{
			return new Point(p1.X + p2.X, p1.Y + p2.Y);
		}
		public static Point operator -(Point p1, Point p2)
		{
			return new Point(p1.X - p2.X, p1.Y - p2.Y);
		}
		public static Point operator *(Point p, double w)
		{
			return new Point(p.X * w, p.Y * w);
		}
		static Point Min(Point p1, Point p2)
		{
			if (Util.IsEqual(p1.X, p2.X))
				if (p1.Y < p2.Y) return p1;
				else return p2;
			else
				if (p1.X < p2.X) return p1;
				else return p2;
		}
		public static Point Min(params Point[] points)
		{
			if (points.Length == 1) return points[0];
			Point result = Min(points[0], points[1]);
			for (int i = 2; i < points.Length; i++)
				result = Min(result, points[i]);
			return result;
		}
		static Point Max(Point p1, Point p2)
		{
			if (Util.IsEqual(p1.X, p2.X))
				if (p1.Y > p2.Y) return p1;
				else return p2;
			else
				if (p1.X > p2.X) return p1;
				else return p2;
		}
		public static Point Max(params Point[] points)
		{
			if (points.Length == 1) return points[0];
			Point result = Max(points[0], points[1]);
			for (int i = 2; i < points.Length; i++)
				result = Max(result, points[i]);
			return result;
		}

		public static bool operator ==(Point p1, Point p2)
		{
			return Util.IsEqual(p1.X, p2.X) && Util.IsEqual(p1.Y, p2.Y);
		}
		public static bool operator !=(Point p1, Point p2) { return !(p1 == p2); }
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			if (obj is Point)
				return this == (Point)obj;
			else
				return false;
		}
		public override string ToString()
		{
			return this.ToString(false);
		}
		public string ToString(bool precision)
		{
			if (precision)
				return string.Format("{0:0.000000},{1:0.000000}", X, Y);
			else
				return string.Format("{0:0.00},{1:0.00}", X, Y);
		}
	}

	public class Edge : IEquatable<Edge>
	{
		public double Weight { get; set; }
		public Point P1 { get; set; }
		public Point P2 { get; set; }
		public double Angle
		{
			get
			{
				if (Util.IsEqual(X1, X2))
					return double.NaN;
				else
					return (Y2 - Y1) / (X2 - X1);
			}
		}

		public double X1 { get { return P1.X; } }
		public double Y1 { get { return P1.Y; } }
		public double X2 { get { return P2.X; } }
		public double Y2 { get { return P2.Y; } }

		public Edge(double x1, double y1, double x2, double y2)
		{
			P1 = new Point(x1, y1);
			P2 = new Point(x2, y2);
			Weight = 2;
		}
		public Edge(Point p1, Point p2)
		{
			P1 = p1;
			P2 = p2;
			Weight = 2;
		}
		public double CrossProduct(Point p3)
		{
			return (P2.X - P1.X) * (p3.Y - P1.Y) - (P2.Y - P1.Y) * (p3.X - P1.X);
		}
		bool SameSide(Edge other)
		{
			int sgn1 = Util.Sign(CrossProduct(other.P1));
			int sgn2 = Util.Sign(CrossProduct(other.P2));
			return sgn1 * sgn2 <= 0;
		}
		public bool Intersects(Edge other)
		{
			return this.SameSide(other) && other.SameSide(this);
		}
		public bool Contains(Point point)
		{
			if (!Util.IsZero(CrossProduct(point))) return false;
			return point.Between(P1, P2);
		}
		public double DistanceTo(Edge other)
		{
			// Crappy implementation :)
			double result = P1.DistanceTo(other);
			result = Math.Min(result, P2.DistanceTo(other));
			result = Math.Min(result, other.P1.DistanceTo(this));
			result = Math.Min(result, other.P2.DistanceTo(this));
			return result;
		}
		#region IEquatable<Edge> Members

		public bool Equals(Edge other)
		{
			return (P1 == other.P1 && P2 == other.P2) ||
				   (P1 == other.P2 && P2 == other.P1);
		}

		#endregion
		public override string ToString()
		{
			return this.ToString(false);
		}
		public string ToString(bool precision)
		{
			return string.Format("({0})-({1})", P1.ToString(precision), P2.ToString(precision));
		}

		public bool ComputeIntersection(Edge other, out Point intersection)
		{
			intersection = new Point();

			double denom = (other.P2.Y - other.P1.Y) * (P2.X - P1.X) -
						   (other.P2.X - other.P1.X) * (P2.Y - P1.Y);
			double nume_a = (other.P2.X - other.P1.X) * (P1.Y - other.P1.Y) -
							(other.P2.Y - other.P1.Y) * (P1.X - other.P1.X);
			double nume_b = (P2.X - P1.X) * (P1.Y - other.P1.Y) -
							(P2.Y - P1.Y) * (P1.X - other.P1.X);

			if (Util.IsZero(denom))
			{
				//if (nume_a == 0 && nume_b == ) -> COINCIDENT
				//else -> PARALLEL
				return false;
			}

			double ua = nume_a / denom;
			double ub = nume_b / denom;

			if (ua >= 0.0 && ua <= 1.0 && ub >= 0.0 && ub <= 1.0)
			{
				intersection.X = P1.X + ua * (P2.X - P1.X);
				intersection.Y = P1.Y + ua * (P2.Y - P1.Y);
				return true;
			}
			else
				return false;
		}
		public Point IntersectWithPerpendicular(Point p)
		{
			// Intersects an edge with the line that contains p and is perpendicular to it.
			double u = ((p.X - X1) * (X2 - X1) + (p.Y - Y1) * (Y2 - Y1)) /
					   ((X2 - X1) * (X2 - X1) + (Y2 - Y1) * (Y2 - Y1));
			Point intersection = new Point();
			intersection.X = X1 + u * (X2 - X1);
			intersection.Y = Y1 + u * (Y2 - Y1);
			return intersection;
		}
	}

}
