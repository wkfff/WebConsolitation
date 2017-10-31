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

namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._011
{
    public partial class Default1 : CustomReportPage
    {
        private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
        private CustomParam Pokaz { get { return (UserParams.CustomParam("Pokaz")); } }
        private CustomParam param { get { return (UserParams.CustomParam("param")); } }
        private CustomParam Lastdate { get { return (UserParams.CustomParam("lastdate")); } }
        private Int32 screen_width { get { return (int)Session["width_size"]; } }
        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }	

        string[] EGTO;
        string[] EGBO;
        static string ed = "Единица";
        private CustomParam GT0Marks;//параметры для грида1
        private CustomParam CT2All_mark;//параметры для диаграмм 1-3
        private CustomParam CC2Marks;//параметры для диаграммы 4
        private CustomParam GB2Marks;//параметры для грида 2
        private CustomParam CB2AllMarks;//параметры для диаграмм 5-6
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
        private void setFont(int typ, Label lab,int percent)
        {
            lab.Font.Name = "arial";
            lab.Font.Size = typ;
            if (typ == 14) { lab.Font.Bold = 1 == 1; };
            if (typ == 10) { lab.Font.Bold = 1 == 1; };
            if (percent != 0) { lab.Width = (screen_width) * percent / 100; ; }
            if (typ == 18) { lab.Font.Bold = 1 == 1; };
            if (typ == 16) { lab.Font.Bold = 1 == 1; };
            //lab.Height = 40;
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
           // grid.Width = screen_width * sizePercent / 100;
            grid.Bands[0].Columns[0].Header.Style.Wrap = 1 == 1;
            grid.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
           // grid.Bands[0].Columns[0].Width = (int)((screen_width * sizePercent / 100) * 0.5);
            grid.Bands[0].Columns[0].Header.Style.VerticalAlign = VerticalAlign.Top;
            for (int i = 1; i < grid.Columns.Count; i++)
            {
                grid.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                grid.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(grid.Bands[0].Columns[i], "### ### ### ###.##");
                grid.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                grid.Bands[0].Columns[i].Header.Style.VerticalAlign = VerticalAlign.Top;
                
            }
            //grid.DisplayLayout.CellClickActionDefault = CellClickAction.Select;
            //grid.DisplayLayout.HeaderTitleModeDefault = CellTitleMode.Always;
            //grid.DisplayLayout.RowSelectorsDefault = RowSelectors.Yes;
            grid.DisplayLayout.GroupByBox.Hidden = 1 == 1;
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
                chart.Legend.SpanPercentage = 35;

            }
            chart.FunnelChart3D.RadiusMax = 0.5;
            chart.FunnelChart3D.RadiusMin = 0.1;
            chart.FunnelChart3D.OthersCategoryText = "Прочие";
            chart.ChartType = typ;
            chart.PieChart3D.OthersCategoryText = "Прочие";
            chart.DoughnutChart.OthersCategoryPercent = 1;
            chart.PieChart3D.OthersCategoryPercent = 1;
            chart.DoughnutChart.OthersCategoryText = "Прочие";
            chart.DoughnutChart.RadiusFactor = 100;
            chart.DoughnutChart.InnerRadius = 25;
            chart.Transform3D.XRotation = 110;
            chart.Transform3D.YRotation = 0;
            chart.Transform3D.ZRotation = 0;
            chart.PyramidChart.Axis = Infragistics.UltraChart.Shared.Styles.HierarchicalChartAxis.Radius;
            chart.DeploymentScenario.FilePath = "../../../../TemporaryImages";
            chart.DeploymentScenario.ImageURL = "../../../../TemporaryImages/Chart_#SEQNUM(100).png";
            if (typ == Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart) { chart.AreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8), Color.Black, "<DATA_VALUE:#>", StringAlignment.Far, StringAlignment.Center, 0)); };
            if (typ == Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D) { chart.Transform3D.Scale = 90; chart.Transform3D.XRotation = 30; }
            if (typ == Infragistics.UltraChart.Shared.Styles.ChartType.PyramidChart3D) { chart.Transform3D.Scale = 80; chart.Transform3D.Perspective = 30; chart.Transform3D.XRotation = 145; };
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
        double coef = 1;
        string BN = "";

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                
                GT0Marks = UserParams.CustomParam("GT0Marks");
                CT2All_mark = UserParams.CustomParam("CT2All_mark");
                CC2Marks = UserParams.CustomParam("CC2Marks");
                GB2Marks = UserParams.CustomParam("GB2Marks");
                CB2AllMarks = UserParams.CustomParam("CB2AllMarks");
                GT0Marks = ForMarks.SetMarks(GT0Marks, ForMarks.Getmarks("GT0_mark_"), true);
                GB2Marks=ForMarks.SetMarks(GB2Marks,ForMarks.Getmarks("GB2_mark_"),true);
                CT2All_mark.Value = RegionSettingsHelper.Instance.GetPropertyValue("CT2All_mark");
                CC2Marks = ForMarks.SetMarks(CC2Marks, ForMarks.Getmarks("CC2_mark_"), true);
                CB2AllMarks = ForMarks.SetMarks(CB2AllMarks, ForMarks.Getmarks("CB2All_mark_"), true);
                if (screen_width == 1024)
                { 
                    
                    coef = 0.95;
                }
                System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
                BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();



                if (!Page.IsPostBack)
                {
                    RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;

                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(GT0);
                    WebAsyncRefreshPanel1.AddRefreshTarget(CT1);
                    WebAsyncRefreshPanel1.AddRefreshTarget(Label2);
                    WebAsyncRefreshPanel2.AddRefreshTarget(CT2);
                    WebAsyncRefreshPanel2.AddRefreshTarget(Label3);
                    WebAsyncRefreshPanel3.AddRefreshTarget(CT3);
                    WebAsyncRefreshPanel3.AddRefreshTarget(Label4);
                    WebAsyncRefreshPanel4.AddLinkedRequestTrigger(GList);
                    WebAsyncRefreshPanel5.AddLinkedRequestTrigger(GB);

                    GList.Width = (int)((screen_width - 55) * 0.33);
                    


                    Lastdate.Value = ELV(getLastDate(RegionSettingsHelper.Instance.GetPropertyValue("way_last_year_mark")));
                    //DropDownList1.Items.Clear();
                    //  DropDownList1.Items.Add("Количество зданий (по материалу стен)");
                    //   DropDownList1.Items.Add("Площадь зданий (по материалу стен)");
                    //   DropDownList1.Items.Add("Количество зданий (по годам возведения)");
                    //   DropDownList1.Items.Add("Площадь зданий (по годам возведения)");
                    //    DropDownList1.Items.Add("Количество зданий (по проценту износа)");
                    //   DropDownList1.Items.Add("Площадь зданий (по проценту износа)");

                }


              
                GT0.DataBind();
                GB.DataBind();
                if (BN == "FIREFOX")
                {
                    GB.Height = 323;
                    GList.Height = 323;
                    GList.DisplayLayout.ScrollBar = Infragistics.WebUI.UltraWebGrid.ScrollBar.Never;
                }
                Label9.Text = RegionSettingsHelper.Instance.GetPropertyValue("page_title");
                GridActiveRow(GB, 0, true);
                //GT0.Columns[GT0.Columns.Count - 1].Selected = true;
                //setFont(16, Label9, 0);
                int s = 10;
                //setFont(s, Label1, 0);

                //setFont(s, Label2, 0);
                //Label2.Width = screen_width * 25 / 100;
                //setFont(s, Label3, (int)(30 * coef));
                //setFont(s, Label4, (int)(30 * coef));
               // setFont(s, Label5, 0);

               // setFont(s, Label6, 0);
                ///setFont(s, Label7, (int)(30 * coef));
                //setFont(s, Label8, (int)(30 * coef));
                //Label6.Width = screen_width * 25 / 100;
                // DropDownList1.Width = (int)((screen_width * 0.33)-80);
 
                //Label10.Text = "Выберите необходимый показатель";
                Label10.Text = RegionSettingsHelper.Instance.GetPropertyValue("grid2_title");


                CellSet CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("GT2PR"));
                EGTO = new string[CLS.Cells.Count];
                for (int i = 0; i < CLS.Cells.Count; i++)
                {
                    EGTO[i] = CLS.Cells[i].Value.ToString().ToLower();
                    GT0.Rows[i].Cells[0].Text += ", " + EGTO[i];

                }
                CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("GB2E"));
                EGBO = new string[CLS.Cells.Count];

                for (int i = 0; i < CLS.Cells.Count; i++)
                {
                    EGBO[i] = CLS.Cells[i].Value.ToString().ToLower();
                    GB.Columns[i + 1].Header.Caption += ", " + EGBO[i];

                }
                GList.DataBind();

                GT0.Columns[GT0.Columns.Count - 1].Selected = true;
            }
            catch { }
            finally
            {
                Pokaz.Value = "Количество зданий (по материалу стен)";

                CC.DataBind();

                Pokaz.Value = GT0.Columns[1].Header.Key;
                CT1.DataBind();
                CT2.DataBind();
                CT3.DataBind();

                Pokaz.Value = GB.Rows[0].Cells[0].Text;
                CB1.DataBind();
                CB2.DataBind();
                Label5.Width = CC.Width;
            };

        }

        protected void GT0_DataBinding(object sender, EventArgs e)
        {
            GT0.DataSource = GetDSForChart("GT2");
            Label1.Text = RegionSettingsHelper.Instance.GetPropertyValue("grid1_title");
        }


        protected void GB_DataBinding(object sender, EventArgs e)
        {
            GB.DataSource = GetDSForChart("GB2");
            Label6.Text = RegionSettingsHelper.Instance.GetPropertyValue("grid3_title");
        }

        protected void CT1_DataBinding(object sender, EventArgs e)
        {
            param.Value = "Количество жилых зданий";
            CT1.DataSource = GetDSForChart("CT2ALL");

            Label2.Text = param.Value + " на " + Pokaz.Value + " год, " + EGTO[0];
            CT1.Tooltips.FormatString = "<ITEM_LABEL>, "+ EGTO[0]+" " + "<b><DATA_VALUE:00.##></b>";
            CT1.Transform3D.Perspective = 30;
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
               conf_Chart((int)(size*coef), CT1, true, Infragistics.UltraChart.Shared.Styles.ChartType.PyramidChart3D); 
        }

        protected void CT2_DataBinding(object sender, EventArgs e)
        {
            param.Value = "Количество проживающих в жилых зданиях";
            CT2.DataSource = GetDSForChart("CT2ALL");
            
            Label3.Text = param.Value + " на " + Pokaz.Value + " год, " + EGTO[1];
            CT2.Tooltips.FormatString = "<ITEM_LABEL>, " + EGTO[1] + " " + "<b><DATA_VALUE:00.##></b>";
            int size = 31;
            if (BN == "IE")
            {
                size = 31;
            }
            if (BN == "FIREFOX")
            {
                size = 31;
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                size = 31;
            }
            conf_Chart((int)(size * coef), CT2, true, Infragistics.UltraChart.Shared.Styles.ChartType.PyramidChart);
        }

        protected void CT3_DataBinding(object sender, EventArgs e)
        {
            param.Value = "Общая площадь жилых помещений";
            CT3.DataSource = GetDSForChart("CT2ALL");
            
            Label4.Text = param.Value + " на " + Pokaz.Value + " год, " + EGTO[2];
            CT3.Tooltips.FormatString = "<ITEM_LABEL>, " + EGTO[2] + "" + "<b><DATA_VALUE:00.##></b>";
            int size = 31;
            if (BN == "IE")
            {
                size = 31;
            }
            if (BN == "FIREFOX")
            {
                size = 31;
                
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                size = 31;
            }
            conf_Chart((int)(size * coef), CT3, true, Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D);
        }

        protected void CC_DataBinding(object sender, EventArgs e)
        {
            CC.DataSource = GetDSForChart("CC2");
            
            Label5.Text = '"' + Pokaz.Value + '"' + ", " + ed.ToLower();
            Label11.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart4_title1"); 
             CC.Tooltips.FormatString = "<ITEM_LABEL>, "+ed.ToLower() + " <b><DATA_VALUE:00.##></b>";
             int size = 62;
             if (BN == "IE")
             {
                 size = 63;             
             }
             if (BN == "FIREFOX")
             {
                 size = 63;
                 if (screen_width == 1024)
                 {
                     size = 65;
                 }
             }
             if (BN == "APPLEMAC-SAFARI")
             {
                 size = 63;             
             }
             conf_Chart((int)(size * coef), CC, true, Infragistics.UltraChart.Shared.Styles.ChartType.CylinderStackColumnChart3D);

        }

        protected void CB1_DataBinding(object sender, EventArgs e)
        {
            param.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart5_title");
           CB1.DataSource = GetDSForChart("CB2ALL");
           
           Label7.Text = Label3.Text = param.Value + " на " + Pokaz.Value + " год, "+EGBO[0];
           CB1.Tooltips.FormatString = "<ITEM_LABEL>, "+EGBO[0] + " <b><DATA_VALUE:00.##></b>";
           int size = 30;
            if (BN=="IE")
            {
                size = 31; 
            }
            if (BN == "FIREFOX")
            {
                size = 31; 
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                size = 31; 
            }
            conf_Chart((int)(size * coef), CB1, true, Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart);
        }

        protected void CB2_DataBinding(object sender, EventArgs e)
        {
            param.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart6_title");
            CB2.DataSource = GetDSForChart("CB2ALL");

            Label8.Text = Label3.Text = param.Value + " на " + Pokaz.Value + " год, " + EGBO[1];
            CB2.Tooltips.FormatString = "<ITEM_LABEL>, " + EGBO[0] + " " + "<b><DATA_VALUE:00.##></b>";
            int size = 30;
            if (BN == "IE")
            {
                size = 31; 
            }
            if (BN == "FIREFOX")
            {
                size = 31; 
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                size = 31; 
            }
            conf_Chart((int)(size * coef), CB2, true, Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart);
        }

        protected void GT0_Click(object sender, ClickEventArgs e)
        {
            int CellIndex;
            if (e.Cell != null)
                CellIndex = e.Cell.Column.Index;
            else
                CellIndex = e.Column.Index;

            if (CellIndex == 0)
                CellIndex++;
            Pokaz.Value = GT0.Columns[CellIndex].Key;
            CT1.DataBind();
            CT2.DataBind();
            CT3.DataBind();
        }

        protected void GB_ActiveRowChange(object sender, RowEventArgs e)
        {
            Pokaz.Value = e.Row.Cells[0].Text;
            CB1.DataBind();
            CB2.DataBind();
        }

        protected void WebCombo1_SelectedRowChanged(object sender, Infragistics.WebUI.WebCombo.SelectedRowChangedEventArgs e)
        {
            Pokaz.Value = e.Row.Cells[0].Text;
            CC.DataBind();
        }

        protected void GT0_InitializeLayout(object sender, LayoutEventArgs e)
        {
            conf_Grid(95, GT0);
            e.Layout.RowSelectorsDefault = RowSelectors.No;


            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.ColorLeft = Color.Gray;
            double GW = screen_width * 95 / 100;
            GT0.Columns[0].Width = (int)(GW * 0.50 * coef);
            GT0.Columns[1].Width = (int)(GW * 0.25 * coef);
            GT0.Columns[2].Width = (int)(GW * 0.25 * coef);
            if (BN == "IE")
            {
                GT0.Columns[0].Width = (int)(GW * 0.50 * coef);
                GT0.Columns[1].Width = (int)(GW * 0.25 * coef);
                GT0.Columns[2].Width = (int)(GW * 0.25 * coef);
            }
            if (BN == "FIREFOX")
            {
                GT0.Columns[0].Width = (int)(GW * 0.50 * coef);
                GT0.Columns[1].Width = (int)(GW * 0.25 * coef);
                GT0.Columns[2].Width = (int)(GW * 0.25 * coef); 
                if (screen_width == 1024)
                {
                    GT0.Columns[0].Width = (int)(GW * 0.50 * coef);
                    GT0.Columns[1].Width = (int)(GW * 0.25 * coef);
                    GT0.Columns[2].Width = (int)(GW * 0.25 * coef);
                }
                
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                GT0.Columns[0].Width = (int)(GW * 0.50 * coef);
                GT0.Columns[1].Width = (int)(GW * 0.25 * coef);
                GT0.Columns[2].Width = (int)(GW * 0.25 * coef);
            }
        }

        protected void GB_InitializeLayout(object sender, LayoutEventArgs e)
        {
            conf_Grid(33, GB);
            //Label10.Width = (int)GB.DisplayLayout.FrameStyle.Width.Value - 5;
            double GW = screen_width * 32 / 120; ;
            e.Layout.Bands[0].Columns[0].Width = (int)((GW * coef) / 3);
            e.Layout.Bands[0].Columns[1].Width = (int)((GW * coef) / 3);
            e.Layout.Bands[0].Columns[2].Width = (int)((GW * coef) / 3);
            if (BN == "IE")
            {
                e.Layout.Bands[0].Columns[0].Width = (int)((GW * coef) / 3);
                e.Layout.Bands[0].Columns[1].Width = (int)((GW * coef) / 3);
                e.Layout.Bands[0].Columns[2].Width = (int)((GW * coef) / 3);
            }
            if (BN == "FIREFOX")
            {
                e.Layout.Bands[0].Columns[0].Width = (int)((GW * coef) / 3);
                e.Layout.Bands[0].Columns[1].Width = (int)((GW * coef) / 3);
                e.Layout.Bands[0].Columns[2].Width = (int)((GW * coef) / 3);
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                e.Layout.Bands[0].Columns[0].Width = (int)((GW * coef) * 0.35);
                e.Layout.Bands[0].Columns[1].Width = (int)((GW * coef) * 0.35);
                e.Layout.Bands[0].Columns[2].Width = (int)((GW * coef) * 0.35);
            }
            //GB.Columns[0].Header.Style.Wrap = 1 == 1;
            //GB.Width = CB1.Width;
            e.Layout.RowSelectorStyleDefault.Width = 15;
        }



        protected void WebImageButton2_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
        {
            Response.Redirect("default.aspx");
        }

        protected void WebImageButton1_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
        {

        }

        protected void CT1_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void GList_DataBinding(object sender, EventArgs e)
        {
            //setFont(10, Label10, 0);
           // DropDownList1.Items.Add("Количество зданий (по материалу стен)");
            //DropDownList1.Items.Add("Площадь зданий (по материалу стен)");
           // DropDownList1.Items.Add("Количество зданий (по годам возведения)");
           // DropDownList1.Items.Add("Площадь зданий (по годам возведения)");
           // DropDownList1.Items.Add("Количество зданий (по проценту износа)");
           // DropDownList1.Items.Add("Площадь зданий (по проценту износа)");
            GList.Rows.Clear();
            GList.Columns.Clear();
           GList.Columns.Add("Показвтель");
           GList.Columns[0].Header.Caption = "Показатель";

           double tempWidth = GList.Width.Value - 14;
           GList.Bands[0].Columns[0].Width = (int)((tempWidth - 20) * 1) - 5;

            
            object[] val = { RegionSettingsHelper.Instance.GetPropertyValue("GList_mark_1") };
                UltraGridRow Row1 = new UltraGridRow(val);
                Row1.Cells[0].Text = val[0].ToString();
                GList.Rows.Add(Row1);
                val[0] = RegionSettingsHelper.Instance.GetPropertyValue("GList_mark_2");
                UltraGridRow Row2 = new UltraGridRow(val);
                Row2.Cells[0].Text = val[0].ToString();
                GList.Rows.Add(Row2);
                val[0] = RegionSettingsHelper.Instance.GetPropertyValue("GList_mark_3");
                UltraGridRow Row3 = new UltraGridRow(val);
                Row3.Cells[0].Text = val[0].ToString();
                GList.Rows.Add(Row3);
                val[0] = RegionSettingsHelper.Instance.GetPropertyValue("GList_mark_4");
                UltraGridRow Row4 = new UltraGridRow(val);
                Row4.Cells[0].Text = val[0].ToString();
                GList.Rows.Add(Row4);
                val[0] = RegionSettingsHelper.Instance.GetPropertyValue("GList_mark_5");
                UltraGridRow Row5 = new UltraGridRow(val);
                Row5.Cells[0].Text = val[0].ToString();
                GList.Rows.Add(Row5);
                val[0] = RegionSettingsHelper.Instance.GetPropertyValue("GList_mark_6");
                UltraGridRow Row6 = new UltraGridRow();
                Row6.Cells[0].Text = val[0].ToString();
                GList.Rows.Add(Row6);
                //Ultraweb

                GList.Bands[0].Columns[0].Width = 500;
                GList.Columns[0].Width = 500;          
               // Pokaz.Value = DropDownList1.SelectedValue;
                //Pokaz.Value = WebCombo1.Rows[0].Cells[0].Text;

                



        }

        protected void GList_ActiveRowChange(object sender, RowEventArgs e)
        {
            Pokaz.Value = e.Row.Cells[0].Text;
            if (e.Row.Index == 5) { CC.Transform3D.Scale = 40; } else { CC.Transform3D.Scale = 60; }
            if ((e.Row.Index == 2) || (e.Row.Index == 0) || (e.Row.Index == 4)) { ed = "Единица"; } else { ed = "Тысяча квадратных метров"; }
            CC.DataBind();
            
        }

        protected void GList_InitializeLayout(object sender, LayoutEventArgs e)
        {

            double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
            //e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
            e.Layout.Bands[0].Columns[0].Width = 330;
            

        }

    }
}
