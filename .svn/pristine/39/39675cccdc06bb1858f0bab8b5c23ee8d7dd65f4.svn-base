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
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebNavigator;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

/**
 *  Анализ розничных цен на социально значимые продовольственные товары по состоянию на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.ORG_0003_0013
{
	public partial class Default : CustomReportPage
	{
		#region Поля

		private DataTable dtGrid;
		private DataTable dtChart1;
		private GridHeaderLayout headerLayout;

		#endregion

		#region Параметры запроса

		// выбранный район/город
		private CustomParam selectedRegion;
		// выбранный продукт
		private CustomParam selectedFood;
		// первая актуальная дата
		private CustomParam firstDate;
		// выбранная дата
		private CustomParam selectedDate;

		#endregion
		// --------------------------------------------------------------------

		// заголовок страницы
		private static string page_title_caption = "Анализ розничных цен на социально значимые продовольственные товары по данным на {0} ({1})";
		private static string page_sub_title_caption = "Ежемесячный мониторинг средних розничных цен на социально значимые продовольственные товары";
		// заголовок для UltraChart
		private static string chart1_title_caption = "Динамика розничной цены на товар \"{0}\", рублей";

		private static string unitName = String.Empty;

		// --------------------------------------------------------------------

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			ComboDate.Title = "Выберите дату";
			ComboDate.Width = 300;
			ComboDate.ParentSelect = true;
			ComboRegion.Title = "Территория";
			ComboRegion.Width = 500;
			UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
			UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.35);
			UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
			UltraWebGrid.DisplayLayout.SelectTypeRowDefault = SelectType.Single;
			UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);

			#region Настройка диаграммы 1
			UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 12);
			UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.35);
			LabelChart1.Width = UltraChart1.Width;

			UltraChart1.ChartType = ChartType.StackAreaChart;
			UltraChart1.Border.Thickness = 0;

			UltraChart1.Axis.X.Extent = 50;
			UltraChart1.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL:N0>";
			UltraChart1.Axis.Y.Extent = 20;
			UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:##.##>";

			UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;

			UltraChart1.Tooltips.FormatString = "Розничная цена на <ITEM_LABEL:N0>\n<b><DATA_VALUE:N2></b> рублей";
			UltraChart1.DataBinding +=new EventHandler(UltraChart1_DataBinding);
			UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			PanelCharts.AddLinkedRequestTrigger(UltraWebGrid);

			UltraChart1.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart1.Axis.X.Margin.Near.Value = 20;
			UltraChart1.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
			UltraChart1.Axis.X.Margin.Far.Value = 20;

			#endregion

			#region Параметры
			selectedDate = UserParams.CustomParam("selected_date");
			selectedRegion = UserParams.CustomParam("selected_region");
			selectedFood = UserParams.CustomParam("selected_food");
			firstDate = UserParams.CustomParam("first_date");
			#endregion

			#region Экспорт
			ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
			ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
			ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;
			#endregion

		}

		// --------------------------------------------------------------------

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);
			if (!Page.IsPostBack)
			{
				fillComboDate("ORG_0003_0013_list_of_dates");
				fillComboRegion("ORG_0003_0013_list_of_regions");
			}
			#region Анализ параметров
			//selectedDateText = MDXDateToShortDateString(selectedDate.Value);
			//selectedRegionText = ComboRegion.SelectedValue;
			selectedRegion.Value = StringToMDXRegion(ComboRegion.SelectedValue);
			Node node = new Node();
			if (ComboDate.SelectedNode.Level == 0)
			{
				node = ComboDate.GetLastChild(ComboDate.GetLastChild(ComboDate.SelectedNode));
			}
			if (ComboDate.SelectedNode.Level == 1)
			{
				node = ComboDate.GetLastChild(ComboDate.SelectedNode);
			}
			if (ComboDate.SelectedNode.Level == 2)
			{
				node = ComboDate.SelectedNode;
			}
			selectedDate.Value = StringToMDXDate(node.Text);
			#endregion

			PageTitle.Text = String.Format(page_title_caption, MDXDateToString(selectedDate.Value), getLastBlock(selectedRegion.Value));
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = page_sub_title_caption;

			headerLayout = new GridHeaderLayout(UltraWebGrid);
			UltraWebGrid.Bands.Clear();
			UltraWebGrid.DataBind();
			UltraWebGrid_FillSceneGraph();
			UltraWebGrid_MarkByStars();
			for (int i = 0; i < UltraWebGrid.Rows.Count; i += 3)
			{
				UltraWebGrid.Rows[i].Cells[0].RowSpan = 3;
			}
			if (String.IsNullOrEmpty(selectedFood.Value))
			{
				selectedFood.Value = UltraWebGrid.Rows[0].Cells[UltraWebGrid.Columns.Count - 1].GetText();
			}
			UltraWebGrid_ActivateRow(CRHelper.FindGridRow(UltraWebGrid, selectedFood.Value, UltraWebGrid.Columns.Count - 1, 0));
			SetDynamicText(MDXDateToString(selectedDate.Value));
		}

		// --------------------------------------------------------------------

		#region Обработчики грида

		protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("ORG_0003_0013_grid");
			dtGrid = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование продукта", dtGrid);
			if (dtGrid.Rows.Count > 0)
			{
				for (int i = 0; i < dtGrid.Columns.Count; ++i)
				{
					dtGrid.Columns[i].Caption = MDXDateToShortDateString(dtGrid.Rows[0][i].ToString());
				}
				dtGrid.Rows.RemoveAt(0);
				for (int i = 1; i < dtGrid.Rows.Count; ++i)
				{
					if (dtGrid.Rows[i][dtGrid.Columns.Count - 2] == DBNull.Value)
					{
						dtGrid.Rows[i][dtGrid.Columns.Count - 2] = dtGrid.Rows[i - 1][dtGrid.Columns.Count - 2];
					}
					if (dtGrid.Rows[i][dtGrid.Columns.Count - 1] == DBNull.Value)
					{
						dtGrid.Rows[i][dtGrid.Columns.Count - 1] = dtGrid.Rows[i - 1][dtGrid.Columns.Count - 1];
					}
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
			if (getBrowser == "Firefox")
			{
				k = 0.95;
			}
			else if (getBrowser == "AppleMAC-Safari")
			{
				k = 0.9;
			}
			e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
			e.Layout.AllowSortingDefault = AllowSorting.No;
			e.Layout.CellClickActionDefault = CellClickAction.RowSelect;
			e.Layout.NullTextDefault = "-";
			e.Layout.RowSelectorStyleDefault.Width = 20;
			e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
			double gridWidth = UltraWebGrid.Width.Value - e.Layout.RowSelectorStyleDefault.Width.Value - 50;
			double columnWidth = CRHelper.GetColumnWidth(gridWidth * 0.8 / 12 * k);
			e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(gridWidth * 0.2 * k - 30);
			for (int i = 1; i < e.Layout.Bands[0].Columns.Count; ++i)
			{
				e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
				e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Left = 5;
			}

			// Заголовки
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
			headerLayout.AddCell("Наименование продукта");
			for (int i = 1; i < dtGrid.Columns.Count - 2; ++i)
			{
				string[] captions = dtGrid.Columns[i].Caption.Split(';');
				headerLayout.AddCell(captions[0]);
			}
			headerLayout.AddCell("Единица измерения");
			headerLayout.AddCell("MDX имя");
			headerLayout.ApplyHeaderInfo();

			e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
			e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 2].Hidden = true;
		}

		protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
		{
			e.Row.Cells[0].Text = e.Row.Cells[0].Text.Split(';')[0] + ", " + e.Row.Cells[e.Row.Cells.Count - 2].Text.ToLower();
			string cellFormat = e.Row.Index % 3 == 2 ? "{0:P2}" : "{0:N2}";
			for (int i = 1; i < e.Row.Cells.Count - 2; ++i)
			{
				double value;
				if (Double.TryParse(e.Row.Cells[i].GetText(), out value))
				{
					e.Row.Cells[i].Text = String.Format(cellFormat, Convert.ToDouble(e.Row.Cells[i].GetText()));
				}
			}
		}

		void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
		{
			if (PanelCharts.IsAsyncPostBack)
			{
				UltraWebGrid_ActivateRow(e.Row);
			}
		}

		protected void UltraWebGrid_ActivateRow(UltraGridRow row)
		{
			selectedFood.Value = row.Cells[row.Cells.Count - 1].GetText();
			unitName = row.Cells[row.Cells.Count - 2].GetText().ToLower();
			UltraChart1.DataBind();
		}

		protected void UltraWebGrid_MarkByStars()
		{
			for (int columnIndex = 1; columnIndex < UltraWebGrid.Columns.Count - 2; ++columnIndex)
			{
				string maxValueRows = String.Empty;
				string minValueRows = String.Empty;
				double maxValue = Double.NegativeInfinity;
				double minValue = Double.PositiveInfinity;
				int rowIndex = 0;
				for (rowIndex = 2; rowIndex < dtGrid.Rows.Count; rowIndex += 3)
				{
					DataRow row = dtGrid.Rows[rowIndex];
					double value;
					if (Double.TryParse(row[columnIndex].ToString(), out value))
					{
						if (value != 0)
						{
							if (value > 0)
							{
								if (value == maxValue)
								{
									maxValueRows = maxValueRows == String.Empty ? rowIndex.ToString() : maxValueRows + " " + rowIndex.ToString();
								}
								if (value > maxValue)
								{
									maxValue = value;
									maxValueRows = rowIndex.ToString();
								}
							}
							if (value < 0)
							{
								if (value == minValue)
								{
									minValueRows = minValueRows == String.Empty ? rowIndex.ToString() : minValueRows + " " + rowIndex.ToString();
								}
								if (value < minValue)
								{
									minValue = value;
									minValueRows = rowIndex.ToString();
								}
							}
						}
					}
				}
				string[] rows = null;
				if (!String.IsNullOrEmpty(maxValueRows))
				{
					rows = maxValueRows.Split(' ');
					foreach (string row in rows)
					{
						rowIndex = Convert.ToInt32(row);
						UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Style.BackgroundImage = "~/images/starGraybb.png";
						UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
						UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Title = "Самый высокий уровень тарифа";
					}
				}
				if (!String.IsNullOrEmpty(minValueRows))
				{
					rows = minValueRows.Split(' ');
					foreach (string row in rows)
					{
						rowIndex = Convert.ToInt32(row);
						UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Style.BackgroundImage = "~/images/starYellowbb.png";
						UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
						UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Title = "Самый низкий уровень тарифа";
					}
				}
			}
		}

		protected void UltraWebGrid_FillSceneGraph()
		{
			for (int i = 0; i < UltraWebGrid.Rows.Count; i += 3)
			{

				for (int j = 1; j < UltraWebGrid.Columns.Count - 2; ++j)
				{
					double value;
					if (Double.TryParse(UltraWebGrid.Rows[i + 1].Cells[j].Text, out value))
					{
						if (value > 0)
						{
							UltraWebGrid.Rows[i].Cells[j].Style.CssClass = "ArrowUpRed";
						}
						else if (value < 0)
						{
							UltraWebGrid.Rows[i].Cells[j].Style.CssClass = "ArrowDownGreen";
						}
					}
				}
			}
		}

		#endregion

		// --------------------------------------------------------------------

		#region Обработчики диаграммы 1

		void UltraChart1_DataBinding(object sender, EventArgs e)
		{
			LabelChart1.Text = String.Format(chart1_title_caption, getLastBlock(selectedFood.Value));
			string query = DataProvider.GetQueryText("ORG_0003_0013_chart1");
			dtChart1 = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Розничная цена", dtChart1);
			query = DataProvider.GetQueryText("ORG_0003_0013_chart1_names");
			DataTable dtChart1Names = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Розничная цена", dtChart1Names);
			double maxValue = Double.NegativeInfinity, minValue = Double.PositiveInfinity;
			for (int i = 1; i < dtChart1.Columns.Count; ++i)
			{
				dtChart1.Columns[i].ColumnName = MDXDateToShortDateString1(dtChart1Names.Rows[0][i].ToString());
				double value;
				if (Double.TryParse(dtChart1.Rows[0][i].ToString(), out value))
				{
					if (value > maxValue)
					{
						maxValue = value;
					}
					if (value < minValue)
					{
						minValue = value;
					}
				}
			}
			if (!Double.IsNegativeInfinity(maxValue) && !Double.IsPositiveInfinity(minValue))
			{
				UltraChart1.Axis.Y.RangeType = AxisRangeType.Custom;
				UltraChart1.Axis.Y.RangeMin = minValue * 0.9;
				UltraChart1.Axis.Y.RangeMax = maxValue * 1.1;
			}

			UltraChart1.DataSource = (dtChart1 == null) ? null : dtChart1.DefaultView;
		}
		#endregion

		// --------------------------------------------------------------------

		#region Заполнение словарей и выпадающих списков параметров

		protected void fillComboRegion(string queryName)
		{
			// Загрузка списка продуктов
			DataTable dtRegion = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtRegion);
			// Закачку придется делать через словарь
			Dictionary<string, int> dict = new Dictionary<string, int>();
			foreach (DataRow row in dtRegion.Rows)
			{
				addPairToDictionary(dict, row[4].ToString(), 0);
			}
			ComboRegion.FillDictionaryValues(dict);
			if (dict.ContainsKey("Город Вологда"))
			{
				ComboRegion.SetСheckedState("Город Вологда", true);
			}
		}

		protected void fillComboDate(string queryName)
		{
			// Загрузка списка актуальных дат
			DataTable dtDate = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
			// Закачку придется делать через словарь
			Dictionary<string, int> dictDate = new Dictionary<string, int>();
			for (int row = 0; row < dtDate.Rows.Count; ++row)
			{
				string year = dtDate.Rows[row][0].ToString();
				string month = dtDate.Rows[row][3].ToString();
				string day = dtDate.Rows[row][4].ToString();
				addPairToDictionary(dictDate, year + " год", 0);
				addPairToDictionary(dictDate, month + " " + year + " года", 1);
				addPairToDictionary(dictDate, day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года", 2);
				if (String.IsNullOrEmpty(firstDate.Value))
				{
					firstDate.Value = StringToMDXDate(day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года");
				}
			}
			ComboDate.FillDictionaryValues(dictDate);
			ComboDate.SelectLastNode();
		}

		protected void addPairToDictionary(Dictionary<string, int> dict, string key, int value)
		{
			if (!dict.ContainsKey(key))
			{
				dict.Add(key, value);
			}
		}

		#endregion

		#region Функции-полезняшки преобразования и все такое

		private static string getBrowser
		{
			get { return HttpContext.Current.Request.Browser.Browser; }
		}

		public string MDXDateToString(string MDXDateString)
		{
			string[] separator = { "].[" };
			string[] dateElements = MDXDateString.Split(separator, StringSplitOptions.None);
			string template = "{0} {1} {2} года";
			string day = dateElements[7].Replace("]", String.Empty);
			string month = CRHelper.RusMonthGenitive(CRHelper.MonthNum(dateElements[6].ToString()));
			string year = dateElements[3];
			return String.Format(template, day, month, year);
		}

		public string StringToMDXDate(string str)
		{
			string template = "[Период].[День].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]";
			string[] dateElements = str.Split(' ');
			int year = Convert.ToInt32(dateElements[2]);
			string month = CRHelper.RusMonth(CRHelper.MonthNum(dateElements[1]));
			int quarter = CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month));
			int halfYear = CRHelper.HalfYearNumByQuarterNum(quarter);
			int day = Convert.ToInt32(dateElements[0]);
			return String.Format(template, year, halfYear, quarter, month, day);
		}

		public string StringToMDXRegion(string region)
		{
			string template = "[Территории].[РФ].[Все территории].[Российская Федерация].[Северо-Западный федеральный округ].[Вологодская область].[Вологодская область].[{0}]";
			return String.Format(template, region);
		}

		public string MDXDateToShortDateString(string MDXDateString)
		{
			string[] separator = { "].[" };
			string[] dateElements = MDXDateString.Split(separator, StringSplitOptions.None);
			if (dateElements.Length < 8)
			{
				return MDXDateString;
			}
			string template = "{0:00}.{1:00}.{2}";
			int day = Convert.ToInt32(dateElements[7].Replace("]", String.Empty));
			int month = CRHelper.MonthNum(dateElements[6]);
			int year = Convert.ToInt32(dateElements[3]);
			return String.Format(template, day, month, year);
		}

		public string MDXDateToShortDateString1(string MDXDateString)
		{
			string[] separator = { "].[" };
			string[] dateElements = MDXDateString.Split(separator, StringSplitOptions.None);
			string template = "{0:00}.{1:00}.{2}";
			int day = Convert.ToInt32(dateElements[7].Replace("]", String.Empty));
			int month = CRHelper.MonthNum(dateElements[6]);
			int year = Convert.ToInt32(dateElements[3].Substring(2, 2));
			return String.Format(template, day, month, year);
		}

		public string getLastBlock(string mdxString)
		{
			if (String.IsNullOrEmpty(mdxString))
			{
				return String.Empty;
			}
			string[] separator = { "].[" };
			string[] stringElements = mdxString.Split(separator, StringSplitOptions.None);
			return stringElements[stringElements.Length - 1].Replace("]", String.Empty);
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

			UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

			ReportPDFExporter1.HeaderCellHeight = 70;
			GridHeaderLayout workHeader = new GridHeaderLayout();
			workHeader = headerLayout;
			if (UltraWebGrid.DataSource != null)
			{
				workHeader.childCells.Remove(workHeader.childCells[workHeader.childCells.Count - 1]);
				workHeader.childCells.Remove(workHeader.childCells[workHeader.childCells.Count - 1]);
			}
			ReportPDFExporter1.Export(workHeader, section1);
			ReportPDFExporter1.Export(UltraChart1, LabelChart1.Text, section2);
		}

		#endregion

		#region Экспорт в Excel

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
			ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

			Workbook workbook = new Workbook();
			Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
			Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма по времени");

			sheet1.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Portrait;
			sheet2.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;

			GridHeaderLayout workHeader = headerLayout;
			workHeader.childCells.RemoveAt(workHeader.childCells.Count - 1);
			workHeader.childCells.RemoveAt(workHeader.childCells.Count - 1);

			SetExportGridParams(workHeader.Grid);

			ReportExcelExporter1.HeaderCellHeight = 50;
			ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
			ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
			ReportExcelExporter1.TitleStartRow = 0;

			ReportExcelExporter1.Export(workHeader, sheet1, 3);

			sheet1.MergedCellsRegions.Clear();
			for (int i = 0; i < UltraWebGrid.Rows.Count; ++i)
			{
				sheet1.Rows[4 + i].Height = 255;
				if (i % 3 == 0)
					sheet1.MergedCellsRegions.Add(4 + i, 0, 6 + i, 0);
			}
			sheet1.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
			sheet1.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			sheet1.Rows[1].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
			sheet1.Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;

			ReportExcelExporter1.WorksheetTitle = String.Empty;
			ReportExcelExporter1.WorksheetSubTitle = String.Empty;

			UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.7));
			ReportExcelExporter1.Export(UltraChart1, LabelChart1.Text, sheet2, 1);
		}

		private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
		{
			//e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
			e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
			e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
		}

		private static void SetExportGridParams(UltraWebGrid grid)
		{
			string exportFontName = "Verdana";
			int fontSize = 10;
			double coeff = 1.35;
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

		#region Формирование динамического текста

		private void SetDynamicText(string string_date)
		{
            LabelDynamicText.Width = UltraWebGrid.Width;
            LabelDynamicText.Height = Unit.Empty;
            int columnIndex = UltraWebGrid.Columns.Count - 3;
            DateTime lastColumnDate;
            if (!DateTime.TryParse(dtGrid.Columns[columnIndex].Caption, out lastColumnDate) ||
                String.Format("{0:D}", lastColumnDate).ToLower() != string_date.Replace(" года", " г.").ToLower())
            {
                LabelDynamicText.Text = String.Format("На <b>{0}</b> данные по розничным ценам отсутствуют.", string_date);
                return;
            }
			Dictionary<string, double> dictDown = new Dictionary<string, double>();
			Dictionary<string, double> dictUp = new Dictionary<string, double>();
			bool hasData = false;
			string head = String.Empty, percentsMore2 = String.Empty, costDown = String.Empty;
			for (int j = 2; j < UltraWebGrid.Rows.Count; j += 3)
			{
				if (UltraWebGrid.Rows[j].Cells[columnIndex].Value != null)
				{
					double num_value;
					if (Double.TryParse(UltraWebGrid.Rows[j].Cells[columnIndex].Text.Replace("%", String.Empty), out num_value))
					{
						num_value /= 100;
						string name = "«" + getLastBlock(UltraWebGrid.Rows[j].Cells[UltraWebGrid.Columns.Count - 1].GetText()) + "»";
						hasData = true;
						if (num_value != 0)
						{
							if (num_value > 0)
							{
								if (num_value > 0.02)
								{
									dictUp.Add(name, num_value);
								}
							}
							else
							{
								if (num_value < -0.02)
								{
									dictDown.Add(name, num_value);
								}
							}
						}
					}
				}
			}
			if ((dictUp.Count == 0) && (dictDown.Count == 0))
			{
				if (hasData)
				{
					head = String.Format("&nbsp;&nbsp;&nbsp;По состоянию на <b>{0}</b> не наблюдалось изменение розничных цен на основные продовольственные товары",
						string_date);
				}
				else
				{
					head = String.Format("&nbsp;&nbsp;&nbsp;По состоянию на <b>{0}</b> данные об изменении розничных цен на продовольственные товары отсутствуют.",
						string_date);
				}
			}
			else
			{
				head = String.Format("&nbsp;&nbsp;&nbsp;По состоянию на <b>{0}</b> наблюдалось изменение розничных цен на продовольственные товары:",
					string_date);
			}
			LabelDynamicText.Text = head;
			if (dictUp.Count != 0)
			{
				string[] names = new string[dictUp.Count];
				double[] values = new double[dictUp.Count];
				dictUp.Keys.CopyTo(names, 0);
				dictUp.Values.CopyTo(values, 0);
				Array.Sort(values, names);
				percentsMore2 = String.Format("<b>{0}</b> (на <b>{1:P2}</b>)", names[names.Length - 1], values[names.Length - 1]);
				for (int i = names.Length - 2; i >= 0; --i)
				{
					percentsMore2 = String.Format("{0}, <b>{1}</b> (на <b>{2:P2}</b>)", percentsMore2, names[i], values[i]);
				}
				LabelDynamicText.Text += "<br/>&nbsp;&nbsp;&nbsp;- увеличение цен более чем на 2% на товары: " + percentsMore2 + ".";
			}
			if (dictDown.Count != 0)
			{
				string[] names = new string[dictDown.Count];
				double[] values = new double[dictDown.Count];
				dictDown.Keys.CopyTo(names, 0);
				dictDown.Values.CopyTo(values, 0);
				Array.Sort(values, names);
				percentsMore2 = String.Format("<b>{0}</b> (на <b>{1:P2}</b>)", names[0], values[0]);
				for (int i = 1; i < names.Length; ++i)
				{
					percentsMore2 = String.Format("{0}, <b>{1}</b> (на <b>{2:P2}</b>)", percentsMore2, names[i], values[i]);
				}
				LabelDynamicText.Text += "<br/>&nbsp;&nbsp;&nbsp;- снижение цен более чем на 2% на товары: " + percentsMore2 + ".";
			}
		}

		#endregion
	}
}
