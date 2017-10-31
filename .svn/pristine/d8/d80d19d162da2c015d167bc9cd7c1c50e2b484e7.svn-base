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
            //if (sizePercent != 0) { grid.Width = screen_width * sizePercent / 100; };
            //grid.Columns[2].CellStyle.Wrap = 1 == 1;
            
            for (int i = 0; i < grid.Columns.Count; i++) 
            {
                grid.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                grid.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(grid.Bands[0].Columns[i], "### ### ### ###.##");
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
            try
            {
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
          
                   

                    CenterGrid.DataBind();
                    conf_Grid(35, CenterGrid);
                    CellSet CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("GCE"));
                    for (int i = 0; i < CLS.Cells.Count; i++)
                    {
                        CenterGrid.Columns[i + 1].Header.Caption += ", " + CLS.Cells[i].Value.ToString().ToLower();

                    }
                    Pokaz.Value = CenterGrid.Rows[0].Cells[0].Text;
                    CenteRightChart.DataBind();

                    CenterLeftChart.DataBind();
                    BottomGrid.DataBind();
                    conf_Grid(95, BottomGrid);
                    Pokaz.Value = BottomGrid.Rows[0].Cells[0].Text;
                    BottomChart.DataBind();
                    ChartTopText.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart1_title") + '"' + TopGrid.Rows[0].Cells[0].Text+'"';
                    ChartCenterLeftText.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart2_title"), CenterGrid.Rows[0].Cells[0].Text);
                    ChartCenterRightText.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart3_title"),CenterGrid.Rows[0].Cells[0].Text);
                    ChartBottomText.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart4_title"), BottomGrid.Rows[0].Cells[0].Text);
                    
                    Label2.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart4_title1");
                    //TopGrid.Height = TopChart.Height;
                   // CenterGrid.Height = CenterLeftChart.Height = CenteRightChart.Height;



                    GridTopText.Text = RegionSettingsHelper.Instance.GetPropertyValue("grid1_title");
                    GridCenterText.Text = RegionSettingsHelper.Instance.GetPropertyValue("grid2_text");
                    GridBottomText.Text = RegionSettingsHelper.Instance.GetPropertyValue("grid3_text");
                    //conf_Chart(28, CenteRightChart, false);
                    //conf_Chart(28, CenterLeftChart, false);
                    ChartBottomText.Width = screen_width * 60 / 100;
                    ChartCenterLeftText.Width = screen_width * 22 / 100;
                    ChartCenterRightText.Width = screen_width * 22 / 100;
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
                    GridActiveRow(TopGrid, 0, true);
                    GridActiveRow(CenterGrid, 0, true);

                    GridActiveRow(BottomGrid, 0, true);
                    int iz = (int)BottomGrid.Width.Value;
                    //BottomChart.Width = iz ;
                    CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("GTE"));
                    for (int i = 0; i < CLS.Cells.Count; i++)
                    {
                       TopGrid.Rows[i].Cells[0].Text += ", " + CLS.Cells[i].Value.ToString().ToLower();

                    }

                    CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("GBE"));
                    for (int i = 0; i < CLS.Cells.Count; i++)
                    {
                       BottomGrid.Rows[i].Cells[0].Text += ", " + CLS.Cells[i].Value.ToString().ToLower();

                    }
                    
                    ed= TopGrid.Rows[0].Cells[0].Text;
                    Pokaz.Value = GetString_(ed);
                    
                    TopChart.DataBind();
                    ChartTopText.Width = TopChart.Width;
                    BottomChart.Tooltips.FormatString = GetString_(BottomGrid.Rows[0].Cells[0].Text) + ", " + _GetString_(BottomGrid.Rows[0].Cells[0].Text) + " <b><DATA_VALUE:00.##></b>";
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
                        CenterGrid.Height = 256;
                    }
                }
            }
            catch
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

        protected void TopChart_DataBinding(object sender, EventArgs e)
        {
            TopChart.DataSource = GetDSForChart("ChartTop");
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
            CenterLeftChart.Tooltips.FormatString = CenterGrid.Columns[1].Header.Caption + " <b><DATA_VALUE:00.##></b>"; 
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
           CenteRightChart.Tooltips.FormatString = CenterGrid.Columns[2].Header.Caption + " <b><DATA_VALUE:00.##></b>"; 
        }

        protected void BottomChart_DataBinding(object sender, EventArgs e)
        {
            BottomChart.DataSource = GetDSForChart("ChartBotom");
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
            ChartTopText.Text = "Динамика показателя " + '"' + e.Row.Cells[0].Text + '"' + ", " + _GetString_(e.Row.Cells[0].Text);
            
        }

        protected void CenterGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            Pokaz.Value = e.Row.Cells[0].Text;

            CenterLeftChart.DataBind();
            CenteRightChart.DataBind();
            ChartCenterLeftText.Text =  "Отпущено тепловой энергии в " + e.Row.Cells[0].Text +" году,"+_GetString_(CenterGrid.Columns[2].Header.Caption);
            ChartCenterRightText.Text = "Отпущено воды в  " + e.Row.Cells[0].Text + " году," + _GetString_(CenterGrid.Columns[1].Header.Caption);
        }

        protected void BottomGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            Pokaz.Value =GetString_(e.Row.Cells[0].Text);


            if (e.Row.Index == 0)
            {
                BottomChart.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom; BottomChart.Axis.Y.RangeMax = 10;
                BottomChart.Axis.Y.RangeMin = 0;
            }
            else
            {
                BottomChart.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Automatic; BottomChart.Axis.Y.RangeMax = 0;
                BottomChart.Axis.Y.RangeMin = 0;
            }
            BottomChart.DataBind();
            ChartBottomText.Text ='"'+e.Row.Cells[0].Text+'"';
            BottomChart.Tooltips.FormatString = GetString_(e.Row.Cells[0].Text)+", "+_GetString_(e.Row.Cells[0].Text) + " <b><DATA_VALUE:00.##></b>"; 
        }

        protected void BottomGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[1].Header.Caption = Lastdate.Value;
            if (BN == "IE")
            {
                e.Layout.Bands[0].Columns[0].Width = (int)(screen_width * 0.6);
                e.Layout.Bands[0].Columns[1].Width = (int)(screen_width * 0.27);
            }
            if (BN == "FIREFOX")
            {
                e.Layout.Bands[0].Columns[0].Width = (int)(screen_width * 0.6);
                e.Layout.Bands[0].Columns[1].Width = (int)(screen_width * 0.27);
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                e.Layout.Bands[0].Columns[0].Width = (int)(screen_width * 0.58);
                e.Layout.Bands[0].Columns[1].Width = (int)(screen_width * 0.26);
            }
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
        }

        protected void CenterGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            conf_Grid(30, CenterGrid);
            //GridCenterText.Width = (int)(screen_width * 0.3);
            double GW = (int)(screen_width * 0.3);
            
            if (BN == "IE")
            {
            e.Layout.Bands[0].Columns[0].Width = (int)(GW * 0.1);
            e.Layout.Bands[0].Columns[1].Width = (int)(GW * 0.45);
            e.Layout.Bands[0].Columns[2].Width = (int)(GW * 0.27);
            }
            if (BN == "APPLEMAC-SAFARI")
            {
            e.Layout.Bands[0].Columns[0].Width = (int)(GW * 0.1);
            e.Layout.Bands[0].Columns[1].Width = (int)(GW * 0.45);
            e.Layout.Bands[0].Columns[2].Width = (int)(GW * 0.20);
            }
            if (BN == "FIREFOX")
            {
            e.Layout.Bands[0].Columns[0].Width = (int)(GW * 0.1);
            e.Layout.Bands[0].Columns[1].Width = (int)(GW * 0.45);
            e.Layout.Bands[0].Columns[2].Width = (int)(GW * 0.28);
            }
            e.Layout.Bands[0].Columns[2].Header.Style.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[1].Header.Style.Wrap = 1 == 1; 
            e.Layout.Bands[0].Columns[0].Header.Caption = "Год";
        }

        protected void TopGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {//grid.Width = 
            conf_Grid(30, TopGrid);
            double GW = screen_width * 30 / 100; ;
            if (BN == "IE")
            {

                e.Layout.Bands[0].Columns[0].Width = (int)(GW * 0.60);
                e.Layout.Bands[0].Columns[1].Width = (int)(GW * 0.21);
            }
            if (BN == "FIREFOX")
            {

                e.Layout.Bands[0].Columns[0].Width = (int)(GW * 0.60);
                e.Layout.Bands[0].Columns[1].Width = (int)(GW * 0.22);
            }
            if (BN == "APPLEMAC-SAFARI")
            {

                e.Layout.Bands[0].Columns[0].Width = (int)(GW * 0.60);
                e.Layout.Bands[0].Columns[1].Width = (int)(GW * 0.17);
            }

            GridTopText.Width = (int)(screen_width * 0.3);
            TopGrid.Columns[1].Header.Caption = Lastdate.Value;
          
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
    }
}
