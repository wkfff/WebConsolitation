using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.Misc;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;
using Infragistics.UltraChart.Core;
using Color = System.Drawing.Color;
using Graphics = System.Drawing.Graphics;
using Image = System.Drawing.Image;
using TextAlignment = Infragistics.Documents.Reports.Report.TextAlignment;
using Dundas.Maps.WebControl;
using System.Drawing.Imaging;
using Infragistics.UltraGauge.Resources;

namespace Krista.FM.Server.Dashboards.reports.EO.EO_0005.EO_0005_MP.EO_0005_MP_001
{
    public partial class Default : CustomReportPage
    {
        string header = "Сводный отчет об исполнении долгосрочных муниципальных целевых программ";
        string chartHeader = "Ход исполнения программы";
        string gaugeHeader = "Процент исполнения программы";
        string C0 = "Наименование целевой программы";
        string C1 = "КОСГУ";
        string C2 = "Тип";
        string C3 = "Утверждено ассигнований на текущий год";
        string C4 = "Кассовый план";
        string C5 = "Исполнено с начала года";
        string C6 = "Исполнено за отчетный месяц";
        string C7 = "Сумма неосвоенных средств с начала года";
        string C8 = "Причина неисполнения";
        private DataTable T = new DataTable();
        private DataTable dt1 = new DataTable();
        private DataTable dt2 = new DataTable();
        private DataSet tableDataSet = new DataSet();
        private CustomParam p1;
        private CustomParam p2;
        private CustomParam p3;
        private CustomParam p4;//полугодие
        private CustomParam p5;//квартал
        private CustomParam p6;//регион
        private CustomParam p7;//программа
        private CustomParam p8;//КОСГУ
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        //private CustomParam lastYear;
        //private CustomParam lastMonth;
        private CellSet CS1;
        private CellSet CS2;
        private CellSet cs;
        private CellSet regionsCellSet;
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Visible = false;

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 3);
            Month.Width = 160;
            Zakaz.Width = 500;
            region.Width = 300;
            p1 = UserParams.CustomParam("year");
            p2 = UserParams.CustomParam("month");
            p3 = UserParams.CustomParam("zakazchik");
            p4 = UserParams.CustomParam("halfyear");
            p5 = UserParams.CustomParam("quarteryear");
            p6 = UserParams.CustomParam("region");
            p7 = UserParams.CustomParam("programm");
            p8 = UserParams.CustomParam("kosgu");
            
