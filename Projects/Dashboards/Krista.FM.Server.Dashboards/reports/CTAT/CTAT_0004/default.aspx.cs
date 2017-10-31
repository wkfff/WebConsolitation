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

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.CTAT_0004
{
    public partial class _default : CustomReportPage
    {
        private CustomParam Param1 { get { return (UserParams.CustomParam("1")); } }
        private CustomParam Param2 { get { return (UserParams.CustomParam("2")); } }
        private CustomParam REGION { get { return (UserParams.CustomParam("REGION")); } }
        private CustomParam DATASOURCE { get { return (UserParams.CustomParam("DATASOURCE")); } }
        private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
        private CustomParam Pokaz { get { return (UserParams.CustomParam("pokaz")); } }
        private CustomParam Pokaz2 { get { return (UserParams.CustomParam("pokaz2")); } }
        private CustomParam Lastdate { get { return (UserParams.CustomParam("lastdate")); } }
        private CustomParam Firstyear { get { return (UserParams.CustomParam("firstyear")); } }
        private Int32 screen_width { get { return (int)Session["width_size"] - 50; } }

        private int IndexColumn = 0;
        public static int IndexRow;
        


        private string RealLastDate;




        protected void FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            int xOct = 0;
            int xNov = 0;
            Text decText = null;
            int year = int.Parse(Components.UserComboBox.getLastBlock(Lastdate.Value));
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
                UltraGridRow row = Grid.Rows[index];
                if (active)
                {
                    row.Activate();
                    row.Activated = true;
                    row.Selected = true;
                }
            }
            catch (Exception)
            {
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
        protected void SetErorFonn(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
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
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(way_ly));
                return cs.Axes[1].Positions[0].Members[0].ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public void SetBeautifulChart(UltraChart chart, bool legend, Infragistics.UltraChart.Shared.Styles.ChartType ChartType, int legendPercent, Infragistics.UltraChart.Shared.Styles.LegendLocation LegendLocation, double SizePercent)
        {
            chart.Width = (int)(screen_width * (SizePercent / 100));
            chart.SplineAreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8), Color.Black, "<DATA_VALUE:#>", StringAlignment.Far, StringAlignment.Center, 0));
            if (legend)
            {
                chart.Legend.Visible = 1 == 1;
                chart.Legend.Location = LegendLocation;
                chart.Legend.SpanPercentage = legendPercent;

            }
            chart.FunnelChart3D.RadiusMax = 0.5;
            chart.FunnelChart3D.RadiusMin = 0.1;
            chart.FunnelChart3D.OthersCategoryText = "Прочие";
            chart.ChartType = ChartType;
            chart.PieChart3D.OthersCategoryText = "Прочие";
            
            
            //доделать AXis
            chart.Transform3D.Scale = 75;
            chart.Axis.X.Extent = 10;
            chart.Axis.Y.Extent = 50;
            chart.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:###,##0.##>";
            //chart.Axis.X.
            chart.Axis.Z.Labels.Visible = 1 == 2;
            chart.Axis.X.Margin.Near.Value = 4;
            if (ChartType == Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D) 
                { 
                    chart.Transform3D.ZRotation = 0; 
                    chart.Transform3D.YRotation = 0; 
                    chart.Transform3D.XRotation = 30; 
                    chart.Transform3D.Scale = 90; 
                };
            if (ChartType == Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart) { chart.AreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8, FontStyle.Bold), Color.Black, "<DATA_VALUE:#>", StringAlignment.Far, StringAlignment.Center, 0)); };
        }
        public void SetGridColumn(UltraWebGrid grid, double sizePercent)
        {

            double Width = screen_width * (sizePercent / 100);
            double WidthColumn = Width / grid.Columns.Count;

            for (int i = 0; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].Width = (int)WidthColumn;
                grid.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                grid.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(grid.Bands[0].Columns[i], "### ### ### ###.##");
                grid.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
            }
            grid.DisplayLayout.GroupByBox.Hidden = 1 == 1;
            //grid.DisplayLayout.RowSelectorsDefault = RowSelectors.Yes;
            //grid.DisplayLayout.RowSelectorStyleDefault.Width = 0;
            //
            grid.Columns[0].Header.Caption = "Год";
            //grid.Columns[0].
            grid.DisplayLayout.NoDataMessage = "В настоящий момент данные отсутствуют";
            //grid.OnClick += new UltraWebGrid.OnClick(CLIK);

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

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
        }
        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);
        }

        private string PARAM2 = "Саратовская область";
        private string PARAM3 = "Приволжский ФО";
        string BN = "IE";
        protected override void Page_Load(object sender, EventArgs e)
        {
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

            
            if (!Page.IsPostBack)
                {
                    Page.Title = "Отчёт";
                    TTG.Text = "Среднемесячная начисленная заработная плата по городам и районам";
                    IndexRow = 1;
                    Param1.Value = "СТАТ Отчетность - Облстат";
                    Lastdate.Value = ELV(getLastDate("last_date"));
                    RealLastDate = Lastdate.Value;
                    int buf = 0;
                    buf = (int.Parse(Lastdate.Value) - 6);
                    Firstyear.Value = buf.ToString();
                    TG.DataBind();
                    BG.DataBind();
                    IndexColumn = 1;
                    IndexRow = 0;
                    DropDownList2.DataBind();
                    TC.DataBind();
                    DropDownList2.SelectedIndex = 1;
                    Pokaz2.Value = DropDownList2.SelectedItem.Text;
                    CC.DataBind();
                    BLC.DataBind();
                    BRC.DataBind();
                    TH.Text = "Основные показатели по труду в субъекте РФ "+PARAM2;
                    setFont(18, TH);
                    

                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(TG);
                    WebAsyncRefreshPanel1.AddRefreshTarget(CC);
                    WebAsyncRefreshPanel1.AddRefreshTarget(TC);


                    WebAsyncRefreshPanel3.AddLinkedRequestTrigger(BG);

                    WebAsyncRefreshPanel3.AddRefreshTarget(BLC);

                    WebAsyncRefreshPanel7.AddLinkedRequestTrigger(Button1);

                    WebAsyncRefreshPanel3.LinkedRefreshControlID = WebAsyncRefreshPanel7.ID;
                    WebAsyncRefreshPanel6.LinkedRefreshControlID = WebAsyncRefreshPanel7.ID;
                    //WebAsyncRefreshPanel2.AddLinkedRequestTrigger(DropDownList2);
                    WebAsyncRefreshPanel2.AddLinkedRequestTrigger(Button4);
                    

                    
                  //  TG_InitializeLayout(sender, null);

                    for (int i = 0; i < BG.Rows.Count; i++)
                    {
                        DropDownList1.Items.Add(new ListItem(BG.Rows[i].Cells[0].Text, i.ToString()));
                        BG.Rows[i].Hidden = 1 == 1;
                    }
                    BG.Rows[0].Hidden = 1 == 2;
                    TC.Height = 280;
                    CC.Height = 440;
                }
        }

        protected void TG_DataBinding(object sender, EventArgs e)
        {
            setFont(10, TTG);
            Lastdate.Value = ELV(getLastDate("last_date2"));
            TTG.Text = "Среднемесячная начисленная заработная плата по городам и районам";
            TG.DataSource = GetDSForChart("TG");
            
            
        }

        protected void BG_DataBinding(object sender, EventArgs e)
        {
            setFont(10, TBG);
            TBG.Text = "Среднегодовая численность занятых в экономике по формам собственности, человек";
            BG.DataSource = GetDSForChart("BG");
        }

        protected void TC_DataBinding(object sender, EventArgs e)
        {
            setFont(10, TTC);
            //Lastdate.Value = RealLastDate;
            Pokaz.Value = TG.Rows[IndexRow].Cells[0].Text;
            TTC.Text = "Динамика величины среднемесячной начисленной заработной платы (" + Pokaz.Value + ")";
            TC.DataSource = GetDSForChart("TC");

            SetBeautifulChart(TC, false, Infragistics.UltraChart.Shared.Styles.ChartType.Stack3DColumnChart, 0, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 64);
            TC.Tooltips.FormatString = "<ITEM_LABEL>, рублей " + "<b><DATA_VALUE:00.##></b>";
        }

        protected void CC_DataBinding(object sender, EventArgs e)
        {
            setFont(10, TCC);
            RealLastDate=Lastdate.Value;
            Lastdate.Value = ELV(getLastDate("last_date"));
            Pokaz.Value = TG.Rows[IndexRow].Cells[0].Text;
            TCC.Text = "Структура среднесписочной численности работников по ОКВЭД ("+Pokaz.Value+") за "+Lastdate.Value +" год";
            CC.DataSource = GetDSForChart("CC");
            Lastdate.Value = RealLastDate;


            SetBeautifulChart(CC,true, Infragistics.UltraChart.Shared.Styles.ChartType.CylinderStackBarChart3D, 48, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 64);
            
            CC.Tooltips.FormatString = "<ITEM_LABEL>, человек <br>" + "<b><DATA_VALUE:00.##></b>";    
        }

        protected void BLC_DataBinding(object sender, EventArgs e)
        {
            setFont(10, TBLC);
            
            Pokaz.Value = BG.Rows[IndexRow].Cells[0].Text;
            TBRC.Text = "Динамика среднегодовой численности занятых в экономике "+'"'+Pokaz.Value+'"';

            TBLC.Width = (int)(screen_width * 0.48 - 30);
            SetBeautifulChart(BLC, false, Infragistics.UltraChart.Shared.Styles.ChartType.StackAreaChart, 0, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 51);
        
            BLC.DataSource = GetDSForChart("BLC");

            double max, min;
            CellSet CS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("BLC"));
            max = double.Parse(CS.Cells[0].Value.ToString());
            min = double.Parse(CS.Cells[0].Value.ToString());
            for (int i = 0; i < CS.Cells.Count; i++)
            {
                if (double.Parse(CS.Cells[i].Value.ToString()) > max) { max = double.Parse(CS.Cells[i].Value.ToString()); }
                if (double.Parse(CS.Cells[i].Value.ToString()) < min) { if (double.Parse(CS.Cells[i].Value.ToString()) != 0) { min = double.Parse(CS.Cells[i].Value.ToString()); } }
            }
            BLC.Axis.Y.RangeMax = max;
            BLC.Axis.Y.RangeMin = min;
            BLC.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
                  
        }

        protected void BRC_DataBinding(object sender, EventArgs e)
        {
            setFont(10, TBRC);
            TBRC.Width = BRC.Width;
            Pokaz.Value = BG.Columns[IndexColumn].Header.Key;
            BRC.DataSource = GetDSForChart("BRC");
            TBLC.Text = "Распределение среднегодовой численности занятых в экономике по формам  собственности за " + Pokaz.Value;

            TBRC.Width = (int)(screen_width* 0.48 - 30);
            SetBeautifulChart(BRC, true, Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D, 35, Infragistics.UltraChart.Shared.Styles.LegendLocation.Right, 51);
            BRC.Tooltips.FormatString = "<ITEM_LABEL>, человек " + "<b><DATA_VALUE:00.##></b>";
        }

        protected void TG_ActiveRowChange(object sender, RowEventArgs e)
        {
           IndexRow =  e.Row.Index;
           TC.DataBind();
           CC.DataBind();
        }

        protected void BG_Click(object sender, ClickEventArgs e)
        {
            IndexColumn = e.Cell.Column.Index;
            IndexRow = e.Cell.Row.Index;
            BRC.DataBind();
            BLC.DataBind();
            
        }

        protected void TG_InitializeLayout(object sender, LayoutEventArgs e)
        {

            TG.Height = CRHelper.GetGridHeight(750);
            

            SetGridColumn(TG, 33);
            TG.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            TG.Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.23);
            TG.Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.10);
            TG.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.37);
        }

        protected void BG_InitializeLayout(object sender, LayoutEventArgs e)
        {


            TBG.Width = (int)(screen_width * 0.95  - 30);
            SetGridColumn(BG, 101);
            BG.Columns[0].CellStyle.BorderDetails.ColorLeft = Color.Gray;
            
        }

        protected void TG_ActiveRowChange1(object sender, RowEventArgs e)
        {
            IndexRow = e.Row.Index;
            TC.DataBind();
            CC.DataBind();
        }
        public int activeRow = 0;
        protected void Button1_Click(object sender, EventArgs e)
        {
            //BG.Rows[activeRow].Hidden = 1 == 1;

            IndexRow = int.Parse(DropDownList1.SelectedValue);
            IndexColumn = 1;

            for (int i = 0; i < BG.Rows.Count; i++)
            {
                //DropDownList1.Items.Add(new ListItem(BG.Rows[i].Cells[0].Text, i.ToString()));
                BG.Rows[i].Hidden = 1 == 1;
            }
            BG.Rows[int.Parse(DropDownList1.SelectedValue)].Hidden = 1 == 2;

            BLC.DataBind();
            BRC.DataBind();
            




        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Response.Redirect("0001/default.aspx");
        }

        protected void Button2_Click1(object sender, EventArgs e)
        {
            Response.Redirect("0001/default.aspx");
        }

        protected void DropDownList2_DataBinding(object sender, EventArgs e)
        {

           // DropDownList2.Items.Add("Значение не указано");
            DropDownList2.Items.Add("Крупные и средние предприятия");
            DropDownList2.Items.Add("Малые предприятия");
            DropDownList2.Items.Add("Организации с участием иностранного капитала");
            DropDownList2.Items.Add("Индивидуальные предприниматели");
        }

        protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            Pokaz2.Value = DropDownList2.SelectedItem.Text;
            Pokaz.Value = TG.Rows[IndexRow].Cells[0].Text;
            CC.DataBind();
            TC.DataBind();
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            DropDownList2_SelectedIndexChanged(null, null);
        }

        

    }
}
