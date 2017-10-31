using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Infragistics.WebUI.Misc;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;
using System.Collections.ObjectModel;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using Infragistics.UltraChart.Core;
using Dundas.Maps.WebControl;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
namespace Krista.FM.Server.Dashboards.reports.EO.EO_0004.EO_0004_LeninRegion1
{
    public partial class _default : CustomReportPage
    {
        string header = "Оценка качества жизни населения в {0} году";
        string chart2Header = "Динамика показателя";
        /*string chart3Header = "Динамика показателя";
        string chart4Header = "Динамика показателя";
        string chart5Header = "Динамика показателя";
        string chart6Header = "Динамика показателя";
        string chart7Header = "Динамика показателя";
        string chart8Header = "Динамика показателя";*/
        string tabel1Header = "Индекс демографической ситуации";
        string tabel2Header = "Индекс ситуации в образовании";
        string tabel3Header = "Индекс ситуации в здравоохранении";
        string tabel4Header = "Индекс социальной защиты населения";
        string tabel5Header = "Индекс ситуации в сфере культуры и отдыха";
        string tabel6Header = "Индекс жилищных условий населения";
        string tabel7Header = "Индекс экономической ситуации";
        string tabel8Header = "Интегральный показатель качества жизни";
        private CustomParam p1;
        private CustomParam p2;
        private CustomParam pok;//показатель
        private CustomParam chart2pok;
        private CustomParam chart2mer;
        private CustomParam grid_pok;
        private CellSet cs;
        private CellSet regionCellSet;
        private CellSet cs1;
        DataTable chart_master1 = new DataTable();
        string BN = "IE";
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            base.Page_PreLoad(sender, e);
          
            Label2.Text = chart2Header;
            Label4.Text = chart2Header;
            Label5.Text = chart2Header;
            Label6.Text = chart2Header;
            Label7.Text = chart2Header;
            Label8.Text = chart2Header;
            Label9.Text = chart2Header;
            Label3.Text = chart2Header;
            Table1Hed.Text = tabel1Header;
            Table2Hed.Text = tabel2Header;
            Table3Hed.Text = tabel3Header;
            Table4Hed.Text = tabel4Header;
            Table5Hed.Text = tabel5Header;
            Table6Hed.Text = tabel6Header;
            Table7Hed.Text = tabel7Header;
            Table8Hed.Text = tabel8Header;
            p1 = UserParams.CustomParam("region");
            p2 = UserParams.CustomParam("lastYear");
            pok = UserParams.CustomParam("pokazatel");
            chart2pok = UserParams.CustomParam("chart2pok");
            chart2mer = UserParams.CustomParam("chart2mer");
            grid_pok = UserParams.CustomParam("grid_pok");
            region.Title = "Территория";
            region.Width = 300;
            Chart2Panel.AddRefreshTarget(Chart2);
            Chart2Panel.AddLinkedRequestTrigger(Table1);
            Chart2Panel.AddRefreshTarget(Label10);
            Chart3Panel.AddLinkedRequestTrigger(Table2);
            Chart3Panel.AddRefreshTarget(Chart3);
            Chart3Panel.AddRefreshTarget(Label11);
            Chart4Panel.AddLinkedRequestTrigger(Table3);
            Chart4Panel.AddRefreshTarget(Chart4);
            Chart4Panel.AddRefreshTarget(Label12);
            Chart5Panel.AddLinkedRequestTrigger(Table4);
            Chart5Panel.AddRefreshTarget(Chart5);
            Chart5Panel.AddRefreshTarget(Label17);
            Chart6Panel.AddLinkedRequestTrigger(Table5);
            Chart6Panel.AddRefreshTarget(Chart6);
            Chart6Panel.AddRefreshTarget(Label13);
            Chart7Panel.AddLinkedRequestTrigger(Table6);
            Chart7Panel.AddRefreshTarget(Chart7);
            Chart7Panel.AddRefreshTarget(Label14);
            Chart8Panel.AddLinkedRequestTrigger(Table7);
            Chart8Panel.AddRefreshTarget(Chart8);
            Chart8Panel.AddRefreshTarget(Label15);

