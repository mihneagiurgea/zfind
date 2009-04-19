using System;
using System.Collections.Generic;
using System.Drawing;
using ZFind.CoreModule;

namespace ZFind.MapModule
{
	public class Map
	{
		protected List<Edge> edges;
		public List<Edge> Edges { get { return edges; } }

		public Map()
		{
		}
		public Map(List<Edge> edges)
		{
			this.edges = edges;
			ComputeMinMaxBox();
		}

		#region Min-Max box

		Point min, max;
		void RelaxMinMaxBox(Point p)
		{
			if (p.X < min.X) min.X = p.X;
			if (p.Y < min.Y) min.Y = p.Y;
			if (p.X > max.X) max.X = p.X;
			if (p.Y > max.Y) max.Y = p.Y;
		}
		void ComputeMinMaxBox()
		{
			min = new Point(double.MaxValue, double.MaxValue);
			max = new Point(double.MinValue, double.MinValue);

			foreach (Edge edge in Edges)
			{
				RelaxMinMaxBox(edge.P1);
				RelaxMinMaxBox(edge.P2);
			}
		}
		public void ComputeMinMaxBox(out Point lowerCorner, out Point upperCorner)
		{
			ComputeMinMaxBox();
			lowerCorner = min;
			upperCorner = max;
		}
		public bool InMinMaxBox(Point pt)
		{
			return (min.X <= pt.X && pt.X <= max.X) && (min.Y <= pt.Y && pt.Y <= max.Y);
		}

		#endregion

		public bool TryMove(Point start, Point end, out double success)
		{
			return TryMove(start, end, 0.01, out success);
		}
		public bool CanMove(Point start, Point end, out double success)
		{
			return TryMove(start, end, 0.0, out success);
		}
		bool TryMove(Point start, Point end, double noTouch, out double success)
		{
			success = start.DistanceTo(end);
			bool result = true;
			Edge other = new Edge(start, end);
			Point intersection;
			foreach (Edge edge in edges)
				if (edge.ComputeIntersection(other, out intersection) && 
					intersection.Between(other.P1, other.P2))
				{
					success = Math.Min(success, start.DistanceTo(intersection) - noTouch);
					result = false;
				}
			if (success < 0) success = 0;
			return result;
		}
		public static double ComputeWallwalkAngle(Point start, Point intersection, Edge wall)
		{
			// Compute the angle needed to turn in order to walk parallel to the wall,
			// with your right hand on it.
			Point heading, end;
			Edge compare;
			end = wall.P1;
			heading = start + end - intersection;
			compare = new Edge(intersection, heading);
			if (compare.CrossProduct(end) > 0)
			{
				end = wall.P2;
				heading = start + end - intersection;
				compare = new Edge(intersection, heading);
			}
			return Math.Atan2(end.Y - intersection.Y, end.X - intersection.X) - 
				   Math.Atan2(intersection.Y - start.Y, intersection.X - start.X);
		}

		public bool IsFree(Point point)
		{
			foreach (Edge edge in edges)
				if (point.DistanceTo(edge) < Util.Epsilon) return false;
			return true;
		}

		#region Removed functionality : Target

		/*
		protected List<Point> targets;
		public List<Point> Targets { get { return targets; } }

		public bool CanMove(Point start, Point end)
		{
			Edge other = new Edge(start, end);
			foreach (Edge edge in edges)
				if (edge.Intersects(other))
					return false;
			return true;
		}
		
		public bool IsTargetInRange(Point location, double range)
		{
			foreach (Point target in targets)
				if (location.DistanceTo(target) <= range &&
					CanMove(location, target)) return true;
			return false;
		}

		public double FindTarget(Point location, double range)
		{
			bool found = false;
			double result = double.MaxValue, dist;
			foreach (Point target in targets)
			{
				dist = location.DistanceTo(target);
				if (dist <= range && CanMove(location, target))
				{
					found = true;
					result = Math.Min(result, dist);
				}
			}
			if (found) return result;
			else return double.NaN;
		}
		*/

		#endregion
	}
}
