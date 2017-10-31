namespace Krista.FM.Client.iMonitoringWM.Controls
{
    partial class SettingsView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsView));
            this.mainControlsPanel = new System.Windows.Forms.Panel();
            this.btChangeEntity = new Resco.Controls.OutlookControls.ImageButton();
            this.reportListCaption = new Krista.FM.Client.iMonitoringWM.Controls.GradientPanel();
            this.scrollReportList = new Krista.FM.Client.iMonitoringWM.Controls.ScrollList();
            this.imageListFor480x640 = new System.Windows.Forms.ImageList();
            this.imageListFor240x320 = new System.Windows.Forms.ImageList();
            this.grandPanel = new System.Windows.Forms.Panel();
            this.btApply = new Resco.Controls.OutlookControls.ImageButton();
            this.pCaption = new Resco.Controls.OutlookControls.ImageButton();
            this.mainControlsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btChangeEntity)).BeginInit();
            this.grandPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btApply)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pCaption)).BeginInit();
            this.SuspendLayout();
            // 
            // mainControlsPanel
            // 
            this.mainControlsPanel.BackColor = System.Drawing.Color.Black;
            this.mainControlsPanel.Controls.Add(this.btChangeEntity);
            this.mainControlsPanel.Controls.Add(this.reportListCaption);
            this.mainControlsPanel.Controls.Add(this.scrollReportList);
            this.mainControlsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainControlsPanel.Location = new System.Drawing.Point(0, 30);
            this.mainControlsPanel.Name = "mainControlsPanel";
            this.mainControlsPanel.Size = new System.Drawing.Size(240, 280);
            // 
            // btChangeEntity
            // 
            this.btChangeEntity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btChangeEntity.BackColor = System.Drawing.Color.Black;
            this.btChangeEntity.BorderColor = System.Drawing.Color.DarkGray;
            this.btChangeEntity.ButtonStyle = Resco.Controls.OutlookControls.ImageButton.ButtonType.ImageButton;
            this.btChangeEntity.Checked = false;
            this.btChangeEntity.FocusedColor = System.Drawing.Color.Black;
            this.btChangeEntity.Font = new System.Drawing.Font("Tahoma", 12.7F, System.Drawing.FontStyle.Bold);
            this.btChangeEntity.GradientColors = new Resco.Controls.OutlookControls.GradientColor(System.Drawing.Color.LightGray, System.Drawing.Color.Black, Resco.Controls.OutlookControls.FillDirection.Vertical);
            this.btChangeEntity.GradientColorsPressed = new Resco.Controls.OutlookControls.GradientColor(System.Drawing.Color.Black, System.Drawing.Color.LightGray, Resco.Controls.OutlookControls.FillDirection.Vertical);
            this.btChangeEntity.ImageDefault = ((System.Drawing.Image)(resources.GetObject("btChangeEntity.ImageDefault")));
            this.btChangeEntity.ImageLocation = new System.Drawing.Point(-1, -1);
            this.btChangeEntity.Location = new System.Drawing.Point(5, 3);
            this.btChangeEntity.MaxStretchImageSize = new System.Drawing.Size(-1, -1);
            this.btChangeEntity.Name = "btChangeEntity";
            this.btChangeEntity.PressedBackColor = System.Drawing.Color.Black;
            this.btChangeEntity.PressedBorderColor = System.Drawing.Color.Black;
            this.btChangeEntity.Size = new System.Drawing.Size(230, 30);
            this.btChangeEntity.StretchImage = true;
            this.btChangeEntity.TabIndex = 4;
            this.btChangeEntity.TextDefault = "Субъекты";
            this.btChangeEntity.TextLocation = new System.Drawing.Point(-1, -1);
            this.btChangeEntity.VistaButtonInflate = new System.Drawing.Size(-4, -5);
            this.btChangeEntity.Click += new System.EventHandler(this.btChangeEntity_Click);
            // 
            // reportListCaption
            // 
            this.reportListCaption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.reportListCaption.BackColor = System.Drawing.Color.Black;
            this.reportListCaption.BorderColor = System.Drawing.Color.Gray;
            this.reportListCaption.EndColor = System.Drawing.Color.Silver;
            this.reportListCaption.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.reportListCaption.ForeColor = System.Drawing.Color.White;
            this.reportListCaption.Location = new System.Drawing.Point(0, 38);
            this.reportListCaption.Name = "reportListCaption";
            this.reportListCaption.Size = new System.Drawing.Size(240, 22);
            this.reportListCaption.StartColor = System.Drawing.Color.Silver;
            this.reportListCaption.TabIndex = 3;
            this.reportListCaption.Text = "  Отчеты";
            // 
            // scrollReportList
            // 
            this.scrollReportList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.scrollReportList.BackColor = System.Drawing.Color.Black;
            this.scrollReportList.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.scrollReportList.ForeColor = System.Drawing.Color.White;
            this.scrollReportList.ImageList = this.imageListFor480x640;
            this.scrollReportList.Location = new System.Drawing.Point(0, 58);
            this.scrollReportList.Name = "scrollReportList";
            this.scrollReportList.Size = new System.Drawing.Size(240, 222);
            this.scrollReportList.TabIndex = 0;
            this.scrollReportList.VOffset = 0;
            // 
            // imageListFor480x640
            // 
            this.imageListFor480x640.ImageSize = new System.Drawing.Size(40, 40);
            this.imageListFor480x640.Images.Clear();
            this.imageListFor480x640.Images.Add(((System.Drawing.Image)(resources.GetObject("resource"))));
            this.imageListFor480x640.Images.Add(((System.Drawing.Image)(resources.GetObject("resource1"))));
            // 
            // imageListFor240x320
            // 
            this.imageListFor240x320.ImageSize = new System.Drawing.Size(20, 20);
            this.imageListFor240x320.Images.Clear();
            this.imageListFor240x320.Images.Add(((System.Drawing.Image)(resources.GetObject("resource2"))));
            this.imageListFor240x320.Images.Add(((System.Drawing.Image)(resources.GetObject("resource3"))));
            // 
            // grandPanel
            // 
            this.grandPanel.BackColor = System.Drawing.Color.Black;
            this.grandPanel.Controls.Add(this.btApply);
            this.grandPanel.Controls.Add(this.mainControlsPanel);
            this.grandPanel.Controls.Add(this.pCaption);
            this.grandPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grandPanel.Location = new System.Drawing.Point(0, 0);
            this.grandPanel.Name = "grandPanel";
            this.grandPanel.Size = new System.Drawing.Size(240, 310);
            // 
            // btApply
            // 
            this.btApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btApply.AutoTransparent = false;
            this.btApply.BackColor = System.Drawing.Color.Black;
            this.btApply.BorderColor = System.Drawing.Color.Black;
            this.btApply.ButtonStyle = Resco.Controls.OutlookControls.ImageButton.ButtonType.ImageButton;
            this.btApply.Checked = false;
            this.btApply.FocusedColor = System.Drawing.Color.Black;
            this.btApply.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.btApply.ForeColor = System.Drawing.Color.White;
            this.btApply.GradientColors = new Resco.Controls.OutlookControls.GradientColor(System.Drawing.Color.LightGray, System.Drawing.Color.Black, Resco.Controls.OutlookControls.FillDirection.Vertical);
            this.btApply.GradientColorsPressed = new Resco.Controls.OutlookControls.GradientColor(System.Drawing.Color.Black, System.Drawing.Color.LightGray, Resco.Controls.OutlookControls.FillDirection.Vertical);
            this.btApply.ImageDefault = ((System.Drawing.Image)(resources.GetObject("btApply.ImageDefault")));
            this.btApply.ImageLocation = new System.Drawing.Point(-1, -1);
            this.btApply.ImagePressed = ((System.Drawing.Image)(resources.GetObject("btApply.ImagePressed")));
            this.btApply.ImageVgaDefault = ((System.Drawing.Image)(resources.GetObject("btApply.ImageVgaDefault")));
            this.btApply.ImageVgaPressed = ((System.Drawing.Image)(resources.GetObject("btApply.ImageVgaPressed")));
            this.btApply.Location = new System.Drawing.Point(170, 0);
            this.btApply.MaxStretchImageSize = new System.Drawing.Size(-1, -1);
            this.btApply.Name = "btApply";
            this.btApply.PressedBackColor = System.Drawing.Color.PowderBlue;
            this.btApply.PressedBorderColor = System.Drawing.Color.Black;
            this.btApply.Size = new System.Drawing.Size(70, 30);
            this.btApply.TabIndex = 5;
            this.btApply.TextDefault = "Применить";
            this.btApply.TextLocation = new System.Drawing.Point(-1, -1);
            this.btApply.VistaButtonInflate = new System.Drawing.Size(-4, -4);
            this.btApply.Click += new System.EventHandler(this.btApply_Click);
            // 
            // pCaption
            // 
            this.pCaption.AutoTransparent = false;
            this.pCaption.BackColor = System.Drawing.Color.Black;
            this.pCaption.BorderColor = System.Drawing.Color.Black;
            this.pCaption.ButtonStyle = Resco.Controls.OutlookControls.ImageButton.ButtonType.PictureBox;
            this.pCaption.Checked = false;
            this.pCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.pCaption.FocusedColor = System.Drawing.Color.Empty;
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
            this.pCaption.PressedBackColor = System.Drawing.Color.PowderBlue;
            this.pCaption.PressedBorderColor = System.Drawing.Color.SteelBlue;
            this.pCaption.Size = new System.Drawing.Size(240, 30);
            this.pCaption.TabIndex = 4;
            this.pCaption.TextDefault = "Настройки";
            this.pCaption.TextLocation = new System.Drawing.Point(-1, -1);
            this.pCaption.VistaButtonInflate = new System.Drawing.Size(0, 0);
            // 
            // SettingsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.grandPanel);
            this.Name = "SettingsView";
            this.Size = new System.Drawing.Size(240, 310);
            this.mainControlsPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btChangeEntity)).EndInit();
            this.grandPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btApply)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pCaption)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainControlsPanel;
        private ScrollList scrollReportList;
        private System.Windows.Forms.ImageList imageListFor240x320;
        private System.Windows.Forms.ImageList imageListFor480x640;
        private System.Windows.Forms.Panel grandPanel;
        private GradientPanel reportListCaption;
        private Resco.Controls.OutlookControls.ImageButton pCaption;
        private Resco.Controls.OutlookControls.ImageButton btApply;
        private Resco.Controls.OutlookControls.ImageButton btChangeEntity;

    }
}
