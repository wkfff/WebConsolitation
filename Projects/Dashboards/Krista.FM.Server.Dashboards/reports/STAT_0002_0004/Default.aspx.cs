using System;
//using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
//using System.Drawing;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
//using Font = Infragistics.Documents.Reports.Graphics.Font;
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
//using System.Drawing.Imaging;
using Infragistics.Documents.Reports.Report.Band;

using System.IO;
//using System.Drawing;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using InitializeRowEventHandler = Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler;
using SerializationFormat = Dundas.Maps.WebControl.SerializationFormat;
using System.Globalization;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report.List;
using System.Collections;
using IList = System.Collections.IList;


namespace Krista.FM.Server.Dashboards.reports.STAT_0002_0004
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;

        private DataTable dtComments;
        
        private CustomParam Faces;
        private CustomParam Units;

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
            get { return IsSmallResolution ? 850 : CustomReportConst.minScreenWidth; }//750
        }

        private int minScreenHeight
        {
            get { return IsSmallResolution ? 700 : CustomReportConst.minScreenHeight; }
        }

        public bool AllFO
        {
            get { return regionsCombo.SelectedIndex == 0; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            int start = Environment.TickCount;
            base.Page_PreLoad(sender, e);
            CRHelper.SaveToUserAgentLog(String.Format("Базовый прелоад {0}", Environment.TickCount - start));

          /*  start = Environment.TickCount;
            if (Faces == null)
            {
                Faces = UserParams.CustomParam("Faces");
            }

            if (Units == null)
            {
                Units = UserParams.CustomParam("Units");
            }
            */
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 10);// - 675);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.30);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 10);// - 675);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.30);
            UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid1_DataBound);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);// - 600);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.85);//818);

            //gridTd.Style.Add("Height", String.Format("{0}px", CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.30 + 10)));
            //chartTd.Style.Add("Height", String.Format("{0}px", CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.75 + 10)));

            UltraChart.ChartType = ChartType.StackColumnChart;
            UltraChart.Border.Thickness = 0;

            UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>\n <DATA_VALUE:N0> чел.";
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.Axis.X.Extent = 70;
            UltraChart.Axis.X.Labels.Visible = true;
            //UltraChart.TitleLeft.Visible = true;
            //UltraChart.TitleLeft.Text = "Чел.";
            UltraChart.TitleLeft.HorizontalAlign = System.Drawing.StringAlignment.Center;
            UltraChart.TitleLeft.HorizontalAlign = System.Drawing.StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = (int)UltraChart.Axis.X.Extent + (int)(UltraChart.Legend.SpanPercentage / 100) * (int)(UltraChart.Height.Value);
            //UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent;
           // UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent + (UltraChart.Legend.SpanPercentage) * (int)(UltraChart.Height.Value);
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";         
            //UltraChart.Legend.Margins.Right = Convert.ToInt32(UltraChart.Width.Value) / 2;
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 55;
            //UltraChart.Legend.Margins.Right = (int)(UltraChart.Width.Value - 200);
            //UltraChart.Legend.Margins.Bottom = (int)(UltraChart.Height.Value / 2);
            UltraChart.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Border.Thickness = 0;
            UltraChart.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            //UltraChart.Legend.FormatString.
            UltraChart.Axis.X.Margin.Near.Value = 2;
            UltraChart.Axis.Y.Margin.Near.Value = 2;
            //UltraChart.Axis.X.

            EmptyAppearance item = new EmptyAppearance();
            item.EnableLineStyle = true;
            item.EnablePoint = false;
            LineStyle style = new LineStyle();
            style.DrawStyle = LineDrawStyle.Dash;
            style.MidPointAnchors = false;
            item.LineStyle = style;
            UltraChart.LineChart.EmptyStyles.Add(item);
            //UltraChart.LineChart.NullHandling = NullHandling.InterpolateCustom;

            UltraChart.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart_ChartDrawItem);

            UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;

            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
           // UltraChart.InterpolateValues += new InterpolateValuesEventHandler(UltraChart_InterpolateValues);
