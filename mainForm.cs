using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZFind.MapModule;
using ZFind.ZeebotModule;

namespace ZFind
{
	public partial class mainForm : Form
	{
		MapContext mapContext;
		ZeebotSystem zeebotSystem;

		public mainForm()
		{
			InitializeComponent();

			mapContext = new MapContext(MapBuilder.Build("Fixtures/map.in"));
			zeebotSystem = new ZeebotSystem(mapContext, 6);

			realMap.ToPaint = mapContext;
			calculatedMap.ToPaint = zeebotSystem;

			timer.Interval = 250;
			timer.Start();
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			if (!playing) return;
			zeebotSystem.Run();

			realMap.Refresh();
			realMap.Invalidate();

			calculatedMap.Refresh();
			calculatedMap.Invalidate();
		}

		bool playing = false;
		int speed = 3;
		int[] speedToInterval = new int[] { 750, 500, 300, 200, 100 };

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			if (playing)
			{
				playing = false;
				playButton.Text = "Play";
			}
			else
			{
				playing = true;
				playButton.Text = "Pause";
			}
		}

		void AlterSpeed(bool faster)
		{
			if (faster) speed++;
			else speed--;
			if (speed < 1) speed = 1;
			if (speed > 5) speed = 5;
			speedLabel.Text = string.Format("Speed: {0}", speed);
			timer.Interval = speedToInterval[speed - 1];
		}
		private void slowerButton_Click(object sender, EventArgs e)
		{
			AlterSpeed(false);
		}
		private void fasterButton_Click(object sender, EventArgs e)
		{
			AlterSpeed(true);
		}

	}
}
