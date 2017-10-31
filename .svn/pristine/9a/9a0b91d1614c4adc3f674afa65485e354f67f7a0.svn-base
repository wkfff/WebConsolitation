using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebNavigator;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0019
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private DataTable dtChart;

        private int firstYear = 2009;
        private int endYear = 2010;
        private string periodCurrentYearStr;
        private string periodLastYearStr;

        #region Параметры запроса

        // Выбранный открывающий период
        private CustomParam openPeriodYear;
        // Выбранный год закрывающего периода
        private CustomParam closePeriodYear;
        // Выбранный месяц открывающего периода
        private CustomParam openPeriodMonth;
        // Выбранный месяц закрывающего периода
        private CustomParam closePeriodMonth;
        
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.6 - 225);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.52 - 100);

            #region Инициализация параметров запроса

            if (openPeriodYear == null)
            {
                openPeriodYear = UserParams.CustomParam("open_period_year");
            }
            if (closePeriodYear == null)
            {
                closePeriodYear = UserParams.CustomParam("close_period_year");
            }
            if (openPeriodMonth == null)
            {
                openPeriodMonth = UserParams.CustomParam("open_period_month");
            }
            if (closePeriodMonth == null)
            {
                closePeriodMonth = UserParams.CustomParam("close_period_month");
            }

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Axis.X.Extent = 130;
            UltraChart.Axis.Y.Extent = 55;

            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.RangeType = AxisRangeType.Automatic;
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart.Axis.Y.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;

            UltraChart.Axis.X.Labels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
            UltraChart.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.None;

            UltraChart.Legend.Margins.Right = Convert.ToInt32(UltraChart.Width.Value) / 4;
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 11;
            UltraChart.Legend.Font = new Font("Verdana", 8);
            UltraChart.Border.Thickness = 0;

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            int chartHeight = Convert.ToInt32(UltraChart.Height.Value);
            UltraChart.TitleLeft.Margins.Bottom = chartHeight - UltraChart.Axis.X.Extent;
            UltraChart.TitleLeft.Text = "Тыс.руб.";
            UltraChart.TitleLeft.Font = new Font("Verdana", 8);

            UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N1> тыс.руб.";
            UltraChart.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #endregion

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport +=new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);

            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0019_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string month = dtDate.Rows[0][3].ToString();

                string lastMonth = String.Format("{0}{1}", month, GetSpace(endYear - firstYear - 1));
                string currentMonth = String.Format("{0}{1}", month, GetSpace(endYear - firstYear));

                ComboOpenPeriod.Title = "Начало периода";
                ComboOpenPeriod.Width = 250;
                ComboOpenPeriod.MultiSelect = false;
                ComboOpenPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillYearWithMonthsValues(firstYear, endYear));
                ComboOpenPeriod.SetСheckedState(lastMonth, true);

                ComboClosePeriod.Title = "Конец периода";
                ComboClosePeriod.Width = 250;
                ComboClosePeriod.MultiSelect = false;
                ComboClosePeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillYearWithMonthsValues(firstYear, endYear));
                ComboClosePeriod.SetСheckedState(currentMonth, true);
            }

            int openMonthNum = CRHelper.MonthNum(ComboOpenPeriod.SelectedValue.TrimEnd(' '));
            int openYearNum = Convert.ToInt32(ComboOpenPeriod.SelectedNodeParent);
            int closeMonthNum = CRHelper.MonthNum(ComboClosePeriod.SelectedValue.TrimEnd(' '));
            int closeYearNum = Convert.ToInt32(ComboClosePeriod.SelectedNodeParent);

            if (openYearNum > closeYearNum || (openYearNum == closeYearNum && openMonthNum > closeMonthNum))
            {
                int tempNum = openYearNum;
                openYearNum = closeYearNum;
                closeYearNum = tempNum;

                tempNum = openMonthNum;
                openMonthNum = closeMonthNum;
                closeMonthNum = tempNum;
            }

            openPeriodMonth.Value = String.Format("[Полугодие {0}].[Квартал {1}].[{2}]", CRHelper.HalfYearNumByMonthNum(openMonthNum), CRHelper.QuarterNumByMonthNum(openMonthNum), CRHelper.RusMonth(openMonthNum));
            openPeriodYear.Value = openYearNum.ToString();
            closePeriodMonth.Value = String.Format("[Полугодие {0}].[Квартал {1}].[{2}]", CRHelper.HalfYearNumByMonthNum(closeMonthNum), CRHelper.QuarterNumByMonthNum(closeMonthNum), CRHelper.RusMonth(closeMonthNum));
            closePeriodYear.Value = closeYearNum.ToString();

            if (openYearNum == closeYearNum)
            {
                periodCurrentYearStr = (openMonthNum == closeMonthNum)
                                           ? String.Format("за {0} {1} года", CRHelper.RusMonth(closeMonthNum).ToLower(), closeYearNum)
                                           : String.Format("с {0} по {1} {2} года", CRHelper.RusMonthGenitive(openMonthNum), CRHelper.RusMonth(closeMonthNum).ToLower(), closeYearNum);

                periodLastYearStr = (openMonthNum == closeMonthNum)
                                        ? String.Format("за {0} {1} года", CRHelper.RusMonth(closeMonthNum).ToLower(), closeYearNum - 1)
                                        : String.Format("с {0} по {1} {2} года", CRHelper.RusMonthGenitive(openMonthNum), CRHelper.RusMonth(closeMonthNum).ToLower(), closeYearNum - 1);
            }
            else
            {
                periodCurrentYearStr = String.Format("с {0} {1} по {2} {3} года", CRHelper.RusMonthGenitive(openMonthNum), openYearNum, CRHelper.RusMonth(closeMonthNum).ToLower(), closeYearNum);
                periodLastYearStr = String.Format("с {0} {1} по {2} {3} года", CRHelper.RusMonthGenitive(openMonthNum), openYearNum - 1, CRHelper.RusMonth(closeMonthNum).ToLower(), closeYearNum - 1);
            }

            Page.Title = String.Format("Анализ расходов бюджетов ГО и консолидированных бюджетов МР на оплату коммунальных услуг");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("за период {0}", periodCurrentYearStr);
            chartCaptionLabel.Text = "Сравнительный анализ расходов МО на коммунальные услуги";

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            UltraChart.DataBind();
        }
        
        private static string GetSpace(int length)
        {
            const string spaceString = "                                      ";
            return spaceString.Substring(0, length);
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0019_grid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = dtGrid;
            }
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {

        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            string[] captionStr = e.Layout.Bands[0].Columns[0].Header.Caption.Split(';');
            if (captionStr.Length > 1)
            {
                e.Layout.Bands[0].Columns[0].Header.Caption = captionStr[1];
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                captionStr = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                if (captionStr.Length > 1)
                {
                    e.Layout.Bands[0].Columns[i].Header.Caption = captionStr[1];
                }

                string formatString = "N1";
                int widthColumn = 150;

                if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("Темп роста"))
                {
                    formatString = "P1";
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(230);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            int multiHeaderPos = 1;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
            {
                int columnGroupNum = (i - 1) / 2;
                
                ColumnHeader ch = new ColumnHeader(true);

                switch(columnGroupNum)
                {
                    case 0:
                        {
                            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Всего за период, тыс.руб.", "Исполнено по коммунальным услугам за период");
                            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "В среднем за период, тыс.руб.", "Исполнено по коммунальным услугам в среднем за месяц");
                            ch.Caption = String.Format("Период: {0}", periodLastYearStr);
                            break;
                        }
                    case 1:
                        {
                            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Всего за период, тыс.руб.", "Исполнено по коммунальным услугам за период");
                            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "В среднем за период, тыс.руб.", "Исполнено по коммунальным услугам в среднем за месяц");
                            ch.Caption = String.Format("Период: {0}", periodCurrentYearStr);
                            break;
                        }
                    case 2:
                        {
                            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Отклонение, тыс. руб.", "Отклонение в среднем за период к прошлому году ");
                            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Темп прироста, %", "Темп роста в среднем за период к прошлому году");                            
                            ch.Caption = "Сравнение с аналогичным периодом прошлого года в среднем за период";
                            break;
                        }
                }

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 2;
                ch.RowLayoutColumnInfo.SpanX = 2;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool rate = (i == 6);

                if (rate)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if (Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                            e.Row.Cells[i].Title = "Прирост к прошлому году";
                        }
                        else if (Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            e.Row.Cells[i].Title = "Снижение к прошлому году";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString().Contains("Итого"))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }

                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != String.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0019_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                if (dtChart.Columns.Count > 2)
                {
                    dtChart.Columns[1].ColumnName = string.Format("В среднем за период {0}", periodLastYearStr);
                    dtChart.Columns[2].ColumnName = string.Format("В среднем за период {0}", periodCurrentYearStr);
                }

                UltraChart.DataSource = dtChart;
            }
        }

        void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {

        }

        #endregion

        #region Экспорт в Excel

        private int startRowIndex = 4;

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text; 
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;
            int rowsCount = UltraWebGrid.Rows.Count;

            e.CurrentWorksheet.Columns[0].Width = 250 * 37;
            for (int i = 1; i < columnCount; i++)
            {
                string formatString = "#,#0.0;[Red]-#,#0.0";

                if (UltraWebGrid.Bands[0].Columns[i].Header.Caption.Contains("Темп роста"))
                {
                    formatString = "#,#0.0%;[Red]-#,#0.0%";
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = 100 * 37;
            }

            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[startRowIndex - 1].Height = 17 * 37;
                e.CurrentWorksheet.Rows[startRowIndex - 1].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[startRowIndex - 1].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[startRowIndex - 1].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;

                e.CurrentWorksheet.Rows[startRowIndex].Height = 17 * 37;
                e.CurrentWorksheet.Rows[startRowIndex].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[startRowIndex].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[startRowIndex].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = startRowIndex;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            switch (e.CurrentColumnIndex)
            {
                case 1:
                case 2:
                    {
                        e.HeaderText = String.Format("Период: {0}", periodLastYearStr);
                        break;
                    }
                case 3:
                case 4:
                    {
                        e.HeaderText = String.Format("Период: {0}", periodCurrentYearStr);
                        break;
                    }
                case 5:
                case 6:
                    {
                        e.HeaderText = "Сравнение с аналогичным периодом прошлого года в среднем за месяц";
                        break;
                    }
                default:
                    {
                        e.HeaderText = UltraWebGrid.Bands[0].Columns[e.CurrentColumnIndex].Header.Caption;
                        break;
                    }
            }
        }
 
        #endregion

        #region Экспорт в Pdf

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
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
        
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            e.Section.AddPageBreak();

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(chartCaptionLabel.Text);

            UltraChart.Legend.Margins.Right = 5;
            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.82));
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        #endregion
    }
}

