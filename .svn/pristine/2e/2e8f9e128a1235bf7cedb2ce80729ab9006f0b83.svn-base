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
using System.Drawing;
using IList = Infragistics.Documents.Reports.Report.List.IList;

namespace Krista.FM.Server.Dashboards.reports.FO_0001_0004
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtComments;
        private CustomParam Year;
        private CustomParam PprevYear;
        private CustomParam PrevYear;
        private CustomParam regionChart;

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
           
            if (Year == null)
            {
                Year = UserParams.CustomParam("Year");
            }
            if (PprevYear == null)
            {
                PprevYear = UserParams.CustomParam("PprevYear");
            }
            if (PrevYear == null)
            {
                PrevYear = UserParams.CustomParam("PrevYear");
            }
            if (regionChart == null)
            {
                regionChart = UserParams.CustomParam("regionChart");
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
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.95));
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.50);
            UltraChart1.ChartType = ChartType.SplineChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N2> ";
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.Axis.X.Extent = 70;
            UltraChart1.TitleLeft.HorizontalAlign = System.Drawing.StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = UltraChart1.Axis.X.Extent;
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";            
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 15;     
            UltraChart1.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.X.Margin.Near.Value = 2;
            UltraChart1.Axis.Y.Margin.Near.Value = 2;
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomLinear;    
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.Data.SwapRowsAndColumns = false;
            UltraChart1.SplineChart.NullHandling = NullHandling.DontPlot;
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
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
                int firstYear = 2006;
                int endYear = 2011;
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState("2011", true);
            }
            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(UltraChart1);
            }
           string firstyear = "2005";
           Year.Value = ComboYear.SelectedValue;
           PrevYear.Value = Convert.ToString( Convert.ToInt32(ComboYear.SelectedValue) - 1);
           PprevYear.Value = Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 2);
           regionChart.Value = RegionsNamingHelper.LocalBudgetUniqueNames["Самара"];
           Page.Title = "Динамика долговой нагрузки местных бюджетов (без учета поселений)";
           Label1.Text = Page.Title;     
           UserParams.PeriodYear.Value = "2008";
           Label2.Text = String.Format("за {0} год", ComboYear.SelectedValue, (UltraWebGrid.Columns.Count - 3));
           string patternValue = UserParams.StateArea.Value;
           int defaultRowIndex = 1;
           UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
           ActiveGridRow(row);
           UltraWebGrid.Bands.Clear();
           UltraWebGrid.DataBind();
           UltraChart1.DataBind();
           CRHelper.SaveToUserAgentLog(String.Format("Остльной лоад {0}", Environment.TickCount - start));            
        }

        #region Обработчики грида
        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {
            if (row == null)
                return;
            if (row.Index < 1 &&
                chartWebAsyncPanel.IsAsyncPostBack)
            {
                //  Response.End();
                return;
            }
            if (row != null && row.Cells.Count > 0 && row.Cells[0].Value != null)
            {
                if (!row.Cells[0].Value.ToString().Contains("Среднее"))
                {
                    UltraChart1.Visible = true;
                    string subject = "субъекту";
                    if (RegionsNamingHelper.LocalBudgetTypes[row.Cells[0].Value.ToString()].Contains("ГО"))
                    {
                        subject = "городскому округу";
                    }
                    else
                    {
                        subject = "муниципальному району";
                    }
                    regionChart.Value = RegionsNamingHelper.LocalBudgetUniqueNames[row.Cells[0].Value.ToString()];
                    ChartCaption1.Text = String.Format("Сравнение долговой нагрузки за {2}, {1} и {0} годы по {4} {3}, в %", ComboYear.SelectedValue, Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 1), Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 2), row.Cells[0].Value.ToString(), subject);
                    UltraChart1.DataBind();
                }
                else
                {
                    if (row.Cells[0].Value.ToString().Contains("городским"))
                    {
                        regionChart.Value = "[Районы__Сопоставимый].[Районы__Сопоставимый].[Все районы].[Среднее по городским округам]";
                    }
                    else if (row.Cells[0].Value.ToString().Contains("районам"))
                    {
                        regionChart.Value = "[Районы__Сопоставимый].[Районы__Сопоставимый].[Все районы].[Среднее по муниципальным районам]";
                    }
                    else if (row.Cells[0].Value.ToString().Contains("образованиям"))
                    {
                        regionChart.Value = "[Районы__Сопоставимый].[Районы__Сопоставимый].[Среднее по муниципальным образованиям]";
                    }
                    ChartCaption1.Text = " ";
                    ChartCaption1.Text = String.Format("Сравнение долговой нагрузки за {2}, {1} и {0} годы {4}, в %", ComboYear.SelectedValue, Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 1), Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 2), row.Cells[0].Value.ToString(), row.Cells[0].Value.ToString());
                    UltraChart1.Visible = false;
                }
            }
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {

                string query = DataProvider.GetQueryText("FO_0001_0004_compare_Grid");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtGrid);
                for (int k = 0; k < dtGrid.Rows.Count; k++)
                {
                    DataRow row = dtGrid.Rows[k];

                   for (int i = 1; i < row.ItemArray.Length; i++)
                    {
                        if (row[i] != DBNull.Value)
                        {
                            if (Convert.ToDouble(row[i]) == 0.000)
                            {
                                row[i] = DBNull.Value;
                            }
                            else  dtGrid.Rows[k][i] = string.Format("{0:N2}", dtGrid.Rows[k][i]);
                          
                        }
                    }  
                }
             if (dtGrid.Rows.Count > 0)
                {             
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
            string query = DataProvider.GetQueryText("FO_0001_0004_compare_Chart1");
            DataTable dtChart1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);

            if (dtChart1.Rows.Count > 0)
            {
                DataTable newDtChart = new DataTable();
                DataColumn column = new DataColumn("УДН", typeof(string));
                newDtChart.Columns.Add(column);

                {
                    for (int i = 1; i < 13; i++)
                    {
                        DataColumn monthColumn = new DataColumn(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(i)), typeof(double));
                        newDtChart.Columns.Add(monthColumn);
                    }
                }
                int year = 0;
                DataRow currRow = null;
                foreach (DataRow row in dtChart1.Rows)
                {
                    int currYear = 0;
                    string period = string.Empty;
                    double measureValue = double.MinValue;
                    try
                    {

                        if (row[0] != DBNull.Value)
                        {
                            period = row[0].ToString();
                            period = GetChartQuarterStr(period);
                        }
                        if (!(row.ItemArray.Length < 1))
                        {
                            if (row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                            {
                                currYear = Convert.ToInt32(row[1]);
                            }
                        }
                        if (!(row.ItemArray.Length < 2))
                        {
                            if (row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                            {
                                measureValue = Convert.ToDouble(row[2]);
                            }
                        }
                    }
                    catch { }
                    if (year != currYear)
                    {
                        year = currYear;
                        DataRow newRow = newDtChart.NewRow();
                        newRow[0] = year;
                        newDtChart.Rows.Add(newRow);

                        currRow = newRow;
                    }
                    if (currRow != null && newDtChart.Columns.Contains(period) && measureValue != double.MinValue)
                    {
                        currRow[period] = measureValue;
                    }
                }
                if (newDtChart.Rows.Count > 0)
                {
                    UltraChart1.DataSource = newDtChart;
                }
                else
                {
                    ChartCaption1.Text = " ";                  
                    UltraChart1.Visible = false;
                }
            }
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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            string[] months = new string[13];
            string[] dates = new string[13];
            months[1] = "Январь";
            months[2] = "Февраль";
            months[3] = "Март";
            months[4] = "Апрель";
            months[5] = "Май";
            months[6] = "Июнь";
            months[7] = "Июль";
            months[8] = "Август";
            months[9] = "Сентябрь";
            months[10] = "Октябрь";
            months[11] = "Ноябрь";
            months[12] = "Декабрь";
            months[0] = "на 01.01.";
            dates[1] = "на 01.02.";
            dates[2] = "на 01.03.";
            dates[3] = "на 01.04.";
            dates[4] = "на 01.05.";
            dates[5] = "на 01.06.";
            dates[6] = "на 01.07.";
            dates[7] = "на 01.08.";
            dates[8] = "на 01.09.";
            dates[9] = "на 01.10.";
            dates[10] = "на 01.11.";
            dates[11] = "на 01.12.";
            dates[12] = "на 01.01.";
            dates[0] = "за январь прошлого года";
            for (int k = 1; k < e.Layout.Bands[0].Columns.Count; k++)
            {
                e.Layout.Bands[0].Columns[k].Width = 85;
                e.Layout.Bands[0].Columns[k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[k].CellStyle.Padding.Right = 5;
                for (int i = 1; i < months.Length; i++)
                {
                    string month = months[i];
                    if (e.Layout.Bands[0].Columns[k].Header.Caption.Contains(month))
                    {

                        if (i == 12) 
                        {
                            e.Layout.Bands[0].Columns[k].Header.Caption = String.Format("на 01.01.{0}", Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue)+1 ));
                        }
                        else
                        e.Layout.Bands[0].Columns[k].Header.Caption = dates[i] + ComboYear.SelectedValue;
                    } 
                }
                CRHelper.SaveToErrorLog(e.Layout.Bands[0].Columns[k].Header.Caption);
                if (k == 1)
                {
                    e.Layout.Bands[0].Columns[k].Header.Caption = String.Format("на 01.01.{0}", Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 1));
                }
                else
                    if (k == 2)
                    {
                        e.Layout.Bands[0].Columns[k].Header.Caption = String.Format("на 01.01.{0}", Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue)));
                    }
                    else
                    {
                        if (e.Layout.Bands[0].Columns[k].Header.Caption.Contains(Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 2)))
                        {
                            e.Layout.Bands[0].Columns[k].Header.Caption = String.Format("на 01.01.{0}", Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 1));
                        }
                        else if (e.Layout.Bands[0].Columns[k].Header.Caption.Contains(Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 1)))
                        {
                            e.Layout.Bands[0].Columns[k].Header.Caption = String.Format("на 01.01.{0}", Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 0));
                        }
                    }
            } 

        }
        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[0].Value != null &&
                       (e.Row.Cells[0].Value.ToString().Contains("Среднее") || e.Row.Cells[0].Value.ToString().Contains("Всего")))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
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
            ActiveGridRow(UltraWebGrid.Rows[0]);
            e.CurrentWorksheet.PrintOptions.ScalingFactor = 65;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            ActiveGridRow(UltraWebGrid.Rows[3]);
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Page.Title;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = Label2.Text;
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
       
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {

            int columnCount = UltraWebGrid.Columns.Count;
            int width = 150;
            e.CurrentWorksheet.Rows[7].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.CurrentWorksheet.Columns[0].Width = width * 65;
            e.CurrentWorksheet.Columns[1].Width = width * 20;
            e.CurrentWorksheet.Columns[2].Width = width * 20;
            e.CurrentWorksheet.Columns[3].Width = width * 20;
            e.CurrentWorksheet.Columns[4].Width = width * 20;
            e.CurrentWorksheet.Columns[5].Width = width * 20;
            e.CurrentWorksheet.Columns[6].Width = width * 20;
            e.CurrentWorksheet.Columns[7].Width = width * 20;
            e.CurrentWorksheet.Columns[8].Width = width * 20;
            e.CurrentWorksheet.Columns[9].Width = width * 20;
            e.CurrentWorksheet.Columns[10].Width = width * 20;
            e.CurrentWorksheet.Columns[11].Width = width * 20;
            e.CurrentWorksheet.Columns[12].Width = width * 20;
            e.CurrentWorksheet.Columns[13].Width = width * 20;
            e.CurrentWorksheet.Columns[14].Width = width * 20;

            int columnCountt = UltraWebGrid.Columns.Count;
            for (int i = 1; i < columnCountt; i = i + 1)
            {
                e.CurrentWorksheet.Rows[6].Cells[i].CellFormat.FillPattern = FillPatternStyle.None;                
                for (int j = 5; j < 20; j++)
                {
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                }
            }
            e.CurrentWorksheet.Rows[6].Cells[0].CellFormat.FillPattern = FillPatternStyle.None;
            int columnCounttt = UltraWebGrid.Columns.Count;
            for (int i = 1; i < columnCounttt; i = i + 1)
            {
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "0.00";
                e.CurrentWorksheet.Columns[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
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
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 16);
            title.Style.Font.Bold = true;
            title.AddContent(Page.Title);

            title = e.Section.AddText();
            font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 14);
            title.AddContent(Label2.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {

            IText title = e.Section.AddText();
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 14);
            title.Style.Font.Bold = true;
            title.AddContent(ChartCaption1.Text);
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart1);
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