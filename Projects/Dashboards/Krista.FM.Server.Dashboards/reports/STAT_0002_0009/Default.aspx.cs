using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using System.Drawing;
using Infragistics.UltraChart.Shared.Styles;
using System.Collections.Generic;
using Infragistics.WebUI.UltraWebNavigator;

namespace Krista.FM.Server.Dashboards.reports.STAT_0002_0009
{
    public enum SliceType
    {
        OKVED,
        OKOPF,
        OKFS
    }

    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable chartDt = new DataTable();
        private DataTable gridDt = new DataTable();
        private DateTime currentDate;
        private DateTime firstDate;
        private DateTime prevDate;
        private static MemberAttributesDigest periodDigest;
        private int firstYear = 2005;
        private CustomParam currentPeriod;
        private CustomParam firstPeriod;
        private CustomParam lastPeriod;
        private CustomParam Finance;
        private CustomParam kind;
        private CustomParam rowset;
        private CustomParam columnset;
        private CustomParam face;
        private GridHeaderLayout headerLayout;
        private UltraGridRow gridrow;
        private string defaultDateInCombo = string.Empty;
        #endregion

        #region Параметры запроса

        // множество для среза данных
        private CustomParam sliceSet;
        private CustomParam num;

        #endregion


        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида
            UltraWebGrid.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.28);
            UltraWebGrid.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            UltraWebGrid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);
            //UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(Grid_ActiveRowChange);
            UltraWebGrid.DataBinding += new System.EventHandler(UltraWebGrid_DataBinding);

            #endregion

            #region Настройка диаграммы динамики
            UltraChart1.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            UltraChart1.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.65 - 100);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.ChartType = ChartType.LineChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.Y.Extent = 50;
            UltraChart1.Axis.X.Extent = 43;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            UltraChart1.LineChart.NullHandling = NullHandling.InterpolateSimple;
            UltraChart1.Tooltips.FormatString = "<SERIES_LABEL> \n по состоянию на <ITEM_LABEL> г.\n<b><DATA_VALUE:N1></b> %";
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
            UltraChart1.Axis.X.Margin.Near.Value = 20;
            UltraChart1.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
            UltraChart1.Axis.X.Margin.Far.Value = 20;
            UltraChart1.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
            UltraChart1.Axis.Y.Margin.Far.Value = 5;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 20;
            UltraChart1.Legend.Font = new Font("Microsoft Sans Serif", 9);
            UltraChart1.Data.SwapRowsAndColumns = true;
            UltraChart1.Legend.Visible = true;

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
            UltraChart2.Height = CRHelper.GetChartHeight(350);
            UltraChart2.ChartType = ChartType.LineChart;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.Axis.Y.Extent = 50;
            UltraChart2.Axis.X.Extent = 43;
            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            UltraChart2.Tooltips.FormatString = "<SERIES_LABEL> \n по состоянию на <ITEM_LABEL> г.\n<b><DATA_VALUE:N1></b> млн.руб.";
            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Font = new Font("Verdana", 8);
            UltraChart2.Legend.SpanPercentage = 14;
            UltraChart2.Legend.Location = LegendLocation.Bottom;
            UltraChart2.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
            UltraChart2.Axis.X.Margin.Near.Value = 20;
            UltraChart2.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
            UltraChart2.Axis.X.Margin.Far.Value = 20;
            UltraChart2.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
            UltraChart2.Axis.Y.Margin.Far.Value = 5;
            UltraChart2.Axis.X.Labels.Font = new Font("Verdana", 07);
            UltraChart2.Axis.X.Labels.WrapText = true;
            UltraChart2.Axis.X.Labels.FontColor = Color.Black;
            UltraChart2.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 12);
            UltraChart2.Axis.Y.Labels.FontColor = Color.Black;
            UltraChart2.Axis.X.Labels.Visible = true;
            UltraChart2.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart2.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart2.Data.SwapRowsAndColumns = true;
            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.Font = new Font("Verdana", 8);
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion

            #region Инициализация параметров запроса

            currentPeriod = UserParams.CustomParam("current_period");
            firstPeriod = UserParams.CustomParam("first_period");
            lastPeriod = UserParams.CustomParam("last_period");
            Finance = UserParams.CustomParam("finance");
            kind = UserParams.CustomParam("kind");
            currentPeriod = UserParams.CustomParam("current_period");
            lastPeriod = UserParams.CustomParam("last_period");
            rowset = UserParams.CustomParam("rowset");
            columnset = UserParams.CustomParam("columnset");
            face = UserParams.CustomParam("face");
            num = UserParams.CustomParam("num");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }
        DateTime lastDate;
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                ComboYear.PanelHeaderTitle = "Выберите текущую дату";
                ComboYear.Title = "Выберите текущую дату";
                ComboYear.Width = 335;
                ComboYear.ParentSelect = false;
                ComboYear.ShowSelectedValue = true;
                ComboYear.MultiSelect = false;
                periodDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "STAT_0002_0009_Date");
                FillComboDate(ComboYear, "STAT_0002_0009_list_of_dates", 0);
                ComboYear.SelectLastNode();

                ComboYearChart.PanelHeaderTitle = "Выберите начальную дату";
                ComboYearChart.Title = "Выберите начальную дату";
                ComboYearChart.Width = 345;
                ComboYearChart.ParentSelect = false;
                ComboYearChart.ShowSelectedValue = true;
                ComboYearChart.MultiSelect = false;
                currentPeriod.Value = StringToMDXDate(ComboYear.SelectedValue);
                FillComboDate(ComboYearChart, "STAT_0002_0009_list_of_dates_first", 1);
            
                chartWebAsyncPanel.AddRefreshTarget(UltraChart1);
                chartWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid.ClientID);
                radioGroupPanel.AddRefreshTarget(UltraChart2);
                num.Value = Convert.ToString(32);
            }
            if (Page.IsPostBack)
            {
                num.Value = Convert.ToString(54);
            }
            currentPeriod.Value = StringToMDXDate(ComboYear.SelectedValue);
            string oldSelected = string.Empty;
            oldSelected = ComboYearChart.SelectedValue;
            ComboYearChart.ClearNodes();
            FillComboDate(ComboYearChart, "STAT_0002_0009_list_of_dates_first", 1);
            if (oldSelected != string.Empty)
            {
                ComboYearChart.SetСheckedState(oldSelected, true);
            }
            string periodUniqueName = string.Empty;
            string firstPeriodUniqueName = string.Empty;
            string prevPeriodUniqueName = string.Empty;
            Page.Title = String.Format("Анализ основных показателей развития банковского сектора");
            string level = string.Empty;
            switch (ComboYear.SelectedNode.Level)
            {
                case 0:
                    {
                        periodUniqueName = StringToMDXDate(ComboYear.GetLastChild(ComboYear.SelectedNode).FirstNode.Text);
                        break;
                    }
                case 1:
                    {
                        periodUniqueName = StringToMDXDate(ComboYear.SelectedNode.FirstNode.Text);
                        break;
                    }
                case 2:
                    {
                        periodUniqueName = StringToMDXDate(ComboYear.SelectedNode.Text);
                        break;
                    }
            }
            if (ComboYearChart.SelectedNode != null)
            {
                switch (ComboYearChart.SelectedNode.Level)
                {
                    case 0:
                        {
                            firstPeriodUniqueName = StringToMDXDate(ComboYearChart.GetLastChild(ComboYearChart.SelectedNode).FirstNode.Text);
                            break;
                        }
                    case 1:
                        {
                            firstPeriodUniqueName = StringToMDXDate(ComboYearChart.SelectedNode.FirstNode.Text);
                            break;
                        }
                    case 2:
                        {
                            firstPeriodUniqueName = StringToMDXDate(ComboYearChart.SelectedNode.Text);
                            break;
                        }
                }
            }
            else
            {
                firstPeriodUniqueName = periodUniqueName;
            }
            for (int i = 0; i <= 5; ++i)
            {
                PaintElement pe = new PaintElement();
                switch (i)
                {
                    case 0:
                        {
                            pe.Fill = Color.Green;
                            break;
                        }
                    case 1:
                        {
                            pe.Fill = Color.Blue;
                            break;
                        }
                    case 2: 
                        { 
                            pe.Fill = Color.Red;
                            break;
                        }
                    case 3: 
                        { 
                            pe.Fill = Color.Maroon;
                            break;
                        }
                    case 4: 
                        { 
                            pe.Fill = Color.Orange;
                            break;
                        }
                    case 5: 
                        { 
                            pe.Fill = Color.Violet;
                            break;
                        }
                } 
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
                LineAppearance lineAppearance = new LineAppearance();
                lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
                lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
                lineAppearance.IconAppearance.PE = pe;
                UltraChart1.LineChart.LineAppearances.Add(lineAppearance);
                UltraChart2.LineChart.LineAppearances.Add(lineAppearance);
            }
            currentPeriod.Value = periodUniqueName;
            firstPeriod.Value = firstPeriodUniqueName;
            string query = DataProvider.GetQueryText("STAT_0002_0009_prev_date");
            DataTable dtPrevDate = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtPrevDate);
            prevPeriodUniqueName = dtPrevDate.Rows[0][1].ToString();
            currentDate = CRHelper.PeriodDayFoDate(periodUniqueName);
            prevDate = CRHelper.PeriodDayFoDate(prevPeriodUniqueName);
            firstDate = CRHelper.PeriodDayFoDate(firstPeriodUniqueName);
            prevDate = prevDate.AddDays(1);
            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            Label1.Text = Page.Title;
            Label2.Text = String.Format("Еженедельный мониторинг основных показателей, характеризующих развитие банковского сектора, Ханты-Мансийский автономный округ-ЮГРА, по состоянию на {0:dd.MM.yyyy} года", currentDate, currentDate, prevDate);
            //ChartCaption2.Text = String.Format("Соотношение объемов вкладов физических лиц в СБРФ и коммерческие банки, млн.руб.");
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            //string periodUniqueName = periodDigest.GetMemberUniqueName(ComboYear.SelectedValue);
            currentDate = CRHelper.PeriodDayFoDate(periodUniqueName);
            lastDate = CRHelper.PeriodDayFoDate(prevPeriodUniqueName);
            string lastPeriodUniqueName = periodDigest.GetMemberUniqueName(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(lastDate.Month)));
            lastPeriod.Value = prevPeriodUniqueName;
            DateTime nextDayDate = currentDate;
            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodLastYear.Value = (currentDate.Year - 1).ToString();
            string lface = string.Empty;
            if (RadioButtonList1.SelectedValue == "Кредиты")
            {
                HeaderCaption.Text = String.Format("Информация о процентных ставках и объеме кредитования юридических и физических лиц, за отчётный период {1:dd.MM.yyyy} - {0:dd.MM.yyyy} г.", currentDate, prevDate);
                RadioButtonList2.Visible = true;
                RadioButtonList3.Visible = false;
                rowset.Value = "Кредиты";
                columnset.Value = "Показатели Кредиты";
                    if (RadioButtonList2.SelectedValue == "Юридические лица")
                    {
                        face.Value = "Кредиты юр. лицам";
                    }
                    else
                    {
                        face.Value = "Кредиты физ. лицам";
                    }
                    if (RadioButtonList2.SelectedValue == "Юридические лица")
                {
                    lface = "юридическим лицам";
                }
                else
                {
                    lface = "физическим лицам";
                }
                    ChartCaption1.Text = string.Format("Динамика минимальных/максимальных процентных ставок по вновь выданным кредитам {0} за период с {2:dd.MM.yyyy} по {1:dd.MM.yyyy} г., %", lface, currentDate, firstDate);
                    ChartCaption2.Text = string.Format("Динамика объема выданных и погашенных кредитов {0} за период с {2:dd.MM.yyyy} по {1:dd.MM.yyyy} г., млн.руб.", lface, currentDate, firstDate);
            }
            else
            {
                HeaderCaption.Text = String.Format("Информация об объеме и процентных ставках по депозитам и расчетным счетам юридических лиц и вкладам физических лиц, за отчётный период {1:dd.MM.yyyy} - {0:dd.MM.yyyy} г.", currentDate, prevDate);
                RadioButtonList2.Visible = false;
                RadioButtonList3.Visible = true;
                rowset.Value = "Депозиты";
                columnset.Value = "Показатели Депозиты";
                columnset = UserParams.CustomParam("columnset");
                face.Value = RadioButtonList3.SelectedValue;
                if (RadioButtonList3.SelectedValue == "Депозиты юр. лиц")
                {
                    lface = "депозитам юр. лиц ";
                    ChartCaption1.Text = string.Format("Динамика минимальных/максимальных процентных ставок по вновь привлеченным {0} за период с {2:dd.MM.yyyy} по {1:dd.MM.yyyy} г., %", lface, currentDate, firstDate);
                    lface = "депозитов юр. лиц ";
                    ChartCaption2.Text = string.Format("Динамика объема денежных средств поступивших и снятых с {0} за период с {2:dd.MM.yyyy} по {1:dd.MM.yyyy} г., млн.руб.", lface, currentDate, firstDate);
                }
                else if (RadioButtonList3.SelectedValue == "Вклады физ. лиц (включая карточные счета)")
                {
                    lface = "вкладам физ. лиц (включая карточные счета)";
                    ChartCaption1.Text = string.Format("Динамика минимальных/максимальных процентных ставок по вновь привлеченным {0} за период с {2:dd.MM.yyyy} по {1:dd.MM.yyyy} г., %", lface, currentDate, firstDate);
                    lface = "вкладов физ. лиц (включая карточные счета)";
                    ChartCaption2.Text = string.Format("Динамика объема денежных средств поступивших и снятых с {0} за период с {2:dd.MM.yyyy} по {1:dd.MM.yyyy} г., млн.руб.", lface, currentDate, firstDate);
                }
                else
                {
                    lface = "рассчетным счетам клиентов ";
                    ChartCaption1.Text = string.Format("Динамика минимальных/максимальных процентных ставок по вновь привлеченным {0} за период с {2:dd.MM.yyyy} по {1:dd.MM.yyyy} г., %", lface, currentDate, firstDate);
                    lface = "рассчетных счетов клиентов ";
                    ChartCaption2.Text = string.Format("Динамика объема денежных средств поступивших и снятых с {0} за период с {2:dd.MM.yyyy} по {1:dd.MM.yyyy} г., млн.руб.", lface, currentDate, firstDate);
                }
                
            }
            if ((!radioGroupPanel.IsAsyncPostBack) && (!chartWebAsyncPanel.IsAsyncPostBack))
            {
                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();
                string patternValue = string.Empty;
                int defaultRowIndex = 0;
                patternValue = Finance.Value;
                UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, UltraWebGrid.Columns.Count - 1, defaultRowIndex);
                ActiveGridRow(row);
            }
            if ((!radioGroupPanel.IsAsyncPostBack)&&(ComboYearChart.SelectedNode != null))
            {
                ChartDataBind1();
            }
            if ((!chartWebAsyncPanel.IsAsyncPostBack)&&(ComboYearChart.SelectedNode != null))
            {
                ChartDataBind2();
            }
            if (ComboYearChart.SelectedNode == null)
            {
                ChartCaption1.Text = string.Empty;
                ChartCaption2.Text = string.Empty;
            }
            else
            {
                ChartDataBind1();
                ChartDataBind2();
            }
        }

        #region Обработчики грида

        private void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0002_0009_grid");
            gridDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);

            if ((gridDt.Rows.Count > 0)&&(gridDt.Columns.Count > 1))
            {
                if (gridDt.Columns.Count > 6)
                {
                    for (int i = 0; i < gridDt.Rows.Count; i++)
                    {
                        gridDt.Rows[i][gridDt.Columns.Count - 3] = string.Format("{0:N1} - {1:N1}", gridDt.Rows[i][gridDt.Columns.Count - 7], gridDt.Rows[i][gridDt.Columns.Count - 6]);
                        gridDt.Rows[i][gridDt.Columns.Count - 2] = string.Format("{0:N1} - {1:N1}", gridDt.Rows[i][gridDt.Columns.Count - 5], gridDt.Rows[i][gridDt.Columns.Count - 4]);
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        gridDt.Columns.RemoveAt(gridDt.Columns.Count - 4);
                    }
                }
                if (gridDt.Columns.Count > 1)
                {
                    gridDt.Columns.RemoveAt(0);
                }

                UltraWebGrid.DataSource = gridDt;
            }
        }

        private void ActiveGridRow(UltraGridRow row)
        {

        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }
            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].MergeCells = true;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(75);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            headerLayout.AddCell("Наименование");

            // Заголовки
            GridHeaderCell lowcell = headerLayout;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(130);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                if (i == e.Layout.Bands[0].Columns.Count - 3)
                {
                    lowcell = headerLayout.AddCell("Проц. ставка по кредитам (min-max)");
                    lowcell.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption);
                }
                if (i == e.Layout.Bands[0].Columns.Count - 2)
                {
                    lowcell.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption);
                }
                else
                {
                    headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption + ", млн.руб.");
                }

                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
            }

            headerLayout.ApplyHeaderInfo();
        }

        protected void Grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }
        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            int cellCount = e.Row.Cells.Count;

            int type = 0;
            if (e.Row.Cells[cellCount - 1].Value != null)
            {
                type = Convert.ToInt32(e.Row.Cells[cellCount - 1].Value.ToString());
            }

            for (int i = 1; i < cellCount - 1; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
                cell.Style.Padding.Right = 3;
                if (i < cellCount - 3)
                {
                    switch (type)
                    {
                        case 0:
                            {
                                if (cell.Value != null)
                                {
                                    if (UltraWebGrid.Columns[i].Header.Caption.Contains("ставка"))
                                    {
                                        cell.Value = string.Empty;
                                        cell.Style.BorderDetails.WidthTop = 0;
                                        cell.Style.BorderDetails.WidthBottom = 0;
                                        break;
                                    }
                                    cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N2");
                                    cell.Title = string.Format("Абсолютное отклонение к {0:dd.MM.yyyy}г.", lastDate);
                                }
                                cell.Style.BorderDetails.WidthTop = 0;
                                cell.Style.BorderDetails.WidthBottom = 0;
                                break;
                            }
                        case 1:
                            {
                                if (cell.Value != null)
                                {
                                    if (UltraWebGrid.Columns[i].Header.Caption.Contains("ставка"))
                                    {
                                        cell.Value = string.Empty;
                                        break;
                                    }
                                    if (gridDt.Columns[i].Caption.Contains("Задолженность") || gridDt.Columns[i].Caption.Contains("Снято"))
                                    {
                                        double growRate = Convert.ToDouble(cell.Value.ToString());
                                        cell.Value = growRate.ToString("P2");

                                        if (growRate > 0)
                                        {
                                            cell.Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                                            cell.Title = string.Format("Темп прироста к {0:dd.MM.yyyy}г.", lastDate);
                                        }
                                        else if (growRate < 0)
                                        {
                                            cell.Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                                            cell.Title = string.Format("Темп прироста к {0:dd.MM.yyyy}г.", lastDate);
                                        }
                                    }
                                    else
                                    {
                                        double growRate = Convert.ToDouble(cell.Value.ToString());
                                        cell.Value = growRate.ToString("P2");

                                        if (growRate > 0)
                                        {
                                            cell.Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                                            cell.Title = string.Format("Темп прироста к {0:dd.MM.yyyy}г.", lastDate);
                                        }
                                        else if (growRate < 0)
                                        {
                                            cell.Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                                            cell.Title = string.Format("Темп прироста к {0:dd.MM.yyyy}г.", lastDate);
                                        }
                                    }
                                    cell.Style.CustomRules = "background-repeat: no-repeat; background-position:  center; margin: 20px";

                                }
                                cell.Style.BorderDetails.WidthTop = 0;
                                break;
                            }
                        case 2:
                            {
                                if (cell.Value != null)
                                {
                                    cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N2");
                                }
                                cell.Style.BorderDetails.WidthBottom = 0;
                                break;
                            }
                    }
                }
                else
                    switch (type)
                    {
                        case 0:
                            {
                                if (cell.Value != null)
                                {
                                    if (UltraWebGrid.Columns[i].Header.Caption.Contains("ставка"))
                                    {
                                        cell.Value = string.Empty;
                                        cell.Style.BorderDetails.WidthTop = 0;
                                        cell.Style.BorderDetails.WidthBottom = 0;
                                        break;
                                    }

                                }
                                cell.Style.BorderDetails.WidthTop = 0;
                                cell.Style.BorderDetails.WidthBottom = 0;
                                break;
                            }
                        case 1:
                            {
                                if (cell.Value != null)
                                {
                                    if (UltraWebGrid.Columns[i].Header.Caption.Contains("ставка"))
                                    {
                                        cell.Value = string.Empty;
                                        break;
                                    }


                                }
                                cell.Style.BorderDetails.WidthTop = 0;
                                break;
                            }

                    }
            }

        }

        protected void FillComboDate(CustomMultiCombo combo, string queryName, int offset)
        {
            DataTable dtDate = new DataTable();
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
            defaultDateInCombo = dtDate.Rows[dtDate.Rows.Count - 32][4].ToString() + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(dtDate.Rows[dtDate.Rows.Count - 32][3].ToString())) + ' ' + dtDate.Rows[dtDate.Rows.Count - 32][0].ToString() + " года";
            combo.FillDictionaryValues(dictDate);
            combo.SetСheckedState(defaultDateInCombo, true);
        }

        protected void AddPairToDictionary(Dictionary<string, int> dict, string key, int value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }
        }

        public string StringToMDXDate(string str)
        {
            string template = "[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]";
            string[] dateElements = str.Split(' ');
            int year = Convert.ToInt32(dateElements[2]);
            string month = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(CRHelper.MonthNum(dateElements[1])));
            int quarter = CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month));
            int halfYear = CRHelper.HalfYearNumByQuarterNum(quarter);
            int day = Convert.ToInt32(dateElements[0]);
            return String.Format(template, year, halfYear, quarter, month, day);
        }

        #endregion

        #region Обработчики диаграммы

        private void ChartDataBind1()
        {
            string queryText = DataProvider.GetQueryText("STAT_0002_0009_chart1");
            chartDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(queryText, "Показатель", chartDt);
            string query = DataProvider.GetQueryText("Dates1");
            DataTable dtGridDate = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForPivotTable(query, dtGridDate);
            if (chartDt.Rows.Count > 0)
            {
                UltraChart1.Series.Clear();
                foreach (DataRow row in chartDt.Rows)
                {
                    for (int i = 0; i < row.ItemArray.Length; i++)
                    {
                        if (i == 0 && row[i] != DBNull.Value)
                        {
                            row[i] = row[i].ToString().Replace("\"", "'");
                        }
                    }
                }
                for (int i = 1; i < chartDt.Columns.Count; i++)
                {
                    DateTime dateTime = CRHelper.DateByPeriodMemberUName(dtGridDate.Rows[i - 1][4].ToString(), 3);
                    chartDt.Columns[i].ColumnName = string.Format("{0:dd.MM.yy}", dateTime);
                }
                for (int i = 1; i < chartDt.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, chartDt);
                    series.Label = chartDt.Columns[i].ColumnName;
                    UltraChart1.Series.Add(series);
                }
            }

        }

        private void ChartDataBind2()
        {

            string queryText = DataProvider.GetQueryText("STAT_0002_0009_chart2");
            chartDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(queryText, "Показатель", chartDt);
            string query = DataProvider.GetQueryText("Dates1");
            DataTable dtGridDate = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForPivotTable(query, dtGridDate);
            if (chartDt.Rows.Count > 0)
            {
                UltraChart2.Series.Clear();
                foreach (DataRow row in chartDt.Rows)
                {
                    for (int i = 0; i < row.ItemArray.Length; i++)
                    {
                        if (i == 0 && row[i] != DBNull.Value)
                        {
                            row[i] = row[i].ToString().Replace("вкладов", "вкл.");
                            row[i] = row[i].ToString().Replace("депозитов", "депоз.");
                            row[i] = row[i].ToString().Replace("коммерческие", "ком.");
                            row[i] = row[i].ToString().Replace("\"", "'");
                            row[i] = row[i].ToString().Replace(" район", " р-н");
                        }
                    }
                }
                for (int i = 1; i < chartDt.Columns.Count; i++)
                {
                    DateTime dateTime = CRHelper.DateByPeriodMemberUName(dtGridDate.Rows[i - 1][4].ToString(), 3);
                    chartDt.Columns[i].ColumnName = string.Format("{0:dd.MM.yy}", dateTime);
                }
                for (int i = 1; i < chartDt.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, chartDt);
                    series.Label = chartDt.Columns[i].ColumnName;
                    UltraChart2.Series.Add(series);
                }
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;
            SetExportGridParams(headerLayout.Grid);
            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            sheet1.Rows[3].Cells[8].Value = String.Empty;
            sheet1.Rows[4].Cells[8].Value = String.Empty;
            sheet1.Rows[3].Cells[8].CellFormat.FillPattern = FillPatternStyle.None;
            sheet1.Rows[3].Cells[8].CellFormat.BottomBorderColor = Color.LightGray;
            sheet1.Rows[3].Cells[8].CellFormat.LeftBorderColor = Color.LightGray;
            sheet1.Rows[3].Cells[8].CellFormat.RightBorderColor = Color.LightGray;
            sheet1.Rows[3].Cells[8].CellFormat.TopBorderColor = Color.LightGray;
            sheet1.Rows[4].Cells[8].CellFormat.BottomBorderColor = Color.LightGray;
            sheet1.Rows[4].Cells[8].CellFormat.TopBorderColor = Color.LightGray;
            sheet1.Rows[4].Cells[8].CellFormat.RightBorderColor = Color.LightGray;
            sheet1.Rows[4].Cells[8].CellFormat.LeftBorderColor = Color.LightGray;
            sheet1.Rows[4].Cells[8].CellFormat.FillPattern = FillPatternStyle.None;
            sheet1.Rows[2].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
            sheet1.Rows[2].Cells[0].Value = HeaderCaption.Text;
            sheet1.Rows[2].CellFormat.Font.Name = "Verdana";
            for (int i = 0; i < UltraWebGrid.Columns.Count; i++)
            {
                if (i > 1)
                {
                    sheet1.Rows[5].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                }
                sheet1.Rows[5].Cells[i].CellFormat.Font.Name = "Verdana";
                sheet1.Rows[5].Cells[i].CellFormat.Font.Height = 200;
            }
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма1");
            UltraChart1.Width = Convert.ToInt32(UltraChart1.Width.Value * 0.7);
            UltraChart2.Width = Convert.ToInt32(UltraChart2.Width.Value * 0.7);
            ReportExcelExporter1.Export(UltraChart1, ChartCaption1.Text, sheet2, 3);
            Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма2");
            ReportExcelExporter1.Export(UltraChart2, ChartCaption2.Text, sheet3, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void ExportGridSetup()
        {
            for (int i = 0; i < UltraWebGrid.Rows.Count; i++)
            {
                UltraGridCell cell = UltraWebGrid.Rows[i].Cells[0];

                int groupIndex = i % 3;

                switch (groupIndex)
                {
                    case 0:
                        {
                            cell.Value = String.Empty;
                            cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
                            break;
                        }
                    case 1:
                        {
                            cell.Style.BorderDetails.StyleTop = BorderStyle.None;
                            cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
                            break;
                        }
                    case 2:
                        {
                            cell.Value = String.Empty;
                            cell.Style.BorderDetails.StyleTop = BorderStyle.None;
                            break;
                        }
                }
            }
        }
        private void SetExportGridParams(UltraWebGrid grid)
        {
            string exportFontName = "Verdana";
            int fontSize = 10;
            double coeff = 1.0;

            grid.Columns.Add("Безымяный столбик");
            foreach (UltraGridRow Row in grid.Rows)
            {
                if (Row.Index % 3 == 0)
                {
                    Row.Cells.FromKey("Безымяный столбик").Value = "Значение";
                    Row.NextRow.Cells.FromKey("Безымяный столбик").Value = "Абсолютное отклонение";
                    Row.NextRow.NextRow.Cells.FromKey("Безымяный столбик").Value = "Темп прироста";
                }
            }

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            headerLayout.AddCell("Наименование");

            // Заголовки
            GridHeaderCell lowcell = headerLayout;
            headerLayout.AddCell(" "); 
            for (int i = 1; i < UltraWebGrid.Columns.Count - 2; i = i + 1)
            {
                if (i == UltraWebGrid.Columns.Count - 4)
                {
                    lowcell = headerLayout.AddCell("Проц. ставка по кредитам (min-max)");
                    lowcell.AddCell(headerLayout.Grid.Columns[i].Header.Caption);
                }
                if (i == UltraWebGrid.Columns.Count - 3)
                {
                    lowcell.AddCell(headerLayout.Grid.Columns[i].Header.Caption);
                }
                else
                {
                    headerLayout.AddCell(headerLayout.Grid.Columns[i].Header.Caption);
                }

            }

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
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ExportGridSetup();
            //ReportPDFExporter1.PageTitle = Label1.Text;
            //ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            SetExportGridParams(headerLayout.Grid);

            ReportPDFExporter1.HeaderCellHeight = 60;
            Infragistics.Documents.Reports.Report.Text.IText header1 = section1.AddText();
            header1.Style.Font.Name = "Verdana";
            header1.Style.Font.Size = 15;
            header1.Style.Font.Bold = true;
            header1.AddContent(Label1.Text);

            Infragistics.Documents.Reports.Report.Text.IText header2 = section1.AddText();
            header2.Style.Font.Name = "Verdana";
            header2.Style.Font.Size = 13;
            header2.AddContent(Label2.Text);

            Infragistics.Documents.Reports.Report.Text.IText header3 = section1.AddText();
            header3.Style.Font.Name = "Verdana";
            header3.Style.Font.Size = 13;
            header3.AddContent(HeaderCaption.Text);

            ReportPDFExporter1.Export(headerLayout, "", section1);

            ISection section2 = report.AddSection();
            UltraChart1.Width = Convert.ToInt32(UltraChart1.Width.Value * 0.8);
            ReportPDFExporter1.Export(UltraChart1, ChartCaption1.Text, section2);

            ISection section3 = report.AddSection();
            ReportPDFExporter1.Export(UltraChart2, ChartCaption2.Text, section3);
        }

        #endregion
    }
}