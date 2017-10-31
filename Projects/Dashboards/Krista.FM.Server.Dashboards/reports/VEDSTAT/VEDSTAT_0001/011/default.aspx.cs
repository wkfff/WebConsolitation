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
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._0110
{
    public partial class Default : CustomReportPage
    {
        private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
        private CustomParam Pokaz { get { return (UserParams.CustomParam("Pokaz")); } }
        private CustomParam param { get { return (UserParams.CustomParam("param")); } }
        private CustomParam Lastdate { get { return (UserParams.CustomParam("lastdate")); } }
        private Int32 screen_width { get { return (int)Session["width_size"]; } }
        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }	

        private string FSO = "<font size = " + '"' + 8 + '"' + ">";
        private string FSB = "</font>";
        private CustomParam G1Marks;
        private CustomParam AllC1Marks;
        private void GridActiveRow(UltraWebGrid Grid, int index, bool active)
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
            if (typ == 10) { lab.Font.Bold = 1 == 1; };
            if (typ == 18) { lab.Font.Bold = 1 == 1; };
            if (typ == 16) { lab.Font.Bold = 1 == 1; };
               
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
        private void conf_Grid(int sizePercent, UltraWebGrid grid, LayoutEventArgs e)
        {

            //grid.Width = screen_width * sizePercent / 100;
            double GW = screen_width * sizePercent / 100;
            grid.Bands[0].Columns[0].Header.Style.Wrap = 1 == 1;
            grid.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            double Coef = 1;
            if (BN == "IE")
            { Coef = 1; }
            if (BN == "FIREFOX")
            {
                Coef = 1.027;
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                Coef = 1.022;
            }

              grid.Bands[0].Columns[0].Width = (int)((GW*Coef) * 0.4);
            for (int i = 1; i < grid.Columns.Count; i++)
            {
                grid.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                grid.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(grid.Bands[0].Columns[i], "### ### ### ###.##");
                grid.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Width = (int)((GW * Coef) * (0.5)) / (grid.Columns.Count - 1);
               


            }
            //grid.DisplayLayout.CellClickActionDefault = CellClickAction.Select;
            //grid.DisplayLayout.HeaderTitleModeDefault = CellTitleMode.Always;
            //grid.DisplayLayout.RowSelectorsDefault = RowSelectors.Yes;
            grid.DisplayLayout.GroupByBox.Hidden = 1 == 1;
            grid.DisplayLayout.NoDataMessage = "Нет данных";
            grid.DisplayLayout.NoDataMessage = "Нет данных";
        }
        private void conf_Chart(int sizePercent, Infragistics.WebUI.UltraWebChart.UltraChart chart, bool leg, Infragistics.UltraChart.Shared.Styles.ChartType typ)
        {
            chart.Width = screen_width * sizePercent / 100;
            chart.SplineAreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8), Color.Black, "<DATA_VALUE:#>", StringAlignment.Far, StringAlignment.Center, 0));
            if (leg)
            {
                chart.Legend.Visible = 1 == 1;
                chart.Legend.Location = Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom;
                chart.Legend.SpanPercentage = 40;

            }
            chart.FunnelChart3D.RadiusMax = 0.5;
            chart.FunnelChart3D.RadiusMin = 0.1;
            chart.FunnelChart3D.OthersCategoryText = "Прочие";
            chart.ChartType = typ;
            chart.PieChart3D.OthersCategoryText = "Прочие";
            chart.DoughnutChart.OthersCategoryPercent = 1;
            chart.PieChart3D.OthersCategoryPercent = 1;
            if (typ == Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D) { chart.Transform3D.Scale = 90; chart.Transform3D.XRotation = 30; }
            chart.DoughnutChart.OthersCategoryText = "Прочие";
            chart.DoughnutChart.RadiusFactor = 100;
            chart.DoughnutChart.InnerRadius = 25;
            chart.DeploymentScenario.FilePath = "../../../../TemporaryImages";
            chart.DeploymentScenario.ImageURL = "../../../../TemporaryImages/Chart_#SEQNUM(100).png";
            if (typ == Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart) { chart.AreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8), Color.Black, "<DATA_VALUE:#>", StringAlignment.Far, StringAlignment.Center, 0)); };
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
        string BN = "IE";
        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                G1Marks = UserParams.CustomParam("G1Marks");
                AllC1Marks = UserParams.CustomParam("AllC1Marks");
                G1Marks = ForMarks.SetMarks(G1Marks, ForMarks.Getmarks("G1_mark_"), true);
                AllC1Marks = ForMarks.SetMarks(AllC1Marks, ForMarks.Getmarks("AllC1_mark_"), true);
                System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
                BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
                RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                Lastdate.Value = ELV(getLastDate(RegionSettingsHelper.Instance.GetPropertyValue("way_last_year_mark")));

                GT.DataBind();
                Pokaz.Value = "2007";
                C1.DataBind();
                C2.DataBind();
                C3.DataBind();
                C4.DataBind();
                C21.DataBind();
                C22.DataBind();
                C23.DataBind();
                C24.DataBind();
                if (BN == "FIREFOX")
                {
                    C23.Height = 317;
                }
                if (BN == "APPLEMAC-SAFARI")
                {
                    C23.Height = 317;
                }
                if (!Page.IsPostBack)
                {
               
                    
                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(GT);


                    Label2.Width = C1.Width;
                    Label3.Width = C2.Width;
                    Label4.Width = C3.Width;
                    Label5.Width = C4.Width;
                    Label6.Width = C21.Width;
                    Label7.Width = C22.Width;
                    Label8.Width = C23.Width;
                    Label9.Width = C24.Width;
                    int size = 10;
                    //setFont(size, Label2);
                    //setFont(size, Label3);
                    //setFont(size, Label4);
                    //setFont(size, Label5);
                    //setFont(size, Label6);
                    //setFont(size, Label7);
                    //setFont(size, Label8);
                    //setFont(size, Label9);
                    Label10.Text = RegionSettingsHelper.Instance.GetPropertyValue("page_title");
                    //setFont(16, Label10);
                    Label1.Text = RegionSettingsHelper.Instance.GetPropertyValue("G1_title");
                    //setFont(size, Label1);
                    GT.Columns[GT.Columns.Count - 1].Selected = true;


                    CellSet CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("G1PR"));
                    for (int i = 0; i < CLS.Cells.Count; i++)
                    {
                        GT.Rows[i].Cells[0].Text += ", " + CLS.Cells[i].Value.ToString().ToLower();

                    }
                }
            }
            catch { };
        }



        protected void GT_DataBinding(object sender, EventArgs e)
        {
            GT.DataSource = GetDSForChart("G1");

        }

        protected void C1_DataBinding(object sender, EventArgs e)
        {
            param.Value = "Количество квартир в жилищном фонде города";
            Label2.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart1_title"),Pokaz.Value);
            //Label2.Text = param.Value + " на " + Pokaz.Value + " год, единиц";

            C1.DataSource = GetDSForChart("ALLC1");
            conf_Chart(25, C1, true, Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D);
            //<ITEM_LABEL>
            C1.Tooltips.FormatString = "<ITEM_LABEL>, единиц <b><DATA_VALUE:00.##></b>"; 
        }

        protected void C2_DataBinding(object sender, EventArgs e)
        {
            //param.Value = "Площадь квартир в жилищном фонде города";
            param.Value = "Количество квартир, находящиеся в собственности юридических лиц";
            Label3.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart2_title"), Pokaz.Value);
           // Label3.Text = param.Value + " на " + Pokaz.Value + " год, единиц";

            C2.DataSource = GetDSForChart("ALLC1");
            conf_Chart(22, C2, true, Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D);
            C2.Tooltips.FormatString = "<ITEM_LABEL>, единиц <b><DATA_VALUE:00.##></b>"; 
        }

        protected void C3_DataBinding(object sender, EventArgs e)
        {
            param.Value = "Количество квартир, находящиеся в собственности граждан";
            Label4.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart3_title"), Pokaz.Value);
            //Label4.Text = param.Value + " на " + Pokaz.Value  + " год, единиц";
            C3.DataSource = GetDSForChart("ALLC1");
            conf_Chart(22, C3, true, Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D);
            C3.Tooltips.FormatString = "<ITEM_LABEL>, единиц <b><DATA_VALUE:00.##></b>";
        }

        protected void C4_DataBinding(object sender, EventArgs e)
        {
            //param.Value = "Площадь квартир, находящиеся в собственности граждан";
            param.Value = "Количество квартир, находящиеся в муниципальной собственности";
            Label5.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart4_title"), Pokaz.Value);
           // Label5.Text = param.Value + " на " + Pokaz.Value +  " год, единиц";
            C4.DataSource = GetDSForChart("ALLC1");
            conf_Chart(22, C4, true, Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D);
            C4.Tooltips.FormatString = "<ITEM_LABEL>, единиц <b><DATA_VALUE:00.##></b>"; 
        }

        protected void C21_DataBinding(object sender, EventArgs e)
        {
            param.Value = "Площадь квартир в жилищном фонде города";
            //
            Label6.Text=String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart21_title"), Pokaz.Value);
            //Label6.Text = param.Value + " на " + Pokaz.Value + " год, тысячи квадратных метров";
            C21.DataSource = GetDSForChart("ALLC1");
            conf_Chart(22, C21, true, Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart);
            C21.Tooltips.FormatString = "<ITEM_LABEL>, тысячи м" + "<font size = 5>² " + "</font>" + "<b><DATA_VALUE:00.##></b>"; 
        }

        protected void C22_DataBinding(object sender, EventArgs e)
        {
            param.Value = "Площадь квартир, находящиеся в собственности юридических лиц";
            Label7.Text=String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart22_title"), Pokaz.Value);
          //  Label7.Text = param.Value + " на " + Pokaz.Value +  " год, тысячи квадратных метров";
            C22.DataSource = GetDSForChart("ALLC1");
            conf_Chart(22, C22, true, Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart);
            C22.Tooltips.FormatString = "<ITEM_LABEL>, тысячи м" + "<font size = 5>² " + "</font>" + "<b><DATA_VALUE:00.##></b>"; 
        }

        protected void C23_DataBinding(object sender, EventArgs e)
        {
            //param.Value = "Количество квартир, находящиеся в муниципальной собственности";
            param.Value = "Площадь квартир, находящиеся в собственности граждан";
            Label8.Text=String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart23_title"), Pokaz.Value);
            //Label8.Text = param.Value + " на " + Pokaz.Value +  " год, тысячи квадратных метров";
            C23.DataSource = GetDSForChart("ALLC1");
            conf_Chart(22, C23, true, Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart);
            C23.Tooltips.FormatString = "<ITEM_LABEL>, тысячи м" + "<font size = 5>² " + "</font>" + "<b><DATA_VALUE:00.##></b>";
        }

        protected void C24_DataBinding(object sender, EventArgs e)
        {
            param.Value = "Площадь квартир, находящиеся в муниципальной собственности";
            Label9.Text=String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart24_title"), Pokaz.Value);
            //Label9.Text = param.Value + " на " + Pokaz.Value + " год, тысячи квадратных метров";
            C24.DataSource = GetDSForChart("ALLC1");
            conf_Chart(22, C24, true, Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart);
            C24.Tooltips.FormatString = "<ITEM_LABEL>, тысячи м" + "<font size = 5>² " + "</font>" + "<b><DATA_VALUE:00.##></b>"; 
        }

        protected void GT_ActiveRowChange(object sender, RowEventArgs e)
        {
            // ;  //e.Row.Cells[0].Text;
            /*C1.DataBind();
            C2.DataBind();
            C3.DataBind();
            C4.DataBind();
            C21.DataBind();
            C22.DataBind();
            C23.DataBind();
            C24.DataBind();
            */
        }

        protected void GT_ActiveCellChange(object sender, CellEventArgs e)
        {
            if ((e.Cell.Column.Index != 0))
            {
                Pokaz.Value = GT.Columns[e.Cell.Column.Index].Header.Key.ToString();   }
                C1.DataBind();
                C2.DataBind();
                C3.DataBind();
                C4.DataBind();
                C21.DataBind();
                C22.DataBind();
                C23.DataBind();
                C24.DataBind();
         
        }

        protected void C1_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void web_grid_Click(object sender, ClickEventArgs e)
        {
            if ((e.Column.Index != 0))
            {
                Pokaz.Value = GT.Columns[e.Column.Index].Header.Key.ToString();}
                C1.DataBind();
                C2.DataBind();
                C3.DataBind();
                C4.DataBind();
                C21.DataBind();
                C22.DataBind();
                C23.DataBind();
                C24.DataBind();
            
        }

        protected void GT_InitializeLayout(object sender, LayoutEventArgs e)
        {
            conf_Grid(97, GT,e);
            e.Layout.RowSelectorStyleDefault.Width = 0;
        }


    }
}
