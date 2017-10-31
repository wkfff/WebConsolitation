using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI
{
	public class BaseDetailTableView : UserControl
	{
		private UltraGridEx grid;

		public BaseDetailTableView()
		{
			InitializeComponent();

			grid.StateRowEnable = true;
			grid.ServerFilterEnabled = false;

			grid.utmMain.Tools["menuLoad"].SharedProps.Visible = false;
			grid.utmMain.Tools["menuSave"].SharedProps.Visible = false;

			// настройки для кнопок копировать/вставить
			grid.utmMain.Tools["CopyRow"].SharedProps.Visible = true;
			grid.utmMain.Tools["PasteRow"].SharedProps.Visible = true;

			grid.OnGridInitializeLayout += new GridInitializeLayout(ugeDetail_OnGridInitializeLayout);

			InfragisticComponentsCustomize.CustomizeUltraGridParams(grid.ugData);
		}

		public UltraGridEx Grid
		{
			get { return grid; }
		}

		/// <summary>
		/// Срабатывает при инициализации грида детали.
		/// </summary>
		public event GridInitializeLayout GridInitializeLayout;

		private void ugeDetail_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
		{
			BaseClsUI.AddDocumentsTypeListToGrid((UltraGrid)sender);
			BaseClsUI.AddDocumentButtons(e.Layout, Grid.CurrentStates);

			if (GridInitializeLayout != null)
			{
				GridInitializeLayout(sender, e);
			}
		}

		private void InitializeComponent()
		{
			this.grid = new Krista.FM.Client.Components.UltraGridEx();
			this.SuspendLayout();
			// 
			// grid
			// 
			this.grid.AllowAddNewRecords = true;
			this.grid.AllowClearTable = true;
			this.grid.Caption = "";
			this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grid.InDebugMode = false;
			this.grid.LoadMenuVisible = false;
			this.grid.Location = new System.Drawing.Point(0, 0);
			this.grid.MaxCalendarDate = new System.DateTime(((long)(0)));
			this.grid.MinCalendarDate = new System.DateTime(((long)(0)));
			this.grid.Name = "grid";
			this.grid.SaveLoadFileName = "";
			this.grid.SaveMenuVisible = false;
			this.grid.ServerFilterEnabled = false;
			this.grid.SingleBandLevelName = "Добавить запись...";
			this.grid.Size = new System.Drawing.Size(527, 223);
			this.grid.sortColumnName = "";
			this.grid.StateRowEnable = false;
			this.grid.TabIndex = 0;
			// 
			// BaseDetailTableView
			// 
			this.Controls.Add(this.grid);
			this.Name = "BaseDetailTableView";
			this.Size = new System.Drawing.Size(527, 223);
			this.ResumeLayout(false);

		}
	}
}