/*
            LineAppearance lineAppearance3 = new LineAppearance();
            lineAppearance3.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance3.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance3.Thickness = 3;
            UltraChart.LineChart.LineAppearances.Add(lineAppearance3);

            LineAppearance lineAppearance1 = new LineAppearance();

            lineAppearance1.Thickness = 0;
            UltraChart.LineChart.LineAppearances.Add(lineAppearance1);

            LineAppearance lineAppearance2 = new LineAppearance();
            lineAppearance2.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance2.Thickness = 0;
            UltraChart.LineChart.LineAppearances.Add(lineAppearance2);
            */

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);// - 600);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.85);//818);

            UltraChart1.ChartType = ChartType.StackColumnChart;
            UltraChart1.Border.Thickness = 0;

            UltraChart1.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N0> чел.";
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.Axis.X.Extent = 70;
            //UltraChart1.TitleLeft.Visible = true;
            //UltraChart1.TitleLeft.Text = "Чел.";
            UltraChart1.TitleLeft.HorizontalAlign = System.Drawing.StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = UltraChart1.Axis.X.Extent;
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            //UltraChart1.Legend.Margins.Right = Convert.ToInt32(UltraChart1.Width.Value) / 2;
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 55;
            //UltraChart1.Legend.Margins.Right = (int)(UltraChart1.Width.Value - 200);
            //UltraChart1.Legend.Margins.Bottom = (int)(UltraChart1.Height.Value / 2);
            UltraChart1.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            //UltraChart1.Legend.FormatString.
            UltraChart1.Axis.X.Margin.Near.Value = 2;
            UltraChart1.Axis.Y.Margin.Near.Value = 2;
            //UltraChart1.Axis.X.

            
            
            style.DrawStyle = LineDrawStyle.Dash;
            style.MidPointAnchors = false;
            item.LineStyle = style;
            UltraChart1.LineChart.EmptyStyles.Add(item);
            //UltraChart1.LineChart.NullHandling = NullHandling.InterpolateCustom;

           // UltraChart1.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart1_ChartDrawItem);

            UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;


            

            item.EnableLineStyle = true;
            item.EnablePoint = false;
            style.DrawStyle = LineDrawStyle.Dash;
            style.MidPointAnchors = false;
            item.LineStyle = style;
            UltraChart1.LineChart.EmptyStyles.Add(item);
            //UltraChart1.LineChart.NullHandling = NullHandling.InterpolateCustom;



            UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;


            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            
            item.EnableLineStyle = true;
            item.EnablePoint = false;
            style.DrawStyle = LineDrawStyle.Dash;
            style.MidPointAnchors = false;
            item.LineStyle = style;
 
            //UltraChart3.LineChart.NullHandling = NullHandling.InterpolateCustom;



           


            UltraChart.Data.SwapRowsAndColumns = true;
            UltraChart1.Data.SwapRowsAndColumns = true;
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
        }

        void UltraChart_InterpolateValues(object sender, InterpolateValuesEventArgs e)
        {
          /*  for (int i = 0; i < e.NullValueIndices.Length; i++)
            {
               e.Values.SetValue(-100000, e.NullValueIndices[i]);
            } */
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
                string query = DataProvider.GetQueryText("STAT_0002_0004_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

                
                regionsCombo.Width = 300;
                regionsCombo.Title = "Территория";
                regionsCombo.FillDictionaryValues((CustomMultiComboDataHelper.FillFOSubjectList(DataDictionariesHelper.FOSubjectList, true)));
                regionsCombo.ParentSelect = false;
                //regionsCombo.RemoveTreeNodeByName("Уральский федеральный округ");
                regionsCombo.SetСheckedState("Тюменская область", true);

          
            }

            string firstyear = "2005";
        /*    if (RadioButtonList1.SelectedIndex == 0)
            {
                Faces.Value = "юридическим лицам";

            }
            else
            {
                Faces.Value = "физическим лицам";

            }

            if (RadioButtonList2.SelectedIndex == 0)
            {
                Units.Value = "в рублях";

            }
            else
            {
                Units.Value = "в иностранной валюте";

            } */


            Page.Title = String.Format("Структурная динамика отраслевой структуры занятости и безработицы населения ({0})", regionsCombo.SelectedValue);
           Label1.Text = Page.Title;
           Label2.Text = "<br/> Анализ структурной динамики среднегодовой численности занятого населения и числа безработных в разрезе ОКВЭД в субъектах Российской Федерации, входящих в Уральский федеральный округ (человек)";

           UserParams.PeriodYear.Value = "2008";



           if (regionsCombo.SelectedValue == "Уральский федеральный округ")
           {
               UserParams.Region.Value = string.Format("[{0}]", regionsCombo.SelectedNode);
               /*UltraChart1.Visible = false;
               UltraWebGrid.Visible = false;
               Label3.Visible = false;
               Label4.Visible = false; */
               ChartTable1.Visible = false;
               GridTable1.Visible = false;
           }
           else
           {

               UserParams.Region.Value = string.Format("[Уральский федеральный округ].[{0}]", regionsCombo.SelectedNode);
            /*   UltraChart1.Visible = true;
               UltraWebGrid.Visible = true;
               Label3.Visible = true;
               Label4.Visible = true; */
               ChartTable1.Visible = true;
               GridTable1.Visible = true;
           }
           
           UserParams.StateArea.Value = regionsCombo.SelectedValue;
                   
            
           UltraWebGrid.Bands.Clear();
           UltraWebGrid.DataBind();
           UltraWebGrid1.Bands.Clear();
           UltraWebGrid1.DataBind();
           UltraChart.DataBind();
           UltraChart1.DataBind();
        

           string patternValue = UserParams.StateArea.Value;
           int defaultRowIndex = 1;

           //UserParams.Region.Value = regionsCombo.SelectedValue;
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
            int start = Environment.TickCount;
            
            
            
                string query = DataProvider.GetQueryText("STAT_0002_0004_compare_Grid");
                dtGrid = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);

                query = DataProvider.GetQueryText("Dates");
                DataTable dtGridDate = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtGridDate);
                int j = 0;
               /* foreach (DataColumn column in dtGrid.Columns)
                {
                    column.Caption.Replace("Государственное управление и обеспечение военной безопасности;обязательное социальное обеспечение", ";ололо ");
                   // column.Caption = j.ToString();
                    j++;
                } */
                try
                { dtGrid.Columns[11].Caption = "Государственное управление и обеспечение военной безопасности; &nbsp обязательное социальное обеспечение"; }
                catch { }
                for (int i = 0; i < dtGrid.Rows.Count; i++)
                {
                    DateTime dateTime = CRHelper.DateByPeriodMemberUName(dtGridDate.Rows[i][4].ToString(), 3);
                    dtGrid.Rows[i][0] = string.Format("{0:dd.MM.yy}", dateTime);
                }
                    
                
                UltraWebGrid.DataSource = dtGrid;           
        }

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            int start = Environment.TickCount;



            string query = DataProvider.GetQueryText("STAT_0002_0004_compare_Grid1");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);

           /* query = DataProvider.GetQueryText("Dates");
            DataTable dtGridDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtGridDate);

            dtGrid.Columns[11].Caption = "Государственное управление и обеспечение военной безопасности; &nbsp обязательное социальное обеспечение";
            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                DateTime dateTime = CRHelper.DateByPeriodMemberUName(dtGridDate.Rows[i + 1][4].ToString(), 3);
                dtGrid.Rows[i][0] = string.Format("{0:dd.MM.yy}", dateTime);
            }
            */
            try
            { dtGrid.Columns[13].Caption = "Государственное управление и обеспечение военной безопасности; &nbsp обязательное социальное обеспечение"; }
            catch { }

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

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(80);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
               
          
            for (int k = 1; k < e.Layout.Bands[0].Columns.Count; k++)
            {
                e.Layout.Bands[0].Columns[k].Width = 120;
                e.Layout.Bands[0].Columns[k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[k].CellStyle.Padding.Right = 5;
            }
            
            

            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        }

        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(80);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;


            for (int k = 1; k < e.Layout.Bands[0].Columns.Count; k++)
            {
                e.Layout.Bands[0].Columns[k].Width = 120;
                e.Layout.Bands[0].Columns[k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[k].CellStyle.Padding.Right = 5;
            }



            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            //e.Layout.Bands[0].Columns[0].SortingAlgorithm.;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {

                    double value = Convert.ToDouble(e.Row.Cells[i].Value);
                    e.Row.Cells[i].Value = string.Format("{0:N0}", value);
            
            } 


        }
        protected void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {

                double value = Convert.ToDouble(e.Row.Cells[i].Value);
                e.Row.Cells[i].Value = string.Format("{0:N0}", value);

            }


        }


        #endregion

        #region Обработчики диаграмы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
           
            DataTable dtChart = new DataTable();
           
                string queryName = "Chart_query";
                string query = DataProvider.GetQueryText(queryName);
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

               /* query = DataProvider.GetQueryText("STAT_0002_0004_compare_Grid");
                DataTable dtGridDate = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtGridDate);
                try
                {
                    for (int i = 1; i < dtChart.Columns.Count; i++)
                    {
                        DateTime dateTime = CRHelper.DateByPeriodMemberUName(dtGridDate.Rows[i - 1][4].ToString(), 3);
                        dtChart.Columns[i].ColumnName = string.Format("{0:dd.MM.yy}", dateTime);
                    }


                    dtChart.Rows[0][0] = "Минимальное значение";
                    dtChart.Rows[1][0] = "Максимальное значение";
                }
                catch { }
                */
             
            UltraChart.Data.SwapRowsAndColumns = false;
            UltraChart.DataSource = dtChart;
           
            
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {

            DataTable dtChart = new DataTable();

            string queryName = "Chart1_query";
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            query = DataProvider.GetQueryText("Dates");
            DataTable dtGridDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtGridDate);

            try
            {
               
                    for (int i = 0; i < dtChart.Rows.Count + 1; i++)
                    {
                        DateTime dateTime = CRHelper.DateByPeriodMemberUName(dtGridDate.Rows[i][4].ToString(), 3);
                        dtChart.Rows[i][0] = string.Format("{0:dd.MM.yy}", dateTime);
                    }

               
            }
            catch { }
            UltraChart1.Data.SwapRowsAndColumns = false;
            UltraChart1.DataSource = dtChart;


        }

        
        

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
            e.CurrentWorksheet.Rows[1].Cells[0].Value = "Анализ структурной динамики среднегодовой численности занятого населения и числа безработных в разрезе ОКВЭД в субъектах Российской Федерации, входящих в Уральский федеральный округ.";
            e.Workbook.Worksheets["Занятость"].Rows[0].Cells[0].Value = "Занятость";
            e.Workbook.Worksheets["Безработица"].Rows[0].Cells[0].Value = "Безработица";
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            //if (UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex ].Header.Key.Split(';')[0].Trim() != "в иностранной валюте")
            /*
            switch (e.CurrentWorksheet.Name)
            {

                case "Занятость":
                    {
                        e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0].Trim();
                        break; 
                    }
                case "Безработица":
                    {
                        e.HeaderText = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0].Trim();
                        break; 
                    }
            }
             */

        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {

            int columnCount = UltraWebGrid.Columns.Count;
            int width = 300;
            e.CurrentWorksheet.Rows[7].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.CurrentWorksheet.Columns[0].Width = width * 37;
            e.CurrentWorksheet.Columns[1].Width = width * 37;
            e.CurrentWorksheet.Columns[2].Width = width * 37;
            e.CurrentWorksheet.Columns[3].Width = width * 37;
            e.CurrentWorksheet.Columns[4].Width = width * 37;
            e.CurrentWorksheet.Columns[5].Width = width * 37;
            e.CurrentWorksheet.Columns[6].Width = width * 37;
            e.CurrentWorksheet.Columns[7].Width = width * 37;
            e.CurrentWorksheet.Columns[8].Width = width * 37;
            e.CurrentWorksheet.Columns[9].Width = width * 37;
            e.CurrentWorksheet.Columns[10].Width = width * 37;
            e.CurrentWorksheet.Columns[11].Width = width * 37;
            e.CurrentWorksheet.Columns[12].Width = width * 37;
            e.CurrentWorksheet.Columns[13].Width = width * 37;
            e.CurrentWorksheet.Columns[14].Width = width * 37;


            int columnCountt = UltraWebGrid.Columns.Count;
            for (int i = 1; i < columnCountt; i = i + 1)
            {
                for (int j = 5; j < 20; j++)
                {
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                    //e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.FillPattern = FillPatternStyle.None;
                }
            }

            int columnCounttt = UltraWebGrid.Columns.Count;
            for (int i = 1; i < columnCounttt; i = i + 1)
            {
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00";
                e.CurrentWorksheet.Columns[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
            } 
        }

        

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Безработица");
            Worksheet sheet2 = workbook.Worksheets.Add("Занятость");
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid1, sheet2);
            
        }

        #endregion


        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            
            Report report = new Report();
           ReportSection section1 = new ReportSection(report, false);
           UltraGridExporter1.PdfExporter.Export(UltraWebGrid1, section1);
           IText title = section1.AddText();
           System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
           title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
           title.Style.Font.Bold = true;
           title.AddContent("Занятость");
           ReportSection section2 = new ReportSection(report, false);
           UltraGridExporter1.PdfExporter.Export(UltraWebGrid, section2);
           title = section2.AddText();
           title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
           title.Style.Font.Bold = true;
           title.AddContent("Безработица");
           Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
           section1.AddImage(img);
           img = UltraGridExporter.GetImageFromChart(UltraChart1);
           section2.AddImage(img);

        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
          // string Label1NoHtml = "М3";
            if (IsExported) return;
            IsExported = true;
            IText title = e.Section.AddText();
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
          //  title.AddContent("М4");

            //UltraGridExporter1.PdfExporter.Export(UltraWebGrid1, e.Section);
            //UltraGridExporter1.PdfExporter.Export(UltraWebGrid2, e.Section);

            title = e.Section.AddText();
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            //title.AddContent("М5");
        }
        bool IsExported = false;
        
        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            //e.Section.AddPageBreak();

            Report report = new Report();
            ReportSection section1 = new ReportSection(report, false);
            ReportSection section2 = new ReportSection(report, false);

           /* IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            //title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(ChartCaption.Text);*/

           // Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
           // section1.AddImage(img);

          /*  title = e.Section.AddText();
            font = new Font("Verdana", 14);
            //title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label3.Text); */

         //   img = UltraGridExporter.GetImageFromChart(UltraChart1);
         //   section2.AddImage(img);
          
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
            //this.withFlowColumns = withFlowColumns;
            section = report.AddSection();
            ITable table = section.AddTable();
            ITableRow row = table.AddRow();
            titleCell = row.AddCell();
            /*if (this.withFlowColumns)
            {
                flow = section.AddFlow();
                IFlowColumn col = flow.AddColumn();
                col.Width = new FixedWidth(815);
               // col = flow.AddColumn();
             //   col.Width = new FixedWidth(525);
            //}*/
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
            /*if (flow != null)
                return flow.AddBand();*/
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
            /*if (flow != null)
                return flow.AddTable();*/
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