           // Chart.Width = 9000;
            //Chart.Legend.SpanPercentage = 25;
            Label1.Text = header;
            Label3.Text = chartHeader;
            Label2.Text = gaugeHeader;
        }

        Dictionary<string, int> YearsLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();

            for (int i = cs.Axes[1].Positions.Count - 1; i>=0;  i--)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
            }
            return d;
        }

        Dictionary<string, int> MonthsLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();

            for (int i = (cs.Axes[1].Positions.Count - 1); i >=0 ; i--)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
            }
            return d;
        }

        Dictionary<string, int> ZakazLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add("Все заказчики", 1);
            for (int i =0 ;i<=cs.Axes[1].Positions.Count - 1; i++)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
            }
            
            return d;
        }

        Dictionary<string, int> RegionsLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            if (cs.Axes[1].Positions.Count<=1) {
                region.Visible = false;
            }
                for (int i = 0; i <= cs.Axes[1].Positions.Count - 1; i++)
                {
                    d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
                }
                Year.SetСheckedState(cs.Axes[1].Positions[cs.Axes[1].Positions.Count-1].Members[0].Caption,true);
            return d;
            
        }
        public string GetLastDate(string query)
        {
            CellSet data;
            data = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(query));
            return data.Axes[1].Positions[0].Members[0].Caption;
        }

        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);
        }

        protected override void Page_LoadComplete(object sender, EventArgs e)
        {
            base.Page_LoadComplete(sender, e);
        }
        public void SetChartRange(double max)
        {
            Chart.Axis.Y.RangeType = AxisRangeType.Custom;
            Chart.Axis.Y.RangeMax = (int)max+0.5;
            Chart.Axis.Y.RangeMin = 0;
            Chart.Axis.Y.TickmarkStyle = AxisTickStyle.DataInterval;
            Chart.Axis.Y.TickmarkInterval = (int)Chart.Axis.Y.RangeMax/10;
        }

        public String getRegion()
        {
            if (region.Visible == true)
            {
                CellSet regionCellSet = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("region"));
                int k = region.SelectedIndex;
                return regionCellSet.Axes[1].Positions[k].Members[0].UniqueName;

            }
            else 
            {
                return RegionSettingsHelper.Instance.RegionBaseDimension;
            }
            
            
        
        }
        public void Format(string s,int index)
        { 
            int k=0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '.')
                { k =k+1; }
            }
            if (k == 2)
            { UltraWebGrid.Rows[index].Cells[1].Style.Font.Bold = true; }
            if (k == 3)
            { UltraWebGrid.Rows[index].Cells[1].Style.Font.Underline = true; }
            if (k == 5)
            { UltraWebGrid.Rows[index].Cells[1].Style.Font.Italic = true; }

        }
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            Chart.Width = 500;
            
                 if (!Page.IsPostBack)
                 {
                     
                     Zakaz.FillDictionaryValues(ZakazLoad("projects"));
                     if (Zakaz.SelectedValue == "Все заказчики")
                     {
                         p3.Value = "[Программы].[Заказчики].[Все заказчики]";
                     }
                     else
                     {
                         p3.Value = string.Format("[Программы].[Заказчики].[Все заказчики].[{0}]", Zakaz.SelectedValue);
                     }
                     
                     Year.FillDictionaryValues(YearsLoad("years"));
                     Month.FillDictionaryValues(MonthsLoad("months"));
                     region.FillDictionaryValues(RegionsLoad("region"));
                     Year.Title = "Год";
                     Month.Title = "Месяц";
                     Zakaz.Title = "Заказчики";
                     region.Title = "Территория РФ";
                     p1.Value = Year.SelectedValue;
                     p2.Value = Month.SelectedValue;
                     p4.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(Month.SelectedIndex + 1));
                     p5.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(Month.SelectedIndex + 1));
                     p6.Value = getRegion();
                     RefreshPanel1.AddRefreshTarget(Chart);
                     RefreshPanel1.AddRefreshTarget(UltraGauge1);
                     RefreshPanel1.AddLinkedRequestTrigger(UltraWebGrid);
                 }
                 else
                 {
                     if (Zakaz.SelectedValue == "Все заказчики")
                     {
                         p3.Value = "[Программы].[Заказчики].[Все заказчики]";
                     }
                     else
                     {
                         p3.Value =string.Format("[Программы].[Заказчики].[Все заказчики].[{0}]", Zakaz.SelectedValue);
                     }
                     p1.Value = Year.SelectedValue;
                     p2.Value = Month.SelectedValue;
                     p4.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(Month.SelectedIndex + 1));
                     p5.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(Month.SelectedIndex + 1));
                     p6.Value = getRegion();
                 }
                    UltraWebGrid.DataBind();
                    string s;
                    for (int i = 0; i < UltraWebGrid.Rows.Count; i++)
                    {
                        s = UltraWebGrid.Rows[i].Cells[0].Text;
                        UltraWebGrid.Rows[i].Cells[0].Text = UltraWebGrid.Rows[i].Cells[0].Text.Split(';')[0];
                        UltraWebGrid.Rows[i].Cells[1].Text = s.Split(';')[1].Remove(0, 1);
                        if (UltraWebGrid.Rows[i].Cells[2].Text == "Программа")
                        {
                            UltraWebGrid.Rows[i].Cells[0].Style.Font.Bold = true;
                        }
                    }
                    UltraWebGrid.Rows.Add();

                    for (int i = 0; i < UltraWebGrid.Rows.Count; i++)
                    {
                       // Format(CS1.Axes[1].Positions[i].Members[1].UniqueName,i);
                    }
            
                    
                    for (int i = 0; i < UltraWebGrid.Rows.Count; i++)
                    {
                        if ((UltraWebGrid.Rows[i].Cells[2].Text == "Программа") && (UltraWebGrid.Rows[i].Cells[1].Text!="Все операции"))
                            {
                               UltraWebGrid.Rows[i].Hidden=true;
                            
                                //i = i - 1;   
                            }
                    }


                        try
                        {
                            p7.Value = CS1.Axes[1].Positions[0].Members[0].UniqueName;
                            p8.Value = CS1.Axes[1].Positions[0].Members[1].UniqueName;
                            UltraWebGrid.Rows[0].Selected = true;
                            UltraWebGrid.Rows[0].Activated = true;
                            UltraGauge1.DataBind();
                            SetChartRange((double)CS1.Cells[1, 0].Value);
                        }
                        catch { };
                    UltraWebGrid.DisplayLayout.CellClickActionDefault = CellClickAction.NotSet;
                    Chart.DataBind();
                    Chart.Width = 850;
                    Chart.Legend.SpanPercentage = 23;


                   
        }
       

        
        protected void Chart_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable chart_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart"), "_", chart_master);
                Chart.DataSource = chart_master.DefaultView;

            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message, ex);
            }
            

        }

        #region 
        DataTable dtChart = new DataTable();
        public DataTable GetDSForChart(string sql)
        {
            dtChart = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(s, "Показатель", dtChart);
            return dtChart;
        }
        #endregion



        #region Экспорт Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value=Year.SelectedValue;
            e.CurrentWorksheet.Rows[1].Cells[1].Value = Month.SelectedValue;
            e.CurrentWorksheet.Rows[1].Cells[2].Value = Zakaz.SelectedValue;
            e.CurrentWorksheet.Rows[1].Cells[3].Value = region.SelectedValue;
            
        }


        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            string formatString = "#,##0.00;[Red]-#,##0.00";
             e.CurrentWorksheet.Columns[0].Width = 250*37;
             e.CurrentWorksheet.Columns[1].Width = 300 * 37;
            
                for (int i = 2; i < UltraWebGrid.Bands[0].Columns.Count; i++)
                {

                        
                        e.CurrentWorksheet.Columns[i].Width = 225*37;
                        e.CurrentWorksheet.Columns[i].Hidden = false;
                }
               //ширина первого столбца
                for (int i = 0; i < UltraWebGrid.Rows.Count; i++)
                {
                    e.CurrentWorksheet.Rows[i+4].Cells[0].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
                    e.CurrentWorksheet.Rows[i+4].Cells[1].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;

                    
                }
              /*  e.CurrentWorksheet = e.Workbook.Worksheets[1]; //Экспорт рисунка
                e.CurrentWorksheet.Columns[0].Width = 30000;//ширина диаграммы
                MemoryStream imageStream = new MemoryStream();
                Chart.SaveTo(imageStream, ImageFormat.Png);
                Infragistics.Documents.Excel.WorksheetImage imageShape =
                new Infragistics.Documents.Excel.WorksheetImage(Image.FromStream(imageStream));
                Infragistics.Documents.Excel.WorksheetCell cellA1 = e.CurrentWorksheet.Rows[0].Cells[0];
                imageShape.TopLeftCornerCell = cellA1;
                imageShape.TopLeftCornerPosition = new PointF(0.0F, 0.0F);
                imageShape.BottomRightCornerCell = e.CurrentWorksheet.Rows[25].Cells[0];
                
                imageShape.BottomRightCornerPosition = new PointF(100.0F, 100.0F);
                e.CurrentWorksheet.Shapes.Add(imageShape);*/
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
          //  e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
          //  e.HeaderText = UltraWebGrid.DisplayLayout.Bands[1].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
         //   Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
            //UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }
        
        #endregion

        protected void SetErorFonn(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "В настоящий момент данные отсутствуют";
            e.LabelStyle.FontColor = System.Drawing.Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new System.Drawing.Font("Verdana", 30);
        }

        protected void G1_DataBinding(object sender, EventArgs e)
        {
            DataTable table1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("G1"),"Наименование целевой программы",table1);
            UltraWebGrid.DataSource = table1;
            string s = DataProvider.GetQueryText("G1");
            CS1 = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s);
            //CS2 = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("AllProg"));
        }

        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (!Page.IsPostBack)
            {
                UltraWebGrid.Columns.Add("Косгу");
                UltraWebGrid.Bands[0].Columns[UltraWebGrid.Bands[0].Columns.Count - 1].Move(1);
            }
            e.Layout.Bands[0].Columns[1].Header.Caption = C1;
            e.Layout.Bands[0].Columns[2].Header.Caption = C2;
            e.Layout.Bands[0].Columns[3].Header.Caption = C3;
            e.Layout.Bands[0].Columns[4].Header.Caption = C4;
            e.Layout.Bands[0].Columns[5].Header.Caption = C5;
            e.Layout.Bands[0].Columns[6].Header.Caption = C6;
            e.Layout.Bands[0].Columns[7].Header.Caption = C7;
            e.Layout.Bands[0].Columns[8].Header.Caption = C8;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.2);
            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = 1 == 1;
            for (int i = 1; i < UltraWebGrid.Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.1);
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
            }
            e.Layout.Bands[0].Columns[2].Hidden = true;
            e.Layout.Bands[0].Columns[0].MergeCells = true;
            for (int i = 3; i < UltraWebGrid.Columns.Count - 1; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
        }

        protected void UltraWebGrid1_ActiveRowChange(object sender, RowEventArgs e)
        {
            p7.Value = CS1.Axes[1].Positions[e.Row.Index].Members[0].UniqueName;
            p8.Value = CS1.Axes[1].Positions[e.Row.Index].Members[1].UniqueName;
            SetChartRange((double)CS1.Cells[1, e.Row.Index].Value);
            Chart.DataBind();
            UltraGauge1.DataBind();

        }

        protected void UltraGauge1_DataBinding(object sender, EventArgs e)
        {
            try
            {
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("gauge"));
                ((RadialGauge)UltraGauge1.Gauges[0]).Scales[0].Markers[0].Value = cs.Cells[0, 0].Value;
                
            }
            catch
            { }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
           /* if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Text = "Всего по программам";
                e.Row.Cells[1].Text = "Все операции";
                e.Row.Cells[3].Text=CS2.Cells[1,0].Value.ToString();
            }*/
        }


    }
}
