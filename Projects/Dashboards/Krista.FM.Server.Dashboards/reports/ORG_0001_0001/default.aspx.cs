using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;

/**
 *  Цены на нефтепродукты по Ханты-Мансийскому автономному округу – Югре
 */
namespace Krista.FM.Server.Dashboards.reports.ORG_0001_0001
{
	public partial class Default : CustomReportPage
	{
		#region Поля

		private static DataTable dtDate;
		private static DataTable dtGrid;
		private static DataTable dtChart1;
		private static DataTable dtChart2;
		private static GridHeaderLayout headerLayout;
		private static int selectedGridRow = 0;

        private static string Istoch =
            "[Источники данных].[Источники данных].[Все источники данных].[СТАТ Отчетность - Департамент экономики]";

		#endregion

		private static bool IsSmallResolution
		{
			get { return CRHelper.GetScreenWidth < 900; }
		}

		private static bool IsMozilla
		{
			get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
		}

		private static string Browser
		{
			get { return HttpContext.Current.Request.Browser.Browser; }
		}

		#region Параметры запроса

        private CustomParam DataIstock;

		private CustomParam selectedDate;
		private CustomParam compareDate;
		private CustomParam date;
		private CustomParam fuel;
		private CustomParam price;

		#endregion

		// --------------------------------------------------------------------

		private const string PageTitleCaption = "Анализ средних розничных и средних закупочных цен по видам нефтепродуктов";
        private const string PageSubTitleCaption = "Ежедекадный мониторинг цен на нефтепродукты на основании данных мониторинга предприятий торговли, Ханты-Мансийский автономный округ - Югра, по состоянию на {0}.";
		private const string Chart1TitleCaption = "Динамика максимальной, минимальной и средней {0} цены на «{1}», рублей за литр";
		private const string Chart2TitleCaption = "Динамика средней закупочной и средней розничной цены на «{0}», рублей за литр";

		// --------------------------------------------------------------------

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			ComboDate.Title = "Выберите дату";
			ComboDate.Width = 275;
			ComboDate.ParentSelect = true;
			ComboCompareDate.Title = "Выберите дату для сравнения";
			ComboCompareDate.Width = 375;
			ComboCompareDate.ParentSelect = true;

			#region Настройка грида

			if (!IsSmallResolution)
			{
				UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
			}
			else
			{
				UltraWebGrid.Width = CRHelper.GetGridWidth(750);
			}
			UltraWebGrid.Height = Unit.Empty;
			UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
			UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
			UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
			UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);
			UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

			#endregion

			#region Настройка диаграммы 1

