using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image = Infragistics.Documents.Reports.Graphics.Image;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Core;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Index;
using Infragistics.Documents.Reports.Report.TOC;
using Infragistics.Documents.Reports.Report.Tree;
using Infragistics.Documents.Reports.Report.Flow;
using Infragistics.Documents.Reports.Report.Grid;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.QuickTable;
using Infragistics.Documents.Reports.Report.QuickList;
using Infragistics.Documents.Reports.Report.QuickText;
using Infragistics.Documents.Reports.Report.Segment;
using Infragistics.Documents.Reports.Report.Band;
using System.IO;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using InitializeRowEventHandler = Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler;
using SerializationFormat = Dundas.Maps.WebControl.SerializationFormat;
using System.Globalization;
using Infragistics.Documents.Reports.Graphics;
using IList = Infragistics.Documents.Reports.Report.List.IList;

namespace Krista.FM.Server.Dashboards.reports.FNS_0002_0002
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtComments;
        private CustomParam Year;
        private CustomParam Month;
        private CustomParam regionChart;
        private CustomParam taxId;
        private CustomParam period;
 

        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1200; }
        }

        private int minScreenWidth
        {
            get { return IsSmallResolution ? 850 : CustomReportConst.minScreenWidth; }
        }

        private int minScreenHeight
        {
            get { return IsSmallResolution ? 700 : CustomReportConst.minScreenHeight; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            int start = Environment.TickCount;
            base.Page_PreLoad(sender, e);
            CRHelper.SaveToUserAgentLog(String.Format("Базовый прелоад {0}", Environment.TickCount - start));
            
            if (Year == null)
            {
                Year = UserParams.CustomParam("Year");
            }
            if (Month == null)
            {
                Month = UserParams.CustomParam("Month");
            }
            if (taxId == null)
            {
                taxId = UserParams.CustomParam("taxId");
            }
            if (regionChart == null)
            {
                regionChart = UserParams.CustomParam("regionChart");
            }
            if (period == null)
            {
                period = UserParams.CustomParam("period");
            }
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 10);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.40);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;
            UltraGridExporter1.Visible = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
            CRHelper.SaveToUserAgentLog(String.Format("Остальной прелоад {0}", Environment.TickCount - start));
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.HeaderChildCellHeight = 100;

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.95);
            UltraChart1.ChartType = ChartType.DoughnutChart3D;
            UltraChart1.DoughnutChart3D.OthersCategoryPercent = 0;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Tooltips.FormatString = "<SERIES_LABEL>\n<DATA_VALUE:N2> ";
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.Axis.X.Extent = 70;
            UltraChart1.TitleLeft.HorizontalAlign = System.Drawing.StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = UltraChart1.Axis.X.Extent;
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";            
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 45;     
            UltraChart1.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.X.Margin.Near.Value = 2;
            UltraChart1.Axis.Y.Margin.Near.Value = 2;
            UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;    
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);      
            UltraChart1.Data.SwapRowsAndColumns = false;
            UltraChart1.LineChart.NullHandling = NullHandling.DontPlot;
            UltraChart1.Transform3D.XRotation = 90;
            UltraChart1.Transform3D.YRotation = 90;

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight);
            UltraChart2.ChartType = ChartType.DoughnutChart3D;
            UltraChart2.DoughnutChart3D.OthersCategoryPercent = 0;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.Tooltips.FormatString = "<SERIES_LABEL>\n<DATA_VALUE:N2> ";
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.Axis.X.Extent = 70;
            UltraChart2.TitleLeft.HorizontalAlign = System.Drawing.StringAlignment.Center;
            UltraChart2.TitleLeft.Margins.Bottom = UltraChart2.Axis.X.Extent;
            UltraChart2.Axis.X.Labels.Visible = true;
            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Bottom;
            UltraChart2.Legend.SpanPercentage = 45;
            UltraChart2.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Border.Thickness = 0;
            UltraChart2.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Axis.X.Margin.Near.Value = 2;
            UltraChart2.Axis.Y.Margin.Near.Value = 2;
            UltraChart2.ColorModel.ModelStyle = ColorModels.PureRandom;
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.Data.SwapRowsAndColumns = false;
            UltraChart2.LineChart.NullHandling = NullHandling.DontPlot;
            UltraChart2.Transform3D.XRotation = 90;
            UltraChart2.Transform3D.YRotation = 90;

            UltraChart3.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);
            UltraChart3.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight);
            UltraChart3.ChartType = ChartType.DoughnutChart3D;
            UltraChart3.DoughnutChart3D.OthersCategoryPercent = 0;
            UltraChart3.Border.Thickness = 0;
            UltraChart3.Tooltips.FormatString = "<SERIES_LABEL>\n<DATA_VALUE:N2> ";
            UltraChart3.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart3.Axis.X.Extent = 70;
            UltraChart3.TitleLeft.HorizontalAlign = System.Drawing.StringAlignment.Center;
            UltraChart3.TitleLeft.Margins.Bottom = UltraChart3.Axis.X.Extent;
            UltraChart3.Axis.X.Labels.Visible = true;
            UltraChart3.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            UltraChart3.Legend.Visible = true;
            UltraChart3.Legend.Location = LegendLocation.Bottom;
            UltraChart3.Legend.SpanPercentage = 45;
            UltraChart3.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart3.Border.Thickness = 0;
            UltraChart3.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart3.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart3.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart3.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart3.Axis.X.Margin.Near.Value = 2;
            UltraChart3.Axis.Y.Margin.Near.Value = 2;
            UltraChart3.ColorModel.ModelStyle = ColorModels.PureRandom;
            UltraChart3.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart3.Data.SwapRowsAndColumns = false;
            UltraChart3.LineChart.NullHandling = NullHandling.DontPlot;
            UltraChart3.Transform3D.XRotation = 90;
            UltraChart3.Transform3D.YRotation = 90;
        }

        void UltraChart_InterpolateValues(object sender, InterpolateValuesEventArgs e)
        {
        
        }

        void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
           

        }

        protected override void Page_Load(object sender, EventArgs e)
        {

            int start = Environment.TickCount;
            base.Page_Load(sender, e);
            CRHelper.SaveToUserAgentLog(String.Format("Базовый лоад {0}", Environment.TickCount - start));

            start = Environment.TickCount;
            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FNS_0002_0002_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                int endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string endMonth = dtDate.Rows[0][3].ToString();
                int firstYear = 2006;
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SelectLastNode();

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(endMonth, true);

                ComboKD.Width = 370;
                ComboKD.Title = "Вид налога";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillTaxesKDIncludingList());
            }

           string firstyear = "2005";
           Year.Value = ComboYear.SelectedValue;
           Month.Value = ComboMonth.SelectedValue;
           taxId.Value = ComboKD.SelectedValue;
           period.Value = RadioButtonList1.SelectedValue;
           //regionChart.Value = RegionsNamingHelper.LocalBudgetUniqueNames["Самара"];
           Page.Title = "Динамика поступления налогов";
            if (RadioButtonList1.SelectedIndex == 1)
            {
                period.Value = "[Measures].[С начала года]";
                string monthcaption = Convert.ToString(ComboMonth.SelectedIndex + 1);
                if (monthcaption.Length == 1)
                {
                    monthcaption = "0" + monthcaption; 
                }
                Label2.Text = String.Format("Анализ поступления ({2}) на 01.{1}.{0} года", ComboYear.SelectedValue, monthcaption, ComboKD.SelectedValue);
            }
            else 
            {
                period.Value = "[Measures].[За период]";
                Label2.Text = String.Format("Анализ поступления ({2}) за {1} {0} года", ComboYear.SelectedValue, ComboMonth.SelectedValue.ToLower(), ComboKD.SelectedValue);
            }
           Label1.Text = Page.Title;
           UserParams.PeriodYear.Value = "2008";
           UltraWebGrid.Bands.Clear();
           UltraWebGrid.DataBind();
           UltraChart1.DataBind();
           UltraChart2.DataBind();
           UltraChart3.DataBind();
           
           string patternValue = UserParams.StateArea.Value;
           int defaultRowIndex = 1;
           UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
           ActiveGridRow(row);
           CRHelper.SaveToUserAgentLog(String.Format("Остльной лоад {0}", Environment.TickCount - start));            
        }
        #region Обработчики грида

        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {

        }

       protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {

                string query = DataProvider.GetQueryText("FNS_0002_0002_compare_Grid");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Вид деятельности", dtGrid);
                if (dtGrid.Rows.Count > 0)
                {
                    int oldcount = dtGrid.Columns.Count;
                    DataColumn column = new DataColumn("Поступление по налогу;%", typeof(string));
                    dtGrid.Columns.Add(column);
                    column = new DataColumn("Поступление по налогу;тыс.руб.", typeof(string));
                    dtGrid.Columns.Add(column);
                    column = new DataColumn("Переплата по налогу;%", typeof(string));
                    dtGrid.Columns.Add(column);
                    column = new DataColumn("Переплата по налогу;тыс.руб.", typeof(string));
                    dtGrid.Columns.Add(column);
                    column = new DataColumn("Недоимка по налогу;%", typeof(string));
                    dtGrid.Columns.Add(column);
                    column = new DataColumn("Недоимка по налогу;тыс.руб.", typeof(string));
                    dtGrid.Columns.Add(column);

                    for (int i = 0; i < dtGrid.Rows.Count; i++)
                    {

                        dtGrid.Rows[i][oldcount] = string.Format("{0:N2}", dtGrid.Rows[i][oldcount - 6]);
                        dtGrid.Rows[i][oldcount + 1] = string.Format("{0:N2}", dtGrid.Rows[i][oldcount - 3]);
                        dtGrid.Rows[i][oldcount + 2] = string.Format("{0:N2}", dtGrid.Rows[i][oldcount - 5]);
                        dtGrid.Rows[i][oldcount + 3] = string.Format("{0:N2}", dtGrid.Rows[i][oldcount - 2]);
                        dtGrid.Rows[i][oldcount + 4] = string.Format("{0:N2}", dtGrid.Rows[i][oldcount - 4]);
                        dtGrid.Rows[i][oldcount + 5] = string.Format("{0:N2}", dtGrid.Rows[i][oldcount - 1]);

                    }

                    for (int i = 0; i < 6; i++)
                    {
                        dtGrid.Columns.RemoveAt(6);
                    }

                    dtGrid.AcceptChanges();
                    UltraWebGrid.DataSource = dtGrid;
                }
        }

        private static string GetChartQuarterStr(string period)
        {
            string[] strs = period.Split(' ');
            if (strs.Length > 1)
            {
                double quarterNumber = Convert.ToInt32(strs[1]);
                return string.Format("{0} квартал", quarterNumber);
            }
            else
            {
                return period;
            }
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FNS_0002_0002_compare_Chart1");
            DataTable dtChart1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);
            for (int i = 0; i < dtChart1.Rows.Count; i++)
            {
                CRHelper.SaveToErrorLog(dtChart1.Rows[i][0].ToString());
                if (dtChart1.Rows[i][0].ToString() == "Оптовая и розничная торговля; ремонт автотранспортных средств, мотоциклов, бытовых изделий и предметов личного пользования")
                {
                    dtChart1.Rows[i][0] = "Оптовая и розничная торговля; ремонт автотранспортных средств,\n мотоциклов, бытовых изделий и предметов личного пользования";
                }
            }

            UltraChart1.DataSource = dtChart1;
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FNS_0002_0002_compare_Chart2");
            DataTable dtChart1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);
            for (int i = 0; i < dtChart1.Rows.Count; i++)
            {
                if (dtChart1.Rows[i][0].ToString() == "Оптовая и розничная торговля; ремонт автотранспортных средств, мотоциклов, бытовых изделий и предметов личного пользования")
                {
                    dtChart1.Rows[i][0] = "Оптовая и розничная торговля; ремонт автотранспортных средств,\n мотоциклов, бытовых изделий и предметов личного пользования";
                }
            }
            UltraChart2.DataSource = dtChart1;
        }

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FNS_0002_0002_compare_Chart3");
            DataTable dtChart1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);
            for (int i = 0; i < dtChart1.Rows.Count; i++)
            {
                CRHelper.SaveToErrorLog(dtChart1.Rows[i][0].ToString());
                if (dtChart1.Rows[i][0].ToString() == "Оптовая и розничная торговля; ремонт автотранспортных средств, мотоциклов, бытовых изделий и предметов личного пользования")
                {
                    dtChart1.Rows[i][0] = "Оптовая и розничная торговля; ремонт автотранспортных средств,\n мотоциклов, бытовых изделий и предметов личного пользования";
                }
            }
            UltraChart3.DataSource = dtChart1;
            
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {

        }

        void UltraWebGrid1_DataBound(object sender, EventArgs e)
        {

        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width, bool hidden)
        {
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(300);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            dtGrid.AcceptChanges();
            UltraWebGrid.DataSource = dtGrid;
            for (int k = 1; k < e.Layout.Bands[0].Columns.Count; k++)
            {
                string formatString = "N2";
                e.Layout.Bands[0].Columns[k].Format = formatString;
                e.Layout.Bands[0].Columns[k].Width = 100;
                e.Layout.Bands[0].Columns[k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[k].CellStyle.Padding.Right = 5;
                e.Layout.Bands[0].Columns[k].Header.RowLayoutColumnInfo.OriginY = 1;
            }
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.OriginY = 0;
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.SpanY = 2;
            e.Layout.Bands[0].Columns[1].Header.Caption = "Поступление по налогу";
            e.Layout.Bands[0].Columns[2].Header.Caption = "Переплата по налогу";
            e.Layout.Bands[0].Columns[3].Header.Caption = "Недоимка по налогу";
            e.Layout.Bands[0].Columns[4].Header.Caption = "Поступление по налогу";
            e.Layout.Bands[0].Columns[5].Header.Caption = "Переплата по налогу";
            e.Layout.Bands[0].Columns[6].Header.Caption = "Недоимка по налогу";
            e.Layout.Bands[0].Columns[7].Header.Caption = "Поступление по налогу,%";
            e.Layout.Bands[0].Columns[8].Header.Caption = "Поступление по налогу";
            e.Layout.Bands[0].Columns[9].Header.Caption = "Переплата по налогу,%";
            e.Layout.Bands[0].Columns[10].Header.Caption = "Переплата по налогу";
            e.Layout.Bands[0].Columns[11].Header.Caption = "Недоимка по налогу,%";
            e.Layout.Bands[0].Columns[12].Header.Caption = "Недоимка по налогу";

            ColumnHeader ch = new ColumnHeader(true);
            ch.Caption = String.Format("{0} {1} год", ComboMonth.SelectedValue, Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 1));
            ch.Title = "";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 1;
            ch.RowLayoutColumnInfo.SpanX = 3;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = String.Format("{0} {1} год", ComboMonth.SelectedValue, ComboYear.SelectedValue); 
            ch.Title = "";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 4;
            ch.RowLayoutColumnInfo.SpanX = 3;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = "Отклонение от аналогичного периода предыдущего года";
            ch.Title = "";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 7;
            ch.RowLayoutColumnInfo.SpanX = 6;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);
        }
        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = e.Row.Cells.Count - 2; i > e.Row.Cells.Count - 7; i -= 2)
            {
                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "Снижение к прошлому году";
                    }
                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "Рост к прошлому году";
                    }
                }

                string style = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                e.Row.Cells[i].Style.CustomRules = style;
            }

           
        }
        
        #endregion

        #region Обработчики диаграмы
        
        void UltraChart_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {

        }
        #endregion
        
        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {

            ActiveGridRow(e.Row);
        }

        protected void UltraWebGrid1_ActiveRowChange(object sender, RowEventArgs e)
        {

            ActiveGridRow(e.Row);
        }
        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            ActiveGridRow(UltraWebGrid.Rows[3]);
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Page.Title;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = Label2.Text;
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            if (e.CurrentColumnIndex > 0)
            {
                if (e.CurrentColumnIndex < 4)
                {
                    e.HeaderText = String.Format("{0} {1} год", ComboMonth.SelectedValue, Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 1));
                }
                else
                if (e.CurrentColumnIndex < 7)
                {
                    e.HeaderText = String.Format("{0} {1} год", ComboMonth.SelectedValue, ComboYear.SelectedValue);
                }
                else
                {
                    e.HeaderText = "Отклонение от аналогичного периода предыдущего года";
                }
            }
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            e.CurrentWorksheet.ImageBackground = null;
            int columnCount = UltraWebGrid.Columns.Count;
            int width = 300;
            e.CurrentWorksheet.Rows[7].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.CurrentWorksheet.Columns[0].Width = width * 90;
            e.CurrentWorksheet.Columns[1].Width = width * 17;
            e.CurrentWorksheet.Columns[2].Width = width * 17;
            e.CurrentWorksheet.Columns[3].Width = width * 17;
            e.CurrentWorksheet.Columns[4].Width = width * 17;
            e.CurrentWorksheet.Columns[5].Width = width * 17;
            e.CurrentWorksheet.Columns[6].Width = width * 17;
            e.CurrentWorksheet.Columns[7].Width = width * 17;
            e.CurrentWorksheet.Columns[8].Width = width * 17;
            e.CurrentWorksheet.Columns[9].Width = width * 17;
            e.CurrentWorksheet.Columns[10].Width = width * 17;
            e.CurrentWorksheet.Columns[11].Width = width * 17;
            e.CurrentWorksheet.Columns[12].Width = width * 17;
            e.CurrentWorksheet.Columns[13].Width = width * 17;
            e.CurrentWorksheet.Columns[14].Width = width * 17;

            int columnCountt = UltraWebGrid.Columns.Count;
            for (int i = 1; i < columnCountt; i = i + 1)
            {
                for (int j = 5; j < 20; j++)
                {
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                }

                e.CurrentWorksheet.Rows[6].Cells[i].CellFormat.FillPattern = FillPatternStyle.None;
            }

            e.CurrentWorksheet.Rows[6].Cells[0].CellFormat.FillPattern = FillPatternStyle.None;

            int columnCounttt = UltraWebGrid.Columns.Count;
            for (int i = 1; i < columnCounttt; i = i + 1)
            {
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00";
                e.CurrentWorksheet.Columns[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
            }
            for (int i = 4; i < UltraWebGrid.Rows.Count; i = i + 1)
            {
                e.CurrentWorksheet.Rows[i].Height = 12 * 35;//Ширина строк
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            } 
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Динамика долговой нагрузки ");
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font.Bold = true;
            title.AddContent(Page.Title);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.AddContent(Label2.Text);
            title.AddLineBreak();
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            e.Section.AddPageBreak();
            IText title = e.Section.AddText();
            title.AddContent(ChartCaption1.Text);

            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart1);
            e.Section.AddImage(img);
            e.Section.AddPageBreak();
            title = e.Section.AddText();
            
            title.AddContent(ChartCaption2.Text);

            img = UltraGridExporter.GetImageFromChart(UltraChart2);
            e.Section.AddImage(img);

            e.Section.AddPageBreak();
            title = e.Section.AddText();

            title.AddContent(ChartCaption3.Text);

            img = UltraGridExporter.GetImageFromChart(UltraChart3);
            e.Section.AddImage(img);

        }

        #endregion   
   }

    public class ReportSection : ISection
    {
        private readonly bool withFlowColumns;
        private readonly ISection section;
        private IFlow flow;
        private ITableCell titleCell;

        public ReportSection(Report report, bool withFlowColumns)
        {
            section = report.AddSection();
            ITable table = section.AddTable();
            ITableRow row = table.AddRow();
            titleCell = row.AddCell();
        }

        public void AddFlowColumnBreak()
        {
            if (flow != null)
                flow.AddColumnBreak();
        }

        public ContentAlignment PageAlignment
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IBand AddBand()
        {
            return section.AddBand();
        }

        #region ISection members
        public ISectionHeader AddHeader()
        {
            throw new NotImplementedException();
        }

        public ISectionFooter AddFooter()
        {
            throw new NotImplementedException();
        }

        public IStationery AddStationery()
        {
            throw new NotImplementedException();
        }

        public IDecoration AddDecoration()
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage()
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage(PageSize size)
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage(float width, float height)
        {
            throw new NotImplementedException();
        }

        public ISegment AddSegment()
        {
            throw new NotImplementedException();
        }

        public IQuickText AddQuickText(string text)
        {
            throw new NotImplementedException();
        }

        public IQuickImage AddQuickImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            throw new NotImplementedException();
        }

        public IQuickList AddQuickList()
        {
            throw new NotImplementedException();
        }

        public IQuickTable AddQuickTable()
        {
            throw new NotImplementedException();
        }

        public IText AddText()
        {
            return this.titleCell.AddText();
        }

        public IImage AddImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            if (flow != null)
                return flow.AddImage(image);
            return this.section.AddImage(image);
        }

        public IMetafile AddMetafile(Metafile metafile)
        {
            throw new NotImplementedException();
        }

        public IRule AddRule()
        {
            throw new NotImplementedException();
        }

        public IGap AddGap()
        {
            throw new NotImplementedException();
        }

        public IGroup AddGroup()
        {
            throw new NotImplementedException();
        }

        public IChain AddChain()
        {
            throw new NotImplementedException();
        }

        public ITable AddTable()
        {
            return this.section.AddTable();
        }

        public IGrid AddGrid()
        {
            throw new NotImplementedException();
        }

        public IFlow AddFlow()
        {
            throw new NotImplementedException();
        }

        public IList AddList()
        {
            throw new NotImplementedException();
        }

        public ITree AddTree()
        {
            throw new NotImplementedException();
        }

        public ISite AddSite()
        {
            throw new NotImplementedException();
        }

        public ICanvas AddCanvas()
        {
            throw new NotImplementedException();
        }

        public IRotator AddRotator()
        {
            throw new NotImplementedException();
        }

        public IContainer AddContainer(string name)
        {
            throw new NotImplementedException();
        }

        public ICondition AddCondition(IContainer container, bool fit)
        {
            throw new NotImplementedException();
        }

        public IStretcher AddStretcher()
        {
            throw new NotImplementedException();
        }

        public void AddPageBreak()
        {
            throw new NotImplementedException();
        }

        public ITOC AddTOC()
        {
            throw new NotImplementedException();
        }

        public IIndex AddIndex()
        {
            throw new NotImplementedException();
        }

        public bool Flip
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public PageSize PageSize
        {
            get { throw new NotImplementedException(); }
            set { this.section.PageSize = new PageSize(2560, 1350); }
        }

        public PageOrientation PageOrientation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }



        public Borders PageBorders
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.Margins PageMargins
        {
            get { return this.section.PageMargins; }
            set { throw new NotImplementedException(); }
        }

        public Paddings PagePaddings
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Background PageBackground
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.Section.PageNumbering PageNumbering
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public SectionLineNumbering LineNumbering
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Report Parent
        {
            get { return this.section.Parent; }
        }

        public IEnumerable Content
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
   
}