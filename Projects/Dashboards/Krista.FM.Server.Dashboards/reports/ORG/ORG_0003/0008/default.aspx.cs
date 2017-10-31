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
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

/**
 *  Анализ тарифов на услуги транспорта
 */
namespace Krista.FM.Server.Dashboards.reports.ORG_0003_0008
{
	public partial class Default : CustomReportPage
	{
		#region Поля

		private DataTable dtDate;
		private DataTable dtGrid;
		private DataTable dtChartByTime;
		private DataTable dtChartByDistrict;
		private DataTable medianDT;
		private GridHeaderLayout headerLayout;

		private static int selectedCellRow = 0;
		private static int selectedCellColumn = 1;

		#endregion

		#region Параметры запроса

		private CustomParam selectedDate;
		private CustomParam firstDate;
		private CustomParam selectedDistrict;
		private CustomParam selectedParameter;

		#endregion
		// --------------------------------------------------------------------

		// заголовок страницы
		private const string PageTitleCaption = "Анализ тарифов на услуги транспорта (в разрезе муниципальных образований)";
		private static string PageSubTitleCaption = "Ежемесячный мониторинг тарифов на транспортные услуги, по состоянию на {0}";

		// --------------------------------------------------------------------

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			ComboDate.Title = "Выберите дату";
			ComboDate.Width = 300;
			ComboDate.ParentSelect = true;
			UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.37 - 50);
			if (IsMozilla)
			{
				UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.75 + 4);
			}
			else
			{
				UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.7 - 4);
			}
			UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
			UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
			UltraWebGrid.ActiveCellChange += new ActiveCellChangeEventHandler(UltraWebGrid_ActiveCellChange);

			#region Настройка диаграммы по времени
			chartByTime.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.62 + 50);
			chartByTime.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 125);
			labelChartByTime.Width = chartByTime.Width;

			chartByTime.ChartType = ChartType.StackAreaChart;
			chartByTime.Data.ZeroAligned = true;
			chartByTime.Border.Thickness = 0;

			chartByTime.Axis.X.Extent = 50;
			chartByTime.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL:N0>";
			chartByTime.Axis.Y.Extent = 25;
			chartByTime.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";

			chartByTime.ColorModel.ModelStyle = ColorModels.PureRandom;

			chartByTime.Tooltips.FormatString = "Стоимость проезда на <ITEM_LABEL:N0>\n<b><DATA_VALUE:N2></b> рублей";
			chartByTime.DataBinding += new EventHandler(chartByTime_DataBinding);
			chartByTime.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			PanelChartByTime.AddLinkedRequestTrigger(UltraWebGrid);

			#endregion

			#region Настройка диаграммы по районам
			chartByDistrict.Width = chartByTime.Width;
			chartByDistrict.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 125);
			labelChartByDistrict.Width = chartByTime.Width;
			chartByDistrict.ChartType = ChartType.ColumnChart;
			chartByDistrict.Border.Thickness = 0;

			chartByDistrict.ColumnChart.SeriesSpacing = 1;
			chartByDistrict.ColumnChart.ColumnSpacing = 1;

			chartByDistrict.Axis.X.Extent = 120;
			chartByDistrict.Axis.X.Labels.Visible = false;
			chartByDistrict.Axis.X.Labels.SeriesLabels.Visible = true;
			chartByDistrict.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
			//chartByDistrict.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
			chartByDistrict.Axis.X.Labels.SeriesLabels.FontSizeBestFit = true;
			//chartByDistrict.Axis.X.Labels.SeriesLabels.WrapText = true;
			chartByDistrict.Axis.Y.Extent = 30;
			chartByDistrict.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";

			chartByDistrict.ColorModel.ModelStyle = ColorModels.PureRandom;

			chartByDistrict.Tooltips.FormatString = "<SERIES_LABEL>\nСтоимость проезда: <b><DATA_VALUE:N2></b> рублей";
			chartByDistrict.DataBinding += new EventHandler(chartByDistrict_DataBinding);
			chartByDistrict.FillSceneGraph += new FillSceneGraphEventHandler(chartByDistrict_FillSceneGraph);
			chartByDistrict.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			PanelChartByDistrict.LinkedRefreshControlID = "PanelChartByTime";
			#endregion

			#region Параметры
			if (selectedDate == null)
			{
				selectedDate = UserParams.CustomParam("selected_date");
			}
			if (firstDate == null)
			{
				firstDate = UserParams.CustomParam("first_date");
			}
			if (selectedDistrict == null)
			{
				selectedDistrict = UserParams.CustomParam("selected_district");
			}
			if (selectedParameter == null)
			{
				selectedParameter = UserParams.CustomParam("selected_parameter");
			}

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
				FillComboDate("ORG_0003_0008_list_of_dates");
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

			#endregion

			PageTitle.Text = PageTitleCaption;
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = String.Format(PageSubTitleCaption, MDXDateToShortDateString(selectedDate.Value, "{0:00}.{1:00}.{2:0000} г."));
			
			headerLayout = new GridHeaderLayout(UltraWebGrid);
			UltraWebGrid.Bands.Clear();
			UltraWebGrid.DataBind();
			UltraWebGrid_MarkByStars(1);
			UltraWebGrid_MarkByStars(2);
			UltraWebGrid.Rows[selectedCellRow].Cells[selectedCellColumn].Activated = true;
			UltraWebGrid.Rows[selectedCellRow].Cells[selectedCellColumn].Selected = true;
			selectedDistrict.Value = StringToMDXDistrict(UltraWebGrid.Rows[selectedCellRow].Cells[0].Value.ToString());
			UltraWebGrid_SelectCell(selectedCellColumn);
		}

		// --------------------------------------------------------------------

		#region Обработчики грида

		protected void UltraWebGrid_SelectCell(int column)
		{
			if (column == 1)
			{
				selectedParameter.Value = "[Организации].[Товары и услуги].[Все товары и услуги].[Транспортные услуги].[Стоимость проезда пассажиров в общественном транспорте по городу (1 поездка)]";
			}
			else
			{
				selectedParameter.Value = "[Организации].[Товары и услуги].[Все товары и услуги].[Транспортные услуги].[Стоимость проезда пассажиров автотранспортом внутрирайонного сообщения (1 км пути)]";
			}
			chartByTime.DataBind();
			chartByDistrict.DataBind();
		}

		protected void UltraWebGrid_ActiveCellChange(object sender, CellEventArgs e)
		{
			selectedDistrict.Value = StringToMDXDistrict(e.Cell.Row.Cells[0].Value.ToString());
			if (e.Cell.Column.Index != 0)
			{
				selectedCellColumn = e.Cell.Column.Index;
				selectedCellRow = e.Cell.Row.Index;
				UltraWebGrid_SelectCell(e.Cell.Column.Index);
			}
			else
			{
				selectedCellColumn = 1;
				selectedCellRow = e.Cell.Row.Index;
				UltraWebGrid_SelectCell(1);
			}
		}


		protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("ORG_0003_0008_grid");
			dtGrid = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGrid);
			if (dtGrid.Rows.Count > 0)
			{
				dtGrid.Columns[1].Caption = "Стоимость проезда пассажиров в общественном транспорте по городу (1 поездка), рублей";
				dtGrid.Columns[2].Caption = "Стоимость проезда пассажиров автотранспортом внутрирайонного сообщения (1 км пути), рублей";
				foreach (DataRow row in dtGrid.Rows)
				{
					row[0] = row[0].ToString().Replace(" муниципальный район", " р-н").Replace("Город ", "г. ");
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
			double firstColumnWidth = 150, columnWidth = 110;
			if (IsMozilla)
			{
				firstColumnWidth = 150;
			}
			e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(firstColumnWidth);
			e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(columnWidth);
			e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(columnWidth);
			CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
			CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
			e.Layout.CellClickActionDefault = CellClickAction.CellSelect;
			e.Layout.RowSelectorsDefault = RowSelectors.No;

			// Заголовки
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
			headerLayout.AddCell("Наименование МО");
			headerLayout.AddCell("Стоимость проезда пассажиров в общественном транспорте по городу (1 поездка), рублей");
			headerLayout.AddCell("Стоимость проезда пассажиров автотранспортом внутрирайонного сообщения (1 км пути), рублей");

			headerLayout.ApplyHeaderInfo();
		}

		protected void UltraWebGrid_MarkByStars(int columnIndex)
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
				rowIndex = Convert.ToInt32(row);
				UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Style.BackgroundImage = "~/images/starGraybb.png";
				UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
				UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Title = "Самый высокий уровень тарифа";
			}
			rows = minValueRows.Split(' ');
			foreach (string row in rows)
			{
				rowIndex = Convert.ToInt32(row);
				UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Style.BackgroundImage = "~/images/starYellowbb.png";
				UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
				UltraWebGrid.Rows[rowIndex].Cells[columnIndex].Title = "Самый низкий уровень тарифа";
			}
		}

		#endregion

		// --------------------------------------------------------------------

		#region Обработчики диаграммы по времени

		void chartByTime_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("ORG_0003_0008_chart_by_time");
			dtChartByTime = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Стоимость проезда", dtChartByTime);
			query = DataProvider.GetQueryText("ORG_0003_0008_chart_by_time_names");
			DataTable dtChartByTimeNames = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Стоимость проезда", dtChartByTimeNames);
			double maxValue = Double.NegativeInfinity;
			for (int i = 1; i < dtChartByTime.Columns.Count; ++i)
			{
				dtChartByTime.Columns[i].ColumnName = MDXDateToShortDateString(dtChartByTimeNames.Rows[0][i].ToString());
				double value;
				if (Double.TryParse(dtChartByTime.Rows[0][i].ToString(), out value))
				{
					if (value > maxValue)
					{
						maxValue = value;
					}
				}
			}
			if (!Double.IsNegativeInfinity(maxValue))
			{
				chartByTime.Axis.Y.RangeType = AxisRangeType.Custom;
				chartByTime.Axis.Y.RangeMax = maxValue * 1.1;
			}
			chartByTime.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			chartByTime.Axis.X.Margin.Near.Value = 20;
			ChartTextAppearance appearance = new ChartTextAppearance();
			appearance.Column = -2;
			appearance.Row = -2;
			appearance.VerticalAlign = StringAlignment.Far;
			appearance.ItemFormatString = "<DATA_VALUE:N2>";
			appearance.ChartTextFont = new Font("Verdana", 8);
			appearance.Visible = true;
			chartByTime.AreaChart.ChartText.Add(appearance);

			labelChartByTime.Text = String.Format("Динамика показателя \"{0}\" ({1}), рублей",
				UserComboBox.getLastBlock(selectedParameter.Value), UserComboBox.getLastBlock(selectedDistrict.Value));

			chartByTime.DataSource = (dtChartByTime == null) ? null : dtChartByTime.DefaultView;
		}

		#endregion

		#region Обработчики диаграммы по районам

		void chartByDistrict_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("ORG_0003_0008_chart_by_district");
			dtChartByDistrict = new DataTable();
			medianDT = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Розничная цена", dtChartByDistrict);
			double minValue = Double.PositiveInfinity; ;
			double maxValue = Double.NegativeInfinity;
			foreach (DataRow row in dtChartByDistrict.Rows)
			{
				if (row[0] != DBNull.Value)
				{
					row[0] = row[0].ToString().Replace(" муниципальный район", " р-н");
					row[0] = row[0].ToString().Replace("Город ", "г. ");
				}
			}
			if (dtChartByDistrict.Rows.Count > 0)
			{
				double avgValue = 0;
				for (int i = 0; i < dtChartByDistrict.Rows.Count; ++i)
				{
					double value = Convert.ToDouble(dtChartByDistrict.Rows[i][1]);
					avgValue += value;
					minValue = value < minValue ? value : minValue;
					maxValue = value > maxValue ? value : maxValue;
				}
				avgValue /= dtChartByDistrict.Rows.Count;
				// рассчитываем медиану
				int medianIndex = MedianIndex(dtChartByDistrict.Rows.Count);
				medianDT = dtChartByDistrict.Clone();
				double medianValue = MedianValue(dtChartByDistrict, 1);
				for (int i = 0; i < dtChartByDistrict.Rows.Count - 1; i++)
				{

					medianDT.ImportRow(dtChartByDistrict.Rows[i]);

					double value;
					Double.TryParse(dtChartByDistrict.Rows[i][1].ToString(), out value);
					double nextValue;
					Double.TryParse(dtChartByDistrict.Rows[i + 1][1].ToString(), out nextValue);
					if (((value <= avgValue) && (nextValue > avgValue)) && (i == medianIndex))
					{
						if (medianValue > avgValue)
						{
							DataRow row = medianDT.NewRow();
							row[0] = "Среднее";
							row[1] = avgValue;
							medianDT.Rows.Add(row);
							row = medianDT.NewRow();
							row[0] = "Медиана";
							row[1] = MedianValue(dtChartByDistrict, 1);
							medianDT.Rows.Add(row);
						}
						else
						{
							DataRow row = medianDT.NewRow();
							row[0] = "Медиана";
							row[1] = MedianValue(dtChartByDistrict, 1);
							medianDT.Rows.Add(row);
							row = medianDT.NewRow();
							row[0] = "Среднее";
							row[1] = avgValue;
							medianDT.Rows.Add(row);
						}
					}
					else
					{
						if ((value <= avgValue) && (nextValue > avgValue))
						{
							DataRow row = medianDT.NewRow();
							row[0] = "Среднее";
							row[1] = avgValue;
							medianDT.Rows.Add(row);
						}

						if (i == medianIndex)
						{
							DataRow row = medianDT.NewRow();
							row[0] = "Медиана";
							row[1] = MedianValue(dtChartByDistrict, 1);
							medianDT.Rows.Add(row);
						}
					}
				}
				medianDT.ImportRow(dtChartByDistrict.Rows[dtChartByDistrict.Rows.Count - 1]);
				if (!Double.IsPositiveInfinity(minValue) && !Double.IsNegativeInfinity(maxValue))
				{
					chartByDistrict.Axis.Y.RangeType = AxisRangeType.Custom;
					chartByDistrict.Axis.Y.RangeMax = maxValue * 1.1;
					chartByDistrict.Axis.Y.RangeMin = minValue / 1.1;
				}
			}
			labelChartByDistrict.Text = String.Format("Распределение территорий по показателю \"{0}\", рублей",
				UserComboBox.getLastBlock(selectedParameter.Value));
			chartByDistrict.DataSource = (medianDT == null) ? null : medianDT.DefaultView;
		}

		void chartByDistrict_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];

				if (primitive is Box)
				{
					Box box = (Box)primitive;
					if (box.DataPoint != null)
					{
						if (box.Series != null && (box.Series.Label == "Среднее" || box.Series.Label == "Медиана"))
						{
							box.PE.Fill = Color.Orange;
							box.PE.FillStopColor = Color.OrangeRed;
						}
					}
				}
			}
		}
		#endregion

		// --------------------------------------------------------------------

		#region Заполнение словарей и выпадающих списков параметров

		protected void FillComboDate(string queryName)
		{
			// Загрузка списка актуальных дат
			dtDate = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
			// Закачку придется делать через словарь
			Dictionary<string, int> dictDate = new Dictionary<string, int>();
			for (int row = 0; row < dtDate.Rows.Count; ++row)
			{
				string year = dtDate.Rows[row][0].ToString();
				string month = dtDate.Rows[row][3].ToString();
				string day = dtDate.Rows[row][4].ToString();
				AddPairToDictionary(dictDate, year + " год", 0);
				AddPairToDictionary(dictDate, month + " " + year + " года", 1);
				if (firstDate.Value == String.Empty)
				{
					firstDate.Value = StringToMDXDate(day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года");
				}
				AddPairToDictionary(dictDate, day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года", 2);
			}
			ComboDate.FillDictionaryValues(dictDate);
			ComboDate.SelectLastNode();
		}

		protected void AddPairToDictionary(Dictionary<string, int> dict, string key, int value)
		{
			if (!dict.ContainsKey(key))
			{
				dict.Add(key, value);
			}
		}

		#endregion

		#region Функции-полезняшки преобразования и все такое

		private static bool IsMozilla
		{
			get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
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

		public string StringToMDXDistrict(string district)
		{

			string template = "[Территории].[РФ].[Все территории].[Российская Федерация].[Северо-Западный федеральный округ].[Вологодская область].[Вологодская область].[{0}]";
			return String.Format(template, district.Replace("г.", "Город").Replace("р-н", "муниципальный район"));
		}

		public string MDXDateToShortDateString(string mdxDateString)
		{
			string[] separator = { "].[" };
			string[] dateElements = mdxDateString.Split(separator, StringSplitOptions.None);
			string template = "{0}.{1}.{2}";
			string day = dateElements[7].Replace("]", String.Empty);
			day = day.Length == 1 ? "0" + day : day;
			string month = CRHelper.MonthNum(dateElements[6]).ToString();
			month = month.Length == 1 ? "0" + month : month;
			string year = dateElements[3].Substring(2, 2);
			return String.Format(template, day, month, year);
		}

		public string MDXDateToShortDateString(string mdxDateString, string template)
		{
			string[] separator = { "].[" };
			string[] dateElements = mdxDateString.Split(separator, StringSplitOptions.None);
			int day = Convert.ToInt32(dateElements[7].Replace("]", String.Empty));
			int month = CRHelper.MonthNum(dateElements[6]);
			int year = Convert.ToInt32(dateElements[3]);
			return String.Format(template, day, month, year);
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

			chartByTime.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
			chartByDistrict.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

			UltraWebGrid.DisplayLayout.RowSelectorsDefault = RowSelectors.Yes;
			UltraWebGrid.DisplayLayout.RowSelectorStyleDefault.Width = 22;
			ReportPDFExporter1.HeaderCellHeight = 70;
			ReportPDFExporter1.Export(headerLayout, section1);
			ReportPDFExporter1.Export(chartByTime, labelChartByTime.Text, section2);
			ReportPDFExporter1.Export(chartByDistrict, labelChartByDistrict.Text, section3);
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
			Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма по районам");

			SetExportGridParams(headerLayout.Grid);

			ReportExcelExporter1.HeaderCellHeight = 50;
			ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
			ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
			ReportExcelExporter1.TitleStartRow = 0;

			ReportExcelExporter1.Export(headerLayout, sheet1, 3);

			ReportExcelExporter1.WorksheetTitle = String.Empty;
			ReportExcelExporter1.WorksheetSubTitle = String.Empty;

			ReportExcelExporter1.Export(chartByTime, labelChartByTime.Text, sheet2, 1);

			ReportExcelExporter1.WorksheetTitle = String.Empty;
			ReportExcelExporter1.WorksheetSubTitle = String.Empty;

			ReportExcelExporter1.Export(chartByDistrict, labelChartByDistrict.Text, sheet3, 1);

			// Ручная обработка
			sheet1.Columns[0].Width = (int)(sheet1.Columns[0].Width * 1.5);
			sheet1.Columns[1].Width = (int)(sheet1.Columns[1].Width * 1.75);
			sheet1.Columns[2].Width = (int)(sheet1.Columns[2].Width * 1.75);

			sheet1.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			sheet1.Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;

			sheet1.MergedCellsRegions.Clear();
			sheet2.MergedCellsRegions.Clear();
			sheet3.MergedCellsRegions.Clear();
		}

		private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
		{
			e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
			e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
			e.CurrentWorksheet.PrintOptions.BottomMargin = 0.25;
			e.CurrentWorksheet.PrintOptions.TopMargin = 0.25;
			e.CurrentWorksheet.PrintOptions.LeftMargin = 0.25;
			e.CurrentWorksheet.PrintOptions.RightMargin = 0.25;
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
