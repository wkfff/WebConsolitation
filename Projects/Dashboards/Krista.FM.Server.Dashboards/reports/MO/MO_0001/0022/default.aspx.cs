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
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.UltraChart.Core.Primitives;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.reports.BC_0001_0022
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
                    decText.bounds.X = xNov + (xNov - xOct);
                    decText.SetTextString(year.ToString());
                    e.SceneGraph.Add(decText);
                    break;
                }
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
        private void setFont(int typ, Label lab, WebControl c)
        {
            lab.Font.Name = "arial";
            lab.Font.Size = typ;
            if (typ == 14) { lab.Font.Bold = 1 == 1; };
            if (typ == 10) { lab.Font.Bold = 1 == 1; };
            if (c != null) { lab.Width = c.Width; }
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

        private void conf_Grid(int sizePercent, UltraWebGrid grid)
        {
            //grid.Width = screen_width * sizePercent / 100;
            grid.Bands[0].Columns[0].Header.Style.Wrap = 1 == 1;
            grid.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            // grid.Bands[0].Columns[0].Width = (int)((screen_width * sizePercent / 100) * 0.5);
            for (int i = 1; i < grid.Columns.Count; i++)
            {
                grid.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                grid.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(grid.Bands[0].Columns[i], "### ### ### ###.##");
                grid.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
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
            //chart.SplineAreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8), Color.Black, "<DATA_VALUE:#>", StringAlignment.Far, StringAlignment.Center, 0));
            if (leg)
            {
                chart.Legend.Visible = 1 == 1;
                chart.Legend.Location = Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom;
                chart.Legend.SpanPercentage = 25;

            }
            chart.FunnelChart3D.RadiusMax = 0.5;
            chart.FunnelChart3D.RadiusMin = 0.1;
            chart.FunnelChart3D.OthersCategoryText = "Прочие";
            //chart.ChartType = typ;
            chart.PieChart3D.OthersCategoryText = "Прочие";
            chart.DoughnutChart.OthersCategoryPercent = 1;
            chart.PieChart3D.OthersCategoryPercent = 1;
            chart.DoughnutChart.OthersCategoryText = "Прочие";
            chart.DoughnutChart.RadiusFactor = 100;
            chart.DoughnutChart.InnerRadius = 25;
            chart.Transform3D.XRotation = 120;
            if (typ == Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D) { chart.Transform3D.XRotation = 30; }
            
            chart.Transform3D.YRotation = 0;
            chart.Transform3D.ZRotation = 0;
            chart.PyramidChart.Axis = Infragistics.UltraChart.Shared.Styles.HierarchicalChartAxis.Radius;
            //chart.DeploymentScenario.FilePath = "../../TemporaryImages";
            //chart.DeploymentScenario.ImageURL = "../../TemporaryImages/Chart_#SEQNUM(100).png";
            chart.AreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 9, FontStyle.Bold), Color.Black, "<DATA_VALUE:###,##0.##>", StringAlignment.Far, StringAlignment.Center, 0)); 
        }


        private string _GetString_(string s)
        {
            string res = "";
            int i = 0;
            for (i = s.Length - 1; s[i] != ','; i--) { res = s[i] + res; };

            return res;


        }

      /*  private string GetString_(string s)
        {
            string res = "";
            int i = 0;
            for (i = s.Length - 1; s[i] != ','; i--) ;
            for (int j = 0; j < i; j++)
            {
                res += s[j];
            }
            return res;


        }*/

        string BN="";
        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {

                System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
                BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

                if (!Page.IsPostBack)
                {
                    RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(GT);
                    WebAsyncRefreshPanel2.AddLinkedRequestTrigger(GB);
                    

                   // Request.Browser;
                    //((System.Web.Configuration.HttpCapabilitiesBase) (Request.Browser)).Browser.ToString
                     //(((System.Web.Configuration.HttpCapabilitiesBase)).Browser.ToUpper().IndexOf("IE") >= 0)
                    //{ }

                    Lastdate.Value = ELV(getLastDate("СОЗДАНИЕ УСЛОВИЙ ДЛЯ ДЕЯТЕЛЬНОСТИ УЧРЕЖДЕНИЙ КУЛЬТУРЫ"));
                    GT.DataBind();
                    GB.DataBind();
                    GridActiveRow(GT, 0, true);
                    GridActiveRow(GB, 0, true);

                    Pokaz.Value = GT.Rows[0].Cells[0].Text;
                    Label3.Text ='"' + Pokaz.Value + '"' + ", тысяча экземпляров";
                    Label8.Text = "Динамика показателя";
                    UltraChart1.DataBind();
                    Pokaz.Value = GB.Rows[0].Cells[0].Text;
                    //e.Row.Cells[0].Text;
                    Label4.Text = "Соотношение числа посещений по видам учреждений культурно-досугового типа в " + Pokaz.Value + " году, посещений";

                    UltraChart2.DataBind();
                    Pokaz.Value = GB.Columns[1].Header.Key;
                    Label5.Text = "Темп роста числа посещений учреждений " + Pokaz.Value;
                    UltraChart3.DataBind();
                    DataTable DT = GetDSForChart("TT");
                    Label7.Text = "По состоянию на <b>" + Lastdate.Value + "</b> год: <br/>";
                    
                    Texto.Text =
        "1. Число общедоступных (публичных) библиотек - <b>" + DT.Rows[0].ItemArray[1].ToString() + "</b> ед. Из них:<br/>" +
             "   &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp     Детские библиотеки - <b>" + DT.Rows[1].ItemArray[1].ToString() + "</b> ед.<br/>";

                    Texto.Text += "   &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp     Компьютерные библиотеки - <b>" + DT.Rows[2].ItemArray[1].ToString() + "</b> ед.<br/>" +
        "2. Число учреждений культурно-досугового типа – <b>" + DT.Rows[3].ItemArray[1].ToString() + "</b> ед. Из них:<br/>";
                    for (int i = 7; i < DT.Rows.Count; i++)
                    {
                        Texto.Text += "  &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp      " + DT.Rows[i].ItemArray[0].ToString() + " - <b>" + DT.Rows[i].ItemArray[1].ToString() + "</b> ед. <br/>";
                    };
                    Texto.Text += "3. Число музыкальных и художественных школ – <b>" + DT.Rows[4].ItemArray[1].ToString() + "</b> ед.<br/>" +
        "4. Численность работников общедоступных (публичных) библиотек – <b>" + DT.Rows[5].ItemArray[1].ToString() + " </b> чел.<br/>";
                    Texto.Text += "5. Численность работников сферы культуры и досуга (без аппарата управления) - <b>" + DT.Rows[6].ItemArray[1].ToString() + "</b> чел.";

                    int size = 10;
                   // setFont(size, Label1, GT);
                    //setFont(size, Label2, GB);
                    Label6.Text = "Деятельность учреждений культуры ";
                   //setFont(size, Label3, UltraChart1);


                   // setFont(size, Label4, UltraChart2);

                   // setFont(size, Label5, UltraChart3);


                   // setFont(16, Label6, null);
                    //Label6.Text = "Деятельность учреждений культуры";

                    //setFont(size, Texto, null);
                    CellSet CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("GBE"));
                    for (int i = 0; i < CLS.Cells.Count; i++)
                    {
                        GB.Columns[i + 1].Header.Caption += ", " + CLS.Cells[i].Value.ToString().ToLower();

                    }
                    CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("GTE"));
                    try
                    {
                        for (int i = 0; i < CLS.Cells.Count; i++)
                        {
                            GT.Rows[i].Cells[0].Text += ", " + CLS.Cells[i].Value.ToString().ToLower();

                        }
                    }
                    catch { };


                }


            }
            catch { }
            ///Label1.Text = Request.Browser.ToString();
            ///

            //System.Web.HttpBrowserCapabilities C = Request.Browser;
            //Label1.Text = ((System.Web.Configuration.HttpCapabilitiesBase)C).Browser.ToUpper();
        }

        protected void GT_InitializeLayout(object sender, LayoutEventArgs e)
        {
            conf_Grid(45, GT);
            double GW = screen_width * 42 / 100;
            if (BN == "IE")
            {
                e.Layout.Bands[0].Columns[0].Width = (int)(GW * 0.7);
                e.Layout.Bands[0].Columns[1].Width = (int)(GW * 0.28);
                Label1.Width = (int)(GW - 50);
                Label2.Width = (int)(GW - 50);
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                e.Layout.Bands[0].Columns[0].Width = (int)(GW * 0.7);
                e.Layout.Bands[0].Columns[1].Width = (int)(GW * 0.28);
                Label1.Width = (int)(GW - 50);
                Label2.Width = (int)(GW - 50);
                //BN = "FIREFOX";
            }
            if (BN == "FIREFOX")
            {
                e.Layout.Bands[0].Columns[0].Width = (int)(GW * 0.7);
                e.Layout.Bands[0].Columns[1].Width = (int)(GW * 0.25);
                Label1.Width = (int)(GW - 50);
                Label2.Width = (int)(GW - 50);
            }}

        protected void GT_DataBinding(object sender, EventArgs e)
        {
            GT.DataSource = GetDSForChart("GT");
        }

        protected void GB_DataBinding(object sender, EventArgs e)
        {
            GB.DataSource = GetDSForChart("GB");
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            UltraChart1.DataSource = GetDSForChart("CT");
          
          int  width = 50;
           if (BN == "IE")
            {
            width = 50;
            }
            if (BN == "APPLEMAC-SAFARI")
           {
            width  =50;

                //BN = "FIREFOX";
            }
            if (BN == "FIREFOX")
            {
                width = 50;
                if (screen_width == 1024)
                {
                    width = 49;
                }
            }



            conf_Chart(width, UltraChart1, false, Infragistics.UltraChart.Shared.Styles.ChartType.BarChart3D);
            
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>, "+Ed.ToLower() + " <b><DATA_VALUE:00.##></b>"; 
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            UltraChart2.DataSource = GetDSForChart("CB1");
            int width = 50;
            if (BN == "IE")
            {
                width = 50;
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                width = 50;

                //BN = "FIREFOX";
            }
            if (BN == "FIREFOX")
            {
                width = 50;
                if (screen_width == 1024)
                {
                    width = 49;
                }
            }
            conf_Chart(width, UltraChart2, 1 == 1, Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D);
            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>, посещений"  + " <b><DATA_VALUE:00.##></b>";
            UltraChart2.PieChart3D.PieThickness = 23;

    }

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            try
            { 
                DataTable DT = GetDSForChart("CB2");
                decimal res = 0;
                object[] MO = new object[DT.Columns.Count];

                MO[0] = DT.Rows[0].ItemArray[0];

                for (int i = DT.Columns.Count - 1; i > 1; i--)
                {
                    MO[i] = ((decimal)DT.Rows[0].ItemArray[i] / (decimal)DT.Rows[0].ItemArray[i - 1]) * 100;
                    //DT.Rows[0].ItemArray[i] = res.ToString();

                }
                MO[1] = 0;
                DT.Rows[0].ItemArray = MO;

                UltraChart3.DataSource = DT;

                if (BN == "IE")
                {
                    conf_Chart(96, UltraChart3, false, Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart);
                }
                if (BN == "APPLEMAC-SAFARI")
                {
                    conf_Chart(96, UltraChart3, false, Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart);
                    //BN = "FIREFOX";
                }
                if (BN == "FIREFOX")
                {
                    conf_Chart(97, UltraChart3, false, Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart);
                    
                }

            }
            catch
            { }


        }
        string Ed = "тысяча экземпляров";
        protected void GT_ActiveRowChange(object sender, RowEventArgs e)
        {
            Ed = _GetString_(e.Row.Cells[0].Text);
            Pokaz.Value = GetString_( e.Row.Cells[0].Text);
            Label3.Text ='"'+ Pokaz.Value+'"'+", "+Ed;
            UltraChart1.DataBind();

        }

        protected void GB_ActiveRowChange(object sender, RowEventArgs e)
        {


        }

        protected void GB_Click(object sender, ClickEventArgs e)
        {
            int CellIndex = 0;
            try
            {
                CellIndex = e.Cell.Column.Index;
            }
            catch
            {
                CellIndex = e.Column.Index;
            }

            Pokaz.Value = GB.Rows[e.Cell.Row.Index].Cells[0].Text;

            Label4.Text = "Соотношение числа посещений по видам учреждений культурно-досугового типа в " + Pokaz.Value + " году, посещений";

            UltraChart2.DataBind();

            if (e.Cell.Column.Index > 0)
                Pokaz.Value = GB.Columns[CellIndex].Header.Key;
            else
                Pokaz.Value = GB.Columns[CellIndex + 1].Header.Key;

            Label5.Text = "Темп роста числа посещений учреждений " + Pokaz.Value;
            UltraChart3.DataBind();
        }

        protected void GB_InitializeLayout(object sender, LayoutEventArgs e)
        {
            //e.Layout.RowSelectorStyleDefault.Width = 1;
            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.ColorLeft = Color.Gray;
            conf_Grid(45, GB);
            double GW = screen_width * 39.3 / 100; ;
            if (BN == "IE")
            {
                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = (int)(GW / e.Layout.Bands[0].Columns.Count);
                }
                
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = (int)((GW+20) / e.Layout.Bands[0].Columns.Count);
                }
               // BN = "FIREFOX";
            }
            if (BN == "FIREFOX")
            {
                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = (int)((GW+10) / e.Layout.Bands[0].Columns.Count);
                }
            }
                        
        }

        protected void UltraChart3_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void GB_ActiveCellChange(object sender, CellEventArgs e)
        {
            int CellIndex = 0;
            int RowIndex = 0;
            try
            {
                CellIndex = e.Cell.Column.Index;
            }
            catch
            {
                //CellIndex = e.Column.Index;
            }

            try
            {
                RowIndex = e.Cell.Row.Index;
            }
            catch
            {
                //RowIndex = e.Row.Index;
            }


            Pokaz.Value = GB.Rows[RowIndex].Cells[0].Text;

            Label4.Text = "Соотношение числа посещений по видам учреждений культурно-досугового типа в " + Pokaz.Value + " году, посещений";

            UltraChart2.DataBind();

            if (CellIndex > 0)
                Pokaz.Value = GB.Columns[CellIndex].Header.Key;
            else
                Pokaz.Value = GB.Columns[CellIndex + 1].Header.Key;

            Label5.Text = "Темп роста числа посещений учреждений " + Pokaz.Value;
            UltraChart3.DataBind();

        }

    }
}
