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

namespace Krista.FM.Server.Dashboards.reports.BC_0001_0028
{
    public partial class Default : CustomReportPage

    {
        private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
        private CustomParam Pokaz { get { return (UserParams.CustomParam("Pokaz")); } }
        private CustomParam Lastdate { get { return (UserParams.CustomParam("lastdate")); } }
        private Int32 screen_width { get { return (int)Session["width_size"]-50; } }
        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }

        private string _GetString_(string s)
        {
            string res = "";
            int i = 0;
            for (i = s.Length - 1; s[i] != ','; i--) { res = s[i] + res; };

            return res;


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
            //lab.Font.Name = "arial";
            //lab.Font.Size = typ;
            //if (typ == 14) { lab.Font.Bold = 1 == 1; };
            //if (typ == 10) { lab.Font.Bold = 1 == 1; };
            //if (typ == 18) { lab.Font.Bold = 1 == 1; };
            //if (typ == 16)
            //{
            //    lab.Font.Bold = 1 == 1;

            //    lab.Font.Size = FontUnit.Medium;
            //};
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
        private void conf_Grid(int sizePercent, UltraWebGrid grid)
        {
            //grid.Width = screen_width * sizePercent / 100;
            grid.Bands[0].Columns[0].Header.Style.Wrap = 1 == 1;
            grid.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
          //  grid.Bands[0].Columns[0].Width = (int)((screen_width * sizePercent / 100) * 0.75);
            for (int i = 1; i < grid.Columns.Count ; i++)
            {
                grid.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                grid.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(grid.Bands[0].Columns[i], "### ### ### ###.##");
                grid.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
            }
            grid.DisplayLayout.CellClickActionDefault = CellClickAction.RowSelect;
            grid.DisplayLayout.HeaderTitleModeDefault = CellTitleMode.Always;
            grid.DisplayLayout.RowSelectorsDefault = RowSelectors.Yes;
            grid.DisplayLayout.GroupByBox.Hidden = 1 == 1;
            grid.DisplayLayout.NoDataMessage = "Нет данных";
            grid.DisplayLayout.NoDataMessage = "Нет данных";
        }
        private void conf_Chart(int sizePercent, Infragistics.WebUI.UltraWebChart.UltraChart chart, bool leg,Infragistics.UltraChart.Shared.Styles.ChartType typ)
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
            chart.FunnelChart3D.OthersCategoryText = "Прочие";
           // chart.ChartType = typ;
          //  if (typ == Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart) { chart.AreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8), Color.Black, "<DATA_VALUE:#>", StringAlignment.Far, StringAlignment.Center, 0)); };
        
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
        string BN = "";
        double coefW = 1;
        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    if (screen_width == 974)
                    {
                        coefW = 0.99;
                        if (BN == "IE")
                        { coefW = 0.9; }
                        if (BN == "APPLEMAC-SAFARI")
                        { coefW = 0.9; }
                        
                    }
                    RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;

                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(GL);
                    WebAsyncRefreshPanel3.AddLinkedRequestTrigger(GR);

                    System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
                    BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();


                    Lastdate.Value = ELV(getLastDate("ПРЕДПРИНИМАТЕЛИ БЕЗ ОБРАЗОВАНИЯ ЮРИДИЧЕСКОГО ЛИЦА"));
                    GR.DataBind();
                    GL.DataBind();
                    Pokaz.Value = GL.Rows[0].Cells[0].Text;
                    CL.DataBind();
                    CC.DataBind();

                    int size = 10;
                    setFont(size, LBR);
                    setFont(size, LBL);
                    setFont(size, LBC);
                    setFont(size, LTL);
                    setFont(size, LTR);
                    setFont(16, LH);
                    LH.Text = "Деятельность предпринимателей без образования юридического лица ";

                    GridActiveRow(GR, 0, true);
                    GridActiveRow(GL, 0, true);

                   // LBC.Height = LBR.Height = LBL.Height = 48;

                    CellSet CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("GLE"));

                    for (int i = 0; i < CLS.Cells.Count; i++)
                    {
                        GL.Columns[i+1].Header.Caption += ", " + CLS.Cells[i].Value.ToString().ToLower();

                    }