			if (!IsSmallResolution)
			{
				UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 17);
			}
			else
			{
				UltraChart1.Width = CRHelper.GetChartWidth(750);
			}
			UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);

			UltraChart1.ChartType = ChartType.LineChart;

			UltraChart1.Border.Thickness = 0;

			UltraChart1.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart1.Axis.X.Margin.Near.Value = 10;
			UltraChart1.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
			UltraChart1.Axis.X.Margin.Far.Value = 10;
			UltraChart1.Axis.Y.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart1.Axis.Y.Margin.Near.Value = 10;
			UltraChart1.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
			UltraChart1.Axis.Y.Margin.Far.Value = 10;

			UltraChart1.Axis.X.Extent = 60;
			UltraChart1.Axis.X.Labels.Visible = true;
			UltraChart1.Axis.Y.Extent = 50;
			UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

			UltraChart1.Legend.Location = LegendLocation.Bottom;
			UltraChart1.Legend.SpanPercentage = 10;
			UltraChart1.Legend.Font = new Font("Microsoft Sans Serif", 9);
			UltraChart1.Legend.Visible = true;

			UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
			UltraChart1.ColorModel.Skin.PEs.Clear();

			PaintElement pe;
			pe = new PaintElement(Color.Red);
			pe.StrokeWidth = 0;
			UltraChart1.ColorModel.Skin.PEs.Add(pe);
			pe = new PaintElement(Color.Yellow);
			pe.StrokeWidth = 0;
			UltraChart1.ColorModel.Skin.PEs.Add(pe);
			pe = new PaintElement(Color.Green);
			pe.StrokeWidth = 0;
			UltraChart1.ColorModel.Skin.PEs.Add(pe);

			LineAppearance lineAppearance = new LineAppearance();
			lineAppearance.IconAppearance.Icon = SymbolIcon.Square;
			lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
			pe = new PaintElement(Color.Red);
			lineAppearance.IconAppearance.PE = pe;
			lineAppearance.Thickness = 0;
			UltraChart1.LineChart.LineAppearances.Add(lineAppearance);

			lineAppearance = new LineAppearance();
			lineAppearance.IconAppearance.Icon = SymbolIcon.Square;
			lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
			pe = new PaintElement(Color.Yellow);
			lineAppearance.IconAppearance.PE = pe;
			lineAppearance.Thickness = 0;
			UltraChart1.LineChart.LineAppearances.Add(lineAppearance);

			lineAppearance = new LineAppearance();
			lineAppearance.IconAppearance.Icon = SymbolIcon.Square;
			lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
			pe = new PaintElement(Color.Green);
			lineAppearance.IconAppearance.PE = pe;
			lineAppearance.Thickness = 0;
			UltraChart1.LineChart.LineAppearances.Add(lineAppearance);

			UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
			UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
			UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

			#endregion
                
			#region Настройка диаграммы 2

			if (!IsSmallResolution)
			{
				UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 17);
			}
			else
			{
				UltraChart2.Width = CRHelper.GetChartWidth(750);
			}
			UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);

			UltraChart2.ChartType = ChartType.StackColumnChart;
			UltraChart2.Data.ZeroAligned = true;

			UltraChart2.Border.Thickness = 0;

			UltraChart2.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart2.Axis.X.Margin.Near.Value = 10;
			UltraChart2.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
			UltraChart2.Axis.X.Margin.Far.Value = 10;
			UltraChart2.Axis.Y.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart2.Axis.Y.Margin.Near.Value = 10;
			UltraChart2.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
			UltraChart2.Axis.Y.Margin.Far.Value = 10;

			UltraChart2.Axis.X.Extent = 60;
			UltraChart2.Axis.X.Labels.Visible = false;
			UltraChart2.Axis.X.Labels.SeriesLabels.Visible = true;
			UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
			UltraChart2.Axis.Y.Extent = 50;
			UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

			UltraChart2.Legend.Location = LegendLocation.Bottom;
			UltraChart2.Legend.SpanPercentage = 10;
			UltraChart2.Legend.Font = new Font("Microsoft Sans Serif", 9);
			UltraChart2.Legend.Visible = true;

			UltraChart2.ColorModel.ModelStyle = ColorModels.CustomSkin;

			Color color1 = Color.LimeGreen;
			Color color2 = Color.Firebrick;

			UltraChart2.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
			UltraChart2.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));
			UltraChart2.ColorModel.Skin.ApplyRowWise = false;

			UltraChart2.Effects.Effects.Clear();
			GradientEffect effect = new GradientEffect();
			effect.Style = GradientStyle.ForwardDiagonal;
			effect.Coloring = GradientColoringStyle.Darken;
			effect.Enabled = true;
			UltraChart2.Effects.Enabled = true;
			UltraChart2.Effects.Effects.Add(effect);

			UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);
			UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
			UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

			#endregion

			#region Параметры

			selectedDate = UserParams.CustomParam("selected_date");
			compareDate = UserParams.CustomParam("compare_date");
			date = UserParams.CustomParam("date");
			fuel = UserParams.CustomParam("fuel");
			price = UserParams.CustomParam("price");

            DataIstock = UserParams.CustomParam("DataIstock");

            DataIstock.Value = Istoch;
			#endregion

			#region Экспорт

			ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
			ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            //ReportPDFExporter1.PdfExporter.HeaderCellExported += new EventHandler<MarginCellExportedEventArgs>(PdfExporter_Test);

			#endregion
		}

		// --------------------------------------------------------------------

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);
			if (!Page.IsPostBack)
			{
				FillComboDate(ComboDate, "ORG_0001_0001_list_of_dates", 0);
				FillComboDate(ComboCompareDate, "ORG_0001_0001_list_of_dates", 1);

				ZPrice.Attributes.Add("onclick", string.Format("uncheck('{0}', false)", RPrice.ClientID));
				RPrice.Attributes.Add("onclick", string.Format("uncheck('{0}', false)", ZPrice.ClientID));
			}

			#region Анализ параметров

			switch (ComboDate.SelectedNode.Level)
			{
				case 0:
					{
						selectedDate.Value = StringToMDXDate(ComboDate.GetLastChild(ComboDate.SelectedNode).FirstNode.Text);
						break;
					}
				case 1:
					{
						selectedDate.Value = StringToMDXDate(ComboDate.SelectedNode.FirstNode.Text);
						break;
					}
				case 2:
					{
						selectedDate.Value = StringToMDXDate(ComboDate.SelectedNode.Text);
						break;
					}
			}
			switch (ComboCompareDate.SelectedNode.Level)
			{
				case 0:
					{
						compareDate.Value = StringToMDXDate(ComboCompareDate.GetLastChild(ComboCompareDate.SelectedNode).FirstNode.Text);
						break;
					}
				case 1:
					{
						compareDate.Value = StringToMDXDate(ComboCompareDate.SelectedNode.FirstNode.Text);
						break;
					}
				case 2:
					{
						compareDate.Value = StringToMDXDate(ComboCompareDate.SelectedNode.Text);
						break;
					}
			}
			if (compareDate.Value == GetMaxMDXDate(selectedDate.Value, compareDate.Value))
			{
				string tmpDate = selectedDate.Value;
				selectedDate.Value = compareDate.Value;
				compareDate.Value = tmpDate;
			}

			#endregion

			PageTitle.Text = PageTitleCaption;
			Page.Title = PageTitle.Text;
            PageSubTitle.Text = string.Format(PageSubTitleCaption, ComboDate.SelectedValue);
                ///String.Format(PageSubTitleCaption, MDXDateToShortDateString(selectedDate.Value));
			
			headerLayout = new GridHeaderLayout(UltraWebGrid);
			UltraWebGrid.DataBind();
			UltraWebGrid.Rows[selectedGridRow].Activated = true;
			UltraWebGrid.Rows[selectedGridRow].Selected = true;
			UltraWebGrid_ChangeRow(selectedGridRow);
		}

		// --------------------------------------------------------------------

		#region Обработчики грида

		private void UltraWebGrid_ChangeRow(int rowIndex)
		{
			UltraChart1.DataBind();
			UltraChart2.DataBind();
		}
		
		private void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
		{
			//if (PanelChart1.IsAsyncPostBack)
			{
				selectedGridRow = e.Row.Index;
				UltraWebGrid_ChangeRow(selectedGridRow);
			}
		}

        void SaveS(object[] caption)
        {
            string SaveString = "";
            foreach (object s in caption)
            {
                SaveString += "|" + s.ToString();
            }
            CRHelper.SaveToErrorLog(SaveString);
        }

		protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
			DataTable dtFuelTypes = new DataTable();
			string query = DataProvider.GetQueryText("ORG_0001_0001_fuel_types");
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Вид топлива", dtFuelTypes);
			if (dtFuelTypes.Rows.Count > 0)
			{
				UltraWebGrid.DisplayLayout.Bands.Clear();
				dtGrid = new DataTable();
				for (int i = 0; i < 13; ++i)
				{
					dtGrid.Columns.Add();
					dtGrid.Columns[i].DataType = typeof(string);
				}

				foreach (DataRow row in dtFuelTypes.Rows)
				{
					DataRow newRow1 = dtGrid.NewRow();
					DataRow newRow2 = dtGrid.NewRow();
					DataRow newRow3 = dtGrid.NewRow();
					newRow1[0] = row[0];
					newRow2[0] = " ";
					newRow3[0] = "подробнее...";

					fuel.Value = row[0].ToString();
					price.Value = "Закупочная цена";
					date.Value = compareDate.Value;
					DataTable dtFuelData = new DataTable();
					//object[] results = new object[5];
                    Dictionary<string, object> results = new Dictionary<string, object>();
					query = DataProvider.GetQueryText("ORG_0001_0001_fuel_data");
					DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные по одному виду топлива", dtFuelData);
					ParseFuelDataMath(dtFuelData, results);

                    newRow1[1] = results["AVG"];

					dtFuelData = new DataTable();
					date.Value = selectedDate.Value;
					query = DataProvider.GetQueryText("ORG_0001_0001_fuel_data");
					DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные по одному виду топлива", dtFuelData);
					ParseFuelDataMath(dtFuelData, results);

                    newRow1[2] = results["AVG"];
                    newRow1[3] = results["MinRegionName"];
                    newRow1[4] = results["MaxRegionName"];

                    newRow2[2] = Grow(newRow1[2], newRow1[1]);

                    newRow3[2] = Minus(newRow1[2], newRow1[1]);

                    newRow3[3] = results["MinRegionValue"];
                    newRow3[4] = results["MaxRegionValue"];

					price.Value = "Розничная цена";
					date.Value = compareDate.Value;
					dtFuelData = new DataTable();
					query = DataProvider.GetQueryText("ORG_0001_0001_fuel_data");
					DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные по одному виду топлива", dtFuelData);
					ParseFuelDataMath(dtFuelData, results);

                    newRow1[5] = results["AVG"];

					dtFuelData = new DataTable();
					date.Value = selectedDate.Value;
					query = DataProvider.GetQueryText("ORG_0001_0001_fuel_data");
					DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные по одному виду топлива", dtFuelData);
					ParseFuelDataMath(dtFuelData, results);

                    newRow1[6] = results["AVG"];
                    newRow1[7] = results["MinRegionName"];
                    newRow1[8] = results["MaxRegionName"];

					newRow2[6] = Grow(newRow1[6], newRow1[5]);

					newRow3[6] = Minus(newRow1[6], newRow1[5]); 

                    newRow3[7] = results["MinRegionValue"];
                    newRow3[8] = results["MaxRegionValue"];

					newRow1[9] = Minus(newRow1[5], newRow1[1]);
					newRow1[10] = Minus(newRow1[6], newRow1[2]);
					newRow2[10] = Grow(newRow1[10], newRow1[9]);
					newRow3[10] = Percent(newRow1[10], newRow1[9]);

					newRow1[11] = Grow(newRow1[5], newRow1[1]);
					newRow1[12] = Grow(newRow1[6], newRow1[2]);
					newRow3[12] = Minus(newRow1[12], newRow1[11]);

					dtGrid.Rows.Add(newRow1);
					dtGrid.Rows.Add(newRow2);
					dtGrid.Rows.Add(newRow3);
				}

				UltraWebGrid.DataSource = dtGrid;
			}
			else
			{
				UltraWebGrid.DataSource = null;
			}
		}

        private string SetStarChar(string RegionName)
        {
            string NameRegion = RegionName;

            string[] StarRegions = new string[12] { "Ханты-Мансийский автономный округ", "Советск", "Сургутск", "Когал", "Ланге", "Мегион", "Нефтеюганск", "Нижневартовский-", "Нягань", "Сургут", "Пыть", "Югорск" };
            foreach (string R in StarRegions)
            {
                if (NameRegion.Contains(R))
                {
                    return NameRegion;
                }
            }
            return NameRegion + "*";
        }

		protected void UltraWebGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
		{
			e.Layout.AllowSortingDefault = AllowSorting.No;
			e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
			e.Layout.RowAlternateStylingDefault = Infragistics.WebUI.Shared.DefaultableBoolean.False;
			e.Layout.NullTextDefault = "-";
			
			// Заголовки
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
			headerLayout.AddCell("Вид топлива");
			GridHeaderCell headerCell = headerLayout.AddCell("Закупочная цена, рублей за литр");
			GridHeaderCell headerCell1 = headerCell.AddCell(MDXDateToShortDateString(compareDate.Value));
			headerCell1.AddCell("средняя");
			headerCell = headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value));
			headerCell.AddCell("средняя");
			headerCell.AddCell("минимальная");
			headerCell.AddCell("максимальная");
			headerCell = headerLayout.AddCell("Розничная цена, рублей за литр");
			headerCell1 = headerCell.AddCell(MDXDateToShortDateString(compareDate.Value));
			headerCell1.AddCell("средняя");
			headerCell = headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value));
			headerCell.AddCell("средняя");
			headerCell.AddCell("минимальная");
			headerCell.AddCell("максимальная");
			headerCell = headerLayout.AddCell("Разница между средней розничной ценой и средней закупочной ценой, рублей за литр");
			headerCell.AddCell(MDXDateToShortDateString(compareDate.Value), 2);
			headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value), 2);
            headerCell = headerLayout.AddCell("Торговая надбавка, %");
			headerCell.AddCell(MDXDateToShortDateString(compareDate.Value), 2);
			headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value), 2);

			e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(120);
			e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
			for (int i = 1; i < e.Layout.Bands[0].Columns.Count; ++i)
			{
				int columnWidth = i == 3 || i == 4 || i == 7 || i == 8 ? 110 : 70;
				e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
				e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Left = 5;
			}
			headerLayout.ApplyHeaderInfo();
		}
        void SetImageFromCell(UltraGridCell Cell, string ImageName)
        {
            string ImagePath = "~/images/" + ImageName;
            //;
            Cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
            Cell.Style.BackgroundImage = ImagePath;
        }
		protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
		{
            
			UltraGridRow row = e.Row;
			if (row.Index % 3 == 2)
			{
				row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Right;
				row.Cells[0].Style.ForeColor = Color.Blue;
			}
			row.Cells[1].Style.ForeColor = Color.Gray;
			row.Cells[5].Style.ForeColor = Color.Gray;
			row.Cells[9].Style.ForeColor = Color.Gray;
			row.Cells[11].Style.ForeColor = Color.Gray;
			foreach (UltraGridCell cell in row.Cells)
			{
				SetCellHint(cell);
				// Стрелочки (индикация)
				if (row.Index % 3 == 2 && (cell.Column.Index == 2 || cell.Column.Index == 6 || cell.Column.Index == 10 || cell.Column.Index == 12))
				{
					UltraGridCell upperCell = UltraWebGrid.Rows[row.Index - 2].Cells[cell.Column.Index];
					if (Convert.ToDouble(cell.Value) > 0.0001)
					{	
                        SetImageFromCell(upperCell, "ArrowRedUpBB.png");
					}
					else if (Convert.ToDouble(cell.Value) < -0.0001)
					{
                        SetImageFromCell(upperCell, "ArrowGreenDownBB.png");	
					}
				}
				if (row.Index % 3 == 0)
				{
					cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
					if (cell.Value != null && !(cell.Column.Index == 0 || cell.Column.Index == 3 || cell.Column.Index == 4 || cell.Column.Index == 7 || cell.Column.Index == 8))
					{
                        try
                        {
                            if (cell.Column.Index < 11)
                            {
                                cell.Value = String.Format("{0:N2}", Convert.ToDouble(cell.Value));
                            }
                            else
                            {
                                cell.Value = String.Format("{0:P2}", Convert.ToDouble(cell.Value));
                            }
                        }
                        catch { }
					}
				}
				else if (row.Index % 3 == 1)
				{
					cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
					cell.Style.BorderDetails.StyleTop = BorderStyle.None;
					if (cell.Value != null && cell.Column.Index > 0)
					{
						cell.Value = String.Format("{0:P2}", Convert.ToDouble(cell.Value));
					}
				}
				else if (row.Index % 3 == 2)
				{
					cell.Style.BorderDetails.StyleTop = BorderStyle.None;
					if (cell.Column.Index != 0 && cell.Value != null)
					{
						if (cell.Column.Index < 11)
						{
							cell.Value = String.Format("{0:N2}", Convert.ToDouble(cell.Value));
						}
						else
						{
							cell.Value = String.Format("{0:P2}", Convert.ToDouble(cell.Value));
						}
					}
				}
				if (cell.GetText() == "подробнее...")
				{
					string fuel = UltraWebGrid.Rows[row.Index - 2].Cells[0].GetText();
					cell.Text = String.Format("<a href='../ORG_0001_0002/Default.aspx?paramlist=fuel={0};selectedDate={1};compareDate={2}'>Подробнее...</a>",                                     HttpContext.Current.Server.UrlEncode(fuel), 
                        HttpContext.Current.Server.UrlEncode(
                        
                        PAck(selectedDate.Value)
                        ), 
                        HttpContext.Current.Server.UrlEncode(
                        
                         PAck(compareDate.Value)
                        ));
				}
			}
		}

        string PAck(string s)
        {
            return s.Replace("[Период__День].[Период__День].[Данные всех периодов].[", "-");
        }

		protected void SetCellHint(UltraGridCell cell)
		{
			int row = cell.Row.Index;
			int column = cell.Column.Index;
			if (row % 3 == 1 && (column == 2 || column == 6 || column == 10))
			{
				cell.Title = String.Format("Изменение в % к {0} года", MDXDateToShortDateString(compareDate.Value));
			}
			if (row % 3 == 2 && (column == 2 || column == 6 || column == 10 || column == 12))
			{
                cell.Title = String.Format("Изменение в руб. {0} года", MDXDateToShortDateString(compareDate.Value));
			}
		}

		#endregion

		// --------------------------------------------------------------------

		#region Обработчики диаграммы 1

		protected void UltraChart1_DataBinding(object sender, EventArgs e)
		{
			UltraChart1.Tooltips.FormatString = "<SERIES_LABEL>\n<b><DATA_VALUE:N2></b>, рублей за литр\n<ITEM_LABEL>";
			fuel.Value = UltraWebGrid.Rows[selectedGridRow - selectedGridRow % 3].Cells[0].GetText();
			if (RPrice.Checked)
			{
				price.Value = "Розничная цена";
				LabelChart1.Text = String.Format(Chart1TitleCaption, "розничной", fuel.Value);
			}
			else
			{
				price.Value = "Закупочная цена";
				LabelChart1.Text = String.Format(Chart1TitleCaption, "закупочной", fuel.Value);
			}
			if (dtDate != null)
			{
				dtChart1 = new DataTable();
				dtChart1.Columns.Add("Дата", typeof(string));
				dtChart1.Columns.Add("Максимальная цена", typeof(double));
				dtChart1.Columns.Add("Средняя цена", typeof(double));
				dtChart1.Columns.Add("Минимальная цена", typeof(double));
				dtChart1.Columns.Add("Район с максимальной ценой", typeof(string));
				dtChart1.Columns.Add("Район с минимальной ценой", typeof(string));

				int start = dtDate.Rows.Count < 36 ? 0 : dtDate.Rows.Count - 36;
				for (int i = start; i < dtDate.Rows.Count; ++i)
				{
					string year = dtDate.Rows[i][0].ToString();
					string month = dtDate.Rows[i][3].ToString();
					string day = dtDate.Rows[i][4].ToString();
					date.Value = StringToMDXDate(day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года");
					string query = DataProvider.GetQueryText("ORG_0001_0001_fuel_data");
					DataTable dtFuelData = new DataTable();
					DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные по одному виду топлива", dtFuelData);
					if (dtFuelData.Rows.Count != 0)
					{

                        Dictionary<string, object> result = new Dictionary<string,object>();
						ParseFuelDataMath(dtFuelData, result);

						DataRow row = dtChart1.NewRow();
						row[0] = MDXDateToShortDateString(date.Value);
                        row[1] = result["MaxRegionValue"];
                        row[2] = result["AVG"];
                        row[3] = result["MinRegionValue"];
                        row[4] = result["MaxRegionName"];
                        row[5] = result["MinRegionName"];
						dtChart1.Rows.Add(row);
					}
				}

				UltraChart1.Series.Clear();
				for (int i = 1; i < 4; i++)
				{
					NumericSeries series = CRHelper.GetNumericSeries(i, dtChart1);
					series.Label = dtChart1.Columns[i].ColumnName;
					UltraChart1.Series.Add(series);
				}
			}
			else
			{
				UltraChart1.DataSource = null;
			}
		}

		protected void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];
				if (!String.IsNullOrEmpty(primitive.Path) && primitive.Path == "Legend" && primitive is Polyline)
				{
					Polyline line = primitive as Polyline;
					Symbol symbol = new Symbol();
					symbol.PE = line.PE;
					symbol.icon = SymbolIcon.Square;
					symbol.iconSize = SymbolIconSize.Medium;
					symbol.point = line.points[1].point;
					e.SceneGraph[i] = symbol;
				}
				if (primitive is PointSet)
				{
					PointSet pointset = primitive as PointSet;
					for (int point_num = 0; point_num < pointset.points.Length; ++point_num)
					{
						DataPoint point = pointset.points[point_num];
						if (point.Series.Label == "Максимальная цена" && dtChart1.Rows[point_num][4] != DBNull.Value)
						{
							point.DataPoint.Label = dtChart1.Rows[point_num][4].ToString();
						}
						if (point.Series.Label == "Минимальная цена" && dtChart1.Rows[point_num][5] != DBNull.Value)
						{
							point.DataPoint.Label = dtChart1.Rows[point_num][5].ToString();
						}
						if ((point.Series.Label != "Максимальная цена") && (point.Series.Label != "Минимальная цена"))
						{
							point.DataPoint.Label = "ХМАО";
						}
					}
				}
			}

            Text Caption = new Text();
            Caption.SetTextString("рублей за литр");
            Caption.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
            Caption.labelStyle.FontColor = Color.Gray;
            Caption.bounds.X = -30;
            Caption.bounds.Y = 120;
            Caption.bounds.Width = 100;
            Caption.bounds.Height = 100;

            e.SceneGraph.Add(Caption);
		}

		#endregion

		#region Обработчики диаграммы 2


        void SaveTable(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                SaveS(row.ItemArray);
            }
        }

        //protected void UltraChart2_DataBinding(object sender, EventArgs e)
        //{   

        //    UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>";
        //    fuel.Value = UltraWebGrid.Rows[selectedGridRow - selectedGridRow % 3].Cells[0].GetText();
        //    LabelChart2.Text = String.Format(Chart2TitleCaption, fuel.Value);

        //    if (dtDate != null)
        //    {
        //        dtChart2 = new DataTable();
        //        dtChart2.Columns.Add("Дата", typeof(string));
        //        dtChart2.Columns.Add("Закупочная цена", typeof(double));
        //        dtChart2.Columns.Add("Розничная цена", typeof(double));

        //        int start = dtDate.Rows.Count < 36 ? 0 : dtDate.Rows.Count - 36;
                
        //        for (int i = start; i < dtDate.Rows.Count; ++i)
        //        {
        //            string year = dtDate.Rows[i][0].ToString();
        //            string month = dtDate.Rows[i][3].ToString();
        //            string day = dtDate.Rows[i][4].ToString();
        //            date.Value = StringToMDXDate(day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года");
        //            price.Value = "Закупочная цена";
        //            string query = DataProvider.GetQueryText("ORG_0001_0001_fuel_data");
        //            DataTable dtFuelData = new DataTable();
        //            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные по одному виду топлива", dtFuelData);

        //            if (dtFuelData.Rows.Count != 0)
        //            {
        //                Dictionary<string,object> result = new Dictionary<string,object>();
        //                DataRow row = dtChart2.NewRow();

        //                ParseFuelDataMath(dtFuelData, result);
        //                row[0] = MDXDateToShortDateString(date.Value);
        //                row[1] = result["AVG"];
        //                price.Value = "Розничная цена";
        //                query = DataProvider.GetQueryText("ORG_0001_0001_fuel_data");
        //                dtFuelData = new DataTable();
        //                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные по одному виду топлива", dtFuelData);
        //                ParseFuelDataMath(dtFuelData, result);
        //                try
        //                {
        //                    row[2] = row[1] == DBNull.Value ? result["AVG"] : Minus(result["AVG"], row["MinRegionName"]);
        //                }
        //                catch { }
        //                dtChart2.Rows.Add(row);
        //            }
        //        }

        //        if (dtChart2.Rows.Count > 0)
        //        {

        //            UltraChart2.Data.SwapRowsAndColumns = true;
        //            UltraChart2.Series.Clear();
        //            for (int i = 1; i < dtChart2.Columns.Count; i++)
        //            {
        //                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart2);
        //                series.Label = dtChart2.Columns[i].ColumnName;
        //                UltraChart2.Series.Add(series);
        //            }
        //        }
        //        else
        //        {
        //            UltraChart2.DataSource = null;
        //        }
        //    }
        //    else
        //    {
        //        UltraChart2.DataSource = null;
        //    }
        //}
        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>";
            fuel.Value = UltraWebGrid.Rows[selectedGridRow - selectedGridRow % 3].Cells[0].GetText();
            LabelChart2.Text = String.Format(Chart2TitleCaption, fuel.Value);
            if (dtDate != null)
            {
                dtChart2 = new DataTable();
                dtChart2.Columns.Add("Дата", typeof(string));
                dtChart2.Columns.Add("Закупочная цена", typeof(double));
                dtChart2.Columns.Add("Розничная цена", typeof(double));

                int start = dtDate.Rows.Count < 36 ? 0 : dtDate.Rows.Count - 36;
                for (int i = start; i < dtDate.Rows.Count; ++i)
                {
                    string year = dtDate.Rows[i][0].ToString();
                    string month = dtDate.Rows[i][3].ToString();
                    string day = dtDate.Rows[i][4].ToString();
                    date.Value = StringToMDXDate(day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года");
                    price.Value = "Закупочная цена";
                    string query = DataProvider.GetQueryText("ORG_0001_0001_fuel_data");
                    DataTable dtFuelData = new DataTable();
                    DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные по одному виду топлива", dtFuelData);
                    if (dtFuelData.Rows.Count != 0)
                    {
                        object[] result = new object[5];
                        DataRow row = dtChart2.NewRow();
                        ParseFuelDataMath(dtFuelData, result);
                        row[0] = MDXDateToShortDateString(date.Value);
                        row[1] = result[0];
                        price.Value = "Розничная цена";
                        query = DataProvider.GetQueryText("ORG_0001_0001_fuel_data");
                        dtFuelData = new DataTable();
                        DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные по одному виду топлива", dtFuelData);
                        ParseFuelDataMath(dtFuelData, result);
                        row[2] = row[1] == DBNull.Value ? result[0] : Minus(result[0], row[1]);

                        dtChart2.Rows.Add(row);
                    }
                }

                UltraChart2.Data.SwapRowsAndColumns = true;
                UltraChart2.Series.Clear();
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

        /// <summary>
        /// Разбирает таблицу с данными. На выходе массив: арифметическое среднее по ХМАО, минимальное и максимальное значение среднего по МР
        /// </summary>
        /// <param name="dtFuelData">Таблица с данными</param>
        /// <param name="results">Массив с результатами</param>
        protected void ParseFuelDataMath(DataTable dtFuelData, object[] results)
        {
            if (dtFuelData == null || dtFuelData.Rows.Count == 0)
            {
                for (int i = 0; i < results.Length; ++i)
                    results[i] = DBNull.Value;
                return;
            }
            double[] gmValues = new double[dtFuelData.Rows.Count];
            string[] gmRegions = new string[dtFuelData.Rows.Count];
            for (int i = 0; i < dtFuelData.Rows.Count; ++i)
            {
                DataRow row = dtFuelData.Rows[i];
                gmRegions[i] = row[0].ToString().Replace("муниципальный район", "р-н").Replace("Город ", "г. ");
                int count = 0;
                for (int j = 1; j < row.ItemArray.Length; ++j)
                {
                    object cell = row[j];
                    if (cell != DBNull.Value)
                    {
                        gmValues[i] = (gmValues[i] == 0) ? Convert.ToDouble(cell) : gmValues[i] + Convert.ToDouble(cell);
                        ++count;
                    }
                }
                gmValues[i] = gmValues[i] / count;
            }
            Array.Sort(gmValues, gmRegions);
            results[0] = 0.0;
            foreach (double value in gmValues)
            {
                results[0] = Convert.ToDouble(results[0]) + value;
            }
            results[0] = Convert.ToDouble(results[0]) / gmValues.Length;
            results[1] = SetStarChar(gmRegions[0]);
            results[2] = gmValues[0];
            results[3] = SetStarChar(gmRegions[gmRegions.Length - 1]);
            results[4] = gmValues[gmValues.Length - 1];
        }
		protected void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];
				Box box;
				if (primitive is Box && primitive.DataPoint != null)
					if (primitive.DataPoint.Label == "Розничная цена")
					{
						box = primitive as Box;
						double value1, value2;
						if (Double.TryParse(dtChart2.Rows[box.Row][1].ToString(), out value1) && Double.TryParse(dtChart2.Rows[box.Row][2].ToString(), out value2))
						{
							box.DataPoint.Label = String.Format("Розничная цена\n<b>{0:N2}</b>, рублей за литр", value1 + value2);
						}
						if (dtChart2.Rows[box.Row][1] == DBNull.Value && Double.TryParse(dtChart2.Rows[box.Row][2].ToString(), out value2))
						{
							box.DataPoint.Label = String.Format("Розничная цена\n<b>{0:N2}</b>, рублей за литр", value2);
						}
					}
					else if (primitive.DataPoint.Label == "Закупочная цена")
					{
						box = primitive as Box;
						double value;
						if (Double.TryParse(dtChart2.Rows[box.Row][1].ToString(), out value))
						{
							box.DataPoint.Label = String.Format("Закупочная цена\n<b>{0:N2}</b>, рублей за литр", value);
						}
					}
			}

            Text Caption = new Text();
            Caption.SetTextString("рублей за литр");
            Caption.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
            Caption.labelStyle.FontColor = Color.Gray;
            Caption.bounds.X = -30;
            Caption.bounds.Y = 120;
            Caption.bounds.Width = 100;
            Caption.bounds.Height = 100;

            e.SceneGraph.Add(Caption);
		}

		#endregion

		// --------------------------------------------------------------------

		#region Общие функции

		/// <summary>
		/// Разбирает таблицу с данными. На выходе массив: арифметическое среднее по ХМАО, минимальное и максимальное значение среднего по МР
		/// </summary>
		/// <param name="dtFuelData">Таблица с данными</param>
		/// <param name="results">Массив с результатами</param>
		protected void ParseFuelDataMath(DataTable dtFuelData,
            Dictionary<string, object> results)
		{
			if (dtFuelData == null || dtFuelData.Rows.Count == 0)
			{

                try
                {
                    results.Add("AVG", DBNull.Value);
                    results.Add("MinRegionName", DBNull.Value);
                    results.Add("MinRegionValue", DBNull.Value);
                    results.Add("MaxRegionName", DBNull.Value);
                    results.Add("MaxRegionValue", DBNull.Value);
                }
                catch { }

				return;
			}
			double[] gmValues = new double[dtFuelData.Rows.Count];
			string[] gmRegions = new string[dtFuelData.Rows.Count];
			for (int i = 0; i < dtFuelData.Rows.Count; ++i)
			{
				DataRow row = dtFuelData.Rows[i];
				gmRegions[i] = row[0].ToString().Replace("муниципальный район", "р-н").Replace("Город ", "г. ");
				int count = 0;
				for (int j = 1; j < row.ItemArray.Length; ++j)
				{
					object cell = row[j];
					if (cell != DBNull.Value)
					{
						gmValues[i] = (gmValues[i] == 0) ? Convert.ToDouble(cell) : gmValues[i] + Convert.ToDouble(cell);
						++count;
					}
				}
				gmValues[i] = gmValues[i] / count;
			}
			Array.Sort(gmValues, gmRegions);
            results["AVG"] = 0.0;
			foreach (double value in gmValues)
			{
                results["AVG"] = Convert.ToDouble(results["AVG"]) + value;
			}
            results["AVG"] = Convert.ToDouble(results["AVG"]) / gmValues.Length;
            results["MinRegionName"] = SetStarChar(gmRegions[0]);
            results["MinRegionValue"] = gmValues[0];
            results["MaxRegionName"] = SetStarChar(gmRegions[gmRegions.Length - 1]);
            results["MaxRegionValue"] = gmValues[gmValues.Length - 1];
		}

		/// <summary>
		/// Разбирает таблицу с данными. На выходе массив: геометрическое среднее по ХМАО, минимальное и максимальное значение среднего по МР
		/// </summary>
		/// <param name="dtFuelData">Таблица с данными</param>
		/// <param name="results">Массив с результатами</param>
		protected void ParseFuelData(DataTable dtFuelData, object[] results)
		{
			if (dtFuelData == null || dtFuelData.Rows.Count == 0)
			{
				for (int i = 0; i < results.Length; ++i)
					results[i] = DBNull.Value;
				return;
			}
			double[] gmValues = new double[dtFuelData.Rows.Count];
			string[] gmRegions = new string[dtFuelData.Rows.Count];
			for (int i = 0; i < dtFuelData.Rows.Count; ++i)
			{
				DataRow row = dtFuelData.Rows[i];
				gmRegions[i] = row[0].ToString().Replace("муниципальный район", "р-н").Replace("Город ", "г. ");
				int count = 0;
				for (int j = 1; j < row.ItemArray.Length; ++j)
				{
					object cell = row[j];
					if (cell != DBNull.Value)
					{
						gmValues[i] = (gmValues[i] == 0) ? Convert.ToDouble(cell) : gmValues[i] * Convert.ToDouble(cell);
						++count;
					}
				}
				gmValues[i] = Math.Pow(gmValues[i], 1.0 / count);
			}
			Array.Sort(gmValues, gmRegions);
			results[0] = 1.0;
			foreach (double value in gmValues)
			{
				results[0] = Convert.ToDouble(results[0]) * value;
			}
			results[0] = Math.Pow(Convert.ToDouble(results[0]), 1.0 / gmValues.Length);
			results[1] = SetStarChar(gmRegions[0]);
			results[2] = gmValues[0];
			results[3] = SetStarChar(gmRegions[gmRegions.Length - 1]);
			results[4] = gmValues[gmValues.Length - 1];
		}

		#endregion

		#region Заполнение словарей и выпадающих списков параметров

		protected void FillComboDate(CustomMultiCombo combo, string queryName, int offset)
		{
			dtDate = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForPivotTable(query, dtDate);
			if (dtDate.Rows.Count == 0)
			{
				throw new Exception("Данные для построения отчета отсутствуют в кубе");
			}
			Dictionary<string, int> dictDate = new Dictionary<string, int>();
			for (int row = 0; row < dtDate.Rows.Count - offset; ++row)
			{
				string year = dtDate.Rows[row][0].ToString();
				string month = dtDate.Rows[row][3].ToString();
				string day = dtDate.Rows[row][4].ToString();
				AddPairToDictionary(dictDate, year + " год", 0);
				AddPairToDictionary(dictDate, month + " " + year + " года", 1);
				AddPairToDictionary(dictDate, day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года", 2);
			}
			combo.FillDictionaryValues(dictDate);
			combo.SelectLastNode();
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

		public object Plus(object firstValue, object secondValue)
		{
			double value1, value2;
			if (Double.TryParse(firstValue.ToString(), out value1) && Double.TryParse(secondValue.ToString(), out value2))
				return value1 + value2;
			else
				return DBNull.Value;
		}

		public object Minus(object firstValue, object secondValue)
		{
			double value1, value2;
			if (Double.TryParse(firstValue.ToString(), out value1) && Double.TryParse(secondValue.ToString(), out value2))
				return value1 - value2;
			else
				return DBNull.Value;
		}

		public object Percent(object firstValue, object secondValue)
		{
			double value1, value2;
			if (Double.TryParse(firstValue.ToString(), out value1) && Double.TryParse(secondValue.ToString(), out value2) && value2 != 0)
				return value1 / value2;
			else
				return DBNull.Value;
		}
		public object Grow(object firstValue, object secondValue)
		{
			double value1, value2;
			if (Double.TryParse(firstValue.ToString(), out value1) && Double.TryParse(secondValue.ToString(), out value2) && value2 != 0)
				return value1 / value2 - 1;
			else
				return DBNull.Value;
		}

		public string GetMaxMDXDate(string firstDate, string secondDate)
		{
			if (Convert.ToInt32(FormatMDXDate(firstDate, "{0}{1:00}{2:00}")) > Convert.ToInt32(FormatMDXDate(secondDate, "{0}{1:00}{2:00}")))
			{
				return firstDate;
			}
			else
			{
				return secondDate;
			}
		}

		public string FormatMDXDate(string mdxDate, string formatString, int yearIndex, int monthIndex, int dayIndex)
		{
			string[] separator = { "].[" };
			string[] dateElements = mdxDate.Split(separator, StringSplitOptions.None);
			if (dateElements.Length < 8)
			{
				return String.Format(formatString, 1998, 1, 1);
			}
			int year = Convert.ToInt32(dateElements[yearIndex]);
			int month = Convert.ToInt32(CRHelper.MonthNum(dateElements[monthIndex]));
			int day = Convert.ToInt32(dateElements[dayIndex]);
			return String.Format(formatString, year, month, day);
		}

		public string FormatMDXDate(string mdxDate, string formatString)
		{
			string[] separator = { "].[" };
			string[] dateElements = mdxDate.Split(separator, StringSplitOptions.None);
			if (dateElements.Length < 8)
			{
				return String.Format(formatString, 1998, 1, 1);
			}
			int year = Convert.ToInt32(dateElements[3]);
			int month = Convert.ToInt32(CRHelper.MonthNum(dateElements[6]));
			int day = Convert.ToInt32(dateElements[7].Replace("]", String.Empty));
			return String.Format(formatString, year, month, day);
		}

		public string StringToMDXDate(string str)
		{
			string template = "[Период__День].[Период__День].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]";
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
			string template = "{0}.{1}.{2}";
			string day = dateElements[7].Replace("]", String.Empty);
			day = day.Length == 1 ? "0" + day : day;
			string month = CRHelper.MonthNum(dateElements[6]).ToString();
			month = month.Length == 1 ? "0" + month : month;
			string year = dateElements[3];
			return String.Format(template, day, month, year);
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

			ReportExcelExporter1.HeaderCellHeight = 80;
			ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 10);
			ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
			ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
			ReportExcelExporter1.TitleStartRow = 0;

			for (int i = 2; i < UltraWebGrid.Rows.Count; i += 3)
			{
				UltraWebGrid.Rows[i].Cells[0].Value = null;
			}

			ReportExcelExporter1.Export(headerLayout, sheet1, 3);

			//sheet1.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
			sheet1.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			//sheet1.Rows[1].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
			sheet1.Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			sheet1.Rows[1].Height = 550;

			for (int i = 6; i < UltraWebGrid.Rows.Count + 6; ++i)
			{
				Font exportFont = new Font("Verdana", 9);
				sheet1.Rows[i].Height = 255;
				for (int j = 1; j < UltraWebGrid.Columns.Count; ++j)
				{
					sheet1.Rows[i].Cells[j].CellFormat.Alignment = HorizontalCellAlignment.Right;
					sheet1.Rows[i].Cells[j].CellFormat.Font.Name = exportFont.Name;
					sheet1.Rows[i].Cells[j].CellFormat.Font.Height = (int)exportFont.Size * 20;
				}
			}

			for (int i = 0; i < UltraWebGrid.Rows.Count; i += 3)
			{
				for (int j = 0; j < UltraWebGrid.Columns.Count; ++j)
				{
					sheet1.Rows[6 + i].Cells[j].CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
					sheet1.Rows[7 + i].Cells[j].CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
					sheet1.Rows[7 + i].Cells[j].CellFormat.TopBorderStyle = CellBorderLineStyle.None;
					sheet1.Rows[8 + i].Cells[j].CellFormat.TopBorderStyle = CellBorderLineStyle.None;
				}
			}

			ReportExcelExporter1.WorksheetTitle = String.Empty;
			ReportExcelExporter1.WorksheetSubTitle = String.Empty;
			UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.6);
			UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4);
			ReportExcelExporter1.Export(UltraChart1, LabelChart1.Text, sheet2, 1);
			UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.6);
			UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4);
			ReportExcelExporter1.Export(UltraChart2, LabelChart2.Text, sheet3, 1);
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

		private  void SetExportGridParams(UltraWebGrid grid)
		{
			string exportFontName = "Verdana";
			int fontSize = 9;
			double coeff = 1.2;

            grid.Columns.Add("Безымяный столбик");
            foreach (UltraGridRow Row in grid.Rows)
            {
                if (Row.Index % 3 == 0)
                {
                    Row.Cells.FromKey("Безымяный столбик").Value = "Значение";
                    Row.NextRow.Cells.FromKey("Безымяный столбик").Value = "Темп прироста";
                    Row.NextRow.NextRow.Cells.FromKey("Безымяный столбик").Value = "Абсолютное отклонение";
                }
            }

            grid.Columns.FromKey("Безымяный столбик").Move(1);
            grid.Columns.FromKey("Безымяный столбик").Width = 140;

            headerLayout = new GridHeaderLayout(UltraWebGrid);

            GridHeaderCell Cell = headerLayout.AddCell("Вид топлива");
            Cell.AddCell("");
            Cell.AddCell("");
            
            GridHeaderCell headerCell = headerLayout.AddCell("Закупочная цена, рублей за литр");
            GridHeaderCell headerCell1 = headerCell.AddCell(MDXDateToShortDateString(compareDate.Value));
            headerCell1.AddCell("средняя");
            headerCell = headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            headerCell.AddCell("средняя");
            headerCell.AddCell("минимальная");
            headerCell.AddCell("максимальная");
            headerCell = headerLayout.AddCell("Розничная цена, рублей за литр");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(compareDate.Value));
            headerCell1.AddCell("средняя");
            headerCell = headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            headerCell.AddCell("средняя");
            headerCell.AddCell("минимальная");
            headerCell.AddCell("максимальная");
            headerCell = headerLayout.AddCell("Разница между средней розничной ценой и средней закупочной ценой, рублей за литр");
            headerCell.AddCell(MDXDateToShortDateString(compareDate.Value), 2);
            headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value), 2);
            headerCell = headerLayout.AddCell("Процент торговой надбавки");
            headerCell.AddCell(MDXDateToShortDateString(compareDate.Value), 2);
            headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value), 2);

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

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();

            IText title = section1.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);


            title = section1.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);

            UltraWebGrid grid = headerLayout.Grid;

            grid.Columns.Add("Безымяный столбик");
            grid.Columns.FromKey("Безымяный столбик").Move(1);
            grid.Columns.FromKey("Безымяный столбик").Width = 180;
            foreach (UltraGridRow Row in grid.Rows)
            {
                if (Row.Index % 3 == 0)
                {
                    Row.Cells.FromKey("Безымяный столбик").Value = "Значение";
                    Row.NextRow.Cells.FromKey("Безымяный столбик").Value = "Темп прироста";
                    Row.NextRow.NextRow.Cells.FromKey("Безымяный столбик").Value = "Абсолютное отклонение";
                    Row.NextRow.NextRow.Cells[0].Text = "";
                }
            }

            headerLayout = new GridHeaderLayout(UltraWebGrid);

            grid.Width = 1650;

            for (int i = 0; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].Width = 100;    
            }
            
            GridHeaderCell c = 
                headerLayout.AddCell("Вид топлива");
            c.AddCell(" ").SpanY = 2;
            c.AddCell(" ").SpanY = 2;
            GridHeaderCell headerCell = headerLayout.AddCell("Закупочная цена, рублей за литр");
            GridHeaderCell headerCell1 = headerCell.AddCell(MDXDateToShortDateString(compareDate.Value));
            headerCell1.AddCell("средняя");
            headerCell = headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            headerCell.AddCell("средняя");
            headerCell.AddCell("минимальная");
            headerCell.AddCell("максимальная");
            headerCell = headerLayout.AddCell("Розничная цена, рублей за литр");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(compareDate.Value));
            headerCell1.AddCell("средняя");
            headerCell = headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            headerCell.AddCell("средняя");
            headerCell.AddCell("минимальная");
            headerCell.AddCell("максимальная");
            headerCell = headerLayout.AddCell("Разница между средней розничной ценой и средней закупочной ценой, рублей за литр");
            headerCell.AddCell(MDXDateToShortDateString(compareDate.Value), 2);
            headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value), 2);
            headerCell = headerLayout.AddCell("Процент торговой надбавки");
            headerCell.AddCell(MDXDateToShortDateString(compareDate.Value), 2);
            headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value), 2);
            headerLayout.ApplyHeaderInfo();

            ReportPDFExporter1.HeaderCellHeight = 60;
            ReportPDFExporter1.Export(headerLayout, section1);

            UltraChart1.Width = LabelChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.8);
            ReportPDFExporter1.Export(UltraChart1, LabelChart1.Text, section2);

            UltraChart2.Width = LabelChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.8);
            ReportPDFExporter1.Export(UltraChart2, LabelChart2.Text, section3);
        }

        #region Рисовалка иерархичного хидера
        Background headerBackground = null;
        Borders headerBorders = null;
        private bool HeaderIsChildren(HeaderBase Parent, HeaderBase Children)
        {
            if (Parent == Children)
            {
                return false;
            }

            if (((Parent.RowLayoutColumnInfo.OriginY + Parent.RowLayoutColumnInfo.SpanY) == Children.RowLayoutColumnInfo.OriginY)
                &&
                ((Parent.RowLayoutColumnInfo.OriginX <= Children.RowLayoutColumnInfo.OriginX) &&
                ((Parent.RowLayoutColumnInfo.OriginX + Parent.RowLayoutColumnInfo.SpanX) > Children.RowLayoutColumnInfo.OriginX)))
            {
                return true;
            }
            return false;
        }

        
        private List<HeaderBase> GetChildHeader(HeaderBase ParentHeder)
        {
            UltraWebGrid Grid = UltraWebGrid.Bands[0].Grid;    
            List<HeaderBase> ChildHeader = new List<HeaderBase>();
            
            foreach (HeaderBase Header in Grid.Bands[0].HeaderLayout)
            {
                if (HeaderIsChildren(ParentHeder, Header))
                {
                    ChildHeader.Add(Header);
                }
            }

            return ChildHeader;
        }

        protected bool HeaderIsRootLevel(HeaderBase Header)
        {
            return Header.RowLayoutColumnInfo.OriginY == 0;
        }

        ITableRow CreateChildrenRow(ITableCell row)
        {
            return row.Parent.Parent.AddRow();
        }

        int[] PDFHeaderHeightsLevel = { 20, 20, 20,20 };

        int PDFGetLevelHeight(int level, int span)
        {
            int sumHeightLevel = 0;
            for (int i = level; i < level + span; i++)
            {
                sumHeightLevel += PDFHeaderHeightsLevel[i];
            }
            return sumHeightLevel;
        }

        private int CreateHierarhyHeader(HeaderBase header, ITableRow row)
        {
            List<HeaderBase> ChildHeaders = GetChildHeader(header);
            row = row.AddCell().AddTable().AddRow();

            ITableCell ParentCell = row.AddCell();

            int width = AddTableCell(ParentCell, header, header.RowLayoutColumnInfo.SpanX, PDFGetLevelHeight(header.RowLayoutColumnInfo.OriginY, header.RowLayoutColumnInfo.SpanY));

            if (ChildHeaders.Count > 0)
            {
                width = 0;
                ITableRow ChildrenRow = row.Parent.AddRow();
                foreach (HeaderBase ChildHeader in ChildHeaders)
                {
                    width += CreateHierarhyHeader(ChildHeader, ChildrenRow);
                }

                setHederWidth(ParentCell, width);
            }
            return width;

        }

        private int CreateAllRootHeader(ITableRow RootRow)
        {
            int sumW = 0;
            foreach (HeaderBase Header in UltraWebGrid.Bands[0].HeaderLayout)
            {
                if (HeaderIsRootLevel(Header))
                {
                    sumW += CreateHierarhyHeader(Header, RootRow);
                }
            }
            return sumW;
        }

        private void ExportHeader(ITable Table)
        {
            ITableRow RootRow = Table.AddRow();

            ITableRow SelectorCol = RootRow.AddCell().AddTable().AddRow();

            int sumW = CreateAllRootHeader(RootRow);
            Table.Width = new FixedWidth(sumW);
        }

        private void ApplyHeader()
        {
            foreach (UltraGridColumn col in UltraWebGrid.Columns)
            {
                if ((col.Hidden))
                {
                    continue;
                }
                UltraWebGrid.Bands[0].HeaderLayout.Add(col.Header);
            }
        }

        class sortHeder : IComparer
        {
            public int Compare(object x, object y)
            {
                if (((HeaderBase)x).RowLayoutColumnInfo.OriginX > ((HeaderBase)y).RowLayoutColumnInfo.OriginX)
                {
                    return 1;
                }
                if (((HeaderBase)x).RowLayoutColumnInfo.OriginX < ((HeaderBase)y).RowLayoutColumnInfo.OriginX)
                {
                    return -1;
                }
                return 0;
            }
        }

        string KeyMainColumn = "Field";
        private void PreProcessing(MarginCellExportedEventArgs e)
        {
            headerBackground = e.ReportCell.Background;
            headerBorders = e.ReportCell.Borders;

            //Скрываем хедер рисуемый по умолчанию
            e.ReportCell.Parent.Height = new FixedHeight(0);

            ApplyHeader();

            SettingHeaderGrid();

            //SettingColWidth();

            UltraWebGrid.Bands[0].HeaderLayout.Sort(new sortHeder());
        }

        protected void SetHeaderPos(ColumnHeader HeaderCol, int OriginX, int OriginY, int SpanX, int SpanY)
        {
            HeaderCol.RowLayoutColumnInfo.OriginX = OriginX;

            HeaderCol.RowLayoutColumnInfo.OriginY = OriginY;

            HeaderCol.RowLayoutColumnInfo.SpanX = SpanX;

            HeaderCol.RowLayoutColumnInfo.SpanY = SpanY;
        }

        protected void GenerationHeader(UltraWebGrid Grid, string Caption, int OriginX, int OriginY, int SpanX, int SpanY)
        {
            ColumnHeader HeaderCol = new ColumnHeader();

            SetHeaderPos(HeaderCol, OriginX, OriginY, SpanX, SpanY);

            HeaderCol.Caption = Caption;

            Grid.Bands[0].HeaderLayout.Add(HeaderCol);
        }

        private void SettingHeaderGrid()
        {
            //UltraWebGrid.Bands[0].HeaderLayout.Clear();
            //UltraWebGrid.DataBind();
            //Infragistics.WebUI.UltraWebGrid.ColumnsCollection Columns = UltraWebGrid.Columns;
            ////GenerationHeader(UltraWebGrid, "текст", 0, 0, 1, 3);
            //SetHeaderPos(Columns[0].Header, 0, 0, 1, 3);

            //GenerationHeader(UltraWebGrid, "текст", 1, 0, 4, 1);
            //GenerationHeader(UltraWebGrid, "текст", 1, 1, 1, 1);
            //GenerationHeader(UltraWebGrid, "текст", 2, 1, 3, 1);

            //SetHeaderPos(Columns[1].Header, 1, 3, 1, 1);
            //SetHeaderPos(Columns[2].Header, 2, 3, 1, 1);
            //SetHeaderPos(Columns[3].Header, 3, 3, 1, 1);
            //SetHeaderPos(Columns[4].Header, 4, 3, 1, 1);
            //SetHeaderPos(Columns[5].Header, 5, 3, 1, 1);
            //SetHeaderPos(Columns[6].Header, 6, 3, 1, 1);
            //SetHeaderPos(Columns[7].Header, 7, 3, 1, 1);
            //SetHeaderPos(Columns[8].Header, 8, 3, 1, 1);

            //SetHeaderPos(Columns[9].Header, 9, 2, 1, 2);
            //SetHeaderPos(Columns[10].Header, 10, 2, 1, 2);
            //SetHeaderPos(Columns[11].Header, 11, 2, 1, 2);
            //SetHeaderPos(Columns[12].Header, 12, 2, 1, 2);



            //GenerationHeader(UltraWebGrid, "текст", 5, 0, 4, 1);
            //GenerationHeader(UltraWebGrid, "текст", 5, 1, 1, 1);
            //GenerationHeader(UltraWebGrid, "текст", 6, 1, 3, 1);


            //GenerationHeader(UltraWebGrid, "текст", 9, 0, 2, 1);
            //////GenerationHeader(UltraWebGrid, "текст", 9, 1, 1, 2);
            //////GenerationHeader(UltraWebGrid, "текст", 10, 1, 1, 2);

            //GenerationHeader(UltraWebGrid, "текст", 11, 0, 2, 1);
            //GenerationHeader(UltraWebGrid, "текст", 11, 1, 1, 2);
            //GenerationHeader(UltraWebGrid, "текст", 12, 1, 1, 2);
            
        }

        private void PdfExporter_Test(object sender, MarginCellExportedEventArgs e)
        {
            if (headerBackground != null)
            {
                return;
            }

            PreProcessing(e);

            ITable Table = e.ReportCell.Parent.Parent;
            ExportHeader(Table);
        }

        #region UtilsFromExportGridToPDF
        private int AddTableCell(ITableCell tableCell, HeaderBase header, Double width, Double Height)
        {
            if (header.Column != null)
            {
                width = 0.75 * (int)header.Column.Width.Value;
            }

            SetCellStyle(tableCell);
            tableCell.Height = new FixedHeight((int)Height);
            tableCell.Width = new FixedWidth((int)width);
            tableCell.Parent.Parent.Width = new FixedWidth((int)width);
            object o = tableCell.Parent.Parent.Parent;
            ITableCell parentCell = (ITableCell)(o);
            parentCell.Width = new FixedWidth((int)width);
            IText text = tableCell.AddText();
            SetFontStyle(text);

            text.AddContent(header.Caption);

            return (int)width;
        }
        public void SetCellStyle(ITableCell headerCell)
        {
            headerCell.Alignment.Horizontal = Alignment.Center;
            headerCell.Alignment.Vertical = Alignment.Middle;
            headerCell.Borders = headerBorders;
            headerCell.Paddings.All = 2;
            headerCell.Background = headerBackground;
        }
        public static void SetFontStyle(IText t)
        {
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font(new System.Drawing.Font("Arial", 8));
            t.Style.Font = font;
            t.Style.Font.Bold = true;
            t.Alignment = Infragistics.Documents.Reports.Report.TextAlignment.Center;
        }
        private ITableCell AddTableCell(ITableRow row, string cellText, Double width, Double Height)
        {
            ITableCell tableCell = row.AddCell();

            SetCellStyle(tableCell);
            tableCell.Height = new FixedHeight((int)Height);
            tableCell.Width = new FixedWidth((int)width);
            tableCell.Parent.Parent.Width = new FixedWidth((int)width);
            object o = tableCell.Parent.Parent.Parent;
            ITableCell parentCell = (ITableCell)(o);
            parentCell.Width = new FixedWidth((int)width);
            IText text = tableCell.AddText();

            text.Style.Font.Size = 1;
            text.Paddings.Left = 100;
            SetFontStyle(text);

            text.AddContent(cellText);

            return tableCell;
        }

        void setHederWidth(ITableCell tableCell, Double width)
        {
            tableCell.Width = new FixedWidth((int)width);
            tableCell.Parent.Parent.Width = new FixedWidth((int)width);
            object o = tableCell.Parent.Parent.Parent;
            ITableCell parentCell = (ITableCell)(o);
            parentCell.Width = new FixedWidth((int)width);
        }
        #endregion
        #endregion

        #endregion
	}
}
