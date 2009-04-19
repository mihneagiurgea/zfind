using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Drawing;
using ZFind.CoreModule;

namespace ZFind.MapModule
{
	public interface IZeebot
	{
		int Id { get; set; }
		MapContext MapContext { get; set; }
	}

	public interface IPaintable
	{
		void Paint(Graphics grfx, float width, float height);
	}

	public class MapContext : IPaintable
	{
		Map map;
		Dictionary<IZeebot, MovingPoint> zeebots = new Dictionary<IZeebot, MovingPoint>();
		public Dictionary<IZeebot, MovingPoint> Zeebots { get { return zeebots; } }

		public const double LaserDistance = 25.0;
		public const double TargetVisibility = 1.0;

		public MapContext(Map map)
		{
			this.map = map;
		}

		MovingPoint GetRandomPoint()
		{
			Point p = new Point(), min, max;
			map.ComputeMinMaxBox(out min, out max);
			do
			{
				p.X = min.X + Util.Random.NextDouble() * (max.X - min.X);
				p.Y = min.Y + Util.Random.NextDouble() * (max.Y - min.Y);
			} while (!map.IsFree(p));
			return new MovingPoint(p, Util.Random.NextDouble() * 2 * Math.PI);
		}

		public void AddZeebotAt(IZeebot zeebot, MovingPoint position)
		{
			zeebot.Id = zeebots.Count;
			zeebot.MapContext = this;
			zeebots[zeebot] = position;
		}
		public void AddZeebot(IZeebot zeebot)
		{
			zeebot.Id = zeebots.Count;
			zeebot.MapContext = this;
			zeebots[zeebot] = GetRandomPoint();
		}

		/// <summary>
		/// Notifies the MapContext that a zeebot will change its facing, by a certain value.
		/// </summary>
		/// <param name="zeebot">The zeebot that is chaning facing.</param>
		/// <param name="value">The increment value with which the facing is changing (positive means
		/// counter-clockwise rotation). </param>
		/// </param>
		public void ChangeFacing(IZeebot zeebot, double value)
		{
			zeebots[zeebot].Facing += value;
		}

		/// <summary>
		/// Notifies the MapContext that a zeebot will try to move forward a certain value.
		/// </summary>
		/// <param name="zeebot">The zeebot that is attempting to move.</param>
		/// <param name="value">The value with which the zeebots tries to move forward. </param>
		/// <param name="success">How much of the value did the zeebot succeed to move forward.
		/// </param>
		/// <returns>Whether the move succeeded or not.</returns>
		public bool TryMove(IZeebot zeebot, double value, out double success)
		{
			MovingPoint mp = zeebots[zeebot];
			Point end = new Point();
			end.X = mp.Location.X + value * Math.Cos(mp.Facing);
			end.Y = mp.Location.Y + value * Math.Sin(mp.Facing);

			bool result = map.TryMove(mp.Location, end, out success);
			end.X = mp.Location.X + success * Math.Cos(mp.Facing);
			end.Y = mp.Location.Y + success * Math.Sin(mp.Facing);
			mp.Location = end;
			if (!map.InMinMaxBox(mp.Location))
				throw new Exception(string.Format("Not in min-max at: {0}", mp.Location));
			return result;
		}

		/// <summary>
		/// A zeebot shoots its laser beam in front of him, and asks the MapContext at 
		/// what distance is an obstacle in front of him, if any. Returns the distance to 
		/// the first obstacle in front of the zeebot, or NaN if no such object exist in the 
		/// range of the laser's distance (the laser expired).
		/// </summary>
		/// <param name="zeebot"> The zeebot that is performing the query.</param>
		/// <returns> The distance to the obstacle in front of the zeebot, or NaN if the laser 
		/// expired. </returns>
		public double QueryLaser(IZeebot zeebot)
		{
			MovingPoint mp = zeebots[zeebot];
			Point end = new Point();
			end.X = mp.Location.X + LaserDistance * Math.Cos(mp.Facing);
			end.Y = mp.Location.Y + LaserDistance * Math.Sin(mp.Facing);

			double result;
			if (map.CanMove(mp.Location, end, out result))
				return double.NaN;
			else
				return result;
		}

		public double QueryZeebotDistance(IZeebot zeebot, IZeebot otherZeebot)
		{
			Point other = zeebots[otherZeebot].Location;
			return zeebots[zeebot].Location.DistanceTo(other);
		}

		/* Removed functionality - Targets
		/// <summary>
		/// A zeebot asks if there are any visible targets nears him. Returns the distance to 
		/// the nearest target, or NaN if no targets exists around the zeebot.
		/// </summary>
		/// <param name="zeebot"> The zeebot that is performing the query.</param>
		/// <returns> The distance to the nearest target around the zeebot, or NaN if 
		/// there is none around. </returns>
		public double QueryTarget(IZeebot zeebot)
		{
			return map.FindTarget(zeebots[zeebot].Location, TargetVisibility);
		}
		*/

		#region Painting

		float ratio;
		float padding = 3.0f;
		Point min, max;

		PointF PointToScreen(Point pt)
		{
			return new PointF(padding + (float)(pt.X - min.X) * ratio,
							  padding + (float)(max.Y - pt.Y) * ratio);
		}

		public void Paint(Graphics grfx, float width, float height)
		{
			map.ComputeMinMaxBox(out min, out max);

			width -= 2 * padding;
			height -= 2 * padding;
			ratio = (float)Math.Min(width / (max.X - min.X), height / (max.Y - min.Y));

			foreach (Edge edge in map.Edges)
				grfx.DrawLine(Pens.Black, PointToScreen(edge.P1), PointToScreen(edge.P2));
			foreach (MovingPoint position in zeebots.Values)
			{
				Size sz = new Size(4, 4);
				PointF pt = PointToScreen(position.Location);
				pt.X -= sz.Width / 2;
				pt.Y -= sz.Height / 2;
				RectangleF rect = new RectangleF(pt, sz);
				grfx.FillPie(System.Drawing.Brushes.Green, pt.X, pt.Y, sz.Width, sz.Height, 0, 360);
			}
		}

		#endregion
	}
}
