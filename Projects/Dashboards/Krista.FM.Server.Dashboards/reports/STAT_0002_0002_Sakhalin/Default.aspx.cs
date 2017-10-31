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
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;

namespace Krista.FM.Server.Dashboards.reports.STAT_0002_0002_Sakhalin
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private CustomParam Faces;
        private CustomParam Units;
        private CustomParam Year;
        private CustomParam month;
        private DataTable candleDT;
        private DataTable chart1DT;
        private Dictionary<DateTime, string> candleLabelsDictionary;
        private DateTime currDateTime;
        //private DateTime lastDateTime;
        private CustomParam currYear;
        // Текущая дата
        private CustomParam periodCurrentDate;
        // На неделю назад
        //private CustomParam periodLastWeekDate;

        private static MemberAttributesDigest dateDigest;

        private GridHeaderLayout headerLayout;
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
            //CRHelper.SaveToUserAgentLog(String.Format("Базовый прелоад {0}", Environment.TickCount - start));
            base.Page_PreLoad(sender, e);
            if (periodCurrentDate == null)
            {
                periodCurrentDate = UserParams.CustomParam("period_current_date");
            }
            //if (periodLastWeekDate == null)
            //{
            //    periodLastWeekDate = UserParams.CustomParam("period_last_week_date");
            //}
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
            UltraWebGrid.Height = Unit.Empty;
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
            UltraChart.Axis.X.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Center;
            UltraChart.Tooltips.FormatString = "<ITEM_LABEL> ";

            //UltraChart.Axis.X.Labels.SeriesLabels.WrapText = false;
            //UltraChart.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            //UltraChart.Axis.X.Labels.SeriesLabels.Layout.BehaviorCollection.Add(new WrapTextAxisLabelLayoutBehavior { Enabled = false });
            //UltraChart.Axis.X.Labels.SeriesLabels.Layout.BehaviorCollection.Add(new ClipTextAxisLabelLayoutBehavior { Trimming = StringTrimming.Word });
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            //UltraChart.Axis.X.Labels.WrapText = true;
            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart.Axis.X.Labels.HorizontalAlign = StringAlignment.Center;
            //UltraChart.Axis.X.Labels.Layout.BehaviorCollection.Add(new ClipTextAxisLabelLayoutBehavior { Trimming = StringTrimming.Word});
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P0>";
            UltraChart.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.X.Extent = 30;
            UltraChart.Axis.Y.Extent = 100;
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart.CandleChart.WickColor = Color.ForestGreen;
            UltraChart1.CandleChart.WickColor = Color.ForestGreen;
            UltraChart.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 9; i++)
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
                    case 8:
                        {
                            color = Color.OrangeRed;
                            break;
                        }
                    case 9:
                        {
                            color = Color.CornflowerBlue;
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
            UltraChart1.Axis.X.Extent = 30;
            UltraChart1.Axis.Y.Extent = 100;
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 9; i++)
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
                    case 8:
                        {
                            color = Color.OrangeRed;
                            break;
                        }
                    case 9:
                        {
                            color = Color.CornflowerBlue;
                            break;
                        }
                }
                pe.Fill = color;
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
            }
            //UltraGridExporter1.Visible = true;
            //UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            //UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            //UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            //UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            //UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            //UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
            //        <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            //UltraGridExporter1.PdfExporter.EndExport += new EventHandler
            //    <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
                        
            //CRHelper.SaveToUserAgentLog(String.Format("Остальной прелоад {0}", Environment.TickCount - start));
            //UltraGridExporter1.MultiHeader = true;
            //UltraGridExporter1.HeaderChildCellHeight = 100;
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        void UltraChart_InterpolateValues(object sender, InterpolateValuesEventArgs e)
        {

        }

        protected override void Page_Load(object sender, EventArgs e)
        {

            int start = Environment.TickCount;
            base.Page_Load(sender, e);
            //CRHelper.SaveToUserAgentLog(String.Format("Базовый лоад {0}", Environment.TickCount - start));
            start = Environment.TickCount;
            if (!Page.IsPostBack)
            {        
                //dtDate = new DataTable();
                //string query = DataProvider.GetQueryText("STAT_0002_0002_Sakhalin_date");
                //DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

                int firstYear = 2009;
                int endYear = 2010;


                ComboPeriod.Width = 300;
                ComboPeriod.MultiSelect = false;
                ComboPeriod.ShowSelectedValue = false;
                ComboPeriod.ParentSelect = false;
                dateDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "STAT_0002_0002_Sakhalin_date");
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(dateDigest.UniqueNames, dateDigest.MemberLevels));
                ComboPeriod.PanelHeaderTitle = "Выберите дату";
                ComboPeriod.SelectLastNode();

            }

            //currDateTime = GetDateString(ComboPeriod.GetSelectedNodePath(), ComboPeriod.SelectedNode.Level);
            //lastDateTime = currDateTime.AddDays(-7);
            //periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", currDateTime, 5);
            periodCurrentDate.Value = dateDigest.GetMemberUniqueName(ComboPeriod.SelectedValue);
            currDateTime = CRHelper.PeriodDayFoDate(periodCurrentDate.Value);
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
           //periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", currDateTime, 5);
           //periodLastWeekDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", lastDateTime, 5);
           //CRHelper.DateByPeriodMemberUName(periodCurrentDate.Value, 5);
           Page.Title = "Банковские максимальные/минимальные средневзвешенные ставки по кредитам и депозитам";
           Label1.Text = Page.Title;
           Label2.Text = String.Format("Анализ динамики банковских средневзвешенных ставок по кредитам и депозитам на {2:dd.MM.yyyy}г ({0}, {1})", faces, Units.Value, CRHelper.PeriodDayFoDate(periodCurrentDate.Value));
           ChartCaption.Text = String.Format("Колебания показателя «Банковские максимальные/минимальные средневзвешенные ставки по кредитам» на {2:dd.MM.yyyy}г ({0}, {1})", Faces.Value, Units.Value, CRHelper.PeriodDayFoDate(periodCurrentDate.Value));
           ChartCaption1.Text = String.Format("Колебания показателя «Банковские максимальные/минимальные средневзвешенные ставки по депозитам» на {2:dd.MM.yyyy}г ({0}, {1})", Faces.Value, Units.Value, CRHelper.PeriodDayFoDate(periodCurrentDate.Value));
           UserParams.PeriodYear.Value = "2008";
           headerLayout = new GridHeaderLayout(UltraWebGrid);
           UltraWebGrid.Bands.Clear();
           UltraWebGrid.DataBind();
           UltraChart.DataBind();
           UltraChart1.DataBind();
           string patternValue = UserParams.StateArea.Value;
           //int defaultRowIndex = 1;
           //UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
           //ActiveGridRow(row);
           //CRHelper.SaveToUserAgentLog(String.Format("Остльной лоад {0}", Environment.TickCount - start));
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
                string query = DataProvider.GetQueryText("STAT_0002_0002_Sakhalin_compare_Grid");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Территория", dtGrid);

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
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtGridDate);
                //CRHelper.SaveToUserAgentLog(String.Format("2 запроса {0}", Environment.TickCount - start));
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
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
       
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

            headerLayout.AddCell("Период");
            GridHeaderCell groupCell = headerLayout.AddCell("Банковские минимальные/максимальные средневзвешенные процентные ставки в области по кредитам, %");
            groupCell.AddCell(e.Layout.Bands[0].Columns[count - 8].Header.Caption);
            groupCell.AddCell(e.Layout.Bands[0].Columns[count - 7].Header.Caption);
            groupCell.AddCell(e.Layout.Bands[0].Columns[count - 6].Header.Caption);
            groupCell.AddCell(e.Layout.Bands[0].Columns[count - 5].Header.Caption);
            GridHeaderCell groupCell2 = headerLayout.AddCell("Банковские минимальные/максимальные средневзвешенные процентные ставки в области по депозитам, %");
            groupCell2.AddCell(e.Layout.Bands[0].Columns[count - 4].Header.Caption);
            groupCell2.AddCell(e.Layout.Bands[0].Columns[count - 3].Header.Caption);
            groupCell2.AddCell(e.Layout.Bands[0].Columns[count - 2].Header.Caption);
            groupCell2.AddCell(e.Layout.Bands[0].Columns[count - 1].Header.Caption);

            headerLayout.ApplyHeaderInfo();
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
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Series Name", dtChart);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Series Name", chart1DT);
            if (dtChart.Rows.Count > 0)
            {
                chart1DT.AcceptChanges();
                //foreach (DataRow row in dtChart.Rows)
                //{
                //    //if (row[0] != DBNull.Value)
                //    //{
                //    //    row[0] = RegionsNamingHelper.ShortName(row[0].ToString());
                //    //}

                //}
                dtChart.Columns.RemoveAt(0);
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
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Series Name", dtChart);
            if (dtChart.Rows.Count > 0)
            {
                dtChart.Columns.RemoveAt(0);
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
    
                        if (shortRegionName.Contains("автономная"))
                        {
                            int index = shortRegionName.IndexOf("автономная");
                            shortRegionName = shortRegionName.Insert(index, "\n");
                        }
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
                        if (shortRegionName.Contains("автономная"))
                        {
                            int index = shortRegionName.IndexOf("автономная");
                            shortRegionName = shortRegionName.Insert(index, "\n");
                        }
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

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
            Worksheet sheet3 = workbook.Worksheets.Add("sheet3");
         
            ReportExcelExporter1.HeaderCellHeight = 50;
            ReportExcelExporter1.SheetColumnCount = 20;

            ReportExcelExporter1.GridColumnWidthScale = 0.8;

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.9));
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.9));

            ReportExcelExporter1.Export(UltraChart, ChartCaption.Text, sheet2, 3);
            ReportExcelExporter1.Export(UltraChart1, ChartCaption1.Text, sheet3, 3);
         
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.86));
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.86));

            ReportPDFExporter1.HeaderCellHeight = 70;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart, ChartCaption.Text, section2);
            ReportPDFExporter1.Export(UltraChart1, ChartCaption1.Text, section3);
        }

        #endregion
 
         
        
        
    }
}