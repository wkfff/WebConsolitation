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
using Infragistics.UltraChart.Core.Layers;
namespace Krista.FM.Server.Dashboards.reports.EO.EO_0007.EO_00060
{
    public partial class Default : CustomReportPage
    {
        private CustomParam region;
        private CustomParam lastDate;
        private CustomParam year;
        private CustomParam Descendants;
        private CustomParam Descendants2;
        private CustomParam current_way;
        private CustomParam chartDynMer;
        private CustomParam chartStrucMer;
        private CellSet Dates;
        private string lastYear;
        private string FormatString1 = "### ##0";
        private string FormatString2 = "";
        private string chart1_title = "Способ размещения заказа: «{0}»";
        private string chart2_title = "Способ размещения заказа: «{0}»";
        private string chart3_title = "Распределение количества закупок, % (ед.)";
        private string chart4_title = "Распределение стоимости закупок, % (млн. р.)";
        private string grid_title = "Распределение закупок по способам размещения заказа";
        private string page_title = "Статистика размещения заказа на {0} год, формируемая на основе данных, содержащихся в реестре государственных контрактов";
        private static String Descendant = "[Мониторинг__Дата принятия сведений].[Мониторинг__Дата принятия сведений].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}]";
        private Int32 screen_width { get { return (int)Session["width_size"]; } }
        double indexReport = 0;
        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);
           
        }
        string BN = "IE";
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            base.Page_PreLoad(sender, e);
             region = UserParams.CustomParam("region");
             lastDate = UserParams.CustomParam("lastDate");
             year = UserParams.CustomParam("year");
             Descendants = UserParams.CustomParam("Descendants");
             Descendants2 = UserParams.CustomParam("DescendantsGrid");
             current_way = UserParams.CustomParam("current_way");
             chartDynMer = UserParams.CustomParam("chartDynMer");
             chartStrucMer = UserParams.CustomParam("chartStrucMer");
             RefreshPanel1.AddLinkedRequestTrigger(Grid);
             RefreshPanel1.AddRefreshTarget(Chart1);
             RefreshPanel1.AddRefreshTarget(Chart2);
             RefreshPanel1.AddRefreshTarget(Chart5);
             RefreshPanel1.AddRefreshTarget(Label9);
            RefreshPanel1.AddRefreshTarget(Label5);
            
            string ReportStyle = RegionSettingsHelper.Instance.GetPropertyValue("ReportStyle");
            
            if (Culture == "Russian (Russia)")
            {
                indexReport = Double.Parse(ReportStyle)/100;
                FormatString2 = "### ##0.##";
            }
            else
            {
               // ReportStyle = ReportStyle.Replace(',','.');
                indexReport = Double.Parse(ReportStyle)/100;
                FormatString2 = "### ##0.##";
            }
            if (BN == "IE")
            {
                Chart1.Width = (int)((screen_width - 45) * indexReport * 0.5);
                Chart2.Width = (int)((screen_width - 45) * indexReport * 0.5);
                Chart3.Width = (int)((screen_width - 45) * indexReport * 0.5);
                Chart4.Width = (int)((screen_width - 45) * indexReport * 0.5);
                Label9.Width = (int)((screen_width - 45) * indexReport * 0.5);
                Label10.Width = (int)((screen_width - 45) * indexReport * 0.5);
                Grid.Width = (int)((screen_width - 45) * indexReport);
                Chart5.Width = (int)((screen_width - 45) * indexReport);
            }
            else
            {
                Chart1.Width = (int)((screen_width - 45) * indexReport * 0.49);
                Chart2.Width = (int)((screen_width - 45) * indexReport * 0.49);
                Chart3.Width = (int)((screen_width - 45) * indexReport * 0.49);
                Chart4.Width = (int)((screen_width - 45) * indexReport * 0.49);
                Label9.Width = (int)((screen_width - 45) * indexReport * 0.49);
                Label10.Width = (int)((screen_width - 45) * indexReport * 0.49);
                Grid.Width = (int)((screen_width - 45) * indexReport);
                Chart5.Width = (int)((screen_width - 45) * indexReport);
            }
            if (BN == "FIREFOX")
            {
                Grid.Height = 165;
            }
            else
            {
                Grid.Height = 150;
            }

        }

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
        Dictionary<string, int> YearsLoad(string sql)
        {
            string s = "";
            int index = 0;
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
                Dictionary<string, int> d = new Dictionary<string, int>();
                try
                {
                    for (int i = 0; i <= (cs.Axes[1].Positions.Count - 1); i++)
                    {

                        d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
                    }
                }
                catch { }
                return d;
            
        }
        

        private string GetLastDate(string query,string year)
        {
            Dates = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(query));
            int i=Dates.Axes[1].Positions.Count-1;
            int k = 0;
            while ((i >= 0) && !(Dates.Axes[1].Positions[i].Members[0].ToString().Contains(year)))
            {
                i = i - 1;
            }

            return Dates.Axes[1].Positions[i].Members[0].ToString();
        }

        public static String getLastBlock(String mdx_member)
        {
            if (mdx_member == null) return null;
            String[] list = mdx_member.Split('.');
            Int32 index = list.Length - 1;
            String total = list[index];
            total = total.Replace("[", "");
            total = total.Replace("]", "");
            return total.Substring(8);
        }
        public static String getPrePreLastBlock(String mdx_member)
        {
            if (mdx_member == null) return null;
            String[] list = mdx_member.Split('.');
            Int32 index = list.Length - 1;
            String total = list[index - 2];
            total = total.Replace("[", "");
            total = total.Replace("]", "");
            return total;
        }
        private void getDescendants()
        {
            if (RegionSettingsHelper.Instance.GetPropertyValue("Descendants_mark") == "notBeginnigOfYear")
            {
                switch (getLastBlock(lastDate.Value))
                {
                    case "1":
                        {
                            if (year.Value == "2009")
                            {
                                Descendants.Value = String.Format(Descendant, year.Value, "1", "1");
                            }
                            else
                            {
                                Descendants.Value = String.Format(Descendant, (int.Parse(year.Value) - 1).ToString(), "1", "2") + "," +
                                                    String.Format(Descendant, (int.Parse(year.Value) - 1).ToString(), "2", "3") + "," +
                                                    String.Format(Descendant, (int.Parse(year.Value) - 1).ToString(), "2", "4") + "," +
                                                    String.Format(Descendant, year.Value, "1", "1");
                            }
                            break;
                        }
                    case "2":
                        {
                            if (year.Value == "2009")
                            {
                                Descendants.Value = String.Format(Descendant, year.Value, "1", "1") + "," +
                                                    String.Format(Descendant, year.Value, "1", "2");
                            }
                            else
                            {
                                Descendants.Value = String.Format(Descendant, (int.Parse(year.Value) - 1).ToString(), "2", "3") + "," +
                                                    String.Format(Descendant, (int.Parse(year.Value) - 1).ToString(), "2", "4") + "," +
                                                    String.Format(Descendant, year.Value, "1", "1") + "," +
                                                    String.Format(Descendant, year.Value, "1", "2");
                            }
                            break;
                        }
                    case "3":
                        {
                            if (year.Value == "2009")
                            {
                                Descendants.Value = String.Format(Descendant, year.Value, "1", "1") + "," +
                                                    String.Format(Descendant, year.Value, "1", "2") + "," +
                                                    String.Format(Descendant, year.Value, "2", "3");
                            }
                            else
                            {
                                Descendants.Value = String.Format(Descendant, (int.Parse(year.Value) - 1).ToString(), "2", "4") + "," +
                                                    String.Format(Descendant, year.Value, "1", "1") + "," +
                                                    String.Format(Descendant, year.Value, "1", "2") + "," +
                                                    String.Format(Descendant, year.Value, "2", "3");
                            }
                            break;
                        }
                    case "4":
                        {
                            Descendants.Value = String.Format(Descendant, year.Value, "1", "1") + "," +
                                                String.Format(Descendant, year.Value, "1", "2") + "," +
                                                String.Format(Descendant, year.Value, "2", "3") + "," +
                                                String.Format(Descendant, year.Value, "2", "4");
                            break;
                        }
                }
            }
            else
            {
                if (YearComboBox.SelectedIndex!=YearComboBox.GetRootNodesCount())
                {
                    switch (getLastBlock(lastDate.Value))
                    {
                        case "1":
                            {

                                Descendants.Value = String.Format(Descendant, year.Value, "1", "1");

                                break;
                            }
                        case "2":
                            {


                                Descendants.Value = String.Format(Descendant, year.Value, "1", "1") + "," +
                                                    String.Format(Descendant, year.Value, "1", "2");

                                break;
                            }
                        case "3":
                            {


                                Descendants.Value = String.Format(Descendant, year.Value, "1", "1") + "," +
                                                    String.Format(Descendant, year.Value, "1", "2") + "," +
                                                    String.Format(Descendant, year.Value, "2", "3");

                                break;
                            }
                        case "4":
                            {
                                Descendants.Value = String.Format(Descendant, year.Value, "1", "1") + "," +
                                                    String.Format(Descendant, year.Value, "1", "2") + "," +
                                                    String.Format(Descendant, year.Value, "2", "3") + "," +
                                                    String.Format(Descendant, year.Value, "2", "4");
                                break;
                            }
                    }
                }
                else
                {
                      Descendants.Value = String.Format(Descendant, year.Value, "1", "1") + "," +
                      String.Format(Descendant, year.Value, "1", "2") + "," +
                      String.Format(Descendant, year.Value, "2", "3") + "," +
                      String.Format(Descendant, year.Value, "2", "4");
                }
            }
        }
        private void getDescendantsGrid()
        {
                switch (getLastBlock(lastDate.Value))
                {
                    case "1":
                        {

                            Descendants2.Value = String.Format(Descendant, int.Parse(year.Value)-1, "2", "4") + "," + 
                                                String.Format(Descendant, year.Value, "1", "1");

                            break;
                        }
                    case "2":
                        {


                            Descendants2.Value = String.Format(Descendant, int.Parse(year.Value) - 1, "2", "4") + "," + 
                                                String.Format(Descendant, year.Value, "1", "1") + "," +
                                                String.Format(Descendant, year.Value, "1", "2");

                            break;
                        }
                    case "3":
                        {


                            Descendants2.Value = String.Format(Descendant, int.Parse(year.Value) - 1, "2", "4") + "," + 
                                                String.Format(Descendant, year.Value, "1", "1") + "," +
                                                String.Format(Descendant, year.Value, "1", "2") + "," +
                                                String.Format(Descendant, year.Value, "2", "3");

                            break;
                        }
                    case "4":
                        {
                            Descendants2.Value = String.Format(Descendant, int.Parse(year.Value) - 1, "2", "4") + "," + 
                                                String.Format(Descendant, year.Value, "1", "1") + "," +
                                                String.Format(Descendant, year.Value, "1", "2") + "," +
                                                String.Format(Descendant, year.Value, "2", "3") + "," +
                                                String.Format(Descendant, year.Value, "2", "4");
                            break;
                        }
                }

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
            region.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            if (!Page.IsPostBack)
            {
                YearComboBox.FillDictionaryValues(YearsLoad("years"));
                YearComboBox.Title = "Год";
                
                YearComboBox.SelectLastNode();
                year.Value = YearComboBox.SelectedValue;
                lastYear = YearComboBox.SelectedValue;
            }
            year.Value = YearComboBox.SelectedValue;
            lastDate.Value = GetLastDate("lastdate", year.Value);
            Label2.Text = grid_title;
            Label1.Text = String.Format(page_title, year.Value);
            getDescendants();
            getDescendantsGrid();
            Grid.DataBind();
            Label5.Text = "Структура статей 94-ФЗ, регламентирующих размещение заказа у единственного поставщика (исполнителя, подрядчика)";
            current_way.Value = ".["+Grid.Rows[0].Cells[0].Text+"]";
            Label3.Text = "Динамика количества закупок, ед.";
            Label4.Text = "Динамика стоимости закупок, млн.р.";
            Label9.Text = String.Format(chart1_title, Grid.Rows[0].Cells[0].Text);
            Label10.Text = String.Format(chart2_title, Grid.Rows[0].Cells[0].Text);
            chartDynMer.Value = "[Measures].[Количество контрактов]";
            Chart1.DataBind();
            chartDynMer.Value = "[Measures].[Стоимость контракта] ";
            Chart2.DataBind();
            Chart5.DataBind();
            Grid.Rows[0].Selected=true;
            Grid.Rows[0].Activated=true;
            Grid.Rows[0].Activate();
            chartStrucMer.Value = "[Measures].[Количество контрактов]";
            Chart3.DataBind();
            chartStrucMer.Value = "[Measures].[Стоимость контракта] ";
            Chart4.DataBind();
            Label6.Text =chart3_title;
            Label8.Text = chart4_title;
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            Grid.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed;
            if ((Grid.Columns.Count >5))
            {
                Grid.Columns.RemoveAt(2);
                Grid.Columns.RemoveAt(1);
            }
            if ((RegionSettingsHelper.Instance.GetPropertyValue("ReportStyle") != "100") && (Grid.Columns.Count > 5))
            {
                int col = Grid.Columns.Count;
                for (int i = 5; i < col; i++)
                {
                    Grid.Columns.RemoveAt(col-i);
                }
            
            }
            Chart1.ColorModel.ModelStyle = ColorModels.CustomRandom;
            Chart2.ColorModel.ModelStyle = ColorModels.PureRandom;

        }

        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            
            try
            {
                Grid.Bands[0].HeaderLayout.Clear();
                Grid.Bands.Clear();
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid"), "Способ закупок", dt);
                dt1.Columns.Add(dt.Columns[0].ColumnName, dt.Columns[0].DataType);
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    dt1.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
                }
                dt1.Columns.Add("За год;Количество", dt.Columns[0].DataType);
                dt1.Columns.Add("За год;Стоимость", dt.Columns[1].DataType);
                object[] o = new object[dt1.Columns.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (dt.Rows[i].ItemArray[j].ToString() == "")
                        {
                            o[j] = 0;
                        }
                        else
                        {
                        o[j] = dt.Rows[i].ItemArray[j];
                        }
                    }
                    o[o.Length - 2] = 0;
                    o[o.Length-1]=0;

                    if ((dt1.Columns.Count > 5) && (dt1.Columns[1].ColumnName.Split(';')[0] == "Квартал 4"))
                    {
                        for (int k = 3; k < dt.Columns.Count; k++)
                        {
                            if (k % 2 == 0)
                            {
                                o[o.Length - 1] = double.Parse(o[o.Length - 1].ToString()) + double.Parse(o[k].ToString());
                            }
                            else
                            {
                                o[o.Length - 2] = double.Parse(o[o.Length - 2].ToString()) + double.Parse(o[k].ToString());
                            }

                        }
                    }
                    
                    if ((dt1.Columns.Count <= 5)||(dt1.Columns[1].ColumnName.Split(';')[0]!="Квартал 4"))
                    {
                        for (int k = 1; k < dt.Columns.Count; k++)
                        {
                            if (k % 2 == 0)
                            {
                                o[o.Length - 1] = double.Parse(o[o.Length - 1].ToString()) + double.Parse(o[k].ToString());
                            }
                            else
                            {
                                o[o.Length - 2] = double.Parse(o[o.Length - 2].ToString()) + double.Parse(o[k].ToString());
                            }

                        }
                    }
                 
                    for (int l = 2; l < dt1.Columns.Count; l += 2)
                    {
                        o[l] = double.Parse(o[l].ToString()) / 1000000;
                    }
                    dt1.Rows.Add(o);
                }
                
                Grid.DataSource = dt1;

              
                
            }
            catch
            { }
        }

      
        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            
            try
            {

                if (BN == "IE")
                {
                    e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.24);
                }
                else
                {
                    e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.22);
                }
                    e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                    for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                    {
                        e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                        e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.08);
                        e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
                        
                    }
                  int c = e.Layout.Bands[0].HeaderLayout.Count;
                    string s = e.Layout.Bands[0].HeaderLayout[e.Layout.Bands[0].HeaderLayout.Count-3].Caption.Split(';')[0];
                    if (e.Layout.Bands[0].Columns.Count > 1)
                    {
                        ColumnHeader colHead;
                        for (int i = 0; i < e.Layout.Bands[0].HeaderLayout.Count; i++)
                        {
                            colHead = e.Layout.Bands[0].HeaderLayout[i] as ColumnHeader;
                            colHead.RowLayoutColumnInfo.OriginY = 1;

                        }
                        
                        int dva = 2;

                      
                        try
                        {
                            for (int i = 1; i < c; i += dva)
                            {
                                ColumnHeader ch = new ColumnHeader(true);
                                if ((RegionSettingsHelper.Instance.GetPropertyValue("ReportStyle") == "100")||(i==c-2))
                                {
                                    ch.Caption = e.Layout.Bands[0].HeaderLayout[i].Caption.Split(';')[0];
                                }
                                else
                                {
                                    ch.Caption = s;
                                }
                                

                                    try
                                    {
                                        e.Layout.Bands[0].HeaderLayout[i].Caption = "Количество";
                                        e.Layout.Bands[0].HeaderLayout[i + 1].Caption = "Стоимость";
                                        CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], FormatString1);
                                        CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], FormatString2);
                                        e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                                        e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.068);
                                        e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
                                        e.Layout.Bands[0].Columns[i+1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                                        e.Layout.Bands[0].Columns[i+1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.0697);
                                        e.Layout.Bands[0].Columns[i+1].Header.Style.Wrap = true;
                                    }
                                    catch
                                    {
                                    }


                                    ch.RowLayoutColumnInfo.OriginX = i;
                                ch.RowLayoutColumnInfo.OriginY = 0;
                                ch.RowLayoutColumnInfo.SpanX = dva;
                                e.Layout.Bands[0].HeaderLayout.Add(ch);
                            }
                        }
                        catch
                        {
                        }
                    }
                    if ((RegionSettingsHelper.Instance.GetPropertyValue("ReportStyle") != "100")&&(e.Layout.Bands[0].Columns.Count>5))
                    {
                        e.Layout.Bands[0].HeaderLayout[e.Layout.Bands[0].Columns.Count+1].Caption = "За год";
                        for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                        {
                            e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        }
                        
                        ColumnHeader ch = new ColumnHeader(true);
                        ch.Caption = "Количество";
                        ch.RowLayoutColumnInfo.OriginX = 3;
                        ch.RowLayoutColumnInfo.SpanX = 1;
                        ch.RowLayoutColumnInfo.OriginY = 1;
                        e.Layout.Bands[0].HeaderLayout.Add(ch);
                        ColumnHeader ch1 = new ColumnHeader(true);
                        ch1.Caption = "Стоимость";
                        ch1.RowLayoutColumnInfo.OriginX = 4;
                        ch1.RowLayoutColumnInfo.SpanX = 1;
                        ch1.RowLayoutColumnInfo.OriginY = 1;
                        e.Layout.Bands[0].HeaderLayout.Add(ch1);

                        ColumnHeader ch2 = new ColumnHeader(true);
                        ch2.Caption = "Количество";
                        ch2.RowLayoutColumnInfo.OriginX = 1;
                        ch2.RowLayoutColumnInfo.SpanX = 1;
                        ch2.RowLayoutColumnInfo.OriginY = 1;
                        e.Layout.Bands[0].HeaderLayout.Add(ch2);
                        ColumnHeader ch3 = new ColumnHeader(true);
                        ch3.Caption = "Стоимость";
                        ch3.RowLayoutColumnInfo.OriginX = 2;
                        ch3.RowLayoutColumnInfo.SpanX = 1;
                        ch3.RowLayoutColumnInfo.OriginY = 1;
                        e.Layout.Bands[0].HeaderLayout.Add(ch3);

                    }
                    if ((RegionSettingsHelper.Instance.GetPropertyValue("ReportStyle") == "100") && (e.Layout.Bands[0].Columns.Count > 5))
                    {
                        for (int i = 0; i < (e.Layout.Bands[0].HeaderLayout.Count - e.Layout.Bands[0].Columns.Count-1); i++)
                        {
                            e.Layout.Bands[0].HeaderLayout[e.Layout.Bands[0].Columns.Count + i].Caption = e.Layout.Bands[0].HeaderLayout[e.Layout.Bands[0].Columns.Count + i + 1].Caption;
                        }
                        ColumnHeader ch = new ColumnHeader(true);
                        ch.Caption="Количество";
                        ch.RowLayoutColumnInfo.OriginX=1;
                        ch.RowLayoutColumnInfo.SpanX = 1;
                        ch.RowLayoutColumnInfo.OriginY = 1;
                        e.Layout.Bands[0].HeaderLayout.Add(ch);
                        ColumnHeader ch1 = new ColumnHeader(true);
                        ch1.Caption = "Стоимость";
                        ch1.RowLayoutColumnInfo.OriginX = 2;
                        ch1.RowLayoutColumnInfo.SpanX = 1;
                        ch1.RowLayoutColumnInfo.OriginY = 1;
                        e.Layout.Bands[0].HeaderLayout.Add(ch1);
                    }
            }
            catch { }
        }

        protected void Grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            try
            {
                if (e.Row.Cells[0].Text == "Все способы закупок")
                {
                    current_way.Value = "";
                }
                else
                {
                    current_way.Value = ".[" + e.Row.Cells[0].Text + "]";
                }
                chartDynMer.Value = "[Measures].[Количество контрактов]";
                Chart1.DataBind();
                chartDynMer.Value = "[Measures].[Стоимость контракта]";
                Chart2.DataBind();
                Label9.Text = String.Format(chart1_title, e.Row.Cells[0].Text);
                Label10.Text = String.Format(chart2_title, e.Row.Cells[0].Text);
                if (e.Row.Cells[0].Text == "единственный поставщик (исполнитель, подрядчик)")
                {
                    
                    Chart5.Visible = true;
                    Label5.Visible = true;
                    Panel1.Visible = true;
                }
                else
                {
                    Chart5.Visible = false;
                    Label5.Visible = false;
                    Panel1.Visible = false;
                }
            }
            catch { }
        }

        protected void Chart1_DataBinding(object sender, EventArgs e)
        {
                CompoChart(Chart1);
        }
        void CompoChart(Infragistics.WebUI.UltraWebChart.UltraChart C)
        {
            C.CompositeChart.ChartAreas.Clear();
            C.CompositeChart.ChartLayers.Clear();
            C.CompositeChart.Legends.Clear();
            C.CompositeChart.Series.Clear();
            C.ChartType = ChartType.Composite;
            ChartArea Area = new ChartArea();
            CompositeLegend lega = new CompositeLegend();
            
            lega.Bounds = new Rectangle(0, 93, 100, 7);
            lega.BoundsMeasureType = MeasureType.Percentage;
            lega.PE.ElementType = PaintElementType.CustomBrush;
            lega.PE.Fill = C.Legend.BackgroundColor;
            lega.LabelStyle.Font = C.Legend.Font;
            lega.LabelStyle.FontSizeBestFit = !(lega.LabelStyle.FontSizeBestFit);
            
            AxisItem Ax = new AxisItem();
            Ax.DataType = AxisDataType.String;
            Ax.SetLabelAxisType = SetLabelAxisType.ContinuousData;
            Ax.OrientationType = AxisNumber.X_Axis;
            Ax.Labels.Visible = 1 == 1;
            Ax.Labels.Orientation = TextOrientation.Horizontal;
            Ax.Labels.ItemFormatString = "<ITEM_LABEL>";
            Ax.Extent = 70;
            Ax.Margin.Far.Value = 5;
            Ax.Margin.Near.Value = 5;
            Ax.LineThickness = 1;
            Ax.Labels.Font = new Font("Verdana", 8);
            Ax.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            Ax.Labels.Layout.Padding = 5;
            Ax.Labels.HorizontalAlign = StringAlignment.Near;
            AxisItem Ay1 = new AxisItem();
            Ay1.Extent = 40;
            Ay1.DataType = AxisDataType.Numeric;
            if (chartDynMer.Value == "[Measures].[Количество контрактов]")
            {
                Ay1.Labels.ItemFormatString = "<DATA_VALUE:" + FormatString2+">";//### ##0.##>";
               
            }
            else
            {
                Ay1.Labels.ItemFormatString = "<DATA_VALUE:" + FormatString2+">";//### ##0.##>";
            }
            
            Ay1.Labels.Visible = 1 == 1;
            Ay1.Labels.HorizontalAlign = StringAlignment.Near;
            Ay1.Labels.Layout.Padding = 5;
            Ay1.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            Ay1.LineThickness = 1;
            Ay1.OrientationType = AxisNumber.Y_Axis;
            Ay1.Margin.Far.Value = 10;
            Ay1.Labels.Font = new Font("Verdana", 8);

            Area.Axes.Add(Ax);
            Area.Axes.Add(Ay1);

            NumericSeries seriesC = GetXYSeriesBound("chart1", 2);
            seriesC.Label = "За квартал";
            C.Legend.Font = new Font("Verdana", 8);
            C.CompositeChart.Series.Add(seriesC);
            ChartLayerAppearance ChartLayApp = new ChartLayerAppearance();
            SplineAreaChartAppearance SplineAreaChartApp = new SplineAreaChartAppearance();
            ChartTextAppearance ChartTextApp = new ChartTextAppearance(C, -2, -2, true, new Font("Verdana", 8), Color.Black, "<DATA_VALUE:### ##0.##>", StringAlignment.Far, StringAlignment.Center, 0);
            ChartTextApp.Column = -2;
            ChartTextApp.Row = -2;
            ChartTextApp.Visible = 1 == 1;
            ChartTextApp.VerticalAlign = StringAlignment.Far;
            SplineAreaChartApp.ChartText.Add(ChartTextApp);
            ChartLayApp.ChartTypeAppearance = SplineAreaChartApp;
            ChartLayApp.ChartType = ChartType.SplineAreaChart;
            ChartLayApp.ChartArea = Area;
            ChartLayApp.AxisX = Ax;
            ChartLayApp.AxisY = Ay1;
            ChartLayApp.Series.Add(seriesC);
            lega.ChartLayers.Add(ChartLayApp);
            C.CompositeChart.ChartLayers.Add(ChartLayApp);

            seriesC = GetXYSeriesBound("chart1", 1);
            if (RegionSettingsHelper.Instance.GetPropertyValue("Descendants_mark") == "BeginnigOfYear")
            {
                seriesC.Label = "С начала года";
            }
            else
            {
                seriesC.Label = "За последние четыре квартала";
            }
            
            seriesC.PEs.Add(new PaintElement(Color.Blue));
            C.CompositeChart.Series.Add(seriesC);

            ChartLayApp = new ChartLayerAppearance();
            LineChartAppearance LineChartApp = new LineChartAppearance();

            ChartTextApp = new ChartTextAppearance(C, -2, -2, true, new Font("Verdana", 8), Color.Black, "<DATA_VALUE:### ##0.##>", StringAlignment.Far, StringAlignment.Center, 0);
            ChartTextApp.Column = -2;
            ChartTextApp.Row = -2;
            ChartTextApp.Visible = 1 == 1;
            ChartTextApp.VerticalAlign = StringAlignment.Far;
            LineChartApp.ChartText.Clear();
            LineChartApp.ChartText.Add(ChartTextApp);
            LineChartApp.DrawStyle = LineDrawStyle.Solid;
            LineChartApp.Thickness = 2;
            
            ChartLayApp.ChartTypeAppearance = LineChartApp;

            ChartLayApp.ChartType = ChartType.LineChart;
            ChartLayApp.ChartArea = Area;


            ChartLayApp.AxisX = Ax;
            ChartLayApp.AxisY = Ay1;

            Ay1.RangeType = AxisRangeType.Automatic;
            Ay1.TickmarkStyle = AxisTickStyle.Smart;
            ChartLayApp.Series.Add(seriesC);

            C.CompositeChart.ChartLayers.Add(ChartLayApp);

            lega.ChartLayers.Add(ChartLayApp);

            C.CompositeChart.Legends.Add(lega);

            C.CompositeChart.ChartAreas.Add(Area);
            if (chartDynMer.Value == "[Measures].[Количество контрактов]")
            {
                C.Tooltips.FormatString = "<SERIES_LABEL> <DATA_VALUE:"+FormatString2+"> ед.";
            }
            else
            {
                C.Tooltips.FormatString = "<SERIES_LABEL> <DATA_VALUE:"+FormatString2+"> млн.р.";
            }
            C.Tooltips.Font.Size = 10;

            C.Border.Color = Color.Transparent;
            Area.Border.Color = Color.Transparent;
            //
            
            
        }
        DataTable tab = new DataTable();
        bool VisibleFlag = true;
        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "sadad");
           // CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("chart1"));
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dt1.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
            }
            dt1.Columns.Add("sd", dt.Columns[dt.Columns.Count - 1].DataType);
            
            object[] o = new object[dt.Columns.Count + 1];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i].ItemArray[0].ToString() == "")
                {
                    o[0] = 0;
                }
                else
                {
                    o[0] = dt.Rows[i].ItemArray[0];
                }

                if (dt.Rows[i].ItemArray[1].ToString() == "")
                {
                    o[2] = 0;
                }
                else
                {
                    if (chartDynMer.Value == "[Measures].[Количество контрактов]")
                    {
                        o[2] = dt.Rows[i].ItemArray[1];
                    }
                    else
                    {
                        o[2] =Double.Parse(dt.Rows[i].ItemArray[1].ToString())/1000000;
                    }
                }

                if (i != 0)
                {
                    o[1] = 0;
                    for (int k = 0; k <= i; k++)
                    {
                        if (dt.Rows[i - k].ItemArray[1].ToString() == "")
                        {
                            o[1] = double.Parse(o[1].ToString()) + 0;
                        }
                        else
                        {
                            o[1] = double.Parse(o[1].ToString()) + double.Parse(dt.Rows[i - k].ItemArray[1].ToString());
                        }

                    }
                    if (chartDynMer.Value != "[Measures].[Количество контрактов]")
                    {
                        o[1] = double.Parse(o[1].ToString()) / 1000000;
                    }

                }
                else
                {
                    if (dt.Rows[i].ItemArray[1].ToString() == "")
                    {
                        o[1] = 0;
                    }
                    else
                    {
                        if (chartDynMer.Value == "[Measures].[Количество контрактов]")
                        {
                            o[1] = dt.Rows[i].ItemArray[1];
                        }
                        else
                        {
                            o[1] =Double.Parse(dt.Rows[i].ItemArray[1].ToString())/1000000;
                        }
                    }
                }
                if (dt.Rows.Count == 1)
                {
                    o[0] = "";
                }
                dt1.Rows.Add(o);
               

            }
            
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("chart1"));
            if (dt1.Rows.Count == 1)
            {
                o[0] = "";
                dt1.Rows.Add(o);
                VisibleFlag = false;
            }
            else
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    object[] tempArray = dt1.Rows[i].ItemArray;
                    tempArray[0] = getLastBlock(cs.Axes[1].Positions[i].Members[0].ToString())
                        + "-й квартал\n     " + getPrePreLastBlock(cs.Axes[1].Positions[i].Members[0].ToString())
                        + "г.";
                    if (dt.Rows.Count == 1)
                    {
                        tempArray[0] = getLastBlock(cs.Axes[1].Positions[i].Members[0].ToString())
                            + "-й квартал\n      " + getPrePreLastBlock(cs.Axes[1].Positions[i].Members[0].ToString())
                            + "г.";
                    }
                    string tempStr = tempArray[1].ToString();
                    if (tempStr == "") tempStr = "0";
                    tempArray[1] = tempStr;
                    dt1.Rows[i].ItemArray = tempArray;
                }
            }
            return dt1;
        }
        private NumericSeries GetXYSeriesBound(string xz, int zx)
        {
            NumericSeries series = new NumericSeries();
            if (xz == "chart1")
            { series.Label = "<ITEM_VALUE> Сумма задолженности по выплате заработной платы, тысяча рублей"; }
            else
            {
                series.Label = "<ITEM_VALUE>, Кол-во граждан, перед которыми имеется задолженность, человек";
            }
           // DataTable table = GetData(xz, zx);
            tab = GetDSForChart(xz);

            Lmax = (System.Decimal)(tab.Rows[0].ItemArray[zx]);
            Lmin = Lmax-100;
            for (int i = 0; i < tab.Rows.Count; i++)
            {

                try
                {

                    series.Points.Add(new NumericDataPoint(Convert.ToDouble((System.Decimal)(tab.Rows[i].ItemArray[zx])), tab.Rows[i].ItemArray[0].ToString(), false));
                }
                catch
                {
                    series.Points.Add(new NumericDataPoint(Convert.ToDouble((System.Decimal)(0)), tab.Rows[i].ItemArray[0].ToString(), 1 == 1));
                    series.Points[series.Points.Count - 1].Empty = 1 == 1;
                    series.Points[series.Points.Count - 1].PE.ElementType = PaintElementType.None;
                }
            }

            return series;
        }

        System.Decimal Lmax = 0;
        System.Decimal Lmin = 0;
        private DataTable GetData(string xz, int zx)
        {
            Lmax = 0;
            Lmin = 0;
            DataTable table = new DataTable();
            table.Columns.Add("Label Column", typeof(string));
            table.Columns.Add("Value Column", typeof(double));
            tab= GetDSForChart(xz);
            
            //CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(xz));

            Lmax = (System.Decimal)(tab.Rows[0].ItemArray[zx]);
            Lmin = Lmax-100;
            object[] o = new object[2];
            for (int i = 0; i < tab.Rows.Count; i++)
            {
                o[0] = tab.Rows[i].ItemArray[0].ToString();
                o[1] = tab.Rows[i].ItemArray[zx].ToString();
                table.Rows.Add(o);
            }
            return table;
        }
        protected void Chart2_DataBinding(object sender, EventArgs e)
        {
            try
            {
              
                CompoChart(Chart2);
            }
            catch { }
        }

        protected void Chart1_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            try
            {
                if (e.Row.Cells.Count >= 7)
                {
                    double m = 0;
                    double m1 = 0;
                    if (RegionSettingsHelper.Instance.GetPropertyValue("ReportStyle") != "100")
                    {

                        m = double.Parse(e.Row.Cells[e.Row.Cells.Count - 4].ToString()) - double.Parse(e.Row.Cells[e.Row.Cells.Count - 6].ToString());
                        m1 = double.Parse(e.Row.Cells[e.Row.Cells.Count - 3].ToString()) - double.Parse(e.Row.Cells[e.Row.Cells.Count - 5].ToString());

                        if (m < 0)
                        {
                            e.Row.Cells[e.Row.Cells.Count - 4].Style.CssClass = "ArrowDownGreen";
                            m = -m;
                            e.Row.Cells[e.Row.Cells.Count - 4].Title = "Ниже на " + String.Format("{0:# ##0}", m)  + " ед. по сравнению с предыдущим кварталом";
                            m = -m;
                        }
                        if (m > 0)
                        {
                            e.Row.Cells[e.Row.Cells.Count - 4].Style.CssClass = "ArrowUpGreen";
                            e.Row.Cells[e.Row.Cells.Count - 4].Title = "Выше на " + String.Format("{0:# ##0}", m)+ " ед. по сравнению с предыдущим кварталом";
                        }
                        if (m1 < 0)
                        {
                            e.Row.Cells[e.Row.Cells.Count - 3].Style.CssClass = "ArrowDownGreen";
                            m1 = -m1;
                            e.Row.Cells[e.Row.Cells.Count - 3].Title = "Ниже на " + String.Format("{0:# ##0.00}", m1) + " млн.р. по сравнению с предыдущим кварталом";
                            m1 = -m1;
                        }
                        if (m1 > 0)
                        {
                            e.Row.Cells[e.Row.Cells.Count - 3].Style.CssClass = "ArrowUpGreen";
                            e.Row.Cells[e.Row.Cells.Count - 3].Title = "Выше на " + String.Format("{0:# ##0.00}", m1) + " млн.р. по сравнению с предыдущим кварталом";
                        }

                    }
                    else
                    {
                        for (int i = 3; i < Grid.Columns.Count - 2; i += 2)
                        {
                            m = double.Parse(e.Row.Cells[i].ToString()) - double.Parse(e.Row.Cells[i - 2].ToString());
                            m1 = double.Parse(e.Row.Cells[i + 1].ToString()) - double.Parse(e.Row.Cells[i + 1 - 2].ToString());

                            if (m < 0)
                            {
                                e.Row.Cells[i].Style.CssClass = "ArrowDownGreen";
                                m = -m;
                                e.Row.Cells[i].Title = "Ниже на " + String.Format("{0:# ##0}", m) + " ед. по сравнению с предыдущим кварталом";
                                m = -m;
                            }
                            if (m > 0)
                            {
                                e.Row.Cells[i].Style.CssClass = "ArrowUpGreen";
                                e.Row.Cells[i].Title = "Выше на " + String.Format("{0:# ##0}", m) + " ед. по сравнению с предыдущим кварталом";
                            }
                            if (m1 < 0)
                            {
                                e.Row.Cells[i + 1].Style.CssClass = "ArrowDownGreen";
                                m1 = -m1;
                                e.Row.Cells[i + 1].Title = "Ниже на " + String.Format("{0:# ##0.00}", m1) + " млн.р. по сравнению с предыдущим кварталом";
                                m1 = -m1;
                            }
                            if (m1 > 0)
                            {
                                e.Row.Cells[i + 1].Style.CssClass = "ArrowUpGreen";
                                e.Row.Cells[i + 1].Title = "Выше на " + String.Format("{0:# ##0.00}", m1) + " млн.р. по сравнению с предыдущим кварталом";
                            }
                        }
                    }
                }
            }
            catch
            { 
            
            }
        }

        protected void Chart5_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataTable dt1 =new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart5"), "dfd", dt);
            object[] tempArray = new object[2];
            dt1.Columns.Add(dt.Columns[0].ColumnName, dt.Columns[0].DataType);
            dt1.Columns.Add(dt.Columns[1].ColumnName, dt.Columns[1].DataType);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                tempArray[0] = dt.Rows[i].ItemArray[1].ToString() +" "+ dt.Rows[i].ItemArray[0].ToString();
                tempArray[1] = dt.Rows[i].ItemArray[1];
                dt1.Rows.Add(tempArray);
            }
                Chart5.DataSource = dt1.DefaultView;
        }
        DataTable dtChart3;
        protected void Chart3_DataBinding(object sender, EventArgs e)
        {
            try
            {

                dtChart3 = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart3"), "dff", dtChart3);
                
                    Chart3.DataSource = dtChart3;
            }
            catch { }
        }
        DataTable dt1Chart4;
        protected void Chart4_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                dt1Chart4 = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart3"), "dff", dt);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dt1Chart4.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
                }
                object[] o = new object[dt1Chart4.Columns.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    o[0] = dt.Rows[i].ItemArray[0];
                    if (dt.Rows[i].ItemArray[1].ToString() == "")
                    {
                        o[1] = 0;
                    }
                    else
                    {
                        o[1] = double.Parse(dt.Rows[i].ItemArray[1].ToString()) / 1000000;
                    }
                    dt1Chart4.Rows.Add(o);
                }
                Chart4.DataSource = dt1Chart4;
            }
            catch { }
        }

        protected void Chart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            Infragistics.UltraChart.Core.Primitives.Text decText = null;



            foreach (Infragistics.UltraChart.Core.Primitives.Primitive primitive in e.SceneGraph)
            {
                if (primitive is Infragistics.UltraChart.Core.Primitives.Text)
                {
                    Infragistics.UltraChart.Core.Primitives.Text text = primitive as Infragistics.UltraChart.Core.Primitives.Text;
                    string primitiveStr = text.GetTextString();
                    if (primitiveStr.Contains("квартал"))
                    {
                        decText = new Infragistics.UltraChart.Core.Primitives.Text();
                        decText.bounds = text.bounds;
                        decText.labelStyle = text.labelStyle;
                        decText.bounds.X = e.ChartCore.GridLayerBounds.Width + e.ChartCore.GridLayerBounds.X - decText.bounds.Width + 10;
                        decText.SetTextString(lastDate.Value + "-й квартал " + year.Value + "г.");
                        e.SceneGraph.Add(decText);
                        break;
                    }
                }
            }
        }

        protected void Chart2_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            if (VisibleFlag == false)
            {
                Infragistics.UltraChart.Core.Primitives.Text textLabel = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle((int)(250*indexReport+2), 200, 100, 100), "1-й квартал\n    " + year.Value + "г.", new LabelStyle(new Font("Verdana", 8), Color.Black, true, false, false, StringAlignment.Center, StringAlignment.Far, TextOrientation.Horizontal));
                e.SceneGraph.Add(textLabel);
            }
        }

        protected void Chart1_FillSceneGraph1(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            if (VisibleFlag == false)
            {
                Infragistics.UltraChart.Core.Primitives.Text textLabel = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle((int)(250 * indexReport+2), 200, 100, 100), "1-й квартал\n    " + year.Value + "г.", new LabelStyle(new Font("Verdana", 8), Color.Black, true, false, false, StringAlignment.Center, StringAlignment.Far, TextOrientation.Horizontal));
                e.SceneGraph.Add(textLabel);
            }
        }

        protected void Chart3_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < dtChart3.Rows.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Text textLabel = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(29, 218 + i * 20 - i, 320, 10), dtChart3.Rows[i].ItemArray[0].ToString(), new LabelStyle(new Font("Verdana", (float)(7.8)), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Near, TextOrientation.Horizontal));
                e.SceneGraph.Add(textLabel);
            }
        }

        protected void Chart4_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < dt1Chart4.Rows.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Text textLabel = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(29, 218 + i * 20 - i, 320, 10), dt1Chart4.Rows[i].ItemArray[0].ToString(), new LabelStyle(new Font("Verdana", (float)(7.8)), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Near, TextOrientation.Horizontal));
                e.SceneGraph.Add(textLabel);
            }
        }
    }
}
