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

namespace Krista.FM.Server.Dashboards.reports.EO.EO_00071.EO_0006frame
{
    public partial class Default : CustomReportPage
    {
        private CustomParam region;
        private CustomParam lastDate;
        private CustomParam year;
        private CustomParam Descendants;
        private CustomParam current_way;
        private string lastYear;
        private string chart1_title = "способ размещения заказа «{0}», ед.";
        private string chart2_title = "способ размещения заказа «{0}», млн. р.";
        private string chart3_title = "Распределение количества закупок, % (ед.)";
        private string chart4_title = "Распределение стоимости закупок, % (млн. р.)";
        private string grid_title = "Распределение закупок по способам размещения заказа";
        private string page_title = "Статистика размещения заказа на {0} год, формируемая на основе данных, содержащихся в реестре государственных контрактов";
        private static String Descendant = "[Мониторинг__Дата принятия сведений].[Мониторинг__Дата принятия сведений].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}]";
        private Int32 screen_width { get { return (int)Session["width_size"]; } }
        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);
           
        }
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
             region = UserParams.CustomParam("region");
             lastDate = UserParams.CustomParam("lastDate");
             year = UserParams.CustomParam("year");
             Descendants = UserParams.CustomParam("Descendants");
             current_way = UserParams.CustomParam("current_way");
             RefreshPanel1.AddLinkedRequestTrigger(Grid);
             RefreshPanel1.AddRefreshTarget(Chart1);
             RefreshPanel1.AddRefreshTarget(Chart2);
             RefreshPanel1.AddRefreshTarget(Label8);
             RefreshPanel1.AddRefreshTarget(Label9);
             //RefreshPanel2.AddRefreshTarget(Chart3);
            // RefreshPanel2.AddRefreshTarget(Chart4);
             Grid.Width = 920;
             Grid.Height = 133;
             Parametr.Width = 200;
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
        Dictionary<string, int> ParametrLoad()
        {


            Dictionary<string, int> d = new Dictionary<string, int>();
            try
            {
                    d.Add("Прямой", 0);
                    d.Add("Нарастающий", 0);
            }
            catch { }

            return d;

        }

        private string GetLastDate(string query,string year)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(query));
            int i=cs.Axes[1].Positions.Count-1;
            int k = 0;
            while ((i >= 0) && !(cs.Axes[1].Positions[i].Members[0].ToString().Contains(year)))
            {
                i = i - 1;
            }
            
            return cs.Axes[1].Positions[i].Members[0].ToString();
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
            if (RegionSettingsHelper.Instance.GetPropertyValue("Descendants_mark") == "reverse")
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
                if (year.Value == lastYear)
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
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            region.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            if (!Page.IsPostBack)
            {
                YearComboBox.FillDictionaryValues(YearsLoad("years"));
                Parametr.FillDictionaryValues(ParametrLoad());
                Parametr.Title = "Параметр";
                YearComboBox.Title = "Год";
                year.Value = YearComboBox.SelectedValue;
                
                YearComboBox.SelectLastNode();
                lastYear = YearComboBox.SelectedValue;
            }
            year.Value = YearComboBox.SelectedValue;
            lastDate.Value = GetLastDate("lastdate", year.Value);
            Label2.Text = grid_title;
            Label1.Text = String.Format(page_title, year.Value);
            getDescendants();
            Grid.DataBind();

            
            current_way.Value = ".["+Grid.Rows[0].Cells[0].Text+"]";
            Label3.Text = "Динамика количества закупок";
            Label4.Text = "Динамика стоимости закупок";
            Label5.Text = "Структура показателя";
            Label6.Text = "Структура показателя";
            Label7.Text = chart3_title;
            Label8.Text = chart4_title;
            Label9.Text = String.Format(chart1_title, Grid.Rows[0].Cells[0].Text);
            Label10.Text = String.Format(chart2_title, Grid.Rows[0].Cells[0].Text);
            Chart1.DataBind();
            Chart2.DataBind();
            Chart3.DataBind();
            Chart4.DataBind();
            Grid.Rows[0].Selected=true;
            Grid.Rows[0].Activated=true;
            Grid.Rows[0].Activate();
            
        }

        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            
            try
            {
                Grid.Bands.Clear();
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid"), "Способ закупок", dt);
                if (Parametr.SelectedValue == "Прямой")
                { Grid.DataSource = dt; }
                else
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        dt1.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
                    }
                    object[] o = new object[dt.Columns.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        for (int k = 0; k < dt.Columns.Count; k++)
                        {
                            if ((dt.Rows[i].ItemArray[k].ToString() == ""))
                            { o[k] = 0; }
                            else
                            {
                                o[k] = dt.Rows[i].ItemArray[k];
                            }
                        }
                            for (int j = 3; j < dt.Columns.Count; j++)
                            {
                                
                                o[j] = double.Parse(o[j].ToString()) + double.Parse(o[j-2].ToString());
                            }
                        dt1.Rows.Add(o);
                    }
                    Grid.DataSource = dt1;
                }
                
            }
            catch
            { }
        }

        protected void ForCrossJoin(LayoutEventArgs e)
        {
            try
            {
                    e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.22);
                    for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                    {
                        e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                        e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.08);
                        e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
                    }
                    if (e.Layout.Bands[0].Columns.Count > 1)
                    {
                        ColumnHeader colHead;
                        for (int i = 0; i < e.Layout.Bands[0].HeaderLayout.Count; i++)
                        {
                            colHead = e.Layout.Bands[0].HeaderLayout[i] as ColumnHeader;
                            colHead.RowLayoutColumnInfo.OriginY = 1;

                        }
                        int dva = 2;

                        int c = e.Layout.Bands[0].HeaderLayout.Count;
                        try
                        {
                            for (int i = 1; i < c; i += dva)
                            {
                                ColumnHeader ch = new ColumnHeader(true);
                                ch.Caption = e.Layout.Bands[0].HeaderLayout[i].Caption.Split(';')[0];
                                try
                                {
                                    e.Layout.Bands[0].HeaderLayout[i].Caption = "Количество, ед.";

                                    e.Layout.Bands[0].HeaderLayout[i + 1].Caption = "Стоимость, млн. р.";
                                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "#.##");
                                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "### ### ### ##0.##");

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



                
            }
            catch { }
        
 
        }
        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            
            try
            {
                
                    e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.22);
                    for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                    {
                        e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                        e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.08);
                        e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
                    }
                    if (e.Layout.Bands[0].Columns.Count > 1)
                    {
                        ColumnHeader colHead;
                        for (int i = 0; i < e.Layout.Bands[0].HeaderLayout.Count; i++)
                        {
                            colHead = e.Layout.Bands[0].HeaderLayout[i] as ColumnHeader;
                            colHead.RowLayoutColumnInfo.OriginY = 1;

                        }
                        int dva = 2;

                        int c = e.Layout.Bands[0].HeaderLayout.Count;
                        try
                        {
                            for (int i = 1; i < c; i += dva)
                            {
                                ColumnHeader ch = new ColumnHeader(true);
                                ch.Caption = e.Layout.Bands[0].HeaderLayout[i].Caption.Split(';')[0];
                                try
                                {
                                    e.Layout.Bands[0].HeaderLayout[i].Caption = "Количество, ед.";

                                    e.Layout.Bands[0].HeaderLayout[i + 1].Caption = "Стоимость, млн. р.";
                                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "#.##");
                                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "### ### ### ##0.##");

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
                Chart1.DataBind();
                Chart2.DataBind();
                Label9.Text = String.Format(chart1_title, e.Row.Cells[0].Text);
                Label10.Text = String.Format(chart2_title, e.Row.Cells[0].Text);
            }
            catch { }
        }

        protected void Chart1_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart1"), "dff", dt);
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("chart1"));
                int cellCount = dt.Rows[0].ItemArray.Length;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    object[] tempArray = dt.Rows[i].ItemArray;
                    tempArray[0] = getLastBlock(cs.Axes[1].Positions[i].Members[0].ToString())
                        + "-й квартал " + getPrePreLastBlock(cs.Axes[1].Positions[i].Members[0].ToString())
                        + "г.";
                    string tempStr = tempArray[1].ToString();
                    if (tempStr == "") tempStr = "0";
                    tempArray[1] = tempStr;
                    dt.Rows[i].ItemArray = tempArray;
                }
                Chart1.DataSource = dt.DefaultView;
            }
            catch { }
        }

        protected void Chart2_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "dff", dt);
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("chart2"));
                int cellCount = dt.Rows[0].ItemArray.Length;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    object[] tempArray = dt.Rows[i].ItemArray;
                    tempArray[0] = getLastBlock(cs.Axes[1].Positions[i].Members[0].ToString())
                        + "-й квартал " + getPrePreLastBlock(cs.Axes[1].Positions[i].Members[0].ToString())
                        + "г.";
                    string tempStr = tempArray[1].ToString();
                    if (tempStr == "") tempStr = "0";
                    tempArray[1] = tempStr;
                    dt.Rows[i].ItemArray = tempArray;
                }
                Chart2.DataSource = dt;
            }
            catch { }
        }

        protected void Chart3_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart3"), "dff", dt);
                Chart3.DataSource = dt;
            }
            catch { }
        }

        protected void Chart4_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart4"), "dff", dt);
                Chart4.DataSource = dt;
            }
            catch { }
        }

        protected void Chart1_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }
    }
}
