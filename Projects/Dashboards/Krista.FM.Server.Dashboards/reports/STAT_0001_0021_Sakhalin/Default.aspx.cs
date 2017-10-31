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
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.STAT_0001_0021_Sakhalin
{
	/// <summary>
	/// Мониторинг иностранных инвестиций
	/// </summary>
	public partial class Default : CustomReportPage
	{
		#region Поля

		private GridHeaderLayout headerLayout;
		private DataTable dtGrid;
		private DataTable dtChart;
		// словарь перекодировки районов в mdx
		private static Dictionary<string, string> dictRegion;

		#endregion

		#region Параметры запроса

		private CustomParam selectedRegion;
		private CustomParam selectedParameter;
		private CustomParam firstQuarter;
		private CustomParam lastQuarter;

		#endregion
		// --------------------------------------------------------------------

		// заголовок страницы
		private const string PageTitleCaption = "Мониторинг финансовых вложений";
		private const string PageSubTitleCaption = "Ежеквартальный мониторинг долгосрочных и краткосрочных финансовых вложений по крупным и средним организациям нарастающим итогом, {0}";
		// заголовок для UltraChart
		private const string ChartTitleCaption = "Динамика и структура видов финансовых вложений по крупным и средним организациям, млн. рублей";

		// --------------------------------------------------------------------

        private static int Resolution
        {
            get { return CRHelper.GetScreenWidth; }
        }

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			#region Настройка грида

			UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
			UltraWebGrid.Height = Unit.Empty;
			UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
			UltraWebGrid.DisplayLayout.SelectTypeRowDefault = SelectType.Single;
			UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
			UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
			UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);

			#endregion

			#region Настройка диаграммы

			UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
			UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6);
			LabelChart.Width = UltraChart.Width;

			UltraChart.ChartType = ChartType.StackColumnChart;
			UltraChart.ColumnChart.NullHandling = NullHandling.DontPlot;

			UltraChart.Border.Thickness = 0;

			UltraChart.Axis.Y.Extent = 75;
			UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);

            UltraChart.Axis.X.Extent = 75;
            //UltraChart.Axis.X.LineThickness = 1;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
            UltraChart.Axis.X.Labels.SeriesLabels.WrapText = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

			UltraChart.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart.Axis.X.Margin.Near.Value = 20;

			UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;

			UltraChart.Legend.Visible = true;
			UltraChart.Legend.Location = LegendLocation.Bottom;
			UltraChart.Legend.SpanPercentage = 10;
            UltraChart.Legend.Font = new Font("Verdana", 8);
			UltraChart.Border.Thickness = 0;

			UltraChart.Tooltips.FormatString = "<ITEM_LABEL>\n<b><DATA_VALUE:N2></b> млн. рублей";
			UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
			UltraChart.DataBinding += new EventHandler(UltraChart_DataBinding);

			#endregion

			#region Параметры
			selectedRegion = UserParams.CustomParam("selected_region");
			selectedParameter = UserParams.CustomParam("selected_parameter");
			firstQuarter = UserParams.CustomParam("first_quarter");
			lastQuarter = UserParams.CustomParam("last_quarter");
			#endregion

			#region Экспорт
			ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
			ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

			ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
			#endregion

            // Установка размеров
            if (Resolution < 900)
            {
                UltraWebGrid.Width = Unit.Parse("725px");
            }
            else if (Resolution < 1200)
            {
                UltraWebGrid.Width = Unit.Parse("950px");
            }
            else if (Resolution < 1800)
            {
                UltraWebGrid.Width = Unit.Parse("1200px");
            }
            else
            {
                UltraWebGrid.Width = Unit.Parse("1800px");
            }
            UltraChart.Width = UltraWebGrid.Width;

		}

		// --------------------------------------------------------------------

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);
			if (!Page.IsPostBack)
			{
				GetQuarters(firstQuarter, lastQuarter, "STAT_0001_0021_Sakhalin_quarters");
			}

			PageTitle.Text = PageTitleCaption;
			Page.Title = PageTitleCaption;
			PageSubTitle.Text = String.Format(PageSubTitleCaption, "Сахалинская область");

			headerLayout = new GridHeaderLayout(UltraWebGrid);

			UltraChart.DataBind();
			UltraWebGrid.DataBind();
		}

		// --------------------------------------------------------------------

		#region Обработчики грида

		protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("STAT_0001_0021_Sakhalin_grid");
			dtGrid = new DataTable();
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);
			if (dtGrid.Rows.Count > 0)
			{
				for (int i = 0; i < dtGrid.Rows.Count; i += 3)
				{
					dtGrid.Rows[i + 1][1] = dtGrid.Rows[i + 2][1] = dtGrid.Rows[i][1];
				}
				for (int i = 0; i < dtGrid.Rows.Count; ++i)
					if (i % 3 != 2)
						for (int j = 2; j < dtGrid.Columns.Count; ++j)
							if (dtGrid.Rows[i][j] != DBNull.Value)
								dtGrid.Rows[i][j] = Convert.ToDouble(dtGrid.Rows[i][j]);
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
			e.Layout.CellClickActionDefault = CellClickAction.CellSelect;
			e.Layout.NullTextDefault = "-";
			e.Layout.Bands[0].Columns[0].MergeCells = true;
			e.Layout.Bands[0].Columns[1].MergeCells = true;
			e.Layout.RowSelectorStyleDefault.Width = 20;
			e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
			double gridWidth = UltraWebGrid.Width.Value - 10;
			double columnWidth = 75;
			for (int i = 0; i < e.Layout.Bands[0].Columns.Count; ++i)
			{
                e.Layout.Bands[0].Columns[i].Width = i < 2 ? CRHelper.GetColumnWidth(columnWidth) : CRHelper.GetColumnWidth(columnWidth + 7);
				e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Left = 5;
			}
			e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;

			// Заголовки
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
			GridHeaderCell header, header1 = null;
			header = headerLayout.AddCell("Период");
			header.AddCell("Год", 2);
			header.AddCell("Квартал", 2);
			header = headerLayout.AddCell("Долгосрочные финансовые вложения, млн. руб.");
			header.AddCell("Всего");
			header1 = header.AddCell("В том числе");
			header1.AddCell("В паи и акции других организаций");
			header1.AddCell("из них: в паи и акции дочерних и зависимых организаций");
			header1.AddCell("В облигации и другие долговые обяза-тельства");
			header1.AddCell("В предостав-ленные займы");
			header1.AddCell("Прочие");
			header = headerLayout.AddCell("Краткосрочные финансовые вложения, млн. руб.");
			header.AddCell("Всего");
			header1 = header.AddCell("В том числе");
			header1.AddCell("В паи и акции других организаций");
			header1.AddCell("из них: в паи и акции дочерних и зависимых организаций");
			header1.AddCell("В облигации и другие долговые обяза-тельства");
			header1.AddCell("В предостав-ленные займы");
			header1.AddCell("Прочие");
			headerLayout.ApplyHeaderInfo();
		}

		protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
		{
			if (!String.IsNullOrEmpty(e.Row.Cells[1].GetText()))
			{
				e.Row.Cells[0].Text = e.Row.Cells[1].GetText().Split(';')[0];
				e.Row.Cells[1].Text = e.Row.Cells[1].GetText().Split(';')[1];
			}
			string appg = String.Empty;
			string year = Convert.ToString(Convert.ToInt32(e.Row.Cells[0].GetText().Split(' ')[1]) - 1) + " года";
			string quarter = e.Row.Cells[1].GetText().Split(' ')[1] + " кварталу ";
			appg = quarter + year;

			for (int i = 2; i < e.Row.Cells.Count; ++i)
			{
				UltraGridCell cell = e.Row.Cells[i];
				double value;
				if (Double.TryParse(cell.Text, out value))
				{
					if (e.Row.Index % 3 == 2)
					{
						//cell.Style.CssClass = value > 1 ? "ArrowUpGreen" : value < 1 ? "ArrowDownRed" : String.Empty;
                        if (value != 1)
                        {
                            if (value > 1)
                            {
                                cell.Style.BackgroundImage = "~/images/ArrowGreenUpBB.png";
                            }
                            else
                            {
                                cell.Style.BackgroundImage = "~/images/ArrowRedDownBB.png";
                            }
                            cell.Style.CustomRules = "background-repeat: no-repeat; background-position: center left";
                        }
					}
					string cellFormat = e.Row.Index % 3 == 2 ? "{0:P2}" : "{0:N2}";
					cell.Text = String.Format(cellFormat, value);
				}
				switch (e.Row.Index % 3)
				{
					case 0:
						{
							cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
							cell.Style.Font.Bold = true;
							break;
						}
					case 1:
						{
							cell.Title = "Прирост к " + appg;
							cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
							cell.Style.BorderDetails.StyleTop = BorderStyle.None;
							break;
						}
					case 2:
						{
							cell.Title = "Темп роста к " + appg;
							cell.Style.BorderDetails.StyleTop = BorderStyle.None;
							break;
						}
				}
			}
		}

		#endregion

		#region Обработчики диаграммы

		void UltraChart_DataBinding(object sender, EventArgs e)
		{
			LabelChart.Text = ChartTitleCaption;

			string query = DataProvider.GetQueryText("STAT_0001_0021_Sakhalin_chart");
			dtChart = new DataTable();
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
			dtChart.Columns.RemoveAt(0);
			foreach (DataRow row in dtChart.Rows)
			{
				row[0] = row[0].ToString().Replace(";", "\n");
			}
			for (int i = 0; i < dtChart.Rows.Count; ++i)
				for (int j = 1; j < dtChart.Columns.Count; ++j)
					if (dtChart.Rows[i][j] != DBNull.Value)
						dtChart.Rows[i][j] = Convert.ToDouble(dtChart.Rows[i][j]);
			UltraChart.Series.Clear();
			UltraChart.Data.SwapRowsAndColumns = true;
			for (int i = 1; i < dtChart.Columns.Count; i++)
			{
				NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
				UltraChart.Series.Add(series);
			}
		}

		void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			/*for (int i = 0; i < e.SceneGraph.Count; ++i)
			{
				Primitive primitive = e.SceneGraph[i];
				//if (primitive is Text && !String.IsNullOrEmpty(primitive.Path) && primitive.Path.Contains("Grid.X"))
				if (primitive is Text)
				{
					Text text = primitive as Text;
					if (text.GetTextString() == "Нет данных")
					{
						text.SetTextString(String.Empty);
					}
				}
			}*/
		}

		#endregion

		#region Экспорт в PDF

		private void PdfExportButton_Click(object sender, EventArgs e)
		{
			/*ReportPDFExporter1.PageTitle = PageTitle.Text;
			ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

			Report report = new Report();
			ISection section1 = report.AddSection();
			ISection section2 = report.AddSection();

			UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

			foreach (UltraGridColumn column in headerLayout.Grid.Columns)
			{
				column.Width = (int)(column.Width.Value * 1.1);
			}
			ReportPDFExporter1.HeaderCellHeight = 70;
			ReportPDFExporter1.Export(headerLayout, section1);
			ReportPDFExporter1.Export(UltraChart, LabelChart.Text, section2);*/
			ReportPDFExporter1.PageTitle = PageTitle.Text;
			ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

			Report report = new Report();
			ISection section1 = report.AddSection();
			ISection section2 = report.AddSection();

			UltraWebGrid grid = headerLayout.Grid;
			if (grid.Rows.Count > 0)
			{
				foreach (UltraGridRow row in grid.Rows)
					foreach (UltraGridCell cell in row.Cells)
						if (cell.Value == null)
							cell.Value = "-";
				for (int i = 0; i < grid.Rows.Count; i += 3)
				{
					grid.Rows[i].Cells[1].Value = null;
					grid.Rows[i + 2].Cells[1].Value = null;
					grid.Rows[i].Cells[1].Style.BorderDetails.StyleBottom = BorderStyle.None;
					grid.Rows[i + 2].Cells[1].Style.BorderDetails.StyleTop = BorderStyle.None;
					grid.Rows[i + 1].Cells[1].Style.BorderDetails.StyleTop = BorderStyle.None;
					grid.Rows[i + 1].Cells[1].Style.BorderDetails.StyleBottom = BorderStyle.None;
				}
				string year = grid.Rows[0].Cells[0].GetText();
				int startRow = 0;
				grid.Rows[0].Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
				grid.Rows[0].Cells[0].Value = null;
				for (int i = 1; i < grid.Rows.Count; ++i)
				{
					UltraGridCell cell = grid.Rows[i].Cells[0];
					if (cell.GetText() != year)
					{
						year = cell.GetText();
						startRow = i;
						cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
					}
					else if ((i == grid.Rows.Count - 1) || (grid.Rows[i + 1].Cells[0].GetText() != year))
					{
						cell.Style.BorderDetails.StyleTop = BorderStyle.None;
						grid.Rows[startRow + (i - startRow) / 2].Cells[0].Value = year;
					}
					else
					{
						cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
						cell.Style.BorderDetails.StyleTop = BorderStyle.None;
					}
					cell.Value = null;
				}
			}

			ReportPDFExporter1.HeaderCellHeight = 50;
			ReportPDFExporter1.Export(headerLayout, section1);

			UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
			ReportPDFExporter1.Export(UltraChart, LabelChart.Text + ", Сахалинская область", section2);
		}

		#endregion

		#region Экспорт в Excel

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
			ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

			Workbook workbook = new Workbook();
			Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            Worksheet sheet3 = workbook.Worksheets.Add("Для пустого грида");

			SetExportGridParams(headerLayout.Grid);

			ReportExcelExporter1.HeaderCellHeight = 25;
			ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
			ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
			ReportExcelExporter1.TitleStartRow = 0;

			UltraWebGrid grid = headerLayout.Grid;
			foreach (UltraGridRow row in grid.Rows)
				foreach (UltraGridCell cell in row.Cells)
					if (cell.Value == null)
						cell.Value = "-";

			ReportExcelExporter1.Export(headerLayout, sheet1, 3);

			//ReportExcelExporter1.WorksheetTitle = String.Empty;
			//ReportExcelExporter1.WorksheetSubTitle = String.Empty;
			UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
			ReportExcelExporter1.Export(UltraChart, LabelChart.Text, sheet2, 3);
            sheet1.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            sheet1.Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            sheet2.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            sheet2.Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;

            for (int i = sheet2.MergedCellsRegions.Count - 1; i >= 0; --i)
            {
                WorksheetMergedCellsRegion wmcr = sheet2.MergedCellsRegions[i];
                if (wmcr.FirstColumn == 0 && (wmcr.FirstRow == 0 || wmcr.FirstRow == 1))
                {
                    sheet2.MergedCellsRegions.Remove(wmcr);
                }
            }

            ReportExcelExporter1.Export(new GridHeaderLayout(emptyExportGrid), sheet3, 3);
            workbook.Worksheets.Remove(sheet3);
        }

		private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
		{
			e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
			e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
			e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
		}

		private static void SetExportGridParams(UltraWebGrid grid)
		{
			string exportFontName = "Verdana";
			int fontSize = 10;
			double coeff = 1.3;
			foreach (UltraGridColumn column in grid.Columns)
			{
				column.Width = Convert.ToInt32(column.Width.Value * coeff);
				column.CellStyle.Font.Name = exportFontName;
				column.Header.Style.Font.Name = exportFontName;
				column.CellStyle.Font.Size = fontSize;
				column.Header.Style.Font.Size = fontSize;
			}
		}

		#endregion

		// --------------------------------------------------------------------

		#region Заполнение словарей и выпадающих списков параметров

		private void GetQuarters(CustomParam firstQuarter, CustomParam lastQuarter, string queryName)
		{
			DataTable dtQuarters = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForPivotTable(query, dtQuarters);
			if (dtQuarters.Rows.Count == 0)
			{
				throw new Exception("Квартальные данные отсутствуют");
			}
			firstQuarter.Value = dtQuarters.Rows[0][3].ToString();
			lastQuarter.Value = dtQuarters.Rows[dtQuarters.Rows.Count - 1][3].ToString();
		}

		protected void AddPairToDictionary(Dictionary<string, int> dict, string key, int value)
		{
			if (!dict.ContainsKey(key))
			{
				dict.Add(key, value);
			}
		}

		#endregion

		static string Browser
		{
			get { return HttpContext.Current.Request.Browser.Browser; }
		}

	}
}
