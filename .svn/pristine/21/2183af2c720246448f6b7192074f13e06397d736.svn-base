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
using Krista.FM.Server.Dashboards.Common;
using Microsoft.AnalysisServices.AdomdClient;


using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;


using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;


namespace Krista.FM.Server.Dashboards.reports.CTAT_0001
{
    public partial class _default : CustomReportPage
    {
        private CustomParam REGION { get { return (UserParams.CustomParam("REGION")); } }
        private CustomParam DATASOURCE { get { return (UserParams.CustomParam("DATASOURCE")); } }
        private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
        private CustomParam Pokaz { get { return (UserParams.CustomParam("pokaz")); } }
        private CustomParam Lastdate { get { return (UserParams.CustomParam("lastdate")); } }
        private CustomParam Firstyear { get { return (UserParams.CustomParam("firstyear")); } }
        private Int32 screen_width { get { return (int)Session["width_size"] - 50; } }



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
            catch 
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


        public void SetBeautifulChart(UltraChart chart, bool legend, Infragistics.UltraChart.Shared.Styles.ChartType ChartType, int legendPercent, Infragistics.UltraChart.Shared.Styles.LegendLocation LegendLocation, int SizePercent)
        {

            double w = CustomReportConst.minScreenWidth * (0.5);
            chart.Width = (int)w;
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
            if (ChartType == Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart) { chart.AreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8), Color.Black, "<DATA_VALUE:#>", StringAlignment.Far, StringAlignment.Center, 0)); };
            //доделать AXis
            chart.Axis.X.Extent = 20;
            chart.Axis.Y.Extent = 20;
            chart.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:###,##0.##>";
            chart.Axis.Z.Labels.Visible = 1 == 2;
            if (ChartType == Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart3D)
            {
                chart.Transform3D.XRotation = 125;
                chart.Transform3D.YRotation = 0;
                chart.Transform3D.ZRotation = 0;
            }

        }
        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            double Width =  CustomReportConst.minScreenWidth;
            GG.Width = CRHelper.GetGridWidth(Width);
        }            


        public void SetGridColumn(UltraWebGrid grid, double sizePercent)
        {


            double Width = (CustomReportConst.minScreenWidth) * sizePercent / 100;
            double WidthColumn = (Width) / grid.Columns.Count;
            
            for (int i = 0; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].Width = CRHelper.GetColumnWidth(WidthColumn+1);
                    grid.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                    grid.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                
                    grid.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
            }
            //grid.DisplayLayout.GroupByBox.Hidden = 1 == 1;

            grid.Columns[0].Header.Caption = "Год";

            grid.DisplayLayout.NoDataMessage = "В настоящий момент данные отсутствуют";


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

        //protected override void Page_PreLoad(object sender, EventArgs e)
        //{
        //    base.Page_PreLoad(sender, e);
        //}
        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);
        }
        string EdI = "";
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                Page.Title = "Отчёт";
                REGION.Value = "Саратовская область";
                DATASOURCE.Value = "СТАТ Отчетность - Облстат";
                TG.Text = "Площадь городских земель и зеленых насаждений";
                TT.Text = "Основные показатели ресурсов субъекта РФ "+REGION.Value;

                Lastdate.Value = ELV(getLastDate("Общая площадь городских земель"));
                int buf = (int.Parse(Lastdate.Value) - 6);
                Firstyear.Value = buf.ToString();                   
                GG.DataBind();
                BG.DataBind();

                CellSet CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("GE"));
                for (int i = 0; i < CLS.Cells.Count; i++)
                {                    
                    GG.Columns[i + 1].Header.Caption += ", "+CLS.Cells[i].Value.ToString().ToLower();
                }
                
                Pokaz.Value = GG.Columns[1].Header.Key;

                CC.Tooltips.FormatString = "<ITEM_LABEL>, " + _GetString_(GG.Columns[1].Header.Caption) + " " + "<b><DATA_VALUE:00.##></b>";
                SetGridColumn(BG, 44);
                SetGridColumn(GG,97);
                CC.DataBind();
                
                
                setFont(10, TG);
                setFont(10, TC);
                setFont(16, TT);
                SetBeautifulChart(CC, false, Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart3D, 0, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 50);
                
                WebAsyncRefreshPanel1.AddLinkedRequestTrigger(GG);
                WebAsyncRefreshPanel1.AddRefreshTarget(CC);

                GG.Height = 50;
                CC.Height = 550;
                CC.Transform3D.Scale = 100;
                BG.Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.31);
                BG.Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.07);
                BG.Columns[2].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.07);
              
            }
        }
        

        protected void C_DataBinding(object sender, EventArgs e)
        {
            TC.Text = "Динамика показателя " + '"' + Pokaz.Value + '"' + ", " + EdI;
            CC.DataSource = GetDSForChart("C");
        }

        protected void G_DataBinding(object sender, EventArgs e)
        {
            GG.DataSource = GetDSForChart("G");
        }
        protected void GRID_CLIK(object sender, ClickEventArgs e)
        {
            int index = 1;
            if ((e.Cell != null))
            {
                index = e.Cell.Column.Index;
                if (index == 0)
                {
                    index = 1;
                }
                Pokaz.Value = GG.Columns[index].Header.Key;
                EdI = _GetString_(GG.Columns[index].Header.Caption);
                CC.DataBind();
                CC.Tooltips.FormatString = "<ITEM_LABEL>, " + EdI + " " + "<b><DATA_VALUE:00.##></b>";
            };
            if ((e.Column != null))
            {
                EdI = _GetString_(GG.Columns[index].Header.Caption);
                index = e.Column.Index; if (index == 0) { index = 1; } Pokaz.Value = GG.Columns[index].Header.Key; CC.DataBind(); CC.Tooltips.FormatString = "<ITEM_LABEL>, " + EdI + " " + "<b><DATA_VALUE:00.##></b>";
            };
        }

        protected void C_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            GG.Columns[0].CellStyle.BorderDetails.ColorLeft = Color.Gray;
            e.Layout.RowSelectorsDefault = RowSelectors.No;
            e.Layout.CellClickActionDefault = CellClickAction.CellSelect;
        }

        protected void BG_DataBinding(object sender, EventArgs e)
        {
            BG.DataSource = GetDSForChart("BG");
        }

        protected void BG_InitializeLayout(object sender, LayoutEventArgs e)
        {
            BG.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.485);
            Label1.Text = "Национальный парк «Хвалынский»";
            setFont(10, Label1);
        }

    }
}
