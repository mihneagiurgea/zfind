namespace ZFind
{
	partial class mainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainForm));
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.realMap = new ZFind.MapPainter();
			this.calculatedMap = new ZFind.MapPainter();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.playButton = new System.Windows.Forms.ToolStripButton();
			this.slowerButton = new System.Windows.Forms.ToolStripButton();
			this.fasterButton = new System.Windows.Forms.ToolStripButton();
			this.speedLabel = new System.Windows.Forms.ToolStripLabel();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.toolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer
			// 
			this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer.Location = new System.Drawing.Point(12, 28);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.realMap);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.calculatedMap);
			this.splitContainer.Size = new System.Drawing.Size(591, 479);
			this.splitContainer.SplitterDistance = 316;
			this.splitContainer.SplitterWidth = 11;
			this.splitContainer.TabIndex = 1;
			// 
			// realMap
			// 
			this.realMap.BackColor = System.Drawing.Color.WhiteSmoke;
			this.realMap.Dock = System.Windows.Forms.DockStyle.Fill;
			this.realMap.Location = new System.Drawing.Point(0, 0);
			this.realMap.Name = "realMap";
			this.realMap.Size = new System.Drawing.Size(316, 479);
			this.realMap.TabIndex = 0;
			this.realMap.ToPaint = null;
			// 
			// calculatedMap
			// 
			this.calculatedMap.BackColor = System.Drawing.Color.WhiteSmoke;
			this.calculatedMap.Dock = System.Windows.Forms.DockStyle.Fill;
			this.calculatedMap.Location = new System.Drawing.Point(0, 0);
			this.calculatedMap.Name = "calculatedMap";
			this.calculatedMap.Size = new System.Drawing.Size(264, 479);
			this.calculatedMap.TabIndex = 0;
			this.calculatedMap.ToPaint = null;
			// 
			// timer
			// 
			this.timer.Interval = 250;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// toolStrip
			// 
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playButton,
            this.slowerButton,
            this.fasterButton,
            this.speedLabel});
			this.toolStrip.Location = new System.Drawing.Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new System.Drawing.Size(615, 25);
			this.toolStrip.TabIndex = 2;
			this.toolStrip.Text = "toolStrip1";
			// 
			// playButton
			// 
			this.playButton.Image = ((System.Drawing.Image)(resources.GetObject("playButton.Image")));
			this.playButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.playButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.playButton.Name = "playButton";
			this.playButton.Size = new System.Drawing.Size(49, 22);
			this.playButton.Text = "Play";
			this.playButton.Click += new System.EventHandler(this.toolStripButton1_Click);
			// 
			// slowerButton
			// 
			this.slowerButton.Image = ((System.Drawing.Image)(resources.GetObject("slowerButton.Image")));
			this.slowerButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.slowerButton.Name = "slowerButton";
			this.slowerButton.Size = new System.Drawing.Size(62, 22);
			this.slowerButton.Text = "Slower";
			this.slowerButton.Click += new System.EventHandler(this.slowerButton_Click);
			// 
			// fasterButton
			// 
			this.fasterButton.Image = ((System.Drawing.Image)(resources.GetObject("fasterButton.Image")));
			this.fasterButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.fasterButton.Name = "fasterButton";
			this.fasterButton.Size = new System.Drawing.Size(58, 22);
			this.fasterButton.Text = "Faster";
			this.fasterButton.Click += new System.EventHandler(this.fasterButton_Click);
			// 
			// speedLabel
			// 
			this.speedLabel.Name = "speedLabel";
			this.speedLabel.Size = new System.Drawing.Size(51, 22);
			this.speedLabel.Text = "Speed: 3";
			// 
			// mainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(615, 519);
			this.Controls.Add(this.toolStrip);
			this.Controls.Add(this.splitContainer);
			this.Name = "mainForm";
			this.Text = "ZFind";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private MapPainter realMap;
		private System.Windows.Forms.SplitContainer splitContainer;
		private MapPainter calculatedMap;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.ToolStripButton playButton;
		private System.Windows.Forms.ToolStripButton slowerButton;
		private System.Windows.Forms.ToolStripLabel speedLabel;
		private System.Windows.Forms.ToolStripButton fasterButton;


	}
}