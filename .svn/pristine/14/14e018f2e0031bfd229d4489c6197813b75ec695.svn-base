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

namespace Krista.FM.Server.Dashboards.reports.FNS_0003_0002
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
        private CustomParam okved;
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
            if (okved == null)
            {
                okved = UserParams.CustomParam("okved");
            }
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth );
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.55);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
            UltraGridExporter1.HeaderChildCellHeight = 100;
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.55);
            UltraChart1.ChartType = ChartType.ColumnChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL> <DATA_VALUE:N2> руб.";
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.Axis.X.Extent = 170;
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
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;

            System.Drawing.Color color1 = System.Drawing.Color.LightGreen;
            System.Drawing.Color color2 = System.Drawing.Color.Red;
            UltraChart1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
            UltraChart1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));
            UltraChart1.ColorModel.Skin.ApplyRowWise = false;
            UltraChart1.Effects.Effects.Clear();
            GradientEffect effect = new GradientEffect();
            effect.Style = GradientStyle.ForwardDiagonal;
            effect.Coloring = GradientColoringStyle.Darken;
            effect.Enabled = true;
            UltraChart1.Effects.Enabled = true;
            UltraChart1.Effects.Effects.Add(effect); 
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.Data.SwapRowsAndColumns = false;
            
        }
        string Label3 = null;
        protected override void Page_Load(object sender, EventArgs e)
        {

            int start = Environment.TickCount;
            base.Page_Load(sender, e);
            CRHelper.SaveToUserAgentLog(String.Format("Базовый лоад {0}", Environment.TickCount - start));

            start = Environment.TickCount;
            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FNS_0003_0002_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                int endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                chartWebAsyncPanel.AddRefreshTarget(UltraChart1);

                dtDate = new DataTable();
                int firstYear = 2000;
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(endYear - 2, endYear));
                ComboYear.SelectLastNode();

                ComboKD.Width = 370;
                ComboKD.Title = "Вид налога";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillTaxesKDIncludingList());

                ComboOKV.Width = 370;
                ComboOKV.Title = "ОКВЭД";
                ComboOKV.MultiSelect = false;
                ComboOKV.ParentSelect = true;
                ComboOKV.FillDictionaryValues(CustomMultiComboDataHelper.FillOKVED(DataDictionariesHelper.OKVEDTypes));
                
            }

           string firstyear = "2005";
           Year.Value = ComboYear.SelectedValue;
           taxId.Value = ComboKD.SelectedValue;
           regionChart.Value = RegionsNamingHelper.LocalBudgetUniqueNames["Самара"];
           if (ComboOKV.SelectedValue.Contains("Все коды ОКВЭД"))
           {
               okved.Value = string.Empty;
           }
           else
           okved.Value = string.Format(".[{0}]",ComboOKV.SelectedValue);
           Page.Title = "Анализ поступления и недоимки по ОКВЭД";
           Label1.Text = Page.Title;
           Label2.Text = String.Format("{0}, {1}, за {2} год, руб.", ComboKD.SelectedValue, ComboOKV.SelectedValue, ComboYear.SelectedValue);
           Label3 = String.Format("{0}, {1}, за {2} год, руб.", ComboKD.SelectedValue, ComboOKV.SelectedValue, ComboYear.SelectedValue);
           
           
           UserParams.PeriodYear.Value = "2008";
           UltraWebGrid.Bands.Clear();
           UltraWebGrid.DataBind();
           string patternValue = UserParams.StateArea.Value;
           int defaultRowIndex = 0;
           regionChart.Value = RegionsNamingHelper.LocalBudgetUniqueNames[UltraWebGrid.Rows[defaultRowIndex].Cells[0].Value.ToString()];
           if (!Page.IsPostBack)
           {
               UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
               ChartCaption.Text = String.Format("Анализ поступления и недоимки по ОКВЭД, {3}, {0}, {1}, за {2} год, руб.", ComboKD.SelectedValue, ComboOKV.SelectedValue, ComboYear.SelectedValue, row.Cells[0].Value.ToString());
               ActiveGridRow(row);
               
           }
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
            if (row.Cells[0].Value.ToString().Contains("Итого"))
            {
                if (row.Cells[0].Value.ToString().Contains("городским"))
                {
                    regionChart.Value = String.Format("[Районы__Сопоставимый].[Районы__Сопоставимый].[{0}]", row.Cells[0].Value);                }
            }
            else
            regionChart.Value = RegionsNamingHelper.LocalBudgetUniqueNames[row.Cells[0].Value.ToString()];
         
        }

        string[,] dtIndication;
        int citycount = 0;
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {

                string query = DataProvider.GetQueryText("FNS_0003_0002_compare_Grid");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование городов и районов", dtGrid);
                UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
                dtIndication = new string[dtGrid.Rows.Count, dtGrid.Columns.Count];
          if (dtGrid.Rows.Count > 0)
            {
                for (int k = 1; k < dtGrid.Columns.Count; k++)
                {
                    if ((k != 5) && (k != 6) && (k != 7) && (k != 12) && (k != 13) && (k != 14))
                    {   int max = 0;
                        int min = 0;
                        int j = 0;
                        {
                            for (j = 0; !dtGrid.Rows[j][0].ToString().Contains("Итого"); j++)
                            {
                                if ((dtGrid.Rows[j][k] != DBNull.Value)) 
                                    {
                                        try
                                        {
                                            if (Convert.ToInt64(dtGrid.Rows[j][k]) > Convert.ToInt64(dtGrid.Rows[max][k]))
                                            {
                                                max = j;
                                            }
                                            if (Convert.ToInt64(dtGrid.Rows[j][k]) < Convert.ToInt64(dtGrid.Rows[min][k]))
                                            {
                                                min = j;
                                            }
                                        }
                                        catch
                                        {
                                        min++;
                                        max++;
                                        j = j - 1;
                                        } 
                                    }

                            }
                        }
                        if (min != max)
                        {
                            dtIndication[min, k] = "минимум";
                            dtIndication[max, k] = "максимум";
                        }
 
                        
                        max = j + 1;
                        min = max;
                        citycount = j;
                        for (int q = j + 1; !dtGrid.Rows[q][0].ToString().Contains("Итого"); q++)
                        {
                            
                            if (dtGrid.Rows[q][k] != DBNull.Value)
                                try
                                {
                                    if (Convert.ToInt32(dtGrid.Rows[q][k]) > Convert.ToInt32(dtGrid.Rows[max][k]))
                                    {
                                        max = q;
                                    }
                                    if (Convert.ToInt32(dtGrid.Rows[q][k]) < Convert.ToInt32(dtGrid.Rows[min][k]))
                                    {
                                        min = q;
                                    }
                                }
                                catch 
                                {
                                    min++;
                                    max++;
                                    q = q - 1;
                                }
                        }
                        if (min != max)
                        {
                           dtIndication[min, k] = "минимум";
                           dtIndication[max, k] = "максимум";
                        }
                        
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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
     
            for (int k = 1; k < e.Layout.Bands[0].Columns.Count; k++)
            {
                string formatString = "N2";
                e.Layout.Bands[0].Columns[k].Format = formatString;
                e.Layout.Bands[0].Columns[k].Width = 120;
                e.Layout.Bands[0].Columns[k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[k].CellStyle.Padding.Right = 5;
                e.Layout.Bands[0].Columns[k].Header.RowLayoutColumnInfo.OriginY = 1;
            }
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.OriginY = 0;
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.SpanY = 2;
            e.Layout.Bands[0].Columns[1].Header.Caption = "I квартал";
            e.Layout.Bands[0].Columns[2].Header.Caption = "II квартал";
            e.Layout.Bands[0].Columns[3].Header.Caption = "III квартал";
            e.Layout.Bands[0].Columns[4].Header.Caption = "IV квартал";
            e.Layout.Bands[0].Columns[5].Header.Caption = "Отношение поступления за II кв. к поступлению за I кв., %";
            e.Layout.Bands[0].Columns[6].Header.Caption = "Отношение поступления за III кв. к поступлению за I кв., %";
            e.Layout.Bands[0].Columns[7].Header.Caption = "Отношение поступления за IV кв. к поступлению за I кв., %";
            e.Layout.Bands[0].Columns[8].Header.Caption = "I квартал";
            e.Layout.Bands[0].Columns[9].Header.Caption = "II квартал";
            e.Layout.Bands[0].Columns[10].Header.Caption = "III квартал";
            e.Layout.Bands[0].Columns[11].Header.Caption = "IV квартал";
            e.Layout.Bands[0].Columns[12].Header.Caption = "Отношение поступления за II кв. к поступлению за I кв. с учетом недоимки, %";
            e.Layout.Bands[0].Columns[13].Header.Caption = "Отношение поступления за III кв. к поступлению за I кв. с учетом недоимки, %";
            e.Layout.Bands[0].Columns[14].Header.Caption = "Отношение поступления за  IV кв. к поступлению за I кв., с учетом недоимки, %";
 
            ColumnHeader ch = new ColumnHeader(true);
            ch.Caption = String.Format("Поступление по налогу");
            ch.Title = "";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 1;
            ch.RowLayoutColumnInfo.SpanX = 7;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = String.Format("Недоимка по налогу"); 
            ch.Title = "";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 8;
            ch.RowLayoutColumnInfo.SpanX = 7;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);
  
        }
        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
           
            for (int i = 5; i < e.Row.Cells.Count; i++)
            {
                if ((i != 8) && (i != 9) && (i != 10) && (i != 11))
                {
                    if (!(i > 7))
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            if (Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                                e.Row.Cells[i].Title = "Снижение поступлений по сравнению с первым кварталом";
                            }
                            else if (Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                                e.Row.Cells[i].Title = "Увеличение поступлений по сравнению с первым кварталом";
                            }
                        }

                        string style = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        e.Row.Cells[i].Style.CustomRules = style;
                    }
                else//иногда значения этих стрелок дифференцируются постановщиками по группам колонок, для этого следующий блок
                {
                   
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            if (Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                                e.Row.Cells[i].Title = "Снижение поступлений по сравнению с первым кварталом";
                            }
                            else if (Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                                e.Row.Cells[i].Title = "Увеличение поступлений по сравнению с первым кварталом";
                            }
                        }

                        string style = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        e.Row.Cells[i].Style.CustomRules = style;
                    }
                }
            }
            }
            CRHelper.SaveToErrorLog(e.Row.Index.ToString());
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                if (i < 7)
                {
                    if (dtIndication[e.Row.Index, i] == "максимум")
                    {

                        e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                        if (e.Row.Index < citycount)
                        {
                            e.Row.Cells[i].Title = "Максимальное поступление среди городских округов";
                        }
                        else
                        {
                            e.Row.Cells[i].Title = "Максимальное поступление среди муниципальных районов";
                        }
                    }
                    if (dtIndication[e.Row.Index, i] == "минимум")
                    {

                        e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                        if (e.Row.Index < citycount)
                        {
                            e.Row.Cells[i].Title = "Минимальное поступление среди городских округов";
                        }
                        else
                        {
                            e.Row.Cells[i].Title = "Минимальное поступление среди муниципальных районов";
                        }
                    }
                }
                else
                {
                    if (dtIndication[e.Row.Index, i] == "максимум")
                    {

                        e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                        if (e.Row.Index < citycount)
                        {
                            e.Row.Cells[i].Title = "Максимальная недоимка среди городских округов";
                        }
                        else
                        {
                            e.Row.Cells[i].Title = "Максимальная недоимка среди муниципальных районов";
                        }
                    }
                    if (dtIndication[e.Row.Index, i] == "минимум")
                    {

                        e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                        if (e.Row.Index < citycount)
                        {
                            e.Row.Cells[i].Title = "Минимальная недоимка среди городских округов";
                        }
                        else
                        {
                            e.Row.Cells[i].Title = "Минимальная недоимка среди муниципальных районов";
                        }
                    }
                }
             e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
            }
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[0].Value != null &&
                       (e.Row.Cells[0].Value.ToString().Contains("Итого") || e.Row.Cells[0].Value.ToString().Contains("Всего")))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }
        }
        
        #endregion

        #region Обработчики диаграмы

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FNS_0003_0002_Chart1");
            DataTable dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            UltraChart1.DataSource = dtChart;
        }

        void UltraChart1_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {

        }
        #endregion
        
        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            if (e.Row != null)
            {
                CRHelper.SaveToErrorLog("Индекс"+e.Row.Index.ToString());
                ChartCaption.Text = String.Format("Анализ поступления и недоимки по ОКВЭД, {3}, {0}, {1}, за {2} год, тыс.руб.", ComboKD.SelectedValue, ComboOKV.SelectedValue, ComboYear.SelectedValue, UltraWebGrid.Rows[e.Row.Index].Cells[0].Value.ToString());
                CRHelper.FindGridRow(UltraWebGrid, "", 0, e.Row.Index);
                ActiveGridRow(e.Row);
                UltraChart1.DataBind();
            }
        }

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            ActiveGridRow(UltraWebGrid.Rows[3]);
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Page.Title;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = Label3;
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            if (e.CurrentColumnIndex > 0)
            {
                if (e.CurrentColumnIndex < 8)
                {
                    e.HeaderText = "Поступление по налогу";
                }
                else
                    if (e.CurrentColumnIndex > 7)
                    {
                        e.HeaderText = "Недоимка по налогу";
                    }
                   
            }
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {

            int columnCount = UltraWebGrid.Columns.Count;
            int width = 150;
            e.CurrentWorksheet.Rows[7].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.CurrentWorksheet.Columns[0].Width = width * 37 * 2;
            e.CurrentWorksheet.Columns[1].Width = width * 37;
            e.CurrentWorksheet.Columns[2].Width = width * 37;
            e.CurrentWorksheet.Columns[3].Width = width * 37;
            e.CurrentWorksheet.Columns[4].Width = width * 37;
            e.CurrentWorksheet.Columns[5].Width = width * 37 * 2;
            e.CurrentWorksheet.Columns[6].Width = width * 37 * 2;
            e.CurrentWorksheet.Columns[7].Width = width * 37 * 2;
            e.CurrentWorksheet.Columns[8].Width = width * 37;
            e.CurrentWorksheet.Columns[9].Width = width * 37;
            e.CurrentWorksheet.Columns[10].Width = width * 37;
            e.CurrentWorksheet.Columns[11].Width = width * 37;
            e.CurrentWorksheet.Columns[12].Width = width * 37 * 2;
            e.CurrentWorksheet.Columns[13].Width = width * 37 * 2;
            e.CurrentWorksheet.Columns[14].Width = width * 37 * 2;

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

                e.CurrentWorksheet.Rows[3].Height = 13 * 35;//Ширина строк
                e.CurrentWorksheet.Rows[4].Height = 13 * 35;
                e.CurrentWorksheet.Rows[3].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[4].CellFormat.WrapText = ExcelDefaultableBoolean.True;
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Анализ поступления и недоимки");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            UltraGridExporter.ChartExcelExport(sheet2.Rows[2].Cells[0], UltraChart1);
            sheet2.Rows[0].Cells[0].Value = ChartCaption.Text;
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
            title.AddContent(Page.Title + ".\n");
            title.AddContent(Label2.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            e.Section.AddPageBreak();
            IText title = e.Section.AddText();
            title.AddContent(ChartCaption.Text);

            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart1);
            e.Section.AddImage(img);
            e.Section.AddPageBreak();
            title = e.Section.AddText();

            title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font.Bold = true;

            title = e.Section.AddText();
            font = new Font("Verdana", 12);

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