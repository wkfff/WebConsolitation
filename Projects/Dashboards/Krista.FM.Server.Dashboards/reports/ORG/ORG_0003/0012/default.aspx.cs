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
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebNavigator;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

/**
 *  Анализ розничных цен на социально значимые продовольственные товары по состоянию на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.ORG_0003_0012
{
	public partial class Default : CustomReportPage
	{
		#region Поля

		private DataTable dtDate;
		private DataTable dtGrid;
		private DataTable dtChart1;
		private DataTable dtChart2;
		private GridHeaderLayout headerLayout;
		private static Dictionary<string, string> dictDates;

		#endregion

		#region Параметры запроса

		// выбранная дата
		private CustomParam selectedDate;
		private CustomParam prevDate;
		private CustomParam selectedRegion;
		private CustomParam firstDate;
		#endregion

		// --------------------------------------------------------------------

		// заголовок страницы
		private static string page_title_caption = "Информация о ценах на бензин и дизельное топливо по г. Вологде и областным центрам соседних регионов на {0}";
		private static string page_sub_title_caption = "Еженедельный мониторинг средних розничных цен на нефтепродукты в разрезе территорий и видов топлива";
		private static string chart1_title_caption = "Динамика розничной цены ({0}), рублей";
		private static string chart2_title_caption = "Уровень цен на различные виды топлива в разрезе территорий на {0}, рублей";
		private static string ParameterText;

		// --------------------------------------------------------------------

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			ComboDate.Title = "Выберите дату";
			ComboDate.Width = 300;
			ComboDate.ParentSelect = true;
			UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30);
			UltraWebGrid.Height = Unit.Empty;
			UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
			UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
			UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
			UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);

			#region Настройка диаграммы по времени
			UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
			UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 125);
			LabelChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);

			UltraChart1.ChartType = ChartType.LineChart;
			UltraChart1.Border.Thickness = 0;

			UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
			PaintElement pe = new PaintElement();
			pe.ElementType = PaintElementType.SolidFill;
			pe.Fill = Color.Green;
			pe.FillOpacity = 150;
			pe.StrokeWidth = 3;
			UltraChart1.ColorModel.Skin.PEs.Add(pe);
			pe = new PaintElement();
			pe.ElementType = PaintElementType.SolidFill;
			pe.Fill = Color.Blue;
			pe.FillOpacity = 150;
			pe.StrokeWidth = 3;
			UltraChart1.ColorModel.Skin.PEs.Add(pe);
			pe = new PaintElement();
			pe.ElementType = PaintElementType.SolidFill;
			pe.Fill = Color.Yellow;
			pe.FillOpacity = 150;
			pe.StrokeWidth = 3;
			UltraChart1.ColorModel.Skin.PEs.Add(pe);
			pe = new PaintElement();
			pe.ElementType = PaintElementType.SolidFill;
			pe.Fill = Color.Red;
			pe.FillOpacity = 150;
			pe.StrokeWidth = 3;
			UltraChart1.ColorModel.Skin.PEs.Add(pe);

			UltraChart1.Legend.Visible = true;
			UltraChart1.Legend.Location = LegendLocation.Top;
			UltraChart1.Legend.SpanPercentage = 15;
			UltraChart1.Border.Thickness = 0;

			UltraChart1.Axis.X.Extent = 50;
			UltraChart1.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL:N0>";
			UltraChart1.Axis.Y.Extent = 20;
			UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:##.##>";

			UltraChart1.Tooltips.FormatString = "<SERIES_LABEL>\nРозничная цена на <ITEM_LABEL:N0>\n<b><DATA_VALUE:N2></b> рублей";
			UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
			UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

			UltraChart1.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart1.Axis.X.Margin.Near.Value = 20;
			UltraChart1.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
			UltraChart1.Axis.X.Margin.Far.Value = 20;
			PanelCharts.AddLinkedRequestTrigger(UltraWebGrid);

			#endregion

			#region Настройка диаграммы по районам
			UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
			UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 75);
			LabelChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
			UltraChart2.ChartType = ChartType.ColumnChart;
			UltraChart2.Border.Thickness = 0;

			UltraChart2.ColumnChart.SeriesSpacing = 1;

			UltraChart2.Axis.X.Extent = 25;
			UltraChart2.Axis.X.Labels.Visible = false;
			UltraChart2.Axis.X.Labels.SeriesLabels.Visible = true;
			UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
			UltraChart2.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
			UltraChart2.Axis.Y.Extent = 20;
			UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:##.##>";

			UltraChart2.ColorModel.ModelStyle = ColorModels.CustomSkin;
			pe = new PaintElement();
			pe.ElementType = PaintElementType.SolidFill;
			pe.Fill = Color.Green;
			pe.FillOpacity = 25;
			UltraChart2.ColorModel.Skin.PEs.Add(pe);
			pe = new PaintElement();
			pe.ElementType = PaintElementType.SolidFill;
			pe.Fill = Color.Blue;
			pe.FillOpacity = 25;
			UltraChart2.ColorModel.Skin.PEs.Add(pe);
			pe = new PaintElement();
			pe.ElementType = PaintElementType.SolidFill;
			pe.Fill = Color.Yellow;
			pe.FillOpacity = 25;
			UltraChart2.ColorModel.Skin.PEs.Add(pe);
			pe = new PaintElement();
			pe.ElementType = PaintElementType.SolidFill;
			pe.Fill = Color.Red;
			pe.FillOpacity = 25;
			UltraChart2.ColorModel.Skin.PEs.Add(pe);
			UltraChart2.ColorModel.Skin.ApplyRowWise = false;

			UltraChart2.Legend.Visible = true;
			UltraChart2.Legend.Location = LegendLocation.Top;
			UltraChart2.Legend.SpanPercentage = 15;
			UltraChart2.Border.Thickness = 0;

			ChartTextAppearance appearance = new ChartTextAppearance();
			appearance.Column = -2;
			appearance.Row = -2;
			appearance.VerticalAlign = StringAlignment.Far;
			appearance.ItemFormatString = "<DATA_VALUE:N2>";
			appearance.ChartTextFont = new Font("Verdana", 8);
			appearance.Visible = true;
			UltraChart2.ColumnChart.ChartText.Add(appearance);

			UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>";
			UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);
			UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
			UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			#endregion

			#region Параметры
			selectedDate = UserParams.CustomParam("selected_date");
			prevDate = UserParams.CustomParam("prev_date");
			firstDate = UserParams.CustomParam("first_date");
			selectedRegion = UserParams.CustomParam("selected_region");

			#endregion

			#region Настройка экспорта
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
				fillComboDate("ORG_0003_0012_list_of_dates");
			}
			#region Анализ параметров
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
			string prevDateString = String.Empty;
			dictDates.TryGetValue(node.Text, out prevDateString);
			prevDate.Value = StringToMDXDate(prevDateString);
			PageTitle.Text = String.Format(page_title_caption, node.Text);
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = page_sub_title_caption;
			#endregion
			headerLayout = new GridHeaderLayout(UltraWebGrid);
			UltraWebGrid.Bands.Clear();
			UltraWebGrid.DataBind();
			UltraWebGrid_MarkByStars();
			if (UltraWebGrid.DataSource != null)
			{
				if (selectedRegion.Value == String.Empty)
				{
					selectedRegion.Value = UltraWebGrid.Rows[0].Cells[UltraWebGrid.Columns.Count - 1].Text;
				}
				UltraWebGrid_ActivateRow(CRHelper.FindGridRow(UltraWebGrid, selectedRegion.Value, UltraWebGrid.Columns.Count - 1, 0));
			}
			UltraChart2.DataBind();
		}

		// --------------------------------------------------------------------

		#region Обработчики грида

		protected void UltraWebGrid_ActivateRow(UltraGridRow row)
		{
			selectedRegion.Value = row.Cells[row.Cells.Count - 1].Text;
			ParameterText = row.Cells[0].Text;
			UltraChart1.DataBind();
		}

		void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
		{
			if (PanelCharts.IsAsyncPostBack)
			{
				UltraWebGrid_ActivateRow(e.Row);
			}
		}

		protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("ORG_0003_0012_grid");
			dtGrid = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Территория", dtGrid);
			if (dtGrid.Rows.Count > 0)
			{
				UltraWebGrid.DataSource = dtGrid;
			}
			else
			{
				UltraWebGrid.DataSource = null;
			}
		}

		protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
		{
			for (int i = 3; i < e.Row.Cells.Count; i += 3)
			{
				if (e.Row.Cells[i] != null)
				{
					if (Convert.ToDouble(e.Row.Cells[i].Value) < 0)
					{
						e.Row.Cells[i].Style.CssClass = "ArrowDownGreen";
					}
					else if (Convert.ToDouble(e.Row.Cells[i].Value) > 0)
					{
						e.Row.Cells[i].Style.CssClass = "ArrowUpRed";
					}
				}
				e.Row.Cells[i - 1].Title = "Абсолютное отклонение по отношению к " + MDXDateToShortDateString(prevDate.Value);
				e.Row.Cells[i].Title = "Темп прироста к " + MDXDateToShortDateString(prevDate.Value);
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
			e.Layout.CellClickActionDefault = CellClickAction.RowSelect;
			e.Layout.NullTextDefault = "-";
			e.Layout.RowSelectorStyleDefault.Width = 20;
			e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
			double gridWidth = UltraWebGrid.Width.Value - e.Layout.RowSelectorStyleDefault.Width.Value - 50;
			double columnWidth = CRHelper.GetColumnWidth(gridWidth * 0.8 / 12 * k);
			e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(gridWidth * 0.2 * k - 30);
			for (int i = 1; i < e.Layout.Bands[0].Columns.Count; ++i)
			{
				if (i % 3 != 2)
					e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
				else
					e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth * 1.1);
				e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Left = 5;
				if (i % 3 == 0)
				{
					CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
				}
				else
				{
					CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
				}
			}

			// Заголовки
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
			headerLayout.AddCell("Территория");
			GridHeaderCell header = null;
			string prevFuel = String.Empty;
			for (int i = 1; i < dtGrid.Columns.Count - 1; ++i)
			{
				string[] captions = dtGrid.Columns[i].Caption.Split(';');
				if (prevFuel != captions[0])
				{
					header = headerLayout.AddCell(captions[0]);
					prevFuel = captions[0];
				}
				if (captions[1].ToLower().Trim() == "текущая цена")
				{
					header.AddCell("Цена, рублей");
				}
				else if (captions[1].ToLower().Trim() == "абсолютное отклонение")
				{
					header.AddCell("Абсолютное отклонение, рублей");
				}
				else
				{
					header.AddCell(captions[1]);
				}
			}
			headerLayout.AddCell("MDX имя");
			headerLayout.ApplyHeaderInfo();

			e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
		}

		protected void UltraWebGrid_MarkByStars()
		{
			for (int columnIndex = 1; columnIndex < UltraWebGrid.Columns.Count - 1; columnIndex += 3)
			{
				string maxValueRows = String.Empty;
				string minValueRows = String.Empty;
				double maxValue = Double.NegativeInfinity;
				double minValue = Double.PositiveInfinity;
				int rowIndex = 0;
				foreach (DataRow row in dtGrid.Rows)
				{
					double value;
					if (Double.TryParse(row[columnIndex].ToString(), out value))
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
					++rowIndex;
				}
				string[] rows = maxValueRows.Split(' ');
				foreach (string row in rows)
				{
                    if (Int32.TryParse(row, out rowIndex))
                    {
                        UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Style.BackgroundImage = "~/images/starGraybb.png";
                        UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Title = "Самый высокий уровень тарифа";
                    }
				}
				rows = minValueRows.Split(' ');
				foreach (string row in rows)
				{
                    if (Int32.TryParse(row, out rowIndex))
                    {
                        UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Style.BackgroundImage = "~/images/starYellowbb.png";
                        UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Title = "Самый низкий уровень тарифа";
                    }
				}
			}
		}

		#endregion

		// --------------------------------------------------------------------

		#region Обработчики диаграммы 1

		void UltraChart1_DataBinding(object sender, EventArgs e)
		{
			LabelChart1.Text = String.Format(chart1_title_caption, ParameterText);
			string query = DataProvider.GetQueryText("ORG_0003_0012_chart_by_time");
			dtChart1 = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Розничная цена", dtChart1);
			query = DataProvider.GetQueryText("ORG_0003_0012_chart_by_time_names");
			DataTable dtChart1Names = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Розничная цена", dtChart1Names);
			double maxValue = Double.NegativeInfinity, minValue = Double.PositiveInfinity;
			for (int i = 1; i < dtChart1.Columns.Count; ++i)
			{
				dtChart1.Columns[i].ColumnName = MDXDateToShortDateString1(dtChart1Names.Rows[0][i].ToString());
				for (int j = 0; j < dtChart1.Rows.Count; ++j)
				{
					double value;
					if (Double.TryParse(dtChart1.Rows[j][i].ToString(), out value))
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

		#region Обработчики диаграммы по районам

		void UltraChart2_DataBinding(object sender, EventArgs e)
		{
			LabelChart2.Text = String.Format(chart2_title_caption, MDXDateToString(selectedDate.Value));
			string query = DataProvider.GetQueryText("ORG_0003_0012_chart_by_district");
			dtChart2 = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Розничная цена", dtChart2);
			UltraChart2.DataSource = (dtChart2 == null) ? null : dtChart2.DefaultView;
		}

		void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			/*for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];

				if (primitive is Box)
				{
					Box box = (Box)primitive;
					if (box.DataPoint != null)
					{
						if (box.Series != null && (box.Series.Label == "Среднее" || box.Series.Label == "Медиана"))
						{
							box.PE.Fill = Color.Yellow;
							box.PE.FillStopColor = Color.Orange;
						}
						else
						{
							if (box.Series != null && (Convert.ToDouble(medianDT.Rows[box.Row][1].ToString()) > 40))
							{
								box.PE.Fill = Color.OrangeRed;
								box.PE.FillStopColor = Color.DarkRed;
							}
							if (box.Series != null && (Convert.ToDouble(medianDT.Rows[box.Row][1].ToString()) <= 40))
							{
								box.PE.Fill = Color.LightGreen;
								box.PE.FillStopColor = Color.DarkGreen;
							}
						}
					}
				}
			}*/
		}
		#endregion

		// --------------------------------------------------------------------

		#region Заполнение словарей и выпадающих списков параметров

		protected void fillComboDate(string queryName)
		{
			// Загрузка списка актуальных дат
			dtDate = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
			// Закачку придется делать через словарь
			string previousDate = String.Empty;
			Dictionary<string, int> dictDate = new Dictionary<string, int>();
			dictDates = new Dictionary<string, string>();
			for (int row = 0; row < dtDate.Rows.Count; ++row)
			{
				string year = dtDate.Rows[row][0].ToString();
				string month = dtDate.Rows[row][3].ToString();
				string day = dtDate.Rows[row][4].ToString();
				string date = day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года";
				if (previousDate == String.Empty)
				{
					previousDate = getPreviousDate(date);
				}
				if (firstDate.Value == String.Empty)
				{
					firstDate.Value = StringToMDXDate(date);
				}
				dictDates.Add(date, previousDate);
				previousDate = date;
				addPairToDictionary(dictDate, year + " год", 0);
				addPairToDictionary(dictDate, month + " " + year + " года", 1);
				addPairToDictionary(dictDate, date, 2);
			}
			ComboDate.FillDictionaryValues(dictDate);
			ComboDate.SelectLastNode();
		}

		protected string getPreviousDate(string date)
		{
			string[] dateElements = date.Split(' ');
			int year = Convert.ToInt32(dateElements[2]);
			int month = CRHelper.MonthNum(dateElements[1]);
			int day = Convert.ToInt32(dateElements[0]);
			DateTime dt = new DateTime(year, month, day);
			dt = dt.AddDays(-1);
			day = dt.Day;
			month = dt.Month;
			year = dt.Year;
			return String.Format("{0} {1} {2} года", day, CRHelper.RusMonthGenitive(month), year);
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

		public string StringToMDXMonth(string str)
		{
			string template = "[Период].[День].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}]";
			string[] dateElements = str.Split(' ');
			int year = Convert.ToInt32(dateElements[1]);
			string month = CRHelper.RusMonth(CRHelper.MonthNum(dateElements[0]));
			int quarter = CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month));
			int halfYear = CRHelper.HalfYearNumByQuarterNum(quarter);
			return String.Format(template, year, halfYear, quarter, month);
		}

		public string StringToMDXRegion(string paramValue)
		{
			string template = "[Организации].[Товары и услуги].[Все товары и услуги].[Медицинские услуги и медикаменты].[Медикаменты].[{0}]";
			return String.Format(template, paramValue);
		}

		public string MDXDateToShortDateString(string MDXDateString)
		{
			string[] separator = { "].[" };
			string[] dateElements = MDXDateString.Split(separator, StringSplitOptions.None);
			string template = "{0}.{1}.{2}";
			string day = dateElements[7].Replace("]", String.Empty);
			day = day.Length == 1 ? "0" + day : day;
			string month = CRHelper.MonthNum(dateElements[6]).ToString();
			month = month.Length == 1 ? "0" + month : month;
			string year = dateElements[3];
			return String.Format(template, day, month, year);
		}

		public string prevMonth(string mdxMonth, int delta)
		{
			string[] dateElements = mdxMonth.Replace("]", String.Empty).Replace("[", String.Empty).Split('.');
			int month = CRHelper.MonthNum(dateElements[6]) - delta;
			int year = Convert.ToInt32(dateElements[3]);
			if (month < 1)
			{
				--year;
				month += 12;
			}
			return String.Format("{0:00}.{1:00}.{2}", CRHelper.MonthLastDay(month), month, year);
		}

		public string getMonthFromString(string date)
		{
			string[] dateElements = date.Split(' ');
			return dateElements[1];
		}

		public string getYearFromString(string date)
		{
			string[] dateElements = date.Split(' ');
			return dateElements[2];
		}

		#endregion

		#region Расчет медианы

		private static bool Even(int input)
		{
			if (input % 2 == 0)
			{
				return true;
			}
			return false;
		}

		private static int MedianIndex(int length)
		{
			if (length == 0)
			{
				return 0;
			}

			if (Even(length))
			{
				return length / 2 - 1;
			}
			else
			{
				return (length + 1) / 2 - 1;
			}
		}

		private static double MedianValue(DataTable dt, int medianValueColumn)
		{
			if (dt.Rows.Count == 0)
			{
				return 0;
			}

			if (Even(dt.Rows.Count))
			{
				double value1;
				double value2;
				Double.TryParse(
						dt.Rows[MedianIndex(dt.Rows.Count)][medianValueColumn].ToString(),
						out value1);
				Double.TryParse(
						dt.Rows[MedianIndex(dt.Rows.Count) + 1][medianValueColumn].ToString(),
						out value2);
				return (value1 + value2) / 2;
			}
			else
			{
				double value;
				Double.TryParse(
						dt.Rows[MedianIndex(dt.Rows.Count)][medianValueColumn].ToString(),
						out value);
				return value;
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
			ISection section3 = report.AddSection();

			UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
			UltraChart2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

			ReportPDFExporter1.HeaderCellHeight = 70;
			GridHeaderLayout workHeader = new GridHeaderLayout();
			workHeader = headerLayout;
			if (UltraWebGrid.DataSource != null)
			{
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
			Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма по времени");
			Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма по территориям");

			SetExportGridParams(headerLayout.Grid);

			ReportExcelExporter1.HeaderCellHeight = 50;
			ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
			ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
			ReportExcelExporter1.TitleStartRow = 0;

			GridHeaderLayout workHeader = new GridHeaderLayout();
			workHeader = headerLayout;
			workHeader.childCells.Remove(workHeader.childCells[workHeader.childCells.Count - 1]);
			ReportExcelExporter1.Export(headerLayout, sheet1, 3);

			ReportExcelExporter1.WorksheetTitle = String.Empty;
			ReportExcelExporter1.WorksheetSubTitle = String.Empty;

			UltraChart1.Width = (int)(UltraChart1.Width.Value * 0.7);
			ReportExcelExporter1.Export(UltraChart1, LabelChart1.Text, sheet2, 1);

			UltraChart2.Width = (int)(UltraChart2.Width.Value * 0.7);
			ReportExcelExporter1.Export(UltraChart2, LabelChart2.Text, sheet3, 1);
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
