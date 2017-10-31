using System;
using System.Data;

using Infragistics.Win.UltraWinGrid;

using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.DataPumpUI
{
	/// <summary>
	/// Отображает список закачек данных.
	/// </summary>
	public partial class DataPumpsListView : BaseView
	{
		public DataPumpsListView()
		{
			InitializeComponent();

			grid.OnGridInitializeLayout += grid_OnGridInitializeLayout;
			grid.OnRefreshData += grid_OnRefreshData;
			grid.OnClickCellButton += grid_OnClickCellButton;
			grid.OnInitializeRow += grid_OnInitializeRow;

			grid_OnRefreshData(grid);

			grid.AllowClearTable = false;
			grid.AllowDeleteRows = false;
			grid.AllowImportFromXML = false;
			grid.AllowAddNewRecords = false;
			grid.ExportImportToolbarVisible = false;
			grid.SaveMenuVisible = false;
			grid.LoadMenuVisible = false;
			grid.StateRowEnable = false;
			grid._utmMain.Tools["ShowHierarchy"].SharedProps.Visible = false;
			grid._utmMain.Tools["ColumnsVisible"].SharedProps.Visible = false;
			grid._utmMain.Tools["ClearCurrentTable"].SharedProps.Visible = false;
			grid._utmMain.Tools["DeleteSelectedRows"].SharedProps.Visible = false;
		}

		/// <summary>
		/// Настройка сетки отображения данных.
		/// </summary>
		private void grid_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
		{
			grid.StateRowEnable = false; 
			
			UltraGridBand band = e.Layout.Bands[0];

			UltraGridColumn clmn;
			band.Columns["ID"].Hidden = true;
			band.Columns["ProgramIdentifier"].Hidden = true;
			
			clmn = band.Columns["State"];
			UltraGridHelper.SetLikelyImageColumnsStyle(clmn, -1);
			clmn.Header.VisiblePosition = 1;
			clmn.CellActivation = Activation.AllowEdit;
			clmn.AllowGroupBy = Infragistics.Win.DefaultableBoolean.False;
			clmn.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
			clmn.AllowRowSummaries = AllowRowSummaries.False;

			clmn = band.Columns["SupplierCode"];
			clmn.CellActivation = Activation.AllowEdit;
			clmn.AllowGroupBy = Infragistics.Win.DefaultableBoolean.True;
			clmn.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
			clmn.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
			clmn.Width = 100;
			clmn.Header.VisiblePosition = 2;

			clmn = band.Columns["DataCode"];
			clmn.CellActivation = Activation.AllowEdit;
			clmn.AllowGroupBy = Infragistics.Win.DefaultableBoolean.True;
			clmn.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
			clmn.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
			clmn.Width = 75;
			clmn.Header.VisiblePosition = 3;

			clmn = band.Columns["DataName"];
			clmn.CellActivation = Activation.AllowEdit;
			clmn.AllowGroupBy = Infragistics.Win.DefaultableBoolean.False;
			clmn.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
			clmn.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
			clmn.Width = 300;
			clmn.Header.VisiblePosition = 4;

			clmn = band.Columns["Name"];
			clmn.CellActivation = Activation.AllowEdit;
			clmn.AllowGroupBy = Infragistics.Win.DefaultableBoolean.False;
			clmn.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
			clmn.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
			clmn.Width = 300;
			clmn.Header.VisiblePosition = 5;

			clmn = band.Columns["Comments"];
			clmn.CellActivation = Activation.AllowEdit;
			clmn.AllowGroupBy = Infragistics.Win.DefaultableBoolean.False;
			clmn.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
			clmn.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
			clmn.Width = 400;
			clmn.Header.VisiblePosition = 6;
		}

		/// <summary>
		/// Инициализация строк сетки данных.
		/// </summary>
		private void grid_OnInitializeRow(object sender, InitializeRowEventArgs e)
		{
			// Установка состояния закачки
            if (Convert.ToBoolean(e.Row.Cells["State"].Value))
            {
                e.Row.Cells["State"].Appearance.Image = imageList.Images[1];
                e.Row.Cells["State"].ToolTipText = "Выполняется закачка данных";
            }
            else
                e.Row.Cells["State"].ToolTipText = string.Empty;
            if (e.Row.Cells.Exists("GoToObject"))
		        e.Row.Cells["GoToObject"].ToolTipText = "Перейти на закачку данных";
		}  

		/// <summary>
		/// Обработчик события клика на кнопке рефреша списка закачек.
		/// </summary>
		private bool grid_OnRefreshData(object sender)
		{
			UltraGridStateSettings gridSettings = UltraGridStateSettings.SaveUltraGridStateSettings(grid.ugData);

			grid.DataSource = WorkplaceSingleton.Workplace.ActiveScheme.DataPumpManager.DataPumpInfo.GetPumpRegistryInfo();

			UltraGridColumn clmn = grid.ugData.DisplayLayout.Bands[0].Columns.Add("GoToObject", "");
			UltraGridHelper.SetLikelyButtonColumnsStyle(clmn, -1);
			clmn.CellButtonAppearance.Image = imageList.Images[0];
			clmn.Header.VisiblePosition = 1;

			gridSettings.RestoreUltraGridStateSettings(grid.ugData);

			return true;
		}

		/// <summary>
		/// Обработчик нажатия кнопки перехода на интерфейс закачки.
		/// </summary>
		private void grid_OnClickCellButton(object sender, CellEventArgs e)
		{
			UltraGridRow activeRow = e.Cell.Row;
			if (activeRow == null)
			{
				return;
			}

			if (activeRow.Cells != null)
			{
				string programIdentifier = activeRow.Cells["ProgramIdentifier"].Text;

				IViewContent vc = WorkplaceSingleton.Workplace.GetOpenedContent(programIdentifier);
				if (vc != null)
				{
					vc.WorkplaceWindow.SelectWindow();
				}
				else
				{
					DataPumpUI viewObject = new DataPumpUI(programIdentifier);
					viewObject.Workplace = WorkplaceSingleton.Workplace;
					viewObject.Initialize();
					viewObject.ViewCtrl.Text = WorkplaceSingleton.Workplace.ActiveScheme.DataPumpManager.DataPumpInfo.PumpRegistry[programIdentifier].Name;

					WorkplaceSingleton.Workplace.ShowView(viewObject);

					viewObject.LoadData();
				}
			}
		}
	}
}
