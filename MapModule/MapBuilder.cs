using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ZFind.MapModule
{
	public static class MapBuilder
	{
		public static Map Build(string filename)
		{
			List<Edge> edges = new List<Edge>();
			Point last, current;

			TextReader reader = new StreamReader(filename);
			string[] lines = reader.ReadToEnd().Split(new char[]{'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries), nrs;
			foreach (string line in lines)
			{
				nrs = line.Split(' ');
				last = new Point(double.Parse(nrs[0]), double.Parse(nrs[1]));
				if (nrs.Length >= 4)
					// Add as edges
					for (int i = 2; i < nrs.Length; i += 2)
					{
						current = new Point(double.Parse(nrs[i]), double.Parse(nrs[i + 1]));
						edges.Add(new Edge(last, current));
						last = current;
					}
			}
			reader.Close();

			return new Map(edges);
		}
	}
}
