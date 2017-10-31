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

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._0260
{
    public partial class Default : CustomReportPage
    {
        protected DataTable maintable = new DataTable();
        private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
        private CustomParam Pokaz { get { return (UserParams.CustomParam("Pokaz")); } }
        private CustomParam Lastdate { get { return (UserParams.CustomParam("lastdate")); } }
        private Int32 screen_width { get { return (int)Session["width_size"]; } }
        // параметр запроса дл€ региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam grid1Marks;
        private CustomParam chart1Marks;
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
        
        public static void setChartErrorFont(Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "¬ насто€щий момент данные отсутствуют";

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
            if (typ == 16) { lab.Font.Bold = 1 == 1;

            lab.Font.Size = FontUnit.Medium;
            };
        }
        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(s, "ѕоказатель", dt);
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
        string BN="IE";
        private void conf_Grid(int sizePercent, UltraWebGrid grid)
        {
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            int width = screen_width * sizePercent / 100;
            grid.Bands[0].Columns[0].Width = (int)(((width) * 0.64));
            grid.Bands[0].Columns[1].Width = (int)(((width) * 0.24));
            if (BN == "IE")
            {
                
                
                grid.Bands[0].Columns[0].Width = (int)(((width) * 0.70));
                grid.Bands[0].Columns[1].Width = (int)(((width) * 0.19));
            }
            if (BN == "FIREFOX")
            {
                grid.Bands[0].Columns[0].Width = (int)(((width) * 0.70));
                grid.Bands[0].Columns[1].Width = (int)(((width) * 0.16));
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                grid.Bands[0].Columns[0].Width = (int)(((width) * 0.70));
                grid.Bands[0].Columns[1].Width = (int)(((width) * 0.20));
            }
            grid.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            
            for (int i = 1; i < grid.Columns.Count; i++)
            {
                grid.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                grid.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(grid.Bands[0].Columns[i], "### ### ### ###.##");
            }
            grid.DisplayLayout.CellClickActionDefault = CellClickAction.RowSelect;
            grid.DisplayLayout.HeaderTitleModeDefault = CellTitleMode.Always;
            grid.DisplayLayout.RowSelectorsDefault = RowSelectors.Yes;
            grid.DisplayLayout.GroupByBox.Hidden = 1 == 1;
            grid.DisplayLayout.NoDataMessage = "Ќет данных";
            grid.DisplayLayout.NoDataMessage = "Ќет данных";
        }
        private void conf_Chart(int sizePercent, Infragistics.WebUI.UltraWebChart.UltraChart chart, bool leg)
        {
            chart.Width = screen_width * sizePercent / 100;
            chart.SplineAreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8), Color.Black, "<DATA_VALUE:#>", StringAlignment.Far, StringAlignment.Center, 0));
            if (leg)
            {
                chart.Legend.Visible = 1 == 1;
                chart.Legend.Location = Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom;
                chart.Legend.SpanPercentage = 30;

            }
            chart.FunnelChart3D.RadiusMax = 0.5;
            chart.FunnelChart3D.RadiusMin = 0.1;
            chart.FunnelChart3D.OthersCategoryText = "ѕрочие";

        }

        private string GetString_(string s)
        {
            string res = "";
            int i = 0;
            for (i = s.Length - 1; s[i] != ','; i--) ;
            for (int j = 0; j < i; j++)
            {
                res += s[j];
            }
            return res;


        }


        private string _GetString_(string s)
        {
            string res = "";
            int i = 0;
            for (i = s.Length - 1; s[i] != ','; i--) { res = s[i] + res; };

            return res;


        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                    grid1Marks = UserParams.CustomParam("grid1Marks");
                    chart1Marks = UserParams.CustomParam("chart1Marks");
                    grid1Marks = ForMarks.SetMarks(grid1Marks, ForMarks.Getmarks("grid1_mark_"), true);
                    chart1Marks = ForMarks.SetMarks(chart1Marks, ForMarks.Getmarks("chart1_mark_"), true);
                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(Grid);

                    Lastdate.Value = ELV(getLastDate(RegionSettingsHelper.Instance.GetPropertyValue("way_last_year_mark")));
                    Grid.DataBind();
                    Pokaz.Value = Grid.Rows[0].Cells[0].Text;
                    Label1.Text = RegionSettingsHelper.Instance.GetPropertyValue("grid1_title");
                    Chart.DataBind();
                    Label2.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart1_title"), Pokaz.Value);
                    
                    Label3.Text = RegionSettingsHelper.Instance.GetPropertyValue("page_title");
                    //setFont(10, Label2);
                    //setFont(10, Label1);
                    //setFont(16, Label3);
                    Label4.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart1_title1");
                    Label2.Width = Chart.Width;
                    Label1.Width = Grid.Width;
                    //Grid.Height = Chart.Height;

                    GridActiveRow(Grid, 0, true);
                    //Label1.Height = Label2.Height = 30;
                    CellSet CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("GE"));

                    for (int i = 0; i < CLS.Cells.Count; i++)
                    {
                        Grid.Rows[i].Cells[0].Text += ", " + CLS.Cells[i].Value.ToString().ToLower();

                    }
                    Chart.Tooltips.FormatString = "<ITEM_LABEL>, " + _GetString_(Grid.Rows[0].Cells[0].Text) + " <DATA_VALUE:00.##>";
                    if (BN == "FIREFOX")
                    {
                        Grid.Height = 323;
                    }
                }
                Chart.DataBind();
            }
            catch { }

  
 
        }

        protected void TopGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            Pokaz.Value = GetString_( e.Row.Cells[0].Text);
            Label2.Text = '"' + Pokaz.Value + '"';
            Chart.DataBind();
            Chart.Tooltips.FormatString = "<ITEM_LABEL>, " + _GetString_(e.Row.Cells[0].Text) + " <DATA_VALUE:00.##>"; 
            
            //for(;eror;){eror = false; Chart.DataBind();};
            
        }

        protected void TopGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            conf_Grid(45, Grid);
            Grid.Columns[1].Header.Caption = Lastdate.Value;
        }

        protected void TopGrid_DataBinding(object sender, EventArgs e)
        {
            Grid.DataSource = GetDSForChart("Grid");
        }
      //  public bool eror = false;
        protected void Chart_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
         //   eror = true;
        }

        protected void Chart_DataBinding(object sender, EventArgs e)
        {
            Chart.DataSource = GetDSForChart("Chart");
            conf_Chart(50, Chart, false);
            
        }
    }
}
