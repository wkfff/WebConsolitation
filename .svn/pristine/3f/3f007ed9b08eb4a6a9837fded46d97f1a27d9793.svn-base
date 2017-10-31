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
using Krista.FM.Server.Dashboards.Components;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.Documents.Reports.Report.Text;

namespace Krista.FM.Server.Dashboards.reports.FO_0001_0017
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";
        private CustomParam kdincomes;
        private GridHeaderLayout headerLayout;
        private DateTime curdate;
        private CustomParam LastMonth;
        private CustomParam LastQuart;
        private CustomParam LastHalf;
        private CustomParam LastYear;
        private CustomParam day;
        private CustomParam CurrMonth;
        private CustomParam CurrQuart;
        private CustomParam CurrHalf;
        private CustomParam CurrYear;
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 200);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 7);

            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.45);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);
            UltraChart.ChartType = ChartType.PieChart;
            UltraChart.TitleTop.Visible = true;
            UltraChart.Border.Thickness = 0;
            UltraChart.CandleChart.SkipN = 0;
            UltraChart.CandleChart.HighLowVisible = true;
            UltraChart.CandleChart.ResetHighLowVisible();
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            Color color1 = Color.LightBlue;
            Color color2 = Color.DarkGreen;
            Color color3 = Color.Red;
            Color color4 = Color.Yellow;
            Color color5 = Color.DarkOrchid;
            Color color6 = Color.DarkSalmon;
            Color color7 = Color.Brown;
            Color color8 = Color.DarkTurquoise;
            Color color9 = Color.BurlyWood;
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color3, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color4, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color5, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color6, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color7, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color8, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color9, 150));
            UltraChart.ColorModel.Skin.ApplyRowWise = true;
            UltraChart.Effects.Effects.Clear();
            GradientEffect effect = new GradientEffect();
            effect.Style = GradientStyle.Default;
            effect.Coloring = GradientColoringStyle.Darken;
            effect.Enabled = true;
            UltraChart.Effects.Enabled = true;
            UltraChart.Effects.Effects.Add(effect);
            UltraChart.CandleChart.VolumeVisible = true;
            UltraChart.Legend.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 13);
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Left;
            UltraChart.Axis.X.Labels.ItemFormatString = "";
            UltraChart.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE:N2> тыс.руб.";
            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            UltraChart.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.X.Extent = 30;
            UltraChart.PieChart.OthersCategoryPercent = 0;
            UltraChart.Legend.SpanPercentage = 0;
            UltraChart.Axis.Y.Extent = 100;
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
       
            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.45);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);
            UltraChart1.ChartType = ChartType.PieChart;
            UltraChart1.TitleTop.Visible = true;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.CandleChart.SkipN = 0;
            UltraChart1.CandleChart.HighLowVisible = true;
            UltraChart1.CandleChart.ResetHighLowVisible();
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
            UltraChart1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));
            UltraChart1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color3, 150));
            UltraChart1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color4, 150));
            UltraChart1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color5, 150));
            UltraChart1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color6, 150));
            UltraChart1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color7, 150));
            UltraChart1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color8, 150));
            UltraChart1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color9, 150));
            UltraChart1.ColorModel.Skin.ApplyRowWise = true;
            UltraChart1.Effects.Effects.Clear();
            UltraChart1.Effects.Enabled = true;
            UltraChart1.Effects.Effects.Add(effect);
            UltraChart1.CandleChart.VolumeVisible = true;
            UltraChart1.Legend.Visible = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 13);
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Right;
            UltraChart1.Axis.X.Labels.ItemFormatString = "";
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE:N2> тыс.руб.";
            UltraChart1.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart1.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            UltraChart1.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.X.Extent = 30;
            UltraChart1.PieChart.OthersCategoryPercent = 0;
            UltraChart1.Axis.Y.Extent = 100;
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            CRHelper.CopyCustomColorModel(UltraChart, UltraChart1);
            CRHelper.CopyCustomColorModel(UltraChart1, UltraChart);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.15);
            UltraChart2.Axis.X.Visible = false;
            UltraChart2.Axis.Y.Visible = false;
            UltraChart2.PieChart.RadiusFactor = 0;
            UltraChart2.TitleTop.Visible = true;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.CandleChart.SkipN = 0;
            UltraChart2.CandleChart.HighLowVisible = true;
            UltraChart2.CandleChart.ResetHighLowVisible();
            UltraChart2.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart2.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
            UltraChart2.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));
            UltraChart2.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color3, 150));
            UltraChart2.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color4, 150));
            UltraChart2.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color5, 150));
            UltraChart2.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color6, 150));
            UltraChart2.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color7, 150));
            UltraChart2.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color8, 150));
            UltraChart2.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color9, 150));
            UltraChart2.ColorModel.Skin.ApplyRowWise = true;
            UltraChart2.Effects.Effects.Clear();
            UltraChart2.Effects.Enabled = true;
            UltraChart2.Effects.Effects.Add(effect);
            UltraChart2.CandleChart.VolumeVisible = true;
            UltraChart2.Legend.Visible = true;
            UltraChart2.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart2.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 13);
            UltraChart2.Axis.X.Labels.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Bottom;
            UltraChart2.Axis.X.Labels.ItemFormatString = "";
            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE:N2> тыс.руб.";
            UltraChart2.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart2.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            UltraChart2.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Axis.X.Extent = 30;
            UltraChart2.Axis.Y.Extent = 100;
            UltraChart2.Legend.SpanPercentage = 100;
            UltraChart2.PieChart.OthersCategoryPercent = 0;
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            CRHelper.CopyCustomColorModel(UltraChart1, UltraChart2);

            if (kdincomes == null)
            {
                kdincomes = UserParams.CustomParam("kdincomes");
            }
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
            if (day == null)
            {
                day = UserParams.CustomParam("day");
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
                string query = DataProvider.GetQueryText("FO_0001_0017_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();
                day.Value = dtDate.Rows[0][4].ToString();
                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = month;
                CurrYear.Value = dtDate.Rows[0][0].ToString();
                CurrMonth.Value = dtDate.Rows[0][3].ToString();
                CurrQuart.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));
                CurrHalf.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));
                DateTime date = new DateTime(
                       Convert.ToInt32(dtDate.Rows[0][0]),
                       CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                           Convert.ToInt32(dtDate.Rows[0][4]));
                CustomCalendar1.WebCalendar.SelectedDate = date;

            }

            int yearNum = Convert.ToInt32(CustomCalendar1.WebCalendar.SelectedDate.Year);

            Page.Title = string.Format("Анализ структуры налоговых и неналоговых доходов областного бюджета");
            Label1.Text = Page.Title;
            Label2.Text = string.Format("Данные по состоянию на {0:dd.MM.yyyy}г. (включительно), тыс.руб.",
                 CustomCalendar1.WebCalendar.SelectedDate);
            UltraChart.TitleTop.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleTop.Font = new System.Drawing.Font("Verdana", 12);
            UltraChart1.TitleTop.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleTop.Font = new System.Drawing.Font("Verdana", 12);
            UltraChart.TitleTop.Text = string.Format("на {0:dd.MM.yyyy}г.",
                 CustomCalendar1.WebCalendar.SelectedDate);
            UltraChart1.TitleTop.Text = string.Format("на {0:dd.MM.yyyy}г.",
                 CustomCalendar1.WebCalendar.SelectedDate.AddYears(-1));
            curdate = CustomCalendar1.WebCalendar.SelectedDate;
            CustomCalendar1.WebCalendar.Layout.DropDownYearsNumber = 0;
            int monthNum = CRHelper.MonthNum(CRHelper.RusMonth(CustomCalendar1.WebCalendar.SelectedDate.Month));
            UserParams.PeriodMonth.Value = String.Format("{0}", CRHelper.RusMonth(CustomCalendar1.WebCalendar.SelectedDate.Month));
            UserParams.PeriodYear.Value = String.Format("{0}", CustomCalendar1.WebCalendar.SelectedDate.Year.ToString());
            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodEndYear.Value = (yearNum - 2).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(monthNum));
            DateTime lastdate = CustomCalendar1.WebCalendar.SelectedDate;
            lastdate = lastdate.AddMonths(-1);
            LastMonth.Value = CRHelper.RusMonth(lastdate.Month);
            LastHalf.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(lastdate.Month));
            LastQuart.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(lastdate.Month));
            LastYear.Value = lastdate.Year.ToString();
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            string patternValue = UserParams.StateArea.Value;
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
            UltraChart.DataBind();
            UltraChart1.DataBind();
            UltraChart2.DataBind();
            if (!Page.IsPostBack)
            {
                int defaultRowIndex = 1;
                UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
                ActiveGridRow(row);
            }
        }

        #region Обработчики грида
        double[] min = new double[200];
        double[] max = new double[200];
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0001_0017_grid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "КД", dtGrid);
            double xMin;
            double xMax;
            for (int i = 3; i < dtGrid.Columns.Count - 1; i++)
            {
                xMin = 100;
                xMax = 0;
                for (int j = 1; j < dtGrid.Rows.Count; j++)
                {
                    if (dtGrid.Rows[j][i] != DBNull.Value)
                    {
                        if (Convert.ToDouble(dtGrid.Rows[j][i].ToString()) > xMax)
                        {
                            xMax = Convert.ToDouble(dtGrid.Rows[j][i]);
                        }
                        if ((Convert.ToDouble(dtGrid.Rows[j][i].ToString()) < xMin)&&(Convert.ToDouble(string.Format("{0:N4}",dtGrid.Rows[j][i])) != 0))
                        {
                            xMin = Convert.ToDouble(dtGrid.Rows[j][i]);
                        }
                    }
                }
                min[i] = xMin;
                max[i] = xMax;
            }
            UltraWebGrid.DataSource = dtGrid;
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            //            UltraWebGrid.Height = Unit.Empty;
            //            UltraWebGrid.Width = Unit.Empty;
        }

        private void ActiveGridRow(UltraGridRow row)
        {


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
            string Date3 = string.Format("{0:dd.MM.yyyy}",
                     curdate.AddYears(-1));
            string header = string.Empty;
            header = string.Format("{0} мес.", curdate.Month - 1);
            headerLayout.AddCell("КД", "").AddCell("1");
            headerLayout.AddCell("Код", "").AddCell("2");
            string headermonth = string.Empty;
            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0001_0017_month");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            headermonth = dtDate.Rows[0][3].ToString();
            if (curdate.Month == 1)
            {
                headerLayout.AddCell(string.Format("План на текущий год", headermonth), String.Format("Учтено при формировании бюджета на {0}г.", curdate.Year)).AddCell("3");
                headerLayout.AddCell("Доля", "Удельный вес доходного источника в общем объеме бюджетных назначений по налоговым и неналоговым доходам").AddCell("4");
                headerLayout.AddCell(String.Format("Исполнено", Date1), String.Format("Исполнено", Date1)).AddCell("5");
                headerLayout.AddCell("Доля", String.Format("Удельный вес доходного источника в общем объеме фактических доходов по состоянию на 01.{0} г.", Date1)).AddCell("6");
                headerLayout.AddCell(String.Format("Исполнено", Date1), String.Format("Исполнено", Date1)).AddCell("7");
                headerLayout.AddCell("Доля", String.Format("Удельный вес доходного источника в общем объеме фактических доходов по состоянию на 01.{0} г.", Date1)).AddCell("8");
            }
            else
            {
                headerLayout.AddCell(string.Format("План на текущий год ({0})", headermonth), String.Format("Учтено при формировании бюджета на {0}г.", curdate.Year)).AddCell("3");
                headerLayout.AddCell("Доля", "Удельный вес доходного источника в общем объеме бюджетных назначений по налоговым и неналоговым доходам").AddCell("4");
                headerLayout.AddCell(String.Format("Исполнено за {0} {1} г. (по данным ежемесячной отчетности)", header, curdate.Year), String.Format("Исполнено за {0} {1} г. (нарастающим итогом по данным ежемесячной отчетности)", header, curdate.Year)).AddCell("5");
                headerLayout.AddCell("Доля", String.Format("Удельный вес доходного источника в общем объеме фактических доходов по состоянию на 01.{0} г.", Date1)).AddCell("6");
                headerLayout.AddCell(String.Format("Исполнено за {0} {1} г. (по данным АС «Бюджет»)", header, curdate.Year), String.Format("Исполнено за {0} {1} г. (нарастающим итогом по данным АС «Бюджет»)", header, curdate.Year)).AddCell("7");
                headerLayout.AddCell("Доля", String.Format("Удельный вес доходного источника в общем объеме фактических доходов по состоянию на 01.{0} г.", Date1)).AddCell("8");
            }
            headerLayout.AddCell(String.Format("Исполнено за месяц на {0} г.", Date2), String.Format("Исполнено за месяц на {0} г.", Date2)).AddCell("9");
            headerLayout.AddCell("Доля", "Удельный вес доходного источника в общей сумме налоговых и неналоговых доходов").AddCell("10");
            headerLayout.AddCell(String.Format("Исполнено за год на {0} г.", Date2), String.Format("Исполнено за год на {0} г. (нарастающим итогом)", Date2)).AddCell("11=5(7)+9");
            headerLayout.AddCell("Доля", String.Format("Удельный вес доходного источника в объеме фактических доходов за год по состоянию на {0} г.", Date2)).AddCell("12");
            headerLayout.AddCell(String.Format("Назначено прошлый год (по данным годовой отчетности)"), String.Format("Учтено при формировании бюджета в прошлом году (по данным годовой отчетности)")).AddCell("13");
            headerLayout.AddCell("Доля", "Удельный вес доходного источника в общем объеме бюджетных назначений по налоговым и неналоговым доходам в прошлом году").AddCell("14");
            headerLayout.AddCell(String.Format("Исполнено прошлый год (по данным годовой отчетности)"), String.Format("Исполнено за прошлый год всего (по данным годовой отчетности)")).AddCell("15");
            headerLayout.AddCell("Доля", "Удельный вес доходного источника в общем объеме фактических налоговых и неналоговых доходов в прошлом году").AddCell("16");
            headerLayout.AddCell(String.Format("Исполнено за год на {0} г.", Date3), String.Format("Исполнено за год на {0} г. (нарастающим итогом)", Date3)).AddCell("17");
            headerLayout.AddCell("Доля", String.Format("Удельный вес доходного источника в общем объеме фактических доходов по состоянию на {0} г.", Date3)).AddCell("18");
            headerLayout.ApplyHeaderInfo();
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
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
                    case 3:
                    case 5:
                    case 7:
                    case 9:
                    case 11:
                    case 13:
                    case 15:
                    case 17:
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
            CRHelper.SaveToErrorLog(min[3].ToString());
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                CRHelper.SaveToErrorLog(i.ToString());
                if (((i == 3) || (i == 5) || (i == 7) || (i == 9) || (i == 11) || (i == 13) || (i == 15) || (i == 17)) && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (Convert.ToDouble(e.Row.Cells[i].Value) == max[i])
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                        e.Row.Cells[i].Title = "Наибольший удельный вес дохода";
                    }
                    else if (Convert.ToDouble(e.Row.Cells[i].Value) == min[i])
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                        e.Row.Cells[i].Title = "Наименьший удельный вес дохода";
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
                                italic = true;
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
            string query = DataProvider.GetQueryText("FO_0001_0017_chart");
            DataTable dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            if (dtChart.Rows.Count > 0)
            {
                UltraChart.DataSource = dtChart;
            }
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0001_0017_chart1");
            DataTable dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            if (dtChart.Rows.Count > 0)
            {
                UltraChart1.DataSource = dtChart;
            }
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0001_0017_chart2");
            DataTable dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            if (dtChart.Rows.Count > 0)
            {
                UltraChart2.DataSource = dtChart;
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
            ISection section3 = report.AddSection();

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);
            IText title = section2.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(chartHeaderLabel.Text);
            ReportPDFExporter1.Export(UltraChart, section2);
            ReportPDFExporter1.Export(UltraChart1, section3);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
            Worksheet sheet3 = workbook.Worksheets.Add("sheet3");
            ReportExcelExporter1.HeaderCellHeight = 50;
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart, chartHeaderLabel.Text, sheet2, 3);
            ReportExcelExporter1.Export(UltraChart1, chartHeaderLabel.Text, sheet3, 3);
        }
        #endregion

    }
}
