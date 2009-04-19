using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZFind.CoreModule;

namespace ZFind.MapModule
{
	public class PartialMap : Map
	{
		List<Point> points;

		public const int MinPointsInLine = 7;
		public const double Epsilon = 0.1;
		public const double EpsilonAngle = 0.0000001; // 10 ** -7

		public List<Point> Points { get { return points; } }

		public PartialMap()
		{
			edges = new List<Edge>();
			points = new List<Point>();
		}

		bool MergePointInEdge(Point p)
		{
			// Checks if this point belongs to an already existing edge.
			foreach (Edge e in edges)
				if (p.DistanceTo(e) < Epsilon && p.Between(e.P1, e.P2))
					return true;
			return false;
		}
		bool CreateEdgesFromPoints(Point p)
		{
			// Finds >= MinPointsInLine points (including point p) that are ~ colinear and 
			// creates an edge from them. Can build multiple edges.
			bool created = false;
			Dictionary<double, List<int>> dex = new Dictionary<double, List<int>>();

			for (int i = 0; i < points.Count; i++)
				if (p.DistanceTo(points[i]) < MinPointsInLine * Epsilon)
				{
					double angle;
					if (Util.IsEqual(p.X, points[i].X))
						angle = double.NaN;
					else
					{
						angle = (p.Y - points[i].Y) / (p.X - points[i].X);
						angle = Math.Truncate(angle / EpsilonAngle) * EpsilonAngle;
					}
					if (!dex.ContainsKey(angle))
						dex[angle] = new List<int>(2 * MinPointsInLine);
					dex[angle].Add(i);
				}
			foreach (KeyValuePair<double, List<int>> item in dex)
				if (item.Value.Count >= MinPointsInLine)
				{
					// Merge the points from item.Value into a single edge.
					List<int> indexes = item.Value;
					Point min, max;
					min = points[indexes[0]];
					max = points[indexes[0]];
					for (int i = 1; i < indexes.Count; i++)
					{
						min = Point.Min(min, points[indexes[i]]);
						max = Point.Max(max, points[indexes[i]]);
					}
					// Remove the points.
					indexes.Sort();
					for (int i = indexes.Count - 1; i >= 0; i--)
						points.RemoveAt(indexes[i]);
					// Add the new edge.
					Edge newEdge = new Edge(min, max);
					if (!MergeEdge(newEdge))
						edges.Add(newEdge);
					created = true;
				}
			return created;
		}
		bool MergeEdge(Edge edge)
		{
			// Tries to merge edge into another, if they are parralel and close.
			foreach (Edge e in edges)
				if (Util.IsEqual(e.Angle, edge.Angle, EpsilonAngle) && edge.P1.DistanceTo(e) < Epsilon)
				{
					Point h1 = edge.IntersectWithPerpendicular(e.P1);
					Point h2 = edge.IntersectWithPerpendicular(e.P2);

					// If edge and (h1, h2) do not intersect and are too far away, ignore e.
					if (!edge.Contains(h1) && !edge.Contains(h2))
					{
						if (Epsilon <= Math.Min(
								Math.Min(e.P1.DistanceTo(edge.P1), e.P1.DistanceTo(edge.P2)),
								Math.Min(e.P2.DistanceTo(edge.P1), e.P2.DistanceTo(edge.P2)))
							)							
							continue;
					}
					Point boxL1, boxL2, boxR1, boxR2;
					boxL1 = Point.Min(edge.P1, edge.P2, h1, h2);
					boxR1 = Point.Max(edge.P1, edge.P2, h1, h2);

					Point g1, g2;
					g1 = e.IntersectWithPerpendicular(edge.P1);
					g2 = e.IntersectWithPerpendicular(edge.P2);
					boxL2 = Point.Min(e.P1, e.P2, g1, g2);
					boxR2 = Point.Max(e.P1, e.P2, g1, g2);

					bool wrong = false;
					if (boxL1.DistanceTo(boxL2) > 0.5) wrong = true;
					if (boxR1.DistanceTo(boxR2) > 0.5) wrong = true;

					if (wrong)
					{
						throw new Exception("muie");
					}

					// Merging:
					double W = edge.Weight + e.Weight;
					double w1 = edge.Weight / W;
					double w2 = e.Weight / W;
					Point old1 = e.P1;
					Point old2 = e.P2;
					e.P1 = (boxL1 * w1) + (boxL2 * w2);
					e.P2 = (boxR1 * w1) + (boxR2 * w2);
					// if (Math.Max(old1.DistanceTo(e.P1), old2.DistanceTo(e.P2)) > 0.5)
					//	throw new Exception(string.Format("Muie {0}!", e));
					e.Weight = W;
					int max = 14;
					if (e.P1.X > max || e.P2.X > max || e.P2.Y > max || e.P1.Y > max)
						throw new Exception(string.Format("Muie {0}!", e));
					return true;
				}
			return false;
		}
		bool CreateEdgesFromEdges(Point p)
		{
			// Point p is near an edge, and we build a perpendicular edge from p to that edge,
			// or a new edge from p to an end of the edge.
			bool created = false;
			int i, N = edges.Count;
			Edge e;
			for (i = 0; i < N; i++)
			{
				e = edges[i];
				double dist = p.DistanceTo(e);
				// If point p is close to e, but not on e...
				if (dist < Epsilon && !Util.IsZero(dist))
				{
					Point intersection = e.IntersectWithPerpendicular(p);
					if (e.Contains(intersection))
					{
						edges.Add(new Edge(p, intersection));
						created = true;
					}
					else
						// Or just create with nearest edge end, if possible
						if (p.DistanceTo(e.P1) < Epsilon)
						{
							edges.Add(new Edge(p, e.P1));
							created = true;
						}
						else
							if (p.DistanceTo(e.P2) < Epsilon)
							{
								edges.Add(new Edge(p, e.P2));
								created = true;
							}
				}
			}
			return created;
		}
		bool ExtendEdges(Point p)
		{
			// Point p is on director vector of edge e, and we extend e to include p.
			List<Edge> extended = new List<Edge>();
			foreach (Edge e in edges)
				if (p.OnLine(e.P1, e.P2) && p != e.P1 && p != e.P2)
				{
					int where = p.Where(e.P1, e.P2);
					if (where == 1 && p.DistanceTo(e.P1) < Epsilon) 
					{
						e.P1 = p;
						extended.Add(e);
					}
					if (where == 2 && p.DistanceTo(e.P2) < Epsilon)
					{
						e.P2 = p;
						extended.Add(e);
					}
				}
			if (extended.Count == 0) return false;
			// We try to link 2 extended edges to p to form a single edge.
			Point P1, P2;
			for (int i = 0; i < extended.Count; i++)
				if (extended[i] != null)
				{
					if (extended[i].P1 == p) P1 = extended[i].P2;
					else P1 = extended[i].P1;
					for (int j = i + 1; j < extended.Count; j++)
						if (extended[j] != null)
						{
							if (extended[j].P1 == p) P2 = extended[j].P2;
							else P2 = extended[j].P1;
							if (p.OnLine(P1, P2))
							{
								// This works because extended[i] is a pointer!
								extended[i].P1 = P1;
								extended[i].P2 = P2;
								edges.Remove(extended[j]);
								extended[j] = null;
								break;
							}
						}
				}
			return true;
		}
		public void AddPoint(Point p)
		{
			if (edges.Count > 1500 || points.Count > 1000)
				return;

			if (!MergePointInEdge(p))
			{
				bool used = false;
				used |= CreateEdgesFromPoints(p);
				// used |= CreateEdgesFromEdges(p);
				used |= ExtendEdges(p);
				if (!used)
					points.Add(p);
			}

			bool debug = true;
			debug = false;
			if (debug && edges.Count > 500 || points.Count > 500)
			{
				foreach (Edge e in edges)
					Console.WriteLine(e.ToString(true));
				throw new Exception(string.Format("Too many edges({0}) and points({1}) .", edges.Count, points.Count));
			}
		}


