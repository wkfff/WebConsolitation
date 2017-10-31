using System;
using System.Drawing;

using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.DataPumpUI
{
	public class DataPumpNavigation : BaseNavigationCtrl
    {
        private static DataPumpNavigation instance;

        internal static DataPumpNavigation Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataPumpNavigation();
                }
                return instance;
            }
        }

		private Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar uebNavi;

		private System.ComponentModel.IContainer components;

		public DataPumpNavigation()
		{
            instance = this;
            Caption = "Закачка данных";
		}

        public override Image TypeImage16
        {
            get { return Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.pump_DataPump_16; }
        }

        public override Image TypeImage24
        {
            get { return Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.pump_DataPump_24; }
        }

        /// <summary>
        /// Инициализация интерфейса
        /// </summary>
        public override void Initialize()
        {
            InitializeComponent();

			uebNavi.ItemCheckStateChanged += uebNavi_ItemCheckStateChanged;
			Workplace.ViewClosed += Workplace_ViewClosed;
			Workplace.ActiveWorkplaceWindowChanged += Workplace_ActiveWorkplaceWindowChanged;

            base.Initialize();

            InfragisticsRusification.LocalizeAll();
        }

		private void Workplace_ActiveWorkplaceWindowChanged(object sender, EventArgs e)
		{
			if (Workplace.WorkplaceLayout.ActiveContent != null)
			{
				string key = ((BaseViewObj)Workplace.WorkplaceLayout.ActiveContent).Key;
				if (uebNavi.Groups[0].Items.Exists(key))
				{
					Workplace.SwitchTo("Закачка данных");

					if (uebNavi.CheckedItem == null)
						return;
					if (key != uebNavi.CheckedItem.Key)
					{
						uebNavi.Groups[0].Items[key].Checked = true;
						uebNavi.Groups[0].Items[key].Active = true;
					}
				}
			}
		}

		private void Workplace_ViewClosed(object sender, ViewContentEventArgs e)
		{
			if ("DC76123D-734A-4541-B956-865413451545" == e.Content.Key)
			{
				if (uebNavi.ActiveItem != null)
				{
					uebNavi.ActiveItem.Checked = false;
				}
			}
		}

		private void uebNavi_ItemCheckStateChanged(object sender, Infragistics.Win.UltraWinExplorerBar.ItemEventArgs e)
		{
			if (e.Item.Checked)
			{
				IViewContent vc = WorkplaceSingleton.Workplace.GetOpenedContent(e.Item.Key);
				if (vc != null)
				{
                    vc.WorkplaceWindow.SelectWindow();
				}
				else
				{
					DataPumpsListUI view = new DataPumpsListUI(e.Item.Key);
					view.Workplace = Workplace;
					view.Initialize();
					OnActiveItemChanged(this, view);
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

		private void InitializeComponent()
		{
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.uebNavi = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar();
            ((System.ComponentModel.ISupportInitialize)(this.uebNavi)).BeginInit();
            this.SuspendLayout();
            // 
            // uebNavi
            // 
            this.uebNavi.AcceptsFocus = Infragistics.Win.DefaultableBoolean.True;
            this.uebNavi.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.uebNavi.Dock = System.Windows.Forms.DockStyle.Fill;
            ultraExplorerBarItem1.Key = "DC76123D-734A-4541-B956-865413451545";
            appearance1.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.pump_DataPump_16;
            ultraExplorerBarItem1.Settings.AppearancesSmall.Appearance = appearance1;
            ultraExplorerBarItem1.Text = "Закачки данных";
            ultraExplorerBarGroup1.Items.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem[] {
            ultraExplorerBarItem1});
            ultraExplorerBarGroup1.Text = "New Group";
            this.uebNavi.Groups.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup[] {
            ultraExplorerBarGroup1});
            this.uebNavi.GroupSettings.AllowDrag = Infragistics.Win.DefaultableBoolean.False;
            this.uebNavi.GroupSettings.AllowItemDrop = Infragistics.Win.DefaultableBoolean.False;
            this.uebNavi.GroupSettings.BorderStyleItemArea = Infragistics.Win.UIElementBorderStyle.None;
            this.uebNavi.GroupSettings.HeaderVisible = Infragistics.Win.DefaultableBoolean.False;
            this.uebNavi.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uebNavi.ItemSettings.AllowDragCopy = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            this.uebNavi.ItemSettings.AllowDragMove = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            this.uebNavi.Location = new System.Drawing.Point(0, 0);
            this.uebNavi.Name = "uebNavi";
            this.uebNavi.ShowDefaultContextMenu = false;
            this.uebNavi.Size = new System.Drawing.Size(221, 472);
            this.uebNavi.Style = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarStyle.VisualStudio2005Toolbox;
            this.uebNavi.TabIndex = 0;
            this.uebNavi.ViewStyle = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarViewStyle.Office2000;
            // 
            // DataPumpNavigation
            // 
            this.Controls.Add(this.uebNavi);
            this.Name = "DataPumpNavigation";
            this.Size = new System.Drawing.Size(221, 472);
            ((System.ComponentModel.ISupportInitialize)(this.uebNavi)).EndInit();
            this.ResumeLayout(false);

		}

		public override void Customize()
		{
			ComponentCustomizer.CustomizeInfragisticsComponents(components);
			base.Customize();
		}
	}
}

