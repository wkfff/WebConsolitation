using System.ComponentModel;
using System.Windows.Forms;
using Infragistics.Win.UltraWinToolbars;

using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using CC = Krista.FM.Client.Components;


namespace Krista.FM.Client.ViewObjects.DataSourcesUI
{
	public class DataSourcesView : BaseView
    {
        private Panel BaseView_Fill_Panel;
        public UltraGridEx ugeSources;
        public ImageList ilImages;
        private IContainer components;
       
		public DataSourcesView()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
         }

        /// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataSourcesView));
            this.BaseView_Fill_Panel = new System.Windows.Forms.Panel();
            this.ugeSources = new Krista.FM.Client.Components.UltraGridEx();
            this.ilImages = new System.Windows.Forms.ImageList(this.components);
            this.BaseView_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // BaseView_Fill_Panel
            // 
            this.BaseView_Fill_Panel.Controls.Add(this.ugeSources);
            this.BaseView_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.BaseView_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BaseView_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.BaseView_Fill_Panel.Name = "BaseView_Fill_Panel";
            this.BaseView_Fill_Panel.Size = new System.Drawing.Size(750, 500);
            this.BaseView_Fill_Panel.TabIndex = 0;
            // 
            // ugeSources
            // 
            this.ugeSources.AllowAddNewRecords = true;
            this.ugeSources.AllowClearTable = true;
            this.ugeSources.Cursor = System.Windows.Forms.Cursors.Default;
            this.ugeSources.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugeSources.InDebugMode = false;
            this.ugeSources.IsReadOnly = true;
            this.ugeSources.LoadMenuVisible = false;
            this.ugeSources.Location = new System.Drawing.Point(0, 0);
            this.ugeSources.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ugeSources.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ugeSources.Name = "ugeSources";
            this.ugeSources.SaveLoadFileName = "";
            this.ugeSources.SaveMenuVisible = false;
            this.ugeSources.ServerFilterEnabled = false;
            this.ugeSources.SingleBandLevelName = "";
            this.ugeSources.Size = new System.Drawing.Size(750, 500);
            this.ugeSources.sortColumnName = "";
            this.ugeSources.StateRowEnable = false;
            this.ugeSources.TabIndex = 4;
            // 
            // ilImages
            // 
            this.ilImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilImages.ImageStream")));
            this.ilImages.TransparentColor = System.Drawing.Color.Magenta;
            this.ilImages.Images.SetKeyName(0, "");
            this.ilImages.Images.SetKeyName(1, "ProtectForm.bmp");
            // 
            // DataSourcesView
            // 
            this.Controls.Add(this.BaseView_Fill_Panel);
            this.Name = "DataSourcesView";
            this.Size = new System.Drawing.Size(750, 500);
            this.BaseView_Fill_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		public override void Customize()
		{
			ComponentCustomizer.CustomizeInfragisticsComponents(this.components);
			base.Customize();
		}

	}
}

