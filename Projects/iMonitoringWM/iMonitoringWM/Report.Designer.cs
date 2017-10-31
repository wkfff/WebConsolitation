namespace iMonotoringWM
{
    partial class Report
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Report));
            this.webPanel = new System.Windows.Forms.Panel();
            this.pbIndicator = new System.Windows.Forms.PictureBox();
            this.lbInfoText = new System.Windows.Forms.Label();
            this.pCaption = new Resco.Controls.OutlookControls.ImageButton();
            this.imageBarListFor240x320 = new System.Windows.Forms.ImageList();
            this.imageBarListFor480x640 = new System.Windows.Forms.ImageList();
            this.imageAngleListFor240x320 = new System.Windows.Forms.ImageList();
            this.imageAngleListFor480x640 = new System.Windows.Forms.ImageList();
            this.ProgressIndicator = new ProgressIndicator.ProgressIndicator();
            this.pbTopIndicator = new System.Windows.Forms.PictureBox();
            this.webPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pCaption)).BeginInit();
            this.SuspendLayout();
            // 
            // webPanel
            // 
            this.webPanel.BackColor = System.Drawing.Color.Black;
            this.webPanel.Controls.Add(this.pbIndicator);
            this.webPanel.Controls.Add(this.lbInfoText);
            this.webPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webPanel.Location = new System.Drawing.Point(0, 31);
            this.webPanel.Name = "webPanel";
            this.webPanel.Size = new System.Drawing.Size(240, 269);
            this.webPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.webPanel_MouseMove);
            this.webPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.webPanel_MouseDown);
            this.webPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.webPanel_MouseUp);
            // 
            // pbIndicator
            // 
            this.pbIndicator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pbIndicator.Location = new System.Drawing.Point(208, 249);
            this.pbIndicator.Name = "pbIndicator";
            this.pbIndicator.Size = new System.Drawing.Size(20, 20);
            this.pbIndicator.Visible = false;
            // 
            // lbInfoText
            // 
            this.lbInfoText.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbInfoText.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.lbInfoText.ForeColor = System.Drawing.Color.Red;
            this.lbInfoText.Location = new System.Drawing.Point(0, 103);
            this.lbInfoText.Name = "lbInfoText";
            this.lbInfoText.Size = new System.Drawing.Size(240, 20);
            this.lbInfoText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbInfoText.Visible = false;
            // 
            // pCaption
            // 
            this.pCaption.AutoTransparent = false;
            this.pCaption.BackColor = System.Drawing.Color.Black;
            this.pCaption.BorderColor = System.Drawing.Color.Black;
            this.pCaption.ButtonStyle = Resco.Controls.OutlookControls.ImageButton.ButtonType.ImageButton;
            this.pCaption.Checked = false;
            this.pCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.pCaption.FocusedColor = System.Drawing.Color.Black;
            this.pCaption.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.pCaption.ForeColor = System.Drawing.Color.White;
            this.pCaption.GradientColors = new Resco.Controls.OutlookControls.GradientColor(System.Drawing.Color.LightGray, System.Drawing.Color.Black, Resco.Controls.OutlookControls.FillDirection.Vertical);
            this.pCaption.GradientColorsPressed = new Resco.Controls.OutlookControls.GradientColor(System.Drawing.Color.Black, System.Drawing.Color.LightGray, Resco.Controls.OutlookControls.FillDirection.Vertical);
            this.pCaption.ImageDefault = ((System.Drawing.Image)(resources.GetObject("pCaption.ImageDefault")));
            this.pCaption.ImageLocation = new System.Drawing.Point(-1, -1);
            this.pCaption.ImageVgaDefault = ((System.Drawing.Image)(resources.GetObject("pCaption.ImageVgaDefault")));
            this.pCaption.Location = new System.Drawing.Point(0, 0);
            this.pCaption.MaxStretchImageSize = new System.Drawing.Size(-1, -1);
            this.pCaption.Name = "pCaption";
            this.pCaption.PressedBackColor = System.Drawing.Color.Black;
            this.pCaption.PressedBorderColor = System.Drawing.Color.Black;
            this.pCaption.Size = new System.Drawing.Size(240, 31);
            this.pCaption.TabIndex = 0;
            this.pCaption.TextLocation = new System.Drawing.Point(-1, -1);
            this.pCaption.VistaButtonInflate = new System.Drawing.Size(0, 0);
            this.pCaption.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pCaption_MouseMove);
            this.pCaption.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pCaption_MouseDown);
            this.pCaption.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pCaption_MouseUp);
            // 
            // imageBarListFor240x320
            // 
            this.imageBarListFor240x320.ImageSize = new System.Drawing.Size(5, 30);
            this.imageBarListFor240x320.Images.Clear();
            this.imageBarListFor240x320.Images.Add(((System.Drawing.Image)(resources.GetObject("resource"))));
            this.imageBarListFor240x320.Images.Add(((System.Drawing.Image)(resources.GetObject("resource1"))));
            // 
            // imageBarListFor480x640
            // 
            this.imageBarListFor480x640.ImageSize = new System.Drawing.Size(10, 60);
            this.imageBarListFor480x640.Images.Clear();
            this.imageBarListFor480x640.Images.Add(((System.Drawing.Image)(resources.GetObject("resource2"))));
            this.imageBarListFor480x640.Images.Add(((System.Drawing.Image)(resources.GetObject("resource3"))));
            // 
            // imageAngleListFor240x320
            // 
            this.imageAngleListFor240x320.ImageSize = new System.Drawing.Size(20, 20);
            this.imageAngleListFor240x320.Images.Clear();
            this.imageAngleListFor240x320.Images.Add(((System.Drawing.Image)(resources.GetObject("resource4"))));
            this.imageAngleListFor240x320.Images.Add(((System.Drawing.Image)(resources.GetObject("resource5"))));
            // 
            // imageAngleListFor480x640
            // 
            this.imageAngleListFor480x640.ImageSize = new System.Drawing.Size(40, 40);
            this.imageAngleListFor480x640.Images.Clear();
            this.imageAngleListFor480x640.Images.Add(((System.Drawing.Image)(resources.GetObject("resource6"))));
            this.imageAngleListFor480x640.Images.Add(((System.Drawing.Image)(resources.GetObject("resource7"))));
            // 
            // ProgressIndicator
            // 
            this.ProgressIndicator.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ProgressIndicator.AutoScroll = true;
            this.ProgressIndicator.BackColor = System.Drawing.Color.Black;
            this.ProgressIndicator.CircleColor = System.Drawing.Color.White;
            this.ProgressIndicator.Location = new System.Drawing.Point(104, 134);
            this.ProgressIndicator.Name = "ProgressIndicator";
            this.ProgressIndicator.Size = new System.Drawing.Size(32, 32);
            this.ProgressIndicator.TabIndex = 0;
            // 
            // pbTopIndicator
            // 
            this.pbTopIndicator.Location = new System.Drawing.Point(0, 0);
            this.pbTopIndicator.Name = "pbTopIndicator";
            this.pbTopIndicator.Size = new System.Drawing.Size(5, 30);
            this.pbTopIndicator.Visible = false;
            // 
            // Report
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.pbTopIndicator);
            this.Controls.Add(this.ProgressIndicator);
            this.Controls.Add(this.webPanel);
            this.Controls.Add(this.pCaption);
            this.Name = "Report";
            this.Size = new System.Drawing.Size(240, 300);
            this.webPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pCaption)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel webPanel;
        private Resco.Controls.OutlookControls.ImageButton pCaption;
        private System.Windows.Forms.ImageList imageBarListFor240x320;
        private System.Windows.Forms.ImageList imageBarListFor480x640;
        private System.Windows.Forms.Label lbInfoText;
        public ProgressIndicator.ProgressIndicator ProgressIndicator;
        private System.Windows.Forms.PictureBox pbIndicator;
        private System.Windows.Forms.ImageList imageAngleListFor240x320;
        private System.Windows.Forms.ImageList imageAngleListFor480x640;
        private System.Windows.Forms.PictureBox pbTopIndicator;


    }
}
