using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZFind.MapModule;

namespace ZFind.ZeebotModule
{
	public class Zeebot : IZeebot
	{
		public Discovery Discovery { get; set; }

		public Zeebot()
		{
			Discovery = new Discovery(this);
			Initialized = false;
		}

		#region CoordinateSystem Shortcuts

		public bool Initialized { get; set; }
		public CoordinateSystem CoordinateSystem { get; set; }
		public Point Location
		{
			get
			{
				return CoordinateSystem.Zeebots[this].Location;
			}
			set
			{
				CoordinateSystem.Zeebots[this].Location = value;
			}
		}
		public double Facing
		{
			get
			{
				return CoordinateSystem.Zeebots[this].Facing;
			}
			set
			{
				CoordinateSystem.Zeebots[this].Facing = value;
			}
		}

		#endregion

		#region MapContext & Shortcuts

		public MapContext MapContext { get; set; }
		public double DistanceTo(Zeebot zeebot)
		{
			return MapContext.QueryZeebotDistance(this, zeebot);
		}
		public void ChangeFacing(double value)
		{
			MapContext.ChangeFacing(this, value);
			if (Initialized)
				Facing += CoordinateSystem.Sign * value;
		}
		public bool TryMove(double value, out double success)
		{
			bool result = MapContext.TryMove(this, value, out success);
			if (Initialized)
			{
				Point end = new Point();
				end.X = Location.X + success * Math.Cos(Facing);
				end.Y = Location.Y + success * Math.Sin(Facing);
				Location = end;
			}
			return result;
		}
		public double QueryLaser()
		{
			return MapContext.QueryLaser(this);
		}

		#endregion

		#region Id Region

		int _id = 0;
		public int Id
		{
			get
			{
				return _id;
			}
			set
			{
				if (_id == 0) _id = value;
				else throw new Exception("Cannot reassign Id.");
			}
		}

		#endregion

		public override string ToString()
		{
			string real = MapContext.Zeebots[this].ToString();
			return string.Format("Zeebot #{0} @ {1} facing {2:0.00}JI | Real : {3}", Id, Location, Facing / Math.PI, real);
		}
	}
}