		#region OldCode
		/*
		 * 		bool CreateEdgesFromPoints(Point p)
		{
			// If point p is near another point q, link those into a single edge.
			bool created = false;
			for (int i = points.Count - 1; i >= 0; i--)
				if (p.DistanceTo(points[i]) < Epsilon)
				{
					Edge edge = new Edge(p, points[i]);
					points.RemoveAt(i);
					if (!MergeEdge(edge))
						edges.Add(edge);
					created = true;
				}
			return created;
		}
		bool MergeEdge(Edge edge)
		{
			// Tries to merge edge into another, if they are parralel and close.
			foreach (Edge e in edges)
				if (Util.IsEqual(e.Angle, edge.Angle, EpsilonAngle) && edge.P1.DistanceTo(e) < Epsilon)
				{
					Point h1 = edge.IntersectWithPerpendicular(e.P1);
					Point h2 = edge.IntersectWithPerpendicular(e.P2);

					// If edge and (h1, h2) do not intersect and are too far away, ignore e.
					if (!edge.Contains(h1) && !edge.Contains(h2))
					{
						if (Epsilon <= Math.Min(
								Math.Min(e.P1.DistanceTo(edge.P1), e.P1.DistanceTo(edge.P2)),
								Math.Min(e.P2.DistanceTo(edge.P1), e.P2.DistanceTo(edge.P2)))
							)							
							continue;
					}
					Point boxL1, boxL2, boxR1, boxR2;
					boxL1 = Point.Min(edge.P1, edge.P2, h1, h2);
					boxR1 = Point.Max(edge.P1, edge.P2, h1, h2);

					Point g1, g2;
					g1 = e.IntersectWithPerpendicular(edge.P1);
					g2 = e.IntersectWithPerpendicular(edge.P2);
					boxL2 = Point.Min(e.P1, e.P2, g1, g2);
					boxR2 = Point.Max(e.P1, e.P2, g1, g2);

					bool wrong = false;
					if (boxL1.DistanceTo(boxL2) > 0.5) wrong = true;
					if (boxR1.DistanceTo(boxR2) > 0.5) wrong = true;

					if (wrong)
					{
						throw new Exception("muie");
					}

					// Merging:
					double W = edge.Weight + e.Weight;
					double w1 = edge.Weight / W;
					double w2 = e.Weight / W;
					Point old1 = e.P1;
					Point old2 = e.P2;
					e.P1 = (boxL1 * w1) + (boxL2 * w2);
					e.P2 = (boxR1 * w1) + (boxR2 * w2);
					// if (Math.Max(old1.DistanceTo(e.P1), old2.DistanceTo(e.P2)) > 0.5)
					//	throw new Exception(string.Format("Muie {0}!", e));
					e.Weight = W;
					int max = 14;
					if (e.P1.X > max || e.P2.X > max || e.P2.Y > max || e.P1.Y > max)
						throw new Exception(string.Format("Muie {0}!", e));
					return true;
				}
			return false;
		}
		bool CreateEdgesFromEdges(Point p)
		{
			// Point p is near an edge, and we build a perpendicular edge from p to that edge,
			// or a new edge from p to an end of the edge.
			bool created = false;
			int i, N = edges.Count;
			Edge e;
			for (i = 0; i < N; i++)
			{
				e = edges[i];
				double dist = p.DistanceTo(e);
				// If point p is close to e, but not on e...
				if (dist < Epsilon && !Util.IsZero(dist))
				{
					Point intersection = e.IntersectWithPerpendicular(p);
					if (e.Contains(intersection))
					{
						edges.Add(new Edge(p, intersection));
						created = true;
					}
					else
						// Or just create with nearest edge end, if possible
						if (p.DistanceTo(e.P1) < Epsilon)
						{
							edges.Add(new Edge(p, e.P1));
							created = true;
						}
						else
							if (p.DistanceTo(e.P2) < Epsilon)
							{
								edges.Add(new Edge(p, e.P2));
								created = true;
							}
				}
			}
			return created;
		}
		bool ExtendEdges(Point p)
		{
			// Point p is on director vector of edge e, and we extend e to include p.
			List<Edge> extended = new List<Edge>();
			foreach (Edge e in edges)
				if (p.OnLine(e.P1, e.P2) && p != e.P1 && p != e.P2)
				{
					int where = p.Where(e.P1, e.P2);
					if (where == 1 && p.DistanceTo(e.P1) < Epsilon) 
					{
						e.P1 = p;
						extended.Add(e);
					}
					if (where == 2 && p.DistanceTo(e.P2) < Epsilon)
					{
						e.P2 = p;
						extended.Add(e);
					}
				}
			if (extended.Count == 0) return false;
			// We try to link 2 extended edges to p to form a single edge.
			Point P1, P2;
			for (int i = 0; i < extended.Count; i++)
				if (extended[i] != null)
				{
					if (extended[i].P1 == p) P1 = extended[i].P2;
					else P1 = extended[i].P1;
					for (int j = i + 1; j < extended.Count; j++)
						if (extended[j] != null)
						{
							if (extended[j].P1 == p) P2 = extended[j].P2;
							else P2 = extended[j].P1;
							if (p.OnLine(P1, P2))
							{
								// This works because extended[i] is a pointer!
								extended[i].P1 = P1;
								extended[i].P2 = P2;
								edges.Remove(extended[j]);
								extended[j] = null;
								break;
							}
						}
				}
			return true;
		}
		public void AddPoint(Point p)
		{
			if (edges.Count > 1500 || points.Count > 1000)
				return;

			bool used = false;
			used |= CreateEdgesFromPoints(p);
			// used |= CreateEdgesFromEdges(p);
			used |= ExtendEdges(p);
			if (!used)
				points.Add(p);

			bool debug = true;
			debug = false;
			if (debug && edges.Count > 500 || points.Count > 500)
			{
				foreach (Edge e in edges)
					Console.WriteLine(e.ToString(true));
				throw new Exception(string.Format("Too many edges({0}) and points({1}) .", edges.Count, points.Count));
			}
		}
		 */
		#endregion
	}
}
