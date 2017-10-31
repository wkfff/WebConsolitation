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

namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._0230
{
    public partial class Default : CustomReportPage
    {
        private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
        private CustomParam Pokaz { get { return (UserParams.CustomParam("Pokaz")); } }
        private CustomParam Lastdate { get { return (UserParams.CustomParam("lastdate")); } }
        private Int32 screen_width { get { return (int)Session["width_size"]; } }
        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam grid1Marks;
        private CustomParam grid2Marks;
        private CustomParam chart1Marks;
        private CustomParam chart2Marks;
        private CustomParam chart3Marks;
        private CustomParam textMarks;
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
            e.Text = "Нет данных";

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
            if (typ == 16)
            {
                lab.Font.Bold = 1 == 1;

                lab.Font.Size = FontUnit.Medium;
            };
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
            for (int i = 1; i < grid.Columns.Count; i++)
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
            chart.Transform3D.Scale = 100;
            chart.FunnelChart3D.RadiusMax = 0.5;
            chart.FunnelChart3D.RadiusMin = 0.1;
            chart.FunnelChart3D.OthersCategoryText = "Прочие";
            if (typ != Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart) { chart.ChartType = typ; };
            chart.PieChart3D.OthersCategoryText = "Прочие";
            //if (typ == Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart) { chart.AreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8), Color.Black, "<DATA_VALUE:#>", StringAlignment.Far, StringAlignment.Center, 0)); };
        }
        string GenText()
        {
            CellSet DT = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("TT"));
            Label1.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("text_title"),Lastdate.Value);
            
            
            return ("Число средств массовой информации, находящихся в собственности муниципального образования составило <b>" + DT.Cells[0].Value.ToString() + "</b> единиц, из них<br>&nbsp&nbsp&nbsp телеканалов <b>" + DT.Cells[1].Value.ToString() + "</b>,<br>&nbsp&nbsp&nbsp радиоканалов <b>" + DT.Cells[2].Value.ToString() + "</b>.");


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
        string BN;
        double safari = 1;
        double Coef = 1;
        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
                BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
                if (BN == "IE")
                {
                    Coef = 1.05;
                }
                if (BN == "FIREFOX")
                {
                    Coef = 1.05;
                    safari = 1.01;
                }
                if (BN == "APPLEMAC-SAFARI")
                {
                    Coef = 1.05;
                    safari = 1.023;
                }
                if (screen_width == 1024)
                {
                    if (BN == "FIREFOX")
                    {
                        safari *= 1;
                    }
                    else
                    {
                        safari *= 0.97;
                    }
                }
                if (!Page.IsPostBack)
                {
                    grid1Marks = UserParams.CustomParam("grid1Marks");
                    grid2Marks = UserParams.CustomParam("grid2Marks");
                    chart1Marks = UserParams.CustomParam("chart1Marks");
                    chart2Marks = UserParams.CustomParam("chart2Marks");
                    chart3Marks = UserParams.CustomParam("chart3Marks");
                    textMarks = UserParams.CustomParam("textMarks");
                    grid1Marks = ForMarks.SetMarks(grid1Marks, ForMarks.Getmarks("grid1_mark_"), true);
                    grid2Marks = ForMarks.SetMarks(grid2Marks, ForMarks.Getmarks("grid2_mark_"), true);
                    chart1Marks = ForMarks.SetMarks(chart1Marks, ForMarks.Getmarks("chart1_mark_"), true);
                    chart2Marks = ForMarks.SetMarks(chart2Marks, ForMarks.Getmarks("chart2_mark_"), true);
                    chart3Marks = ForMarks.SetMarks(chart3Marks, ForMarks.Getmarks("chart3_mark_"), true);
                    textMarks = ForMarks.SetMarks(textMarks, ForMarks.Getmarks("text_mark_"), true);
                    RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;

                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(GT);
                    WebAsyncRefreshPanel3.AddLinkedRequestTrigger(GB);

                    
                    Lastdate.Value = ELV(getLastDate(RegionSettingsHelper.Instance.GetPropertyValue("way_last_year_mark")));
                    GT.DataBind();
                    GB.DataBind();
                    Pokaz.Value = GT.Rows[0].Cells[0].Text;
                    LRCT.Text = "Структура телевещания в " + Pokaz.Value + " году, час";
                    LLCT.Text = "Структура радиовещания в " + Pokaz.Value + " году, час";
                    CTR.DataBind();
                    CTL.DataBind();

                    GB.Rows[0].Cells[0].Style.BorderDetails.ColorBottom = Color.White;
                    GB.Rows[2].Cells[0].Style.BorderDetails.ColorBottom = Color.White;
                    Pokaz.Value = "[Газета " + '"' + "Нефтяник приполярья" + '"' + "].[Годовой тираж]";
                    GB.Rows[0].Cells[0].Text = "Газета " + '"' + "Нефтяник ";// +'"' + "Нефтяник приполярья" + '"';
                    GB.Rows[1].Cells[0].Text = "приполярья" + '"';
                    GB.Rows[2].Cells[0].Text = "Газета " + '"' + "Вектор ";// +'"' + "Нефтяник приполярья" + '"';Газета "Вектор информ"])
                    GB.Rows[3].Cells[0].Text = "информ" + '"';
                    GB.Columns[1].Header.Title = "";
                    CB.DataBind();
                    LT.Text = GenText();
                    LGT.Text = RegionSettingsHelper.Instance.GetPropertyValue("grid1_title");
                    LGB.Text = RegionSettingsHelper.Instance.GetPropertyValue("grid2_title");
                    LGT.Width = GT.Width;
                    LGB.Width = GB.Width;
                    LLCT.Width = CTL.Width;
                    LRCT.Width = CTR.Width;
                    LCB.Width = CB.Width;
                    LH.Text = RegionSettingsHelper.Instance.GetPropertyValue("page_title");
                    LCB.Text = GB.Rows[0].Cells[0].Text + GB.Rows[1].Cells[0].Text + ", " + GB.Rows[0].Cells[1].Text.ToLower();
                    int size = 10;
                    //setFont(size, LGT);
                    //setFont(size, LGB);
                    //setFont(size, LRCT);
                    //setFont(size, LLCT);
                    //setFont(size, LGB);
                   // setFont(size, LCB);
                    //setFont(10, LT);
                    //setFont(16, LH);
                    GridActiveRow(GT, 0, true);
                    GridActiveRow(GB, 0, true);
                    CB.Tooltips.FormatString = "<ITEM_LABEL>, экземпляр" + " <DATA_VALUE:00.##>";
                    CTL.Legend.FormatString = "";
                    CTR.Legend.FormatString = "";
                    Label2.Text = "Динамика показателя";
                    CellSet CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("GBE"));
 
                        for (int i = 0; i < CLS.Cells.Count; i++)
                        {
                            GB.Rows[i].Cells[1].Text += ", " + CLS.Cells[i].Value.ToString().ToLower();

                        }
                        CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("GTE"));

                        for (int i = 0; i < CLS.Cells.Count; i++)
                        {
                            GT.Columns[i+1].Header.Caption += ", " + CLS.Cells[i].Value.ToString().ToLower();

                        }
                    //А хызы
                        Label2.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart3_title");

                }
            }
            catch { };
        }

        protected void GT_DataBinding(object sender, EventArgs e)
        {
            GT.DataSource = GetDSForChart("GT");
        }

        protected void GT_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            conf_Grid(95, GT);
            double GW = screen_width * 947 / 1000; ;
            GW *= Coef*safari;
            if (screen_width == 1024)
            {
                if (BN == "FIREFOX")
                {
                    GW *= 0.99;
                }
                if (BN == "APPLEMAC-SAFARI")
                {
                    GW *= 0.9;
                }
            }


            GT.Bands[0].Columns[1].Width = (int)(GW * 20 / 100);
            GT.Bands[0].Columns[2].Width = (int)(GW * 20 / 100);
            GT.Bands[0].Columns[3].Width = (int)(GW * 20 / 100);
            GT.Bands[0].Columns[4].Width = (int)(GW * 20 / 100);
            e.Layout.Bands[0].Columns[0].Header.Caption = "Год";
        }

        protected void GT_ActiveRowChange(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            Pokaz.Value = e.Row.Cells[0].Text;
            LLCT.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart2_title"), Pokaz.Value);
            LRCT.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart1_title"), Pokaz.Value);
            
            //.Tooltips.FormatString = "<ITEM_LABEL>, " + Ed.ToLower() + " <b><DATA_VALUE:00.##></b>"; 
            CTL.DataBind();
            CTR.DataBind();
        }
        DataTable CTgrid_master = new DataTable();
        protected void CT_DataBinding(object sender, EventArgs e)
        {
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("CTL"), "Показатель", CTgrid_master);
            CTL.DataSource = CTgrid_master;
            //CTL.DataSource = GetDSForChart("CTL");
            conf_Chart((int)(45*Coef), CTL, true, Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D);
            CTL.Tooltips.FormatString = "<ITEM_LABEL>, час" + " <DATA_VALUE:00.##>";
        }

        protected void CT_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }
        DataTable CTRgrid_master = new DataTable();
        protected void CTR_DataBinding(object sender, EventArgs e)
        {
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("CTL"), "Показатель", CTRgrid_master);
            CTR.DataSource = CTRgrid_master;
            //CTR.DataSource = GetDSForChart("CTR");
            conf_Chart((int)(45 * Coef), CTR, true, Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D);
            CTR.Tooltips.FormatString = "<ITEM_LABEL>, час" + " <DATA_VALUE:00.##>";
        }

        protected void CTR_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void GB_DataBinding(object sender, EventArgs e)
        {
            GB.Columns.Add("aa", "");
            GB.DataSource = GetDSForChart("GB");
        
        }

        protected void GB_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            conf_Grid(95, GB);
            double GW = screen_width * 86 / 100;
            GW *= Coef*safari;
            if (screen_width == 1024)
            {
                if (BN == "FIREFOX")
                {
                    GW *= 0.90;
                }
                if (BN == "APPLEMAC-SAFARI")
                {
                    GW *= 1;
                }
            }
            GB.Columns[1].Width = (int)(GW * 25 / 100);
            //GB.Columns[0].Width = (int)(GW * 75 / 100);
            //for
            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++) { e.Layout.Bands[0].Columns[i].Width = (int)((screen_width * 11 / 100)*Coef*safari); }

            



        }
        private string _GetString_(string s)
        {
            string res = "";
            int i = 0;
            for (i = s.Length - 1; s[i] != ','; i--) { res = s[i] + res; };

            return res;


        }
        protected void GB_ActiveRowChange(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            //GB.Rows[1].Cells[0].Selected = true;
            //:TODO Сначало надо вывести как то
            //Динамика показателя (Подстановка наименования родительского показателя) (Подстановка наименования выбранного показателя) в (подстановка выбранного года) году, (подстановка ед.изм. выбранного показателя)
            if ((e.Row.Index == 0)||(e.Row.Index == 1))
            {
                Pokaz.Value = "["+GB.Rows[0].Cells[0].Text+GB.Rows[1].Cells[0].Text+"].[" + GetString_(e.Row.Cells[1].Text)+"]";
                LCB.Text =GB.Rows[0].Cells[0].Text + GB.Rows[1].Cells[0].Text + ", " + GetString_(e.Row.Cells[1].Text.ToLower()) + ", " + _GetString_(e.Row.Cells[1].Text); 
            }
            else
            {
                LCB.Text = GB.Rows[2].Cells[0].Text + GB.Rows[3].Cells[0].Text + ", " + GetString_(e.Row.Cells[1].Text.ToLower())+", " + _GetString_(e.Row.Cells[1].Text);;
                Pokaz.Value = "[" + GB.Rows[2].Cells[0].Text + GB.Rows[3].Cells[0].Text + "].[" + GetString_(e.Row.Cells[1].Text) + "]";
            }
            CB.Tooltips.FormatString = "<ITEM_LABEL>, " + _GetString_(e.Row.Cells[1].Text) + " <DATA_VALUE:00.##>";
            CB.DataBind();
           
            //


        }

        protected void CB_DataBinding(object sender, EventArgs e)
        {
            int width = 86;
            CB.DataSource = GetDSForChart("CB");
            if (screen_width == 1024)
            { width = 86; }
            conf_Chart((int)(width*Coef), CB, false, Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart);
            CB.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            CB.Axis.X.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
        }

        protected void CB_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }


        protected void Chart6_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
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
                    //text.labelStyle.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
                    //text.labelStyle.VerticalAlign = StringAlignment.Center;
                    //text.labelStyle.HorizontalAlign = StringAlignment.Center;
                    
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
                    decText.bounds.X = xNov + (xNov - xOct);
                    decText.SetTextString(year.ToString());
                    e.SceneGraph.Add(decText);
                    break;
                }
            }
        }

        protected void CTL_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {

            for (int i = 0; i < CTgrid_master.Rows.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Text textLabel = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(28, 208 + i * 20 - i, 250, 10), CTgrid_master.Rows[i].ItemArray[0].ToString(), new Infragistics.UltraChart.Shared.Styles.LabelStyle(new Font("Microsoft Sans Serif", 8), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Far, Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal));
                e.SceneGraph.Add(textLabel);
            }
        }

        protected void CTR_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {

            for (int i = 0; i < CTRgrid_master.Rows.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Text textLabel = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(28, 208 + i * 20 - i, 250, 10), CTRgrid_master.Rows[i].ItemArray[0].ToString(), new Infragistics.UltraChart.Shared.Styles.LabelStyle(new Font("Microsoft Sans Serif", 8), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Far, Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal));
                e.SceneGraph.Add(textLabel);
            }
        }
    }
}
