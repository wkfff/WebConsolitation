using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.SEP_0001_0001
{
	/// <summary>
	/// Мониторинг СЭП
	/// </summary>
	public partial class Default : CustomReportPage
	{
		#region Поля

		private GridHeaderLayout headerLayout;
		private DataTable dtGrid;
		private DataTable dtChart;
		// словарь перекодировки районов в mdx
		private static Dictionary<string, string> dictRegion;
		// Для заполнения таблицы (направление должно выводиться только 1 раз)
		private static string prevDirection;

		private static int selectedGridRow = 0;

		#endregion

		#region Параметры запроса

		private CustomParam cpCube;
		private CustomParam cpGroup;
		private CustomParam cpParameter;

		#endregion
		// --------------------------------------------------------------------

		// заголовок страницы
		private const string PageTitleCaption = "Мониторинг социально-экономического развития по состоянию на {0} год";
		private const string PageSubTitleCaption = "Анализ социально-экономического положения Ханты-Мансийский автономный округ - Югра по основным направлениям развития";
		// заголовок для UltraChart
		private const string ChartTitleCaption = "Динамика показателя «{0}», {1}";

		// --------------------------------------------------------------------

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			PageSubTitle.Text = PageSubTitleCaption;

			#region Настройка грида
			UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
			UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.75);
			UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
			UltraWebGrid.DisplayLayout.SelectTypeRowDefault = SelectType.Single;
			UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
			UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
			UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
			UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);
			#endregion

			#region Настройка диаграммы
			UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
			UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);
			LabelChart.Width = UltraChart.Width;

			UltraChart.ChartType = ChartType.ColumnChart;
			UltraChart.Data.ZeroAligned = true;

			UltraChart.Border.Thickness = 0;

			UltraChart.Axis.X.Extent = 50;
			UltraChart.Axis.X.LineThickness = 1;
			UltraChart.Axis.X.Labels.SeriesLabels.Visible = false;
			UltraChart.Axis.Y.Extent = 50;
			UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

			UltraChart.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart.Axis.X.Margin.Near.Value = 20;

			UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;

			UltraChart.Legend.Visible = false;
			UltraChart.Legend.Location = LegendLocation.Bottom;
			UltraChart.Legend.SpanPercentage = 10;
			UltraChart.Border.Thickness = 0;

			UltraChart.Tooltips.FormatString = "<ITEM_LABEL> год\n<b><DATA_VALUE></b>";
			UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			UltraChart.DataBinding += new EventHandler(UltraChart_DataBinding);

			PanelChart.AddLinkedRequestTrigger(UltraWebGrid);

			#endregion

			#region Параметры
			cpCube = UserParams.CustomParam("cube");
			cpGroup = UserParams.CustomParam("group");
			cpParameter = UserParams.CustomParam("parameter");
			#endregion

			#region Экспорт
			ReportExcelExporter.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
			ReportExcelExporter.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
			ReportExcelExporter.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);

			ReportPDFExporter.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
			#endregion
		}

		// --------------------------------------------------------------------

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);

			if (!PanelChart.IsAsyncPostBack)
			{
				prevDirection = String.Empty;
				headerLayout = new GridHeaderLayout(UltraWebGrid);

				UltraWebGrid.DataBind();
				UltraWebGrid_ActivateRow(UltraWebGrid.Rows[selectedGridRow]);
			}

		}

		// --------------------------------------------------------------------
		#region Обработчики грида

		protected void UltraWebGrid_ActivateRow(UltraGridRow row)
		{
			row.Selected = true;
			row.Activate();
			UltraChart_SetParams(row);
			UltraChart.DataBind();
			selectedGridRow = row.Index;
		}

		protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
		{
			UltraWebGrid_ActivateRow(e.Row);
		}

		protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("SEP_0001_0001_grid");
			dtGrid = new DataTable();
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);
			if (dtGrid.Rows.Count > 0)
			{
				PageTitle.Text = String.Format(PageTitleCaption, dtGrid.Columns[dtGrid.Columns.Count - 1].Caption);
				Page.Title = PageTitle.Text;
				query = DataProvider.GetQueryText("SEP_0001_0001_grid_properties");
				DataTable dtGridProperties = new DataTable();
				DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtGridProperties);
				dtGrid.Columns.Add("Описание", typeof(string));
				for (int i = 0; (i < dtGrid.Rows.Count) && (i < dtGridProperties.Rows.Count); ++i)
				{
					dtGrid.Rows[i]["Описание"] = dtGridProperties.Rows[i][1];
				}
				UltraWebGrid.DataSource = dtGrid;
			}
			else
			{
				UltraWebGrid.DataSource = null;
			}
		}

		protected void UltraWebGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
		{
			double k = 0.95;
			if (Browser == "Firefox")
			{
				k = 0.9;
			}
			else if (Browser == "AppleMAC-Safari")
			{
				k = 0.85;
			}
			e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
			e.Layout.AllowSortingDefault = AllowSorting.No;
			e.Layout.CellClickActionDefault = CellClickAction.RowSelect;
			//e.Layout.NullTextDefault = "-";
			e.Layout.RowSelectorStyleDefault.Width = 20;
			e.Layout.Bands[0].Columns.Insert(0, "Направление");
			e.Layout.Bands[0].Columns[0].Width = 100;
			e.Layout.Bands[0].Columns[1].Width = 200;
			e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
			e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
			for (int i = 2; i < e.Layout.Bands[0].Columns.Count; ++i)
			{
				e.Layout.Bands[0].Columns[i].Width = 100;
				e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Left = 5;
			}
			
			// Заголовки
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
			for (int i = 0; i < e.Layout.Bands[0].Columns.Count; ++i)
			{
				headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Key);
			}
			headerLayout.ApplyHeaderInfo();

			e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
		}

		protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
		{
			if (!String.Equals(prevDirection, e.Row.Cells[1].GetText().Split('_')[0]))
			{
				prevDirection = e.Row.Cells[0].Text = e.Row.Cells[1].GetText().Split('_')[0];
			}
			e.Row.Cells[1].Text = e.Row.Cells[1].GetText().Split('_')[1] + ", " + e.Row.Cells[e.Row.Cells.Count - 1].GetText().Split(';')[2].ToLower();
			for (int i = 2; i < e.Row.Cells.Count - 1; ++i)
			{
				UltraGridCell cell = e.Row.Cells[i];
				if (cell.GetText().Split(';')[0] == "0")
				{
					cell.Value = null;
				}
				else
				{
					string[] cellElements = cell.GetText().Split(';');
					string cellValue = String.Empty;
					cellValue += String.Format("{0:N2}<br/>", Convert.ToDouble(cellElements[0]));
					cellValue += String.Format("{0:N2}<br/>", Convert.ToDouble(cellElements[1]));
					cellValue += String.Format("{0:P2}", Convert.ToDouble(cellElements[2]));
					cell.Value = cellValue;
					double value = Convert.ToDouble(cellElements[1]);
					if (value != 0)
					{
						string paramType = e.Row.Cells[e.Row.Cells.Count - 1].GetText().Split(';')[3];
						if (paramType.ToLower().Contains("обратный"))
						{
							cell.Style.CssClass = (value > 0) ? "ArrowUpRed" : "ArrowDownGreen";
						}
						else
						{
							cell.Style.CssClass = (value > 0) ? "ArrowUpGreen" : "ArrowDownRed";
						}
						cell.Title = String.Format("Темп роста к {0} году", UltraWebGrid.Columns[i -1].Key);
					}
				}
			}
		}

		#endregion

		#region Обработчики диаграммы

		protected void UltraChart_SetParams(UltraGridRow row)
		{
			UltraGridCell cell = row.Cells[row.Cells.Count - 1];
			string[] cellParams = cell.GetText().Split(';');
			cpCube.Value = cellParams[0];
			cpParameter.Value = cellParams[1];
			cpGroup.Value = cellParams[4];
			LabelChart.Text = String.Format(ChartTitleCaption, GetLastBlock(cpParameter.Value), cellParams[2].ToLower());
		}

		protected void UltraChart_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("SEP_0001_0001_chart");
			dtChart = new DataTable();
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
			UltraChart.DataSource = dtChart == null ? null : dtChart.DefaultView;
		}

		#endregion

		#region Экспорт в PDF

		private void PdfExportButton_Click(object sender, EventArgs e)
		{
			ReportPDFExporter.PageTitle = PageTitle.Text;
			ReportPDFExporter.PageSubTitle = PageSubTitle.Text;

			Report report = new Report();
			ISection section1 = report.AddSection();
			ISection section2 = report.AddSection();

			UltraWebGrid.Columns.RemoveAt(UltraWebGrid.Columns.Count - 1);
			headerLayout.childCells.RemoveAt(headerLayout.ChildCount - 1);
			for (int i = 0; i < UltraWebGrid.Rows.Count; ++i)
			{
				for (int j = 2; j < UltraWebGrid.Columns.Count; ++j)
				{
					if (UltraWebGrid.Rows[i].Cells[j].Value != null)
					{
						UltraWebGrid.Rows[i].Cells[j].Value = UltraWebGrid.Rows[i].Cells[j].GetText().Replace("<br/>", "\n");
					}
				}
			}
			UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

			ReportPDFExporter.HeaderCellHeight = 70;
			ReportPDFExporter.Export(headerLayout, section1);
			ReportPDFExporter.Export(UltraChart, LabelChart.Text, section2);
		}

		#endregion

		#region Экспорт в Excel

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			ReportExcelExporter.WorksheetTitle = PageTitle.Text;
			ReportExcelExporter.WorksheetSubTitle = PageSubTitle.Text;

			Workbook workbook = new Workbook();
			Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
			Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");

			SetExportGridParams(headerLayout.Grid);

			ReportExcelExporter.HeaderCellHeight = 25;
			ReportExcelExporter.HeaderCellFont = new Font("Verdana", 11);
			ReportExcelExporter.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
			ReportExcelExporter.SubTitleFont = new Font("Verdana", 11);
			ReportExcelExporter.TitleAlignment = HorizontalCellAlignment.Center;
			ReportExcelExporter.TitleStartRow = 3;

			UltraWebGrid.Columns.RemoveAt(UltraWebGrid.Columns.Count - 1);
			headerLayout.childCells.RemoveAt(headerLayout.ChildCount - 1);

			ReportExcelExporter.Export(headerLayout, sheet1, 6);
			for (int i = 0; i < UltraWebGrid.Rows.Count; ++i)
			{
				sheet1.Rows[7 + i].Height = 255;
				for (int j = 0; j < UltraWebGrid.Columns.Count; ++j)
				{
					WorksheetCell cell = sheet1.Rows[7 + i].Cells[j];
					cell.Value = null;
				}
			}
			sheet1.MergedCellsRegions.Clear();
			sheet1.Rows[3].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
			sheet1.Rows[3].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			sheet1.Rows[4].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
			sheet1.Rows[4].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			for (int i = 0; i < UltraWebGrid.Rows.Count; ++i)
			{
				WorksheetCell cell = sheet1.Rows[7 + i * 3].Cells[0];
				cell.Value = UltraWebGrid.Rows[i].Cells[0].GetText();
				sheet1.MergedCellsRegions.Add(7 + i * 3, 0, 9 + i * 3, 0);
				SetEmptyCellFormat(sheet1, i, 0);
				cell = sheet1.Rows[7 + i * 3].Cells[1];
				SetEmptyCellFormat(sheet1, i, 1);
				cell.Value = UltraWebGrid.Rows[i].Cells[1].GetText();
				sheet1.MergedCellsRegions.Add(7 + i * 3, 1, 9 + i * 3, 1);
				for (int j = 2; j < UltraWebGrid.Columns.Count; ++j)
				{
					if (UltraWebGrid.Rows[i].Cells[j].Value != null)
					{
						string[] separator = { "<br/>" };
						string[] values = UltraWebGrid.Rows[i].Cells[j].GetText().Split(separator, StringSplitOptions.None);
						for (int n = 0; n < 3; ++n)
						{
							cell = sheet1.Rows[7 + n + i * 3].Cells[j];
							cell.Value = values[n];
							SetCellFormat(cell);
							cell.CellFormat.Alignment = HorizontalCellAlignment.Right;
							if (n < 2)
							{
								cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
							}
							if (n > 0)
							{
								cell.CellFormat.TopBorderStyle = CellBorderLineStyle.None;
							}
						}
					}
					else
					{
						SetEmptyCellFormat(sheet1, i, j);
					}
				}
			}

			UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.7));
			ReportExcelExporter.Export(UltraChart, LabelChart.Text, sheet2, 3);
		}

		private void SetEmptyCellFormat(Worksheet sheet, int row, int column)
		{
			WorksheetCell cell = sheet.Rows[7 + row * 3].Cells[column];
			cell.CellFormat.Font.Name = "Verdana";
			cell.CellFormat.Font.Height = 200;
			cell.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;

			cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.LeftBorderColor = Color.Black;
			cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.RightBorderColor = Color.Black;
			cell.CellFormat.TopBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.TopBorderColor = Color.Black;
			cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
			cell.CellFormat.BottomBorderColor = Color.Black;

			cell = sheet.Rows[8 + row * 3].Cells[column];
			cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.LeftBorderColor = Color.Black;
			cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.RightBorderColor = Color.Black;
			cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
			cell.CellFormat.BottomBorderColor = Color.Black;
			cell.CellFormat.TopBorderStyle = CellBorderLineStyle.None;
			cell.CellFormat.TopBorderColor = Color.Black;

			cell = sheet.Rows[9 + row * 3].Cells[column];
			cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.BottomBorderColor = Color.Black;
			cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.LeftBorderColor = Color.Black;
			cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.RightBorderColor = Color.Black;
			cell.CellFormat.TopBorderStyle = CellBorderLineStyle.None;
			cell.CellFormat.TopBorderColor = Color.Black;
		}

		private void SetCellFormat(WorksheetCell cell)
		{
			cell.CellFormat.Font.Name = "Verdana";
			cell.CellFormat.Font.Height = 200;
			cell.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
			cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.BottomBorderColor = Color.Black;
			cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.LeftBorderColor = Color.Black;
			cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.RightBorderColor = Color.Black;
			cell.CellFormat.TopBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.TopBorderColor = Color.Black;
		}

		private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
		{
			e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
			e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
			//e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
			//e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
			//e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
			e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
			e.CurrentWorksheet.PrintOptions.TopMargin = 0;
			e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
			e.CurrentWorksheet.PrintOptions.RightMargin = 0;
		}

		private static void SetExportGridParams(UltraWebGrid grid)
		{
			string exportFontName = "Verdana";
			int fontSize = 10;
			double coeff = 1.0;
			foreach (UltraGridColumn column in grid.Columns)
			{
				column.Width = Convert.ToInt32(column.Width.Value * coeff);
				column.CellStyle.Font.Name = exportFontName;
				column.Header.Style.Font.Name = exportFontName;
				column.CellStyle.Font.Size = fontSize;
				column.Header.Style.Font.Size = fontSize;
			}
		}

		private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
		{
		}

		#endregion

		// --------------------------------------------------------------------

		static string Browser
		{
			get { return HttpContext.Current.Request.Browser.Browser; }
		}
		
		/// <summary>
		/// Возвращает последний элемент из mdx-имени
		/// </summary>
		private string GetLastBlock(string mdxName)
		{
			string[] separator = { "].[" };
			string[] elements = mdxName.Split(separator, StringSplitOptions.None);
			return elements[elements.Length - 1].Replace("]", String.Empty);
		}

	}
}