            Chart1Panel.AddLinkedRequestTrigger(Table8);
            Chart1Panel.AddRefreshTarget(Chart1);
            Chart1Panel.AddRefreshTarget(Label16);
            
            
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
           base.Page_Load(sender, e);
           try
           {
               if (!Page.IsPostBack)
               {
                   region.FillDictionaryValues(RegionsLoad("regions"));
                   p1.Value = getRegion();
                   p2.Value = get_last_year();
               }
               else
               {
                   p1.Value = getRegion();
                   p2.Value = get_last_year();
               }
               Label1.Text = String.Format(header, cs.Axes[1].Positions[0].Members[0].Caption);
               TableDataBind(Table1, "Демография");
               TableFormat(Table1);
               Chart2.DataBind();
               SetChartRange(Chart2);
               Label10.Text = "Демография";
               TableDataBind(Table2, "Образование");
               TableFormat(Table2);
               Chart3.DataBind();
               SetChartRange(Chart3);
               Label11.Text = "Образование";

               TableDataBind(Table3, "Здравоохранение");
               TableFormat(Table3);
               Chart4.DataBind();
               SetChartRange(Chart4);
               Label12.Text = "Здравоохранение";

               TableDataBind(Table4, "Социальная защита населения");
               TableFormat(Table4);
               Chart5.DataBind();
               SetChartRange(Chart5);
               Label17.Text = "Социальная защита населения";

               TableDataBind(Table5, "Культура и отдых");
               TableFormat(Table5);
               Chart6.DataBind();
               SetChartRange(Chart6);
               Label13.Text = "Культура и отдых";

               TableDataBind(Table6, "Жилищные условия");
               TableFormat(Table6);
               Chart7.DataBind();
               SetChartRange(Chart7);
               Label14.Text = "Жилищные условия";

               TableDataBind(Table7, "Экономика");
               TableFormat(Table7);
               Chart8.DataBind();
               SetChartRange(Chart8);
               Label15.Text = "Экономика";

               TableDataBind(Table8, "");
               TableFormat(Table8);
               Table8.Rows[0].Cells[0].Text = Table8.Rows[0].Cells[0].Text.Split(';')[0];
               Chart1.DataBind();
               SetChartRange(Chart1);
               for (int i = 2; i < Table8.Rows.Count; i++)
               {
                   Table8.Rows[i].Cells[0].Text = "   " + Table8.Rows[i].Cells[0].Text;
               }
               Label16.Text = "Интегральный показатель";
           }
           catch { }
        }


