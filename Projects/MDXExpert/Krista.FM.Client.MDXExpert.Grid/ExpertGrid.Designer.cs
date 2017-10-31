namespace Krista.FM.Client.MDXExpert.Grid
{
    partial class ExpertGrid
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExpertGrid));
            this.gridPlace = new Krista.FM.Client.MDXExpert.Grid.RichPanel();
            this.hScrollBar = new Infragistics.Win.UltraWinScrollBar.UltraScrollBar();
            this.filtersScrollBar = new Infragistics.Win.UltraWinScrollBar.UltraScrollBar();
            this.vScrollBar = new Infragistics.Win.UltraWinScrollBar.UltraScrollBar();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.gridPlace.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hScrollBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.filtersScrollBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vScrollBar)).BeginInit();
            this.SuspendLayout();
            // 
            // gridPlace
            // 
            this.gridPlace.BackColor = System.Drawing.Color.Transparent;
            this.gridPlace.Controls.Add(this.hScrollBar);
            this.gridPlace.Controls.Add(this.filtersScrollBar);
            this.gridPlace.Controls.Add(this.vScrollBar);
            this.gridPlace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPlace.Location = new System.Drawing.Point(0, 0);
            this.gridPlace.MinimumSize = new System.Drawing.Size(200, 0);
            this.gridPlace.Name = "gridPlace";
            this.gridPlace.Size = new System.Drawing.Size(869, 619);
            this.gridPlace.TabIndex = 8;
            this.gridPlace.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.gridPlace_MouseWheel);
            this.gridPlace.Paint += new System.Windows.Forms.PaintEventHandler(this.gridPlace_Paint);
            this.gridPlace.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.gridPlace_PreviewKeyDown);
            this.gridPlace.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gridPlace_MouseMove);
            this.gridPlace.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gridPlace_MouseClick);
            this.gridPlace.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridPlace_MouseDown);
            this.gridPlace.Resize += new System.EventHandler(this.gridPlace_Resize);
            this.gridPlace.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridPlace_MouseUp);
            this.gridPlace.SizeChanged += new System.EventHandler(this.gridPlace_SizeChanged);
            // 
            // hScrollBar
            // 
            this.hScrollBar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.hScrollBar.Location = new System.Drawing.Point(657, 603);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.NonAutoSize = new System.Drawing.Size(80, 16);
            this.hScrollBar.ScrollBarInfo.Enabled = true;
            this.hScrollBar.ScrollBarInfo.LargeChange = 100;
            this.hScrollBar.ScrollBarInfo.SetFocusOnClick = true;
            this.hScrollBar.ScrollBarInfo.SmallChange = 20;
            this.hScrollBar.Size = new System.Drawing.Size(28, 16);
            this.hScrollBar.TabIndex = 6;
            this.hScrollBar.ValueChanged += new System.EventHandler(this.hScrollBar_ValueChanged);
            // 
            // filtersScrollBar
            // 
            this.filtersScrollBar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.filtersScrollBar.Location = new System.Drawing.Point(784, 3);
            this.filtersScrollBar.Name = "filtersScrollBar";
            this.filtersScrollBar.NonAutoSize = new System.Drawing.Size(80, 16);
            this.filtersScrollBar.ScrollBarInfo.Enabled = true;
            this.filtersScrollBar.ScrollBarInfo.LargeChange = 101;
            this.filtersScrollBar.ScrollBarInfo.SetFocusOnClick = true;
            this.filtersScrollBar.ScrollBarInfo.SmallChange = 20;
            this.filtersScrollBar.Size = new System.Drawing.Size(28, 16);
            this.filtersScrollBar.TabIndex = 4;
            this.filtersScrollBar.ValueChanged += new System.EventHandler(this.filtersScrollBar_ValueChanged);
            // 
            // vScrollBar
            // 
            this.vScrollBar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.vScrollBar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.vScrollBar.Location = new System.Drawing.Point(850, 260);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.NonAutoSize = new System.Drawing.Size(16, 80);
            this.vScrollBar.ScrollBarInfo.Enabled = true;
            this.vScrollBar.ScrollBarInfo.LargeChange = 101;
            this.vScrollBar.ScrollBarInfo.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.vScrollBar.ScrollBarInfo.SetFocusOnClick = true;
            this.vScrollBar.ScrollBarInfo.SmallChange = 5;
            this.vScrollBar.Size = new System.Drawing.Size(16, 0);
            this.vScrollBar.TabIndex = 5;
            this.vScrollBar.ValueChanged += new System.EventHandler(this.vScrollBar_ValueChanged);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Copy.bmp");
            this.imageList.Images.SetKeyName(1, "FiltersMenu.bmp");
            this.imageList.Images.SetKeyName(2, "Check.bmp");
            this.imageList.Images.SetKeyName(3, "detalise_tr.gif");
            this.imageList.Images.SetKeyName(4, "hide_trans.gif");
            // 
            // ExpertGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.gridPlace);
            this.Name = "ExpertGrid";
            this.Size = new System.Drawing.Size(869, 619);
            this.gridPlace.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.hScrollBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.filtersScrollBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vScrollBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private RichPanel gridPlace;
        private Infragistics.Win.UltraWinScrollBar.UltraScrollBar hScrollBar;
        private Infragistics.Win.UltraWinScrollBar.UltraScrollBar filtersScrollBar;
        private Infragistics.Win.UltraWinScrollBar.UltraScrollBar vScrollBar;
        public System.Windows.Forms.ImageList imageList;

    }
}
