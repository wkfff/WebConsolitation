using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.UltraWebGrid;
using System.Drawing;
using Microsoft.AnalysisServices.AdomdClient;


using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;

using Infragistics.UltraChart.Core.Layers;

using Infragistics.UltraChart.Core;

namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._016
{
    public partial class Default : CustomReportPage
    {
        protected DataTable maintable = new DataTable();
        private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
        private CustomParam Pokaz { get { return (UserParams.CustomParam("Pokaz")); } }
        private CustomParam Lastdate { get { return (UserParams.CustomParam("lastdate")); } }
        private Int32 screen_width { get { return (int)Session["width_size"]; } }
        private CustomParam grid1Marks;
        private CustomParam grid2Marks;
        private CustomParam grid3Marks;
        private CustomParam chart1Marks;
        private CustomParam chart2Marks;
        private CustomParam chart3Marks;
        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }

        #region //получение низшей иерархии
        private String ELV(String s)
        {
            int i = s.Length;
            string res = "";
            while (s[--i] != ']') ;
            while (s[--i] != '[')
            {
                res = s[i] + res;
            }
            return res;

        }
        #endregion

        private string _GetString_(string s)
        {
            string res = "";
            int i = 0;
            for (i = s.Length - 1; s[i] != ','; i--) { res = s[i] + res; };

            return res;


        }
        static public class ForMarks
        {

            public static ArrayList Getmarks(string prefix)
            {
                ArrayList AL = new ArrayList();
                string CurMarks = RegionSettingsHelper.Instance.GetPropertyValue(prefix + "1");
                int i = 2;
                while (!string.IsNullOrEmpty(CurMarks))
                {
                    AL.Add(CurMarks.ToString());

                    CurMarks = RegionSettingsHelper.Instance.GetPropertyValue(prefix + i.ToString());

                    i++;
                }

                return AL;
            }

            public static CustomParam SetMarks(CustomParam param, ArrayList AL, params bool[] clearParam)
            {
                if (clearParam.Length > 0 && clearParam[0]) { param.Value = ""; }
                int i;
                for (i = 0; i < AL.Count - 1; i++)
                {
                    param.Value += AL[i].ToString() + ",";
                }
                param.Value += AL[i].ToString();

                return param;
            }


        }
        protected void FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            int xOct = 0;
            int xNov = 0;
            Text decText = null;
            int year = int.Parse(UserComboBox.getLastBlock(Lastdate.Value));
            String year1 = (year - 1).ToString();
            String year2 = (year - 2).ToString();


            foreach (Primitive primitive in e.SceneGraph)
            {
                if (primitive is Text)
                {
                    Text text = primitive as Text;

                    if (year2 == text.GetTextString())
                    {
                        xOct = text.bounds.X;
                        continue;
                    }
                    if (year1 == text.GetTextString())
                    {
                        xNov = text.bounds.X;
                        decText = new Text();
                        decText.bounds = text.bounds;
                        decText.labelStyle = text.labelStyle;
                        continue;
                    }
                }
                if (decText != null)
                {
                    decText.bounds.X = e.ChartCore.GridLayerBounds.Width + e.ChartCore.GridLayerBounds.X - decText.bounds.Width + 10;
                    decText.SetTextString(year.ToString());
                    e.SceneGraph.Add(decText);
                    break;
                }
            }
            //Text decText = null;


        }



        private void GridActiveRow(UltraWebGrid Grid,int index, bool active)
        {
            try
            {
                if (index < 0)
                {
                    index = Grid.Rows.Count - 1;
                }
                // получаем выбранную строку
                UltraGridRow row = Grid.Rows[index];
                // устанавливаем ее активной, если необходимо
                if (active)
                {
                    row.Activate();
                    row.Activated = true;
                    row.Selected = true;
                }
                // получение заголовка выбранной отрасли
                //selected_year.Value = row.Cells[0].Value.ToString();
                //UltraChart1.DataBind();
                //chart1_caption.Text = String.Format(chart1_title_caption, row.Cells[0].Value.ToString());
            }
            catch (Exception)
            {
                // выбор сторки пошел с ошибкой ...
            }
        }




        public static void setChartErrorFont(Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "В настоящий момент данные отсутствуют";

            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new Font("Verdana", 30);
            //e.LabelStyle.Font.Size = 15;
        }
        private void setFont(int typ, Label lab)
        {
            lab.Font.Name = "arial";
            lab.Font.Size = typ;
            if (typ == 14) { lab.Font.Bold = 1 == 1; };
            if (typ == 16) { lab.Font.Bold = 1 == 1; };
            if (typ == 10) { lab.Font.Bold = 1 == 1; };
            if (typ == 18) { lab.Font.Bold = 1 == 1; };
        }
        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(s, "Показатель", dt);
            return dt;
        }
        private String getLastDate(String way_ly)
        {
            try
            {
                way_last_year.Value = way_ly;
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("last_date"));
                return cs.Axes[1].Positions[0].Members[0].ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
        private void conf_Grid(int sizePercent,  UltraWebGrid grid)
        {
            //if (BN == "FIREFOX")
            //{

            //    //e.Layout.Bands[0].Columns[0].Width = (int)(GW * 0.58);
            //    //e.Layout.Bands[0].Columns[1].Width = (int)(GW * 0.21);
            //    if (sizePercent != 0) { grid.Width = screen_width * sizePercent / 100 - 15; };
            //}else
            if (sizePercent != 0) { grid.Width = screen_width * sizePercent / 100; };
            //grid.Columns[2].CellStyle.Wrap = 1 == 1;
            grid.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            for (int i = 1; i < grid.Columns.Count; i++) 
            {
                grid.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                grid.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(grid.Bands[0].Columns[i], "### ### ### ##0.00");
            }
            grid.DisplayLayout.CellClickActionDefault = CellClickAction.RowSelect;
            grid.DisplayLayout.HeaderTitleModeDefault = CellTitleMode.Always;
            grid.DisplayLayout.RowSelectorsDefault = RowSelectors.Yes;
            grid.DisplayLayout.GroupByBox.Hidden = 1 == 1;
            grid.DisplayLayout.NoDataMessage = "Нет данных";
            grid.DisplayLayout.NoDataMessage = "Нет данных";
        }
        private void conf_Chart(int sizePercent,  Infragistics.WebUI.UltraWebChart.UltraChart chart,bool leg)
        {
            if (sizePercent != 0) { chart.Width = screen_width * sizePercent / 100; };
            chart.SplineAreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8), Color.Black, "<DATA_VALUE:#>", StringAlignment.Far, StringAlignment.Center, 0));
            if (leg) 
            { 
                chart.Legend.Visible = 1 == 1;
                chart.Legend.Location = Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom;
                chart.Legend.SpanPercentage = 30;
                
            }
            chart.FunnelChart3D.RadiusMax = 0.5;
            chart.FunnelChart3D.RadiusMin = 0.1;
            chart.FunnelChart3D.OthersCategoryText = "Прочие";
        
        }
        private string GetString_(string s)
        {
            string res = "";
            int i = 0;
            for (i = s.Length-1; s[i] != ','; i--) ;
            for (int j = 0;j<i;j++)
            {
                res +=s[j];
            }
            return res;


        }







        String BN = "";




        protected override void Page_Load(object sender, EventArgs e)
        {
            //try
            {
                String.Format(Pokaz.Value);
                System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
                BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();


                if (!Page.IsPostBack)
                {
                    RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                    grid1Marks = UserParams.CustomParam("grid1Marks");
                    grid1Marks = ForMarks.SetMarks(grid1Marks, ForMarks.Getmarks("grid1_mark_"), true);
                    grid2Marks = UserParams.CustomParam("grid2Marks");
                    grid2Marks = ForMarks.SetMarks(grid2Marks, ForMarks.Getmarks("grid2_mark_"), true);
                    grid3Marks = UserParams.CustomParam("grid3Marks");
                    grid3Marks = ForMarks.SetMarks(grid3Marks, ForMarks.Getmarks("grid3_mark_"), true);
                    chart1Marks = UserParams.CustomParam("chart1Marks");
                    chart1Marks=ForMarks.SetMarks(chart1Marks, ForMarks.Getmarks("chart1_mark_"), true);
                    chart2Marks = UserParams.CustomParam("chart2Marks");
                    chart2Marks=ForMarks.SetMarks(chart2Marks, ForMarks.Getmarks("chart2_mark_"), true);
                    chart3Marks = UserParams.CustomParam("chart3Marks");
                    chart3Marks=ForMarks.SetMarks(chart3Marks, ForMarks.Getmarks("chart3_mark_"), true);
                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(TopGrid);
                    WebAsyncRefreshPanel2.AddLinkedRequestTrigger(CenterGrid);
                    WebAsyncRefreshPanel4.AddLinkedRequestTrigger(BottomGrid);
                    
                    
                    

                    Lastdate.Value = ELV(getLastDate(RegionSettingsHelper.Instance.GetPropertyValue("way_last_year_mark")));
                    TopGrid.DataBind();
                    conf_Grid(35, TopGrid);
                    CellSet CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("GTE"));
                    for (int i = 0; i < CLS.Cells.Count; i++)
                    {
                        TopGrid.Rows[i].Cells[0].Text += ", " + CLS.Cells[i].Value.ToString().ToLower();

                    }

                    Pokaz.Value = GetString_(TopGrid.Rows[0].Cells[0].Text);
                    ed = (TopGrid.Rows[0].Cells[0].Text);
                    TopChart.DataBind();
                    ChartTopText.Text = "Динамика показателя " + '"' + GetString_(ed) + '"' + ", " + _GetString_(ed);

                    
                    GridActiveRow(TopGrid, 0, true);
                    //ChartTopText.Width = TopChart.Width;
                    //BottomChart.Tooltips.FormatString = GetString_(BottomGrid.Rows[0].Cells[0].Text) + ", " + _GetString_(BottomGrid.Rows[0].Cells[0].Text) + " <b><DATA_VALUE:00.##></b>";
                   

                    CenterGrid.DataBind();
                    conf_Grid(35, CenterGrid);
                    CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("GCE"));
                    for (int i = 0; i < CLS.Cells.Count; i++)
                    {
                        CenterGrid.Columns[i + 1].Header.Caption += ", " + CLS.Cells[i].Value.ToString().ToLower();

                    }
                    Pokaz.Value = CenterGrid.Rows[CenterGrid.Rows.Count-1].Cells[0].Text;
                    GridActiveRow(CenterGrid, -1, true);
                    CenteRightChart.DataBind();
                    CenterLeftChart.DataBind();

                    BottomGrid.DataBind();
                    conf_Grid(95, BottomGrid);
                    CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("GBE"));
                    for (int i = 0; i < CLS.Cells.Count; i++)
                    {
                        BottomGrid.Rows[i].Cells[0].Text += ", " + CLS.Cells[i].Value.ToString().ToLower();

                    }

                    Pokaz.Value = GetString_(BottomGrid.Rows[0].Cells[0].Text);
                    BottomChart.DataBind();
                    ChartTopText.Text = "Динамика показателя " + '"' + GetString_(TopGrid.Rows[0].Cells[0].Text) + '"' + ", " + _GetString_(TopGrid.Rows[0].Cells[0].Text);


                    ChartCenterLeftText.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart2_title"), CenterGrid.Rows[CenterGrid.Rows.Count-1].Cells[0].Text);
                    ChartCenterRightText.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart3_title"), CenterGrid.Rows[CenterGrid.Rows.Count - 1].Cells[0].Text);
                    //ChartBottomText.Text = //String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart4_title"), BottomGrid.Rows[0].Cells[0].Text);
                    ChartBottomText.Text = "Динамика показателей "+'"' + GetString_(BottomGrid.Rows[0].Cells[0].Text) + '"' + ", " + _GetString_(BottomGrid.Rows[0].Cells[0].Text);
                    
                    //Label2.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart4_title1");
                    //TopGrid.Height = TopChart.Height;
                   // CenterGrid.Height = CenterLeftChart.Height = CenteRightChart.Height;



                    GridTopText.Text = RegionSettingsHelper.Instance.GetPropertyValue("grid1_title");
                    GridCenterText.Text = RegionSettingsHelper.Instance.GetPropertyValue("grid2_text");
                    GridBottomText.Text = RegionSettingsHelper.Instance.GetPropertyValue("grid3_text");
                    //conf_Chart(28, CenteRightChart, false);
                    //conf_Chart(28, CenterLeftChart, false);
                    //ChartBottomText.Width = screen_width * 60 / 100;
                    //ChartCenterLeftText.Width = screen_width * 22 / 100;
                    //ChartCenterRightText.Width = screen_width * 22 / 100;
                   // ChartCenterLeftText.Height = ChartCenterRightText.Height = 20;
                    //GridCenterText.Height = 23;
                    
                    int s = 10;
                    //setFont(s, ChartTopText);
                    //setFont(s, GridBottomText);
                    //setFont(s, GridCenterText);
                    //setFont(s, GridTopText);
                    //setFont(s, ChartBottomText);
                    //setFont(s, ChartTopText);
                    //setFont(s, ChartCenterLeftText);
                    //setFont(s, ChartCenterRightText);
                    Label1.Text = RegionSettingsHelper.Instance.GetPropertyValue("page_title"); ;
                    //setFont(16, Label1);

                    int iz = (int)BottomGrid.Width.Value;
                    //BottomChart.Width = iz ;
                    


                    

                    
                    //ed= TopGrid.Rows[0].Cells[0].Text;
                    //Pokaz.Value = GetString_(ed);
                    
                    //TopChart.DataBind();
                    
                    if (BN == "FIREFOX")
                    {
                        TopGrid.Height = 306;
                        CenterGrid.Height = 315;
                    }
                    if (BN=="IE")
                    { 
                        TopGrid.Height=286;
                        CenterGrid.Height=269;
                    }
                    if (BN == "APPLEMAC-SAFARI")
                    {
                        TopGrid.Height = 286;
                        CenterGrid.Height = 290-3;
                    }
                }


                if (!Page.IsPostBack)
                {
                    GridActiveRow(BottomGrid, 0, true);
                }
            }
            TopChart.Legend.SpanPercentage = 25;
            //catch
            {}
        }

        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {

        }

        protected void TopGrid_DataBinding(object sender, EventArgs e)
        {
            TopGrid.DataSource = GetDSForChart("GridTop");
            
        }

        protected void CenterGrid_DataBinding(object sender, EventArgs e)
        {
            
            CenterGrid.DataSource = GetDSForChart("GridCenter");
        }


        protected void BottomGrid_DataBinding(object sender, EventArgs e)
        {
            BottomGrid.DataSource = GetDSForChart("GridBottom");
        }

        double[] ValuNoEmpty;
        double[] IndexNoEmpty;
        DataTable dtTopChart;
        protected void TopChart_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDSForChart("ChartTop");
            double max = double.Parse(dt.Rows[0].ItemArray[1].ToString()),
            min = double.Parse(dt.Rows[0].ItemArray[1].ToString());
            try
            {
                for (int j = 0; j < dt.Rows.Count; j++)
                    for (int i = 2; i < dt.Rows[j].ItemArray.Length; i++)
                    {
                        try
                        {
                            if (double.Parse(dt.Rows[j].ItemArray[i].ToString()) > max) { max = double.Parse(dt.Rows[j].ItemArray[i].ToString()); }
                            if (double.Parse(dt.Rows[j].ItemArray[i].ToString()) < min) { min = double.Parse(dt.Rows[j].ItemArray[i].ToString()); }
                        }
                        catch { }
                    }
            }
            catch { }
            TopChart.Axis.Y.RangeMax = max * 1.2;
            TopChart.Axis.Y.RangeMin = min * 0.8;
            TopChart.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;

            dtTopChart = dt;
            TopChart.DataSource = dt;
            if (BN == "IE")
            {
                conf_Chart(61, TopChart, true);
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                conf_Chart(62, TopChart, true);
            }
            if (BN == "FIREFOX")
            {
                conf_Chart(61, TopChart, true);
            }
            TopChart.Tooltips.FormatString = GetString_(ed)+", " +_GetString_(ed)+ " <b><DATA_VALUE:00.##></b>"; 
        }

        protected void CenterLeftChart_DataBinding(object sender, EventArgs e)
        {
            CenterLeftChart.DataSource = GetDSForChart("ChartLeftCenter");
            if (BN == "IE")
            {
                conf_Chart(30, CenterLeftChart, false);
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                conf_Chart(30, CenterLeftChart, false);
            }
            if (BN == "FIREFOX")
            {
                conf_Chart(30, CenterLeftChart, false);
            } 
            //CenterLeftChart.Tooltips.FormatString = CenterGrid.Columns[1].Header.Caption + " <b><DATA_VALUE:00.##></b>"; 
            CenterLeftChart.Tooltips.FormatString = "<ITEM_LABEL>, "+ _GetString_(CenterGrid.Columns[1].Header.Caption) + " <b><DATA_VALUE:00.##></b>"; 
        }

        protected void CenteRightChart_DataBinding(object sender, EventArgs e)
        {
            
            CenteRightChart.DataSource = GetDSForChart("ChartRightCenter");
            if (BN == "IE")
            {
                conf_Chart(30, CenteRightChart, false);
            }
            if (BN == "FIREFOX")
            {
                conf_Chart(30, CenteRightChart, false);
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                conf_Chart(31, CenteRightChart, false);
            }
           //CenteRightChart.Tooltips.FormatString = CenterGrid.Columns[2].Header.Caption + " <b><DATA_VALUE:00.##></b>";
            CenteRightChart.Tooltips.FormatString = "<ITEM_LABEL>, " + _GetString_(CenterGrid.Columns[2].Header.Caption) + " <b><DATA_VALUE:00.##></b>"; 
        }

        protected void BottomChart_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDSForChart("ChartBotom");
            

            double max = double.Parse(dt.Rows[0].ItemArray[1].ToString()),
                    min = double.Parse(dt.Rows[0].ItemArray[1].ToString());
            try
            {
                for (int j = 0; j < dt.Rows.Count; j++)
                    for (int i = 2; i < dt.Rows[j].ItemArray.Length; i++)
                    {
                        try
                        {
                            if (double.Parse(dt.Rows[j].ItemArray[i].ToString()) > max) { max = double.Parse(dt.Rows[j].ItemArray[i].ToString()); }
                            if (double.Parse(dt.Rows[j].ItemArray[i].ToString()) < min) { min = double.Parse(dt.Rows[j].ItemArray[i].ToString()); }
                        }
                        catch { }
                    }
            }
            catch { }
            BottomChart.Axis.Y.RangeMax = max*1.2;
            BottomChart.Axis.Y.RangeMin = min*0.8;
            BottomChart.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
            BottomChart.DataSource = dt;

            if (BN == "IE")
            {
                conf_Chart(95, BottomChart, false);
            }
            if (BN == "FIREFOX")
            {
                conf_Chart(95, BottomChart, false);
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                conf_Chart(95, BottomChart, false);
                if (screen_width != 1024)
                { conf_Chart(94, BottomChart, false); }
                
            }
            //BottomChart.DataBind();
            
        }
        string ed = "karambyla!!!";
        protected void TopGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            Pokaz.Value = GetString_(e.Row.Cells[0].Text);
            ed = (e.Row.Cells[0].Text);
            TopChart.DataBind();
            ChartTopText.Text = "Динамика показателя " + '"' + GetString_(e.Row.Cells[0].Text) + '"' + ", " + _GetString_(e.Row.Cells[0].Text);
            
        }

        protected void CenterGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            Pokaz.Value = e.Row.Cells[0].Text;

            CenterLeftChart.DataBind();
            CenteRightChart.DataBind();

            ChartCenterLeftText.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart2_title"), e.Row.Cells[0].Text);
            ChartCenterRightText.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart3_title"), e.Row.Cells[0].Text);
            //ChartCenterLeftText.Text = "Отпущено тепловой энергии в " + e.Row.Cells[0].Text + " году, %";// +_GetString_(CenterGrid.Columns[2].Header.Caption);
            //ChartCenterRightText.Text = "Отпущено воды в  " + e.Row.Cells[0].Text + " году, %";// +_GetString_(CenterGrid.Columns[1].Header.Caption);
        }

        protected void BottomGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            String.Format(Pokaz.Value);
            Pokaz.Value =GetString_(e.Row.Cells[0].Text);
            String.Format(Pokaz.Value);

            BottomChart.DataBind();
            //ChartBottomText.Text = '"'+GetString_(e.Row.Cells[0].Text)+'"'+", "+_GetString_(e.Row.Cells[0].Text);
            ChartBottomText.Text = "Динамика показателей " + '"' + GetString_(e.Row.Cells[0].Text) + '"' + ", " + _GetString_(e.Row.Cells[0].Text);
            BottomChart.Tooltips.FormatString = GetString_(e.Row.Cells[0].Text)+", "+_GetString_(e.Row.Cells[0].Text) + " <b><DATA_VALUE:00.##></b>"; 
        }

        protected void BottomGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[1].Header.Caption = Lastdate.Value;
            if (BN == "IE")
            {
                e.Layout.Bands[0].Columns[0].Width = (int)(screen_width * 0.6);
                e.Layout.Bands[0].Columns[1].Width = (int)(screen_width * 0.1);
            }
            if (BN == "FIREFOX")
            {
                e.Layout.Bands[0].Columns[0].Width = (int)(screen_width * 0.6);
                e.Layout.Bands[0].Columns[1].Width = (int)(screen_width * 0.1);
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                e.Layout.Bands[0].Columns[0].Width = (int)(screen_width * 0.58);
                e.Layout.Bands[0].Columns[1].Width = (int)(screen_width * 0.1);
            }
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].Header.Style.HorizontalAlign = HorizontalAlign.Center;

        }

        protected void CenterGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            conf_Grid(35, CenterGrid);
            //GridCenterText.Width = (int)(screen_width * 0.3);
            double GW = (int)(screen_width * 0.40);
            
            if (BN == "IE")
            {
            e.Layout.Bands[0].Columns[0].Width = (int)(GW * 0.1*0.95);
            e.Layout.Bands[0].Columns[1].Width = (int)((GW * 0.45 + GW * 0.27) / 2.0 * 0.95);
            e.Layout.Bands[0].Columns[2].Width = (int)((GW * 0.45 + GW * 0.27) / 2.0 * 0.95);
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                e.Layout.Bands[0].Columns[0].Width = (int)(GW * 0.1 * 0.95);
                e.Layout.Bands[0].Columns[1].Width = (int)((GW * 0.45 + GW * 0.20) / 2 * 0.95);
                e.Layout.Bands[0].Columns[2].Width = (int)((GW * 0.45 + GW * 0.20) / 2 * 0.95);
            }
            if (BN == "FIREFOX")
            {
            e.Layout.Bands[0].Columns[0].Width = (int)(GW * 0.1-10);
            e.Layout.Bands[0].Columns[1].Width = (int)(-10+(GW * 0.45 + GW * 0.28) / 2 * 0.95);
            e.Layout.Bands[0].Columns[2].Width = (int)(-10+(GW * 0.45 + GW * 0.28) / 2 * 0.95);
            }
            e.Layout.Bands[0].Columns[2].Header.Style.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[1].Header.Style.Wrap = 1 == 1; 
            e.Layout.Bands[0].Columns[0].Header.Caption = "Год";
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "### ### ##0.00");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "### ### ##0.00");
        }

        protected void TopGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {//grid.Width = 
            conf_Grid(30, TopGrid);
            double GW = screen_width * 40.0 / 100.0; ;
            if (BN == "IE")
            {

                e.Layout.Bands[0].Columns[0].Width = (int)(GW * 0.58-10);
                e.Layout.Bands[0].Columns[1].Width = (int)(GW * 0.20-5);
            }
            if (BN == "FIREFOX")
            {

                e.Layout.Bands[0].Columns[0].Width = (int)(GW * 0.58-10);
                e.Layout.Bands[0].Columns[1].Width = (int)(GW * 0.21-5);
            }
            if (BN == "APPLEMAC-SAFARI")
            {

                e.Layout.Bands[0].Columns[0].Width = (int)(GW * 0.57-10);
                e.Layout.Bands[0].Columns[1].Width = (int)(GW * 0.17-5);
            }

            //GridTopText.Width = (int)(screen_width * 0.3);
            TopGrid.Columns[1].Header.Caption = Lastdate.Value;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].Header.Style.HorizontalAlign = HorizontalAlign.Center;
          
        }

        protected void CenterLeftChart_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void CenteRightChart_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void TopChart_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void BottomChart_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void TopChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];
                        //for (int i = 0; i < dtTopChart.Rows.Count; i++)
           
            foreach(Primitive pr in e.SceneGraph)
            {
                //Primitive pr = e.SceneGraph[i];
//                if (pr is Polyline)
//                {
//DataPoint dp =  new DataPoint(1,1);
//dp.DataPoint.
//                    Polyline line = new Polyline( ((Polyline)pr);
//                    previousLine = lastLine;
//                    lastLine = ((Polyline)pr);
                    //line.Visible = line.points[0].point.X < line.points[line.points.Length - 1].point.X;
//line.points.
                }
            //}
            for (int i = 0; i < dtTopChart.Rows.Count; i++)
            {
                for (int j = 1; j < dtTopChart.Columns.Count; j++)
                {
                    //try
                    {
                        //String.Format(dtTopChart.Rows[i][j + 1].ToString());  
                        if (!(string.IsNullOrEmpty(dtTopChart.Rows[i][j].ToString())))// & (string.IsNullOrEmpty(dtTopChart.Rows[i][j - 1].ToString())))
                        {
                            DataPoint dp1 = new DataPoint((int)xAxis.Map(j - 1), (int)yAxis.Map(dtTopChart.Rows[i][j]));
                            //DataPoint dp2 = new DataPoint((int)xAxis.Map(j - 1), (int)yAxis.Map(dtTopChart.Rows[i][j]));
                            dp1.Row = i;
                            dp1.Column = j;
                            // = 42;
                            double value;
                            double.TryParse(dtTopChart.Rows[i][j].ToString(), out value);
                            dp1.Value = value;
                            //dp1.
                            e.SceneGraph.Add(dp1);
                            //Polyline pl = new Polyline(dp1, dp2);
                            //dp1.PE
                            Box box = new Box(new Rectangle(
                                        (int)xAxis.Map(j - 1) - 3, (int)yAxis.Map(dtTopChart.Rows[i][j]) - 3, 6, 6));
                            //box.PE.ElementType = Infragistics.UltraChart.Shared.Styles.PaintElementType.Gradient;
                            //box.PE.FillGradientStyle =Infragistics.UltraChart.Shared.Styles.GradientStyle.ForwardDiagonal;
                            box.PE.Fill = Color.Gray;//Color.FromArgb(101, 162, 203);
                            box.PE.FillStopColor = Color.Gray;//Color.FromArgb(8, 106, 172);
                            if (i == 0)
                            {
                                box.PE.Fill = Color.Red;//Color.FromArgb(101, 162, 203);
                                box.PE.FillStopColor = Color.Red;//Color.FromArgb(8, 106, 172);
                                box.PE.Stroke = Color.Black;
                            }
                            if (i == 1)
                            {
                                box.PE.Fill = Color.Blue;//Color.FromArgb(101, 162, 203);
                                box.PE.FillStopColor = Color.Blue;//Color.FromArgb(8, 106, 172);
                                box.PE.Stroke = Color.Black;
                            }
                            if (i == 2)
                            {
                                box.PE.Fill = Color.Green;//Color.FromArgb(101, 162, 203);
                                box.PE.FillStopColor = Color.Green;//Color.FromArgb(8, 106, 172);
                                box.PE.Stroke = Color.Black;
                            }
                            if (i == 3)
                            {
                                box.PE.Fill = Color.Yellow;//Color.FromArgb(101, 162, 203);
                                box.PE.FillStopColor = Color.Yellow;//Color.FromArgb(8, 106, 172);
                                box.PE.Stroke = Color.Black;
                            if (i == 4)
                            {
                                box.PE.Fill = Color.Gray;//Color.FromArgb(101, 162, 203);
                                box.PE.FillStopColor = Color.Gray;//Color.FromArgb(8, 106, 172);
                                box.PE.Stroke = Color.Black;
                            }

                            }
                            
                            //box.PE.StrokeWidth = 1;

                            //box.Layer = e.ChartCore.GetChartLayer();
                            //box.Chart = Infragistics.UltraChart.Shared.Styles.ChartType.LineChart;//this.ultraChart.ChartType;
                            //ox.DataPoint = new Da//.Label = dtTopChart.Rows[i][0].ToString();
                            
                             //box.

                            e.SceneGraph.Add(box);

                        }
                    }
                    //catch { }
               }
                
            }
        }
    }
}
