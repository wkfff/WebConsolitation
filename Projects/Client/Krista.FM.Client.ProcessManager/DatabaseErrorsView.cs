using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common;

namespace Krista.FM.Client.ProcessManager
{
	public partial class DatabaseErrorsView : UserControl
	{
		public DatabaseErrorsView()
		{
			InitializeComponent();

			grid._utmMain.Tools["ShowHierarchy"].SharedProps.Visible = false;
			grid._utmMain.Tools["ColumnsVisible"].SharedProps.Visible = false;
			grid._utmMain.Tools["ClearCurrentTable"].SharedProps.Visible = false;
			grid._utmMain.Tools["DeleteSelectedRows"].SharedProps.Visible = false;

			grid.OnGridInitializeLayout += new Krista.FM.Client.Components.GridInitializeLayout(grid_OnGridInitializeLayout);
		}

		private void grid_OnGridInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
		{
			UltraGridBand band = e.Layout.Bands[0];

			UltraGridColumn clmn;

			clmn = band.Columns["Error"];
			clmn.CellActivation = Activation.ActivateOnly;
			clmn.AllowGroupBy = DefaultableBoolean.True;
			clmn.AllowRowFiltering = DefaultableBoolean.True;
			clmn.AllowRowSummaries = AllowRowSummaries.False;
			clmn.Header.VisiblePosition = 1;
			clmn.Hidden = false;
			clmn.Header.Caption = "Ошибка";
			clmn.Width = 350;

			clmn = band.Columns["ObjectName"];
			clmn.CellActivation = Activation.ActivateOnly;
			clmn.AllowGroupBy = DefaultableBoolean.True;
			clmn.AllowRowFiltering = DefaultableBoolean.True;
			clmn.AllowRowSummaries = AllowRowSummaries.False;
			clmn.Header.VisiblePosition = 2;
			clmn.Hidden = false;
			clmn.Header.Caption = "Имя объекта";
			clmn.Width = 250;

			clmn = band.Columns["ObjectId"];
			clmn.CellActivation = Activation.ActivateOnly;
			clmn.AllowGroupBy = DefaultableBoolean.False;
			clmn.AllowRowFiltering = DefaultableBoolean.True;
			clmn.AllowRowSummaries = AllowRowSummaries.False;
			clmn.Header.VisiblePosition = 9;
			clmn.Hidden = false;
			clmn.Header.Caption = "ID объекта";
			clmn.Width = 250;

			clmn = band.Columns["ObjectType"];
			clmn.CellActivation = Activation.ActivateOnly;
			clmn.AllowGroupBy = DefaultableBoolean.False;
			clmn.AllowRowFiltering = DefaultableBoolean.True;
			clmn.AllowRowSummaries = AllowRowSummaries.False;
			clmn.Header.VisiblePosition = 9;
			clmn.Hidden = false;
			clmn.Header.Caption = "Тип объекта";
			clmn.Width = 70;
		}

		public void Connect(DataTable dataTable)
		{
			InfragisticComponentsCustomize.CustomizeUltraGridParams(grid._ugData);
			Dock = DockStyle.Fill;
			grid.DataSource = dataTable;
		}
	}
}
