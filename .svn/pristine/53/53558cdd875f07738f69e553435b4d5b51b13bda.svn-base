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

namespace Krista.FM.Server.Dashboards.reports.EO.EO_0005.EO_0005_MP0.EO_0005_MP_001
{
    public partial class Default : CustomReportPage
    {
        string header = "Сводный отчет об исполнении долгосрочных муниципальных целевых программ";
        string subTitle = "Анализ исполнения долгосрочных муниципальных целевых программ";
        string chartHeader = "Финансирование программы «{0}», тыс. руб.";
        string chartHeaderAll = "Общий объем финансирования, тыс. руб.";
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
        private CustomParam SelectedYear;
        private CustomParam SelectedMonth;
        private CustomParam Customer;
        private CustomParam HalfYear;//полугодие
        private CustomParam QuarterYear;//квартал
        private CustomParam CurrentRegion;//регион
        private CustomParam Programm;//программа
        private CustomParam KOSGY;//КОСГУ
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        //private CustomParam lastYear;
        //private CustomParam lastMonth;
        private CellSet CS1;
        private CellSet CS2;
        //private CellSet cs;
        private CellSet regionsCellSet;
        
        string BN = "IE";
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            base.Page_PreLoad(sender, e);
            
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Visible = false;
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);

            Month.Width = 250;
            Zakaz.Width = 500;
            region.Width = 300;
            UltraWebGrid.Height = Unit.Empty;

            SelectedYear = UserParams.CustomParam("year");
            SelectedMonth = UserParams.CustomParam("month");
            Customer = UserParams.CustomParam("zakazchik");
            HalfYear = UserParams.CustomParam("halfyear");
            QuarterYear = UserParams.CustomParam("quarteryear");
            CurrentRegion = UserParams.CustomParam("region");
            Programm = UserParams.CustomParam("programm");
            KOSGY = UserParams.CustomParam("kosgu");

