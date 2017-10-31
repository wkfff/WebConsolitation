using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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

namespace Krista.FM.Server.Dashboards.reports.STAT_0002_0001
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtComments;
        private CustomParam Faces;
        private CustomParam Units;

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
            start = Environment.TickCount;
            if (Faces == null)
            {
                Faces = UserParams.CustomParam("Faces");
            }
            if (Units == null)
            {
                Units = UserParams.CustomParam("Units");
            }
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 10);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.30);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
           
            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4);
            UltraChart.ChartType = ChartType.LineChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL> <DATA_VALUE:P2>";
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.Axis.X.Extent = 70;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P2>";
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 11;
            UltraChart.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Border.Thickness = 0;
            UltraChart.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.X.Margin.Near.Value = 2;
            UltraChart.Axis.Y.Margin.Near.Value = 2;
            
            EmptyAppearance item = new EmptyAppearance();
            item.EnableLineStyle = true;
            item.EnablePoint = false;
            LineStyle style = new LineStyle();
            style.DrawStyle = LineDrawStyle.Dash;
            style.MidPointAnchors = false;
            item.LineStyle = style;
            
            UltraChart.LineChart.EmptyStyles.Add(item);
            UltraChart.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart_ChartDrawItem);
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
                                    
            UltraChart1.LineChart.EmptyStyles.Add(item);
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4);            
            UltraChart1.ChartType = ChartType.LineChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL> <DATA_VALUE:P2>";
            UltraChart1.Axis.X.Extent = 70;
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P2>";
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 11;
            UltraChart1.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.Y.Margin.Near.Value = 2;
            UltraChart1.LineChart.EmptyStyles.Add(item);
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomLinear;
         
            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4);
            UltraChart2.ChartType = ChartType.LineChart;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL> <DATA_VALUE:P2>";
            UltraChart2.Axis.X.Extent = 70;
            UltraChart2.Axis.X.Labels.Visible = true;
            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P2>";          
            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Bottom;
            UltraChart2.Legend.SpanPercentage = 11;
            UltraChart2.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Border.Thickness = 0;
            UltraChart2.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Axis.X.Margin.Near.Value = 2;
            UltraChart2.Axis.Y.Margin.Near.Value = 2;
            UltraChart2.LineChart.EmptyStyles.Add(item);
            UltraChart2.ColorModel.ModelStyle = ColorModels.CustomLinear;
            
            UltraChart3.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);
            UltraChart3.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4);
            UltraChart3.ChartType = ChartType.LineChart;
            UltraChart3.Border.Thickness = 0;
            UltraChart3.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL> <DATA_VALUE:P2>";
            UltraChart3.Axis.X.Extent = 70;
            UltraChart3.Axis.X.Labels.Visible = true;
            UltraChart3.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P2>";
            UltraChart3.Legend.Visible = true;
            UltraChart3.Legend.Location = LegendLocation.Bottom;
            UltraChart3.Legend.SpanPercentage = 11;
            UltraChart3.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart3.Border.Thickness = 0;
            UltraChart3.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart3.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart3.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart3.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart3.Axis.X.Margin.Near.Value = 2;
            UltraChart3.Axis.Y.Margin.Near.Value = 2;

            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart3.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart3.LineChart.EmptyStyles.Add(item);
            UltraChart3.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart.Data.SwapRowsAndColumns = true;
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
                string query = DataProvider.GetQueryText("STAT_0002_0001_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            }
            Units.Value = "в рублях";
            Faces.Value = "юридическим лицам";
            string firstyear = "2005";
            if (RadioButtonList2.SelectedIndex == 1)
            {
                Faces.Value = "физическим лицам";

            }
            else if (RadioButtonList2.SelectedIndex == 2)
            {
                Faces.Value = "юридическим лицам";

            }

            if (RadioButtonList1.SelectedIndex == 1)
            {
                Units.Value = "в иностранной валюте";

            }
            else if (RadioButtonList1.SelectedIndex == 2)
            {
                Units.Value = "в рублях";

            }

           Page.Title = "Процентные ставки";
           Label1.Text = Page.Title;
           Label2.Text = String.Format("Анализ динамики процентных ставок по кредитам и депозитам, Ханты-Мансийский автономный округ – Югра");
           ChartCaption.Text = String.Format("Динамика банковских минимальных/максимальных средневзвешенных ставок по кредитам ({0}, {1})", Faces.Value, Units.Value);
           ChartCaption1.Text = String.Format("Динамика банковских минимальных/максимальных средневзвешенных ставок по депозитам ({0}, {1})", Faces.Value, Units.Value);
           ChartCaption2.Text = String.Format("Динамика процентных ставок по выданным ипотечным кредитам ({0})", Units.Value);
           ChartCaption3.Text = String.Format("Динамика процентных ставок по выданным потребительским кредитам ({0})", Units.Value);
           UserParams.PeriodYear.Value = "2008";
           UltraWebGrid.Bands.Clear();
           UltraWebGrid.DataBind();
           UltraChart.DataBind();
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
                int start = Environment.TickCount;
                string query = DataProvider.GetQueryText("STAT_0002_0001_compare_Grid");
                dtGrid = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);

                query = DataProvider.GetQueryText("Dates1");
                DataTable dtGridDate = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtGridDate);

                CRHelper.SaveToUserAgentLog(String.Format("2 запроса {0}", Environment.TickCount - start));

                CRHelper.SaveToErrorLog(dtGrid.Rows.Count.ToString());
              

                for (int k = 0; k < dtGrid.Rows.Count; k++)
                {
                    DataRow row = dtGrid.Rows[k];

                    for (int i = 1; i < row.ItemArray.Length - 1; i++)
                    {
                        if (row[i] != DBNull.Value)
                        {
                            if ((i == 73) || (i == 94)) row[i] = Convert.ToDouble(row[i]) / 1000000;
                            else 
                            {
                                row[i] = Convert.ToDouble(row[i]) * 100;
                            }
                        }
                    }
                }        
               
                int oldcount = dtGrid.Columns.Count - 1;

                DataColumn column = new DataColumn("Банковские минимальные/максимальные средневзвешенные процентные ставки в области по кредитам, %;1", typeof(string));
                dtGrid.Columns.Add(column);
                column = new DataColumn("Банковские минимальные/максимальные средневзвешенные процентные ставки в области по кредитам, %;2", typeof(string));
                dtGrid.Columns.Add(column);
                column = new DataColumn("Банковские минимальные/максимальные средневзвешенные процентные ставки в области по кредитам, %;3", typeof(string));
                dtGrid.Columns.Add(column);
                column = new DataColumn("Банковские минимальные/максимальные средневзвешенные процентные ставки в области по кредитам, %;4", typeof(string));
                dtGrid.Columns.Add(column);
                column = new DataColumn("Банковские минимальные/максимальные средневзвешенные процентные ставки в области по депозитам, %;1", typeof(string));
                dtGrid.Columns.Add(column);
                column = new DataColumn("Банковские минимальные/максимальные средневзвешенные процентные ставки в области по депозитам, %;2", typeof(string));
                dtGrid.Columns.Add(column);
                column = new DataColumn("Банковские минимальные/максимальные средневзвешенные процентные ставки в области по депозитам, %;3", typeof(string));
                dtGrid.Columns.Add(column);
                column = new DataColumn("Банковские минимальные/максимальные средневзвешенные процентные ставки в области по депозитам, %;4", typeof(string));
                dtGrid.Columns.Add(column);
                column = new DataColumn("Потребительские кредиты:;1", typeof(string));
                dtGrid.Columns.Add(column);
                column = new DataColumn("Потребительские кредиты:;2", typeof(string));
                dtGrid.Columns.Add(column);
                column = new DataColumn("Потребительские кредиты:;3", typeof(string));
                dtGrid.Columns.Add(column);
                column = new DataColumn("Ипотечные кредиты:;1", typeof(string));
                dtGrid.Columns.Add(column);
                column = new DataColumn("Ипотечные кредиты:;2", typeof(string));
                dtGrid.Columns.Add(column);
                column = new DataColumn("Ипотечные кредиты:;3", typeof(string));
                dtGrid.Columns.Add(column);

                for (int i = 0; i < dtGrid.Rows.Count; i++)
                {
                    try
                    {
                        DateTime dateTime = CRHelper.DateByPeriodMemberUName(dtGridDate.Rows[i][4].ToString(), 3);
                        dtGrid.Rows[i][0] = string.Format("{0:dd.MM.yy}", dateTime);
                    }
                    catch { }
                    
                    dtGrid.Rows[i][oldcount + 1] = string.Format("{0:N2} - {1:N2}", dtGrid.Rows[i][15], dtGrid.Rows[i][14]);//Физ. лица в рублях
                    dtGrid.Rows[i][oldcount + 2] = string.Format("{0:N2} - {1:N2}", dtGrid.Rows[i][18], dtGrid.Rows[i][17]);//Физ. лица в иностр. валюте
                    dtGrid.Rows[i][oldcount + 3] = string.Format("{0:N2} - {1:N2}", dtGrid.Rows[i][30], dtGrid.Rows[i][29]);//Юр. лица в рублях
                    dtGrid.Rows[i][oldcount + 4] = string.Format("{0:N2} - {1:N2}", dtGrid.Rows[i][33], dtGrid.Rows[i][32]);//Юр. лица в иностр. валюте
                    dtGrid.Rows[i][oldcount + 5] = string.Format("{0:N2} - {1:N2}", dtGrid.Rows[i][51], dtGrid.Rows[i][50]);//Физ. лица в рублях
                    dtGrid.Rows[i][oldcount + 6] = string.Format("{0:N2} - {1:N2}", dtGrid.Rows[i][54], dtGrid.Rows[i][53]);//Физ. лица в иностр. валюте
                    dtGrid.Rows[i][oldcount + 7] = string.Format("{0:N2} - {1:N2}", dtGrid.Rows[i][63], dtGrid.Rows[i][62]);//Юр. лица в рублях
                    dtGrid.Rows[i][oldcount + 8] = string.Format("{0:N2} - {1:N2}", dtGrid.Rows[i][66], dtGrid.Rows[i][65]);//Юр. лица в иностр. валюте
                    dtGrid.Rows[i][oldcount + 9] = string.Format("{0:N2}",dtGrid.Rows[i][73]);//Объёмы потр. кред.
                    dtGrid.Rows[i][oldcount + 10] = string.Format("{0:N2} - {1:N2}", dtGrid.Rows[i][84], dtGrid.Rows[i][83]);
                    dtGrid.Rows[i][oldcount + 11] = string.Format("{0:N2} - {1:N2}", dtGrid.Rows[i][87], dtGrid.Rows[i][86]);
                    dtGrid.Rows[i][oldcount + 12] = string.Format("{0:N2}", dtGrid.Rows[i][94]);//Объёмы ипот. кред.
                    dtGrid.Rows[i][oldcount + 13] = string.Format("{0:N2} - {1:N2}", dtGrid.Rows[i][105], dtGrid.Rows[i][104]);
                    dtGrid.Rows[i][oldcount + 14] = string.Format("{0:N2} - {1:N2}", dtGrid.Rows[i][108], dtGrid.Rows[i][107]);
                 
                }

                for (int i = 0; i < oldcount; i++)
                {
                    dtGrid.Columns.RemoveAt(1);
                }
                dtGrid.AcceptChanges();
                UltraWebGrid.DataSource = dtGrid;            
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(60);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

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
            int count = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[0].Hidden = false;

            for (int k = 1; k < 15; k++)
            {               
                e.Layout.Bands[0].Columns[count - k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[count - k].CellStyle.Padding.Right = 5;
            }

            e.Layout.Bands[0].Columns[count - 14].Header.Caption = "Физические лица (в рублях)";
            e.Layout.Bands[0].Columns[count - 13].Header.Caption = "Физические лица (в иностранной валюте)";
            e.Layout.Bands[0].Columns[count - 12].Header.Caption = "Юридические лица (в рублях)";
            e.Layout.Bands[0].Columns[count - 11].Header.Caption = "Юридические лица (в иностранной валюте)";
            e.Layout.Bands[0].Columns[count - 10].Header.Caption = "Физические лица (в рублях)";
            e.Layout.Bands[0].Columns[count - 9].Header.Caption = "Физические лица (в иностранной валюте)";
            e.Layout.Bands[0].Columns[count - 8].Header.Caption = "Юридические лица (в рублях)";
            e.Layout.Bands[0].Columns[count - 7].Header.Caption = "Юридические лица (в иностранной валюте)";
            e.Layout.Bands[0].Columns[count - 6].Header.Caption = "Объёмы (млн.руб)";
            e.Layout.Bands[0].Columns[count - 5].Header.Caption = "Процентная ставка по выданным кредитам (средневзвеш.), % (в рублях)";
            e.Layout.Bands[0].Columns[count - 4].Header.Caption = "Процентная ставка по выданным кредитам (средневзвеш.), % (в иностранной валюте)";
            e.Layout.Bands[0].Columns[count - 3].Header.Caption = "Объёмы (млн.руб)";
            e.Layout.Bands[0].Columns[count - 2].Header.Caption = "Процентная ставка по выданным кредитам (средневзвеш.), % (в рублях)";
            e.Layout.Bands[0].Columns[count - 1].Header.Caption = "Процентная ставка по выданным кредитам (средневзвеш.), % (в иностранной валюте)";

            ColumnHeader ch = new ColumnHeader(true);
            ch.Caption = "Банковские минимальные/максимальные средневзвешенные процентные ставки в области по кредитам, %";
            ch.Title = "";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = count - 14;
            ch.RowLayoutColumnInfo.SpanX = 4;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = "Банковские минимальные/максимальные средневзвешенные процентные ставки в области по депозитам, %";
            ch.Title = "";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = count - 10;
            ch.RowLayoutColumnInfo.SpanX = 4;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = "Потребительские кредиты:";
            ch.Title = "";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = count - 6;
            ch.RowLayoutColumnInfo.SpanX = 3;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = "Ипотечные кредиты:";
            ch.Title = "";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = count - 3;
            ch.RowLayoutColumnInfo.SpanX = 3;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);
            Collection<string> cellsCaption = new Collection<string>();
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        }
  
        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
     
        }

        #endregion

        #region Обработчики диаграмы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
           
            DataTable dtChart = new DataTable();
           
                string queryName = "Chart_query";
                string query = DataProvider.GetQueryText(queryName);
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

                query = DataProvider.GetQueryText("Dates2");
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
                
             
            UltraChart.Data.SwapRowsAndColumns = false;
            UltraChart.DataSource = dtChart;

        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {

            DataTable dtChart = new DataTable();

            string queryName = "Chart1_query";
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            query = DataProvider.GetQueryText("Dates3");
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

                if (dtChart.Rows.Count > 1)
                {

                }
            }
            catch { }
            UltraChart1.Data.SwapRowsAndColumns = false;
            UltraChart1.DataSource = dtChart;

        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {

            DataTable dtChart = new DataTable();

            string queryName = "Chart2_query";
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            query = DataProvider.GetQueryText("Dates4");
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



                if (dtChart.Rows.Count > 1)
                {

                }
            }
            catch { }
            UltraChart2.Data.SwapRowsAndColumns = false;
            UltraChart2.DataSource = dtChart;

        }

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {

            DataTable dtChart = new DataTable();

            string queryName = "Chart3_query";
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            query = DataProvider.GetQueryText("Dates5");
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
            UltraChart3.Data.SwapRowsAndColumns = false;
            UltraChart3.DataSource = dtChart;


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
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Page.Title;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = Label2.Text;
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            if (UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex ].Header.Key.Split(';')[0].Trim() != "в иностранной валюте")
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex ].Header.Key.Split(';')[0].Trim();
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {

            int columnCount = UltraWebGrid.Columns.Count;
            int width = 300;
            e.CurrentWorksheet.Rows[7].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.CurrentWorksheet.Columns[0].Width = width * 10;
            e.CurrentWorksheet.Columns[1].Width = width * 10;
            e.CurrentWorksheet.Columns[2].Width = width * 10;
            e.CurrentWorksheet.Columns[3].Width = width * 10;
            e.CurrentWorksheet.Columns[4].Width = width * 10;
            e.CurrentWorksheet.Columns[5].Width = width * 10;
            e.CurrentWorksheet.Columns[6].Width = width * 10;
            e.CurrentWorksheet.Columns[7].Width = width * 10;
            e.CurrentWorksheet.Columns[8].Width = width * 10;
            e.CurrentWorksheet.Columns[9].Width = width * 10;
            e.CurrentWorksheet.Columns[10].Width = width * 10;
            e.CurrentWorksheet.Columns[11].Width = width * 10;
            e.CurrentWorksheet.Columns[12].Width = width * 10;
            e.CurrentWorksheet.Columns[13].Width = width * 10;
            e.CurrentWorksheet.Columns[14].Width = width * 10;


            int columnCountt = UltraWebGrid.Columns.Count;
            for (int i = 1; i < columnCountt; i = i + 1)
            {
                for (int j = 1; j < 5; j++)
                {
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                    e.CurrentWorksheet.Rows[j].Height = 650;
                }
                for (int j = 5; j < 20; j++)
                {
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
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
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма1");
            Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма2");
            Worksheet sheet4 = workbook.Worksheets.Add("Диаграмма3");
            Worksheet sheet5 = workbook.Worksheets.Add("Диаграмма4");
            sheet2.Rows[0].Cells[0].Value = ChartCaption.Text;
            UltraGridExporter.ChartExcelExport(sheet2.Rows[1].Cells[0], UltraChart);
            sheet3.Rows[0].Cells[0].Value = ChartCaption1.Text;
            UltraGridExporter.ChartExcelExport(sheet3.Rows[1].Cells[0], UltraChart1);
            sheet4.Rows[0].Cells[0].Value = ChartCaption2.Text;
            UltraGridExporter.ChartExcelExport(sheet4.Rows[1].Cells[0], UltraChart2);
            sheet5.Rows[0].Cells[0].Value = ChartCaption3.Text;
            UltraGridExporter.ChartExcelExport(sheet5.Rows[1].Cells[0], UltraChart3);
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.MultiHeader = true;

            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Page.Title);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(ChartCaption.Text);

            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(ChartCaption1.Text);

            img = UltraGridExporter.GetImageFromChart(UltraChart1);
            e.Section.AddImage(img);

            e.Section.AddPageBreak();

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(ChartCaption2.Text);

            img = UltraGridExporter.GetImageFromChart(UltraChart2);
            e.Section.AddImage(img);
 

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(ChartCaption3.Text);

            img = UltraGridExporter.GetImageFromChart(UltraChart3);
            e.Section.AddImage(img);
        }

        #endregion
 
         
        
        
    }
}