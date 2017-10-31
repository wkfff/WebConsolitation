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
using Infragistics.Documents.Reports.Report.List;
using System.Drawing;
using System;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report.Tree;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Krista.FM.Server.Dashboards.Components;
using System.Globalization;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Band;
using Infragistics.Documents.Reports.Report.Flow;
using Infragistics.Documents.Reports.Report.Grid;
using Infragistics.Documents.Reports.Report.Index;
using Infragistics.Documents.Reports.Report.List;
using Infragistics.Documents.Reports.Report.QuickList;
using Infragistics.Documents.Reports.Report.QuickTable;
using Infragistics.Documents.Reports.Report.QuickText;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Segment;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.TOC;
using System.Collections;
using Infragistics.UltraChart.Shared.Styles;
using System.Collections.Generic;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Core;
using System.Collections.ObjectModel;
namespace Krista.FM.Server.Dashboards.reports.FO_0006_0002
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtComments;
        private CustomParam Faces;
        private CustomParam Units;
        private CustomParam Year;
        private CustomParam month;
        private CustomParam mul;
        private DataTable candleDT;
        private DataTable chart1DT;
        private Dictionary<DateTime, string> candleLabelsDictionary;
        private Dictionary<DateTime, string> candleLabelsDictionary1;
        private DateTime currDateTime;
        private DateTime lastDateTime;
        private CustomParam ufo;
        private CustomParam currYear;
        private CustomParam years;
        private CustomParam monthlab;
        // Текущая дата
        private CustomParam periodCurrentDate;
        // На неделю назад
        private CustomParam periodLastWeekDate;
        private int endYear = 2009;
        private string endmonth = "Декабрь";
        private int indexOfRow = 0;
        /// <summary>
        /// Выбраны ли
        /// федеральные округа
        /// </summary>
        ///
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
            base.Page_PreLoad(sender, e);
            if (periodCurrentDate == null)
            {
                periodCurrentDate = UserParams.CustomParam("period_current_date");
            }
            if (periodLastWeekDate == null)
            {
                periodLastWeekDate = UserParams.CustomParam("period_last_week_date");
            }
            start = Environment.TickCount;
            if (Faces == null)
            {
                Faces = UserParams.CustomParam("Faces");
            }
            if (monthlab == null)
            {
                monthlab = UserParams.CustomParam("monthlab");
            }
            if (Year == null)
            {
                Year = UserParams.CustomParam("Year");
            }
            if (month == null)
            {
                month = UserParams.CustomParam("month");
            }
            if (periodCurrentDate == null)
            {
                periodCurrentDate = UserParams.CustomParam("period_current_date");
            }
            if (currYear == null)
            {
                currYear = UserParams.CustomParam("currYear");
            }
            if (mul == null)
            {
                mul = UserParams.CustomParam("mul");
            }
            if (years == null)
            {
                years = UserParams.CustomParam("years");
            }
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.23);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.45);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.43);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid1_DataBound);
            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.33);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.ChartType = ChartType.PieChart;
            UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 20;
            UltraChart.PieChart.OthersCategoryPercent = 0;
            UltraChart.Border.Thickness = 0;
            UltraChart.CandleChart.SkipN = 0;
            UltraChart.CandleChart.HighLowVisible = true;
            UltraChart.CandleChart.ResetHighLowVisible();
            UltraChart.CandleChart.VolumeVisible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 20);
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL>";
            UltraChart.Tooltips.FormatString = "";
            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.X.Extent = 30;
            UltraChart.Axis.Y.Extent = 100;
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.23);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.40);
            UltraChart1.ChartType = ChartType.PieChart;
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.SpanPercentage = 33;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.PieChart.OthersCategoryPercent = 0;
            UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.CandleChart.SkipN = 0;
            UltraChart1.CandleChart.HighLowVisible = true;
            UltraChart1.CandleChart.ResetHighLowVisible();
            UltraChart1.CandleChart.VolumeVisible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 20);
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL>";
            UltraChart1.Tooltips.FormatString = "";
            UltraChart1.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart1.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.X.Extent = 30;
            UltraChart1.Axis.Y.Extent = 100;
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.TitleTop.Text = "Федеральные налоги";
            UltraChart1.TitleTop.HorizontalAlign = StringAlignment.Center;

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
            UltraGridExporter1.MultiHeader = false;
            UltraGridExporter1.HeaderChildCellHeight = 100;
        }

        void UltraChart_InterpolateValues(object sender, Infragistics.UltraChart.Shared.Events.InterpolateValuesEventArgs e)
        {

        }

        string meas = string.Empty;
        protected override void Page_Load(object sender, EventArgs e)
        {

            int start = Environment.TickCount;
            base.Page_Load(sender, e);
            CRHelper.SaveToUserAgentLog(String.Format("Базовый лоад {0}", Environment.TickCount - start));
            start = Environment.TickCount;
            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0006_0002_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            endYear = Convert.ToInt32(dtDate.Rows[0][0]);
            endmonth = dtDate.Rows[0][3].ToString();
            int firstYear = 2010;
            if (!Page.IsPostBack)
            {
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState((endYear).ToString(), true);

                ComboMonth.Title = "Месяц";

                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(endmonth, true);
            }
            string faces = string.Empty;
            if (RadioList.SelectedIndex == 0)
            {
                mul.Value = Convert.ToString(1000);
                meas = "тыс.руб";
            }
            else
            {
                mul.Value = Convert.ToString(1000000);
                meas = "млн.руб";
            }
            Collection<string> selectedValues = ComboYear.SelectedValues;
            if (selectedValues.Count > 0)
            {
                string gridDescendants = String.Empty;
                string chartDescendants = String.Empty;
                years.Value = String.Empty;
                for (int i = 0; i < selectedValues.Count; i++)
                {
                    string year = selectedValues[i];
                    string sign = string.Empty;
                    if (i != selectedValues.Count - 1)
                    {
                        sign = ",";
                    }
                    years.Value += string.Format("{1}.[Данные всех периодов].[{0}]{2}",
                    year, "[Период__Период].[Период__Период]", sign);
                }
            }
            UltraChart.Tooltips.FormatString = String.Format("<ITEM_LABEL>\n<DATA_VALUE:N2> %");
            UltraChart1.Tooltips.FormatString = String.Format("<ITEM_LABEL>\n<DATA_VALUE:N2> %");
            currDateTime = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), CRHelper.MonthNum(ComboMonth.SelectedValue), 01);
            currDateTime = currDateTime.AddMonths(1);
            Page.Title = "Справка о недоимке по платежам в консолидированный бюджет субъекта";
            Label1.Text = String.Format("Справка о недоимке по платежам в консолидированный бюджет субъекта, по состоянию на {0:dd.MM.yyyy} года, {1}", currDateTime, meas);
            GridCaption.Text = string.Format("Структура недоимки доходов консолидированного бюджета субъекта на {0:dd.MM.yyyy} года, {1}", currDateTime, meas);
            Label2.Text = string.Empty;
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodLastYear.Value = Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 1);
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            int defaultRowIndex = 0;
            UltraWebGrid.DataBind();
            UltraWebGrid1.DataBind();
            UltraChart.DataBind();
            UltraChart1.DataBind();
            UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, monthlab.Value, 0, defaultRowIndex);
            ActiveGridRow(row);
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
            int start = Environment.TickCount;
            string query = DataProvider.GetQueryText("FO_0006_0002_compare_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);
            dtGrid.Columns.RemoveAt(0);
            dtGrid.AcceptChanges();
            for (int i = 0; i < dtGrid.Rows.Count - 2; i++)
            {
                DateTime DateTime;
                if ((i == 0))
                {
                    DateTime = new DateTime(Convert.ToInt32(ComboYear.SelectedValue) - 1, CRHelper.MonthNum(dtGrid.Rows[i][0].ToString()), 01);
                }
                else
                DateTime = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), CRHelper.MonthNum(dtGrid.Rows[i][0].ToString()), 01);
                DateTime = DateTime.AddMonths(1);
                dtGrid.Rows[i][0] = String.Format("{0:dd.MM.yyyy}", DateTime);
            }
            UltraWebGrid.DataSource = dtGrid;
        }

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            int start = Environment.TickCount;
            string query = DataProvider.GetQueryText("FO_0006_0002_compare_Grid1");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);
            UltraWebGrid1.DataSource = dtGrid;
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {

        }

        void UltraWebGrid1_DataBound(object sender, EventArgs e)
        {

        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width, bool hidden)
        {
            CRHelper.FormatNumberColumn(layout.Bands[bandIndex].Columns[columnIndex], format);
            layout.Bands[bandIndex].Columns[columnIndex].Hidden = hidden;
            layout.Bands[bandIndex].Columns[columnIndex].Width = CRHelper.GetColumnWidth(width);
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(100);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            if (e.Layout.Bands[0].Columns.Count > 1)
            {
                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    if (i == 0)
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 1;
                    }
                    else
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                        e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(950 / UltraWebGrid.Columns.Count);
                    }
                    string formatString = "N0";
                    e.Layout.Bands[0].Columns[i].Format = formatString;
                    if (i > 0)
                    {
                        if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("Факт"))
                        {
                            e.Layout.Bands[0].Columns[i].Header.Caption = e.Layout.Bands[0].Columns[i].Header.Caption.Substring(0, 4);
                            e.Layout.Bands[0].Columns[i].Header.Caption = String.Format("{0}{1}", e.Layout.Bands[0].Columns[i].Header.Caption, " г.");
                        }
                        else if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("роста"))
                        {
                            e.Layout.Bands[0].Columns[i].Header.Caption = "% роста";
                        }
                    }
                }
                e.Layout.Bands[0].Columns[0].Header.Caption = "Период";
                e.Layout.Bands[0].Columns[1].Header.Caption = "Всего";
                e.Layout.Bands[0].Columns[2].Header.Caption = "Процент к налоговым доходам, %";
                e.Layout.Bands[0].Columns[2].Header.Title = "Удельный вес недоимки в общем объеме поступлений доходов в консолидированный бюджет субъекта";
                e.Layout.Bands[0].Columns[3].Header.Caption = "Федеральные налоги";
                e.Layout.Bands[0].Columns[4].Header.Caption = "Процент к налоговым доходам, %";
                e.Layout.Bands[0].Columns[4].Header.Title = "Удельный вес недоимки в общем объеме поступлений доходов в консолидированный бюджет субъекта";
                e.Layout.Bands[0].Columns[5].Header.Caption = "Региональные налоги";
                e.Layout.Bands[0].Columns[6].Header.Caption = "Процент к налоговым доходам, %";
                e.Layout.Bands[0].Columns[6].Header.Title = "Удельный вес недоимки в общем объеме поступлений доходов в консолидированный бюджет субъекта";
                e.Layout.Bands[0].Columns[7].Header.Caption = "Местные налоги";
                e.Layout.Bands[0].Columns[8].Header.Caption = "Процент к налоговым доходам, %";
                e.Layout.Bands[0].Columns[8].Header.Title = "Удельный вес недоимки в общем объеме поступлений доходов в консолидированный бюджет субъекта";
                e.Layout.Bands[0].Columns[9].Header.Caption = "Налоги со спец. налоговым режимом";
                e.Layout.Bands[0].Columns[10].Header.Caption = "Процент к налоговым доходам, %";
                e.Layout.Bands[0].Columns[10].Header.Title = "Удельный вес недоимки в общем объеме поступлений доходов в консолидированный бюджет субъекта";
                int count = e.Layout.Bands[0].Columns.Count;
                e.Layout.Bands[0].Columns[0].Hidden = false;
                e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[1].CellStyle.Padding.Right = 5;
                for (int k = 1; k < e.Layout.Bands[0].Columns.Count; k++)
                {
                    e.Layout.Bands[0].Columns[count - k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    e.Layout.Bands[0].Columns[count - k].CellStyle.Padding.Right = 5;
                }
                Collection<string> cellsCaption = new Collection<string>();
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            }
        }

        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(110);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            if (e.Layout.Bands[0].Columns.Count > 1)
            {
                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    if (i == 0)
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 1;
                    }
                    else
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                        e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(850 / UltraWebGrid.Columns.Count);
                    }
                    string formatString = "N1";
                    e.Layout.Bands[0].Columns[i].Format = formatString;
                    if (i > 0)
                    {
                        if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("Факт"))
                        {
                            e.Layout.Bands[0].Columns[i].Header.Caption = e.Layout.Bands[0].Columns[i].Header.Caption.Substring(0, 4);
                            e.Layout.Bands[0].Columns[i].Header.Caption = String.Format("{0}{1}", e.Layout.Bands[0].Columns[i].Header.Caption, " г.");
                        }
                    }
                }
                e.Layout.Bands[0].Columns[0].Header.Caption = "Показатели";
                e.Layout.Bands[0].Columns[1].Header.Caption = "Всего";
                e.Layout.Bands[0].Columns[2].Header.Caption = "Доля в структуре";
                e.Layout.Bands[0].Columns[2].Format = "N1";
                e.Layout.Bands[0].Columns[2].Header.Title = "Процент от общей суммы недоимки доходов";
                e.Layout.Bands[0].Columns[3].Header.Caption = "Прирост с начала года";
                e.Layout.Bands[0].Columns[3].Header.Title = "Прирост суммы недоимки с первого января отчетного года по текущую дату";
                int count = e.Layout.Bands[0].Columns.Count;
                e.Layout.Bands[0].Columns[0].Hidden = false;
                e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[1].CellStyle.Padding.Right = 5;
                for (int k = 1; k < e.Layout.Bands[0].Columns.Count; k++)
                {
                    e.Layout.Bands[0].Columns[count - k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    e.Layout.Bands[0].Columns[count - k].CellStyle.Padding.Right = 5;
                }
                Collection<string> cellsCaption = new Collection<string>();
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 1; (i < UltraWebGrid.Columns.Count); i += 2)
            {
                if (UltraWebGrid.Rows[e.Row.Index].Cells[0].Value.ToString() == "Прирост за месяц")
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            e.Row.Cells[i].Title = "Cнижение недоимки относительно прошлого месяца";
                        }
                        else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                            e.Row.Cells[i].Title = "Рост недоимки относительно прошлого месяца";
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }
                else
                    if (UltraWebGrid.Rows[e.Row.Index].Cells[0].Value.ToString() == "Прирост с начала года")
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                                e.Row.Cells[i].Title = "Cнижение недоимки относительно начала года";
                            }
                            else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                                e.Row.Cells[i].Title = "Рост недоимки относительно начала года";
                            }
                            e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        }
                    }
            }
        }

        protected void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            int columnIndex = 3;

            if (e.Row.Cells[columnIndex].Value != null && e.Row.Cells[columnIndex].Value.ToString() != string.Empty)
            {
                if (100 * Convert.ToDouble(e.Row.Cells[columnIndex].Value) < 0)
                {
                    e.Row.Cells[columnIndex].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                    e.Row.Cells[columnIndex].Title = "Cнижение недоимки относительно начала года";
                }
                else if (100 * Convert.ToDouble(e.Row.Cells[columnIndex].Value) > 0)
                {
                    e.Row.Cells[columnIndex].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                    e.Row.Cells[columnIndex].Title = "Рост недоимки относительно начала года";
                }
                e.Row.Cells[columnIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
            }
            if (e.Row.Index != 0)
            {
                if ((e.Row.Index > 1) && (e.Row.Index < 6))
                {
                    e.Row.Cells[0].Style.Padding.Left = 15;
                }
            }
            for (int i = 0; (i < UltraWebGrid1.Columns.Count); i ++)
            {
                if ((e.Row.Index == 1) || (e.Row.Index > 5))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }
        }

        #endregion

        #region Обработчики диаграмы
        DataTable dtChart = new DataTable();
        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            dtChart = new DataTable();
            string query = DataProvider.GetQueryText("Chart_query");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            if (dtChart.Rows.Count > 0)
            {
                UltraChart.DataSource = dtChart;
            }
        }
        DataTable dtChart1 = new DataTable();
        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            dtChart = new DataTable();
            string query = DataProvider.GetQueryText("Chart_query1");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            if (dtChart.Rows.Count > 0)
            {
                UltraChart1.DataSource = dtChart;
            }
        }

        void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {
                        string grbsName = box.DataPoint.Label;
                        box.DataPoint.Label = DataDictionariesHelper.GetFullFMGRBSNames(grbsName);

                        box.PE.ElementType = PaintElementType.Gradient;
                        box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;

                        int boxRow = box.Column;

                        if (boxRow == 0)
                        {
                            box.PE.Fill = Color.LightGreen;
                            box.PE.FillStopColor = Color.Green;
                        }
                        else
                        {
                            if (Convert.ToInt32(dtChart.Rows[0][box.Column + 1]) > Convert.ToInt32(dtChart.Rows[0][box.Column]))
                            {
                                box.PE.Fill = Color.LightGreen;
                                box.PE.FillStopColor = Color.Green;
                                box.DataPoint.Label += "\nРост объема фактических поступлений по сравнению с аналогичным периодом прошлого года";
                            }
                            else if (Convert.ToInt32(dtChart.Rows[0][box.Column + 1]) < Convert.ToInt32(dtChart.Rows[0][box.Column]))
                            {
                                box.PE.Fill = Color.LightPink;
                                box.PE.FillStopColor = Color.Red;
                                box.DataPoint.Label += "\nСнижение объема фактических поступлений по сравнению с аналогичным периодом прошлого года";
                            }
                        }
                    }
                }
            }
        }


        void UltraChart_FillSceneGraph1(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Polyline)
                {
                    Polyline poly = (Polyline)primitive;

                    if (poly.Series != null)
                    {
                        for (int j = 0; j < poly.points.Length; j++)
                        {
                            DataPoint point = poly.points[j];
                            CRHelper.SaveToErrorLog(dtChart1.Rows[point.Row][point.Column + 1].ToString());
                            if (dtChart1.Rows[point.Row][point.Column] != DBNull.Value)
                            {
                                if (Convert.ToDouble(dtChart1.Rows[point.Row][point.Column + 1]) > 1)
                                {
                                    point.DataPoint.Label = String.Format("Рост поступлений по сравнению с аналогичным периодом прошлого года");
                                }
                                else if (Convert.ToDouble(dtChart1.Rows[point.Row][point.Column + 1]) < 1)
                                {
                                    point.DataPoint.Label = String.Format("Снижение поступлений по сравнению с аналогичным периодом прошлого года");
                                }
                            }
                        }
                    }
                }
            }

            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            int lineStart = (int)xAxis.MapMinimum;
            int lineLength = (int)xAxis.MapMaximum;

            Line line = new Line();
            line.lineStyle.DrawStyle = LineDrawStyle.Dash;
            line.PE.Stroke = System.Drawing.Color.LightGray;
            line.PE.StrokeWidth = 2;
            line.p1 = new System.Drawing.Point(lineStart, (int)yAxis.Map(1));
            line.p2 = new System.Drawing.Point(lineStart + lineLength, (int)yAxis.Map(1));
            e.SceneGraph.Add(line);
        }


        void UltraChart_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {

        }
        #endregion

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }
        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            ActiveGridRow(UltraWebGrid.Rows[3]);
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text;
            e.CurrentWorksheet.Rows[UltraWebGrid.Rows.Count + 6].Cells[0].Value = GridCaption.Text;
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {

        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            //if (e.CurrentWorksheet.Name == Спр. о недоимке)
            int columnCount = UltraWebGrid.Columns.Count;
            int width = 300;
            e.CurrentWorksheet.Rows[7].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.CurrentWorksheet.Columns[0].Width = width * 17;
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
            }
            int columnCounttt = UltraWebGrid.Columns.Count;
            for (int i = 1; i < columnCounttt; i = i + 1)
            {
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#0";
                e.CurrentWorksheet.Columns[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
            }
            for (int i = 22; i < UltraWebGrid.Rows.Count; i = i + 1)
            {
                e.CurrentWorksheet.Rows[i].CellFormat.FormatString = "#,##0;[Red]-#,##0"; ;
                e.CurrentWorksheet.Rows[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Спр. о недоимке");
            Worksheet sheet2 = workbook.Worksheets.Add("Структура");
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
            UltraGridExporter.ChartExcelExport(sheet2.Rows[2].Cells[0], UltraChart);
            UltraGridExporter.ChartExcelExport(sheet2.Rows[2].Cells[9], UltraChart1);
            sheet2.Rows[1].Cells[1].Value = GridCaption.Text;
            UltraGridExporter1.ExcelExporter.ExcelStartRow = UltraWebGrid.Rows.Count + 9;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid1, sheet1, UltraWebGrid.Rows.Count + 9, 0);
            //UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

        }

        protected void RadioList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #endregion

        #region Экспорт в PDF
        ReportSection section1 = null;
        ReportSection section2 = null;
        PageSize size = new PageSize(200, 200);
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            Report report = new Report();
            section1 = new ReportSection(report, false);
            section2 = new ReportSection(report, false);
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid, section1);
            section1.PageSize = size;
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid1, section2);
            string label = Label2.Text.Replace("<br/>", "");
            if (IsExported) return;
            IsExported = true;
            IText title = section1.AddText();
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 16);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = section2.AddText();
            font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 14);
            title.AddContent(GridCaption.Text);

            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            section2.AddImage(img);

            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            img = UltraGridExporter.GetImageFromChart(UltraChart1);
            section2.AddImage(img);
        }

        bool IsExported = false;
        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            e.Section.PageSize = size;   
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
     
        }

        #endregion

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

            public Infragistics.Documents.Reports.Report.ContentAlignment PageAlignment
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

            public IMetafile AddMetafile(Infragistics.Documents.Reports.Graphics.Metafile metafile)
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

            public Infragistics.Documents.Reports.Report.List.IList AddList()
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
}