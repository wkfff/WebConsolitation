using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
 *  Анализ розничных цен на социально значимые продовольственные товары по состоянию на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.ORG_0003_0009
{
	public partial class Default : CustomReportPage
	{

		#region Поля

		private DataTable dtGrid;
		private DataTable dtChartByTime;
		private DataTable dtChart1;
		private DataTable dtChart2;
		private GridHeaderLayout headerLayout;

		#endregion

		#region Параметры запроса

		// выбранный район/город
		private CustomParam selectedRegion;
		// все регионы с данными, надо для диаграммы
		private CustomParam allRegions;
		// выбранный товар
		private CustomParam selectedFood;
		// первая актуальная дата
		private CustomParam firstDate;
		// выбранная дата
		private CustomParam selectedDate;
		// предыдущий месяц дата
		private CustomParam previousDate;
		// начало года
		private CustomParam yearDate;
		// те же, но в текстовом формате (для вывода на экран, чтобы не конвертировать)
		private static string selectedRegionText;
		private static string selectedDateText;
		private static string previousDateText;
		private static string yearDateText;
		private static string selectedFoodText;
		private static string selectedFoodUnitText;
		private string[] Towns;
		private string[] Towns2;

		#endregion

		// --------------------------------------------------------------------

		// заголовок страницы
		private const string PageTitleCaption = "Анализ розничных цен на основные продукты питания";
		private const string PageSubTitleCaption = "Еженедельный мониторинг средних розничных цен на основные продукты питания, {0}, по состоянию на {1}";
		// заголовок для UltraChart
		private const string ChartByTimeTitleCaption = "Динамика розничной цены на товар «{0}», рублей за {1}";
		private const string Chart1TtleCaption = "Уровень розничной цены на товар «{0}» в крупных городах Вологодской области и областных центрах соседних регионов по отношению к предыдущему отчетному периоду, рублей за {1}";
		private const string Chart2TitleCaption = "Уровень розничной цены на товар «{0}» в крупных городах Вологодской области и областных центрах соседних регионов по отношению к началу года, рублей за {1}";
		// Перекодировка регионов
		private static Dictionary<string, string> dictRegions;

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
			UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.5);
			UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
			UltraWebGrid.DisplayLayout.SelectTypeRowDefault = SelectType.Single;
			UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);

			#region Настройка диаграммы по времени
			ChartByTime.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 12);
			ChartByTime.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 125);
			LabelChartByTime.Width = ChartByTime.Width;

			ChartByTime.ChartType = ChartType.AreaChart;
			ChartByTime.Border.Thickness = 0;

			ChartByTime.Axis.X.Extent = 50;
			ChartByTime.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL:N0>";
			ChartByTime.Axis.Y.Extent = 20;
			ChartByTime.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:##.##>";

			ChartByTime.ColorModel.ModelStyle = ColorModels.PureRandom;
			
			LineAppearance lineAppearance;

			lineAppearance = new LineAppearance();
			lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
			lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
			ChartByTime.AreaChart.LineAppearances.Add(lineAppearance);


			ChartByTime.Tooltips.FormatString = "Розничная цена на <ITEM_LABEL:N0>\n<b><DATA_VALUE:N2></b> рублей";
			ChartByTime.DataBinding += new EventHandler(ChartByTime_DataBinding);
			ChartByTime.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			PanelCharts.AddLinkedRequestTrigger(UltraWebGrid);

			ChartByTime.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			ChartByTime.Axis.X.Margin.Near.Value = 20;
			ChartByTime.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
			ChartByTime.Axis.X.Margin.Far.Value = 20;
			ChartTextAppearance appearance = new ChartTextAppearance();
			appearance.Column = -2;
			appearance.Row = -2;
			appearance.VerticalAlign = StringAlignment.Far;
			appearance.ItemFormatString = "<DATA_VALUE:N2>";
			appearance.ChartTextFont = new Font("Verdana", 8);
			appearance.Visible = true;
			//ChartByTime.AreaChart.ChartText.Add(appearance);

			#endregion

			#region Настройка диаграммы 1
			UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.49 - 6);
			UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);
			LabelChart1.Width = UltraChart1.Width;

			UltraChart1.ChartType = ChartType.StackColumnChart;
			UltraChart1.Axis.X.Extent = 150;
			UltraChart1.Axis.Y.Extent = 50;
			UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

			UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
			UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

			//UltraChart1.Legend.Margins.Right = Convert.ToInt32(UltraChart1.Width.Value) / 2;
			UltraChart1.Legend.Visible = true;
			UltraChart1.Legend.Font = new Font("Verdana", 8);
			UltraChart1.Legend.Location = LegendLocation.Bottom;
			UltraChart1.Legend.SpanPercentage = 15;
			UltraChart1.Border.Thickness = 0;

			UltraChart1.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart1.Axis.X.Margin.Near.Value = 20;

			UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>";
			UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
			UltraChart1.DataBinding +=new EventHandler(UltraChart1_DataBinding);

			#endregion

			#region Настройка диаграммы 2
			UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.49 - 6);
			UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);
			LabelChart2.Width = UltraChart2.Width;

			UltraChart2.ChartType = ChartType.StackColumnChart;
			UltraChart2.Axis.X.Extent = 150;
			UltraChart2.Axis.Y.Extent = 50;
			UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

			UltraChart2.Axis.X.Labels.SeriesLabels.Visible = true;
			UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

			//UltraChart1.Legend.Margins.Right = Convert.ToInt32(UltraChart1.Width.Value) / 2;
			UltraChart2.Legend.Visible = true;
			UltraChart2.Legend.Font = new Font("Verdana", 8);
			UltraChart2.Legend.Location = LegendLocation.Bottom;
			UltraChart2.Legend.SpanPercentage = 15;
			//UltraChart2.Legend.FormatString = "<ITEM_LABEL>";
			UltraChart2.Border.Thickness = 0;

			UltraChart2.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart2.Axis.X.Margin.Near.Value = 20;

			UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>";
			UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
			UltraChart2.DataBinding +=new EventHandler(UltraChart2_DataBinding);
			#endregion
			
			#region Параметры
			if (selectedDate == null)
			{
				selectedDate = UserParams.CustomParam("selected_date");
			}
			if (previousDate == null)
			{
				previousDate = UserParams.CustomParam("previous_date");
			}
			if (yearDate == null)
			{
				yearDate = UserParams.CustomParam("year_date");
			}
			if (selectedRegion == null)
			{
				selectedRegion = UserParams.CustomParam("selected_region");
			}
			if (allRegions == null)
			{
				allRegions = UserParams.CustomParam("all_regions");
			}
			if (selectedFood == null)
			{
				selectedFood = UserParams.CustomParam("selected_food");
			}
			if (firstDate == null)
			{
				firstDate = UserParams.CustomParam("first_date");
			}
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
				FillComboDate("ORG_0003_0009_list_of_dates");
				FillComboRegion("ORG_0003_0009_list_of_regions");
			}

			#region Анализ параметров
			GetDates(ComboDate, selectedDate, previousDate, yearDate);
			selectedDateText = MDXDateToShortDateString(selectedDate.Value);
			previousDateText = MDXDateToShortDateString(previousDate.Value);
			yearDateText = MDXDateToShortDateString(yearDate.Value);
			selectedRegionText = ComboRegion.SelectedValue;
			selectedRegion.Value = StringToMDXRegion(ComboRegion.SelectedValue);
			#endregion

			PageTitle.Text = PageTitleCaption;
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = String.Format(PageSubTitleCaption, ComboRegion.SelectedValue, MDXDateToShortDateString(selectedDate.Value));

			headerLayout = new GridHeaderLayout(UltraWebGrid);
			UltraWebGrid.Bands.Clear();
			UltraWebGrid.DataBind();
			if (selectedFood.Value == String.Empty)
			{
				UltraWebGrid_SetSelectedFoodparams(0);
				UltraWebGrid.Rows[0].Activated = true;
				UltraWebGrid.Rows[0].Activate();
				UltraWebGrid.Rows[0].Selected = true;
			}
			UltraWebGrid_SetSelectedFoodparams(CRHelper.FindGridRow(UltraWebGrid, selectedFood.Value, 2, 0).Index);

			ChartByTime.DataBind();
			UltraChart1.DataBind();
			UltraChart2.DataBind();
		}

		protected void GetDates(CustomMultiCombo combo, CustomParam selectedDate, CustomParam previousDate, CustomParam yearDate)
		{
			Node node = new Node();
			if (combo.SelectedNode.Level == 0)
			{
				node = combo.GetLastChild(combo.SelectedNode).FirstNode;
			}
			if (combo.SelectedNode.Level == 1)
			{
				node = combo.SelectedNode.FirstNode;
			}
			if (combo.SelectedNode.Level == 2)
			{
				node = combo.SelectedNode;
			}
			selectedDate.Value = StringToMDXDate(node.Text);
			Node prevNode = null;
			if (node.PrevNode != null)
			{
				prevNode = node.PrevNode;
			}
			else
			{
				if (node.Parent.PrevNode != null)
				{
					prevNode = combo.GetLastChild(node.Parent.PrevNode);
				}
				else
				{
					if (node.Parent.Parent.PrevNode != null)
					{
						prevNode = combo.GetLastChild(combo.GetLastChild(node));
					}
					else
					{
						prevNode = null;
					}
				}
			}
			previousDate.Value = prevNode != null ? StringToMDXDate(prevNode.Text) : StringToMDXDate(getPreviousDate(node.Text));
			Node yearNode = node.Parent.Parent.FirstNode.FirstNode;
			if (CRHelper.MonthNum(getMonthFromString(yearNode.Text)) == 1)
			{
				yearDate.Value = StringToMDXDate(yearNode.Text);
			}
			else
			{
				yearDate.Value = StringToMDXDate(getYearDate(node.Text));
			}
		}

		// --------------------------------------------------------------------

		#region Обработчики грида

		protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("ORG_0003_0009_grid");
			dtGrid = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование продукта", dtGrid);
			if (dtGrid.Rows.Count > 0)
			{
				dtGrid.Columns.Add("Абсолютное отклонение по отношению к предыдущему периоду", typeof(Double));
				dtGrid.Columns.Add("Темп прироста по отношению к предыдущему периоду", typeof(Double));
				dtGrid.Columns.Add("Абсолютное отклонение по отношению к началу года", typeof(Double));
				dtGrid.Columns.Add("Темп прироста по отношению к началу года", typeof(Double));
				UltraWebGrid.DataSource = dtGrid;
			}
			else
			{
				UltraWebGrid.DataSource = null;
			}
		}

		protected void UltraWebGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
		{
			e.Layout.AllowSortingDefault = AllowSorting.No;
			e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(350);
			int columnWidth = Convert.ToInt32((Convert.ToInt32(e.Layout.Grid.Width.Value) - CRHelper.GetColumnWidth(300)) / 8 - 10);
			for (int i = 1; i < e.Layout.Bands[0].Columns.Count; ++i)
			{
				e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
				e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Left = 5;
				CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
			}
			CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "P2");
			CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[9], "P2");
			// Заголовки
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
			headerLayout.AddCell("Наименование продукта");
			headerLayout.AddCell("Единица измерения");
			headerLayout.AddCell("MDX имя");
			headerLayout.AddCell(String.Format("Розничная цена на {0}, рубль", yearDateText));
			headerLayout.AddCell(String.Format("Розничная цена на {0}, рубль", previousDateText));
			headerLayout.AddCell(String.Format("Розничная цена на {0}, рубль", selectedDateText));

			GridHeaderCell header = headerLayout.AddCell("Динамика к предыдущему отчетному периоду");
			header.AddCell("Абсолютное отклонение, рубль");
			header.AddCell("Темп прироста, %");

			header = headerLayout.AddCell("Динамика за период с начала года");
			header.AddCell("Абсолютное отклонение, рубль");
			header.AddCell("Темп прироста, %");

			headerLayout.ApplyHeaderInfo();
			e.Layout.Bands[0].Columns[1].Hidden = true;
			e.Layout.Bands[0].Columns[2].Hidden = true;
		}

		protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
		{
			double value;
			if (!Double.TryParse(e.Row.Cells[5].ToString(), out value))
			{
				e.Row.Delete();
			}
			e.Row.Cells[0].Text = e.Row.Cells[0].Text + ", " + e.Row.Cells[1].Text.ToLower();
			double prevValue, yearValue;
			if (Double.TryParse(e.Row.Cells[4].ToString(), out prevValue))
			{
				e.Row.Cells[6].Value = value - prevValue;
				e.Row.Cells[7].Value = String.Format("{0:P2}", (value - prevValue) / prevValue);
				if (value != prevValue)
				{
					if (value < prevValue)
					{
						e.Row.Cells[7].Style.CssClass = "ArrowDownGreen";
					}
					else
					{
						e.Row.Cells[7].Style.CssClass = "ArrowUpRed";
					}
				}
			}
			else
			{
				e.Row.Cells[4].Value = "-";
				e.Row.Cells[6].Value = "-";
				e.Row.Cells[7].Value = "-";
			}
			if (Double.TryParse(e.Row.Cells[3].ToString(), out yearValue))
			{
				e.Row.Cells[8].Value = value - yearValue;
				e.Row.Cells[9].Value = String.Format("{0:P2}", (value - yearValue) / yearValue);
				if (value != yearValue)
				{
					if (value < yearValue)
					{
						e.Row.Cells[9].Style.CssClass = "ArrowDownGreen";
					}
					else
					{
						e.Row.Cells[9].Style.CssClass = "ArrowUpRed";
					}
				}
			}
			else
			{
				e.Row.Cells[3].Value = "-";
				e.Row.Cells[8].Value = "-";
				e.Row.Cells[9].Value = "-";
			}
			// Хинты
			e.Row.Cells[6].Title = String.Format("Абсолютное отклонение по отношению к {0}", previousDateText);
			e.Row.Cells[7].Title = String.Format("Темп прироста к {0}", previousDateText);
			e.Row.Cells[8].Title = String.Format("Абсолютное отклонение по отношению к {0}", yearDateText);
			e.Row.Cells[9].Title = String.Format("Темп прироста к {0}", yearDateText);
		}

		void UltraWebGrid_SetSelectedFoodparams(int gridRow)
		{
			selectedFood.Value = UltraWebGrid.Rows[gridRow].Cells[2].Value.ToString();
			selectedFoodUnitText = UltraWebGrid.Rows[gridRow].Cells[1].Value.ToString().ToLower();
			selectedFoodText = UltraWebGrid.Rows[gridRow].Cells[0].Value.ToString().Replace(", " + selectedFoodUnitText.ToLower(), String.Empty);
		}

		void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
		{
			//selectedFood.Value = e.Row.Cells[2].Value.ToString();
			if (PanelCharts.IsAsyncPostBack)
			{
				UltraWebGrid_SetSelectedFoodparams(e.Row.Index);
				ChartByTime.DataBind();
				UltraChart1.DataBind();
				UltraChart2.DataBind();
			}
		}

		#endregion

		// --------------------------------------------------------------------

		#region Обработчики диаграммы 1

		void ChartByTime_DataBinding(object sender, EventArgs e)
		{
			LabelChartByTime.Text = String.Format(ChartByTimeTitleCaption, selectedFoodText, selectedFoodUnitText);
			string query = DataProvider.GetQueryText("ORG_0003_0009_chart_by_time");
			dtChartByTime = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Розничная цена", dtChartByTime);
			query = DataProvider.GetQueryText("ORG_0003_0009_chart_by_time_names");
			DataTable dtChartByTimeNames = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Розничная цена", dtChartByTimeNames);
			double maxValue = Double.NegativeInfinity, minValue = Double.PositiveInfinity;
			for (int i = 1; i < dtChartByTime.Columns.Count; ++i)
			{
				dtChartByTime.Columns[i].ColumnName = MDXDateToShortDateString1(dtChartByTimeNames.Rows[0][i].ToString());
				double value;
				if (Double.TryParse(dtChartByTime.Rows[0][i].ToString(), out value))
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
				ChartByTime.Axis.Y.RangeType = AxisRangeType.Custom;
				ChartByTime.Axis.Y.RangeMin = minValue * 0.9;
				ChartByTime.Axis.Y.RangeMax = maxValue * 1.1;
			}

			//labelChartByTime.Text = String.Format("Динамика показателя \"{0}\" ({1}), рублей",
			//	UserComboBox.getLastBlock(selectedParameter.Value), UserComboBox.getLastBlock(selectedDistrict.Value));

			ChartByTime.DataSource = (dtChartByTime == null) ? null : dtChartByTime.DefaultView;
		}
		#endregion

		#region Обработчики диаграммы 2
		protected void UltraChart1_DataBinding(object sender, EventArgs e)
		{
			LabelChart1.Text = String.Format(Chart1TtleCaption, selectedFoodText, selectedFoodUnitText);
			string query = DataProvider.GetQueryText("ORG_0003_0009_chart_by_previous_month");
			dtChart1 = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);
			UltraChart1.Series.Clear();
			UltraChart1.Data.SwapRowsAndColumns = true;
			Towns = null;
			Array.Resize(ref Towns, dtChart1.Rows.Count);
			for (int i = 0; i < dtChart1.Columns.Count; i++)
			{
				if (i > 0)
				{
					NumericSeries series = CRHelper.GetNumericSeries(i, dtChart1);
					series.Label = dtChart1.Columns[i].ColumnName;
					UltraChart1.Series.Add(series);
				}
				for (int j = 0; j < dtChart1.Rows.Count; ++j)
				{
					if (Towns[j] != null)
					{
						Towns[j] += ";";
					}
					if (dtChart1.Rows[j][i] != DBNull.Value)
					{
						Towns[j] += dtChart1.Rows[j][i].ToString();
					}
				}
			}
		}

		protected string UltraChart_MakeLabelForDataPoint(string Town, string date)
		{
			string[] TownInfo = Town.Split(';');
			string result = TownInfo[0];
			if (TownInfo[1] != String.Empty)
			{
				if (TownInfo[2] != String.Empty)
				{
					result += String.Format("\nРозничная цена: <b>{0:N2}</b> рублей", Convert.ToDouble(TownInfo[1]) + Convert.ToDouble(TownInfo[2]));
					result += String.Format("\nПрирост по отношению к {1}: <b>{0:N2}</b> рублей", Convert.ToDouble(TownInfo[2]), date);
				}
				else
				{
					if (TownInfo[3] != String.Empty)
					{
						result += String.Format("\nРозничная цена: <b>{0:N2}</b> рублей", Convert.ToDouble(TownInfo[1]));
						result += String.Format("\nСнижение по отношению к {1}: <b>{0:N2}</b> рублей", Convert.ToDouble(TownInfo[3]), date);
					}
					else
					{
						result += String.Format("\nРозничная цена: <b>{0:N2}</b> рублей", Convert.ToDouble(TownInfo[1]));
						result += String.Format("\nЦена по отношению к {0} не изменилась", date);
					}
				}
			}
			return result;
		}
		
		protected void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			string label = String.Empty;
			int j = 0;
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];
				if (primitive is Box)
				{
					Box box = (Box)primitive;
					if (box.DataPoint != null)
					{
						switch (box.DataPoint.Label)
						{
							case "Цена (без учета прироста)":
								{
									label = UltraChart_MakeLabelForDataPoint(Towns[j], previousDateText);
									box.DataPoint.Label = label;
									++j;
									break;
								}
							case "Прирост цены, входящий в состав текущей цены":
								{
									box.DataPoint.Label = label;
									break;
								}
							case "Снижение цены ":
								{
									box.DataPoint.Label = label;
									break;
								}
						}
					}
				}
			}
		}
		#endregion

		#region Обработчики диаграммы 3
		protected void UltraChart2_DataBinding(object sender, EventArgs e)
		{
			LabelChart2.Text = String.Format(Chart2TitleCaption, selectedFoodText, selectedFoodUnitText);
			string query = DataProvider.GetQueryText("ORG_0003_0009_chart_by_start_year");
			dtChart2 = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart2);
			UltraChart2.Series.Clear();
			UltraChart2.Data.SwapRowsAndColumns = true;
			Towns2 = null;
			Array.Resize(ref Towns2, dtChart2.Rows.Count);
			for (int i = 0; i < dtChart2.Columns.Count; i++)
			{
				if (i > 0)
				{
					NumericSeries series = CRHelper.GetNumericSeries(i, dtChart2);
					series.Label = dtChart2.Columns[i].ColumnName;
					UltraChart2.Series.Add(series);
				}
				for (int j = 0; j < dtChart2.Rows.Count; ++j)
				{
					if (Towns2[j] != null)
					{
						Towns2[j] += ";";
					}
					if (dtChart2.Rows[j][i] != DBNull.Value)
					{
						Towns2[j] += dtChart2.Rows[j][i].ToString();
					}
				}
			}
		}

		protected void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			string label = String.Empty;
			int j = 0;
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];
				if (primitive is Box)
				{
					Box box = (Box)primitive;
					if (box.DataPoint != null)
					{
						switch (box.DataPoint.Label)
						{
							case "Цена (без учета прироста)":
								{
									label = UltraChart_MakeLabelForDataPoint(Towns2[j], yearDateText);
									box.DataPoint.Label = label;
									++j;
									break;
								}
							case "Прирост цены, входящий в состав текущей цены":
								{
									box.DataPoint.Label = label;
									break;
								}
							case "Снижение цены ":
								{
									box.DataPoint.Label = label;
									break;
								}
						}
					}
				}
			}
		}
		#endregion

		// --------------------------------------------------------------------

		#region Заполнение словарей и выпадающих списков параметров

		protected void FillComboRegion(string queryName)
		{
			// Загрузка списка продуктов
			DataTable dtRegion = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtRegion);
			// Закачку придется делать через словарь
			dictRegions = new Dictionary<string, string>();
			allRegions.Value = String.Empty;
			Dictionary<string, int> dict = new Dictionary<string, int>();
			foreach (DataRow row in dtRegion.Rows)
			{
				string regionType = row[7].ToString();
				if (regionType == "ГП")
				{
					AddPairToDictionary(dict, row[5].ToString(), 0);
					dictRegions.Add(row[5].ToString(), row[6].ToString());
					if (allRegions.Value == String.Empty)
					{
						allRegions.Value = row[6].ToString();
					}
					else
					{
						allRegions.Value += ", " + row[6].ToString();
					}
				}
				else
				{
					if (regionType == "ГО")
					{
						AddPairToDictionary(dict, row[4].ToString(), 0);
						dictRegions.Add(row[4].ToString(), row[6].ToString());
						if (allRegions.Value == String.Empty)
						{
							allRegions.Value = row[6].ToString();
						}
						else
						{
							allRegions.Value += ", " + row[6].ToString();
						}
					}
				}
			}
			ComboRegion.FillDictionaryValues(dict);
			if (dict.ContainsKey("Город Вологда"))
			{
				ComboRegion.SetСheckedState("Город Вологда", true);
			}
		}

		protected void FillComboDate(string queryName)
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
				AddPairToDictionary(dictDate, year + " год", 0);
				AddPairToDictionary(dictDate, month + " " + year + " года", 1);
				AddPairToDictionary(dictDate, day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года", 2);
				if (firstDate.Value == String.Empty)
				{
					firstDate.Value = StringToMDXDate(day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года");
				}
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
			string mdxRegionString;
			return (dictRegions.TryGetValue(comboRegionValue, out mdxRegionString)) ? mdxRegionString : String.Empty;
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

		public string getPreviousDate(string date)
		{
			string[] dateElements = date.Split(' ');
			int year = Convert.ToInt32(dateElements[2]);
			int month = CRHelper.MonthNum(dateElements[1]);
			int day = Convert.ToInt32(dateElements[0]);
			DateTime dt = new DateTime(year, month, day);
			dt = dt.AddDays(-7);
			day = dt.Day;
			month = dt.Month;
			year = dt.Year;
			return String.Format("{0} {1} {2} года", day, CRHelper.RusMonthGenitive(month), year);
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
			ISection section4 = report.AddSection();

			ChartByTime.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
			UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
			UltraChart2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

			ReportPDFExporter1.HeaderCellHeight = 70;
			GridHeaderLayout workHeader = new GridHeaderLayout();
			workHeader = headerLayout;
			workHeader.childCells.Remove(workHeader.childCells[2]);
			workHeader.childCells.Remove(workHeader.childCells[1]);
			ReportPDFExporter1.Export(workHeader, section1);
			ReportPDFExporter1.Export(ChartByTime, LabelChartByTime.Text, section2);
			ReportPDFExporter1.Export(UltraChart1, LabelChart1.Text, section3);
			ReportPDFExporter1.Export(UltraChart2, LabelChart2.Text, section4);

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
			Worksheet sheet4 = workbook.Worksheets.Add("Диаграмма 3");

			SetExportGridParams(headerLayout.Grid);

			ReportExcelExporter1.HeaderCellHeight = 30;
			ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
			ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
			ReportExcelExporter1.TitleStartRow = 0;

			GridHeaderLayout workHeader = new GridHeaderLayout();
			workHeader = headerLayout;
			workHeader.childCells.Remove(workHeader.childCells[2]);
			workHeader.childCells.Remove(workHeader.childCells[1]);

			ReportExcelExporter1.Export(workHeader, sheet1, 3);

			ReportExcelExporter1.WorksheetTitle = String.Empty;
			ReportExcelExporter1.WorksheetSubTitle = String.Empty;

			ChartByTime.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.7);
			ReportExcelExporter1.Export(ChartByTime, LabelChartByTime.Text, sheet2, 1);
			UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.7);
			ReportExcelExporter1.Export(UltraChart1, LabelChart1.Text, sheet3, 2);
			UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.7);
			ReportExcelExporter1.Export(UltraChart2, LabelChart2.Text, sheet4, 2);

			// Ручная настройка
			sheet1.Columns[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
			sheet1.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			sheet1.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
			sheet1.Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			sheet1.Rows[1].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;

			sheet2.MergedCellsRegions.Clear();
			sheet3.MergedCellsRegions.Clear();
			sheet3.MergedCellsRegions.Add(0, 0, 1, 17);
			sheet3.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
			sheet4.MergedCellsRegions.Clear();
			sheet4.MergedCellsRegions.Add(0, 0, 1, 17);
			sheet4.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
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
			double coeff = 1.1;
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
