using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZFind.MapModule;

namespace ZFind
{
	public partial class MapPainter : UserControl
	{
		public MapPainter()
		{
			InitializeComponent();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (ToPaint != null)
				ToPaint.Paint(e.Graphics, Width, Height);
		}

		public IPaintable ToPaint { get; set; }
	}
}
