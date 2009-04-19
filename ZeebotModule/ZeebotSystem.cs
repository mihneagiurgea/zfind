using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZFind.MapModule;

namespace ZFind.ZeebotModule
{
	public class ZeebotSystem : IPaintable
	{
		// Required for testing
		public MapContext MapContext { get { return mapContext; } }
		public CoordinateSystem CoordinateSystem { get { return coordinateSystem; } }
		public List<Zeebot> Zeebots { get { return zeebots; } }

		// I don't think wee need to store the mapContext pointer.
		MapContext mapContext;
		CoordinateSystem coordinateSystem;
		List<Zeebot> zeebots;

		public ZeebotSystem(MapContext mapContext) : this(mapContext, 0) { }
		public ZeebotSystem(MapContext mapContext, int nrZeebots)
		{
			this.mapContext = mapContext;
			coordinateSystem = new CoordinateSystem();
			zeebots = new List<Zeebot>(nrZeebots);
			for (int i = 0; i < nrZeebots; i++)
			{
				Zeebot zeebot = new Zeebot();
				zeebots.Add(zeebot);

				// Add to real map.
				zeebot.MapContext = mapContext;
				mapContext.AddZeebot(zeebot);

				// Add to calculated map.
				zeebot.CoordinateSystem = coordinateSystem;
				coordinateSystem.AddZeebot(zeebot);
			}
		}

		public void Run()
		{
			foreach (Zeebot zeebot in zeebots)
			{
				zeebot.Discovery.Run();
			}
		}
		
		#region Painting

		float ratio;
		float padding = 3.0f;
		Point min, max;

		System.Drawing.PointF PointToScreen(Point pt)
		{
			return new System.Drawing.PointF(padding + (float)(pt.X - min.X) * ratio,
											 padding + (float)(max.Y - pt.Y) * ratio);
		}

		public void Paint(System.Drawing.Graphics grfx, float width, float height)
		{
			coordinateSystem.ComputeMinMaxBox(out min, out max);

			width -= 2 * padding;
			height -= 2 * padding;
			ratio = (float)Math.Min(width / (max.X - min.X), height / (max.Y - min.Y));

			foreach (Point point in coordinateSystem.PartialMap.Points)
			{
				System.Drawing.SizeF sz = new System.Drawing.SizeF(1, 1);
				System.Drawing.PointF pt = PointToScreen(point);
				pt.X -= sz.Width / 2;
				pt.Y -= sz.Height / 2;
				System.Drawing.RectangleF rect = new System.Drawing.RectangleF(pt, sz);
				grfx.FillPie(System.Drawing.Brushes.Gray, pt.X, pt.Y, sz.Width, sz.Height, 0, 360);
			}
			foreach (Edge edge in coordinateSystem.PartialMap.Edges)
				grfx.DrawLine(System.Drawing.Pens.Black, PointToScreen(edge.P1), PointToScreen(edge.P2));
			foreach (Zeebot zeebot in zeebots)
			{
				System.Drawing.SizeF sz = new System.Drawing.SizeF(4, 4);
				System.Drawing.PointF pt = PointToScreen(zeebot.Location);
				pt.X -= sz.Width / 2;
				pt.Y -= sz.Height / 2;
				System.Drawing.RectangleF rect = new System.Drawing.RectangleF(pt, sz);
				grfx.FillPie(System.Drawing.Brushes.Green, pt.X, pt.Y, sz.Width, sz.Height, 0, 360);
			}
		}

		#endregion
	}
}
