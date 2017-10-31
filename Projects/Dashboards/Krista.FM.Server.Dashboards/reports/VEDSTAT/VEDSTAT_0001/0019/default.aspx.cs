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

namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._0019
{
    public partial class Default : CustomReportPage
    {
        protected DataTable maintable = new DataTable();
        private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
        private CustomParam Pokaz { get { return (UserParams.CustomParam("Pokaz")); } }
        private CustomParam Lastdate { get { return (UserParams.CustomParam("lastdate")); } }
        private CustomParam Postdate { get { return (UserParams.CustomParam("postdate")); } }
        private Int32 screen_width { get { return (int)Session["width_size"]; } }
        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam textMarks;
        private CustomParam gridMarks;
        private CustomParam chartMarks;
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
        #region ForChart
        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(s, "a", dt);
            return dt;
        }

        #endregion
        //--------------------------------------------
        /// <summary>
        /// Get datu
        /// </summary>
        /// <param name="way_ly">po kakomy pokazately</param>
        /// <returns></returns>

        private string _GetString_(string s)
        {
            string res = "";
            int i = 0;
            for (i = s.Length - 1; s[i] != ','; i--) { res = s[i] + res; };

            return res;


        }

        private void setFont(int typ,Label lab)
        {
            lab.Font.Name = "arial";
            lab.Font.Size = typ;
            if (typ == 14) { lab.Font.Bold = 1 == 1; };
            if (typ == 10) { lab.Font.Bold = 1 == 1; };
            if (typ == 18) { lab.Font.Bold = 1 == 1; };
           // if (typ == 9) { lab.Font.Size = FontSize.Small; }

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

        private void IniCustomize(LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.CellClickActionDefault = CellClickAction.RowSelect;
            e.Layout.HeaderTitleModeDefault = CellTitleMode.Always;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            e.Layout.RowSelectorsDefault = RowSelectors.Yes;
            e.Layout.GroupByBox.Hidden = 1 == 1;
            e.Layout.NoDataMessage = "Нет данных";
            e.Layout.NoDataMessage = "Нет данных";
            //double GW = e.Layout.FrameStyle.Width.Value - 60;
           // e.Layout.Bands[0].Columns[0].Width = (int)(GW * 0.6);
            //e.Layout.Bands[0].Columns[1].Width = (int)(GW * 0.4);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "### ### ### ###.##");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "### ### ### ###.##");
            //e.Layout.Bands[0].Columns[1].Width = (int)(screen_width / 10);

        }

        protected double[] getPercent(double[] val)
        {
            double sum = 0;
            for (int i = 0; i < val.Length; i++)
            {
                sum += val[i]; 
            }
            for (int i = 0; i < val.Length; i++)
            {
                val[i] /= sum/100;
            }
            return val;
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
        string BN = "";
        protected override void Page_Load(object sender, EventArgs e)
        {
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            string BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            try
            {



                if (!Page.IsPostBack)
                {
                    textMarks = UserParams.CustomParam("textMarks");
                    textMarks = ForMarks.SetMarks(textMarks, ForMarks.Getmarks("text_mark_"), true);
                    gridMarks = UserParams.CustomParam("gridMarks");
                    gridMarks = ForMarks.SetMarks(gridMarks, ForMarks.Getmarks("grid_mark_"), true);
                    chartMarks = UserParams.CustomParam("chartMarks");
                    chartMarks=ForMarks.SetMarks(chartMarks, ForMarks.Getmarks("chart_mark_"), true);
                    RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(grid);

                    grid.Width = (int)((screen_width - 55) * 0.33);
                    Chart.Width = (int)((screen_width - 55) * 0.66);                    


                    Postdate.Value = ELV(getLastDate(RegionSettingsHelper.Instance.GetPropertyValue("way_last_year_Post")));
                    Lastdate.Value = ELV(getLastDate(RegionSettingsHelper.Instance.GetPropertyValue("way_last_year_Last")));
                    CellSet set = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("text"));
                    double[] mas = new double[set.Cells.Count];
                    string[] mas_t = new string[set.Cells.Count];
                    for (int i = 0; i < mas.Length; i++)
                    {
                        mas_t[i] = set.Axes[1].Positions[i].Members[0].Caption;
                        mas[i] = double.Parse(set.Cells[i].Value.ToString());
                    }
                    mas = getPercent(mas);
                    Label1.Text = "По данным на " + Postdate.Value + " год ";
                   // WebPanel1.Header.Text = "По данным на " + Postdate.Value + " год ";
                    User_param.Text += "Процентное соотношение типов документов, хранящихся в муниципальных архивах, таково:" + "<br/>";
                    for (int i = 0; i < mas.Length; i++)
                    {
                        mas[i] = Math.Round(mas[i], 2);
                        User_param.Text += "&nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;" + "<b>" + mas[i].ToString() + "</b>" + "% - " + mas_t[i] + "<br/>";
                    }
                    User_param.Text += "<br/>";
                    Heder.Text += RegionSettingsHelper.Instance.GetPropertyValue("page_title");
                    grid.DataBind();
                    Pokaz.Value = grid.Rows[0].Cells[0].Text;
                    
                    Chart.DataBind();
                   
                    
                    
                    HederTable.Text = RegionSettingsHelper.Instance.GetPropertyValue("grid_title");
                    //setFont(18, Heder);
                    //setFont(9, User_param);
                    //setFont(10, HederChart);
                   // setFont(10, HederTable);
                    //grid.Height = Chart.Height;
                    GridActiveRow(grid, 0, true);
                    //HederTable.Height = HederChart.Height = 35;

                    for (int i = 0; i < grid.Rows.Count; i++)
                    {
                        grid.Rows[i].Cells[0].Text += ", " + grid.Rows[i].Cells[2].Text.ToLower();

                    }
                    grid.Columns[2].Hidden = 1 == 1;
                    Chart.Tooltips.FormatString = "<ITEM_LABEL>, " + _GetString_(grid.Rows[0].Cells[0].Text) + " <b><DATA_VALUE:00.##></b>";

                    HederChart.Text = '"' + Pokaz.Value + '"' + ", " + _GetString_(grid.Rows[0].Cells[0].Text);
                }
                Label2.Text =RegionSettingsHelper.Instance.GetPropertyValue("chart_title1");
                if (BN == "FIREFOX")
                {
                    grid.Height = 312;
                }
            }
            catch
            { }

            HederTable.Width = grid.Width;
            HederChart.Width = Chart.Width;


        }
        private void Ini_Chart(Infragistics.WebUI.UltraWebChart.UltraChart e)
        {
            //e.Legend.Location = Infragistics.UltraChart.Shared.Styles.LegendLocation.Left;
            //e.Legend.SpanPercentage = 10;
            //e.Border.Color = Color.White;
            //e.Legend.BorderColor = Color.White;
            //e.Legend.Visible = 1 == 1;

            
           // e.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.SplineAreaChart;

        }

        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            string s = DataProvider.GetQueryText("grid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(s, "Показатель", maintable);
            grid.DataSource = maintable;
        }

        protected void Grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            Pokaz.Value = GetString_( e.Row.Cells[0].Text);
            HederChart.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart_title"), Pokaz.Value, _GetString_(e.Row.Cells[0].Text));
           // HederChart.Text ='"' + Pokaz.Value + '"' + ", " + _GetString_(e.Row.Cells[0].Text);
            Chart.Tooltips.FormatString = "<ITEM_LABEL>, " + _GetString_(e.Row.Cells[0].Text) + " <b><DATA_VALUE:00.##></b>"; 
            Chart.DataBind();
        }

        protected void Chart_DataBinding(object sender, EventArgs e)
        {

            Chart.DataSource = GetDSForChart("Chart");
            //Ini_Chart(Chart);
            Chart.DoughnutChart3D.Labels.FormatString = " <DATA_VALUE:#> ( <PERCENT_VALUE:#0.00>%)";
            

        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {
                // настройка столбцов
                //e.Layout.Bands[0].Columns[0].Hidden = true;

                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth - 20) * 0.7) - 5;

                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.3) - 5;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "### ### ### ##0");
                e.Layout.Bands[0].Columns[1].Header.Style.HorizontalAlign = HorizontalAlign.Center;

            }
            catch
            {
                // ошибка инициализации
            }
        }

        protected void Chart_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
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
    }
}
