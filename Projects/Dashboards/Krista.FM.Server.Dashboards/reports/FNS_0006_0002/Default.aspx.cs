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
using Infragistics.Documents.Reports.Graphics;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using IList = Infragistics.Documents.Reports.Report.List.IList;
using Image = Infragistics.Documents.Reports.Graphics.Image;

namespace Krista.FM.Server.Dashboards.reports.FNS_0006_0002
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
        private CustomParam mul;
        private CustomParam dec;
        private CustomParam regions;
        private CustomParam settlements;
        private CustomParam regionsLevel;
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
            regionsLevel = UserParams.CustomParam("regions_level");
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
            if (mul == null)
            {
                mul = UserParams.CustomParam("mul");
            }
            if (dec == null)
            {
                dec = UserParams.CustomParam("dec");
            }
            if (regions == null)
            {
                regions = UserParams.CustomParam("regions");
            }
            if (settlements == null)
            {
                settlements = UserParams.CustomParam("settlements");
            }
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth -20);
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
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.70);
            UltraChart1.ChartType = ChartType.ColumnChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n<ITEM_LABEL> за {0} год\n<DATA_VALUE:N1> {1}", ComboYear.SelectedValue, units);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.Axis.X.Extent = 140;
            UltraChart1.TitleLeft.HorizontalAlign = System.Drawing.StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = UltraChart1.Axis.X.Extent;
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";
            UltraChart1.Axis.X.Labels.ItemFormatString = " ";
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 12;
            UltraChart1.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 13);
            UltraChart1.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart1.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.Data.SwapRowsAndColumns = false;
            UltraChart1.LineChart.NullHandling = NullHandling.DontPlot;
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
        }

        void UltraChart_InterpolateValues(object sender, InterpolateValuesEventArgs e)
        {
        
        }



        string units = string.Empty;
        protected override void Page_Load(object sender, EventArgs e)
        {

            int start = Environment.TickCount;
            base.Page_Load(sender, e);
            CRHelper.SaveToUserAgentLog(String.Format("Базовый лоад {0}", Environment.TickCount - start));

            start = Environment.TickCount;
            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string queryName = "FNS_0006_0002_date";
                string query = DataProvider.GetQueryText(queryName);
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                int endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                int firstYear = 2006;
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
            }
            
           
                if (RList.SelectedIndex == 0)
                {
                    mul.Value = Convert.ToString(1000);
                    units = "тыс.руб.";
                }
                else
                {
                    mul.Value = Convert.ToString(1000000);
                    units = "млн.руб.";
                }
                
           
           string firstyear = "2005";
           Year.Value = ComboYear.SelectedValue;
           PrevYear.Value = Convert.ToString( Convert.ToInt32(ComboYear.SelectedValue) - 1);
           PprevYear.Value = Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 2);
           Page.Title = "Анализ начисленных сумм по земельному налогу в разрезе категорий налогоплательщиков";
           regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
           Label1.Text = Page.Title;
           UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;
           UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
           UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
           ChartCaption1.Text = String.Format("Начисления по категориям налогоплательщиков в разрезе территорий, {0}",units);
           UltraChart1.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N1> {1}", ComboYear.SelectedValue, units);
           UserParams.PeriodYear.Value = "2008";
           regions.Value = RegionSettingsHelper.Instance.RegionsLevel;
           settlements.Value = RegionSettingsHelper.Instance.SettlementLevel;
           Label2.Text = String.Format("за {0} год", ComboYear.SelectedValue, (UltraWebGrid.Columns.Count - 3));
           string patternValue = UserParams.StateArea.Value;
           int defaultRowIndex = 1;
           UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
           if (GO.Checked)
           {
               dec.Value = String.Empty;
               UltraChart1.DataBind();
           }
           else
           {
               dec.Value = ",Descendants([Районы].[Сопоставимый].[Все районы].[г.Омск],[Районы].[Сопоставимый].[Все районы].[г.Омск].Level,SELF_AND_AFTER )";
               UltraChart1.DataBind();
           }
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

        }
        int minf = 0;
        int minu = 0;
        bool [] firstrang = new bool [500];
        bool [] secondrang = new bool [500];
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {

            string query = DataProvider.GetQueryText("FNS_0006_0002_compare_Grid");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtGrid);

             if (dtGrid.Rows.Count > 0)
                {
                    minf = Convert.ToInt32(dtGrid.Rows[3][4]);
                    minu = Convert.ToInt32(dtGrid.Rows[3][8]);
                    for (int k = 3; k < dtGrid.Rows.Count; k++)
                    {
                            if (dtGrid.Rows[k][4] != DBNull.Value)
                        {
                            if (Convert.ToInt32(dtGrid.Rows[k][4]) > minf)
                            minf = Convert.ToInt32(dtGrid.Rows[k][4]);
                        }
                            if (dtGrid.Rows[k][8] != DBNull.Value)
                        {
                            if (Convert.ToInt32(dtGrid.Rows[k][8]) > minu)
                            minu = Convert.ToInt32(dtGrid.Rows[k][8]);
                        }
                    }
                 int minr = 100;
                 int startrow = 2;
                 for (int colnumb = 5; colnumb < dtGrid.Columns.Count; colnumb+=4)
                 {
                                      
                    for (int k = startrow; k < dtGrid.Rows.Count; k++)
                    {
                        
                        if (dtGrid.Rows[k][0].ToString().Contains("МР"))
                        {
                            int rowscount = 0;
                            bool rangfounded = false;
                            int oneCount = 0;
                            int countinmo = 0;
                            int minrang = 1;
                            bool complete = false;
                            int rang = 0;
                            int maxrang = 0;
                            for (rang = minrang; !complete; rang++)
                            {
                                int min = 0;
                                rangfounded = false;
                                if (dtGrid.Rows[k + 1][colnumb] != DBNull.Value)
                                {
                                    min = Convert.ToInt32(dtGrid.Rows[k + 1][colnumb]);
                                }
                                for (int m = k + 1; ((rangfounded == false) && (m < dtGrid.Rows.Count)); m++)
                                {
                                    if (dtGrid.Rows[m][0].ToString().Contains("МР"))
                                        break;
                                    if (dtGrid.Rows[m][colnumb] != DBNull.Value)
                                    {
                                        if (Convert.ToInt32(dtGrid.Rows[m][colnumb]) == rang)
                                        {
                                            rangfounded = true;
                                        }
                                        if ((Convert.ToInt32(dtGrid.Rows[m][colnumb]) > min))
                                        {
                                            min = Convert.ToInt32(dtGrid.Rows[m][colnumb]);
                                        }
                                    }
                                }
                                if (rangfounded == false)
                                {

                                    for (int n = k + 1; (((n < dtGrid.Rows.Count))); n++)
                                    {
                                        if (dtGrid.Rows[n][0].ToString().Contains("МР"))
                                            break;
                                        if (dtGrid.Rows[n][colnumb] != DBNull.Value)
                                        {
                                            if ((Convert.ToInt32(dtGrid.Rows[n][colnumb]) < min) && (Convert.ToInt32(dtGrid.Rows[n][colnumb]) > rang))
                                            {
                                                min = Convert.ToInt32(dtGrid.Rows[n][colnumb]);
                                            }
                                        }

                                    }
                                    for (int v = k + 1; (((v < dtGrid.Rows.Count))); v++)
                                    {
                                        if (dtGrid.Rows[v][0].ToString().Contains("МР"))
                                            break;
                                        if (dtGrid.Rows[v][colnumb] != DBNull.Value)
                                        {
                                            if (Convert.ToInt32(dtGrid.Rows[v][colnumb]) == min)
                                            {
                                                dtGrid.Rows[v][colnumb] = rang;
                                            }
                                        }

                                    }
                                }

                                {
                                    complete = true;
                                    for (int q = k + 1; (((q < dtGrid.Rows.Count))) && complete; q++)
                                    {
                                        if (dtGrid.Rows[q][0].ToString().Contains("МР"))
                                            break;
                                        if ((dtGrid.Rows[q][colnumb] != DBNull.Value) && (Convert.ToInt32(dtGrid.Rows[q][colnumb]) != 1))
                                        {
                                            complete = false;
                                            {
                                                for (int j = k + 1; ((j < dtGrid.Rows.Count) && !complete); j++)
                                                {
                                                    if (dtGrid.Rows[q][0].ToString().Contains("МР"))
                                                        break;
                                                    if (dtGrid.Rows[j][colnumb] != DBNull.Value)
                                                    {
                                                        complete = (Convert.ToInt32(dtGrid.Rows[j][colnumb]) == Convert.ToInt32(dtGrid.Rows[q][colnumb]) - 1);
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                            
                            maxrang = k + 1;
                            for (int q = k + 1; ((q < dtGrid.Rows.Count)); q++)
                            {
                                if (dtGrid.Rows[q][0].ToString().Contains("МР"))
                                    break;
                                if ((dtGrid.Rows[q][colnumb] != DBNull.Value))
                                {
                                    if (dtGrid.Rows[maxrang][colnumb] == DBNull.Value)
                                    {
                                        maxrang = q;
                                    }
                                    if ((Convert.ToInt32(dtGrid.Rows[q][colnumb])) > (Convert.ToInt32(dtGrid.Rows[maxrang][colnumb])))
                                    {
                                        maxrang = q;
                                    }
                                }
                                
                            }
                            switch (colnumb)
                                    {
                                        case 5:                //первая колонка рангов
                                            {
                                                firstrang[maxrang] = true;
                                                break;
                                            }
                                        case 9:                //вторая колонка рангов
                                            {
                                                secondrang[maxrang] = true;
                                                break;
                                            }
                                    }
                                }
                            }
                        } 
                    
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

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        double percent = 0;
                        string columnname = string.Empty;
                        if (box.DataPoint.Label == "Начисления по юридическим лицам")
                        {
                            columnname = "Доля начислений по юридическим лицам в общей сумме начислений, % ";
                        }
                        if (box.DataPoint.Label == "Начисления по физическим лицам")
                        {
                            columnname = "Доля начислений по физическим лицам в общей сумме начислений, % ";
                        }

                            for (int k = 0; k < Chart.Columns.Count; k++)
                            {
                                CRHelper.SaveToErrorLog("Колонки:" + Chart.Columns[k].Caption);
                                if (Chart.Columns[k].Caption == columnname)
                                {
                                    percent = Convert.ToDouble(Chart.Rows[box.Row][k]);
                                }
                            }
                        if (percent > 0)
                        {
                            box.DataPoint.Label =
                                String.Format("Доля в общей сумме начислений {0:N2}%<br />{1} за {2} год", percent, box.DataPoint.Label, ComboYear.SelectedValue);
                        }
                    }
                }
            }

        }


        DataTable Chart = new DataTable();
        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FNS_0006_0002_compare_Chart1");
            DataTable dtChart1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);
            query = DataProvider.GetQueryText("FNS_0006_0002_compare_Chart1_1");
            Chart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", Chart);

                if (dtChart1.Rows.Count > 0)
                {
                    dtChart1.Columns.RemoveAt(3);
                    dtChart1.Columns.RemoveAt(3);
                    UltraChart1.DataSource = dtChart1;
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
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.OriginY = 0;
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.SpanY = 2;
            for (int k = 1; k < e.Layout.Bands[0].Columns.Count; k++)
            {

                string formatString = "N2";
                if ((k == 3)||(k == 7))
                {
                    formatString = "N2";
                }
                else
                    if ((k == 4) || (k == 5) || (k == 8) || (k == 9))
                    {
                        formatString = "N0";
                    }
                    else
                    {
                        formatString = "N1";
                    }
                e.Layout.Bands[0].Columns[k].Format = formatString;
                e.Layout.Bands[0].Columns[k].Width = 106;
                e.Layout.Bands[0].Columns[k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[k].CellStyle.Padding.Right = 5;
                e.Layout.Bands[0].Columns[10].Hidden = true;
                e.Layout.Bands[0].Columns[11].Hidden = true;

                e.Layout.Bands[0].Columns[1].Header.Title = "Общая сумма начислений по земельному налогу";
                e.Layout.Bands[0].Columns[2].Header.Title = "Начислено земельного налога по физическим лицам";
                e.Layout.Bands[0].Columns[3].Header.Title = "Доля начислений по физическим лицам в общей сумме начислений";
                e.Layout.Bands[0].Columns[4].Header.Title = "Ранг муниципальных районов по доле начислений по физическим лицам в общей сумме начислений";
                e.Layout.Bands[0].Columns[5].Header.Title = "Ранг поселений по доле начислений по физическим лицам в общей сумме начислений";
                e.Layout.Bands[0].Columns[6].Header.Title = "Начислено земельного налога по юридическим лицам";
                e.Layout.Bands[0].Columns[7].Header.Title = "Доля начислений по юридическим лицам в общей сумме начислений";
                e.Layout.Bands[0].Columns[8].Header.Title = "Ранг муниципальных районов по доле начислений по юридическим лицам в общей сумме начислений";
                e.Layout.Bands[0].Columns[9].Header.Title = "Ранг поселений по доле начислений по юридическим лицам в общей сумме начислений";

                if (k > 1)
                {
                    e.Layout.Bands[0].Columns[k].Header.RowLayoutColumnInfo.OriginY = 1;
                }
                else
                {
                    e.Layout.Bands[0].Columns[k].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[k].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                e.Layout.Bands[0].Columns[1].Header.Caption = String.Format("Начислено земельного налога всего, {0}", units);
                e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("По физическим лицам, {0}", units);
                e.Layout.Bands[0].Columns[6].Header.Caption = String.Format("По юридическим лицам, {0}", units);
            }

            ColumnHeader ch = new ColumnHeader(true);
            ch.Caption = "в том числе:";
            ch.Title = "";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 2;
            ch.RowLayoutColumnInfo.SpanX = 10;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);
                
            

        }
        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int min = 0;
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {

                if (((i == 5) || (i == 9)) && e.Row.Index > dtGrid.Rows.Count)
                {
                    if (i == 5)
                    {
                        min = Convert.ToInt32(e.Row.Cells[i + 6].Value);
                    }
                    if (i == 9)
                    {
                        min = Convert.ToInt32(e.Row.Cells[i + 1].Value);
                    }
                    
                }
                {
                    if (i == 5)
                    {
                        if (e.Row.Cells[i].Value != null && min != null &&
                        e.Row.Cells[i].Value.ToString() != string.Empty &&
                        min.ToString() != string.Empty)
                        {

                            if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                                e.Row.Cells[i].Title = string.Format("Самый высокий ");
                            }
                            else if (firstrang[e.Row.Index])
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                                e.Row.Cells[i].Title = string.Format("Самый низкий ");
                            }
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                    if (i == 9)
                        if (e.Row.Cells[i].Value != null && min != null &&
                        e.Row.Cells[i].Value.ToString() != string.Empty &&
                        min.ToString() != string.Empty)
                        {

                            if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                                e.Row.Cells[i].Title = string.Format("Самый высокий ");
                            }
                            else if (secondrang[e.Row.Index])
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                                e.Row.Cells[i].Title = string.Format("Самый низкий ");
                            }
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                if (((i == 4) || (i == 8)))
                {
                    if (i == 4)
                    {
                      min = minf;
                    }
                    if (i == 8)
                    {
                     min = minu;
                    }
                    if (e.Row.Cells[i].Value != null && 
                        e.Row.Cells[i].Value.ToString() != string.Empty )
                    {

                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = string.Format("Самый высокий ");
                        }
                        else if (Convert.ToInt32(e.Row.Cells[i].Value) == min)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = string.Format("Самый низкий ");
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }                
            
                if (e.Row.Cells[0].Value != null &&
                       (e.Row.Cells[0].Value.ToString().Contains("МР")) || (e.Row.Cells[0].Value.ToString().Contains("бюджеты")) || (e.Row.Cells[0].Value.ToString().Contains("всего")))
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
            if (e.CurrentColumnIndex > 1)
            {
                e.HeaderText = "в том числе";//UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0].Trim();
            }
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {

            int columnCount = UltraWebGrid.Columns.Count;
            int width = 150;
            e.CurrentWorksheet.Rows[7].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.CurrentWorksheet.Rows[5].Height = 100;
            
            e.CurrentWorksheet.Columns[0].Width = width * 65;
            e.CurrentWorksheet.Columns[1].Width = width * 30;
            e.CurrentWorksheet.Columns[2].Width = width * 30;
            e.CurrentWorksheet.Columns[3].Width = width * 30;
            e.CurrentWorksheet.Columns[4].Width = width * 30;
            e.CurrentWorksheet.Columns[5].Width = width * 30;
            e.CurrentWorksheet.Columns[6].Width = width * 30;
            e.CurrentWorksheet.Columns[7].Width = width * 30;
            e.CurrentWorksheet.Columns[8].Width = width * 30;
            e.CurrentWorksheet.Columns[9].Width = width * 30;
            e.CurrentWorksheet.Columns[10].Width = width * 30;
            e.CurrentWorksheet.Columns[11].Width = width * 30;
            e.CurrentWorksheet.Columns[12].Width = width * 30;
            e.CurrentWorksheet.Columns[13].Width = width * 30;
            e.CurrentWorksheet.Columns[14].Width = width * 30;

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
                if ((i == 3) || (i == 7))
                {
                    e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.000";
                }
                else
                    if ((i == 4) || (i == 5) || (i == 8) || (i == 9))
                    {
                        e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0;[Red]-#,##0";
                    }
                    else
                    {
                        e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.0;[Red]-#,##0.0";
                    }
                e.CurrentWorksheet.Columns[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
            }
            for (int i = 3; i <  5; i++)
            {
                e.CurrentWorksheet.Rows[i].Height = 20 * 35;
            }
            e.CurrentWorksheet.Rows[5].Height = 20 * 15;
            for (int k = 1; k < columnCounttt; k = k + 1)
            {
                e.CurrentWorksheet.Rows[3].Cells[k].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[4].Cells[k].CellFormat.WrapText = ExcelDefaultableBoolean.True;
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
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font.Bold = true;
            title.AddContent(ChartCaption1.Text);
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            Image img = UltraGridExporter.GetImageFromChart(UltraChart1);
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