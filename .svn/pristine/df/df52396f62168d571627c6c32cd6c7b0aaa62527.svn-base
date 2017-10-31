using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebNavigator;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = System.Drawing.Font;
using Orientation=Infragistics.Documents.Excel.Orientation;

namespace Krista.FM.Server.Dashboards.reports.STAT_0001_0007
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;
        private DataTable dtDebtsGrid;
        private DataTable dtChart;
        private DataTable dtNormalChart;
        private DateTime currDateTime;
        private DateTime lastDateTime;
        private DateTime debtsCurrDateTime;
        private DateTime debtsLastDateTime;

        public bool IsYearJoint()
        {
            return (currDateTime.Year != lastDateTime.Year);
        }

        #endregion

        #region Параметры запроса

        // Текущая дата
        private CustomParam periodCurrentDate;
        // Текущий год
        private CustomParam сurrentYear;
        // Прошлый год
        private CustomParam lastYear;

        // Текущая дата для задолженности
        private CustomParam debtsPeriodCurrentDate;
        // На неделю назад для задолженности
        private CustomParam debtsPeriodLastWeekDate;

        #endregion

        private int GetScreenWidth
        {
            get
            {
                if (Request.Cookies != null)
                {
                    if (Request.Cookies[CustomReportConst.ScreenWidthKeyName] != null)
                    {
                        HttpCookie cookie = Request.Cookies[CustomReportConst.ScreenWidthKeyName];
                        int value = Int32.Parse(cookie.Value);
                        return value;
                    }
                }
                return (int) Session["width_size"];
            }
        }

        private bool IsSmallResolution
        {
            get { return GetScreenWidth < 1200; }
        }

        private int MinScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }

        private int MinScreenHeight
        {
            get { return IsSmallResolution ? 600 : CustomReportConst.minScreenHeight; }
        }
        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(MinScreenWidth - 12);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(MinScreenHeight - 120);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraWebGrid2.Width = CRHelper.GetGridWidth(MinScreenWidth - 12);
            UltraWebGrid2.Height = CRHelper.GetGridHeight(MinScreenHeight - 120);
            UltraWebGrid2.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid2.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraChart.Width = CRHelper.GetChartWidth(MinScreenWidth - 15);
            double chartHeight = IsSmallResolution ? 1.2*MinScreenHeight : MinScreenHeight*0.8;
            UltraChart.Height = CRHelper.GetChartHeight(chartHeight - 110);
            UltraChart.FillSceneGraph +=new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #region Инициализация параметров запроса

            if (periodCurrentDate == null)
            {
                periodCurrentDate = UserParams.CustomParam("period_current_date");
            }
            if (сurrentYear == null)
            {
                сurrentYear = UserParams.CustomParam("current_year");
            }
            if (lastYear == null)
            {
                lastYear = UserParams.CustomParam("last_year");
            }
            if (debtsPeriodCurrentDate == null)
            {
                debtsPeriodCurrentDate = UserParams.CustomParam("period_current_date_debts");
            }
            if (debtsPeriodLastWeekDate == null)
            {
                debtsPeriodLastWeekDate = UserParams.CustomParam("period_last_week_date_debts");
            }

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.StackBarChart;
            UltraChart.StackChart.StackStyle = StackStyle.Complete;
            UltraChart.Border.Thickness = 0;

            UltraChart.Axis.Y.Extent = 230;
            UltraChart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart.Axis.Y.Labels.SeriesLabels.WrapText = true;
            UltraChart.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 8);
            UltraChart.Axis.Y.Labels.SeriesLabels.FontColor = Color.Black;
            UltraChart.Axis.Y.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.None;

            UltraChart.Axis.X.Extent = 40;
            UltraChart.Axis.X.TickmarkStyle = AxisTickStyle.Percentage;
            UltraChart.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>%";
            UltraChart.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart.Axis.X.Labels.FontColor = Color.Black;

            UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE_ITEM:N2>%";

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 14;
            UltraChart.Legend.Margins.Right = IsSmallResolution ? 5 : Convert.ToInt32(UltraChart.Width.Value) / 2;
            UltraChart.Legend.Font = new Font("Verdana", 10);

            UltraChart.Data.SwapRowsAndColumns = false;

            UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart.ColorModel.Skin.ApplyRowWise = false;
            UltraChart.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 6; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.Green;
                            stopColor = Color.ForestGreen;
                            break;
                        }
                    case 2:
                        {
                            color = Color.Yellow;
                            stopColor = Color.Gold;
                            break;
                        }
                    case 3:
                        {
                            color = Color.Gray;
                            stopColor = Color.Black;
                            break;
                        }
                    case 4:
                        {
                            color = Color.LightGray;
                            stopColor = Color.LightSlateGray;
                            break;
                        }
                    case 5:
                        {
                            color = Color.Red;
                            stopColor = Color.Maroon;
                            break;
                        }
                    case 6:
                        {
                            color = Color.LightBlue;
                            stopColor = Color.DodgerBlue;
                            break;
                        }
                }
                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                UltraChart.ColorModel.Skin.PEs.Add(pe);
            }
            
            UltraChart.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion

            UltraGridExporter1.MultiHeader = !IsSmallResolution;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                ComboPeriod.Width = 300;
                ComboPeriod.MultiSelect = false;
                ComboPeriod.ShowSelectedValue = false;
                ComboPeriod.ParentSelect = false;
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillLabourMarketNonEmptyDays(DataDictionariesHelper.LabourMarketNonEmptyDays));
                ComboPeriod.SelectLastNode();
                ComboPeriod.PanelHeaderTitle = "Выберите дату";
            }

            currDateTime = GetDateString(ComboPeriod.GetSelectedNodePath(), ComboPeriod.SelectedNode.Level);

            Node selectedNode = ComboPeriod.SelectedNode;
            // если выбран месяц, то берем в нем последний день
            if (selectedNode.Nodes.Count != 0)
            {
                selectedNode = ComboPeriod.GetLastChild(selectedNode);
            }

            lastDateTime = GetDateString(ComboPeriod.GetPreviousSublingNodePath(selectedNode), selectedNode.Level);

            if (lastDateTime == DateTime.MinValue)
            {
                lastDateTime = currDateTime.AddDays(-7);
            }

            Page.Title = "Анализ вклада субъектов в ситуацию в УрФО";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Format("по состоянию на {0:dd.MM.yyyy}", currDateTime);

            chartElementCaption.Text = "Доля значения субъекта в сумме значений показателей по УрФО";

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", currDateTime, 5);
            сurrentYear.Value = currDateTime.Year.ToString();
            lastYear.Value = (currDateTime.Year - 1).ToString();

            string query = DataProvider.GetQueryText("STAT_0001_0007_debts_date");
            DataTable dtDebtsDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtDebtsDate);

            if (dtDebtsDate.Rows.Count > 1)
            {
                if (dtDebtsDate.Rows[0][1] != DBNull.Value && dtDebtsDate.Rows[0][1].ToString() != string.Empty)
                {
                    debtsPeriodLastWeekDate.Value = dtDebtsDate.Rows[0][1].ToString();
                    debtsLastDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[0][1].ToString(), 3);
                }
                if (dtDebtsDate.Rows[1][1] != DBNull.Value && dtDebtsDate.Rows[1][1].ToString() != string.Empty)
                {
                    debtsPeriodCurrentDate.Value = dtDebtsDate.Rows[1][1].ToString();
                    debtsCurrDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[1][1].ToString(), 3);
                }
            }

            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
            UltraWebGrid2.Bands.Clear();
            UltraWebGrid2.DataBind();

            UltraChart.DataBind();
        }

        public DateTime GetDateString(string source, int level)
        {
            string[] sts = source.Split('|');
            if (sts.Length > 1)
            {
                switch (level)
                {
                    // нулевой уровень выбрать нельзя
                    case 1:
                        {
                            return GetDateString(ComboPeriod.GetNodeLastChild(ComboPeriod.SelectedNode), level + 1);
                        }
                    case 2:
                        {
                            string month = sts[1].TrimEnd(' ');
                            string day = sts[2].TrimEnd(' ');
                            return new DateTime(Convert.ToInt32(sts[0]), CRHelper.MonthNum(month), Convert.ToInt32(day));
                        }
                }
            }
            return DateTime.MinValue;
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = IsSmallResolution ? DataProvider.GetQueryText("STAT_0001_0007_grid_mini") : DataProvider.GetQueryText("STAT_0001_0007_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                DataColumn numberColumn = new DataColumn("№", typeof(string));
                dtGrid.Columns.Add(numberColumn);

                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[0] != DBNull.Value && row[0].ToString().Split(';').Length > 0)
                    {
                        bool isRank = false;
                        string columnName = IsSmallResolution ? "Доля" : "Прирост";
                        string rowString = string.Empty;
                        if ((row[0].ToString().Contains("Уровень регистрируемой безработицы") ||
                             row[0].ToString().Contains("Число зарегистрированных безработных в расчёте на 1 вакансию")) && row[0].ToString().Contains(columnName))
                        {
                            rowString = row[0].ToString().Split(';')[0];
                            row[0] = "ранг по УрФО";
                            isRank = true;
                        }

                        row[0] = row[0].ToString().Split(';')[0];

                        if (DataDictionariesHelper.LabourMarketIndicatorNumbers.ContainsKey(row[0].ToString()) ||
                            (isRank && DataDictionariesHelper.LabourMarketIndicatorNumbers.ContainsKey(rowString)) ||
                            row[0].ToString().Contains("АППГ"))
                        {
                            if (isRank)
                            {
                                row["№"] = DataDictionariesHelper.GetLabourMarketIndicatorNumber(rowString);
                            }
                            else
                            {
                                row["№"] = DataDictionariesHelper.GetLabourMarketIndicatorNumber(row[0].ToString());
                            }

                            if (row[0].ToString().Contains("АППГ"))
                            {
                                row[0] = string.Format("Аналогичный период прошлого года ({0} г.)", currDateTime.Year - 1);
                            }
                        }
                    }
                }

                ((UltraWebGrid)sender).DataSource = dtGrid;
            }
        }

        protected void UltraWebGrid2_DataBinding(object sender, EventArgs e)
        {
            string query = IsSmallResolution ? DataProvider.GetQueryText("STAT_0001_0007_grid_debts_mini") : DataProvider.GetQueryText("STAT_0001_0007_grid_debts");
            dtDebtsGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtDebtsGrid);
            if (dtDebtsGrid.Rows.Count > 0)
            {
                DataColumn numberColumn = new DataColumn("№", typeof(string));
                dtDebtsGrid.Columns.Add(numberColumn);

                foreach (DataRow row in dtDebtsGrid.Rows)
                {
                    if (row[0] != DBNull.Value && row[0].ToString().Split(';').Length > 0)
                    {
                        row[0] = row[0].ToString().Split(';')[0];
                        if (DataDictionariesHelper.LabourMarketIndicatorNumbers.ContainsKey(row[0].ToString()))
                        {
                            row["№"] = DataDictionariesHelper.GetLabourMarketIndicatorNumber(row[0].ToString());
                        }
                    }
                }

                ((UltraWebGrid)sender).DataSource = dtDebtsGrid;
            }
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid2.Height = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.RowAlternateStyleDefault.BackColor = Color.White;
            e.Layout.RowSelectorsDefault = IsSmallResolution ? RowSelectors.No : RowSelectors.Yes;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            DateTime currDT = (sender == UltraWebGrid1) ? currDateTime : debtsCurrDateTime;

            // перемещаем колонку с номером в начало
            UltraGridColumn numberColumn = e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1];
            e.Layout.Bands[0].Columns.RemoveAt(e.Layout.Bands[0].Columns.Count - 1);
            e.Layout.Bands[0].Columns.Insert(0, numberColumn);

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                int widthColumn;
                if (IsSmallResolution)
                {
                    widthColumn = i < e.Layout.Bands[0].Columns.Count - 1 ? 63 : 65;
                }
                else
                {
                    widthColumn = i < e.Layout.Bands[0].Columns.Count - 2 ? 63 : 65;
                }

                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn, 1280);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(20, 1280);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].MergeCells = true;

            int nameColumnWidth = IsSmallResolution ? 200 : 210;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(nameColumnWidth, 1280);
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].MergeCells = true;

            if (IsSmallResolution)
            {
                for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    string caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];
                    caption = string.Format("{0}\n({1:dd.MM})", RegionsNamingHelper.ShortName(caption), currDT);
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, caption, "");
                }
            }
            else
            {
                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    if (i == 0 || i == 1)
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                    }
                    else
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                    }
                }

                int multiHeaderPos = 2;

                for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
                {
                    ColumnHeader ch = new ColumnHeader(true);
                    ch.Caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];

                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, string.Format("{0:dd.MM}", currDT), "");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Доля в УрФО", "Доля значения субъекта в сумме по УрФО");

                    ch.RowLayoutColumnInfo.OriginY = 0;
                    ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                    ch.RowLayoutColumnInfo.SpanY = 1;
                    multiHeaderPos += 2;
                    ch.RowLayoutColumnInfo.SpanX = 2;
                    e.Layout.Bands[0].HeaderLayout.Add(ch);
                }
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int cellsCount = e.Row.Cells.Count;

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                int rowIndex = e.Row.Index;

                // номер показателя
                int groupSize = IsSmallResolution ? 2 : 3;
                int k = (rowIndex % groupSize);
                // строка с уровнем регистрируемой безработицы
                bool isRedundantLevel = (e.Row.Cells[1].Value != null &&
                                         e.Row.Cells[1].Value.ToString().ToLower().Contains("уровень регистрируемой безработицы"));
                bool isRedundantLevelRank = (e.Row.PrevRow != null && e.Row.PrevRow.Cells[1].Value != null &&
                                             e.Row.PrevRow.Cells[1].Value.ToString().ToLower().Contains("уровень регистрируемой безработицы"));
                // строка с рангом уровня регистрируемой безработицы
                bool isRankRow = (e.Row.Cells[1].Value != null &&
                                  e.Row.Cells[1].Value.ToString().Contains("ранг по УрФО"));
                // яркая колонка
                bool bright = IsSmallResolution || (i % 2 != 0);
                bool share = (i % 2 != 0);

                if (!IsSmallResolution)
                {
                    if (i != 0 && bright)
                    {
                        e.Row.Cells[i].Style.BorderDetails.WidthRight = 2;
                    }
                    else
                    {
                        e.Row.Cells[i].Style.BorderDetails.WidthLeft = 2;
                    }
                }

                if (i == 1 && isRankRow && !IsSmallResolution)
                {
                    e.Row.Cells[i].Style.HorizontalAlign = HorizontalAlign.Right;
                    if (e.Row.PrevRow != null && e.Row.PrevRow.PrevRow != null)
                    {
                        e.Row.PrevRow.PrevRow.Cells[i].Style.BorderDetails.WidthBottom = 0;
                    }
                    e.Row.Cells[i].Style.BorderDetails.WidthTop = 0;
                }
                else if (i == 1 && isRankRow)
                {
                    e.Row.Cells[i].Style.HorizontalAlign = HorizontalAlign.Right;
                    if (e.Row.PrevRow != null)
                    {
                        e.Row.PrevRow.Cells[i].Style.BorderDetails.WidthBottom = 0;
                    }
                    e.Row.Cells[i].Style.HorizontalAlign = HorizontalAlign.Right;
                }

                if (i > 1)
                {
                    switch (k)
                    {
                        case 0:
                            {
                                e.Row.Cells[i].Style.BorderDetails.WidthBottom = 0;
                                break;
                            }
                        case 1:
                            {
                                if (!IsSmallResolution)
                                {
                                    e.Row.Cells[i].Style.BorderDetails.WidthTop = 0;
                                    e.Row.Cells[i].Style.BorderDetails.WidthBottom = 0;
                                }
                                break;
                            }
                        case 2:
                            {
                                e.Row.Cells[i].Style.BorderDetails.WidthTop = 0;
                                break;
                            }
                    }
                }

                if (i != 0 && i != 1 && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    double value = Convert.ToDouble(e.Row.Cells[i].Value);

                    bool growRate = (k == 1);
                    bool rate = (!isRedundantLevel && k == 2);
                    bool mlnUnit = (e.Row.Cells[0].Value != null && e.Row.Cells[1].Value.ToString().Contains("млн.руб."));
                    bool thsUnit = (e.Row.Cells[0].Value != null && e.Row.Cells[1].Value.ToString().Contains("Число зарегистрированных безработных в расчёте на 1 вакансию"));

                    if (!IsSmallResolution  && growRate)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            e.Row.Cells[i].Title = "Темп прироста";
                            if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                            }
                            else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            }
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }

                    if (!IsSmallResolution && !isRankRow && rate)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                            {
                                e.Row.Cells[i].Title = (sender == UltraWebGrid1) ? "Прирост к прошлой неделе" : "Прирост к прошлой дате";
                            }
                            else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                            {
                                e.Row.Cells[i].Title = (sender == UltraWebGrid1) ? "Падение относительно прошлой недели" : "Падение относительно прошлой даты";
                            }
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }

                    if (isRankRow)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                                e.Row.Cells[i].Title = isRedundantLevelRank ? "Самый высокий уровень безработицы" : "Самое большое число безработных на 1 вакансию";
                            }
                            else if (Convert.ToInt32(e.Row.Cells[i].Value) == 6)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                                e.Row.Cells[i].Title = isRedundantLevelRank ? "Самый низкий уровень безработицы" : "Самое маленькое число безработных на 1 вакансию";
                            }
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }

                    switch (k)
                    {
                        case 0:
                            {
                                e.Row.Cells[i].Style.Font.Bold = true;
                                if (!IsSmallResolution && share)
                                {
                                    e.Row.Cells[i].Value = string.Format("{0:P2}", value);
                                }
                                else
                                {
                                    if (mlnUnit)
                                    {
                                        e.Row.Cells[i].Value = string.Format("{0:N3}", value);
                                    }
                                    else if (thsUnit)
                                    {
                                        e.Row.Cells[i].Value = string.Format("{0:N2}", value);
                                    }
                                    else if (isRedundantLevel)
                                    {
                                        e.Row.Cells[i].Value = string.Format("{0:N3}%", value);
                                    }
                                    else
                                    {
                                        e.Row.Cells[i].Value = string.Format("{0:N0}", value);
                                    }
                                }
                                break;
                            }
                        case 1:
                            {
                                if (IsSmallResolution && isRankRow)
                                {
                                    e.Row.Cells[i].Value = string.Format("{0:N0}", value);
                                }
                                else
                                {
                                    e.Row.Cells[i].Value = string.Format("{0:P2}", value);
                                }
                                break;
                            }
                        case 2:
                            {
                                if (mlnUnit)
                                {
                                    e.Row.Cells[i].Value = string.Format("{0:N3}", value);
                                }
                                else if (thsUnit)
                                {
                                    e.Row.Cells[i].Value = string.Format("{0:N2}", value);
                                }
                                else
                                {
                                    e.Row.Cells[i].Value = string.Format("{0:N0}", value);
                                }
                                break;
                            }
                    }
                }

                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    string cellValue = cell.Value.ToString();
                    if (cellValue.Contains("%"))
                    {
                        cellValue = cellValue.TrimEnd('%');
                    }

                    decimal value;
                    if (!IsSmallResolution &&i != 0 && (k == 1 || k == 2) && 
                        decimal.TryParse(cellValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out value))
                    {
                        if (value > 0)
                        {
                            cell.Value = string.Format("+{0}", cell.Value);
                        }
                    }

                    e.Row.Cells[i].Style.Padding.Right = (i == 0) ? 1 : 5;

                    if ((!IsSmallResolution && i >= cellsCount - 2) && 
                    (!IsSmallResolution && i >= cellsCount - 1))
                    {
                        cell.Style.Font.Bold = true;
                    }
                }
            }
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0007_chart");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                foreach (DataColumn column in dtChart.Columns)
                {
                    column.ColumnName = column.ColumnName.Replace(
                        "Численность уволенных работников в связи с ликвидацией организаций, сокращением численности или штата работников всего",
                        "Численность уволенных\nработников");
                    column.ColumnName = column.ColumnName.Replace(
                        "Количество работников, предупрежденных о предстоящем увольнении в связи с ликвидацией организаций, сокращением численности или штата работников",
                        "Количество работников, предупрежденных о предстоящем увольнении");
                }
                CRHelper.NormalizeDataTable(dtChart);

                UltraChart.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart.Series.Add(series);
                }