            Label1.Text = header;
            Label2.Text = subTitle;
        }

        protected int DotCount(string s)
        { 
            int k=0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '.')
                {
                    k += 1;
                }
            }
            return k;
        }
        Dictionary<string, int> MonthsLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            string s = "";
            for (int i = 1; i < cs.Axes[1].Positions.Count; i++)
            {
                
                if (cs.Axes[1].Positions[i].Members[0].LevelDepth == 1)
                {
                    s = cs.Axes[1].Positions[i].Members[0].Caption;
                    d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
                }
                if (cs.Axes[1].Positions[i].Members[0].LevelDepth == 4)
                {
                    d.Add(cs.Axes[1].Positions[i].Members[0].Caption+" "+s+" года", 1);
                }
            }
            return d;
        }

        Dictionary<string, int> ZakazLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add("Все заказчики", 0);
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
        public void Formatr(string s,int index)
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
        public void FormatUltraWebGrid()//форматирование содержимого грида
        {
                for (int i = 1; i < UltraWebGrid.Rows.Count; i++)//удаление строк с мероприятиями или косгу
                {
                    if ((Mer.Checked == false)&&((UltraWebGrid.Rows[i].Cells[2].Text!="Программа")||(UltraWebGrid.Rows[i].Cells[0].Text == "Данные всех источников")))
                    {
                        UltraWebGrid.Rows.Remove(UltraWebGrid.Rows[i]);
                        //UltraWebGrid.Rows[i].Hidden = true;
                      //  UltraWebGrid.Rows[i].Delete();
                        i-=1;
                    }
                    if ((KOSGU.Checked==false)&&(UltraWebGrid.Rows[i].Cells[1].Text!="Все операции"))
                    {
                        UltraWebGrid.Rows.Remove(UltraWebGrid.Rows[i]);
                        //UltraWebGrid.Rows[i].Hidden = true;
                     //UltraWebGrid.Rows[i].Delete();
                        i-=1;
                    }
                }
                
                UltraWebGrid.Rows[0].Cells[0].Text = "ВСЕГО ПО ПРОГРАММАМ";
                UltraWebGrid.DisplayLayout.NoDataMessage = "В настоящий момент данные отсутствуют";
                UltraWebGrid.Rows[0].Cells[8].Text = GenGanga(double.Parse(UltraWebGrid.Rows[0].Cells[8].Text), "/EO_MOP_GAdge", "../../../../../TemporaryImages", 200, 30) + " " + Math.Round(double.Parse(UltraWebGrid.Rows[0].Cells[8].Text), 0) + "%";
                for (int i = 0; i < UltraWebGrid.Rows.Count; i++)//для отображения полоски с процентами
                {
                    if ((UltraWebGrid.Rows[i].Cells[2].Text == "Программа") && (UltraWebGrid.Rows[i].Cells[1].Text=="Все операции"))
                    {
                        double value = double.Parse(UltraWebGrid.Rows[i].Cells[8].Text);
                        UltraWebGrid.Rows[i].Cells[8].Text = GenGanga(value, "/EO_MOP_GAdge", "../../../../../TemporaryImages", 200, 30) + " " + Math.Round(value, 0) + "%";
                    }
                }
                UltraWebGrid.Columns[UltraWebGrid.Columns.Count-2].Move(1);
                
        }
        /*protected string GenGanga(double value, string prefix, string prefixPage, int width, int height)
        {
            //крута! каментов больше чем кода!
            value = Math.Round(value);
            string path = prefix + value.ToString() + ".png";
            System.Double V1 = value;
            Infragistics.UltraGauge.Resources.LinearGaugeScale scale = ((Infragistics.UltraGauge.Resources.LinearGauge)UltraGauge1.Gauges[0]).Scales[0];
            scale.Markers[0].Value = V1;
            System.Drawing.Size size = new Size(width, height);
            UltraGauge1.SaveTo(Server.MapPath("~/TemporaryImages" + path), GaugeImageType.Png, size);
            return "<img style=\"FLOAT: left;\" src =\"" + prefixPage + path + "\"/>";
        }*/
        protected string GenGanga(double value, string prefix, string prefixPage, int width, int height)
        {
           /* value = Math.Round(value);
            string path = prefix + value.ToString() + ".png";
            System.Double V1 = value;
            Infragistics.UltraGauge.Resources.LinearGaugeScale scale = ((Infragistics.UltraGauge.Resources.LinearGauge)Ga1.Gauges[0]).Scales[0];
            scale.Markers[0].Value = V1;
            System.Drawing.Size size = new Size(width, height);
            Ga1.SaveTo(Server.MapPath("~/TemporaryImages" + path), GaugeImageType.Png, size);
            return "<img style=\"FLOAT: left;\" src =\"" + prefixPage + path + "\"/>";*/

            value = Math.Round(value);
            string path = prefix + value.ToString() + ".png";
            System.Double V1 = value;
            Infragistics.UltraGauge.Resources.LinearGaugeScale scale = ((Infragistics.UltraGauge.Resources.LinearGauge)Ga1.Gauges[0]).Scales[0];
            scale.Markers[0].Value = V1;
            Infragistics.UltraGauge.Resources.MultiStopLinearGradientBrushElement BrushElement =
             (Infragistics.UltraGauge.Resources.MultiStopLinearGradientBrushElement)(scale.Markers[0].BrushElement);
            BrushElement.ColorStops.Clear();
            if (V1 > 80)
            {

                BrushElement.ColorStops.Add(Color.FromArgb(223,255,192), 0);
                BrushElement.ColorStops.Add(Color.FromArgb(128, 255, 128), 0.41F);
                BrushElement.ColorStops.Add(Color.FromArgb(0, 192, 0), 0.428F);
                BrushElement.ColorStops.Add(Color.Green, 1); 
            }
            else
            {
                if (V1 < 50)
                {

                    BrushElement.ColorStops.Add(Color.FromArgb(253, 119, 119), 0);
                    BrushElement.ColorStops.Add(Color.FromArgb(239, 87, 87), 0.41F);
                    BrushElement.ColorStops.Add(Color.FromArgb(224, 0, 0), 0.428F);
                    BrushElement.ColorStops.Add(Color.FromArgb(199,0,0), 1); 
                }
                else
                {
                    BrushElement.ColorStops.Add(Color.FromArgb(255,255,128),0);
                    BrushElement.ColorStops.Add(Color.Yellow, 0.41F);
                    BrushElement.ColorStops.Add(Color.Yellow, 0.428F);
                    BrushElement.ColorStops.Add(Color.FromArgb(255,128,0), 1); 
                }
            }
            System.Drawing.Size size = new Size(200, height);
            Ga1.SaveTo(Server.MapPath("~/TemporaryImages" + path), GaugeImageType.Png, size);
            return "<img style=\"FLOAT: left;\" src =\"" + prefixPage + path + "\"/>";
        }
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                Zakaz.FillDictionaryValues(ZakazLoad("projects"));
                Month.FillDictionaryValues(MonthsLoad("months"));
                Month.SelectLastNode();

                region.FillDictionaryValues(RegionsLoad("region"));
                Month.Title = "Месяц";
                Zakaz.Title = "Заказчики";
                region.Title = "Территория РФ";
            }
            if (Zakaz.SelectedValue == "Все заказчики")
            {
                Customer.Value = "[Программы].[Заказчики].[Все заказчики]";
            }
            else
            {
                Customer.Value = string.Format("[Программы].[Заказчики].[Все заказчики].[{0}]", Zakaz.SelectedValue);
            }
            SelectedYear.Value = Month.SelectedNode.Parent.Text;
            SelectedMonth.Value = Month.SelectedValue.Split(' ')[0];
            int num = CRHelper.MonthNum(Month.SelectedValue.Split(' ')[0]);
            HalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(num));
            QuarterYear.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(num));
            CurrentRegion.Value = getRegion();
            UltraWebGrid.DataBind();

            if (UltraWebGrid.DataSource != null)
            {
                FormatUltraWebGrid();
                Programm.Value = CS1.Axes[1].Positions[0].Members[0].UniqueName;
                KOSGY.Value = CS1.Axes[1].Positions[0].Members[1].UniqueName;
                UltraWebGrid.Rows[0].Selected = true;
                UltraWebGrid.Rows[0].Activated = true;
                UltraWebGrid.Rows[0].Activate();
                UltraWebGrid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
                UltraWebGrid.DisplayLayout.CellClickActionDefault = CellClickAction.RowSelect;
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
       
            
        }


        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            try
            {
                string formatString = "#,##0.00;[Black]-#,##0.00";
                e.CurrentWorksheet.Columns[0].Width = 650 * 37;
                e.CurrentWorksheet.Columns[1].Width = 300 * 37;

                for (int i = 2; i < UltraWebGrid.Columns.Count; i++)
                {
                    e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                    e.CurrentWorksheet.Columns[i].Width = 225 * 37;
                    e.CurrentWorksheet.Columns[i].Hidden = false;
                }
                //ширина первого столбца
                e.CurrentWorksheet.Rows[1].Cells[1].CellFormat.Alignment = HorizontalCellAlignment.Right;
                for (int i = 0; i < UltraWebGrid.Rows.Count; i++)
                {
                    e.CurrentWorksheet.Rows[i + 1].Cells[0].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
                    e.CurrentWorksheet.Rows[i + 1].Cells[1].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
                    e.CurrentWorksheet.Rows[i + 1].Height = 500;
                    if (e.CurrentWorksheet.Rows[i + 1].Cells[1].Value.ToString().Contains("%"))
                    {
                        string s = e.CurrentWorksheet.Rows[i + 1].Cells[1].Value.ToString().Split('>')[1];
                        e.CurrentWorksheet.Rows[i + 1].Cells[1].Value = s;
                    }

                }
                

            }
            catch 
            {
                
            }
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {

        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
         //   Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 1;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
            //UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }
        
        #endregion

        protected void SetErorFonn(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            try
            {
                e.Text = "В настоящий момент данные отсутствуют";
                e.LabelStyle.FontColor = System.Drawing.Color.LightGray;
                e.LabelStyle.VerticalAlign = StringAlignment.Center;
                e.LabelStyle.HorizontalAlign = StringAlignment.Center;
                e.LabelStyle.Font = new System.Drawing.Font("Verdana", 30);
            }
            catch { }
        }

        protected void G1_DataBinding(object sender, EventArgs e)
        {
            UltraWebGrid.Rows.Clear();
            UltraWebGrid.Columns.Clear();
            DataTable table1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("G1"), "Наименование целевой программы", table1);
            string s = DataProvider.GetQueryText("G1");
            CS1 = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s);
            if (table1.Rows.Count == 0)
            {
                UltraWebGrid.DataSource = null;
                UltraWebGrid.Columns.Clear();
                UltraWebGrid.DisplayLayout.NoDataMessage = "В настоящее время данные отсутствуют";
            }
            else
            {
                UltraWebGrid.DataSource = table1;
            }
        }

        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            
                if ((!Page.IsPostBack) || (UltraWebGrid.Columns.Count < 10))
                {
                    UltraWebGrid.Columns.Add("Косгу");
                    UltraWebGrid.Bands[0].Columns[UltraWebGrid.Bands[0].Columns.Count - 1].Move(1);
                }
                for (int i = 3; i < e.Layout.Bands[0].Columns.Count-2; i++)
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ##0.00");
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                }
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "### ### ##0");
                    e.Layout.Bands[0].Columns[1].Header.Caption = C1;
                    e.Layout.Bands[0].Columns[2].Header.Caption = C2;
                    e.Layout.Bands[0].Columns[3].Header.Caption = C3;
                    e.Layout.Bands[0].Columns[4].Header.Caption = C4;
                    e.Layout.Bands[0].Columns[5].Header.Caption = C5;
                    e.Layout.Bands[0].Columns[6].Header.Caption = C6;
                    e.Layout.Bands[0].Columns[7].Header.Caption = C7;
                    e.Layout.Bands[0].Columns[8].Header.Caption = "Процент исполнения";
                    e.Layout.Bands[0].Columns[9].Header.Caption = C8;
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.32);
                e.Layout.Bands[0].Columns[0].Header.Style.Wrap = 1 == 1;

                for (int i = 1; i < UltraWebGrid.Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].CellStyle.Wrap = true;
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.076);
                    e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                }
                e.Layout.Bands[0].Columns[2].Hidden = true;
                e.Layout.Bands[0].Columns[1].Hidden = true;
                e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
                e.Layout.Bands[0].Columns[8].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[8].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.205);
                e.Layout.Bands[0].Columns[8].Header.Style.Wrap = 1 == 1;
            
        }



        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            #region Разделение КОСГУ и программ
            int spl = e.Row.Cells[0].Text.Split(';').Length - 1;
            e.Row.Cells[1].Text = e.Row.Cells[0].Text.Split(';')[spl].Remove(0, 1);
            e.Row.Cells[0].Text = e.Row.Cells[0].Text.Remove(e.Row.Cells[0].Text.LastIndexOf(';'));

            if ((e.Row.Cells[2].Text == "Программа") || (e.Row.Cells[0].Text == "Данные всех источников"))
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
            if (((e.Row.Cells[1].Text != "Все операции") || (e.Row.Cells[2].Text != "Программа")) && (e.Row.Index != 0))
            {
                e.Row.Cells[UltraWebGrid.Columns.Count - 2].Text = " ";
            }
            #endregion

            string ss = CS1.Axes[1].Positions[e.Row.Index].Members[1].UniqueName;

            Formatr(ss, e.Row.Index);
            if ((e.Row.Cells[1].Text != "Все операции"))//форматирование КОСГУ
            {
                e.Row.Cells[0].Text = "        " + e.Row.Cells[1].Text;
                e.Row.Cells[0].Style.Font.Bold = e.Row.Cells[1].Style.Font.Bold;
                e.Row.Cells[0].Style.Font.Italic = e.Row.Cells[1].Style.Font.Italic;
                e.Row.Cells[0].Style.Font.Overline = e.Row.Cells[1].Style.Font.Overline;
            }
            else//Перенос причин неисполнения в первый столбец
            {
                if (!String.IsNullOrEmpty(e.Row.Cells[e.Row.Cells.Count - 1].Text))
                {
                    e.Row.Cells[0].Style.BackgroundImage = "~/images/cornerGreen.gif";
                    e.Row.Cells[0].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; margin: 2px";
                    e.Row.Cells[0].Title = e.Row.Cells[e.Row.Cells.Count - 1].Text;
                }
            }
        }

        











        


    }
}
