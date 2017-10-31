using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.WRP_0001_0001
{
	/// <summary>
	/// Валовый региональный продукт
	/// </summary>
	public partial class Default : CustomReportPage
	{
		#region Поля

		private GridHeaderLayout headerLayout;
		private DataTable dtGrid;
		private DataTable dtChart1;
		private static Dictionary<string, string> dictUnits;
		private static int selectedGridRow = 0;

		#endregion

		#region Параметры запроса

		// выбранная дата
		private CustomParam selectedYear;
		private CustomParam startYear;
		private CustomParam selectedAction;
		private CustomParam selectedParameter;

		#endregion
		// --------------------------------------------------------------------

		// заголовок страницы
		private const string PageTitleCaption = "Валовой региональный продукт";
		private const string PageSubTitleCaption = "Анализ динамики валового регионального продукта и источников его возникновения по ОКВЭД, Ханты-Мансийский автономный округ – Югра, {0} год";
		// заголовок для UltraChart
		private const string Chart1TitleCaption = "Динамика выпуска продукции и доля ВРП в нем";
		private const string Chart2TitleCaption = "Стркутура выпуска продукции, по ОКВЭД, {0}г.";
		private const string Chart3TitleCaption = "Структура промежуточного потребления, по ОКВЭД, {0}г.";
		private const string Chart4TitleCaption = "Структура валового регионального продукта, по ОКВЭД, {0}г.";

		// --------------------------------------------------------------------

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			ComboDate.Title = "Год";
			ComboDate.Width = 100;
			ComboDate.ParentSelect = true;
			PageTitle.Text = PageTitleCaption;
			Page.Title = PageTitleCaption;
			#region Настройка грида
			UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
			UltraWebGrid.Height = Unit.Empty;
			UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
			UltraWebGrid.DisplayLayout.SelectTypeRowDefault = SelectType.Single;
			UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);
			UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
			UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
			UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
			#endregion

			PanelCharts.AddLinkedRequestTrigger(UltraWebGrid);
			
			#region Настройка диаграммы 1
			UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
			UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);
			LabelChart1.Width = UltraChart1.Width;

			UltraChart1.ChartType = ChartType.StackColumnChart;
			UltraChart1.Border.Thickness = 0;

			UltraChart1.Axis.X.Extent = 50;
			UltraChart1.Axis.X.LineThickness = 3;
			UltraChart1.Axis.Y.Extent = 50;
			UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

			UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
			UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 12);
			UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;

			UltraChart1.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart1.Axis.X.Margin.Near.Value = 20;

			//UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
			UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;

			UltraChart1.Legend.Visible = true;
			UltraChart1.Legend.Location = LegendLocation.Bottom;
			UltraChart1.Legend.SpanPercentage = 10;
			UltraChart1.Border.Thickness = 0;

			UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>";
			UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
			UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);

			#endregion

			UltraChartInit(UltraChart2);
			LabelChart2.Width = UltraChart2.Width;
			UltraChartInit(UltraChart3);
			UltraChartInit(UltraChart4);

			#region Диаграмма-легенда
			UltraChart5.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
			UltraChart5.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.18);

			UltraChart5.Legend.Visible = true;
			UltraChart5.Legend.Location = LegendLocation.Top;
			UltraChart5.Legend.SpanPercentage = 100;
			UltraChart5.Border.Thickness = 0;

			UltraChart5.ChartType = ChartType.PieChart;
			UltraChart5.PieChart.OthersCategoryPercent = 0;
			UltraChart5.ColorModel.ModelStyle = ColorModels.CustomSkin;
			CRHelper.CopyCustomColorModel(UltraChart2, UltraChart5);

			UltraChart5.DataBinding += new EventHandler(chart_DataBinding);
			UltraChart5.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart5_FillSceneGraph);

			#endregion

			#region Параметры
			selectedYear = UserParams.CustomParam("selected_year");
			startYear = UserParams.CustomParam("start_year");
			selectedAction = UserParams.CustomParam("selected_action");
			selectedParameter = UserParams.CustomParam("selected_parameter");
			#endregion

		}

		// --------------------------------------------------------------------

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);
			if (!Page.IsPostBack)
			{
				FillComboDate("WRP_0001_0001_list_of_dates");
				FillUnitsDict("WRP_0001_0001_units");
			}
			#region Анализ параметров
			selectedYear.Value = ComboDate.SelectedValue;
			#endregion

			PageSubTitle.Text = String.Format(PageSubTitleCaption, selectedYear.Value);
			LabelChart2.Text = String.Format(Chart2TitleCaption, selectedYear.Value);
			LabelChart3.Text = String.Format(Chart3TitleCaption, selectedYear.Value);
			LabelChart4.Text = String.Format(Chart4TitleCaption, selectedYear.Value);
			
			headerLayout = new GridHeaderLayout(UltraWebGrid);

			UltraWebGrid.DataBind();
			UltraWebGrid_ActivateRow(UltraWebGrid.Rows[selectedGridRow]);
			UltraChart2.DataBind();
			UltraChart3.DataBind();
			UltraChart4.DataBind();
			UltraChart5.DataBind();
		}

		// --------------------------------------------------------------------
		#region Обработчики грида

		protected void UltraWebGrid_ActivateRow(UltraGridRow row)
		{
			selectedAction.Value = GetAction(row.Cells[0].GetText());
			selectedGridRow = row.Index;
			row.Activate();
			row.Selected = true;
			if (row.Cells[0].GetText() == "Всего ")
			{
				LabelChart1.Text = Chart1TitleCaption + ", вид экономической деятельности - «Все виды»";
			}
			else
			{
				LabelChart1.Text = Chart1TitleCaption + String.Format(", вид экономической деятельности - «{0}»", row.Cells[0].GetText());
			}
			UltraChart1.DataBind();
		}

		protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
		{
			if (PanelCharts.IsAsyncPostBack)
			{
				UltraWebGrid_ActivateRow(e.Row);
			}
		}

		protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("WRP_0001_0001_grid");
			dtGrid = new DataTable();
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Вид экономической деятельности", dtGrid);
			if (dtGrid.Rows.Count > 0)
			{
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
				k = 0.95;
			}
			else if (Browser == "AppleMAC-Safari")
			{
				k = 0.9;
			}
			e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
			e.Layout.CellClickActionDefault = CellClickAction.RowSelect;
			e.Layout.NullTextDefault = "-";
			e.Layout.RowSelectorStyleDefault.Width = 20;
			e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
			double gridWidth = UltraWebGrid.Width.Value - e.Layout.RowSelectorStyleDefault.Width.Value - 10;
			double columnWidth = CRHelper.GetColumnWidth(gridWidth * 0.8 / 9 * k);
			e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(gridWidth * 0.2 * k - 10);
			for (int i = 1; i < e.Layout.Bands[0].Columns.Count; ++i)
			{
				e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
				e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Left = 5;
				if (i % 3 == 1)
				{
					CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
				}
				else
				{
					CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
				}
			}

			// Заголовки
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
			headerLayout.AddCell("Вид экономической деятельности");
			GridHeaderCell header = null;
			string unit;
			dictUnits.TryGetValue("Выпуск", out unit);
			header = headerLayout.AddCell("Выпуск<br/>(V1)");
			header.AddCell("Значение, " + unit.ToLower());
			header.AddCell("Доля в общем объеме");
			header.AddCell("ТП");
			dictUnits.TryGetValue("Промежуточное потребление", out unit);
			header = headerLayout.AddCell("Промежуточное потребление<br/>(V2)");
			header.AddCell("Значение, " + unit.ToLower());
			header.AddCell("Доля в общем объеме");
			header.AddCell("ТП");
			dictUnits.TryGetValue("Валовой региональный продукт в текущих ценах", out unit);
			header = headerLayout.AddCell("Валовой региональный продукт<br/>(GRP=V1-V2)");
			header.AddCell("Значение, " + unit.ToLower());
			header.AddCell("Доля в общем объеме");
			header.AddCell("ТП");
			headerLayout.ApplyHeaderInfo();
		}

		protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
		{
			if (e.Row.Cells[0].Text.Trim().ToLower() == "всего")
			{
				e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Right;
				e.Row.Style.Font.Bold = true;
			}
			else
			{
			for (int i = 3; i < e.Row.Cells.Count; i += 3)
			{
				double value;
				if (Double.TryParse(e.Row.Cells[i].Text, out value))
				{
					e.Row.Cells[i].Style.CssClass = value > 0 ? "ArrowUpGreen" : value < 0 ? "ArrowDownRed" : String.Empty;
				}
			}
			}
		}

		#endregion

		#region Обработчики диаграммы 1

		void UltraChart1_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("WRP_0001_0001_chart1");
			dtChart1 = new DataTable();
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);
			UltraChart1.Series.Clear();
			UltraChart1.Data.SwapRowsAndColumns = true;
			for (int i = 1; i < dtChart1.Columns.Count; i++)
			{
				NumericSeries series = CRHelper.GetNumericSeries(i, dtChart1);
				series.Label = dtChart1.Columns[i].ColumnName;
				UltraChart1.Series.Add(series);
			}
		}

		void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			string label = String.Empty;
			int j = 0;
			int k = 0;
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
							case "Промежуточное потребление":
								{
									label = "Промежуточное потребление;\n";
									label += String.Format("{0:N2}, ", Convert.ToDouble(dtChart1.Rows[j][1]));
									string unit = String.Empty;
									dictUnits.TryGetValue("Промежуточное потребление", out unit);
									label += unit.ToLower() + ";\n";
									label += String.Format("{0:P2}", Convert.ToDouble(dtChart1.Rows[j][1]) / (Convert.ToDouble(dtChart1.Rows[j][1]) + Convert.ToDouble(dtChart1.Rows[j][2])));
									box.DataPoint.Label = label;
									++j;
									break;
								}
							case "Валовой региональный продукт в текущих ценах":
								{
									label = "Валовой региональный продукт в текущих ценах;\n";
									label += String.Format("{0:N2}, ", Convert.ToDouble(dtChart1.Rows[k][2]));
									string unit = String.Empty;
									dictUnits.TryGetValue("Валовой региональный продукт в текущих ценах", out unit);
									label += unit.ToLower() + ";\n";
									label += String.Format("{0:P2}", Convert.ToDouble(dtChart1.Rows[k][2]) / (Convert.ToDouble(dtChart1.Rows[k][1]) + Convert.ToDouble(dtChart1.Rows[k][2])));
									box.DataPoint.Label = label;
									++k;
									break;
								}
						}
					}
				}
			}
		}

		#endregion

		#region Обработчики диаграмм 2,3 и 4 - они все равно одинаковые

		protected void UltraChartInit(UltraChart chart)
		{
			chart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.32);
			chart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);

			chart.Border.Thickness = 0;

			chart.ChartType = ChartType.PieChart;

			chart.PieChart.OthersCategoryPercent = 0;

			chart.ColorModel.ModelStyle = ColorModels.CustomSkin;

			chart.Tooltips.FormatString = "<ITEM_LABEL>";

			switch (chart.ID)
			{
				case "UltraChart2":
					{
						CRHelper.FillCustomColorModel(chart, 18, true);
						break;
					}
				case "UltraChart3":
					{
						CRHelper.CopyCustomColorModel(UltraChart2, chart);
						break;
					}
				case "UltraChart4":
					{
						CRHelper.CopyCustomColorModel(UltraChart2, chart);
						break;
					}
				case "UltraChart5":
					{
						CRHelper.CopyCustomColorModel(UltraChart2, chart);
						chart.Tooltips.FormatString = String.Empty;
						break;
					}
			}

			chart.DataBinding += new EventHandler(chart_DataBinding);
			chart.FillSceneGraph += new FillSceneGraphEventHandler(chart_FillSceneGraph);
		}

		protected void chart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			string label = String.Empty;
			UltraChart chart = sender as UltraChart;
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];
				if (primitive is Wedge)
				{
					Wedge wedge = (Wedge)primitive;
					if (wedge.DataPoint != null)
					{
						label = wedge.DataPoint.Label + ";\n";
						label += String.Format("{0:N2}, ", primitive.Value);
						switch (chart.ID)
						{
							case "UltraChart2":
								{
									string unit = String.Empty;
									dictUnits.TryGetValue("Выпуск", out unit);
									label += unit.ToLower() + ";\n";
									break;
								}
							case "UltraChart3":
								{
									string unit = String.Empty;
									dictUnits.TryGetValue("Промежуточное потребление", out unit);
									label += unit.ToLower() + ";\n";
									break;
								}
							case "UltraChart4":
								{
									string unit = String.Empty;
									dictUnits.TryGetValue("Валовой региональный продукт в текущих ценах", out unit);
									label += unit.ToLower() + ";\n";
									break;
								}
						}
						label += String.Format("{0:N2}%", wedge.Percent);
						wedge.DataPoint.Label = label;
					}
				}
			}
		}

		protected void chart_DataBinding(object sender, EventArgs e)
		{
			UltraChart chart = sender as UltraChart;
			DataTable dtChart = null;
			if (chart.ID == "UltraChart2")
			{
				selectedParameter.Value = "Выпуск";
			}
			else if (chart.ID == "UltraChart3")
			{
				selectedParameter.Value = "Промежуточное потребление";
			}
			else
			{
				selectedParameter.Value = "Валовой региональный продукт в текущих ценах";
			}
			string query = DataProvider.GetQueryText("WRP_0001_0001_charts");
			dtChart = new DataTable();
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
			chart.DataSource = (dtChart == null) ? null : dtChart.DefaultView;
		}

		#endregion

		#region Диаграмма-легенда

		private void UltraChart5_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];
				if (!(!String.IsNullOrEmpty(primitive.Path) && (primitive.Path.Contains("Legend"))))
				{
					primitive.Visible = false;
				}
			}
		}

		#endregion

		// --------------------------------------------------------------------

		#region Заполнение словарей и выпадающих списков параметров

		protected void FillUnitsDict(string queryName)
		{
			DataTable dtUnits = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForPivotTable(query, dtUnits);
			dictUnits = new Dictionary<string, string>();
			for (int i = 1; i < dtUnits.Columns.Count; ++i)
			{
				dictUnits.Add(UserComboBox.getLastBlock(dtUnits.Columns[i].ColumnName), dtUnits.Rows[0][i].ToString());
			}
		}

		protected void FillComboDate(string queryName)
		{
			// Загрузка списка актуальных дат
			DataTable dtDate = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForPivotTable(query, dtDate);
			if (dtDate.Rows.Count == 0)
			{
				throw new Exception("Данные по актуальным датам не найдены");
			}
			// Закачку придется делать через словарь
			Dictionary<string, int> dictDate = new Dictionary<string, int>();
			for (int row = 0; row < dtDate.Rows.Count; ++row)
			{
				string year = dtDate.Rows[row][0].ToString();
				AddPairToDictionary(dictDate, year, 0);
				if (String.IsNullOrEmpty(startYear.Value))
				{
					startYear.Value = year;
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

		static string Browser
		{
			get { return HttpContext.Current.Request.Browser.Browser; }
		}

		private string GetAction(string action)
		{
			if (action == "Всего ")
			{
				return "[ОК__ОКВЭД].[ОК__ОКВЭД].[Все виды экономической деятельности]";
			}
			else
			{
				return String.Format("[ОК__ОКВЭД].[ОК__ОКВЭД].[Все виды экономической деятельности].[{0}]", action);
			}
		}

	}
}
