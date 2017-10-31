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

namespace Krista.FM.Server.Dashboards.reports.FO_0001_0019
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
        private CustomParam day;
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
            Color color10 = Color.Aquamarine;
            Color color11 = Color.Beige;
            Color color12 = Color.Chocolate;
            Color color13 = Color.Coral;
            Color color14 = Color.Cornsilk;
            Color color15 = Color.Cyan;
            Color color16 = Color.DeepPink;
            Color color17 = Color.CornflowerBlue;
            Color color18 = Color.DarkMagenta;
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color3, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color4, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color5, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color6, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color7, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color8, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color9, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color10, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color11, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color12, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color13, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color14, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color15, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color16, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color17, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color18, 150));
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
            UltraChart.Tooltips.FormatString = "<ITEM_LABEL> <PERCENT_VALUE:N2>%\n<DATA_VALUE:N2> тыс.руб.";
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
            UltraChart1.CandleChart.VolumeVisible = true;
            UltraChart1.Effects.Effects.Clear();
            effect = new GradientEffect();
            effect.Style = GradientStyle.Default;
            effect.Coloring = GradientColoringStyle.Darken;
            effect.Enabled = true;
            UltraChart1.Effects.Enabled = true;
            UltraChart1.Effects.Effects.Add(effect);
            UltraChart1.Legend.Visible = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 13);
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Right;
            UltraChart1.Axis.X.Labels.ItemFormatString = "";
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL> <PERCENT_VALUE:N2>%\n<DATA_VALUE:N2> тыс.руб.";
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
            
            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth + 10);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.38);
            UltraChart2.Axis.X.Visible = false;
            UltraChart2.Axis.Y.Visible = false;
            UltraChart2.TitleTop.Visible = true;
            UltraChart2.PieChart.RadiusFactor = 0;
            UltraChart2.Effects.Effects.Clear();
            effect = new GradientEffect();
            effect.Style = GradientStyle.Default;
            effect.Coloring = GradientColoringStyle.Darken;
            effect.Enabled = true;
            UltraChart2.Effects.Enabled = true;
            UltraChart2.Effects.Effects.Add(effect);
            UltraChart2.Border.Thickness = 0;
            UltraChart2.CandleChart.SkipN = 0;
            UltraChart2.CandleChart.HighLowVisible = true;
            UltraChart2.CandleChart.ResetHighLowVisible();
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
            UltraChart2.PieChart.RadiusFactor = 0;
            UltraChart2.PieChart.OthersCategoryPercent = 0;
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            CRHelper.CopyCustomColorModel(UltraChart1, UltraChart2);
            UltraChart.TitleTop.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleTop.Font = new System.Drawing.Font("Verdana", 12);
            UltraChart1.TitleTop.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleTop.Font = new System.Drawing.Font("Verdana", 12);

            if (kdincomes == null)
            {
                kdincomes = UserParams.CustomParam("kdincomes");
            }
            if (day == null)
            {
                day = UserParams.CustomParam("day");
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0001_0019_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = month;
                DateTime date = new DateTime(
                       Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                       CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                       Convert.ToInt32(dtDate.Rows[0][4]));
                CustomCalendar1.WebCalendar.SelectedDate = date;

            }

            int yearNum = Convert.ToInt32(CustomCalendar1.WebCalendar.SelectedDate.Year);

            Page.Title = string.Format("Исполнение областного бюджета по расходам");
            Label1.Text = Page.Title;
            Label2.Text = string.Format("Данные по состоянию на {0:dd.MM.yyyy}, тыс.руб.",
                 CustomCalendar1.WebCalendar.SelectedDate);
            UltraChart.TitleTop.Text = "План на месяц";
            UltraChart1.TitleTop.Text = "Факт за день";
            chartHeaderLabel.Text = string.Format("Структура запланированных и фактических расходов областного бюджета  по состоянию на {0:dd.MM.yyyy}, тыс.руб.",
                 CustomCalendar1.WebCalendar.SelectedDate);
            curdate = CustomCalendar1.WebCalendar.SelectedDate;
            CustomCalendar1.WebCalendar.Layout.DropDownYearsNumber = 0;
            int monthNum = CRHelper.MonthNum(CRHelper.RusMonth(CustomCalendar1.WebCalendar.SelectedDate.Month));
            UserParams.PeriodMonth.Value = String.Format("{0}", CRHelper.RusMonth(CustomCalendar1.WebCalendar.SelectedDate.Month));
            UserParams.PeriodYear.Value = String.Format("{0}", CustomCalendar1.WebCalendar.SelectedDate.Year.ToString());
            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodEndYear.Value = (yearNum - 2).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(monthNum));
            day.Value = String.Format("{0}", CustomCalendar1.WebCalendar.SelectedDate.Day.ToString());
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
        double[] level = new double[200];
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0001_0019_grid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Раздел ФКР", dtGrid);
            for (int i = 3; i < dtGrid.Columns.Count - 1; i++)
            {
                double xMin = 0;
                double xMax = 0;

                for (int j = 0; j < dtGrid.Rows.Count; j++)
                {
                    if (dtGrid.Rows[j][i] != DBNull.Value)
                    {
                        if (Convert.ToDouble(dtGrid.Rows[j][i].ToString()) > xMax)
                        {
                            xMax = Convert.ToDouble(dtGrid.Rows[j][i]);
                        }
                        if (Convert.ToDouble(dtGrid.Rows[j][i].ToString()) < xMin)
                        {
                            xMin = Convert.ToDouble(dtGrid.Rows[j][i]);
                        }
                    }
                }
                min[i] = xMin;
                max[i] = xMax;
            }
          /*  for (int j = 0; j < dtGrid.Rows.Count; j++)
            {
                if (dtGrid.Rows[j][dtGrid.Columns.Count - 1].ToString() == "(All)") 
                {
                    dtGrid.Rows[j][2] = string.Format("{0:0000}", dtGrid.Rows[j][2]);
                }
                else
                {
                    dtGrid.Rows[j][2] = string.Format("{0:000}", dtGrid.Rows[j][2]);
                }
            }
           */
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
            e.Layout.Bands[0].Columns.RemoveAt(0);
            e.Layout.Bands[0].HeaderStyle.Wrap = true;
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            string Date1 = string.Format("{0:MM.yyyy}",
                     curdate);
            string Date2 = string.Format("{0:dd.MM.yyyy}",
                     curdate);
            string Date3 = string.Format("{0:dd.MM.yyyy}",
                     curdate.AddYears(-1));
            headerLayout.AddCell("Раздел ФКР", "").AddCell("1"); 
            headerLayout.AddCell("Код", "").AddCell("2");
            headerLayout.AddCell(String.Format("Факт {0} г.", Date2), String.Format("Фактическое исполнение на {0} г., тыс.руб.", Date2)).AddCell("3");
            headerLayout.AddCell(String.Format("План на {0}", CRHelper.RusMonth(curdate.Month)), String.Format("План на {0}, тыс.руб.", CRHelper.RusMonth(curdate.Month))).AddCell("4");
            headerLayout.AddCell(String.Format("Факт за месяц на {0}", Date2), String.Format("Фактическое исполнение за месяц по состоянию на {0} г., тыс.руб.", Date2)).AddCell("5");
            headerLayout.AddCell(String.Format("% исполнения за {0}", CRHelper.RusMonth(curdate.Month)), "Процент выполнения назначений за месяц").AddCell("6");
            headerLayout.AddCell(String.Format("План на {0} год", curdate.Year), String.Format("План на год {0}, тыс.руб.", curdate.Year)).AddCell("7");
            headerLayout.AddCell(String.Format("Факт с начала года на {0}", Date2), String.Format("Фактическое исполнение за год по состоянию на {0} г., тыс.руб.", Date2)).AddCell("8");
            headerLayout.AddCell("% исполнения годовой", "Процент выполнения назначений за год").AddCell("9");
            
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
                            width = 85;
                            //CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "0000");
                            e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
                            e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                            break;
                        }
                    case 5:
                    case 8:
                        {
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                            width = 112;
                            break;
                        }
                    default:
                        {
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                            width = 112;
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
                CRHelper.SaveToErrorLog(i.ToString());

                if (e.Row.Cells[e.Row.Cells.Count - 1] != null && e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString() != string.Empty)
                {
                    string level = e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString();
                    int fontSize = 8;
                    bool bold = false;
                    bool italic = false;
                    switch (level)
                    {
                        case "Группа":
                            {
                                fontSize = 10;
                                bold = false;
                                italic = false;
                                break;
                            }
                        case "Подгруппа":
                            {
                                fontSize = 10;
                                bold = false;
                                italic = true;
                                break;
                            }
                        case "(All)":
                            {
                                fontSize = 8;
                                bold = true;
                                italic = false;
                                break;
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
            string query = DataProvider.GetQueryText("FO_0001_0019_chart");
            DataTable dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            if (dtChart.Rows.Count > 0)
            {
                UltraChart.DataSource = dtChart;
            }
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0001_0019_chart1");
            DataTable dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            if (dtChart.Rows.Count > 0)
            {
                UltraChart1.DataSource = dtChart;
            }
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0001_0019_chart2");
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