                    CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("GRE"));

                    for (int i = 0; i < CLS.Cells.Count; i++)
                    {
                        GR.Rows[i].Cells[0].Text += ", " + CLS.Cells[i].Value.ToString().ToLower();

                    }
                    Pokaz.Value = GR.Rows[0].Cells[0].Text;
                    CR.DataBind();


                }
            }
            catch { };
            //GR.Height = 230;
            //GL.Height = 150;
        }

        protected void TopGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            conf_Grid(66, GL);
            double GW = screen_width * 63 / 100;
            int SC = 16;
            int add = 0;
            if (BN == "IE")
            {
                SC = 16;
            }
            if (BN == "FIREFOX")
            {
                SC = 16;
                if (coefW != 1) { SC = 17; }
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                SC = 17;
            }
            SC = (int)(SC* coefW);
            e.Layout.Bands[0].Columns[0].Width = (int)(GW) * SC / 100;
            e.Layout.Bands[0].Columns[1].Width = (int)(GW) * SC / 100;
            e.Layout.Bands[0].Columns[2].Width = (int)(GW) * SC / 100;
            e.Layout.Bands[0].Columns[3].Width = (int)(GW) * SC / 100;
            e.Layout.Bands[0].Columns[4].Width = (int)(GW) * SC / 100;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Width = (int)(GW) * SC / 100;
            e.Layout.Bands[0].Columns[0].Header.Caption = "Год";
            LTL.Width = (int)(GW * 0.9*coefW);
      
        }

        protected void TopGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            Pokaz.Value = e.Row.Cells[0].Text;
            CL.DataBind();
            CC.DataBind();
        }

        protected void GR_ActiveRowChange(object sender, RowEventArgs e)
        {
            Pokaz.Value =  e.Row.Cells[0].Text;
            CR.DataBind();
        }

        protected void GL_DataBinding(object sender, EventArgs e)
        {
            LTR.Text = "Основные показатели деятельности ПБОЮЛ в "+Lastdate.Value+" году";
            GL.DataSource = GetDSForChart("gridleft");
        }
        protected void GR_DataBinding(object sender, EventArgs e)
        {
            LTL.Text = "Объем оборота торговли, работ и услуг ПБОЮЛ";
            GR.DataSource = GetDSForChart("gridright");
        }

        protected void GR_InitializeLayout(object sender, LayoutEventArgs e)
        {
            conf_Grid(33, GR);
            double GW = screen_width * 28 / 100;
            double Coef = 1;
            if (BN == "IE")
            {
                Coef = 1.05;
            }
            if (BN == "FIREFOX")
            {
                Coef = 1.01;
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                Coef = 1.05;
            }
            Coef *= coefW;
            GR.Bands[0].Columns[0].Width = (int)((GW) * 0.70*Coef);
            GR.Bands[0].Columns[1].Width = (int)((GW) * 0.20*Coef);

            LTR.Width = (int)(GW*0.9*coefW);

        }

        protected void CL_DataBinding(object sender, EventArgs e)
        {
            //GL.Columns.
            LBL.Text = "Оборота розничной торговли в " + Pokaz.Value + " году, рубль";
            CL.DataSource = GetDSForChart("chartleft");
            
            CL.Tooltips.FormatString = "<ITEM_LABEL>, рубль" + " " + "<b><DATA_VALUE:00.##></b>";
            int size = 33;
            if (BN == "IE")
            {

                size = 33;
            }
            if (BN == "FIREFOX")
            {

                size = 33;
            }
            if (BN == "APPLEMAC-SAFARI")
            {

                size = 33;
            }
            size = (int)(size* coefW);
            LBL.Width = (int)((screen_width * size*0.9) / 100); ;

            conf_Chart(size, CL, 1 == 1, Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart);
        }

        protected void CL_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void CC_DataBinding(object sender, EventArgs e)
        {
            LBC.Text = "Оборот оптовой торговли в " + Pokaz.Value + " году, рубль";
            CC.DataSource = GetDSForChart("chartceter");
            
            CC.Tooltips.FormatString = "<ITEM_LABEL>, рубль" + " " + "<b><DATA_VALUE:00.##></b>";
            int size = 33;
            if (BN == "IE")
            { size = 33; }
            if (BN == "APPLEMAC-SAFARI")
            { size = 33; }
            if (BN == "FIREFOX")
            { size = 33; }
            size  = (int)(size* coefW);
            LBC.Width = (int)((screen_width * size * 0.9)/100);
 
            conf_Chart(size, CC, 1 == 1, Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart);
        }

        protected void CC_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void CR_DataBinding(object sender, EventArgs e)
        {
            LBR.Text = "" + '"' + GetString_(Pokaz.Value) + '"' + ", " + _GetString_(Pokaz.Value);
            CR.Tooltips.FormatString = "<ITEM_LABEL>, "+_GetString_(Pokaz.Value) + " " + "<b><DATA_VALUE:00.##></b>";
            Pokaz.Value = GetString_(Pokaz.Value);
            CR.DataSource = GetDSForChart("chartright");
            int size = 33;
            if (BN == "IE")
            {
                if (coefW!=1)
                {  size = 34;}else {size = 33;}
                //conf_Chart(size, CR, 1 == 2, Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart);
            }
            if (BN == "FIREFOX")
            {
                size = 34;
                //conf_Chart(size, CR, 1 == 2, Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart);
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                size = 33;
                
            }
            size = (int)(size*coefW);
            LBR.Width = (int)((screen_width * size * 0.9)/100);           
            conf_Chart(size, CR, 1 == 2, Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart);
        }

        protected void CR_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }
    }
}
