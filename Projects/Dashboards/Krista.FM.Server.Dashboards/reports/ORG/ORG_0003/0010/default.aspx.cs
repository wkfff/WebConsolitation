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
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebNavigator;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

/**
 *  Анализ тарифов в отраслях хозяйственного комплекса по состоянию на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.ORG_0003_0010
{
	public partial class Default : CustomReportPage
	{
		#region Поля

		private DataTable dtGrid;
		private DataTable dtGridDates;
		private DataTable dtChart1;
		private DataTable dtChart2;
		private GridHeaderLayout headerLayout;

		#endregion

		#region Параметры запроса

		// выбранный район/город
		private CustomParam selectedRegion;
		// выбранный параметр грида
		private CustomParam selectedParameter;
		// выбранная дата
		private CustomParam selectedDate;
		// выбранная дата
		private CustomParam firstDate;

		#endregion
		// --------------------------------------------------------------------

		// заголовок страницы
		private static string page_title_caption = "Анализ тарифов в отраслях хозяйственного комплекса по состоянию на {0} ({1})";
		private static string page_sub_title_caption = "Ежемесячный мониторинг тарифов в отраслях хозяйственного комплекса на услуги, оказываемые населению";
		// заголовок для UltraChart
		private static string chart1_title_caption = "Динамика показателя «{0}», {1}";
		private static string chart2_title_caption = "Процент оплаты населением услуг ЖКХ от полной стоимости услуги, %";
		private static string UnitText;
		private static string ParameterText;

		// --------------------------------------------------------------------

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			ComboDate.Title = "Выберите дату";
			ComboDate.Width = 300;
			ComboDate.ParentSelect = true;
			ComboRegion.Title = "Территория";
			ComboRegion.Width = 500;
			PageSubTitle.Text = page_sub_title_caption;
			UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
			UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.35 + 10);
			UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
			UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);

			#region Настройка диаграммы 1
			UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.49 - 6);
			UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.35);
			LabelChart1.Width = UltraChart1.Width;

			UltraChart1.ChartType = ChartType.StackAreaChart;
			UltraChart1.Border.Thickness = 0;

			UltraChart1.Axis.X.Extent = 50;
			UltraChart1.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL:N0>";
			UltraChart1.Axis.Y.Extent = 30;
			UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";

			UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;

			UltraChart1.Tooltips.FormatString = "Тариф на <ITEM_LABEL:N0>\n<b><DATA_VALUE:N2></b> рублей";
			UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
			UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

			UltraChart1.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart1.Axis.X.Margin.Near.Value = 20;
			UltraChart1.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
			UltraChart1.Axis.X.Margin.Far.Value = 20;

			ChartTextAppearance appearance = new ChartTextAppearance();
			appearance.Column = -2;
			appearance.Row = -2;
			appearance.VerticalAlign = StringAlignment.Far;
			appearance.ItemFormatString = "<DATA_VALUE:N2>";
			appearance.ChartTextFont = new Font("Verdana", 8);
			appearance.Visible = true;
			UltraChart1.AreaChart.ChartText.Add(appearance);
			PanelCharts.AddLinkedRequestTrigger(UltraWebGrid);

			#endregion

			#region Настройка диаграммы 2
			UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.49 - 6);
			UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.35);
			LabelChart2.Width = UltraChart2.Width;

			UltraChart2.ChartType = ChartType.StackColumnChart;
			UltraChart2.Border.Thickness = 0;

			UltraChart2.Axis.X.Extent = 50;
			UltraChart2.Axis.Y.Extent = 50;
			UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>%";

			UltraChart2.Axis.X.Labels.SeriesLabels.Visible = true;
			//UltraChart2.Axis.X.Labels.SeriesLabels.FontSizeBestFit = true;
			//UltraChart2.Axis.X.Labels.SeriesLabels.WrapText = true;
			UltraChart2.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 9);
			UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;

			UltraChart2.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart2.Axis.X.Margin.Near.Value = 20;

			UltraChart2.ColorModel.ModelStyle = ColorModels.CustomSkin;
			PaintElement pe = new PaintElement();
			pe.ElementType = PaintElementType.SolidFill;
			pe.Fill = Color.Blue;
			pe.FillOpacity = 25;
			UltraChart2.ColorModel.Skin.PEs.Add(pe);
			pe = new PaintElement();
			pe.ElementType = PaintElementType.SolidFill;
			pe.Fill = Color.Red;
			pe.FillOpacity = 25;
			UltraChart2.ColorModel.Skin.PEs.Add(pe);
			UltraChart2.ColorModel.Skin.ApplyRowWise = false;

			UltraChart2.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>: <b><DATA_VALUE:N2>%</b>";
			UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
			UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);

			#endregion

			#region Параметры
			selectedDate = UserParams.CustomParam("selected_date");
			selectedRegion = UserParams.CustomParam("selected_region");
			selectedParameter = UserParams.CustomParam("selected_parameter");
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
				fillComboDate("ORG_0003_0010_list_of_dates");
				fillComboRegion("ORG_0003_0010_list_of_regions");
			}
			#region Анализ параметров
			Node node = new Node();
			if (ComboDate.SelectedNode.Level == 0)
			{
				node = ComboDate.GetLastChild(ComboDate.SelectedNode).FirstNode;
			}
			if (ComboDate.SelectedNode.Level == 1)
			{
				node = ComboDate.SelectedNode.FirstNode;
			}
			if (ComboDate.SelectedNode.Level == 2)
			{
				node = ComboDate.SelectedNode;
			}
			selectedDate.Value = StringToMDXDate(node.Text);
			selectedRegion.Value = StringToMDXRegion(ComboRegion.SelectedValue);
			#endregion

			PageTitle.Text = String.Format(page_title_caption, node.Text, ComboRegion.SelectedValue);
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = page_sub_title_caption;

			headerLayout = new GridHeaderLayout(UltraWebGrid);
			UltraWebGrid.Bands.Clear();
			UltraWebGrid.DataBind();
			if (selectedParameter.Value == String.Empty)
			{
				UltraWebGrid_ActivateRow(UltraWebGrid.Rows[1]);
				UltraWebGrid.Rows[1].Activate();
				UltraWebGrid.Rows[1].Selected = true;
			}
			else if (UltraWebGrid.DataSource != null)
			{
				UltraWebGrid_ActivateRow(CRHelper.FindGridRow(UltraWebGrid, selectedParameter.Value,
					UltraWebGrid.Columns.Count - 2, 1));
			}
			UltraChart2.DataBind();
		}

		// --------------------------------------------------------------------
		#region Обработчики грида

		protected void UltraWebGrid_ActivateRow(UltraGridRow row)
		{
			selectedParameter.Value = row.Cells[row.Cells.Count - 2].Text;
			UnitText = row.Cells[row.Cells.Count - 1].Text.ToLower();
			ParameterText = row.Cells[0].Value.ToString().Replace(", " + UnitText.ToLower(), String.Empty);
			UltraChart1.DataBind();
		}
		
		protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
			string prevServiceKind = String.Empty;
			string query = DataProvider.GetQueryText("ORG_0003_0010_grid");
			dtGrid = new DataTable();
			DataTable dtGridPart1 = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Жилищные услуги", dtGridPart1);
			dtGrid = dtGridPart1.Clone();
			if (dtGridPart1.Rows.Count > 0)
			{
				foreach (DataRow row in dtGridPart1.Rows)
				{
					bool rowNotEmpty = false;
					for (int i = 1; i < dtGridPart1.Columns.Count - 2; ++i)
					{
						double testValue;
						rowNotEmpty |= Double.TryParse(row[i].ToString(), out testValue);
					}
					if (rowNotEmpty)
					{
						string serviceKind = row[dtGridPart1.Columns.Count - 2].ToString().Replace("[", String.Empty).Replace("]", String.Empty).Split('.')[3];
						if (serviceKind != prevServiceKind)
						{
							DataRow kindRow = dtGrid.NewRow();
							kindRow[0] = serviceKind;
							dtGrid.Rows.Add(kindRow);
							prevServiceKind = serviceKind;
						}
						dtGrid.ImportRow(row);
					}
				}
				if (dtGrid.Rows.Count != 0)
				{
					UltraWebGrid.DataSource = dtGrid;
				}
				else
				{
					UltraWebGrid.DataSource = null;
				}
			}
			else
			{
				UltraWebGrid.DataSource = null;
			}
		}

		protected void UltraWebGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
		{
			double k = 0.9;
			if (getBrowser == "Firefox")
			{
				k = 0.95;
			}
			else if (getBrowser == "AppleMAC-Safari")
			{
				k = 0.9;
			}
			e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
			string query = DataProvider.GetQueryText("ORG_0003_0010_grid_dates");
			dtGridDates = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Даты", dtGridDates);

			e.Layout.RowSelectorStyleDefault.Width = 20;
			double gridWidth = UltraWebGrid.Width.Value - e.Layout.RowSelectorStyleDefault.Width.Value - 50;
			double columnWidth = CRHelper.GetColumnWidth(gridWidth * 2 / 3 / 12 * k + 25);
			e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(gridWidth / 3 * k);
			for (int i = 1; i < e.Layout.Bands[0].Columns.Count; ++i)
			{
				e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
				e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Left = 5;
				CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
			}

			// Заголовки
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
			headerLayout.AddCell("Показатели");
			string prevYear = String.Empty;
			GridHeaderCell header = null;
			for (int i = 1; i < dtGridDates.Columns.Count; ++i)
			{
				string year = getYearFromString(dtGridDates.Rows[0][i].ToString());
				if (year != prevYear)
				{
					header = headerLayout.AddCell(String.Format("Тариф в {0} году", year));
					prevYear = year;
				}
				header.AddCell(MDXDateToShortDateString1(dtGridDates.Rows[0][i].ToString()));
			}
			headerLayout.AddCell("MDX имя");
			headerLayout.AddCell("Единица измерения");
			headerLayout.ApplyHeaderInfo();

			e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
			e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 2].Hidden = true;
		}

		protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
		{
			if (e.Row.Cells[0].Text.ToLower() == "жилищные услуги" || e.Row.Cells[0].Text.ToLower() == "коммунальные услуги")
			{
				e.Row.Cells[0].ColSpan = e.Row.Cells.Count - 2;
				e.Row.Cells[0].Style.Font.Bold = true;
			}
			else
			{
				e.Row.Cells[0].Text += ", " + e.Row.Cells[e.Row.Cells.Count - 1].Text.ToLower();
				for (int i = 1; i < e.Row.Cells.Count - 2; ++i)
				{
					double prevValue, value;
					if (Double.TryParse(e.Row.Cells[i - 1].Text, out prevValue) && Double.TryParse(e.Row.Cells[i].Text, out value))
					{
						if (value > prevValue)
						{
							e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedbb.png";
							e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
							e.Row.Cells[i].Title = "Повышение тарифа";
						}
						else if (value < prevValue)
						{
							e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
							e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
							e.Row.Cells[i].Title = "Снижение тарифа";
						}
					}
				}
			}
		}

		void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
		{
			if (PanelCharts.IsAsyncPostBack)
			{
				if (e.Row.Cells[0].Text.ToLower() == "жилищные услуги" || e.Row.Cells[0].Text.ToLower() == "коммунальные услуги")
				{
					e.Cancel = true;
				}
				else
				{
					UltraWebGrid_ActivateRow(e.Row);
				}
			}
		}

		#endregion

		// --------------------------------------------------------------------

		#region Обработчики диаграммы 1

		void UltraChart1_DataBinding(object sender, EventArgs e)
		{
			LabelChart1.Text = String.Format(chart1_title_caption, ParameterText, UnitText);
			string query = DataProvider.GetQueryText("ORG_0003_0010_chart_by_time");
			dtChart1 = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Розничная цена", dtChart1);
			query = DataProvider.GetQueryText("ORG_0003_0010_chart_by_time_names");
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

		#region Обработчики диаграммы 2
		protected void UltraChart2_DataBinding(object sender, EventArgs e)
		{
			LabelChart2.Text = String.Format(chart2_title_caption);
			string query = DataProvider.GetQueryText("ORG_0003_0010_chart2");
			dtChart2 = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart2);
			if (dtChart2.Rows.Count != 0)
			{
				UltraChart2.Series.Clear();
				UltraChart2.Data.SwapRowsAndColumns = true;
				for (int i = 1; i < dtChart2.Columns.Count; i++)
				{
					NumericSeries series = CRHelper.GetNumericSeries(i, dtChart2);
					series.Label = dtChart2.Columns[i].ColumnName;
					UltraChart2.Series.Add(series);
				}
			}
			else
			{
				UltraChart2.DataSource = null;
			}
		}

		protected void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];
				if (primitive.Path != null && primitive.Path == "Border.Title.Grid.X" && primitive is Text)
				{
					Text text = primitive as Text;
					text.labelStyle.Font = new Font("Verdana", 8);
				}
			}
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
				if (firstDate.Value == String.Empty)
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

		public string StringToMDXRegion(string comboRegionValue)
		{
			return String.Format("[Территории].[РФ].[Все территории].[Российская Федерация].[Северо-Западный федеральный округ].[Вологодская область].[Вологодская область].[{0}]",
				comboRegionValue);
		}

		public string MDXDateToShortDateString(string MDXDateString)
		{
			string[] separator = { "].[" };
			string[] dateElements = MDXDateString.Split(separator, StringSplitOptions.None);
			string template = "{0}.{1}.{2} г.";
			string day = dateElements[7].Substring(0, dateElements[7].Length - 1);
			day = day.Length == 1 ? "0" + day : day;
			string month = CRHelper.MonthNum(dateElements[6]).ToString();
			month = month.Length == 1 ? "0" + month : month;
			string year = dateElements[3];
			return String.Format(template, day, month, year);
		}

		public string MDXDateToShortDateString1(string MDXDateString)
		{
			string[] separator = { "].[" };
			string[] dateElements = MDXDateString.Split(separator, StringSplitOptions.None);
			string template = "{0}.{1}.{2}";
			string day = dateElements[7].Replace("]", String.Empty);
			day = day.Length == 1 ? "0" + day : day;
			string month = CRHelper.MonthNum(dateElements[6]).ToString();
			month = month.Length == 1 ? "0" + month : month;
			string year = dateElements[3].Substring(2, 2);
			return String.Format(template, day, month, year);
		}

		public bool isPreviousMonth(string firstMonth, string secondMonth)
		{
			int MonthNumDelta = CRHelper.MonthNum(firstMonth) - CRHelper.MonthNum(secondMonth);
			return ((MonthNumDelta == 1) || (MonthNumDelta == 11));
		}

		public string getMonthFromString(string date)
		{
			string[] dateElements = date.Split(' ');
			return dateElements[1];
		}

		public string getYearFromString(string mdxDate)
		{
			string[] dateElements = mdxDate.Replace("]", String.Empty).Replace("[", String.Empty).Split('.');
			return dateElements[3];
		}

		public string getYearDate(string date)
		{
			string[] dateElements = date.Split(' ');
			return String.Format("11 января {0} года", dateElements[2]);
		}

		public string replaceMonth(string date)
		{
			string[] dateElements = date.Split(' ');
			int monthIndex = CRHelper.MonthNum(dateElements[1]);
			int year = Convert.ToInt32(dateElements[2]);
			string newMonth = String.Empty;
			if (monthIndex == 1)
			{
				newMonth = "декабря";
				--year;
			}
			else
			{
				newMonth = CRHelper.RusMonthGenitive(monthIndex - 1);
			}
			return String.Format("1 {0} {1} года", newMonth, year);
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
			ISection section3 = report.AddSection();

			UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
			UltraChart2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

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
			ReportPDFExporter1.Export(UltraChart2, LabelChart2.Text, section3);
		}

		#endregion

		#region Экспорт в Excel

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
			ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

			Workbook workbook = new Workbook();
			Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
			Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма 1");
			Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма 2");

			SetExportGridParams(headerLayout.Grid);

			ReportExcelExporter1.HeaderCellHeight = 10;
			ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
			ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
			ReportExcelExporter1.TitleStartRow = 0;

			GridHeaderLayout workHeader = new GridHeaderLayout();
			workHeader = headerLayout;
			if (UltraWebGrid.DataSource != null)
			{
				workHeader.childCells.Remove(workHeader.childCells[workHeader.childCells.Count - 1]);
				workHeader.childCells.Remove(workHeader.childCells[workHeader.childCells.Count - 1]);
				ReportExcelExporter1.Export(workHeader, sheet1, 3);
			}

			ReportExcelExporter1.WorksheetTitle = String.Empty;
			ReportExcelExporter1.WorksheetSubTitle = String.Empty;

			if (UltraChart1.DataSource != null)
			{
				ReportExcelExporter1.Export(UltraChart1, LabelChart1.Text, sheet2, 1);
			}
			if (UltraChart2.Series.Count > 0)
			{
				ReportExcelExporter1.Export(UltraChart2, LabelChart2.Text, sheet3, 1);
			}

			// Ручная настройка
			sheet1.Columns[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
			sheet1.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
			sheet1.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			sheet1.Rows[1].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
			sheet1.Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			foreach (WorksheetMergedCellsRegion region in sheet1.MergedCellsRegions)
			{
				if (region.FirstColumn == 0 && region.FirstRow == 0)
				{
					sheet1.MergedCellsRegions.Remove(region);
					break;
				}
			}
			foreach (WorksheetMergedCellsRegion region in sheet1.MergedCellsRegions)
			{
				if (region.FirstColumn == 0 && region.FirstRow == 1)
				{
					sheet1.MergedCellsRegions.Remove(region);
					break;
				}
			}
			sheet2.MergedCellsRegions.Clear();
			sheet3.MergedCellsRegions.Clear();
		}

		private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
		{
			e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
			e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
			e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
			e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
			e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
			e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
			e.CurrentWorksheet.PrintOptions.TopMargin = 0;
			e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
			e.CurrentWorksheet.PrintOptions.RightMargin = 0;
		}

		private static void SetExportGridParams(UltraWebGrid grid)
		{
			string exportFontName = "Verdana";
			int fontSize = 10;
			double coeff = 1;
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
	}
}
