using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZFind.CoreModule
{
	static class Util
	{
		public const double Epsilon = 0.001;

		public static bool IsZero(double nr)
		{
			return Math.Abs(nr) <= Epsilon;
		}

		public static bool IsEqual(double a, double b)
		{
			return IsZero(a - b);
		}
		public static bool IsEqual(double a, double b, double epsilon)
		{
			if (double.IsNaN(a) || double.IsNaN(b))
			{
				return double.IsNaN(a) && double.IsNaN(b);
			}
			else
				return Math.Abs(a - b) <= epsilon;
		}

		public static bool IsPositiveOrZero(double nr)
		{
			return IsZero(nr) || nr > 0;
		}

		public static bool IsPositive(double nr)
		{
			return nr > Epsilon;
		}

		public static int Sign(double nr)
		{
			if (IsZero(nr)) return 0;
			else
				if (nr > 0) return 1;
				else return -1;
		}
		#region Random

		static Random random = null;
		public static int seed = 47;
		public static Random Random
		{
			get
			{
				if (random == null)
					random = new Random(seed);
				return random;
			}
		}

		#endregion

		public static double ModPI(double angle)
		{
			while (angle <= -Math.PI) angle += 2 * Math.PI;
			while (angle > Math.PI) angle -= 2 * Math.PI;
			return angle;
		}
	}
}
