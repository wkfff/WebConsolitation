namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Navigation
{
	partial class EntityNavigationControl
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
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EntityNavigationControl));
            this.ultraExplorerBar = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ultraExplorerBar)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraExplorerBar
            // 
            this.ultraExplorerBar.Dock = System.Windows.Forms.DockStyle.Fill;
            ultraExplorerBarGroup1.Text = "New Group";
            this.ultraExplorerBar.Groups.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup[] {
            ultraExplorerBarGroup1});
            this.ultraExplorerBar.GroupSettings.AllowDrag = Infragistics.Win.DefaultableBoolean.False;
            this.ultraExplorerBar.GroupSettings.AllowItemDrop = Infragistics.Win.DefaultableBoolean.False;
            this.ultraExplorerBar.GroupSettings.BorderStyleItemArea = Infragistics.Win.UIElementBorderStyle.None;
            this.ultraExplorerBar.GroupSettings.HeaderVisible = Infragistics.Win.DefaultableBoolean.False;
            this.ultraExplorerBar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ultraExplorerBar.ItemSettings.AllowDragCopy = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            this.ultraExplorerBar.ItemSettings.AllowDragMove = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            this.ultraExplorerBar.Location = new System.Drawing.Point(0, 0);
            this.ultraExplorerBar.Name = "ultraExplorerBar";
            this.ultraExplorerBar.ShowDefaultContextMenu = false;
            this.ultraExplorerBar.Size = new System.Drawing.Size(216, 472);
            this.ultraExplorerBar.Style = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarStyle.VisualStudio2005Toolbox;
            this.ultraExplorerBar.TabIndex = 0;
            this.ultraExplorerBar.ViewStyle = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarViewStyle.Office2000;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Magenta;
            this.imageList.Images.SetKeyName(0, "Fixedcls.bmp");
            this.imageList.Images.SetKeyName(1, "DataCls.bmp");
            this.imageList.Images.SetKeyName(2, "Bridge_Cls.bmp");
            this.imageList.Images.SetKeyName(3, "Facttable.bmp");
            // 
            // EntityNavigationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraExplorerBar);
            this.Name = "EntityNavigationControl";
            ((System.ComponentModel.ISupportInitialize)(this.ultraExplorerBar)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar ultraExplorerBar;
        internal System.Windows.Forms.ImageList imageList;
	}
}