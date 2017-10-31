using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.SFEP_0001_0002
{
	/// <summary>
	/// Соотношение объемов отгруженных товаров собственного производства, выполненных работ и услуг и налогов
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
		private const string PageTitleCaption = "Соотношение объемов отгруженных товаров собственного производства, выполненных работ и услуг и налогов";
		private const string PageSubTitleCaption = "Анализ динамики объемов отгруженных товаров собственного производства, выполненных работ и услуг и налогов (налог на прибыль, НДФЛ, налог на имущество организаций)  в разрезе ОКВЭД в субъектах Российской Федерации, входящих в Уральский федеральный округ";
		private const string Chart1TitleCaption = "Динамика и соотношение объема отгруженных товаров, выполненных работ (услуг) и налога на прибыль по «{0}»  за {1} год, {2}, миллион рублей";
		private const string Chart3TitleCaption = "Соотношение объема отгруженных товаров, выполненных работ (услуг) и налогов по «{0}» за {1} год";

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

			UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth / 2 - 10);
			UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6);
			LabelChart1.Width = UltraChart1.Width;
			LabelChart1.Height = 75;

			UltraChart1.ChartType = ChartType.ColumnChart;
			UltraChart1.Border.Thickness = 0;

			UltraChart1.ColumnChart.SeriesSpacing = 1;

			UltraChart1.Data.ZeroAligned = true;

			UltraChart1.Axis.X.Extent = 60;
			UltraChart1.Axis.X.Labels.Visible = false;
			UltraChart1.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
			UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
			UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
			UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
			UltraChart1.Axis.Y.Extent = 60;
			UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
			UltraChart1.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 8);

			UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;

			UltraChart1.Legend.Visible = true;
			UltraChart1.Legend.Font = new Font("Verdana", 10);
			UltraChart1.Legend.Location = LegendLocation.Bottom;
			UltraChart1.Legend.SpanPercentage = 15;
			UltraChart1.Border.Thickness = 0;

			UltraChart1.Tooltips.FormatString = "<b><DATA_VALUE:N2></b> миллион рублей";
			UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
			UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
			
			#endregion

			#region Настройка диаграммы 2

			UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
			UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.75);
			LabelChart2.Width = UltraChart2.Width;

			UltraChart2.ChartType = ChartType.LineChart;
			UltraChart2.Border.Thickness = 0;

			UltraChart2.Data.ZeroAligned = true;
			
			UltraChart2.Axis.X.Extent = 60;
			UltraChart2.Axis.X.Labels.Visible = true;
			UltraChart2.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
			UltraChart2.Axis.X.Labels.SeriesLabels.Visible = false;
			UltraChart2.Axis.X.Labels.Font = new Font("Verdana", 8);
			UltraChart2.Axis.Y.Extent = 60;
			UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
			UltraChart2.Axis.Y.Labels.Font = new Font("Verdana", 8);

			UltraChart2.Legend.Visible = true;
			UltraChart2.Legend.Font = new Font("Verdana", 10);
			UltraChart2.Legend.Location = LegendLocation.Bottom;
			UltraChart2.Legend.SpanPercentage = 10;
			UltraChart2.Border.Thickness = 0;

			UltraChart2.ColorModel.ModelStyle = ColorModels.PureRandom;

			UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			//UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
			UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);

			#endregion

			#region Настройка диаграммы 3

			UltraChart3.Width = UltraChart1.Width;
			UltraChart3.Height = UltraChart1.Height;
			LabelChart3.Width = UltraChart3.Width;
			LabelChart3.Height = LabelChart1.Height;

			UltraChart3.ChartType = ChartType.ScatterChart;
			UltraChart3.ScatterChart.Icon = SymbolIcon.Square;
			UltraChart3.ScatterChart.IconSize = SymbolIconSize.Medium;

			UltraChart3.BorderWidth = 0;
			
			UltraChart3.Axis.X.Extent = 60;
			UltraChart3.Axis.Y.Extent = 60;

			UltraChart3.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
			UltraChart3.Axis.Y.Labels.Font = new Font("Verdana", 8);
			UltraChart3.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";
			UltraChart3.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
			UltraChart3.Axis.X.Labels.Font = new Font("Verdana", 8);

			UltraChart3.TitleLeft.Visible = true;
			UltraChart3.TitleLeft.HorizontalAlign = StringAlignment.Far;
			UltraChart3.TitleLeft.Text = "Объем отгруженных товаров собственного производства, выполненных работ и услуг, млн. руб.";

			UltraChart3.TitleRight.Visible = true;
			UltraChart3.TitleRight.Text = "Объем налогов, млн. руб.";
			UltraChart3.TitleRight.Margins.Bottom = 60;
			UltraChart3.TitleRight.HorizontalAlign = StringAlignment.Far;

			UltraChart3.Legend.Visible = true;
			UltraChart3.Legend.Font = new Font("Verdana", 10);
			UltraChart3.Legend.Location = LegendLocation.Bottom;
			UltraChart3.Legend.SpanPercentage = 15;
			UltraChart3.Border.Thickness = 0;
			
			UltraChart3.ColorModel.ModelStyle = ColorModels.CustomSkin;

			AddLineAppearencesUltraChart(UltraChart3);
			
			UltraChart3.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			UltraChart3.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart3_FillSceneGraph);
			UltraChart3.DataBinding += new EventHandler(UltraChart3_DataBinding);
			
			#endregion

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

			#region Экспорт

			ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
			ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

			#endregion
		}

		// --------------------------------------------------------------------

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);
			if (!Page.IsPostBack)
			{
				FillComboDate("SFEP_0001_0002_list_of_dates");
				FillComboRegion("SFEP_0001_0002_list_of_regions");
				FillComboActions();
			}
			#region Анализ параметров
			selectedYear.Value = ComboDate.SelectedValue;
			selectedAction.Value = ComboActions.SelectedValue.Split('.')[1].Trim();
			selectedRegion.Value = ComboRegion.SelectedValue;
			PageTitle.Text = PageTitleCaption;
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = PageSubTitleCaption;
			#endregion

			UltraChart3.Tooltips.FormatString = "<SERIES_LABEL>\nОбъем налогов,\n<b><ITEM_LABEL:N2></b> миллион рублей\nОбъем отгруженных товаров собственного производства, выполненных работ и услуг,\n<b><DATA_VALUE:N2></b> миллион рублей";

			UltraChart1.DataBind();
			UltraChart2.DataBind();
			UltraChart3.DataBind();
			UltraWebGrid.DataBind();
		}

		// --------------------------------------------------------------------

		#region Обработчики грида

		private void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("SFEP_0001_0002_grid");
			dtGrid = new DataTable();
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);
			if (dtGrid != null)
			{
				if (selectedAction.Value == "Строительство")
				{
					dtGrid.Rows.RemoveAt(9);
					dtGrid.Rows.RemoveAt(8);
				}
				else
				{
					dtGrid.Rows.RemoveAt(7);
					dtGrid.Rows.RemoveAt(6);
				}
			}
			UltraWebGrid.DataSource = dtGrid == null ? null : dtGrid.DefaultView;
		}

		private void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
		{
			e.Layout.AllowSortingDefault = AllowSorting.No;
			e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
			e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

			e.Layout.Bands[0].Columns[0].MergeCells = true;
			e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
			GridHeaderCell header = null;
			headerLayout.AddCell("Показатель");
			header = headerLayout.AddCell(selectedYear.Value);
			for (int i = 1; i < dtGrid.Columns.Count; ++i)
			{
				e.Layout.Bands[0].Columns[i].Width = 85;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
				e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
				header.AddCell(dtGrid.Columns[i].Caption);
			}
			headerLayout.ApplyHeaderInfo();
		}

		private void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
		{
			e.Row.Cells[0].Value = e.Row.Cells[0].GetText().Split(';')[0];
			for (int i = 1; i < e.Row.Cells.Count; ++i)
			{
				UltraGridCell cell = e.Row.Cells[i];
				cell.Value = e.Row.Index % 2 == 0 ? String.Format("{0:N2}", cell.Value) : String.Format("{0:P2}", cell.Value);
				if (e.Row.Index % 2 == 1)
				{
					cell.Title = "Темп роста";
					double value;
					if (Double.TryParse(cell.Text.Replace("%", String.Empty), out value) && value != 1)
					{
						if (value > 100)
						{
							cell.Style.CssClass = "ArrowUpGreen";
						}
						else
						{
							cell.Style.CssClass = "ArrowDownRed";
						}
					}
				}
			}
		}

		#endregion

		// --------------------------------------------------------------------

		#region Обработчики диаграммы 1

		private void UltraChart1_DataBinding(object sender, EventArgs e)
		{
			LabelChart1.Text = String.Format(Chart1TitleCaption, selectedAction.Value, selectedYear.Value, selectedRegion.Value);
			string query = DataProvider.GetQueryText("SFEP_0001_0002_chart1");
			dtChart1 = new DataTable();
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Розничная цена", dtChart1);
			UltraChart1.DataSource = (dtChart1 == null) ? null : dtChart1.DefaultView;
		}

		private void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];
				if (primitive is Text)
				{
					Text text = primitive as Text;
					if (text.GetTextString().Contains("Объём работ, выполненных по виду деятельности"))
					{
						text.SetTextString("Объём работ, выполненных по виду деятельности");
					}
				}
			}
		}

		#endregion

		#region Обработчики диаграммы 2

		private void UltraChart2_DataBinding(object sender, EventArgs e)
		{
			string query = String.Empty;
			if (selectedAction.Value == "Строительство")
			{
				query = DataProvider.GetQueryText("SFEP_0001_0002_chart2_building");
				LabelChart2.Text = String.Format("Динамика ввода жилых домов за {0} год, {1}, кв. м.", selectedYear.Value, selectedRegion.Value);
				UltraChart2.Tooltips.FormatString = "<b><DATA_VALUE:N0></b> кв. м.";
			}
			else
			{
				query = DataProvider.GetQueryText("SFEP_0001_0002_chart2_production");
				LabelChart2.Text = String.Format("Динамика индекса промышленного производства по «{0}» за {1} год, {2}, процент", selectedAction.Value, selectedYear.Value, selectedRegion.Value);
				UltraChart2.Tooltips.FormatString = "<b><DATA_VALUE:N0>%</b>";
			}
			dtChart2 = new DataTable();
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart2);
			UltraChart2.DataSource = (dtChart2 == null) ? null : dtChart2.DefaultView;
		}

		#endregion

		#region Обработчики диаграммы 3

		private void UltraChart3_DataBinding(object sender, EventArgs e)
		{
			LabelChart3.Text = String.Format(Chart3TitleCaption, selectedAction.Value, selectedYear.Value);
			dtChart3 = new DataTable();
			string query = DataProvider.GetQueryText("SFEP_0001_0002_chart3");
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtChart3);
			for (int i = 0; i < dtChart3.Columns.Count; i += 2)
			{
				XYSeries series = CRHelper.GetXYSeries(i, i + 1, dtChart3);
				series.Label = dtChart3.Columns[i].Caption.Split(';')[0];
				UltraChart3.Series.Add(series);
			}
		}

		private void UltraChart3_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			int j = 1;
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];
				if (primitive is PointSet)
				{
					PointSet tooltip = (PointSet)primitive;
					if (tooltip.points[0].DataPoint != null)
					{
						if (tooltip.points[0].Value is Array)
						{
							object[] x = (object[])tooltip.points[0].Value;
							tooltip.points[0].DataPoint.Label = String.Format("{0:N2}", x[0]);
						}
					}
				}
				if (primitive is Text)
				{
					Text text = (Text)primitive;
					text.SetTextString(RegionsNamingHelper.ShortName(text.GetTextString()));
				}
				if (primitive is Symbol && !String.IsNullOrEmpty(primitive.Path) && primitive.Path == "Legend")
				{
					Symbol symbol = primitive as Symbol;
					symbol.iconSize = SymbolIconSize.Medium;
					Color color = GetColor(j);
					j++;
					primitive.PE.Fill = color;
					primitive.PE.Stroke = Color.Black;
					primitive.PE.FillGradientStyle = GradientStyle.None;
					primitive.PE.StrokeWidth = 1;
				}
			}
		}

		#endregion

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
				pe.StrokeWidth = 0;
				chart.ColorModel.Skin.PEs.Add(pe);
				pe.Stroke = Color.Black;
				pe.StrokeWidth = 0;
				LineAppearance lineAppearance2 = new LineAppearance();

				lineAppearance2.IconAppearance.PE = pe;

				chart.LineChart.LineAppearances.Add(lineAppearance2);
			}
		}

		// --------------------------------------------------------------------

		// Экспорт-------------------------------------------------------------

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

			ReportExcelExporter1.HeaderCellHeight = 50;
			ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
			ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
			ReportExcelExporter1.TitleStartRow = 0;

			ReportExcelExporter1.Export(headerLayout, sheet1, 3);
			sheet1.Rows[1].Height = 550;

			ReportExcelExporter1.WorksheetTitle = String.Empty;
			ReportExcelExporter1.WorksheetSubTitle = String.Empty;

			ReportExcelExporter1.Export(UltraChart1, LabelChart1.Text, sheet2, 1);
			sheet2.Rows[0].Height = 550;
			sheet2.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
			//sheet2.MergedCellsRegions.Clear();
			ReportExcelExporter1.Export(UltraChart3, LabelChart3.Text, sheet3, 1);
			sheet3.Rows[0].Height = 550;
			sheet3.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
			//sheet3.MergedCellsRegions.Clear();
			UltraChart2.Width = (int)(UltraChart2.Width.Value * 0.7);
			UltraChart2.Height = (int)(UltraChart2.Height.Value * 0.7);
			ReportExcelExporter1.Export(UltraChart2, LabelChart2.Text, sheet4, 1);
			//sheet4.MergedCellsRegions.Clear();
		}

		private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
		{
			e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
			e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
		}

		private static void SetExportGridParams(UltraWebGrid grid)
		{
			string exportFontName = "Verdana";
			int fontSize = 8;
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
			AddPairToDictionary(dictActions, "Раздел С. Добыча полезных ископаемых", 0);
			AddPairToDictionary(dictActions, "Раздел D. Обрабатывающие производства", 0);
			AddPairToDictionary(dictActions, "Раздел E. Производство и распределение электроэнергии, газа и воды", 0);
			AddPairToDictionary(dictActions, "Раздел F. Строительство", 0);
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
			Color color = Color.White;
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
