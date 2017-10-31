using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
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
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebNavigator;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

/**
 *  Анализ розничных цен на социально значимые продовольственные товары по состоянию на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.MD_0001_0001
{
	public partial class Default : CustomReportPage
	{

		#region Поля

		private static DataTable dtGrid;
		private static DataTable dtChart1;
		private static DataTable dtChart2;
		private static GridHeaderLayout headerLayout;

		#endregion

		#region Параметры запроса

		private CustomParam selectedRegion;
		private CustomParam selectedDrug;
		private CustomParam compareDate;
		private CustomParam selectedDate;
		private CustomParam selectedProducer;
		private CustomParam selectedKey;

		#endregion

		// --------------------------------------------------------------------

		// заголовок страницы
		private const string PageTitleCaption = "Анализ средних цен на лекарственные средства";
		private const string PageSubTitleCaption = "Ежемесячный мониторинг средних цен на лекарственные средства в субъекте РФ, {0}, по состоянию на {1}.";
		// заголовок для UltraChart
		private const string Chart1TitleCaption = "Динамика цен на лекарственное средство «{0}», {1}, в <i>амбулаторном сегменте</i>, рубль";
		private const string Chart2TitleCaption = "Динамика оптовых цен на лекарственное средство «{0}», {1}, в <i>госпитальном сегменте</i>, рубль";

		private static Dictionary<string, string> dictRegions = new Dictionary<string, string>();
		private static Dictionary<string, string> dictDates = new Dictionary<string, string>();

		// --------------------------------------------------------------------

		private static bool IsSmallResolution
		{
			get { return CRHelper.GetScreenWidth < 900; }
		}

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			if (!IsSmallResolution)
			{
				ComboDate.Width = 300;
				ComboRegion.Width = 400;
			}
			else
			{
				ComboDate.Width = 200;
				ComboRegion.Width = 250;
			}
			ComboDate.Title = "Период";
			ComboDate.ParentSelect = true;
			ComboRegion.Title = "Субъект РФ";
			ComboRegion.ParentSelect = false;

			GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

			#region Грид

			if (!IsSmallResolution)
				UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
			else
				UltraWebGrid.Width = CRHelper.GetGridWidth(750);
			UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.4 + 10);
			UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
			UltraWebGrid.DisplayLayout.SelectTypeRowDefault = SelectType.Single;
			UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);
			UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
			UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
			UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);

			#endregion

			#region Настройка диаграммы 1

			if (!IsSmallResolution)
				UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
			else
				UltraChart1.Width = CRHelper.GetChartWidth(750);
			UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.35);
			LabelChart1.Width = UltraChart1.Width;

			UltraChart1.ChartType = ChartType.LineChart;
			UltraChart1.Border.Thickness = 0;

			#region Настройка цветов для диаграммы

			UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
			UltraChart1.ColorModel.Skin.ApplyRowWise = true;
			UltraChart1.ColorModel.Skin.PEs.Clear();

			for (int i = 0; i < 3; ++i)
			{
				PaintElement pe = new PaintElement();
				switch (i)
				{
					case 0:
						{
							pe.Fill = Color.DeepSkyBlue;
							break;
						}
					case 1:
						{
							pe.Fill = Color.Crimson;
							break;
						}
					case 2:
						{
							pe.Fill = Color.SpringGreen;
							break;
						}
				}
				pe.StrokeWidth = 2;
				UltraChart1.ColorModel.Skin.PEs.Add(pe);

				LineAppearance lineAppearance = new LineAppearance();

				lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
				lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
				lineAppearance.IconAppearance.PE = pe;

				UltraChart1.LineChart.LineAppearances.Add(lineAppearance);
			}

			#endregion

			UltraChart1.Axis.X.Extent = 50;
			UltraChart1.Axis.X.Labels.Visible = true;

			UltraChart1.Axis.Y.Extent = 50;
			UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";

			UltraChart1.LineChart.NullHandling = NullHandling.InterpolateSimple;

			UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>\nна <SERIES_LABEL>\n<b><DATA_VALUE:N2></b>, рубль";
			UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
			UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
			UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

			UltraChart1.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart1.Axis.X.Margin.Near.Value = 20;
			UltraChart1.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
			UltraChart1.Axis.X.Margin.Far.Value = 20;
			UltraChart1.Axis.Y.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart1.Axis.Y.Margin.Near.Value = 20;
			UltraChart1.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
			UltraChart1.Axis.Y.Margin.Far.Value = 20;

			UltraChart1.Legend.Location = LegendLocation.Bottom;
			UltraChart1.Legend.SpanPercentage = 15;
			UltraChart1.Legend.Font = new Font("Microsoft Sans Serif", 9);
			UltraChart1.Legend.Visible = true;

			#endregion

			#region Настройка диаграммы 2

			if (!IsSmallResolution)
				UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
			else
				UltraChart2.Width = CRHelper.GetChartWidth(750);
			UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.35);
			LabelChart2.Width = UltraChart2.Width;

			UltraChart2.ChartType = ChartType.LineChart;
			UltraChart2.Border.Thickness = 0;

			#region Настройка цветов для диаграммы

			UltraChart2.ColorModel.ModelStyle = ColorModels.CustomSkin;
			UltraChart2.ColorModel.Skin.ApplyRowWise = true;
			UltraChart2.ColorModel.Skin.PEs.Clear();

			for (int i = 0; i < 1; ++i)
			{
				PaintElement pe = new PaintElement();
				switch (i)
				{
					case 0:
						{
							pe.Fill = Color.DarkViolet;
							break;
						}
				}
				pe.StrokeWidth = 2;
				UltraChart2.ColorModel.Skin.PEs.Add(pe);

				LineAppearance lineAppearance = new LineAppearance();

				lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
				lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
				lineAppearance.IconAppearance.PE = pe;

				UltraChart2.LineChart.LineAppearances.Add(lineAppearance);
			}

			#endregion

			UltraChart2.Axis.X.Extent = 50;
			UltraChart2.Axis.Y.Extent = 50;
			UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";

			UltraChart2.LineChart.NullHandling = NullHandling.InterpolateSimple;

			UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>\nна <SERIES_LABEL>\n<b><DATA_VALUE:N2></b>, рубль";
			UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);
			UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);

			UltraChart2.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart2.Axis.X.Margin.Near.Value = 20;
			UltraChart2.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
			UltraChart2.Axis.X.Margin.Far.Value = 20;
			UltraChart2.Axis.Y.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart2.Axis.Y.Margin.Near.Value = 20;
			UltraChart2.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
			UltraChart2.Axis.Y.Margin.Far.Value = 20;

			#endregion

			#region Параметры

			selectedDate = UserParams.CustomParam("selected_date");
			selectedRegion = UserParams.CustomParam("selected_region");
			selectedDrug = UserParams.CustomParam("selected_drug");
			selectedProducer = UserParams.CustomParam("selected_producer");
			compareDate = UserParams.CustomParam("compare_date");
			selectedKey = UserParams.CustomParam("selected_key");

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
				FillComboDate("MD_0001_0001_list_of_dates");
				FillComboRegion("MD_0001_0001_list_of_regions");

				PanelChart1.AddLinkedRequestTrigger(UltraWebGrid);
				PanelChart1.AddRefreshTarget(UltraChart1);
				PanelChart1.AddRefreshTarget(UltraChart2);
			}

			#region Анализ параметров

			string mdxRegionName;
			dictRegions.TryGetValue(ComboRegion.SelectedValue, out mdxRegionName);
			selectedRegion.Value = mdxRegionName;

			Node node = new Node();
			if (ComboDate.SelectedNode.Level == 0)
			{
				node = ComboDate.GetLastChild(ComboDate.SelectedNode);
			}
			else if (ComboDate.SelectedNode.Level == 1)
			{
				node = ComboDate.SelectedNode;
			}
			Node prevNode = new Node();
			if (node.PrevNode != null)
				prevNode = node.PrevNode;
			else if (node.Parent.PrevNode != null)
				prevNode = ComboDate.GetLastChild(node.Parent.PrevNode);
			else
				prevNode = null;

			string mdxDate;
			dictDates.TryGetValue(node.Text, out mdxDate);
			selectedDate.Value = mdxDate;
			if (prevNode == null)
				compareDate.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[1998].[Полугодие 1].[Квартал 1].[Январь]";
			else
			{
				dictDates.TryGetValue(prevNode.Text, out mdxDate);
				compareDate.Value = mdxDate;
			}
			#endregion

			PageTitle.Text = PageTitleCaption;
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = String.Format(PageSubTitleCaption, ComboRegion.SelectedValue, node.Text.ToLower());

			if (!PanelChart1.IsAsyncPostBack)
			{
				headerLayout = new GridHeaderLayout(UltraWebGrid);
				UltraWebGrid.Bands.Clear();
				UltraWebGrid.DataBind();
				UltraWebGrid_FillSceneGraph();
				string patternValue = selectedKey.Value;
				if (UltraWebGrid.Rows.Count > 0)
				{
					UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, UltraWebGrid.Columns.Count - 1, 0);
					UltraWebGrid_ChangeRow(row);
				}
			}
		}

		// --------------------------------------------------------------------

		#region Обработчики грида

		void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
		{
			UltraWebGrid_ChangeRow(e.Row);
		}

		protected void UltraWebGrid_ChangeRow(UltraGridRow row)
		{
			if (row == null)
				return;
			selectedKey.Value = row.Cells[row.Cells.Count - 1].GetText();
			selectedDrug.Value = row.Cells[row.Cells.Count - 2].GetText();
			selectedProducer.Value = row.Cells[1].GetText();
			UltraChart1.DataBind();
			UltraChart2.DataBind();
		}

		protected Dictionary<string, string> UltraWebGrid_GetHospList(DataTable dtHosp)
		{
			Dictionary<string, string> dict = new Dictionary<string, string>();
			for (int i = 0; i < dtHosp.Rows.Count; i += 3)
			{
				string key = String.Empty, value = String.Empty;
				DataRow row = dtHosp.Rows[i];
				DataRow row1 = dtHosp.Rows[i + 1];
				DataRow row2 = dtHosp.Rows[i + 2];
				key = row["ID для поиска"].ToString();
				value = row["Оптовая цена, ГС"] + ";" + row1["Оптовая цена, ГС"] + ";" + row2["Оптовая цена, ГС"];
				AddPairToDictionary(dict, key, value);
			}
			return dict;
		}

		protected void UltraWebGrid_FillHostCost(DataTable dtGrid, Dictionary<string, string> dict)
		{
			for (int i = 0; i < dtGrid.Rows.Count; i += 3)
			{
				string key = String.Empty, value = String.Empty;
				DataRow row = dtGrid.Rows[i];
				DataRow row1 = dtGrid.Rows[i + 1];
				DataRow row2 = dtGrid.Rows[i + 2];
				key = row["ID для поиска"].ToString();
				if (dict.TryGetValue(key, out value))
				{
					string[] data = value.Split(';');
					double cost;
					if (Double.TryParse(data[0], out cost))
						row["Оптовая цена, ГС"] = cost;
					if (Double.TryParse(data[1], out cost))
						row1["Оптовая цена, ГС"] = cost;
					if (Double.TryParse(data[2], out cost))
						row2["Оптовая цена, ГС"] = cost;
				}
			}
		}

		protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("MD_0001_0001_grid_h");
			DataTable dtGridH = new DataTable();
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование лекарства", dtGridH);
			dtGridH.Columns.Remove("Наименование лекарства");

			query = DataProvider.GetQueryText("MD_0001_0001_grid");
			dtGrid = new DataTable();
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование лекарства", dtGrid);
			dtGrid.Columns.Remove("Наименование лекарства");
			if (dtGrid.Rows.Count > 0)
			{
				Dictionary<string, string> dictHosp = UltraWebGrid_GetHospList(dtGridH);
				UltraWebGrid_FillHostCost(dtGrid, dictHosp);
				//dtGrid.Columns.Add("ID для поиска", typeof(string));

				UltraWebGrid.DataSource = dtGrid;
			}
			else
			{
				if (dtGridH.Rows.Count > 0)
				{
					foreach (DataRow hRow in dtGridH.Rows)
					{
						DataRow newRow = dtGrid.NewRow();
						foreach (DataColumn column in dtGridH.Columns)
						{
							newRow[column.ColumnName] = hRow[column.ColumnName];
						}
						dtGrid.Rows.Add(newRow);
					}
					UltraWebGrid.DataSource = dtGrid;
				}
				else
					UltraWebGrid.DataSource = null;
			}
		}

		protected void UltraWebGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
		{
			UltraGridBand band = e.Layout.Bands[0];
			e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
			e.Layout.AllowSortingDefault = AllowSorting.No;
			e.Layout.NullTextDefault = "-";
			e.Layout.RowSelectorStyleDefault.Width = 20;
			band.Columns[0].CellStyle.Wrap = true;
			band.Columns[1].CellStyle.Wrap = true;
			band.Columns[2].CellStyle.Wrap = true;
			double columnWidth = CRHelper.GetColumnWidth(120);
			band.Columns[0].Width = CRHelper.GetColumnWidth(225);
			band.Columns[1].Width = CRHelper.GetColumnWidth(150);
			band.Columns[2].Width = CRHelper.GetColumnWidth(150);
			for (int i = 2; i < band.Columns.Count; ++i)
			{
				band.Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
				band.Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
				band.Columns[i].CellStyle.Padding.Right = 5;
				band.Columns[i].CellStyle.Padding.Left = 5;
			}
			band.Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
			
			// Заголовки
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
			headerLayout.AddCell("Наименование лекарства");
			headerLayout.AddCell("Производитель (Страна)");
			GridHeaderCell headerCell = headerLayout.AddCell("Амбулаторный сегмент");
			headerCell.AddCell("Цена производителя, рубль");
			headerCell.AddCell("Оптовая цена, рубль");
			headerCell.AddCell("Розничная цена, рубль");
			headerCell.AddCell("Оптовая надбавка, %");
			headerCell.AddCell("Розничная надбавка, %");
			headerCell = headerLayout.AddCell("Госпитальный сегмент");
			headerCell.AddCell("Оптовая цена, рубль");
			headerLayout.AddCell("MDX имя лекарства");
			headerLayout.AddCell("ID для поиска");
			headerLayout.ApplyHeaderInfo();

			band.Columns[band.Columns.Count - 1].Hidden = true;
			band.Columns[band.Columns.Count - 2].Hidden = true;
		}

		protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
		{
			string cellFormat = e.Row.Index % 3 == 2 ? "{0:P2}" : "{0:N2}";
			for (int i = 2; i < e.Row.Cells.Count - 2; ++i)
			{
				UltraWebGrid_SetCellHint(sender as UltraWebGrid, e.Row.Cells[i]);
				double value;
				if (Double.TryParse(e.Row.Cells[i].GetText(), out value))
				{
					e.Row.Cells[i].Text = String.Format(cellFormat, Convert.ToDouble(e.Row.Cells[i].GetText()));
				}
			}
		}

		protected void UltraWebGrid_FillSceneGraph()
		{
			for (int i = 0; i < UltraWebGrid.Rows.Count; i += 3)
			{
				UltraWebGrid.Rows[i].Cells[0].RowSpan = 3;
				UltraWebGrid.Rows[i].Cells[1].RowSpan = 3;

				for (int j = 2; j < UltraWebGrid.Columns.Count - 2; ++j)
				{
					double value;
					if (Double.TryParse(UltraWebGrid.Rows[i + 1].Cells[j].Text, out value))
					{
						if (value > 0)
						{
							UltraWebGrid.Rows[i + 2].Cells[j].Style.CssClass = "ArrowUpRed";
						}
						else if (value < 0)
						{
							UltraWebGrid.Rows[i + 2].Cells[j].Style.CssClass = "ArrowDownGreen";
						}
					}
				}
			}
		}

		protected void UltraWebGrid_SetCellHint(UltraWebGrid grid, UltraGridCell cell)
		{
			string prevDateMonth = GetLastBlock(compareDate.Value);
			string prevDateYear = GetLastBlock(compareDate.Value, 4);
			string prevDate = CRHelper.RusMonthDat(CRHelper.MonthNum(prevDateMonth)) + " " + prevDateYear;
			if (cell.Column.Index > 1)
			{
				if (cell.Row.Index % 3 == 1)
				{
					cell.Title = String.Format("Прирост к {0} г.", prevDate);
				}
				else if (cell.Row.Index % 3 == 2)
				{
					cell.Title = String.Format("Темп прироста к {0} г.", prevDate);
				}
			}
		}

		#endregion

		// --------------------------------------------------------------------

		#region Обработчики диаграммы 1

		protected void UltraChart1_DataBinding(object sender, EventArgs e)
		{
			string query;
			LabelChart1.Text = String.Format(Chart1TitleCaption, GetLastBlock(selectedDrug.Value), GetLastBlock(selectedProducer.Value));
			query = DataProvider.GetQueryText("MD_0001_0001_chart1_ambulance");
			dtChart1 = new DataTable();
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Территория", dtChart1);
			if (dtChart1 == null || dtChart1.Rows.Count == 0)
			{
				UltraChart1.DataSource = null;
				return;
			}

			dtChart1.Columns.RemoveAt(0);

			foreach (DataColumn column in dtChart1.Columns)
				column.ColumnName = column.ColumnName.Replace("Выбранное лекарство;", String.Empty);
			
			UltraChart1.Data.SwapRowsAndColumns = true;
			UltraChart1.DataSource = dtChart1.DefaultView;
		}

		protected void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			for (int i = 0; i < e.SceneGraph.Count; ++i)
			{
				Primitive primitive = e.SceneGraph[i];
				if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
				{
					Text text = primitive as Text;
					string[] textElements = text.GetTextString().Split(' ');
					if (textElements.Length == 3)
					{
						text.SetTextString(textElements[0].Substring(0, 3) + ". " + textElements[1].Substring(2, 2));
					}
				}
			}
		}

		#endregion

		#region Обработчики диаграммы 2

		protected void UltraChart2_DataBinding(object sender, EventArgs e)
		{
			string query;
			LabelChart2.Text = String.Format(Chart2TitleCaption, GetLastBlock(selectedDrug.Value), GetLastBlock(selectedProducer.Value));
			query = DataProvider.GetQueryText("MD_0001_0001_chart1_hospital");
			dtChart2 = new DataTable();
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Территория", dtChart2);
			if (dtChart2 == null || dtChart2.Rows.Count == 0)
			{
				UltraChart2.DataSource = null;
				return;
			}

			dtChart2.Columns.RemoveAt(0);

			foreach (DataColumn column in dtChart2.Columns)
				column.ColumnName = column.ColumnName.Replace("Выбранное лекарство;", String.Empty);

			UltraChart2.Data.SwapRowsAndColumns = true;
			UltraChart2.DataSource = dtChart2.DefaultView;
		}

		protected void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			for (int i = 0; i < e.SceneGraph.Count; ++i)
			{
				Primitive primitive = e.SceneGraph[i];
				if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
				{
					Text text = primitive as Text;
					string[] textElements = text.GetTextString().Split(' ');
					if (textElements.Length == 3)
					{
						text.SetTextString(textElements[0].Substring(0, 3) + ". " + textElements[1].Substring(2, 2));
					}
				}
			}
		}

		#endregion

		// --------------------------------------------------------------------

		#region Заполнение словарей и выпадающих списков параметров

		protected void FillComboRegion(string queryName)
		{
			DataTable dtRegion = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtRegion);
			Dictionary<string, int> dict = new Dictionary<string, int>();
			if (dtRegion == null || dtRegion.Rows.Count == 0)
				throw new Exception("Нет данных для построения отчета!");
			foreach (DataRow row in dtRegion.Rows)
			{
				AddPairToDictionary(dict, row[2].ToString(), 0);
				AddPairToDictionary(dict, row[1].ToString(), 1);
				AddPairToDictionary(dictRegions, row[1].ToString(), row[3].ToString());
			}
			ComboRegion.FillDictionaryValues(dict);
			if (dict.ContainsKey("Ханты-Мансийский АО"))
			{
				ComboRegion.SetСheckedState("Ханты-Мансийский АО", true);
			}
		}

		protected void FillComboDate(string queryName)
		{
			DataTable dtDate = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Месяц", dtDate);
			Dictionary<string, int> dict = new Dictionary<string, int>();
			if (dtDate == null || dtDate.Rows.Count == 0)
				throw new Exception("Нет данных для построения отчета!");
			foreach (DataRow row in dtDate.Rows)
			{
				AddPairToDictionary(dict, row[1].ToString(), 0);
				AddPairToDictionary(dict, row[0].ToString() + " " + row[1].ToString() + " года", 1);
				AddPairToDictionary(dictDates, row[0].ToString() + " " + row[1].ToString() + " года", row[2].ToString());
			}
			ComboDate.FillDictionaryValues(dict);
			ComboDate.SelectLastNode();
		}

		protected void AddPairToDictionary(Dictionary<string, int> dict, string key, int value)
		{
			if (!dict.ContainsKey(key))
			{
				dict.Add(key, value);
			}
		}

		protected void AddPairToDictionary(Dictionary<string, string> dict, string key, string value)
		{
			if (!dict.ContainsKey(key))
			{
				dict.Add(key, value);
			}
		}

		#endregion

		#region Функции-полезняшки преобразования и все такое

		private static string Browser
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
			string template = "[Период].[Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]";
			string[] dateElements = str.Split(' ');
			int year = Convert.ToInt32(dateElements[2]);
			string month = CRHelper.RusMonth(CRHelper.MonthNum(dateElements[1]));
			int quarter = CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month));
			int halfYear = CRHelper.HalfYearNumByQuarterNum(quarter);
			int day = Convert.ToInt32(dateElements[0]);
			return String.Format(template, year, halfYear, quarter, month, day);
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

		public string GetLastBlock(string mdxString)
		{
			if (String.IsNullOrEmpty(mdxString))
			{
				return String.Empty;
			}
			string[] separator = { "].[" };
			string[] stringElements = mdxString.Split(separator, StringSplitOptions.None);
			return stringElements[stringElements.Length - 1].Replace("]", String.Empty);
		}

		public string GetLastBlock(string mdxString, int indexFromTail)
		{
			if (String.IsNullOrEmpty(mdxString))
			{
				return String.Empty;
			}
			string[] separator = { "].[" };
			string[] stringElements = mdxString.Split(separator, StringSplitOptions.None);
			return stringElements[stringElements.Length - indexFromTail].Replace("]", String.Empty);
		}

		#endregion

		#region Экспорт в PDF

		private void PdfExportButton_Click(object sender, EventArgs e)
		{
			UltraWebGrid grid = headerLayout.Grid;

			ReportPDFExporter1.PageTitle = PageTitle.Text;
			ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

			Report report = new Report();
			ISection section1 = report.AddSection();
			ISection section2 = report.AddSection();
			ISection section3 = report.AddSection();

			UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
			UltraChart2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

			ReportPDFExporter1.HeaderCellHeight = 70;

			foreach (UltraGridRow row in headerLayout.Grid.Rows)
			{
				if (row.Index % 3 != 0)
				{
					row.Cells[0].Style.BorderDetails.StyleTop = BorderStyle.None;
					row.Cells[1].Style.BorderDetails.StyleTop = BorderStyle.None;
				}
				else
				{
					row.Cells[0].Value = null;
					row.Cells[1].Value = null;
				}
				if (row.Index % 3 != 2)
				{
					row.Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
					row.Cells[1].Style.BorderDetails.StyleBottom = BorderStyle.None;
				}
				else
				{
					row.Cells[0].Value = null;
					row.Cells[1].Value = null;
				}
			}

			headerLayout.childCells.Remove(headerLayout.GetChildCellByCaption("MDX имя лекарства"));
			headerLayout.childCells.Remove(headerLayout.GetChildCellByCaption("ID для поиска"));

			grid.Columns.Remove(grid.Columns.FromKey("MDX имя лекарства"));
			grid.Columns.Remove(grid.Columns.FromKey("ID для поиска"));

			ReportPDFExporter1.Export(headerLayout, section1);
			ReportPDFExporter1.Export(UltraChart1, LabelChart1.Text, section2);
			ReportPDFExporter1.Export(UltraChart2, LabelChart2.Text, section3);
		}

		#endregion

		#region Экспорт в Excel

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			UltraWebGrid grid = headerLayout.Grid;

			ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
			ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

			Workbook workbook = new Workbook();
			Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
			Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма, амбулаторный сегмент");
			Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма, госпитальный сегмент");
			Worksheet sheet4 = workbook.Worksheets.Add("Надо, потом удалю");

			SetExportGridParams(grid);

			ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
			ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
			ReportExcelExporter1.TitleStartRow = 0;

			foreach (UltraGridRow row in grid.Rows)
			{
				if (row.IsActiveRow())
				{
					row.Activated = false;
					row.Selected = false;
				}
			}

			headerLayout.childCells.Remove(headerLayout.GetChildCellByCaption("MDX имя лекарства"));
			headerLayout.childCells.Remove(headerLayout.GetChildCellByCaption("ID для поиска"));

			grid.Columns.Remove(grid.Columns.FromKey("MDX имя лекарства"));
			grid.Columns.Remove(grid.Columns.FromKey("ID для поиска"));

			ReportExcelExporter1.Export(headerLayout, sheet1, 3);

			sheet1.MergedCellsRegions.Clear();
			sheet1.MergedCellsRegions.Add(0, 0, 0, 7);
			sheet1.MergedCellsRegions.Add(1, 0, 1, 7);
			sheet1.Rows[1].Height = 600;
			sheet1.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
			sheet1.Rows[1].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
			sheet1.MergedCellsRegions.Add(3, 2, 3, 6);
			sheet1.MergedCellsRegions.Add(3, 0, 4, 0);
			sheet1.MergedCellsRegions.Add(3, 1, 4, 1);
			for (int i = 5; i < grid.Rows.Count + 5; i += 3)
			{
				sheet1.MergedCellsRegions.Add(i, 0, i + 2, 0);
				sheet1.MergedCellsRegions.Add(i, 1, i + 2, 1);
				sheet1.Rows[i].Height = 255;
				sheet1.Rows[i + 1].Height = 255;
				sheet1.Rows[i + 2].Height = 255;
			}

			sheet1.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			sheet1.Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;

			ReportExcelExporter1.WorksheetTitle = String.Empty;
			ReportExcelExporter1.WorksheetSubTitle = String.Empty;

			UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.7));
			ReportExcelExporter1.Export(UltraChart1, Regex.Replace(LabelChart1.Text, "<[\\s\\S]*?>", String.Empty), sheet2, 1);
			UltraChart2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.7));
			ReportExcelExporter1.Export(UltraChart2, Regex.Replace(LabelChart2.Text, "<[\\s\\S]*?>", String.Empty), sheet3, 1);

			sheet2.MergedCellsRegions.Clear();
			sheet3.MergedCellsRegions.Clear();
			sheet2.MergedCellsRegions.Add(0, 0, 0, 18);
			sheet3.MergedCellsRegions.Add(0, 0, 0, 18);
			sheet2.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
			sheet3.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
			sheet2.Rows[0].Height = 750;
			sheet3.Rows[0].Height = 750;
			GridHeaderLayout emptyGridLayout = new GridHeaderLayout(emptyExportGrid);
			ReportExcelExporter1.Export(emptyGridLayout, sheet4, 0);
			workbook.Worksheets.Remove(sheet4);
		}

		private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
		{
			e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
			e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
			e.CurrentWorksheet.PrintOptions.BottomMargin = 0.25;
			e.CurrentWorksheet.PrintOptions.TopMargin = 0.25;
			e.CurrentWorksheet.PrintOptions.LeftMargin = 0.25;
			e.CurrentWorksheet.PrintOptions.RightMargin = 0.25;
			if (e.CurrentWorksheet.Name != "Таблица")
				e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
		}

		private static void SetExportGridParams(UltraWebGrid grid)
		{
			string exportFontName = "Verdana";
			int fontSize = 10;
			double coeff = 0.9;
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