        private void TableFormat(UltraWebGrid Grid)
        {
            double m;
            for (int i = 0; i < Grid.Rows.Count; i++)
            {
               
                if (Grid.Rows[i].Cells[0].Text != "Показатель")
                {
                    m = double.Parse(Grid.Rows[i].Cells[4].Text) - double.Parse(Grid.Rows[i].Cells[1].Text);
                    if (m < 0)
                    {
                        if ((Grid.Rows[i].Cells[Grid.Rows[i].Cells.Count - 1].Text == "0")||(String.IsNullOrEmpty(Grid.Rows[i].Cells[Grid.Rows[i].Cells.Count - 1].Text)))
                        {
                            Grid.Rows[i].Cells[4].Style.CssClass = "ArrowDownRed";
                        }
                        else
                        {
                            Grid.Rows[i].Cells[4].Style.CssClass = "ArrowDownGreen";
                        }
                    }
                    if (m > 0)
                    {
                        if ((Grid.Rows[i].Cells[Grid.Rows[i].Cells.Count - 1].Text == "0")||(String.IsNullOrEmpty(Grid.Rows[i].Cells[Grid.Rows[i].Cells.Count - 1].Text)))
                        {
                            Grid.Rows[i].Cells[4].Style.CssClass = "ArrowUpGreen";
                        }
                        else
                        {
                            Grid.Rows[i].Cells[4].Style.CssClass = "ArrowUpRed";
                        }
                    }
                }
                else
                { }
            
            
            }
        
        }
        private void TableDataBind(UltraWebGrid Grid, String pokazatel)
        {
            try
            {
                if (pokazatel != "")
                {
                    pok.Value = ".[" + pokazatel + "]";
                    grid_pok.Value = "[Measures].[Значение]";
                }
                else
                {
                    pok.Value = pokazatel;
                    grid_pok.Value = "[Measures].[Оценка]";
                }
                DataTable dt1 = new DataTable();
                DataTable dt3 = new DataTable();
                DataSet tableDataSet = new DataSet();
                string query = DataProvider.GetQueryText("test1");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt1);
                Grid.DataSource = dt1;
                Grid.DataBind();
                Grid.Rows.Add();
                for (int i = 0; i < Grid.Columns.Count; i++)
                { 
                    Grid.Rows[Grid.Rows.Count - 1].Cells[i].Text = dt1.Rows[0].ItemArray[i].ToString();
                }

                
                if (pokazatel != "")
                {
                    Grid.Rows.Add();
                    Grid.Rows[Grid.Rows.Count - 1].Cells[0].Text = "Показатель";
                    Grid.Rows[Grid.Rows.Count - 1].Cells[4].Text = "Значение";
                    Grid.Rows[Grid.Rows.Count - 1].Cells[4].Style.HorizontalAlign = HorizontalAlign.Left;
                    Grid.Rows[Grid.Rows.Count - 1].Cells[1].Style.HorizontalAlign = HorizontalAlign.Left;
                    Grid.Rows[Grid.Rows.Count - 1].Cells[0].Style.BackColor = Color.LightGray;
                    Grid.Rows[Grid.Rows.Count - 1].Cells[4].Style.BackColor = Color.LightGray;
                    Grid.Rows[Grid.Rows.Count - 1].Cells[4].Style.Font.Bold = true;
                    Grid.Rows[Grid.Rows.Count - 1].Cells[0].Style.Font.Bold = true;
                }
               /* else
                {

                    Grid.Rows[Grid.Rows.Count - 1].Cells[4].Text = "Оценка";
                    Grid.Rows[Grid.Rows.Count - 1].Cells[4].Style.HorizontalAlign = HorizontalAlign.Left;
                }*/
                

                if (pokazatel != "")
                {
                    chart2pok.Value = ".[" + pokazatel + "]";
                }
                else
                {
                    chart2pok.Value = "";
                }
                chart2mer.Value = "[Measures].[Оценка]";
                Grid.Rows[1].Selected = true;
                Grid.Rows[1].Activate();
                
                DataTable dt2 = new DataTable();
                if (pokazatel != "")
                {
                    query = DataProvider.GetQueryText("test2");
                }
                else
                {
                    query = DataProvider.GetQueryText("test2Second");
                }
                
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt2);
                
                
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        Grid.Rows.Add();
                        if (pokazatel != "")
                        {
                            Grid.Rows[Grid.Rows.Count - 1].Cells[0].Text = dt2.Rows[i].ItemArray[0].ToString() + ","+"  " + dt2.Rows[i].ItemArray[2].ToString().ToLower();
                        }
                        else 
                        {
                            Grid.Rows[Grid.Rows.Count - 1].Cells[0].Text = dt2.Rows[i].ItemArray[0].ToString();
                        }
                        for (int j = 1; j < dt2.Columns.Count; j++)
                        {
                            Grid.Rows[Grid.Rows.Count - 1].Cells[j].Text = dt2.Rows[i].ItemArray[j].ToString();
                        }
                            //Grid.Rows[Grid.Rows.Count - 1].Cells[1].Text = dt2.Rows[i].ItemArray[1].ToString();
                    }
                    Grid.Columns[4].Header.Caption = Grid.Columns[4].Header.Caption.Split(';')[1];
                dt2.Clear();

                if ((BN == "IE") || (BN == "APPLEMAC-SAFARI"))
                {
                    Grid.Height = 335;
                    Grid.Width = 367;
                }
                else 
                {
                    Grid.Height = 355;
                    Grid.Width = 367;
                }

                dt1.Clear();
                Grid.Rows[0].Hidden=true;
                Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
                CRHelper.FormatNumberColumn(Grid.Columns[4], "### ### ##0.00");
            }
            catch { }
        
        }
        

        private String get_last_year()
        {
                cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("LastYear"));
                return cs.Axes[1].Positions[0].Members[0].ToString();
            
        }

        public String getRegion()
        {
            if (region.Visible == true)
            {
                CellSet regionCellSet = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("regions"));
                int k = region.SelectedIndex;
                return regionCellSet.Axes[1].Positions[k].Members[0].UniqueName;
            }
            else
            {
                return RegionSettingsHelper.Instance.RegionBaseDimension;
            }
        }

        Dictionary<string, int> RegionsLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            if (cs.Axes[1].Positions.Count <= 1)
            {
                region.Visible = false;
                RefreshButton.Visible = false;
            }
            for (int i = 0; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
            }

            return d;
        }



        protected void Chart2_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable chart_master2 = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "_", chart_master2);
                Chart2.DataSource = chart_master2;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public void SetChartRange(UltraChart chart)
        {
            DataTable data1=new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "fgf", data1);
            double max =double.Parse(data1.Rows[0].ItemArray[1].ToString());
            double min = double.Parse(data1.Rows[0].ItemArray[1].ToString());
            for (int i = 1; i < data1.Columns.Count; i++)
            {
                if (double.Parse(data1.Rows[0].ItemArray[i].ToString()) > max)
                {
                    max = double.Parse(data1.Rows[0].ItemArray[i].ToString());
                }
                if (double.Parse(data1.Rows[0].ItemArray[i].ToString()) < min)
                {
                    min = double.Parse(data1.Rows[0].ItemArray[i].ToString());
                }
            }
            if (chart2pok.Value.Contains("Миграционный"))
            { chart.Axis.Y.RangeType = AxisRangeType.Automatic;}
            else
            {             chart.Axis.Y.RangeType = AxisRangeType.Custom;
            chart.Axis.Y.RangeMax = max *1.1;
            chart.Axis.Y.RangeMin =min*0.95;
            //chart.Axis.Y.TickmarkStyle = AxisTickStyle.Smart;
           // chart.Data.ZeroAligned = true;
            chart.Axis.Y.TickmarkStyle = AxisTickStyle.Smart;
           // chart.Axis.Y.TickmarkInterval = (chart.Axis.Y.RangeMax - chart.Axis.Y.RangeMin) / 10;
            }

        }
        protected void SetErorFonn(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "В настоящий момент данные отсутствуют";
            e.LabelStyle.FontColor = System.Drawing.Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new System.Drawing.Font("Verdana", 30);
        }

        protected void Chart3_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable chart_master3 = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "_", chart_master3);
                Chart3.DataSource = chart_master3;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        protected void Chart4_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable chart_master4 = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "_", chart_master4);
                Chart4.DataSource = chart_master4;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        protected void Chart5_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable chart_master5 = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "_", chart_master5);
                Chart5.DataSource = chart_master5;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        protected void Chart6_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable chart_master6 = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "_", chart_master6);
                Chart6.DataSource = chart_master6;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        protected void Chart7_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable chart_master7 = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "_", chart_master7);
                Chart7.DataSource = chart_master7;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        protected void Chart8_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable chart_master8 = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "_", chart_master8);
                Chart8.DataSource = chart_master8;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        protected void Table1_DataBinding(object sender, EventArgs e)
        {        
        }

        protected void Table1_ActiveRowChange(object sender, RowEventArgs e)
        {
            try
            {
                if (e.Row.Cells[0].Text != "Показатель")
                {

                    if (e.Row.Index==1)
                    {
                        Label10.Text = "Демография";
                        chart2mer.Value = "[Measures].[Оценка]";
                        chart2pok.Value = ".[Демография]";
                        Chart2.DataBind();
                        SetChartRange(Chart2);
                    }
                    else
                    {
                        string s = e.Row.Cells[0].Text.Remove(e.Row.Cells[0].Text.LastIndexOf(','));
                        Label10.Text = e.Row.Cells[0].Text;
                        chart2mer.Value = "[Measures].[Значение]";
                        chart2pok.Value = ".[Демография]." + "[" + s + "]";
                        Chart2.DataBind();
                        SetChartRange(Chart2);
                    }
                }
                
                
            }
            catch { }
            
        }

        protected void Table2_ActiveRowChange(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Text != "Показатель")
            {
              

                if (e.Row.Index== 1)
                {
                    Label11.Text = "Образование";
                    chart2mer.Value = "[Measures].[Оценка]";
                    chart2pok.Value = ".[" + e.Row.Cells[0].Text + "]";
                    Chart3.DataBind();
                    SetChartRange(Chart3);
                }
                else
                {
                    string s = e.Row.Cells[0].Text.Remove(e.Row.Cells[0].Text.LastIndexOf(','));
                    Label11.Text = e.Row.Cells[0].Text;
                        chart2mer.Value = "[Measures].[Значение]";
                        chart2pok.Value = ".[Образование]." + "[" + s + "]";
                        Chart3.DataBind();
                        SetChartRange(Chart3);
                    
                }
            }
           
        }



        protected void Table3_ActiveRowChange(object sender, RowEventArgs e)
        {
            try
            {
                if (e.Row.Cells[0].Text != "Показатель")
                {

                    if (e.Row.Index==1)
                    {
                        Label12.Text = "Здравоохранение";
                        chart2mer.Value = "[Measures].[Оценка]";
                        chart2pok.Value = ".[" + e.Row.Cells[0].Text + "]";
                        Chart4.DataBind();
                        SetChartRange(Chart4);
                    }
                    else
                    {
                        string s = e.Row.Cells[0].Text.Remove(e.Row.Cells[0].Text.LastIndexOf(','));
                        Label12.Text = e.Row.Cells[0].Text;
                            chart2mer.Value = "[Measures].[Значение]";
                            chart2pok.Value = ".[Здравоохранение]." + "[" + s + "]";
                            Chart4.DataBind();
                            SetChartRange(Chart4);
                        
                    }
                }
            }
            catch
            { }
            
        }


        protected void Table4_ActiveRowChange(object sender, RowEventArgs e)
        {
            try
            {
                if (e.Row.Cells[0].Text != "Показатель")
                {
                    
                    if (e.Row.Index==1)
                    {
                        Label17.Text = "Социальная защита населения";
                        chart2mer.Value = "[Measures].[Оценка]";
                        chart2pok.Value = ".[" + e.Row.Cells[0].Text + "]";
                        Chart5.DataBind();
                        SetChartRange(Chart5);
                    }
                    else
                    {
                        string s = e.Row.Cells[0].Text.Remove(e.Row.Cells[0].Text.LastIndexOf(','));
                        Label17.Text = e.Row.Cells[0].Text;
                            chart2mer.Value = "[Measures].[Значение]";
                            chart2pok.Value = ".[Социальная защита населения]." + "[" + s + "]";
                            Chart5.DataBind();
                            SetChartRange(Chart5);
                        
                    }
                }
            }
            catch
            { }
            
        }





        protected void Table5_ActiveRowChange(object sender, RowEventArgs e)
        {
            try
            {
                if (e.Row.Cells[0].Text != "Показатель")
                {
                    
                    if (e.Row.Index==1)
                    {
                        Label13.Text = "Культура и отдых";
                        chart2mer.Value = "[Measures].[Оценка]";
                        chart2pok.Value = ".[" + e.Row.Cells[0].Text + "]";
                        Chart6.DataBind();
                        SetChartRange(Chart6);
                    }
                    else
                    {
                        string s = e.Row.Cells[0].Text.Remove(e.Row.Cells[0].Text.LastIndexOf(','));
                        Label13.Text = e.Row.Cells[0].Text;
                            chart2mer.Value = "[Measures].[Значение]";
                            chart2pok.Value = ".[Культура и отдых]." + "[" + s + "]";
                            Chart6.DataBind();
                            SetChartRange(Chart6);
                        
                    }
                }
            }
            catch
            { }
            
        }



        protected void Table6_ActiveRowChange(object sender, RowEventArgs e)
        {
            try
            {
                if (e.Row.Cells[0].Text != "Показатель")
                {

                    if (e.Row.Index==1)
                    {
                        Label14.Text = "Жилищные условия";
                        chart2mer.Value = "[Measures].[Оценка]";
                        chart2pok.Value = ".[" + e.Row.Cells[0].Text + "]";
                        Chart7.DataBind();
                        SetChartRange(Chart7);
                    }
                    else
                    {
                        string s = e.Row.Cells[0].Text.Remove(e.Row.Cells[0].Text.LastIndexOf(','));
                        Label14.Text = e.Row.Cells[0].Text;
                            chart2mer.Value = "[Measures].[Значение]";
                            chart2pok.Value = ".[Жилищные условия]." + "[" + s + "]";
                            Chart7.DataBind();
                            SetChartRange(Chart7);
                        
                    }
                }
            }
            catch
            { }
            
        }

        protected void Table7_ActiveRowChange(object sender, RowEventArgs e)
        {
            try
            {
                if (e.Row.Cells[0].Text != "Показатель")
                {
                  
                    if (e.Row.Index==1)
                    {
                        Label15.Text = "Экономика";
                        chart2mer.Value = "[Measures].[Оценка]";
                        chart2pok.Value = ".[" + e.Row.Cells[0].Text + "]";
                        Chart8.DataBind();
                        SetChartRange(Chart8);
                    }
                    else
                    {
                        string s = e.Row.Cells[0].Text.Remove(e.Row.Cells[0].Text.LastIndexOf(','));
                            Label15.Text = e.Row.Cells[0].Text;
                            chart2mer.Value = "[Measures].[Значение]";
                            chart2pok.Value = ".[Экономика]." + "[" + s + "]";

                            Chart8.DataBind();
                            SetChartRange(Chart8);

                        
                    }
                }
            }
            catch
            { }
            
        }


        protected void Table1_InitializeLayout(object sender, LayoutEventArgs e)
        {

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "### ### ##0.00");
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[2].Hidden = true;
            e.Layout.Bands[0].Columns[1].Hidden = true;
            e.Layout.Bands[0].Columns[2].Hidden = true;
            e.Layout.Bands[0].Columns[3].Hidden = true;
            e.Layout.Bands[0].Columns[5].Hidden = true;
            e.Layout.Bands[0].Columns[6].Hidden = true;
            
            if (BN == "IE")
            {

                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.171);
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.03);
               
                
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.175);
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.05);
                
            }
            if (BN == "FIREFOX")
            {
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.162);
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.05);
               
            }
            
            


        }

        protected void Chart1_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable chart_master1 = new DataTable();
                
                    DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "_", chart_master1);
                

                Chart1.DataSource = chart_master1;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        protected void Table8_ActiveRowChange(object sender, RowEventArgs e)
        {
            try
            {
                if (e.Row.Cells[0].Text != "Показатель")
                {
                
                    if (e.Row.Index == 1)
                    {
                        Label16.Text = "Интегральный показатель";
                        chart2mer.Value = "[Measures].[Оценка]";
                        chart2pok.Value = " ";
                        Chart1.DataBind();
                        SetChartRange(Chart1);
                        
                    }
                    else
                    {
                        chart2mer.Value = "[Measures].[Оценка]";
                        chart2pok.Value = ".[" + e.Row.Cells[0].Text.Remove(0,3) + "]";
                        Chart1.DataBind();
                        SetChartRange(Chart1);
                        Label16.Text = e.Row.Cells[0].Text.Remove(0,3);
                    }
                }
            }
            catch
            {}
            
        }



        protected void Table8_InitializeRow(object sender, RowEventArgs e)
        {


        }

        protected void Table1_InitializeRow(object sender, RowEventArgs e)
        {
 
           
        }








    }
}
