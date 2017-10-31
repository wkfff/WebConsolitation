using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.Documents.Reports.Report.Text;

namespace Krista.FM.Server.Dashboards.reports.FO_0001_0016
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";
        private CustomParam kdincomes;
        private CustomParam pastmonth;
        private CustomParam day;
        private GridHeaderLayout headerLayout;
        private DateTime curdate;
        private CustomParam LastMonth;
        private CustomParam LastQuart;
        private CustomParam LastHalf;
        private CustomParam LastYear;
        private CustomParam CurrMonth;
        private CustomParam CurrQuart;
        private CustomParam CurrHalf;
        private CustomParam CurrYear;
        private string headermonth = string.Empty;
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.3);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 7);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 10);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.8);
            UltraChart.ChartType = ChartType.StackColumnChart;
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            Color color1 = Color.Green;
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
            UltraChart.ColorModel.Skin.ApplyRowWise = true;
            UltraChart.Effects.Effects.Clear();
            GradientEffect effect = new GradientEffect();
            effect.Style = GradientStyle.Default;
            effect.Coloring = GradientColoringStyle.Darken;
            effect.Enabled = true;
            UltraChart.Effects.Enabled = true;
            UltraChart.Effects.Effects.Add(effect);
            UltraChart.Border.Thickness = 0;
            UltraChart.CandleChart.SkipN = 0;
            UltraChart.CandleChart.HighLowVisible = true;
            UltraChart.CandleChart.ResetHighLowVisible();
            UltraChart.CandleChart.VolumeVisible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 13);
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.Labels.ItemFormatString = "";
            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            UltraChart.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.X.Extent = 30;
            UltraChart.Axis.Y.Extent = 100;
            //UltraChart.Axis.Y.LogZero = 1;
            //UltraChart.Axis.Y.NumericAxisType = NumericAxisType.Logarithmic;
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            if (kdincomes == null)
            {
                kdincomes = UserParams.CustomParam("kdincomes");
            }
            if (day == null)
            {
                day = UserParams.CustomParam("day");
            }
            if (pastmonth == null)
            {
                pastmonth = UserParams.CustomParam("pastmonth");
            }
            if (LastYear == null)
            {
                LastYear = UserParams.CustomParam("LastYear");
            }
            if (LastHalf == null)
            {
                LastHalf = UserParams.CustomParam("LastHalf");
            }
            if (LastQuart == null)
            {
                LastQuart = UserParams.CustomParam("LastQuart");
            }
            if (LastMonth == null)
            {
               LastMonth = UserParams.CustomParam("LastMonth");
            }
            if (CurrYear == null)
            {
                CurrYear = UserParams.CustomParam("CurrYear");
            }
            if (CurrHalf == null)
            {
                CurrHalf = UserParams.CustomParam("CurrHalf");
            }
            if (CurrQuart == null)
            {
                CurrQuart = UserParams.CustomParam("CurrQuart");
            }
            if (CurrMonth == null)
            {
                CurrMonth = UserParams.CustomParam("CurrMonth");
            }
        }  

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0001_0016_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();
                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = month;
                CurrYear.Value = dtDate.Rows[0][0].ToString();
                CurrMonth.Value = dtDate.Rows[0][3].ToString();
                CurrQuart.Value = string.Format("Квартал {0}",CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));
                CurrHalf.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));
                DateTime date = new DateTime(
                       Convert.ToInt32(dtDate.Rows[0][0]),
                       CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                        Convert.ToInt32(dtDate.Rows[0][4]));
                CustomCalendar1.WebCalendar.SelectedDate = date;

            }

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(UltraChart);
            }
            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid);
                chartWebAsyncPanel.AddRefreshTarget(UltraChart);
                chartWebAsyncPanel.AddRefreshTarget(chartHeaderLabel);
            }

            int yearNum = Convert.ToInt32(CustomCalendar1.WebCalendar.SelectedDate.Year);

            Page.Title = string.Format("Анализ роста налоговых и неналоговых доходов областного бюджета");
            Label1.Text = Page.Title;
            Label2.Text = string.Format("Данные по состоянию на {0:dd.MM.yyyy}г. (включительно), тыс.руб.",
                 CustomCalendar1.WebCalendar.SelectedDate);
            curdate = CustomCalendar1.WebCalendar.SelectedDate;
            DateTime lastdate = CustomCalendar1.WebCalendar.SelectedDate;
            CustomCalendar1.WebCalendar.Layout.DropDownYearsNumber = 0;
            pastmonth.Value = String.Format("{0}", CRHelper.RusMonth(CustomCalendar1.WebCalendar.SelectedDate.Month));
            int monthNum = CRHelper.MonthNum(CRHelper.RusMonth(CustomCalendar1.WebCalendar.SelectedDate.Month));
            UserParams.PeriodMonth.Value = String.Format("{0}", CRHelper.RusMonth(CustomCalendar1.WebCalendar.SelectedDate.Month));
            UserParams.PeriodYear.Value = String.Format("{0}", CustomCalendar1.WebCalendar.SelectedDate.Year.ToString());
            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodEndYear.Value = (yearNum - 2).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(monthNum));
            day.Value = String.Format("{0}", CustomCalendar1.WebCalendar.SelectedDate.Day.ToString());
            lastdate = lastdate.AddMonths(-1);
            LastMonth.Value = CRHelper.RusMonth(lastdate.Month);
            LastHalf.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(lastdate.Month));
            LastQuart.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(lastdate.Month));
            LastYear.Value = lastdate.Year.ToString();
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            string patternValue = UserParams.StateArea.Value;
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

                int defaultRowIndex = 1;
                UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
                ActiveGridRow(row);
            
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0001_0016_grid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "КД", dtGrid);

            UltraWebGrid.DataSource = dtGrid;
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            //            UltraWebGrid.Height = Unit.Empty;
            //            UltraWebGrid.Width = Unit.Empty;
        }

        private void ActiveGridRow(UltraGridRow row)
        {
            if (row == null)
                return;
            chartHeaderLabel.Text = String.Format("Динамика исполнения областного бюджета за {0} {1} г. по доходу: {2}, тыс.руб.", CRHelper.RusMonth(CustomCalendar1.WebCalendar.SelectedDate.Month), CustomCalendar1.WebCalendar.SelectedDate.Year.ToString(), dtGrid.Rows[row.Index][0].ToString());
            UltraChart.Tooltips.FormatString = string.Format("<SERIES_LABEL:00>.{0:MM.yyyy}г.\n{1}\n<DATA_VALUE:N2> тыс.руб.", curdate, dtGrid.Rows[row.Index][0].ToString());
            kdincomes.Value = dtGrid.Rows[row.Index][row.Cells.Count - 2].ToString();
            UltraChart.DataBind();
        }

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {

            ActiveGridRow(e.Row);
        }
        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {

            e.Layout.GroupByBox.Hidden = true;
            e.Layout.Bands[0].HeaderStyle.Wrap = true;
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            string Date1 = string.Format("{0:MM.yyyy}",
                     curdate);
            string Date2 = string.Format("{0:dd.MM.yyyy}",
                     curdate);
            string Date2d = string.Format("{0:dd.MM.yyyy}",
                     curdate.AddDays(1));
            string Date3 = string.Format("{0:dd.MM.yyyy}",
                     curdate.AddYears(-1));
            string header = string.Empty;
            header = string.Format("{0} мес.", curdate.Month - 1);
            headerLayout.AddCell("КД", "").AddCell("1"); 
            headerLayout.AddCell("Код", "").AddCell("2");
            string headermonth = string.Empty;
            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0001_0016_month");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            headermonth = dtDate.Rows[0][3].ToString();
            if (curdate.Month == 1)
            {
                headerLayout.AddCell(string.Format("План на текущий год", headermonth), String.Format("Учтено при формировании бюджета на {0}г.", curdate.Year)).AddCell("3");
                headerLayout.AddCell(String.Format("Исполнено", header, curdate.Year), String.Format("Исполнено", header, curdate.Year)).AddCell("4");
                headerLayout.AddCell(String.Format("Исполнено", header, curdate.Year), String.Format("Исполнено", header, curdate.Year)).AddCell("5");                
            }
            else
            {
                headerLayout.AddCell(string.Format("План на текущий год ({0})", headermonth), String.Format("Учтено при формировании бюджета на {0}г.", curdate.Year)).AddCell("3");
                headerLayout.AddCell(String.Format("Исполнено за {0} {1} г. (по данным ежемесячной отчетности)", header, curdate.Year), String.Format("Исполнено за {0} {1} г. (нарастающим итогом по данным ежемесячной отчетности)", header, curdate.Year)).AddCell("4");
                headerLayout.AddCell(String.Format("Исполнено за {0} {1} г. (по данным АС «Бюджет»)", header, curdate.Year), String.Format("Исполнено за {0} {1} г. (нарастающим итогом по данным АС «Бюджет»)", header, curdate.Year)).AddCell("5");
            }
            headerLayout.AddCell(String.Format("Исполнено за месяц на {0} г.", Date2), String.Format("Исполнено за месяц на {0} г.", Date2)).AddCell("6"); 
            headerLayout.AddCell(String.Format("Исполнено за год на {0} г.", Date2), String.Format("Исполнено за год на {0} г. (нарастающим итогом)", Date2)).AddCell("7=4(5)+6"); 
            headerLayout.AddCell(String.Format("Исполнено %"), String.Format("% выполнения принятых назначений за текущий год")).AddCell("8=7/3"); 
            headerLayout.AddCell(String.Format("Остаток до исполнения бюджета"), String.Format("Остаток до исполнения бюджета по состоянию на {0} г.", Date2)).AddCell("9=3-7");
            headerLayout.AddCell(String.Format("Назначено прошлый год (по данным годовой отчетности)"), String.Format("Учтено при формировании бюджета в прошлом году (по данным АС «Бюджет»)")).AddCell("10");
            headerLayout.AddCell(String.Format("Исполнено прошлый год (по данным годовой отчетности)"), String.Format("Исполнено за прошлый год всего (по данным АС «Бюджет»)")).AddCell("11");
            headerLayout.AddCell(String.Format("Исполнено за год на {0} г.", Date3), String.Format("Исполнено за год на {0} г. (нарастающим итогом)", Date3)).AddCell("12");
            headerLayout.AddCell(String.Format("Наполнение бюджета на {0} г.", Date3), String.Format("% наполнения бюджета за прошлый год")).AddCell("13=12/11"); 
            headerLayout.AddCell("Темп роста плановых назначений", "Темп роста (снижения) плановых назначений текущего года к прошлому").AddCell("14=3/10");
            headerLayout.AddCell("Темп роста доходов", "Темп роста (снижения) доходов текущего года к прошлому").AddCell("15=7/12"); 
            headerLayout.AddCell("Прирост / снижение доходов", "Прирост (снижение) доходов").AddCell("16=7-12"); ;
            headerLayout.ApplyHeaderInfo();
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 2].Hidden = true;
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = (i == 0) ?
                                                                         HorizontalAlign.Left :
                                                                         HorizontalAlign.Right;
                double width;
                switch (i)
                {
                    case 0:
                        {
                            width = 270;
                            break;
                        }
                    case 1:
                        {
                            width = 114;
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "0");
                            e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
                            e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                            break;
                        }
                    case 7:
                    case 12:
                    case 13:
                    case 14:
                        {
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                            width = 114;
                            break;
                        }
                    default:
                        {
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                            width = 114;
                            e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
                            break;
                        }
                }
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(width);
            }
            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(95);
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (((i == 14) || (i == 13)) && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "Рост к прошлому отчетному периоду";
                    }
                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "Снижение к прошлому отчетному периоду";
                    }
                }
               UltraGridCell cell = e.Row.Cells[i];
               if (cell.Value != null && cell.Value.ToString() != string.Empty)
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
                if (e.Row.Cells[e.Row.Cells.Count - 1] != null && e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString() != string.Empty && i != 1)
                {
                    string level = e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString();
                    int fontSize = 8;
                    bool bold = false;
                    bool italic = false;
                    switch (level)
                    {
                        case "Подгруппа":
                            {
                                fontSize = 10;
                                bold = true;
                                italic = false;
                                break;
                            }
                        case "Статья":
                            {
                                fontSize = 10;
                                bold = false;
                                italic = false;
                                break;
                            }
                        case "Подстатья":
                            {
                                fontSize = 8;
                                bold = false;
                                italic = false;
                                break;
                            }
                    }
                    e.Row.Cells[i].Style.Font.Size = fontSize;
                    e.Row.Cells[i].Style.Font.Bold = bold;
                    e.Row.Cells[i].Style.Font.Italic = italic;
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
                }
            }
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0001_0016_chart");
            DataTable dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            if (dtChart.Rows.Count > 0)
            {
                UltraChart.DataSource = dtChart;
            }
        }


        #endregion

        #region Экспорт

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);
            IText title = section2.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(chartHeaderLabel.Text);
            ReportPDFExporter1.Export(UltraChart, section2);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
            ReportExcelExporter1.HeaderCellHeight = 50;
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart, chartHeaderLabel.Text, sheet2, 3);
        }
        #endregion

    }
}