//                UltraChart.DataSource = dtChart;
            }
        }

        void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.Y"))
                {
                    Text text = (Text) primitive;
                    text.bounds.Width = 230;
                    text.bounds.Height = 50;
                    //text.bounds.Offset(0, -10);
                    text.labelStyle.VerticalAlign = StringAlignment.Near;
                    text.labelStyle.Font = new Font("Verdana", 8);
                }

//                if (primitive is Box)
//                {
//                    Box box = (Box)primitive;
//
//                    if (box.Series != null)
//                    {
//                        string value = string.Empty;
//                        if (dtChart != null)
//                        {
//                            if (dtGrid.Rows[box.Row].ItemArray[box.Column + 1] != DBNull.Value &&
//                                dtGrid.Rows[box.Row].ItemArray[box.Column + 1].ToString() != string.Empty)
//                            {
//                                value = Convert.ToDouble(dtGrid.Rows[box.Row].ItemArray[box.Column + 1].ToString()).ToString("N3");
//                            }
//                        }
//                        //box.DataPoint.Label = string.Format("{0}\n{1}", box.DataPoint.Label, value);
//                    }
//                }
//                if (primitive is Box)
//                {
//                    Box box = (Box)primitive;
//                    if (box.DataPoint != null)
//                    {
//                        if (box.DataPoint.Label != null)
//                        {
//                            int columnIndex = box.Column + 1;
//                            int rowIndex = box.Row;
//
//                            if (dtChart != null && dtChart.Rows.Count != 0 &&
//                                dtChart.Rows[rowIndex][columnIndex] != DBNull.Value)
//                            {
//                                double value;
//                                Double.TryParse(dtChart.Rows[rowIndex][columnIndex].ToString(), out value);
//                                //  box.Series.Label = string.Format("{0} {1:N2} тыс.руб.\n доля {2:N2}%", box.Series.Label, value, box.Value);
//                                box.DataPoint.Label =
//                                        string.Format("{0}\n{1}\n{2}\n{3:P2}", box.Series.Label, box.DataPoint.Label, value, box.Value);
//                            }
//                        }
//                    }
//                }
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = string.Format("по состоянию на {0:dd.MM.yyyy}",
                e.CurrentWorksheet == sheet1 ? currDateTime : debtsCurrDateTime);
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid1.Columns.Count;
            int rowsCount = UltraWebGrid1.Rows.Count;

            e.CurrentWorksheet.Columns[0].Width = 20*37;
            e.CurrentWorksheet.Columns[1].Width = 200*37;

            for (int i = 2; i < columnCount; i++)
            {
                e.CurrentWorksheet.Columns[i].Width = 60*37;
            }
            
            // расставляем стили у начальных колонок
            for (int i = 4; i < rowsCount + 4; i++)
            {
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            }

            if (!IsSmallResolution)
            {
                // расставляем стили у ячеек хидера
                for (int i = 1; i < columnCount; i++)
                {
                    e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                    e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                }
            }
        }

        private Workbook workbook;
        private Worksheet sheet1;
        private Worksheet sheet2;

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            workbook = new Workbook();
            sheet1 = workbook.Worksheets.Add("Sheet1");
            sheet2 = workbook.Worksheets.Add("Sheet2");
            
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid1, sheet1);

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid2, sheet2);
        }

        private int offset = 0;

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            UltraGridColumn col = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            while (col.Hidden)
            {
                offset++;
                col = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            }
            e.HeaderText = col.Header.Key.Split(';')[0];
        }

        #endregion

        #region Экспорт в PDF

        private void SetExportGridParams(UltraWebGrid grid)
        {
            bool unit1 = false;
            bool unit4 = false;
            foreach (UltraGridRow row in grid.Rows)
            {
                // номер показателя
                int groupSize = IsSmallResolution ? 2 : 3;
                int k = (row.Index % groupSize);
                UltraGridCell nameCell = row.Cells[1];
                UltraGridCell numberCell = row.Cells[0];
                switch (k)
                {
                    case 0:
                        {
                            if (!unit4 && numberCell.Value != null && numberCell.Value.ToString() == "4")
                            {
                                unit4 = true;
                            }
                            else if (!unit1 && numberCell.Value != null && numberCell.Value.ToString() == "1")
                            {
                                unit1 = true;
                            }
                            else
                            {
                                numberCell.Value = "";
                            }
                            numberCell.Style.BorderDetails.WidthBottom = 0;
                            nameCell.Style.BorderDetails.WidthBottom = 0;
                            break;
                        }
                    case 1:
                        {
                            if (numberCell.Value != null && numberCell.Value.ToString() == "1" ||
                                numberCell.Value != null && numberCell.Value.ToString() == "4")
                            {
                                numberCell.Value = "";
                            }

                            numberCell.Value = "";
                            if (nameCell.Value != null && nameCell.Value.ToString() != "ранг по УрФО")
                            {
                                nameCell.Value = "доля";
                            }
                            
                            nameCell.Style.HorizontalAlign = HorizontalAlign.Right;
                            numberCell.Style.BorderDetails.WidthBottom = 0;
                            numberCell.Style.BorderDetails.WidthTop = 0;
                            nameCell.Style.BorderDetails.WidthBottom = 0;
                            nameCell.Style.BorderDetails.WidthTop = 0;
                            break;
                        }
                    case 2:
                        {
                            if (numberCell.Value != null && row.NextRow != null && row.NextRow.Cells[0].Value != null &&
                                ((numberCell.Value.ToString() == "4" && row.NextRow.Cells[0].Value.ToString() == "4") ||
                                 (numberCell.Value.ToString() == "1" && row.NextRow.Cells[0].Value.ToString() == "1")))
                            {
                                numberCell.Style.BorderDetails.WidthBottom = 0;
                            }

                            numberCell.Value = "";
                            if (nameCell.Value != null && nameCell.Value.ToString() != "ранг по УрФО")
                            {
                                nameCell.Value = "прирост";
                            }
                            nameCell.Style.HorizontalAlign = HorizontalAlign.Right;
                            numberCell.Style.BorderDetails.WidthTop = 0;
                            nameCell.Style.BorderDetails.WidthTop = 0;
                            break;
                        }
                }
            }
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

            SetExportGridParams(UltraWebGrid1);
            SetExportGridParams(UltraWebGrid2);
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid1);
        }
        
        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = (i == 1) ? CRHelper.GetColumnWidth(300) : CRHelper.GetColumnWidth(60);
            }

            if (!titleAdded && !grid2Added)
            {
                IText title = e.Section.AddText();
                Font font = new Font("Verdana", 16);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.Style.Font.Bold = true;
                title.AddContent(PageTitle.Text);

                title = e.Section.AddText();
                font = new Font("Verdana", 14);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.AddContent(PageSubTitle.Text);
            }
            titleAdded = true;

        }

        private bool titleAdded = false;
        private bool grid2Added = false;

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            if (titleAdded && !grid2Added)
            {
                grid2Added = true;
                UltraGridExporter1.PdfExporter.Export(UltraWebGrid2, e.Section);
                
                UltraChart.Legend.Margins.Right = 0;
                UltraChart.Width = Unit.Pixel((int) (CustomReportConst.minScreenWidth*0.8));
                Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
                e.Section.AddImage(img);
            }
        }

        #endregion
    }
}
