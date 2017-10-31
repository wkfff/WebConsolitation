using System;
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
using Infragistics.WebUI.UltraWebGrid.ExcelExport;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.Table;

/**
 *  Цены на нефтепродукты по муниципальным образованиям Ханты-Мансийского автономного округа – Югры
 */
namespace Krista.FM.Server.Dashboards.reports.ORG_0001_0003
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private static DataTable dtDate;
        private static DataTable dtGrid;
        private static DataTable dtChart1;
        private static DataTable dtRegion;
        private static GridHeaderLayout headerLayout;

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

        private CustomParam selectedDate;
        private CustomParam compareDate;
        private CustomParam date;
        private CustomParam fuel;
        private CustomParam price;
        private CustomParam region;
        private CustomParam selectedFuel;

        #endregion

        // --------------------------------------------------------------------

        private const string PageTitleCaption = "Анализ средних розничных и средних закупочных цен на нефтепродукты";
        private const string PageSubTitleCaption = "Ежедекадный мониторинг закупочных и розничных цен на нефтепродукты, реализуемые на территории муниципального образования, {0}, по состоянию на {1}";
        private string Chart1TitleCaption = "Динамика средней закупочной и средней розничной цены по виду нефтепродукта «{0}», рублей за литр";

        // --------------------------------------------------------------------

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            if (!IsSmallResolution)
            {
                ComboRegion.Title = "Территория";
                ComboRegion.Width = 400;
                ComboRegion.ShowSelectedValue = true;
                ComboRegion.ParentSelect = true;
                ComboDate.Title = "Выберите дату";
                ComboDate.Width = 275;
                ComboDate.ShowSelectedValue = true;
                ComboDate.ParentSelect = true;
                ComboCompareDate.Title = "Выберите дату для сравнения";
                ComboCompareDate.Width = 375;
                ComboCompareDate.ShowSelectedValue = true;
                ComboCompareDate.ParentSelect = true;
            }
            else
            {
                ComboRegion.PanelHeaderTitle = "Территория";
                ComboRegion.Width = 150;
                ComboRegion.ShowSelectedValue = false;
                ComboRegion.ParentSelect = true;
                ComboDate.PanelHeaderTitle = "Выберите дату";
                ComboDate.Width = 150;
                ComboDate.ShowSelectedValue = false;
                ComboDate.ParentSelect = true;
                ComboCompareDate.PanelHeaderTitle = "Выберите дату для сравнения";
                ComboCompareDate.Width = 250;
                ComboCompareDate.ShowSelectedValue = false;
                ComboCompareDate.ParentSelect = true;
            }

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
            UltraChart1.LineChart.NullHandling = NullHandling.InterpolateSimple;

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
            UltraChart1.Axis.Y.Extent = 40;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:### ### ##0.##>";

            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 10;
            UltraChart1.Legend.Font = new Font("Microsoft Sans Serif", 9);
            UltraChart1.Legend.Visible = true;

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.PEs.Clear();

            PaintElement pe;
            pe = new PaintElement(Color.Green);
            pe.StrokeWidth = 2;
            UltraChart1.ColorModel.Skin.PEs.Add(pe);
            pe = new PaintElement(Color.Blue);
            pe.StrokeWidth = 2;
            UltraChart1.ColorModel.Skin.PEs.Add(pe);
            pe = new PaintElement(Color.Yellow);
            pe.StrokeWidth = 2;
            UltraChart1.ColorModel.Skin.PEs.Add(pe);
            pe = new PaintElement(Color.Red);
            pe.StrokeWidth = 2;
            UltraChart1.ColorModel.Skin.PEs.Add(pe);

            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion

            #region Параметры

            selectedDate = UserParams.CustomParam("selected_date");
            compareDate = UserParams.CustomParam("compare_date");
            date = UserParams.CustomParam("date");
            fuel = UserParams.CustomParam("fuel");
            price = UserParams.CustomParam("price");
            region = UserParams.CustomParam("region");
            selectedFuel = UserParams.CustomParam("selected_fuel");

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
                FillComboRegion(ComboRegion, "ORG_0001_0003_list_of_regions");
                FillComboDate(ComboDate, "ORG_0001_0003_list_of_dates", 0);
                FillComboDate(ComboCompareDate, "ORG_0001_0003_list_of_dates", 1);
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
            region.Value = ComboRegion.SelectedValue.Replace("*", "");

            #endregion

            PageTitle.Text = PageTitleCaption;
            Page.Title = PageTitle.Text;

            PageSubTitle.Text = String.Format(PageSubTitleCaption, SetStarChar(region.Value), ComboDate.SelectedValue);

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
            if (!Page.IsPostBack)
            {
                selectedFuel.Value = UltraWebGrid.Rows[0].Cells[0].Text;
            }
            UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, selectedFuel.Value, 0, 0);
            UltraWebGrid_ChangeRow(row);
            if (UltraChart1.DataSource == null)
            {
                UltraChart1.DataBind();
            }
        }

        // --------------------------------------------------------------------

        #region Обработчики грида

        private void UltraWebGrid_ChangeRow(UltraGridRow row)
        {
            if (row == null)
                return;
            selectedFuel.Value = row.Cells[0].Text;

            DeaciveAllRow(UltraWebGrid);
            row.Activate();
            row.Activated = true;
            row.Selected = true;
            UltraChart1.DataBind();
        }

        private void DeaciveAllRow(Infragistics.WebUI.UltraWebGrid.UltraWebGrid UltraWebGrid)
        {
            foreach (UltraGridRow r in UltraWebGrid.Rows)
            {
                r.Activated = false;
                r.Selected = false;
            }
        }

        private void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            UltraWebGrid_ChangeRow(e.Row);
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {

            DataTable dtFuelTypes = new DataTable();
            string query = DataProvider.GetQueryText("ORG_0001_0003_fuel_types");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Вид топлива", dtFuelTypes);
            if (dtFuelTypes.Rows.Count > 0)
            {
                dtGrid = new DataTable();
                for (int i = 0; i < 17; ++i)
                {
                    dtGrid.Columns.Add();
                    dtGrid.Columns[i].DataType = typeof(string);
                }

                foreach (DataRow fuel_row in dtFuelTypes.Rows)
                {
                    fuel.Value = fuel_row[0].ToString();

                    DataRow row1 = dtGrid.NewRow();
                    DataRow row2 = dtGrid.NewRow();
                    DataRow row3 = dtGrid.NewRow();

                    // Название вида топлива
                    row1[0] = row2[0] = row3[0] = fuel.Value;

                    DataTable dtFuelData;
                    object byHMAO, byRegion;
                    // Закупочная цена
                    price.Value = "Закупочная цена";
                    date.Value = compareDate.Value;
                    dtFuelData = new DataTable();
                    query = DataProvider.GetQueryText("ORG_0001_0003_fuel_data");
                    DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные по одному виду топлива", dtFuelData);
                    ParseFuelDataMath(dtFuelData, region.Value, out byHMAO, out byRegion);
                    row1[1] = byRegion;
                    row1[2] = byHMAO;

                    date.Value = selectedDate.Value;
                    dtFuelData = new DataTable();
                    query = DataProvider.GetQueryText("ORG_0001_0003_fuel_data");
                    DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные по одному виду топлива", dtFuelData);
                    ParseFuelDataMath(dtFuelData, region.Value, out byHMAO, out byRegion);
                    row1[3] = byRegion;
                    row2[3] = Grow(row1[3], row1[1]);
                    row3[3] = Minus(row1[3], row1[1]);

                    row1[4] = byHMAO;
                    row2[4] = Grow(row1[4], row1[2]);
                    row3[4] = Minus(row1[4], row1[2]);

                    // Розничная цена
                    price.Value = "Розничная цена";
                    date.Value = compareDate.Value;
                    dtFuelData = new DataTable();
                    query = DataProvider.GetQueryText("ORG_0001_0003_fuel_data");
                    DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные по одному виду топлива", dtFuelData);
                    ParseFuelDataMath(dtFuelData, region.Value, out byHMAO, out byRegion);
                    row1[5] = byRegion;
                    row1[6] = byHMAO;

                    date.Value = selectedDate.Value;
                    dtFuelData = new DataTable();
                    query = DataProvider.GetQueryText("ORG_0001_0003_fuel_data");
                    DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные по одному виду топлива", dtFuelData);
                    ParseFuelDataMath(dtFuelData, region.Value, out byHMAO, out byRegion);

                    row1[7] = byRegion;
                    row2[7] = Grow(row1[7], row1[5]);
                    row3[7] = Minus(row1[7], row1[5]);

                    row1[8] = byHMAO;
                    row2[8] = Grow(row1[8], row1[6]);
                    row3[8] = Minus(row1[8], row1[6]);

                    // Разница между средней закупочной и средней розничной ценой
                    row1[9] = Minus(row1[5], row1[1]);
                    row1[10] = Minus(row1[6], row1[2]);
                    row1[11] = Minus(row1[7], row1[3]);
                    row2[11] = Grow(row1[11], row1[9]);
                    row3[11] = Minus(row1[11], row1[9]);
                    row1[12] = Minus(row1[8], row1[4]);
                    row2[12] = Grow(row1[12], row1[10]);
                    row3[12] = Minus(row1[12], row1[10]);

                    // Процент надбавки
                    row1[13] = Grow(row1[5], row1[1]);
                    row1[14] = Grow(row1[6], row1[2]);
                    row1[15] = Grow(row1[7], row1[3]);
                    row3[15] = Minus(row1[15], row1[13]);
                    row1[16] = Grow(row1[8], row1[4]);
                    row3[16] = Minus(row1[16], row1[14]);

                    dtGrid.Rows.Add(row1);
                    dtGrid.Rows.Add(row2);
                    dtGrid.Rows.Add(row3);
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
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.RowAlternateStylingDefault = Infragistics.WebUI.Shared.DefaultableBoolean.False;
            e.Layout.NullTextDefault = "-";

            // Заголовки
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            headerLayout.AddCell("Вид топлива");
            GridHeaderCell headerCell = headerLayout.AddCell("Средняя закупочная цена, рублей за литр");
            GridHeaderCell headerCell1 = headerCell.AddCell(MDXDateToShortDateString(compareDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerCell = headerLayout.AddCell("Средняя розничная цена, рублей за литр");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(compareDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerCell = headerLayout.AddCell("Разница между средней розничной ценой и средней закупочной ценой, рублей за литр");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(compareDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerCell = headerLayout.AddCell("Торговая надбавка, %");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(compareDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerLayout.ApplyHeaderInfo();

            // Настройка колонок
            UltraGridBand band = e.Layout.Bands[0];
            band.Columns[0].Width = CRHelper.GetColumnWidth(120);
            band.Columns[0].CellStyle.Wrap = true;
            for (int i = 1; i < band.Columns.Count; ++i)
            {
                int columnWidth = 60;
                band.Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
                band.Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                band.Columns[i].CellStyle.Padding.Right = 5;
                band.Columns[i].CellStyle.Padding.Left = 5;
                if ((i - 1) % 4 < 2)
                {
                    band.Columns[i].CellStyle.ForeColor = Color.Gray;
                }
            }
            band.Columns[0].MergeCells = true;
        }

        protected string GetCellFormatString(int row, int column)
        {
            if (row % 3 == 1 || column > UltraWebGrid.Columns.Count - 5)
                return "{0:P2}";
            else
                return "{0:N2}";
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
            UltraWebGrid grid = sender as UltraWebGrid;

            if ((e.Row.Cells[0].Text.Contains("Газ сжиженный углеводородный для заправки автотранспортных средств")) && (ComboRegion.SelectedValue.Contains("Белоярский муниципальный район")))
            {
                e.Row.Cells[0].Style.BackgroundImage = "~/images/cornerGreen.gif";
                e.Row.Cells[0].Title = "Единица измерения: куб.метр";
                e.Row.Cells[0].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; margin: 2px";
                for (int i = 2; i < e.Row.Cells.Count; i +=2)
                {
                    e.Row.Cells[i].Value = null;
                }

            }

            foreach (UltraGridCell cell in e.Row.Cells)
            {
                if (cell.Column.Index > 0 && cell.Value != null)
                {
                    if (cell.Row.Index % 3 == 2)
                        if (Convert.ToDouble(cell.GetText()) > 0.0001)
                        {
                            SetImageFromCell(grid.Rows[e.Row.Index - 2].Cells[cell.Column.Index], "ArrowRedUpBB.png");
                        }
                        else if (Convert.ToDouble(cell.GetText()) < -0.0001)
                        {
                            SetImageFromCell(grid.Rows[e.Row.Index - 2].Cells[cell.Column.Index], "ArrowGreenDownBB.png");
                        }
                    cell.Value = String.Format(GetCellFormatString(cell.Row.Index, cell.Column.Index), Convert.ToDouble(cell.GetText()));
                }
                SetCellHint(cell);
            }
        }

        protected void SetCellHint(UltraGridCell cell)
        {
            int row = cell.Row.Index;
            int column = cell.Column.Index;
            if (row % 3 == 1 && (column == 3 || column == 4 || column == 7 || column == 8 || column == 11 || column == 12))
            {
                cell.Title = String.Format("Изменение в % к {0} года", MDXDateToShortDateString(compareDate.Value));
            }
            if (row % 3 == 2 && (column == 3 || column == 4 || column == 7 || column == 8 || column == 11 || column == 12 || column == 15 || column == 16))
            {
                cell.Title = String.Format("Изменение в руб. к {0} года", MDXDateToShortDateString(compareDate.Value));
            }
        }

        #endregion

        // --------------------------------------------------------------------

        #region Обработчики диаграммы 1

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {


            if ((selectedFuel.Value.Contains("Газ сжиженный углеводородный для заправки автотранспортных средств")) && (ComboRegion.SelectedValue.Contains("Белоярский муниципальный район")))
            {
                Chart1TitleCaption = "Динамика средней закупочной и средней розничной цены по виду нефтепродукта «{0}», рублей за куб. метр";
            }

            LabelChart1.Text = String.Format(Chart1TitleCaption, selectedFuel.Value);
            fuel.Value = selectedFuel.Value;
            UltraChart1.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>\n<b><DATA_VALUE:N2></b>, рублей за " + (ComboRegion.SelectedValue.Contains("Белоярский муниципальный район") ? "куб.метр" : "литр");
            if (dtDate != null)
            {
                dtChart1 = new DataTable();
                dtChart1.Columns.Add("Дата", typeof(string));
                dtChart1.Columns.Add("Закупочная по МО", typeof(double));
                dtChart1.Columns.Add("Закупочная по ХМАО", typeof(double));
                dtChart1.Columns.Add("Розничная по МО", typeof(double));
                dtChart1.Columns.Add("Розничная по ХМАО", typeof(double));

                int start = dtDate.Rows.Count < 36 ? 0 : dtDate.Rows.Count - 36;
                for (int i = start; i < dtDate.Rows.Count; ++i)
                {
                    object byHMAO, byRegion;
                    string year = dtDate.Rows[i][0].ToString();
                    string month = dtDate.Rows[i][3].ToString();
                    string day = dtDate.Rows[i][4].ToString();
                    date.Value = StringToMDXDate(day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года");
                    price.Value = "Закупочная цена";
                    string query = DataProvider.GetQueryText("ORG_0001_0003_fuel_data");
                    DataTable dtFuelData = new DataTable();
                    DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные по одному виду топлива", dtFuelData);
                    ParseFuelDataMath(dtFuelData, region.Value, out byHMAO, out byRegion);
                    DataRow row = dtChart1.NewRow();
                    row[0] = MDXDateToShortDateString(date.Value);
                    row[1] = byRegion;
                    row[2] = byHMAO;
                    price.Value = "Розничная цена";
                    query = DataProvider.GetQueryText("ORG_0001_0003_fuel_data");
                    dtFuelData = new DataTable();
                    DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные по одному виду топлива", dtFuelData);
                    ParseFuelDataMath(dtFuelData, region.Value, out byHMAO, out byRegion);
                    row[3] = byRegion;
                    row[4] = byHMAO;
                    dtChart1.Rows.Add(row);
                }

                UltraChart1.Series.Clear();
                //return;
                //if (
                //    (selectedFuel.Value.Contains("Газ сжиженный углеводородный для заправки автотранспортных средств")) &&
                //    (ComboRegion.SelectedValue.Contains("Белоярский муниципальный район")))
                //{
                //    dtChart1.Columns.Remove("Закупочная по ХМАО");
                //    dtChart1.Columns.Remove("Розничная по ХМАО");

                //}

                //bool b = true;
                    //= (ComboRegion.SelectedValue.Contains("Белоярский муниципальный район") && selectedFuel.Value.Contains("Газ сжиженный углеводородный для заправки автотранспортных средств"));

                for (int i = 1; i < dtChart1.Columns.Count; i++)
                {

                    if (ComboRegion.SelectedValue.Contains("Белоярский муниципальный район") && selectedFuel.Value.Contains("Газ сжиженный углеводородный для заправки автотранспортных средств"))
                    //if (b)
                    {
                        if (!((i != 1) && (i != 3)))
                        {
                            NumericSeries series = CRHelper.GetNumericSeries(i, dtChart1);
                            series.Label = dtChart1.Columns[i].ColumnName;
                            UltraChart1.Series.Add(series);
                        }
                        
                    }
                    else
                    {
                        NumericSeries series = CRHelper.GetNumericSeries(i, dtChart1);
                        series.Label = dtChart1.Columns[i].ColumnName;
                        UltraChart1.Series.Add(series);
                    }
                }
            }
            else
            {
                UltraChart1.DataSource = null;
            }
        }

        protected void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
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
        /// Разбирает таблицу с данными. На выходе: арифметическое среднее по ХМАО, геометрическое среднее по МО
        /// </summary>
        /// <param name="dtFuelData">Таблица с данными</param>
        /// <param name="region">Наименование МО</param>
        /// <param name="byHMAO">Геометрическое среднее по ХМАО</param>
        /// <param name="byRegion">Геометрическое среднее по МО</param>
        protected void ParseFuelDataMath(DataTable dtFuelData, string region, out object byHMAO, out object byRegion)
        {
            double[] gmValues = new double[dtFuelData.Rows.Count];
            double localByRegion = 0.0;
            for (int i = 0; i < dtFuelData.Rows.Count; ++i)
            {
                DataRow row = dtFuelData.Rows[i];
                int count = 0;
                gmValues[i] = 0.0;
                for (int j = 1; j < row.ItemArray.Length; ++j)
                {
                    object cell = row[j];
                    if (cell != DBNull.Value)
                    {
                        gmValues[i] = gmValues[i] + Convert.ToDouble(cell);
                        ++count;
                    }
                }
                gmValues[i] = gmValues[i] / count;
                if (row[0].ToString() == region)
                {
                    localByRegion = gmValues[i];
                }
            }
            double localByHMAO = 0.0;
            foreach (double value in gmValues)
            {
                localByHMAO += value;
            }
            localByHMAO = gmValues.Length != 0 ? localByHMAO / gmValues.Length : 0.0;
            if (localByRegion == 0)
                byRegion = DBNull.Value;
            else
                byRegion = localByRegion;
            if (localByHMAO == 0)
                byHMAO = DBNull.Value;
            else
                byHMAO = localByHMAO;
        }

        /// <summary>
        /// Разбирает таблицу с данными. На выходе: геометрическое среднее по ХМАО, геометрическое среднее по МО
        /// </summary>
        /// <param name="dtFuelData">Таблица с данными</param>
        /// <param name="region">Наименование МО</param>
        /// <param name="byHMAO">Геометрическое среднее по ХМАО</param>
        /// <param name="byRegion">Геометрическое среднее по МО</param>
        protected void ParseFuelData(DataTable dtFuelData, string region, out object byHMAO, out object byRegion)
        {
            double[] gmValues = new double[dtFuelData.Rows.Count];
            double localByRegion = 0.0;
            for (int i = 0; i < dtFuelData.Rows.Count; ++i)
            {
                DataRow row = dtFuelData.Rows[i];
                int count = 0;
                gmValues[i] = 1.0;
                for (int j = 1; j < row.ItemArray.Length; ++j)
                {
                    object cell = row[j];
                    if (cell != DBNull.Value)
                    {
                        gmValues[i] = gmValues[i] * Convert.ToDouble(cell);
                        ++count;
                    }
                }
                gmValues[i] = Math.Pow(gmValues[i], 1.0 / count);
                if (row[0].ToString() == region)
                {
                    localByRegion = gmValues[i];
                }
            }
            double localByHMAO = 1.0;
            foreach (double value in gmValues)
            {
                localByHMAO *= value;
            }
            localByHMAO = Math.Pow(localByHMAO, 1.0 / gmValues.Length);
            if (localByRegion == 0)
                byRegion = DBNull.Value;
            else
                byRegion = localByRegion;
            if (localByHMAO == 1)
                byHMAO = DBNull.Value;
            else
                byHMAO = localByHMAO;
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


        class SortDataRow : System.Collections.Generic.IComparer<DataRow>
        {
            #region Члены IComparer<RegionValue>

            public int Compare(DataRow x, DataRow y)
            {
                return -Compare_(x, y);
            }

            public int Compare_(DataRow x, DataRow y)
            {
                string Xname = x[4].ToString();
                string Yname = y[4].ToString();

                if (Xname == Yname)
                {
                    return 0;
                }

                if (Xname == "Ханты-Мансийский автономный округ")
                {
                    return 1;
                }

                if (Yname == "Ханты-Мансийский автономный округ")
                {
                    return -1;
                }

                if (Xname.Contains("Город Ханты-Мансийск"))
                {
                    return 1;
                }

                if (Yname.Contains("Город Ханты-Мансийск"))
                {
                    return -1;
                }
                if ((Xname[0] == 'Г') && (Yname[0] != 'Г'))
                {
                    return 1;
                }

                if ((Xname[0] != 'Г') && (Yname[0] == 'Г'))
                {
                    return -1;
                }


                return Yname.CompareTo(Xname);
            }

            #endregion
        }

        DataTable SortTable(DataTable Table)
        {
            DataTable TableSort = new DataTable();

            foreach (DataColumn col in Table.Columns)
            {
                TableSort.Columns.Add(col.ColumnName, col.DataType);
            }

            List<DataRow> LR = new System.Collections.Generic.List<DataRow>();

            foreach (DataRow row in Table.Rows)
            {
                LR.Add(row);
            }

            LR.Sort(new SortDataRow());



            foreach (DataRow Row in LR)
            {
                TableSort.Rows.Add(Row.ItemArray);
            }
            return TableSort;
        }

        protected void FillComboRegion(CustomMultiCombo combo, string queryName)
        {
            dtRegion = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForPivotTable(query, dtRegion);

            dtRegion = SortTable(dtRegion);

            Dictionary<string, int> dictRegion = new Dictionary<string, int>();
            for (int row = 0; row < dtRegion.Rows.Count; ++row)
            {
                string region = SetStarChar(dtRegion.Rows[row][4].ToString());
                AddPairToDictionary(dictRegion, region, 0);
            }
            combo.FillDictionaryValues(dictRegion);
            if (dtRegion != null)
            {
                combo.SetСheckedState(dtRegion.Rows[0][4].ToString(), true);
            }
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

        public string GetYearFromMDXDate(string mdxDate)
        {
            string[] separator = { "].[" };
            string[] mdxDateElements = mdxDate.Split(separator, StringSplitOptions.None);
            if (mdxDateElements.Length == 8)
            {
                return mdxDateElements[3];
            }
            else
            {
                return "2010";
            }
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();

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
            for (int i = 1; i < grid.Rows.Count; i += 3)
            {
                UltraGridCell cell0 = grid.Rows[i - 1].Cells[0];
                UltraGridCell cell1 = grid.Rows[i].Cells[0];
                UltraGridCell cell2 = grid.Rows[i + 1].Cells[0];

                cell0.Style.BorderDetails.StyleBottom = BorderStyle.None;
                cell0.Value = null;

                cell1.Style.BorderDetails.StyleTop = BorderStyle.None;
                cell1.Style.BorderDetails.StyleBottom = BorderStyle.None;

                cell2.Style.BorderDetails.StyleTop = BorderStyle.None;
                cell2.Value = null;
            }


            grid.Columns.Add("Безымяный столбик");
            foreach (UltraGridRow Row in grid.Rows)
            {
                if (Row.Index % 3 == 0)
                {
                    Row.Cells.FromKey("Безымяный столбик").Value = "Значение";
                    Row.NextRow.Cells.FromKey("Безымяный столбик").Value = "Темп прироста";
                    Row.NextRow.NextRow.Cells.FromKey("Безымяный столбик").Value = "Абсолютное отклонение";
                }
                Row.Cells[0].Style.CustomRules = "";
                Row.Cells[0].Style.BackgroundImage = "";

            }
            headerLayout = new GridHeaderLayout(UltraWebGrid);

            GridHeaderCell Cell = headerLayout.AddCell("Вид топлива");
            Cell.AddCell("   ").SpanY = 2;
            Cell.AddCell("  ").SpanY = 2;

            GridHeaderCell headerCell = headerLayout.AddCell("Средняя закупочная цена, рублей за литр");
            GridHeaderCell headerCell1 = headerCell.AddCell(MDXDateToShortDateString(compareDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerCell = headerLayout.AddCell("Средняя розничная цена, рублей за литр");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(compareDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerCell = headerLayout.AddCell("Разница между средней розничной ценой и средней закупочной ценой, рублей за литр");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(compareDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerCell = headerLayout.AddCell("Процент торговой надбавки");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(compareDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerLayout.ApplyHeaderInfo();


            grid.Columns.FromKey("Безымяный столбик").Move(1);
            grid.Columns.FromKey("Безымяный столбик").Width = 180;


            ReportPDFExporter1.HeaderCellHeight = 25;
            ReportPDFExporter1.Export(headerLayout, section1);

            if (ComboRegion.SelectedValue.Contains("Белояр"))
            {
                IText t = section1.AddText();
                t.AddContent("  ");

                ITable tabletext = section1.AddTable();

                ITableRow tr = tabletext.AddRow();

                ITableCell tc = tr.AddCell();
                tc.Width = new FixedWidth(30);



                IImage ima = tc.AddImage(new Infragistics.Documents.Reports.Graphics.Image(Server.MapPath("~/images/cornerGreen.gif")));

                IText text = tr.AddCell().AddText();

                text.AddContent(" - по Белоярскому району цены на газ сжиженный углеводородный для заправки автотранспортных средств указаны в куб. метрах");
            }

            UltraChart1.Width = LabelChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.8);
            ReportPDFExporter1.Export(UltraChart1, LabelChart1.Text, section2);


        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");

            SetExportGridParams(headerLayout.Grid);

            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
            ReportExcelExporter1.TitleStartRow = 0;

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);

            sheet1.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
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

            for (int i = 6; i < UltraWebGrid.Rows.Count + 6; i += 3)
            {
                for (int j = 0; j < UltraWebGrid.Columns.Count; ++j)
                {
                    sheet1.Rows[i].Cells[j].CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
                    sheet1.Rows[1 + i].Cells[j].CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
                    sheet1.Rows[1 + i].Cells[j].CellFormat.TopBorderStyle = CellBorderLineStyle.None;
                    sheet1.Rows[2 + i].Cells[j].CellFormat.TopBorderStyle = CellBorderLineStyle.None;
                }
            }


            for (int i = 5; i < UltraWebGrid.Rows.Count + 5; i++)
            {
                try
                {
                    if (ComboRegion.SelectedValue.Contains("Белоярский"))
                        if (sheet1.Rows[i].Cells[0].Value.ToString().Contains("Газ сжиженный углеводор"))
                        {
                            //CRHelper.SaveToErrorLog("Белоярский");
                            sheet1.Rows[i].Cells[0].Comment = new WorksheetCellComment();
                            sheet1.Rows[i].Cells[0].Comment.Visible = true;
                            sheet1.Rows[i].Cells[0].Comment.Text = new FormattedString("Единица измерения: куб.метр");
                        }
                }
                catch { }
            }

            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;
            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.6);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6);
            ReportExcelExporter1.Export(UltraChart1, LabelChart1.Text, sheet2, 1);
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

        private void SetExportGridParams(UltraWebGrid grid)
        {
            string exportFontName = "Verdana";
            int fontSize = 10;
            double coeff = 0.9;

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
            headerLayout = new GridHeaderLayout(UltraWebGrid);

            GridHeaderCell Cell = headerLayout.AddCell("Вид топлива");
            Cell.AddCell("");
            Cell.AddCell("");

            GridHeaderCell headerCell = headerLayout.AddCell("Средняя закупочная цена, рублей за литр");
            GridHeaderCell headerCell1 = headerCell.AddCell(MDXDateToShortDateString(compareDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerCell = headerLayout.AddCell("Средняя розничная цена, рублей за литр");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(compareDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerCell = headerLayout.AddCell("Разница между средней розничной ценой и средней закупочной ценой, рублей за литр");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(compareDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerCell = headerLayout.AddCell("Процент торговой надбавки");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(compareDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerCell1 = headerCell.AddCell(MDXDateToShortDateString(selectedDate.Value));
            headerCell1.AddCell("МО");
            headerCell1.AddCell("Округ");
            headerLayout.ApplyHeaderInfo();


            grid.Columns.FromKey("Безымяный столбик").Move(1);
            grid.Columns.FromKey("Безымяный столбик").Width = 180;


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
