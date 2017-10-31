using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.UltraChart.Core.Primitives;
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

namespace Krista.FM.Server.Dashboards.reports.STAT_0001_0020
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
		private static Dictionary<string, string> dictRegion;

		#endregion

		#region Параметры запроса

		private CustomParam selectedRegion;
		private CustomParam selectedParameter;
		private CustomParam firstQuarter;
		private CustomParam lastQuarter;
		private CustomParam group;

		#endregion

		// --------------------------------------------------------------------

		// заголовок страницы
        private const string PageTitleCaption = "Динамика и структура иностранных инвестиций";
        private const string PageSubTitleCaption = "Ежеквартальный мониторинг иностранных инвестиций, {0}";
		// заголовок для UltraChart
		private const string ChartTitleCaption = "Динамика и структура иностранных инвестиций по видам инвестиций, тыс. дол. США";

		// --------------------------------------------------------------------

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

            ComboRegion.Title = "Выберите территорию";
			ComboRegion.Width = 600;
			ComboRegion.ParentSelect = true;
			PageTitle.Text = PageTitleCaption;
			Page.Title = PageTitleCaption;
			LabelChart.Text = ChartTitleCaption;

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
			UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);
			LabelChart.Width = UltraChart.Width;

			UltraChart.ChartType = ChartType.StackColumnChart;
			UltraChart.ColumnChart.NullHandling = NullHandling.DontPlot;

			UltraChart.Border.Thickness = 0;

			UltraChart.Axis.X.Extent = 200;
			UltraChart.Axis.X.LineThickness = 1;
			UltraChart.Axis.Y.Extent = 50;
			UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

			UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
			UltraChart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 12);
			UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

			UltraChart.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart.Axis.X.Margin.Near.Value = 20;

			UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;

			UltraChart.Legend.Visible = true;
            UltraChart.Legend.Margins.Right = 500;
			UltraChart.Legend.Location = LegendLocation.Bottom;
			UltraChart.Legend.SpanPercentage = 8;
			UltraChart.Legend.Font = new Font("Microsoft Sans Serif", 9);
			UltraChart.Border.Thickness = 0;

			UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>\n<b><DATA_VALUE:N2></b> тыс. дол. США";
			UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
			UltraChart.DataBinding += new EventHandler(UltraChart1_DataBinding);

			#endregion

			#region Параметры
			selectedRegion = UserParams.CustomParam("selected_region");
			selectedParameter = UserParams.CustomParam("selected_parameter");
			firstQuarter = UserParams.CustomParam("first_quarter");
			lastQuarter = UserParams.CustomParam("last_quarter");
			group = UserParams.CustomParam("group");
			#endregion

			#region Экспорт
			ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
			ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

			ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
			#endregion
		}

		// --------------------------------------------------------------------

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);
			if (!Page.IsPostBack)
			{
				FillComboRegion(ComboRegion, "STAT_0001_0020_list_of_regions");
				GetQuarters(firstQuarter, lastQuarter, "STAT_0001_0020_quarters");
			}
			#region Анализ параметров
			string mdxRegion; 
			dictRegion.TryGetValue(ComboRegion.SelectedValue, out mdxRegion);
			if (ComboRegion.SelectedValue == "Ханты-Мансийский автономный округ - Югра")
			{
				group.Value = "[Группировки__Инвестиции_Иностранные вложения].[Группировки__Инвестиции_Иностранные вложения].[Все группировки].[По видам инвестиций]";
			}
			else
			{
				group.Value = "[Группировки__Инвестиции_Иностранные вложения].[Группировки__Инвестиции_Иностранные вложения].[Все группировки].[По видам инвестиций, по городам и районам]";
			}
			selectedRegion.Value = mdxRegion;
			#endregion

			PageSubTitle.Text = String.Format(PageSubTitleCaption, ComboRegion.SelectedValue);

			headerLayout = new GridHeaderLayout(UltraWebGrid);

			UltraChart.DataBind();
			UltraWebGrid.DataBind();
		}

		// --------------------------------------------------------------------

		#region Обработчики грида

		protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("STAT_0001_0020_grid");
			dtGrid = new DataTable();
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);
			if (dtGrid.Rows.Count > 0)
			{
				for (int i = 0; i < dtGrid.Rows.Count; i += 3)
				{
					dtGrid.Rows[i + 1][1] = dtGrid.Rows[i + 2][1] = dtGrid.Rows[i][1];
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
			e.Layout.CellClickActionDefault = CellClickAction.CellSelect;
			e.Layout.NullTextDefault = "-";
			e.Layout.RowSelectorStyleDefault.Width = 20;
			e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
			e.Layout.Bands[0].Columns[0].MergeCells = true;
			e.Layout.Bands[0].Columns[1].MergeCells = true;
			double gridWidth = UltraWebGrid.Width.Value - 10;
			double columnWidth = CRHelper.GetColumnWidth(gridWidth / 10 * k);
			for (int i = 0; i < e.Layout.Bands[0].Columns.Count; ++i)
			{
				e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
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
			header = headerLayout.AddCell("Иностранные инвестиции из-за рубежа в субъект РФ, тыс. дол. США");
			header.AddCell("Всего инвестиций");
			header1 = header.AddCell("В том числе");
			header1.AddCell("Прямые инвестиции");
			header1.AddCell("Портфельные инвестиции");
			header1.AddCell("Прочие инвестиции");
			header = headerLayout.AddCell("Иностранные инвестиции из субъекта РФ за рубеж, тыс. дол. США");
			header.AddCell("Всего инвестиций");
			header1 = header.AddCell("В том числе");
			header1.AddCell("Прямые инвестиции");
			header1.AddCell("Портфельные инвестиции");
			header1.AddCell("Прочие инвестиции");
			headerLayout.ApplyHeaderInfo();

            for (int i = 1; i < UltraWebGrid.Columns.Count; i++)
            {
                UltraWebGrid.Columns[i].DataType = "decimal";
            }
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
						cell.Style.CssClass = value > 1 ? "ArrowUpGreen" : value < 1 ? "ArrowDownRed" : String.Empty;
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

		void UltraChart1_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("STAT_0001_0020_chart");
			dtChart = new DataTable();
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
			dtChart.Columns.RemoveAt(0);
			foreach (DataRow row in dtChart.Rows)
			{
				string[] elements = row[0].ToString().Split(';');
				if (elements.Length == 3)
				{
					string quarter = elements[1].Split(' ')[1];
					row[0] = String.Format("{0} квартал {1} года\n{2}", quarter, elements[0], elements[2]);
				}
				else
				{
					row[0] = "Нет данных";
				}
			}
			UltraChart.Series.Clear();
			UltraChart.Data.SwapRowsAndColumns = true;
			for (int i = 1; i < dtChart.Columns.Count; i++)
			{
				NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
				UltraChart.Series.Add(series);
			}
		}

		void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{

            List<Primitive> DeleteList = new List<Primitive>();

            int k = 0;
            for (int i = 0; i < e.SceneGraph.Count; ++i)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    try
                    {
                        Box b = (Box)primitive;
                        if (b.rect.Height < 1)
                        {
                            DeleteList.Add(primitive);
                        }
                        else
                        if (primitive.Series.Label == "Нет данных")
                        {
                            DeleteList.Add(primitive);
                        }
                    }
                    catch { } 
                }

                if (primitive is Text)
                {
                    Text text = primitive as Text;
                    if (text.GetTextString() == "Нет данных")
                    {
                        text.SetTextString(String.Empty);
                    }
                    if ((UltraChart.Height.Value - text.bounds.Y) < 50)
                    {
                        text.bounds.X += 40*k;
                        text.bounds.Width = text.bounds.Width + 10;                        
                    }
                }
                if (primitive.Path != null && primitive.Path == "Legend" && primitive is Box)
                {
                    Box box = primitive as Box;
                    box.rect.X += 40*k;
                    k++;

                }               
            }
            foreach (Primitive p in DeleteList)
            {
                e.SceneGraph.Remove(p);
            }
		}

		#endregion

		#region Экспорт в PDF

		private void PdfExportButton_Click(object sender, EventArgs e)
		{
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

			ReportPDFExporter1.HeaderCellHeight = 20;
			ReportPDFExporter1.Export(headerLayout, section1);

			UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
			ReportPDFExporter1.Export(UltraChart, LabelChart.Text, section2);
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

			sheet1.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Portrait;
			sheet2.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;

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

			ReportExcelExporter1.WorksheetTitle = String.Empty;
			ReportExcelExporter1.WorksheetSubTitle = String.Empty;
			UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.7));
            UltraChart.Legend.Margins.Right = 300;
			sheet2.MergedCellsRegions.Clear();
			ReportExcelExporter1.Export(UltraChart, LabelChart.Text, sheet2, 1);
		}

		private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
		{
			e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
			e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
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

		protected void FillComboRegion(CustomMultiCombo combo, string queryName)
		{
			DataTable dtRegion = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForPivotTable(query, dtRegion);
			if (dtRegion.Rows.Count == 0)
			{
				throw new Exception("Муниципальных образований не найдено");
			}
			// Закачку придется делать через словарь
			Dictionary<string, int> dict = new Dictionary<string, int>();
			dictRegion = new Dictionary<string, string>();
			int level = 0;
			for (int row = 0; row < dtRegion.Rows.Count; ++row)
			{
				string region = dtRegion.Rows[row][4].ToString();
				if (region.Contains("ДАННЫЕ"))
				{
					region = region.Replace("ДАННЫЕ", String.Empty).Replace(")", String.Empty).Replace("(", String.Empty).Trim();
					AddPairToDictionary(dict, region, 0);
					level = 1;
				}
				else
				{
					AddPairToDictionary(dict, region, level);
				}
				dictRegion.Add(region, dtRegion.Rows[row][5].ToString());
			}
			combo.FillDictionaryValues(dict);
			combo.SetСheckedState("Ханты-Мансийский автономный округ - Югра", true);
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
