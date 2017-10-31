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
using Infragistics.UltraChart.Core;
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
 *  Анализ розничных цен на медикаменты по состоянию на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.ORG_0003_0011
{
	public partial class Default : CustomReportPage
	{
		#region Поля

		private DataTable dtDate;
		private DataTable dtGrid;
		private DataTable dtGridDates;
		private DataTable dtChart1;
		private DataTable dtChart2;
		private DataTable medianDT;
		private GridHeaderLayout headerLayout;

		#endregion

		#region Параметры запроса

		// выбранная дата
		private CustomParam selectedMonth;
		private CustomParam selectedDate;
		private CustomParam firstDate;
		private CustomParam selectedParameter;
		private static string ParameterText = String.Empty;

		#endregion
		// --------------------------------------------------------------------

		// заголовок страницы
		private const string page_title_caption = "Анализ розничных цен на медикаменты по Вологодской области по состоянию на {0}";
		private const string page_sub_title_caption = "Ежемесячный мониторинг средних розничных цен на медикаменты по Вологодской области";
		private const string chart1_title_caption = "Динамика розничной цены на товар «{0}», рублей";
		private const string chart2_title_caption = "Уровень средней розничной надбавки на ЖНВЛС в разрезе муниципальных образований (на {0}), %";

		// --------------------------------------------------------------------

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			ComboDate.Title = "Выберите дату";
			ComboDate.Width = 300;
			ComboDate.ParentSelect = true;
			UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30);
			UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.4);
			UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
			UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
			UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
			UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);

			#region Настройка диаграммы по времени
			UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
			UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 125);
			LabelChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);

			UltraChart1.ChartType = ChartType.StackAreaChart;
			UltraChart1.Border.Thickness = 0;

			UltraChart1.Axis.X.Extent = 50;
			UltraChart1.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL:N0>";
			UltraChart1.Axis.Y.Extent = 20;
			UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:##.##>";

			UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;

			UltraChart1.Tooltips.FormatString = "Розничная цена на <ITEM_LABEL:N0>\n<b><DATA_VALUE:N2></b> рублей";
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

			#region Настройка диаграммы по районам
			UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
			UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 75);
			LabelChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
			UltraChart2.ChartType = ChartType.ColumnChart;
			UltraChart2.Border.Thickness = 0;

			UltraChart2.ColumnChart.SeriesSpacing = 1;
			UltraChart2.ColumnChart.ColumnSpacing = 1;

			UltraChart2.Axis.X.Extent = 150;
			UltraChart2.Axis.X.Labels.Visible = false;
			UltraChart2.Axis.X.Labels.SeriesLabels.Visible = true;
			UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
			UltraChart2.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
			UltraChart2.Axis.Y.Extent = 20;
			UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:##.##>";

			UltraChart2.ColorModel.ModelStyle = ColorModels.PureRandom;

			UltraChart2.Tooltips.FormatString = "<SERIES_LABEL>\nТорговая надбавка: <b><DATA_VALUE:N2></b>%";
			UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);
			UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
			UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			#endregion

			#region Параметры
			selectedMonth = UserParams.CustomParam("selected_month");
			selectedDate = UserParams.CustomParam("selected_date");
			firstDate = UserParams.CustomParam("first_date");
			selectedParameter = UserParams.CustomParam("selected_parameter");

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
				fillComboDate("ORG_0003_0011_list_of_dates");
			}
			#region Анализ параметров
			Node node = new Node();
			if (ComboDate.SelectedNode.Level == 0)
			{
				node = ComboDate.GetLastChild(ComboDate.SelectedNode);
			}
			if (ComboDate.SelectedNode.Level == 1)
			{
				node = ComboDate.SelectedNode;
			}
			selectedMonth.Value = StringToMDXMonth(node.Text);
			PageSubTitle.Text = page_sub_title_caption;
			#endregion
			headerLayout = new GridHeaderLayout(UltraWebGrid);
			UltraWebGrid.Bands.Clear();
			UltraWebGrid.DataBind();
			if (UltraWebGrid.DataSource != null)
			{
				if (selectedParameter.Value == String.Empty)
				{
					UltraWebGrid_ActivateRow(CRHelper.FindGridRow(UltraWebGrid, UltraWebGrid.Rows[0].Cells[0].Text, 0, 0));
				}
				else
				{
					UltraWebGrid_ActivateRow(CRHelper.FindGridRow(UltraWebGrid, ParameterText, 0, 0));
				}
			}
			SetDynamicText(MDXDateToString(selectedDate.Value));
		}

		// --------------------------------------------------------------------

		#region Обработчики грида

		protected void UltraWebGrid_ActivateRow(UltraGridRow row)
		{
			selectedParameter.Value = StringToMDXParameter(row.Cells[0].Text);
			ParameterText = row.Cells[0].Text;
			UltraChart1.DataBind();
			UltraChart2.DataBind();
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
			string query = DataProvider.GetQueryText("ORG_0003_0011_grid");
			dtGrid = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование товара", dtGrid);
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
			}
			for (int i = 0; i < 3; ++i)
			{
				string prevDate = dtGridDates.Rows[0][i + 1].ToString() == String.Empty ?
					prevMonth(selectedMonth.Value, 3 - i) : MDXDateToShortDateString(dtGridDates.Rows[0][i + 1].ToString());
				e.Row.Cells[2 + i * 3].Title = "Абсолютное отклонение по отношению к " + prevDate;
				e.Row.Cells[3 + i * 3].Title = "Темп прироста к " + prevDate;
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
			string query = DataProvider.GetQueryText("ORG_0003_0011_grid_dates");
			dtGridDates = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Даты", dtGridDates);

			e.Layout.RowSelectorStyleDefault.Width = 20;
			e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
			double gridWidth = UltraWebGrid.Width.Value - e.Layout.RowSelectorStyleDefault.Width.Value - 50;
			double columnWidth = CRHelper.GetColumnWidth(gridWidth * 0.6 / 9 * k);
			e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(gridWidth * 0.4 * k - 30);
			for (int i = 1; i < e.Layout.Bands[0].Columns.Count; ++i)
			{
				e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
				e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Left = 5;
				CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
			}
			CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P2");
			CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P2");
			CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[9], "P2");

			// Заголовки
			e.Layout.StationaryMargins = StationaryMargins.Header;
			e.Layout.TableLayout = TableLayout.Fixed;
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
			headerLayout.AddCell("Наименование товара");
			GridHeaderCell header = null;
			for (int i = 2; i < dtGridDates.Columns.Count; ++i)
			{
				string headerCellTitle = String.Empty;
				if (dtGridDates.Rows[0][i].ToString() == String.Empty)
				{
					headerCellTitle = prevMonth(selectedMonth.Value, 4 - i);
				}
				else
				{
					headerCellTitle = MDXDateToShortDateString(dtGridDates.Rows[0][i].ToString());
				}
				header = headerLayout.AddCell(headerCellTitle);
				header.AddCell("Розничная цена, рублей");
				header.AddCell("Абсолютное отклонение, рублей");
				header.AddCell("Темп прироста, %");
				selectedDate.Value = dtGridDates.Rows[0][i].ToString();
			}
			PageTitle.Text = String.Format(page_title_caption, MDXDateToString(selectedDate.Value));
			Page.Title = PageTitle.Text;
			//headerLayout.AddCell("MDX имя");
			//headerLayout.AddCell("Единица измерения");
			headerLayout.ApplyHeaderInfo();
		}

		#endregion

		// --------------------------------------------------------------------

		#region Обработчики диаграммы 1

		void UltraChart1_DataBinding(object sender, EventArgs e)
		{
			LabelChart1.Text = String.Format(chart1_title_caption, ParameterText);
			string query = DataProvider.GetQueryText("ORG_0003_0011_chart_by_time");
			dtChart1 = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Розничная цена", dtChart1);
			query = DataProvider.GetQueryText("ORG_0003_0011_chart_by_time_names");
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

		#region Обработчики диаграммы по районам

		void UltraChart2_DataBinding(object sender, EventArgs e)
		{
			LabelChart2.Text = String.Format(chart2_title_caption, MDXDateToString(selectedMonth.Value + ".[15]"));
			string query = DataProvider.GetQueryText("ORG_0003_0011_chart_by_district");
			dtChart2 = new DataTable();
			medianDT = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Розничная цена", dtChart2);
			double minValue = Double.PositiveInfinity; ;
			double maxValue = Double.NegativeInfinity;
			foreach (DataRow row in dtChart2.Rows)
			{
				if (row[0] != DBNull.Value)
				{
					row[0] = row[0].ToString().Replace(" муниципальный район", " р-н");
					row[0] = row[0].ToString().Replace("Город ", "г. ");
				}
			}
			if (dtChart2.Rows.Count > 0)
			{
				double avgValue = 0;
				for (int i = 0; i < dtChart2.Rows.Count; ++i)
				{
					double value = Convert.ToDouble(dtChart2.Rows[i][1]);
					avgValue += value;
					minValue = value < minValue ? value : minValue;
					maxValue = value > maxValue ? value : maxValue;
				}
				avgValue /= dtChart2.Rows.Count;
				// рассчитываем медиану
				int medianIndex = MedianIndex(dtChart2.Rows.Count);
				medianDT = dtChart2.Clone();
				double medianValue = MedianValue(dtChart2, 1);
				for (int i = 0; i < dtChart2.Rows.Count - 1; i++)
				{

					medianDT.ImportRow(dtChart2.Rows[i]);

					double value;
					Double.TryParse(dtChart2.Rows[i][1].ToString(), out value);
					double nextValue;
					Double.TryParse(dtChart2.Rows[i + 1][1].ToString(), out nextValue);
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
							row[1] = MedianValue(dtChart2, 1);
							medianDT.Rows.Add(row);
						}
						else
						{
							DataRow row = medianDT.NewRow();
							row[0] = "Медиана";
							row[1] = MedianValue(dtChart2, 1);
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
							row[1] = MedianValue(dtChart2, 1);
							medianDT.Rows.Add(row);
						}
					}
				}
				medianDT.ImportRow(dtChart2.Rows[dtChart2.Rows.Count - 1]);
				if (!Double.IsPositiveInfinity(minValue) && !Double.IsNegativeInfinity(maxValue))
				{
					UltraChart2.Axis.Y.RangeType = AxisRangeType.Custom;
					UltraChart2.Axis.Y.RangeMax = maxValue * 1.1;
					UltraChart2.Axis.Y.RangeMin = minValue / 1.1;
				}
			}
			UltraChart2.DataSource = (medianDT == null) ? null : medianDT.DefaultView;
		}

		void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			//Хитрая процедура рисования
			IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
			IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

			if (xAxis == null || yAxis == null)
				return;

			int textWidht = 300;
			int textHeight = 12;
			int lineStart = (int)xAxis.MapMinimum;
			int lineLength = (int)xAxis.MapMaximum;

			Line line = new Line();
			line.lineStyle.DrawStyle = LineDrawStyle.Dash;
			line.PE.Stroke = Color.DarkGray;
			line.PE.StrokeWidth = 1;
			line.p1 = new Point(lineStart, (int)yAxis.Map(40));
			line.p2 = new Point(lineStart + lineLength, (int)yAxis.Map(40));
			e.SceneGraph.Add(line);

			Text text = new Text();
			text.PE.Fill = Color.Black;
			text.bounds = new Rectangle(30, ((int)yAxis.Map(40)) - textHeight, textWidht, textHeight);
			text.SetTextString(string.Format("Предельный уровень розничной надбавки на ЖНВЛС: 40%"));
			e.SceneGraph.Add(text);

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
			}


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
			Dictionary<string, int> dictDate = new Dictionary<string, int>();
			for (int row = 0; row < dtDate.Rows.Count; ++row)
			{
				string year = dtDate.Rows[row][0].ToString();
				string month = dtDate.Rows[row][3].ToString();
				string day = dtDate.Rows[row][4].ToString();
				addPairToDictionary(dictDate, year + " год", 0);
				addPairToDictionary(dictDate, month + " " + year + " года", 1);
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

		public string StringToMDXParameter(string paramValue)
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
			ReportPDFExporter1.Export(headerLayout, section1);
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
			Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма по районам");
			Worksheet sheet4 = workbook.Worksheets.Add("Так надо, потом удалю");

			SetExportGridParams(headerLayout.Grid);

			ReportExcelExporter1.HeaderCellHeight = 50;
			ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
			ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
			ReportExcelExporter1.TitleStartRow = 0;

			sheet3.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Portrait;
			ReportExcelExporter1.Export(headerLayout, sheet1, 3);
			for (int i = 0; i < UltraWebGrid.Rows.Count; ++i)
				for (int j = 0; j < UltraWebGrid.Columns.Count; ++j)
					SetCellParams(sheet1.Rows[5 + i].Cells[j]);

			ReportExcelExporter1.WorksheetTitle = String.Empty;
			ReportExcelExporter1.WorksheetSubTitle = String.Empty;

			UltraChart1.Width = (int)(UltraChart1.Width.Value * 0.7);
			sheet2.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
			ReportExcelExporter1.Export(UltraChart1, LabelChart1.Text, sheet2, 1);

			UltraChart2.Width = (int)(UltraChart2.Width.Value * 0.7);
			sheet3.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
			ReportExcelExporter1.Export(UltraChart2, LabelChart2.Text, sheet3, 1);
			sheet3.MergedCellsRegions.Clear();

			GridHeaderLayout emptyGridLayout = new GridHeaderLayout(emptyExportGrid);
			ReportExcelExporter1.Export(emptyGridLayout, sheet4, 0);
			workbook.Worksheets.Remove(sheet4);
		}

		private void SetCellParams(WorksheetCell cell)
		{
			cell.CellFormat.Font.Name = "Verdana";
			cell.CellFormat.Font.Height = 200;

			cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.TopBorderStyle = CellBorderLineStyle.Thin;
		}
		
		private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
		{
			//e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Portrait;
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

		#region Формирование динамического текста

		private string dynamicSeparator = ", ";

		private void SetDynamicText(string string_date)
		{
			Dictionary<string, double> dictDown = new Dictionary<string, double>();
			Dictionary<string, double> dictUp = new Dictionary<string, double>();
			bool hasData = false;
			string head = String.Empty, percentsMore2 = String.Empty, costDown = String.Empty;
			for (int j = 0; j < UltraWebGrid.Rows.Count; ++j)
			{
				if (UltraWebGrid.Rows[j].Cells[9].Value != null)
				{
					double num_value;
					if (Double.TryParse(UltraWebGrid.Rows[j].Cells[9].Text, out num_value))
					{
						string name = "«" + UltraWebGrid.Rows[j].Cells[0].GetText() + "»";
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
					head = String.Format("&nbsp;&nbsp;&nbsp;По состоянию на <b>{0}</b> розничные цены на медикаменты изменились незначительно.",
						string_date);
				}
				else
				{
					head = String.Format("&nbsp;&nbsp;&nbsp;По состоянию на <b>{0}</b> данные об изменении розничных цен на медикаменты отсутствуют.",
						string_date);
				}
			}
			else
			{
				head = String.Format("&nbsp;&nbsp;&nbsp;По состоянию на <b>{0}</b> наблюдалось изменение розничных цен на медикаменты:",
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
				LabelDynamicText.Text += "<br/>&nbsp;&nbsp;&nbsp;- увеличение цен более чем на 2%: " + percentsMore2 + ".";
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
				LabelDynamicText.Text += "<br/>&nbsp;&nbsp;&nbsp;- снижение цен более чем на 2%: " + percentsMore2 + ".";
			}
			LabelDynamicText.Width = UltraWebGrid.Width;
			LabelDynamicText.Height = Unit.Empty;
		}

		private int SortDesc(double x, double y)
		{
			if (x == y)
				return 0;
			else if (x < y)
				return -1;
			else
				return 1;
		}

		#endregion
	}
}
