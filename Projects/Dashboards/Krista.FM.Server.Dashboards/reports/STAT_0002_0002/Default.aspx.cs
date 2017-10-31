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

namespace Krista.FM.Server.Dashboards.reports.STAT_0002_0002
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
        private DataTable candleDT;
        private DataTable chart1DT;
        private Dictionary<DateTime, string> candleLabelsDictionary;
        private Dictionary<DateTime, string> candleLabelsDictionary1;
        private DateTime currDateTime;
        private DateTime lastDateTime;
        private CustomParam ufo;
        private CustomParam currYear;
        // Текущая дата
        private CustomParam periodCurrentDate;
        // На неделю назад
        private CustomParam periodLastWeekDate;
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
            if (Units == null)
            {
                Units = UserParams.CustomParam("Units");
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
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth);// - 675);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.18);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 10);// - 600);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4);//818);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.ChartType = ChartType.CandleChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.CandleChart.SkipN = 0;
            UltraChart.CandleChart.HighLowVisible = true;
            UltraChart.CandleChart.ResetHighLowVisible();
            UltraChart.CandleChart.VolumeVisible = true; 
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.FormatString = "<SERIES_LABEL>";
            UltraChart.Tooltips.FormatString = "<ITEM_LABEL> ";    
            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P0>";
            UltraChart.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.X.Extent = 10;
            UltraChart.Axis.Y.Extent = 100;
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart.CandleChart.WickColor = Color.ForestGreen;
            UltraChart1.CandleChart.WickColor = Color.ForestGreen;
            UltraChart.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 7; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.Green;
                            break;
                        }
                    case 2:
                        {
                            color = Color.Gold;
                            break;
                        }
                    case 3:
                        {
                            color = Color.Black;
                            break;
                        }
                    case 4:
                        {
                            color = Color.LightSlateGray;
                            break;
                        }
                    case 5:
                        {
                            color = Color.Red;
                            break;
                        }
                    case 6:
                        {
                            color = Color.Blue;
                            break;
                        }
                    case 7:
                        {
                            color = Color.DarkViolet;
                            break;
                        }
                }
                pe.Fill = color;
                UltraChart.ColorModel.Skin.PEs.Add(pe);
            }
            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 10);// - 600);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4);//818);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph1);
            UltraChart1.ChartType = ChartType.CandleChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.CandleChart.SkipN = 0;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.FormatString = "<SERIES_LABEL>";
            UltraChart1.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart1.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P0>";
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL> ";
            UltraChart1.Axis.X.Extent = 10;
            UltraChart1.Axis.Y.Extent = 100;
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 7; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.Green;
                            break;
                        }
                    case 2:
                        {
                            color = Color.Gold;
                            break;
                        }
                    case 3:
                        {
                            color = Color.Black;
                            break;
                        }
                    case 4:
                        {
                            color = Color.LightSlateGray;
                            break;
                        }
                    case 5:
                        {
                            color = Color.Red;
                            break;
                        }
                    case 6:
                        {
                            color = Color.Blue;
                            break;
                        }
                    case 7:
                        {
                            color = Color.DarkViolet;
                            break;
                        }
                }
                pe.Fill = color;
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
            }
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

        protected override void Page_Load(object sender, EventArgs e)
        {

            int start = Environment.TickCount;
            base.Page_Load(sender, e);
            CRHelper.SaveToUserAgentLog(String.Format("Базовый лоад {0}", Environment.TickCount - start));
            start = Environment.TickCount;
            if (!Page.IsPostBack)
            {        
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("STAT_0002_0002_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

                int firstYear = 2009;
                int endYear = 2010;


                ComboPeriod.Width = 300;
                ComboPeriod.MultiSelect = false;
                ComboPeriod.ShowSelectedValue = false;
                ComboPeriod.ParentSelect = false;
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillCreditsNonEmptyDays(DataDictionariesHelper.CreditsNonEmptyDays));
                ComboPeriod.SelectLastNode();
                ComboPeriod.PanelHeaderTitle = "Выберите дату";

            }

            currDateTime = GetDateString(ComboPeriod.GetSelectedNodePath(), ComboPeriod.SelectedNode.Level);
            lastDateTime = currDateTime.AddDays(-7);
            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", currDateTime, 5);
            string firstyear = "2005";
            string faces = string.Empty;
            if (RadioButtonList2.SelectedIndex == 0)
            {
                Faces.Value = "юридическим лицам";
                faces = "для юридических лиц";
            }
            else
            {
                Faces.Value = "физическим лицам";
                faces = "для физических лиц";
            }

            if (RadioButtonList1.SelectedIndex == 0)
            {
                Units.Value = "в рублях";
            }
            else
            {
                Units.Value = "в иностранной валюте";
            }
           periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", currDateTime, 5);
           periodLastWeekDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", lastDateTime, 5);
           Page.Title = "Банковские максимальные/минимальные средневзвешенные ставки по кредитам и депозитам";
           Label1.Text = Page.Title;
           Label2.Text = String.Format("<br/>Анализ динамики банковских средневзвешенных ставок по кредитам и депозитам на {2:dd.MM.yyyy}г ({0}, {1})", faces, Units.Value, currDateTime);
           ChartCaption.Text = String.Format("Колебания показателя «Банковские максимальные/минимальные средневзвешенные ставки по кредитам» на {2:dd.MM.yyyy}г ({0}, {1})", Faces.Value, Units.Value, currDateTime);
           ChartCaption1.Text = String.Format("Колебания показателя «Банковские максимальные/минимальные средневзвешенные ставки по депозитам» на {2:dd.MM.yyyy}г ({0}, {1})", Faces.Value, Units.Value, currDateTime);
           UserParams.PeriodYear.Value = "2008"; 
           UltraWebGrid.Bands.Clear();
           UltraWebGrid.DataBind();
           UltraChart.DataBind();
           UltraChart1.DataBind();
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
                string query = DataProvider.GetQueryText("STAT_0002_0002_compare_Grid");
                dtGrid = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Территория", dtGrid);

                for (int k = 0; k < dtGrid.Rows.Count; k++)
                {
                    DataRow row = dtGrid.Rows[k];

                    for (int i = 1; i < row.ItemArray.Length - 1; i++)
                    {
                        if (row[i] != DBNull.Value)
                        {
                            row[i] = Convert.ToDouble(row[i]) * 100;
                        }
                    }
                }   

                query = DataProvider.GetQueryText("Dates");
                DataTable dtGridDate = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtGridDate);
                CRHelper.SaveToUserAgentLog(String.Format("2 запроса {0}", Environment.TickCount - start));
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
                for (int i = 0; i < dtGrid.Rows.Count; i++)
                {                   
                    dtGrid.Rows[i][oldcount + 1] = string.Format("{0:N2} - {1:N2}", dtGrid.Rows[i][15], dtGrid.Rows[i][14]);
                    dtGrid.Rows[i][oldcount + 2] = string.Format("{0:N2} - {1:N2}", dtGrid.Rows[i][18], dtGrid.Rows[i][17]);
                    dtGrid.Rows[i][oldcount + 3] = string.Format("{0:N2} - {1:N2}", dtGrid.Rows[i][30], dtGrid.Rows[i][29]);
                    dtGrid.Rows[i][oldcount + 4] = string.Format("{0:N2} - {1:N2}", dtGrid.Rows[i][33], dtGrid.Rows[i][32]);
                    dtGrid.Rows[i][oldcount + 5] = string.Format("{0:N2} - {1:N2}", dtGrid.Rows[i][51], dtGrid.Rows[i][50]);
                    dtGrid.Rows[i][oldcount + 6] = string.Format("{0:N2} - {1:N2}", dtGrid.Rows[i][54], dtGrid.Rows[i][53]);
                    dtGrid.Rows[i][oldcount + 7] = string.Format("{0:N2} - {1:N2}", dtGrid.Rows[i][63], dtGrid.Rows[i][62]);
                    dtGrid.Rows[i][oldcount + 8] = string.Format("{0:N2} - {1:N2}", dtGrid.Rows[i][66], dtGrid.Rows[i][65]);                    
                }

                for (int i = 0; i < oldcount; i++)
                {
                    dtGrid.Columns.RemoveAt(1);
                }
                dtGrid.AcceptChanges();
                if (dtGrid.Rows.Count < 4)
                {
                    UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.12);
                }
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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(180);
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
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
                }
            }
            int count = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[0].Hidden = false;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.Bands[0].Columns[1].CellStyle.Padding.Right = 5;
            for (int k = 1; k < 8; k++)
            {               
                e.Layout.Bands[0].Columns[count - k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[count - k].CellStyle.Padding.Right = 5;
            }
            e.Layout.Bands[0].Columns[count - 8].Header.Caption = "Физические лица \n(в рублях)";
            e.Layout.Bands[0].Columns[count - 7].Header.Caption = "Физические лица \n(в иностранной валюте)";
            e.Layout.Bands[0].Columns[count - 6].Header.Caption = "Юридические лица \n(в рублях)";
            e.Layout.Bands[0].Columns[count - 5].Header.Caption = "Юридические лица \n(в иностранной валюте)";
            e.Layout.Bands[0].Columns[count - 4].Header.Caption = "Физические лица \n(в рублях)";
            e.Layout.Bands[0].Columns[count - 3].Header.Caption = "Физические лица \n(в иностранной валюте)";
            e.Layout.Bands[0].Columns[count - 2].Header.Caption = "Юридические лица \n(в рублях)";
            e.Layout.Bands[0].Columns[count - 1].Header.Caption = "Юридические лица \n(в иностранной валюте)";
            ColumnHeader ch = new ColumnHeader(true);
            ch.Caption = "Банковские минимальные/максимальные средневзвешенные процентные ставки в области по кредитам, %";
            ch.Title = "";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = count - 8;
            ch.RowLayoutColumnInfo.SpanX = 4;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);
            ch = new ColumnHeader(true);
            ch.Caption = "Банковские минимальные/максимальные средневзвешенные процентные ставки в области по депозитам, %";
            ch.Title = "";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = count - 4;
            ch.RowLayoutColumnInfo.SpanX = 4;
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
            currYear.Value = String.Format("{0}", currDateTime.Year);
            string query = DataProvider.GetQueryText("chart_сandle");
            DataTable dtChart = new DataTable();
            chart1DT = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Series Name", dtChart);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Series Name", chart1DT);
            if (dtChart.Rows.Count > 0)
            {
                chart1DT.AcceptChanges();
                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = RegionsNamingHelper.ShortName(row[0].ToString());
                    }
                }
                candleLabelsDictionary = new Dictionary<DateTime, string>();
                UltraChart.Series.Clear();
                UltraChart.Series.Add(GetCandleSeries("Name", GetCandleChartDT(dtChart)));
            } 
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            currYear.Value = String.Format("{0}", currDateTime.Year);
            string query = DataProvider.GetQueryText("chart_сandle1");
            DataTable dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Series Name", dtChart);
            if (dtChart.Rows.Count > 0)
            {
                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = RegionsNamingHelper.ShortName(row[0].ToString());
                    }
                }
                candleLabelsDictionary = new Dictionary<DateTime, string>();
                chart1DT.AcceptChanges();
                UltraChart1.Series.Clear();
                UltraChart1.Series.Add(GetCandleSeries("Name", GetCandleChartDT(dtChart)));
            }
        }

        private DataTable GetCandleChartDT(DataTable chartDT)
        {
            candleDT = new DataTable();
            DataColumn nameColumn = new DataColumn("Name", typeof(string));
            candleDT.Columns.Add(nameColumn);
            DataColumn dateColumn = new DataColumn("DateTime", typeof(DateTime));
            candleDT.Columns.Add(dateColumn);
            for (int i = 1; i < chartDT.Columns.Count; i++)
            {
                DataColumn candleColumn = new DataColumn(chartDT.Columns[i].ColumnName, chartDT.Columns[i].DataType);
                candleDT.Columns.Add(candleColumn);
            }
            DateTime time = Convert.ToDateTime("01-01-90");
            for (int i = 0; i < chartDT.Rows.Count; i++)
            {
                DataRow row = chartDT.Rows[i];
                DataRow candleRow = candleDT.NewRow();
                candleRow[1] = time;
                candleRow[0] = row[0];
                candleLabelsDictionary.Add(time, candleRow[0].ToString());
                time = time.AddYears(1);

                for (int j = 1; j < row.ItemArray.Length; j++)
                {
                    candleRow[j + 1] = row[j];
                }
                candleDT.Rows.Add(candleRow);
            }

            return candleDT;
        }

        public static CandleSeries GetCandleSeries(string name, object dataSource)
        {
            DataTable dataTable = (DataTable)dataSource;
            CandleSeries candleSeries = new CandleSeries();
            candleSeries.Label = name;
            candleSeries.Data.LabelColumn = dataTable.Columns[0].ColumnName;
            candleSeries.Data.DateColumn = dataTable.Columns[1].ColumnName;
            candleSeries.Data.OpenColumn = dataTable.Columns[2].ColumnName;
            candleSeries.Data.CloseColumn = dataTable.Columns[3].ColumnName;
            candleSeries.Data.LowColumn = dataTable.Columns[4].ColumnName;
            candleSeries.Data.HighColumn = dataTable.Columns[5].ColumnName;
            candleSeries.Data.VolumeColumn = dataTable.Columns[6].ColumnName;
            candleSeries.Data.DataSource = dataSource;
            candleSeries.DataBind();
            return candleSeries;
        }

        void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive != null && primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    DateTime dateTimeLabel = Convert.ToDateTime(text.GetTextString());
                    if (candleLabelsDictionary != null && candleLabelsDictionary.ContainsKey(dateTimeLabel))
                    {
                        string shortRegionName = candleLabelsDictionary[dateTimeLabel];
                        text.SetTextString(shortRegionName);
                    }
                }

                if (primitive.DataPoint != null)
                {
                    CandleDataPoint dataPoint = (CandleDataPoint)primitive.DataPoint;
                   
                        primitive.DataPoint.Label =
                            String.Format(
                                "{6}\nДиапазон значений с 01.01.{7:yy} по {7:dd.MM.yyyy}:\nминимальное значение на {4:dd.MM.yyyy} - {0:P2}\nмаксимальное значение на {5:dd.MM.yyyy} - {1:P2}\nТекущее значение на {7:dd.MM.yyyy}\nминимальное значение - {2:P2}\nмаксимальное значение - {3:P2}",
                            dataPoint.Low, dataPoint.High, dataPoint.Open, dataPoint.Close, CRHelper.DateByPeriodMemberUName(chart1DT.Rows[primitive.Row][6].ToString(), 3),
                                CRHelper.DateByPeriodMemberUName(chart1DT.Rows[primitive.Row][7].ToString(), 3), candleDT.Rows[primitive.Row][0],
                                currDateTime,
                                currDateTime);
                }
            }
        }


        void UltraChart_FillSceneGraph1(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive != null && primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    DateTime dateTimeLabel = Convert.ToDateTime(text.GetTextString());
                    if (candleLabelsDictionary != null && candleLabelsDictionary.ContainsKey(dateTimeLabel))
                    {
                        string shortRegionName = candleLabelsDictionary[dateTimeLabel];
                        text.SetTextString(shortRegionName);
                    }
                }
                if (primitive.DataPoint != null)
                {
                    CandleDataPoint dataPoint = (CandleDataPoint)primitive.DataPoint;

                    primitive.DataPoint.Label =
                        String.Format(
                            "{6}\nДиапазон значений с 01.01.{7:yy} по {7:dd.MM.yyyy}:\nминимальное значение на {4:dd.MM.yyyy} - {0:P2}\nмаксимальное значение на {5:dd.MM.yyyy} - {1:P2}\nТекущее значение на {7:dd.MM.yyyy}\nминимальное значение - {2:P2}\nмаксимальное значение - {3:P2}",
                            dataPoint.Low,dataPoint.High,dataPoint.Open,dataPoint.Close, CRHelper.DateByPeriodMemberUName(candleDT.Rows[primitive.Row][7].ToString(), 3),
                            CRHelper.DateByPeriodMemberUName(candleDT.Rows[primitive.Row][8].ToString(), 3), candleDT.Rows[primitive.Row][0],
                            currDateTime,
                            currDateTime);
                }
            }
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
            e.CurrentWorksheet.Rows[1].Cells[0].Value = String.Format("Анализ динамики банковских средневзвешенных ставок по кредитам и депозитам {0}, {1}", Faces.Value, Units.Value); ;
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
                }
            }
            int columnCounttt = UltraWebGrid.Columns.Count;
            for (int i = 1; i < columnCounttt; i = i + 1)
            {
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00";
                e.CurrentWorksheet.Columns[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
            } 
        }

        public DateTime GetDateString(string source, int level)
        {
            string[] sts = source.Split('|');
            if (sts.Length > 1)
            {
                switch (level)
                {
                    case 1:
                        {
                            return GetDateString(ComboPeriod.GetNodeLastChild(ComboPeriod.SelectedNode), level + 1);
                        }
                    case 2:
                        {
                            string month = sts[1].TrimEnd(' ');
                            string day = sts[2].TrimEnd(' ');
                            return new DateTime(Convert.ToInt32(sts[0]), CRHelper.MonthNum(month), Convert.ToInt32(day));
                        }
                }
            }
            return DateTime.MinValue;
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
            
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
            title.AddContent(Label2.Text.Replace("<br/>",""));
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(ChartCaption.Text);

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
            e.Section.AddPageBreak();
            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(ChartCaption1.Text);

            img = UltraGridExporter.GetImageFromChart(UltraChart1);
            e.Section.AddImage(img);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;

        }

        #endregion
 
         
        
        
    }
}