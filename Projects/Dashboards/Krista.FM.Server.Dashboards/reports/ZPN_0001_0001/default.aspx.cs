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
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.ZPN_0001_0001
{
	/// <summary>
	/// Анализ динамики среднемесячной заработной платы и НДФЛ в разрезе ОКВЭД в субъектах Российской Федерации, входящих в Уральский федеральный округ.
	/// </summary>
	public partial class Default : CustomReportPage
	{
		#region Поля

		private DataTable dtDate;
		private DataTable dtRegion;
		private DataTable dtGrid;
		private DataTable dtChart1;
		private DataTable dtChart1_1;
		private DataTable dtChart2;
		private DataTable dtChartLegend;
		private DataTable dtChart3;
		private GridHeaderLayout headerLayout;

		#endregion

		#region Параметры запроса

		// выбранная дата
		private CustomParam selectedYear;
		private CustomParam selectedAction;
		private CustomParam selectedRegion;
		#endregion

		// --------------------------------------------------------------------

		// заголовок страницы
		private const string PageTitleCaption = "Соотношение объема среднемесячной заработной платы и НДФЛ по видам деятельности";
		private const string PageSubTitleCaption = "Анализ динамики объема среднемесячной заработной платы и НДФЛ в разрезе ОКВЭД в субъектах Российской Федерации, входящих в Уральский федеральный округ, {0} год";
		private const string Chart1TitleCaption = "Динамика и соотношение объема среднемесячной заработной платы и НДФЛ по виду деятельности «{0}», {1}, миллион рублей";
		private const string Chart2TitleCaption = "Динамика задолженности по НДФЛ по виду деятельности «{0}», миллион рублей";
		private const string Chart3TitleCaption = "НДФЛ по виду деятельности «{0}», миллион рублей";

		// --------------------------------------------------------------------

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			ComboDate.Title = "Год";
			ComboDate.Width = 100;
			ComboRegion.MultiSelect = false;
			ComboDate.ParentSelect = true;

			ComboRegion.Width = 350;
			ComboRegion.MultiSelect = false;
			ComboRegion.ParentSelect = true;
			ComboRegion.Title = "Территория";

			ComboActions.Width = 650;
			ComboActions.MultiSelect = false;
			ComboActions.ParentSelect = true;
			ComboActions.Title = "Вид деятельности";

			#region Настройка диаграммы 1
			UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
			UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);
			LabelChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
			UltraChart1.ChartType = ChartType.ColumnChart;
			UltraChart1.Border.Thickness = 0;

			UltraChart1.ColumnChart.SeriesSpacing = 1;

			UltraChart1.Data.ZeroAligned = true;

			UltraChart1.Axis.X.Extent = 25;
			UltraChart1.Axis.X.Labels.Visible = false;
			UltraChart1.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
			UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
			UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
			UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
			UltraChart1.Axis.Y.Extent = 60;
			UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

			UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;
			UltraChart1.ColorModel.ColorBegin = Color.DarkGoldenrod;
			UltraChart1.ColorModel.ColorEnd = Color.Navy;

			UltraChart1.TitleTop.Visible = true;
			UltraChart1.TitleTop.Text = "Млн.руб.";
			UltraChart1.TitleTop.HorizontalAlign = StringAlignment.Near;

			UltraChart1.Legend.Visible = true;
			UltraChart1.Legend.Font = new Font("Verdana", 10);
			UltraChart1.Legend.Location = LegendLocation.Bottom;
			UltraChart1.Legend.SpanPercentage = 12;
			UltraChart1.Border.Thickness = 0;

			UltraChart1.Tooltips.FormatString = "<b><DATA_VALUE:N3></b> миллион рублей";
			UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
			UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			#endregion

			#region Настройка диаграммы 2
			UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
			UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4);
			LabelChart2.Width = UltraChart2.Width;

			UltraChart2.ChartType = ChartType.StackColumnChart;
			UltraChart2.Border.Thickness = 0;

			UltraChart2.Data.ZeroAligned = true;
			
			UltraChart2.Axis.X.Extent = 50;
			//UltraChart2.Axis.X.LineThickness = 3;
			UltraChart2.Axis.Y.Extent = 50;
			UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

			UltraChart2.Axis.X.Labels.SeriesLabels.Visible = true;
			UltraChart2.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 12);
			UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;

			//UltraChart2.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			//UltraChart2.Axis.X.Margin.Near.Value = 20;

			UltraChart2.ColorModel.ModelStyle = ColorModels.CustomSkin;

			AddLineAppearencesUltraChart(UltraChart2);

			UltraChart2.TitleTop.Visible = true;
			UltraChart2.TitleTop.Text = "Млн.руб.";
			UltraChart2.TitleTop.HorizontalAlign = StringAlignment.Near;

			UltraChart2.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>: <b><DATA_VALUE:N3></b> миллион рублей";
			UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
			UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);

			#endregion

			#region Настройка диаграммы, которая легенда
			UltraChartLegend.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
			UltraChartLegend.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.12);
			UltraChartLegend.Border.Thickness = 0;

			UltraChartLegend.ChartType = ChartType.LineChart;
			UltraChartLegend.LineChart.NullHandling = NullHandling.Zero;

			UltraChartLegend.Legend.Visible = true;
			UltraChartLegend.Legend.Font = new Font("Verdana", 10);
			UltraChartLegend.Legend.Location = LegendLocation.Top;
			UltraChartLegend.Legend.SpanPercentage = 100;
			UltraChartLegend.Border.Thickness = 0;

			AddLineAppearencesUltraChart(UltraChartLegend);

			UltraChartLegend.DataBinding += new EventHandler(UltraChartLegend_DataBinding);
			UltraChartLegend.FillSceneGraph += new FillSceneGraphEventHandler(UltraChartLegend_FillSceneGraph);

			#endregion

			#region Настройка диаграммы 3
			UltraChart3.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
			UltraChart3.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.75);
			LabelChart3.Width = UltraChart3.Width;

			UltraChart3.ChartType = ChartType.ParetoChart;
			UltraChart3.Data.SwapRowsAndColumns = true;
			
			UltraChart3.BorderWidth = 0;
			UltraChart3.Axis.X.Extent = 50;
			UltraChart3.Axis.Y.Extent = 50;

			UltraChart3.Data.ZeroAligned = true;

			UltraChart3.TitleTop.Visible = true;
			UltraChart3.TitleTop.Text = "Млн.руб.";
			UltraChart3.TitleTop.HorizontalAlign = StringAlignment.Near;

			UltraChart3.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
			UltraChart3.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
			UltraChart3.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
			UltraChart3.ParetoChart.ColumnSpacing = 0;
			
			UltraChart3.ColorModel.ModelStyle = ColorModels.CustomSkin;

			AddLineAppearencesUltraChart(UltraChart3);

			UltraChart3.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			UltraChart3.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart3_FillSceneGraph);
			UltraChart3.DataBinding += new EventHandler(UltraChart3_DataBinding);
			
			#endregion

			CRHelper.CopyCustomColorModel(UltraChartLegend, UltraChart2);
			CRHelper.CopyCustomColorModel(UltraChartLegend, UltraChart3);

			#region Настройка грида
			UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30);
			UltraWebGrid.Height = Unit.Empty;
			UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
			UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
			UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
			UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
			headerLayout = new GridHeaderLayout(UltraWebGrid);
			UltraWebGrid.Bands.Clear();
			#endregion

			#region Параметры
			selectedYear = UserParams.CustomParam("selected_year");
			selectedAction = UserParams.CustomParam("selected_action");
			selectedRegion = UserParams.CustomParam("selected_region");
			#endregion
		}

		// --------------------------------------------------------------------

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);
			if (!Page.IsPostBack)
			{
				FillComboDate("ZPN_0001_0001_list_of_dates");
				FillComboRegion("ZPN_0001_0001_list_of_regions");
				FillComboActions();
			}
			#region Анализ параметров
			selectedYear.Value = ComboDate.SelectedValue;
			selectedAction.Value = ComboActions.SelectedValue.Split('.')[1].Trim();
			selectedRegion.Value = ComboRegion.SelectedValue;
			PageTitle.Text = PageTitleCaption;
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = String.Format(PageSubTitleCaption, selectedYear.Value);
			#endregion

			UltraChart3.Tooltips.FormatString = String.Format("<ITEM_LABEL>\n{0} год,\n<b><DATA_VALUE:N3></b> миллион рублей", selectedYear.Value);

			UltraChart1.DataBind();
			UltraChart2.DataBind();
			UltraChartLegend.DataBind();
			UltraChart3.DataBind();
			UltraWebGrid.DataBind();
		}

		// --------------------------------------------------------------------

		#region Обработчики грида

		private void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("ZPN_0001_0001_grid_fns28");
			dtGrid = new DataTable();
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);
			foreach (DataRow row in dtGrid.Rows)
			{
				for (int i = 1; i < dtGrid.Columns.Count; i += 3)
				{
					row[i] = DBNull.Value;
				}
			}
			query = DataProvider.GetQueryText("ZPN_0001_0001_grid_stat_trud");
			DataTable dtGridStat = new DataTable();
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Период", dtGridStat);
			foreach (DataRow row in dtGridStat.Rows)
			{
				int i = 0, j = 0;
				for (i = 0; (i < dtGrid.Rows.Count) && (row[0].ToString() != dtGrid.Rows[i][0].ToString()); ++i) {}
				if (i < dtGrid.Rows.Count)
				{
					int rowIndex = i;
					for (i = 1; i < row.ItemArray.Length; ++i)
					{
						for (j = 1; (j < dtGrid.Columns.Count) && (dtGridStat.Columns[i].ColumnName != dtGrid.Columns[j].ColumnName); ++j) {}
						if (j < dtGrid.Columns.Count)
						{
							dtGrid.Rows[rowIndex][j] = row[i];
						}
					}
				}
			}
			if (dtGrid.Rows.Count > 0)
			{
				DataRow row = dtGrid.NewRow();
				row[0] = selectedYear.Value;
				dtGrid.Rows.InsertAt(row, 0);
			}
			UltraWebGrid.DataSource = dtGrid == null ? null : dtGrid.DefaultView;
		}

		private void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
		{
			// Заголовки
			e.Layout.AllowSortingDefault = AllowSorting.No;
			e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
			e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
			headerLayout.AddCell("Период");
			GridHeaderCell header = null;
			for (int i = 1; i < dtGrid.Columns.Count; ++i)
			{
				e.Layout.Bands[0].Columns[i].Width = 115;
				if (i % 3 == 1)
				{
					string region = dtGrid.Columns[i].Caption.Split(';')[0].Trim();
					header = headerLayout.AddCell(region);
					header.AddCell("Объем среднемесячной заработной платы, рубль");
					header.AddCell("НДФЛ поступило, рубль");
					header.AddCell("Задолженность по НДФЛ, рубль");
				}
				CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
			}
			headerLayout.ApplyHeaderInfo();
		}

		private void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
		{
			for (int i = 3; i < e.Row.Cells.Count; i += 3)
			{
				double value;
				UltraGridCell cell = e.Row.Cells[i];
				if (Double.TryParse(cell.GetText(), out value))
				{
					if (value < 0)
					{
						cell.Style.ForeColor = Color.Red;
						cell.Title = "Кредиторская задолженность";
					}
					else
					{
						cell.Style.ForeColor = Color.Black;
						cell.Title = "Дебиторская задолженность";
					}
				}
			}
		}

		#endregion

		// --------------------------------------------------------------------

		private void AddLineAppearencesUltraChart(UltraChart chart)
		{
			chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
			chart.ColorModel.Skin.ApplyRowWise = false;
			chart.ColorModel.Skin.PEs.Clear();

			for (int i = 1; i <= 7; i++)
			{
				PaintElement pe = new PaintElement();
				Color color = Color.White;
				color = GetColor(i);

				pe.Fill = color;
				pe.StrokeWidth = 1;
				chart.ColorModel.Skin.PEs.Add(pe);
				pe.Stroke = Color.Black;
				pe.StrokeWidth = 1;
				pe.FillGradientStyle = GradientStyle.None;
				LineAppearance lineAppearance2 = new LineAppearance();

				lineAppearance2.IconAppearance.Icon = SymbolIcon.Square;
				lineAppearance2.IconAppearance.IconSize = SymbolIconSize.Medium;
				lineAppearance2.IconAppearance.PE = pe;

				chart.LineChart.LineAppearances.Add(lineAppearance2);

				chart.LineChart.Thickness = 0;
			}

			if (chart.ChartType == ChartType.ParetoChart)
			{
				PaintElement pe = new PaintElement();
				Color color = Color.Blue;

				pe.Fill = color;
				pe.StrokeWidth = 1;
				chart.ParetoChart.LinePE = pe;
			}
		}

		#region Обработчики диаграммы 1

		private void UltraChart1_DataBinding(object sender, EventArgs e)
		{
			LabelChart1.Text = String.Format(Chart1TitleCaption, selectedAction.Value, selectedRegion.Value);
			string query = DataProvider.GetQueryText("ZPN_0001_0001_chart1_stat_trud");
			dtChart1 = new DataTable();
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Розничная цена", dtChart1);
			DataColumn column = new DataColumn("НДФЛ поступило", typeof(Double));
			dtChart1.Columns.Add(column);
			query = DataProvider.GetQueryText("ZPN_0001_0001_chart1_fns28");
			dtChart1_1 = new DataTable();
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Розничная цена", dtChart1_1);
			foreach (DataRow row1_1 in dtChart1_1.Rows)
			{
				foreach (DataRow row1 in dtChart1.Rows)
				{
					if (row1[0].ToString() == row1_1[0].ToString())
					{
						row1[3] = Convert.ToDouble(row1_1[1]);
					}
				}
			}
			UltraChart1.DataSource = (dtChart1 == null) ? null : dtChart1.DefaultView;
		}

		#endregion

		#region Обработчики диаграммы 2

		private void UltraChart2_DataBinding(object sender, EventArgs e)
		{
			LabelChart2.Text = String.Format(Chart2TitleCaption, selectedAction.Value);
			string query = DataProvider.GetQueryText("ZPN_0001_0001_chart2");
			dtChart2 = new DataTable();
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart2);
			UltraChart2.DataSource = (dtChart2 == null) ? null : dtChart2.DefaultView;
		}

		private void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];
				/*
				if (primitive is Box && primitive.DataPoint != null)
				{
					Color color = GetColor(GetRegionNum(primitive.DataPoint.Label));
					primitive.PE.Fill = color;
					primitive.PE.Stroke = Color.Black;
					primitive.PE.FillGradientStyle = GradientStyle.None;
					primitive.PE.StrokeWidth = 1;
				}*/
			}
		}

		#endregion

		#region Обработчики диаграммы-легенды

		private void UltraChartLegend_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("ZPN_0001_0001_chartLegend");
			dtChartLegend = new DataTable();
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChartLegend);
			UltraChartLegend.DataSource = (dtChartLegend == null) ? null : dtChartLegend.DefaultView;
		}

		private void UltraChartLegend_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
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

		#region Обработчики диаграммы 3

		private void UltraChart3_DataBinding(object sender, EventArgs e)
		{
			LabelChart3.Text = String.Format(Chart3TitleCaption, selectedAction.Value);
			dtChart3 = new DataTable();
			string query = DataProvider.GetQueryText("ZPN_0001_0001_chart3");
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtChart3);
			for (int i = 1; i < dtChart3.Columns.Count; i++)
			{
				NumericSeries series = CRHelper.GetNumericSeries(i, dtChart3);
				UltraChart3.Series.Add(series);
			} 
		}

		private void UltraChart3_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];
				if (primitive is Text)
				{
					Text text = (Text)primitive;
					text.SetTextString(RegionsNamingHelper.ShortName(text.GetTextString()));
				}
				/*if (primitive is Box && primitive.DataPoint != null)
				{
					Color color = GetColor(GetRegionNum(primitive.DataPoint.Label));
					primitive.PE.Fill = color;
					primitive.PE.Stroke = Color.Black;
					primitive.PE.FillGradientStyle = GradientStyle.None;
					primitive.PE.StrokeWidth = 1;
				}*/
			}
		}

		#endregion

		// --------------------------------------------------------------------

		#region Заполнение словарей и выпадающих списков параметров

		protected void FillComboDate(string queryName)
		{
			dtDate = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
			Dictionary<string, int> dictDate = new Dictionary<string, int>();
			for (int row = 0; row < dtDate.Rows.Count; ++row)
			{
				string year = dtDate.Rows[row][0].ToString();
				AddPairToDictionary(dictDate, year, 0);
			}
			ComboDate.FillDictionaryValues(dictDate);
			//ComboDate.SelectLastNode();
			ComboDate.SetСheckedState("2010", true);
		}

		protected void FillComboRegion(string queryName)
		{
			dtRegion = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtRegion);
			Dictionary<string, int> dictRegion = new Dictionary<string, int>();
			for (int row = 0; row < dtRegion.Rows.Count; ++row)
			{
				string region = dtRegion.Rows[row][0].ToString();
				AddPairToDictionary(dictRegion, region, 0);
			}
			ComboRegion.FillDictionaryValues(dictRegion);
			ComboRegion.SetСheckedState("Курганская область", true);
		}

		protected void FillComboActions()
		{
			Dictionary<string, int> dictActions = new Dictionary<string, int>();
			AddPairToDictionary(dictActions, "Раздел А. Сельское хозяйство, охота и лесное хозяйство", 0);
			AddPairToDictionary(dictActions, "Раздел С. Добыча полезных ископаемых", 0);
			AddPairToDictionary(dictActions, "Раздел D. Обрабатывающие производства", 0);
			AddPairToDictionary(dictActions, "Раздел E. Производство и распределение электроэнергии, газа и воды", 0);
			AddPairToDictionary(dictActions, "Раздел F. Строительство", 0);
			AddPairToDictionary(dictActions, "Раздел I. Транспорт и связь", 0);
			AddPairToDictionary(dictActions, "Раздел M. Образование", 0);
			AddPairToDictionary(dictActions, "Раздел N. Здравоохранение и предоставление социальных услуг", 0);
			ComboActions.FillDictionaryValues(dictActions);
			ComboActions.SetСheckedState("Раздел F. Строительство", true);
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

		private static Color GetColor(int i)
		{
			Color color = Color.Aquamarine;
			switch (i)
			{
				case 1:
					{
						// Курганская
						color = Color.Green;
						break;
					}
				case 2:
					{
						// Свердловская
						color = Color.Gold;
						break;
					}
				case 3:
					{
						// Тюменская
						color = Color.Black;
						break;
					}
				case 4:
					{
						// ХМАО
						color = Color.LightSlateGray;
						break;
					}
				case 5:
					{
						// Челябинская
						color = Color.Red;
						break;
					}
				case 6:
					{
						// ЯНАО
						color = Color.Blue;
						break;
					}
				case 7:
					{
						color = Color.DarkViolet;
						break;
					}
			}
			return color;
		}
		
		private int GetRegionNum(string regionName)
		{
			int num = 0;
			switch (regionName)
			{
				case "Курганская область":
					{
						num = 1;
						break;
					}
				case "Свердловская область":
					{
						num = 2;
						break;
					}
				case "Тюменская область":
					{
						num = 3;
						break;
					}
				case "Челябинская область":
					{
						num = 4;
						break;
					}
				case "Ханты-Мансийский автономный округ":
					{
						num = 5;
						break;
					}
				case "Ямало-Ненецкий автономный округ":
					{
						num = 6;
						break;
					}
			}
			return num;
		}

		private static string Browser
		{
			get { return HttpContext.Current.Request.Browser.Browser; }
		}

		#endregion

	}
}
