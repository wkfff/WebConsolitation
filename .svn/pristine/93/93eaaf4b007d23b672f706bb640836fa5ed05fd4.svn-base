using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Krista.FM.Client.Common;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.ViewObjects.DataSourcesUI
{
	public class DataSourcesNavigation : Krista.FM.Client.ViewObjects.BaseViewObject.BaseNavigationCtrl
	{
        private const string dataSourcesKey = "4CD14FBE-1C08-483b-A433-463B4542DEB3";

        private static DataSourcesNavigation instance;

        internal static DataSourcesNavigation Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataSourcesNavigation();
                }
                return instance;
            }
        }

        private DataSourcesUI openedViewObject;

        private Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar uebDataSources;
		private System.ComponentModel.Container components = null;

		public DataSourcesNavigation()
		{
            instance = this;

            Caption = "Источники данных";
		}

        public override System.Drawing.Image TypeImage16
        {
            get { return Krista.FM.Client.ViewObjects.DataSourcesUI.Properties.Resources.pump_DataSources_16; }
        }

        public override System.Drawing.Image TypeImage24
        {
            get { return Krista.FM.Client.ViewObjects.DataSourcesUI.Properties.Resources.pump_DataSources_24; }
        }

        public override void Initialize()
        {
            InitializeComponent();
            
            base.Initialize();

            uebDataSources.ItemCheckStateChanged += new Infragistics.Win.UltraWinExplorerBar.ItemCheckStateChangedEventHandler(uebDataSources_ItemCheckStateChanged);

            Workplace.ViewClosed += new Krista.FM.Client.Common.Gui.ViewContentEventHandler(Workplace_ViewClosed);
            Workplace.ActiveWorkplaceWindowChanged += new EventHandler(Workplace_ActiveWorkplaceWindowChanged);
            InfragisticsRusification.LocalizeAll();
        }

        void Workplace_ActiveWorkplaceWindowChanged(object sender, EventArgs e)
        {
            if (Workplace.WorkplaceLayout.ActiveContent != null)
            {
                string key = ((BaseViewObj)Workplace.WorkplaceLayout.ActiveContent).Key;
                if (uebDataSources.Groups[0].Items.Exists(key))
                {
                    Workplace.SwitchTo("Источники данных");

                    if (uebDataSources.CheckedItem == null)
                        return;
                    if (key != uebDataSources.CheckedItem.Key)
                    {
                        uebDataSources.Groups[0].Items[key].Checked = true;
                        uebDataSources.Groups[0].Items[key].Active = true;
                    }
                }
            }
        }

	    internal IDataSourceManager DataSourceManager
	    {
            get { return Workplace.ActiveScheme.DataSourceManager; }
	    }

        private void uebDataSources_ItemCheckStateChanged(object sender, Infragistics.Win.UltraWinExplorerBar.ItemEventArgs e)
        {
            if (e.Item.Checked)
            {
                if (openedViewObject == null)
                {
                    openedViewObject = new DataSourcesUI(e.Item.Key);
                    openedViewObject.Workplace = Workplace;
                    openedViewObject.Initialize();
                    openedViewObject.ViewCtrl.Text = "Все источники данных";
                    OnActiveItemChanged(this, openedViewObject);
                }
                else
                {
                    OnActiveItemChanged(this, openedViewObject);
                }
            }
        }

        private void Workplace_ViewClosed(object sender, Krista.FM.Client.Common.Gui.ViewContentEventArgs e)
        {
            if (openedViewObject == e.Content)
            {
                openedViewObject = null;
                if (uebDataSources.ActiveItem != null)
                {
                    uebDataSources.ActiveItem.Checked = false;
                }
            }
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
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.uebDataSources = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar();
            ((System.ComponentModel.ISupportInitialize)(this.uebDataSources)).BeginInit();
            this.SuspendLayout();
            // 
            // uebDataSources
            // 
            this.uebDataSources.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.uebDataSources.Dock = System.Windows.Forms.DockStyle.Fill;
            ultraExplorerBarItem1.Key = "4CD14FBE-1C08-483b-A433-463B4542DEB3";
            ultraExplorerBarItem1.Settings.AllowDragCopy = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            ultraExplorerBarItem1.Settings.AllowDragMove = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            appearance1.Image = global::Krista.FM.Client.ViewObjects.DataSourcesUI.Properties.Resources.pump_DataSources_16;
            ultraExplorerBarItem1.Settings.AppearancesSmall.Appearance = appearance1;
            ultraExplorerBarItem1.Text = "Все источники данных";
            ultraExplorerBarGroup1.Items.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem[] {
            ultraExplorerBarItem1});
            ultraExplorerBarGroup1.Settings.AllowDrag = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup1.Settings.AllowItemDrop = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup1.Settings.HeaderVisible = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup1.Text = "New Group";
            this.uebDataSources.Groups.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup[] {
            ultraExplorerBarGroup1});
            this.uebDataSources.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uebDataSources.Location = new System.Drawing.Point(0, 0);
            this.uebDataSources.Name = "uebDataSources";
            this.uebDataSources.ShowDefaultContextMenu = false;
            this.uebDataSources.Size = new System.Drawing.Size(216, 472);
            this.uebDataSources.Style = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarStyle.VisualStudio2005Toolbox;
            this.uebDataSources.TabIndex = 0;
            this.uebDataSources.ViewStyle = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarViewStyle.Office2000;
            // 
            // DataSourcesNavigation
            // 
            this.Controls.Add(this.uebDataSources);
            this.Name = "DataSourcesNavigation";
            ((System.ComponentModel.ISupportInitialize)(this.uebDataSources)).EndInit();
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

