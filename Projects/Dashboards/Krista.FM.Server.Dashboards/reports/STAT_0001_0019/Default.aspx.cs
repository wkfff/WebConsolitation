using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.STAT_0001_0019
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;
        private DataTable dtDate;
        private GridHeaderLayout headerLayout;

        private int year = 2009;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            ReportExcelExporter1.ExcelExporter.EndExport += new Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
            #region Настройка диаграммы

            SetupChart1();
            SetupChart2();
            SetupChart3();
            SetupChart4();

            gridCaptionElement.Text =
                "Общая информация по показателям, характеризующим отрасль добычи полезных ископаемых";
            chart1ElementCaption.Text = "<nobr>Объем отгруженных товаров собственного производства, выполненных работ и услуг собственными силами организаций добывающих производств</nobr>";
            chart2ElementCaption.Text = "Объем отгруженных товаров собственного производства, выполненных работ и услуг собственными силами на одного работника";
            chart3ElementCaption.Text = "Удельный вес прибыльных и убыточных организаций в общем числе организаций";
            chart4ElementCaption.Text = "Производство основных видов продукции";

            #endregion
        }
                
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            dtDate = new DataTable();
            
            string query = DataProvider.GetQueryText("STAT_0001_0019_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Район", dtDate);

            year = Convert.ToInt32(dtDate.Rows[0][0]);

            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.PeriodFirstYear.Value = (year - 2).ToString();

            if (!Page.IsPostBack)
            {
                ComboYear.Width = 90;
                ComboYear.MultiSelect = false;
                ComboYear.ParentSelect = true;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(2005, year));
                ComboYear.SetСheckedState(UserParams.PeriodYear.Value, true);
                ComboYear.Title = "Год";
            }

            year = Convert.ToInt32(ComboYear.SelectedValue);

            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.PeriodFirstYear.Value = (year - 2).ToString();

            Page.Title = "Добыча полезных ископаемых";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text =
                String.Format("Динамика и структура основных показателей, характеризующих добычу полезных ископаемых, Ханты-Мансийский автономный округ-Югра, {0} год", year);

            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();

            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
            UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);
            UltraChart3.DataBinding += new EventHandler(UltraChart3_DataBinding);
            UltraChart4.DataBinding += new EventHandler(UltraChart4_DataBinding);

            UltraChart1.DataBind();
            UltraChart2.DataBind();
            UltraChart3.DataBind();
            UltraChart4.DataBind();

            BindLabel1();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0019_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            foreach(DataRow row in dtGrid.Rows)
            {
                row[0] = String.Format("{0}, {1}", row[0], row[1].ToString().Replace("Миллион рублей", "млн.руб.").Replace("Единица", "Единиц").Replace("Процент", "%").ToLower());                
            }

            dtGrid.Columns.RemoveAt(1);
            ((UltraWebGrid)sender).DataSource = dtGrid;
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid1.Height = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250, 1280);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            headerLayout.AddCell("Показатель");

            GridHeaderCell cell = headerLayout.AddCell((year - 2).ToString());
            cell.AddCell("Значение");
            cell.AddCell("Прирост");
            cell.AddCell("Темп роста");

            cell = headerLayout.AddCell((year - 1).ToString());
            cell.AddCell("Значение");
            cell.AddCell("Прирост");
            cell.AddCell("Темп роста");

            cell = headerLayout.AddCell(year.ToString());
            cell.AddCell("Значение");
            cell.AddCell("Прирост");
            cell.AddCell("Темп роста");

            headerLayout.ApplyHeaderInfo();

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[9], "P2");

            for (int i = 1; i < 10; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {

        }

        #endregion

        #region Тексты

        private void BindLabel1()
        {
            double grownValue = 0;
            if (dtGrid.Rows[1][8] != DBNull.Value)
            {
                grownValue = Convert.ToDouble(dtGrid.Rows[1][8]);
            }
            double grownTemp = 0;
            if (dtGrid.Rows[1][9] != DBNull.Value)
            {
                grownTemp = Convert.ToDouble(dtGrid.Rows[1][9]) - 1;
            }
            string grownDescription = "не изменился";
            if (grownValue < 0)
            {
                grownDescription = String.Format("<nobr><img src='../../images/arrowRedDownBB.png'> уменьшился</nobr> на <b>{0:N2}</b> млн.руб. (<b>{1:P2}</b>)", Math.Abs(grownValue), Math.Abs(grownTemp));
            }
            else if (grownValue > 0)
            {
                grownDescription = String.Format("<nobr><img src='../../images/arrowGreenUpBB.png'> увеличился</nobr> на <b>{0:N2}</b> млн.руб. (<b>{1:P2}</b>)", Math.Abs(grownValue), Math.Abs(grownTemp));
            }

            Label1.Text = String.Format("По сравнению с <b>{0}</b> годом в <b>{1}</b> году объем отгруженных товаров собственного производства, выполненных работ и услуг собственными силами {2} и составил <b>{3:N2}</b> млн.руб.", year - 1, year, grownDescription, dtGrid.Rows[1][7]);
        }

        #endregion

        #region Обработчики диаграммы 1

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0019_chart1");
            DataTable dtChart1 = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtChart1);

            for (int i = 1; i < dtChart1.Columns.Count; i++)
            {
                ChartTextAppearance appearance = new ChartTextAppearance();
                appearance.Column = i - 1;
                appearance.Row = -2;
                appearance.VerticalAlign = StringAlignment.Far;
                appearance.ItemFormatString = "<DATA_VALUE_ITEM:N3>";
                appearance.ChartTextFont = new Font("Verdana", 10);
                appearance.Visible = true;
                UltraChart1.ColumnChart.ChartText.Add(appearance);
            }
            UltraChart1.DataSource = dtChart1;
        }

        private void SetupChart1()
        {
            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
            UltraChart1.Height = CRHelper.GetChartHeight(250);

            UltraChart1.ChartType = ChartType.ColumnChart;
            UltraChart1.Border.Thickness = 0;

            UltraChart1.Axis.Y.Extent = 50;
            UltraChart1.Axis.X.Extent = 10;

            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL> год\n<b><DATA_VALUE:N3></b> млрд.руб.";

            UltraChart1.Legend.Visible = false;

            UltraChart1.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.FontColor = Color.Black;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart1.Axis.Y.Labels.FontColor = Color.Black;

            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart1.Axis.X.Labels.Orientation = TextOrientation.Horizontal;

            UltraChart1.Data.SwapRowsAndColumns = false;

            CRHelper.FillCustomColorModel(UltraChart1, 1, true);

            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Font = new Font("Verdana", 8);
            UltraChart1.TitleLeft.Text = "млрд.руб.";
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;

            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
        }

        #endregion

        #region Обработчики диаграммы 2

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0019_chart2");
            DataTable dtChart2 = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtChart2);

            for (int i = 1; i < dtChart2.Columns.Count; i++)
            {
                ChartTextAppearance appearance = new ChartTextAppearance();
                appearance.Column = i - 1;
                appearance.Row = -2;
                appearance.VerticalAlign = StringAlignment.Far;
                appearance.ItemFormatString = "<DATA_VALUE:N3>";
                appearance.ChartTextFont = new Font("Verdana", 10);
                appearance.Visible = true;
                UltraChart2.SplineAreaChart.ChartText.Add(appearance);
            }

            UltraChart2.DataSource = dtChart2;

            double grownValue = 0;
            if (dtGrid.Rows[2][8] != DBNull.Value)
            {
                grownValue = Convert.ToDouble(dtGrid.Rows[2][8]);
            }

            double grownTemp = 0;
            if (dtGrid.Rows[2][9] != DBNull.Value)
            {
                grownTemp = Convert.ToDouble(dtGrid.Rows[2][9]) - 1;
            }

            string grownDescription = "не изменилась";
            if (grownValue < 0)
            {
                grownDescription = String.Format("<img src='../../images/arrowRedDownBB.png'> уменьшилась на <b>{0:P2}</b>", Math.Abs(grownTemp));
            }
            else if (grownValue > 0)
            {
                grownDescription = String.Format("<img src='../../images/arrowGreenUpBB.png'> увеличилась на <b>{0:P2}</b>", Math.Abs(grownTemp));
            }

            double grownValueAverageThisYear = 0;
            if (dtChart2.Rows[0][year.ToString()] != DBNull.Value)
            {
                grownValueAverageThisYear = Convert.ToDouble(dtChart2.Rows[0][year.ToString()]);
            }

            double grownValueAverageLastYear = 0;
            if (dtChart2.Rows[0][(year - 1).ToString()] != DBNull.Value)
            {
                grownValueAverageLastYear = Convert.ToDouble(dtChart2.Rows[0][(year - 1).ToString()]);
            }
            double grownTempAverage = 0;
            if (grownValueAverageLastYear != 0)
            {
                grownTempAverage = grownValueAverageThisYear / grownValueAverageLastYear - 1;
            }
            string grownAverageDescription = "не изменился";
            if (grownTempAverage < 0)
            {
                grownAverageDescription = String.Format("<img src='../../images/arrowRedDownBB.png'> уменьшился на <b>{0:P2}</b>", Math.Abs(grownTempAverage));
            }
            else if (grownTempAverage > 0)
            {
                grownAverageDescription = String.Format("<img src='../../images/arrowGreenUpBB.png'> увеличился на <b>{0:P2}</b>", Math.Abs(grownTempAverage));
            }

            Label2.Text = String.Format("По сравнению с <b>{0}</b> годом в <b>{1}</b> году среднегодовая численность работников организаций {2} и составила <b>{3:N2}</b> чел., при этом объем отгруженных товаров собственного производства, выполненных работ и услуг собственными силами на одного работника организаций {4} и составил <b>{5:N2}</b> млн.руб.",
                                        year - 1, year, grownDescription, dtGrid.Rows[2][7], grownAverageDescription, grownValueAverageThisYear);
        }

        private void SetupChart2()
        {
            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
            UltraChart2.Height = CRHelper.GetChartHeight(250);

            UltraChart2.ChartType = ChartType.SplineAreaChart;
            UltraChart2.Border.Thickness = 0;

            UltraChart2.Axis.Y.Extent = 50;
            UltraChart2.Axis.X.Extent = 10;

            UltraChart2.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart2.Axis.X.Labels.FontColor = Color.Black;
            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart2.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart2.Axis.Y.Labels.FontColor = Color.Black;

            UltraChart2.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
                        
            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.Font = new Font("Verdana", 8);
            UltraChart2.TitleLeft.Text = "млн.руб.";
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
           
            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL> год\n<b><DATA_VALUE:N3></b> млн.руб.";
            
            UltraChart2.Data.SwapRowsAndColumns = false;

            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChart2.Axis.X.Margin.Near.Value = 25;
            UltraChart2.Axis.X.Margin.Far.Value = 25;

            UltraChart2.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
            UltraChart2.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
        }

        #endregion

        #region Обработчики диаграммы 3

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0019_chart3");
            DataTable dtChart3 = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtChart3);
            UltraChart3.DataSource = dtChart3;

            double grownValue = 0;
            if (dtGrid.Rows[0][8] != DBNull.Value)
            {
                grownValue = Convert.ToDouble(dtGrid.Rows[0][8]);
            }
            string grownDescription = "не изменилось";
            if (grownValue < 0)
            {
                grownDescription = String.Format("<img src='../../images/arrowRedDownBB.png'> уменьшилось на <b>{0:N0}</b> единицы", Math.Abs(grownValue));
            }
            else if (grownValue > 0)
            {
                grownDescription = String.Format("<img src='../../images/arrowGreenUpBB.png'> увеличилось на <b>{0:N0}</b> единицы", Math.Abs(grownValue));
            }

            double grownSaldoValue = 0;
            if (dtGrid.Rows[3][8] != DBNull.Value)
            {
                grownSaldoValue = Convert.ToDouble(dtGrid.Rows[3][8]);
            }
            double grownSaldoTemp = 0;
            if (dtGrid.Rows[3][9] != DBNull.Value)
            {
                grownSaldoTemp = Convert.ToDouble(dtGrid.Rows[3][9]) - 1;
            }
            string grownSaldoDescription = "не изменился";
            if (grownSaldoValue < 0)
            {
                grownSaldoDescription = String.Format("<img src='../../images/arrowRedDownBB.png'> уменьшился на <b>{0:N2}</b>&nbsp;млн.руб. (<b>{1:P2}</b>)", Math.Abs(grownSaldoValue), Math.Abs(grownSaldoTemp));
            }
            else if (grownSaldoValue > 0)
            {
                grownSaldoDescription = String.Format("<img src='../../images/arrowGreenUpBB.png'> увеличился на <b>{0:N2}</b>&nbsp;млн.руб. (<b>{1:P2}</b>)", Math.Abs(grownSaldoValue), Math.Abs(grownSaldoTemp));
            }

            double grownValueProfitThisYear = 0;
            if (dtChart3.Rows[0][year.ToString()] != DBNull.Value)
            {
                grownValueProfitThisYear = Convert.ToDouble(dtChart3.Rows[0][year.ToString()]);
            }
            double grownValueProfitLastYear = 0;
            if (dtChart3.Rows[0][(year - 1).ToString()] != DBNull.Value)
            {
                grownValueProfitLastYear = Convert.ToDouble(dtChart3.Rows[0][(year - 1).ToString()]);
            }
            double grownTempProfit = 0;
            if (grownValueProfitLastYear != 0)
            {
                grownTempProfit = grownValueProfitThisYear / grownValueProfitLastYear - 1;
            }
            string grownProfitDescription = "не изменился";
            if (grownTempProfit < 0)
            {
                grownProfitDescription = String.Format("<img src='../../images/arrowRedDownBB.png'> уменьшился на <b>{0:P2}</b>", Math.Abs(grownTempProfit));
            }
            else if (grownTempProfit > 0)
            {
                grownProfitDescription = String.Format("<img src='../../images/arrowGreenUpBB.png'> увеличился на <b>{0:P2}</b>", Math.Abs(grownTempProfit));
            }


            Label3.Text = String.Format("По сравнению с <b>{0}</b> годом в <b>{1}</b> году количество организаций {2}, сальдированный финансовый результат работы организаций {3} и составил <b>{4:N2}</b> млн.руб., удельный вес прибыльных организаций {5} и составил <b>{6:N2}%</b> от общего числа организаций.",
                                        year - 1, year, grownDescription, grownSaldoDescription, dtGrid.Rows[3][7], grownProfitDescription, grownValueProfitThisYear);
        }

        private void SetupChart3()
        {
            UltraChart3.Axis.Y.Extent = 50;
            UltraChart3.Axis.X.Extent = 20;

            UltraChart3.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
            UltraChart3.Height = CRHelper.GetChartHeight(300);

            UltraChart3.ChartType = ChartType.StackColumnChart;
            UltraChart3.Border.Thickness = 0;

            UltraChart3.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
            UltraChart3.Axis.X.Labels.SeriesLabels.FontColor = Color.Black;
            UltraChart3.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>%";
            UltraChart3.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart3.Axis.Y.Labels.FontColor = Color.Black;
                        
            UltraChart3.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;

            UltraChart3.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL> год\n<b><DATA_VALUE:N2>%</b>";

            UltraChart3.Data.SwapRowsAndColumns = true;

            UltraChart3.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChart3.Legend.Visible = true;
            UltraChart3.Legend.Location = LegendLocation.Bottom;
            UltraChart3.Legend.SpanPercentage = 15;
            UltraChart3.Legend.Font = new Font("Verdana", 8);

            CRHelper.FillCustomColorModel(UltraChart3, 2, false);
        }

        #endregion

        #region Обработчики диаграммы 4

        protected void UltraChart4_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0019_chart4");
            DataTable dtChart4 = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtChart4);

            for (int i = 1; i < dtChart4.Columns.Count; i++)
            {
                ChartTextAppearance appearance = new ChartTextAppearance();
                appearance.Column = i - 1;
                appearance.Row = -2;
                appearance.VerticalAlign = StringAlignment.Far;
                appearance.ItemFormatString = "<DATA_VALUE:N1>";
                appearance.ChartTextFont = new Font("Verdana", 10);
                appearance.Visible = true;
                UltraChart4.SplineChart.ChartText.Add(appearance);
            }

            UltraChart4.DataSource = dtChart4;
        }

        private void SetupChart4()
        {
            UltraChart4.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
            UltraChart4.Height = CRHelper.GetChartHeight(300);

            UltraChart4.ChartType = ChartType.SplineChart;
            UltraChart4.Border.Thickness = 0;

            UltraChart4.Axis.Y.NumericAxisType = NumericAxisType.Logarithmic;
            UltraChart4.Axis.Y.LogBase = 1.5;

            UltraChart4.Axis.Y.Extent = 50;
            UltraChart4.Axis.X.Extent = 10;

            UltraChart4.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart4.Axis.X.Labels.FontColor = Color.Black;
            UltraChart4.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart4.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart4.Axis.Y.Labels.FontColor = Color.Black;

            UltraChart4.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart4.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;

            UltraChart4.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL> год\n<b><DATA_VALUE:N1></b>";

            UltraChart4.Data.SwapRowsAndColumns = false;

            UltraChart4.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChart4.Legend.Visible = true;
            UltraChart4.Legend.Location = LegendLocation.Bottom;
            UltraChart4.Legend.SpanPercentage = 25;
            UltraChart4.Legend.Font = new Font("Verdana", 8);

            UltraChart4.Axis.X.Margin.Near.Value = 35;
            UltraChart4.Axis.X.Margin.Far.Value = 25;
            UltraChart4.Axis.Y.Margin.Far.Value = 25;

            UltraChart4.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
            UltraChart4.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
            UltraChart4.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[3].Height = 30 * 37;
            e.CurrentWorksheet.Rows[4].Height = 30 * 37;

            e.CurrentWorksheet.MergedCellsRegions.Add(18, 0, 18, 9);
            e.CurrentWorksheet.Rows[18].Cells[0].Value = Regex.Replace(chart1ElementCaption.Text, "<[\\s\\S]*?>", String.Empty).Replace("&nbsp;", " ");
            e.CurrentWorksheet.MergedCellsRegions.Add(19, 0, 19, 9);
            e.CurrentWorksheet.Rows[19].Cells[0].Value = Regex.Replace(Label1.Text, "<[\\s\\S]*?>", String.Empty).Replace("&nbsp;", " ");

            ReportExcelExporter.ChartExcelExport(e.CurrentWorksheet.Rows[20].Cells[0], UltraChart1);

            e.CurrentWorksheet.MergedCellsRegions.Add(40, 0, 40, 9);
            e.CurrentWorksheet.MergedCellsRegions.Add(41, 0, 41, 9);
            e.CurrentWorksheet.Rows[40].Cells[0].Value = Regex.Replace(chart2ElementCaption.Text, "<[\\s\\S]*?>", String.Empty).Replace("&nbsp;", " ");
            e.CurrentWorksheet.Rows[41].Cells[0].Value = Regex.Replace(Label2.Text, "<[\\s\\S]*?>", String.Empty).Replace("&nbsp;", " ");
            ReportExcelExporter.ChartExcelExport(e.CurrentWorksheet.Rows[42].Cells[0], UltraChart2);

            e.CurrentWorksheet.MergedCellsRegions.Add(62, 0, 62, 9);
            e.CurrentWorksheet.MergedCellsRegions.Add(63, 0, 63, 9);
            e.CurrentWorksheet.Rows[62].Cells[0].Value = Regex.Replace(chart3ElementCaption.Text, "<[\\s\\S]*?>", String.Empty).Replace("&nbsp;", " ");
            e.CurrentWorksheet.Rows[63].Cells[0].Value = Regex.Replace(Label3.Text, "<[\\s\\S]*?>", String.Empty).Replace("&nbsp;", " ");
            ReportExcelExporter.ChartExcelExport(e.CurrentWorksheet.Rows[64].Cells[0], UltraChart3);

            e.CurrentWorksheet.MergedCellsRegions.Add(88, 0, 88, 9);
            e.CurrentWorksheet.Rows[88].Cells[0].Value = Regex.Replace(chart4ElementCaption.Text, "<[\\s\\S]*?>", String.Empty).Replace("&nbsp;", " ");
            ReportExcelExporter.ChartExcelExport(e.CurrentWorksheet.Rows[89].Cells[0], UltraChart4);
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraWebGrid1.Width = 1200;
            UltraChart1.Width = 1200;
            UltraChart2.Width = 1200;
            UltraChart3.Width = 1200;
            UltraChart4.Width = 1200;

            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ReportPDFExporter1.PdfExporter.TargetPaperSize = new PageSize(section1.PageSize.Height, 1000f);

            ReportPDFExporter1.Export(headerLayout, section1);
            
            ReportPDFExporter.AddTextContent(section1, chart1ElementCaption.Text, 14, false);
            ReportPDFExporter.AddTextContent(section1, Label1.Text, 12, false);
            ReportPDFExporter1.Export(UltraChart1, section1);

            ReportPDFExporter.AddTextContent(section1, chart2ElementCaption.Text, 14, false);
            ReportPDFExporter.AddTextContent(section1, Label2.Text, 12, false);
            ReportPDFExporter1.Export(UltraChart2, section1);

            ReportPDFExporter.AddTextContent(section1, chart3ElementCaption.Text, 14, false);
            ReportPDFExporter.AddTextContent(section1, Label3.Text, 12, false);
            ReportPDFExporter1.Export(UltraChart3, section1);

            ReportPDFExporter.AddTextContent(section1, chart4ElementCaption.Text, 14, false);
            ReportPDFExporter1.Export(UltraChart4, section1);
        }

        void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            
        }

        #endregion
    }
}
