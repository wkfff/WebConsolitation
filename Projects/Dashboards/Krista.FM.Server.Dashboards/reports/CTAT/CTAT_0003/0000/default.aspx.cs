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

namespace Krista.FM.Server.Dashboards.reports.CTAT_0003
{
    public partial class _default : CustomReportPage
    {
        private CustomParam REGION { get { return (UserParams.CustomParam("REGION")); } }
        private CustomParam DATASOURCE { get { return (UserParams.CustomParam("DATASOURCE")); } }

        private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
        private CustomParam Pokaz { get { return (UserParams.CustomParam("pokaz")); } }
        private CustomParam Lastdate { get { return (UserParams.CustomParam("lastdate")); } }
        private CustomParam Firstyear { get { return (UserParams.CustomParam("firstyear")); } }
        private CustomParam IS { get { return (UserParams.CustomParam("la")); } }
        private CustomParam IS2 { get { return (UserParams.CustomParam("la")); } }
        private Int32 screen_width { get { return (int)Session["width_size"] - 50; } }
        /// <summary>
        /// массив запросов 
        /// </summary>
        private string[] MST = new string[] { "GT1", "GT2", "GT3", "GT4", "GT5" };
        private string[] MSC = new string[] { "GC1", "GC2", "GC3", "GC4", "GC5" };
        private string[] MSCC = new string[] { "CC1", "CC2", "CC3", "CC4", "CC5" };
        private string[] MSB = new string[] { "GB1", "GB2", "GB3", "GB4", "GB5" };
        string[] GridHeder = { "Число зарегистрированных браков на 1000 населения, промилле (0,1 процента) ", "Число зарегистрированных разводов на 1000 населения, единица", "Естественный прирост (убыль) на 1000 населения, промилле (0,1 процента) ", "Миграционный прирост (убыль), человек", "Численность постоянного населения (на начало года), человек" };
        private static int IndexSelect = 1;
        string[] GridEd = { "промилле (0,1 процента)", "единица", "промилле (0,1 процента)", "человек", "человек" };
        string[] HSE = { "единица", "промилле (0,1 процента)", "человек", "год", "человек" };

        private string RealLastDate;



        private string GetString__(string s, char ch)
        {

            string res = "";
            int i = 0;
            for (i = 1; s[i] != ch; i++) ;
            i++;
            for (int j = i; j < s.Length; j++)
            {
                res += s[j];
            }
            return res;


        }
        private string __GetString(string s, char ch)
        {

            string res = "";
            
            for (int i = 0; s[i] != ch; i++)
            {
                res += s[i];
            }
            return res;


        }
        private string _GetString_(string s, char ch)
        {
            string res = "";
            int i = 0;
            for (i = s.Length - 1; s[i] != ch; i--) { res = s[i] + res; };

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
        public static void setChartErrorFont(Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "В настоящий момент данные отсутствуют";

            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new Font("Verdana", 30);
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
                CellSet cs =  DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("last_date"));
                return cs.Axes[1].Positions[0].Members[0].ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public void SetBeautifulChart(UltraChart chart, bool legend, Infragistics.UltraChart.Shared.Styles.ChartType ChartType, int legendPercent, Infragistics.UltraChart.Shared.Styles.LegendLocation LegendLocation, double SizePercent)
        {
            chart.Width = CRHelper.GetChartWidth((int)(CustomReportConst.minScreenWidth * ( SizePercent/ 100)));
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
            if (ChartType == Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart) { chart.AreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8,FontStyle.Bold), Color.Black, "<DATA_VALUE:#>", StringAlignment.Far, StringAlignment.Center, 0)); };
            
            chart.Axis.X.Extent = 20;
            chart.Axis.Y.Extent = 20;
            chart.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:###,##0.##>";
            chart.Axis.Z.Labels.Visible = 1 == 2;
            chart.DoughnutChart.OthersCategoryPercent = 0;

        }
        public  void SetGridColumn(UltraWebGrid grid, double sizePercent)
        {
            try
            {
                double Width = CustomReportConst.minScreenWidth * (sizePercent / 100);
                double WidthColumn = Width / grid.Columns.Count;

                for (int i = 0; i < grid.Columns.Count; i++)
                {
                    grid.Columns[i].Width = CRHelper.GetColumnWidth(WidthColumn);
                    grid.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                    grid.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                    CRHelper.FormatNumberColumn(grid.Bands[0].Columns[i], "### ### ### ###.##");
                    grid.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                }
                grid.DisplayLayout.GroupByBox.Hidden = 1 == 1;
                grid.Columns[0].CellStyle.BorderDetails.ColorLeft = Color.Gray;
                grid.Columns[0].Header.Caption = "Год";
                grid.DisplayLayout.NoDataMessage = "В настоящий момент данные отсутствуют";
            }
            catch { }

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


        private string PARAM2 = "Саратовская область";
        private string PARAM3 = "Приволжский ФО";

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
        }
        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);
        }


        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender,e);
            try
            {
                if (!Page.IsPostBack)
                {

                    Page.Title = "Отчёт";

                    Lastdate.Value = ELV(getLastDate("s"));
                    RealLastDate = Lastdate.Value;
                    TG_.DataBind();




                    SetGridColumn(TG_, (95.4));


                    TG_.DisplayLayout.RowSelectorsDefault = RowSelectors.No;
                    TG_.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.99);
                    TG_.DisplayLayout.CellClickActionDefault = CellClickAction.CellSelect;

                    for (int i = int.Parse(Lastdate.Value); i >= 1998; i--) { DD_.Items.Add(i.ToString()); }
                    IndexSelect = 0;
                    CC_.DataBind();
                    CG_.DataBind();

                    GB_.DataBind();
                    for (int i = int.Parse(Lastdate.Value); i >= 1998; i--) { DD2_.Items.Add(i.ToString()); }

                    SetGridColumn(CG_, (60));
                    CG_.Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.11);
                    CG_.Columns[3].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.09);


                    IndexSelect = 1;

                    Lastdate.Value = RealLastDate;
                    setFont(10, TCC_);
                    LBC_.DataBind();
                    RBC_.DataBind();
                    CBBC_.DataBind();
                    IndexSelect--;
                    CBC_.DataBind();
                    IndexSelect++;
                    setFont(10, TTG);
                    setFont(10, Label5_);
                    setFont(10, Label3_);
                    setFont(10, Label1_);
                    setFont(10, Label2_);


                    Label3_.Text = GB_.Columns[1].Header.Key + " в " + Lastdate.Value + " году";
                    Label2_.Text = GB_.Columns[1].Header.Key + " в " + Lastdate.Value + " году";
                    Label1_.Text = GB_.Columns[1].Header.Key + "  " + Lastdate.Value + " году";
                    Label5_.Text = GetString_(GB_.Columns[1].Header.Key) + " в " + Lastdate.Value + " году, человек";

                    setFont(10, TTG);
                    setFont(10, Label1_);
                    setFont(10, Label2_);
                    setFont(10, Label3_);
                    setFont(10, Label4_);
                    setFont(10, Label5_);
                    setFont(10, Label6_);
                    setFont(16, TT);


                    WebAsyncRefreshPanel1_.AddLinkedRequestTrigger(TG_);

                    WebAsyncRefreshPanel1_.AddLinkedRequestTrigger(Button1);
                    WebAsyncRefreshPanel1_.AddRefreshTarget(CC_);
                    WebAsyncRefreshPanel1_.AddRefreshTarget(TCC_);

                    WebAsyncRefreshPanel2_.AddLinkedRequestTrigger(CG_);
                    WebAsyncRefreshPanel2_.AddLinkedRequestTrigger(Button2);
                    //WebAsyncRefreshPanel2_.AddRefreshTarget(CBC_);


                    WebAsyncRefreshPanel3_.AddLinkedRequestTrigger(GB_);



                    //WebAsyncRefreshPanel4_.AddRefreshTarget(CBBC_);
                    //WebAsyncRefreshPanel5_.AddRefreshTarget(RBC_);
                    WebAsyncRefreshPanel3_.AddRefreshTarget(LBC_);
                    //WebAsyncRefreshPanel6_.AddRefreshTarget(OneChart_);



                    GB_.Rows[GB_.Rows.Count - 1].Activate();
                    GB_.Rows[GB_.Rows.Count - 1].Activated = 1 == 1;
                    GB_.Rows[GB_.Rows.Count - 1].Selected = 1 == 1;
                }
                //TG_.Rows[0].Hidden = 1 == 1;
                CG_.Rows[0].Hidden = 1 == 1;
                try
                {
                    TG_.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                    TG_.Rows[0].Hidden = 1 == 1;
                }
                catch { }
                try
                {
                    CG_.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                }
                catch { }
                Label7.Text = "Выберите показатель и год для отображения данных на диаграмме. Щёлкните на ячейку пересечения столбца и строки.<br/><br/>";
            }
            catch { }
        }

        protected void G_DataBinding(object sender, EventArgs e)
        {

        }

        protected void TG_DataBinding(object sender, EventArgs e)
        {
            try
            {
                string[] GridHeder = { "Число зарегистрированных браков на 1000 населения, промилле (0,1 процента) ", "Число зарегистрированных разводов на 1000 населения, единица", "Естественный прирост (убыль) на 1000 населения, промилле (0,1 процента) ", "Миграционный прирост (убыль), человек", "Численность постоянного населения (на начало года), человек" };

                CellSet[] MCS = new CellSet[MST.Length];
                DataTable DT = new DataTable();
                //:TODO
                DT.Columns.Add("Територия");
                for (int i = 0; i < MST.Length; i++)
                {
                    string s = DataProvider.GetQueryText(MST[i]);
                    MCS[i] = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(MST[i]));
                    try
                    {
                        string ss =MCS[i].Axes[1].Positions[0].Members[0].Caption;
                    }
                    catch 
                    {
                        MCS[i] = null;
                        
                    }
                    DT.Columns.Add(GridHeder[i]);

                }

                foreach (Position pos in MCS[0].Axes[0].Positions)
                {   // создание списка значений для строки UltraWebGrid
                    object[] values = {
                        //pos.Members[0].MemberProperties[0].Value.ToString(),
                       MCS[0].Axes[0].Positions[pos.Ordinal].Members[0].Caption,

                         MCS[0].Cells[pos.Ordinal].Value,
                         (MCS[1] !=null? MCS[1].Cells[pos.Ordinal].Value : ""),
                         (MCS[2] !=null? MCS[2].Cells[pos.Ordinal].Value : ""),
                         (MCS[3] !=null? MCS[3].Cells[pos.Ordinal].Value : ""),
                         (MCS[4] !=null? MCS[4].Cells[pos.Ordinal].Value : "")
                    };
                    // заполнение строки данными
                    DT.Rows.Add(values);
                } /**/
                TG_.DataSource = DT;
                TTG.Text = "Основные показатели по городам и районам за " + Lastdate.Value + " год (Саратовская область)";
                
            }
            catch { }
        }

        protected void C_DataBinding(object sender, EventArgs e)
        {

        }

        protected void C_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void TG_Click(object sender, ClickEventArgs e)
        {
            Lastdate.Value = DD_.SelectedItem.Text;
            int index = 1;
            if ((e.Cell != null))
            {
                index = e.Cell.Column.Index;
                if (index == 0) { index = 1; }
                //Pokaz.Value = G.Columns[index].Header.Key; 
                TG_.Columns[index].Selected = 1 == 1;
               // e.Cell.Column.Header.Style.ForeColor = Color.Green;
                IndexSelect = index;
                CC_.DataBind();
                
            };
            if ((e.Column != null)) 
                { 
                    //index = e.Column.Index; 
                    //if (index == 0) { index = 1; } 
                    //Pokaz.Value = G.Columns[index].Header.Key;
                    //for (int i = 0; i < TG_.Columns.Count; i++)
                   // {
                      //  TG_.Columns[IndexSelect + 1].Selected = 1 == 2;
                 //   }
                    
                    CC_.DataBind();
               //     TG_.Columns[IndexSelect+1].Selected = 1 == 1;
                };
                TCC_.Text = __GetString(TG_.Columns[IndexSelect].Header.Caption, ',') + " за " + DD_.SelectedItem.Text + " год, " + GridEd[IndexSelect - 1];


                Lastdate.Value = RealLastDate;

        }
        protected void CG_Click(object sender, ClickEventArgs e)
        {
            Lastdate.Value = DD2_.SelectedItem.Text;
            int index = 1;
            if ((e.Cell != null))
            {
                index = e.Cell.Column.Index;
                if (index == 0) { index = 1; }
                //Pokaz.Value = G.Columns[index].Header.Key; 
                IndexSelect = index - 1;
                CBC_.DataBind();
                CG_.Columns[index].Selected = 1 == 1;
                
            };
            if ((e.Column != null))
            {
                //index = e.Column.Index;
                //if (index == 0) { index = 1; }
                //Pokaz.Value = G.Columns[index].Header.Key;
                //IndexSelect = index - 1;
                CBC_.DataBind();
                //CG_.Columns[index].Selected = 1 == 1;
                //e.Column.so
               // WebAsyncRefreshPanel9.LinkedRefreshControlID = null;
            };
            //Label4_.Text = CG_.Columns[index].Header.Caption + " в " + DD2_.SelectedItem.Text + " году, человек";
            //&& (e.Cell.Column.Index > 0)&& (e.Column.Index > 0)
            
            Lastdate.Value = RealLastDate;

        }

        protected void CC_DataBinding(object sender, EventArgs e)
        {
            try
            {
                Lastdate.Value = DD_.SelectedItem.Text;
                CC_.DataSource = GetDSForChart(MST[IndexSelect]);
                double Coef = 1;


                SetBeautifulChart(CC_, false, Infragistics.UltraChart.Shared.Styles.ChartType.ColumnChart, 0, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, (99));
                CC_.Axis.X.Extent = 145;
                CC_.Axis.Y.Extent = 50;
                CC_.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalRightFacing;
                setFont(10, TDD_);
                TCC_.Text = __GetString(TG_.Columns[IndexSelect + 1].Header.Caption, ',') + " за " + DD_.SelectedItem.Text + " год, " + GridEd[IndexSelect];
                CC_.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
                CC_.Tooltips.FormatString = "<DATA_VALUE:### ##.##>";
                IS.Value = (IndexSelect).ToString();
            }
            catch 
            {
                CC_.DataSource = null;
                TG_.DataSource = null;
            }
        }

        protected void CCC_DataBinding(object sender, EventArgs e)
        {
            try
            {
                Lastdate.Value = DD2_.SelectedItem.Text;
                CBC_.DataSource = GetDSForChart(MSCC[IndexSelect]);
                // SetBeautifulChart(CBC, false, Infragistics.UltraChart.Shared.Styles.ChartType.ColumnChart, 0, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 80);

                double Coef = 1;

                SetBeautifulChart(CBC_, false, Infragistics.UltraChart.Shared.Styles.ChartType.BarChart, 0, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, (36));
                CBC_.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
                CBC_.Axis.X.Extent = 50;
                CBC_.Transform3D.Scale = 70;
                //CC_.Axis.Y.Extent = 50;
                CC_.Height = 530;
                CBC_.Height = 346;
                CBC_.Axis.Y.Extent = 135;
                CBC_.Axis.Y.Labels.Visible = 1 == 2;
                CBC_.Axis.Y.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
                CBC_.Axis.Y.Labels.SeriesLabels.Font = new Font("Arial", 8, FontStyle.Bold);
                CBC_.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
                //CBC_.Axis.X.Labels.HorizontalAlign = StringAlignment.Far;

                Label4_.Text = __GetString(CG_.Columns[IndexSelect + 1].Header.Caption, ',') + " в " + Lastdate.Value + " году, " + HSE[IndexSelect];
                Label4_.Width = CBC_.Width;
                CBC_.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Far;
                CBC_.Axis.X.Labels.ItemFormatString = "<DATA_VALUE: ### ### #0.##>";
                IS2.Value = IndexSelect.ToString();
            }
            catch { }
           // CBC_.Data.SwapRowsAndColumns = 1 == 1;
        }

        protected void CG_DataBinding(object sender, EventArgs e)
        {
            try
            {
                string[] HS = { "Число зарегистрированных разводов на 1000 зарегистрированных браков, единица", "Число умерших детей до 1 года на 1000 родившихся живыми, промилле (0,1 процента)", "Миграционный прирост (убыль), человек", "Ожидаемая продолжительность жизни при рождении , год", "Численность постоянного населения (на начало года), человек" };


                CellSet[] MCS = new CellSet[MSC.Length];
                DataTable DT = new DataTable();
                //:TODO
                //Lastdate.Value = "2006";
                DT.Columns.Add("Територия");
                for (int i = 0; i < MSC.Length; i++)
                {
                    string s = DataProvider.GetQueryText(MSC[i]);
                    MCS[i] = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s);
                    try
                    {

                        string ss = MCS[i].Axes[1].Positions[0].Members[0].Caption;
                    }
                    catch
                    {
                        MCS[i] = null;
                    }

                    DT.Columns.Add(HS[i]);
                }

                foreach (Position pos in MCS[0].Axes[0].Positions)
                {
                    // создание списка значений для строки UltraWebGrid
                    object[] values = {
                        //pos.Members[0].MemberProperties[0].Value.ToString(),
                       MCS[0].Axes[0].Positions[pos.Ordinal].Members[0].Caption,

                           (MCS[0] !=null? MCS[0].Cells[pos.Ordinal].Value : ""),
                         (MCS[1] !=null? MCS[1].Cells[pos.Ordinal].Value : ""),
                         (MCS[2] !=null? MCS[2].Cells[pos.Ordinal].Value : ""),
                         (MCS[3] !=null? MCS[3].Cells[pos.Ordinal].Value : ""),
                         (MCS[4] !=null? MCS[4].Cells[pos.Ordinal].Value : "")
                    };
                    // заполнение строки данными
                    DT.Rows.Add(values);
                } /**/
                CG_.DataSource = DT;
                setFont(10, TCG_);
                TCG_.Text = "Основные показатели по субъектам за " + Lastdate.Value + " год (Приволжский ФО)";
                Lastdate.Value = ELV(getLastDate(""));
            }
            catch 
            {
                CG_.DataSource = null;
                CBC_.DataSource = null;
            }
        }
        protected void BG_DataBinding(object sender, EventArgs e)
        {


            string[] LH = { "единица", "человек", "человек", "человек", "человек" };
            CellSet[] MCS = new CellSet[MSC.Length];
            DataTable DT = new DataTable();
            //:TODO
            
            DT.Columns.Add("Год");
            for (int i = 0; i < MSC.Length; i++)
            {
                string s = DataProvider.GetQueryText(MSB[i]);
                MCS[i] = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s);
                DT.Columns.Add(MCS[i].Axes[1].Positions[0].Members[0].Caption + ", " + LH[i]);

            }

            foreach (Position pos in MCS[0].Axes[0].Positions)
            {
                // создание списка значений для строки UltraWebGrid
                object[] values = {
                        //pos.Members[0].MemberProperties[0].Value.ToString(),
                       MCS[0].Axes[0].Positions[pos.Ordinal].Members[0].Caption,

                        MCS[0].Cells[pos.Ordinal].Value,
                         MCS[1].Cells[pos.Ordinal].Value,
                         MCS[2].Cells[pos.Ordinal].Value,
                         MCS[3].Cells[pos.Ordinal].Value,
                         MCS[4].Cells[pos.Ordinal].Value
                    };
                // заполнение строки данными
                DT.Rows.Add(values);
            } /**/
            GB_.DataSource = DT;
            setFont(10, TBG_);
            TBG_.Text = "Основные показатели по населению ("+PARAM2+")";
            //setFont(10, Label7);
        }

        protected void LBC_DataBinding(object sender, EventArgs e)
        {
            //IndexSelect++;


            if (IndexSelect != 1) { LBC_.Visible = 1 == 1; LBC_.DataSource = GetDSForChart("C" + IndexSelect.ToString() + "1"); }
            else
            {
                OneChart_.Visible = 1 == 1; 
                OneChart_.DataBind(); }
                double Coef = 1;

           
                SetBeautifulChart(LBC_, true, Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart, 35, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, (32.7));
                
                LBC_.Tooltips.FormatString = "<ITEM_LABEL> " + "<b><DATA_VALUE:00.##></b>"+", человек "; 
            //IndexSelect--;
              
        }



        protected void CBBC_DataBinding(object sender, EventArgs e)
        {
            //IndexSelect++;

            if (IndexSelect != 1) { CBBC_.Visible = 1 == 1; CBBC_.DataSource = GetDSForChart("C" + IndexSelect.ToString() + "2"); }
            else
            { CBBC_.Visible = 1 == 2; }



            SetBeautifulChart(CBBC_, true, Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart, 35, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom,(32.7));
            CBBC_.Tooltips.FormatString = "<ITEM_LABEL>, человек " + "<b><DATA_VALUE:00.##></b>";
            if (IndexSelect == 4)
            {
                SetBeautifulChart(CBBC_, false, Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart, 35, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, (32.7));
                CBBC_.DoughnutChart.Labels.FormatString = "<ITEM_LABEL> (<PERCENT_VALUE:#0.00>%)";
                CBBC_.Legend.Visible = 1 == 2;
                CBBC_.DoughnutChart.Labels.Font = new Font("Arial", 7);
                CBBC_.DoughnutChart.InnerRadius = 85;
            }
            else
            {
                SetBeautifulChart(CBBC_, true, Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart, 35, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, (32.7));
                CBBC_.DoughnutChart.Labels.FormatString = "<PERCENT_VALUE:#0.00>%";
                CBBC_.Legend.Visible = 1 == 1;
                CBBC_.DoughnutChart.InnerRadius = RBC_.DoughnutChart.InnerRadius;
                //CBBC_.DoughnutChart.Labels.Font = new Font("Arial", 7);
            
            }
            
        }

        protected void RBC_DataBinding(object sender, EventArgs e)
        {
            //IndexSelect++;

            Label1_.Visible = 1 == 1;
            Label2_.Visible = 1 == 1;
            Label3_.Visible = 1 == 1;
            Label5_.Visible = 1 == 1;

            if (IndexSelect != 1) { Label5_.Visible = 1 == 2; RBC_.Visible = 1 == 1; RBC_.DataSource = GetDSForChart("C" + IndexSelect.ToString() + "3"); }
                        else
            { RBC_.Visible = 1 == 2;
            Label1_.Visible = 1 == 2;
            Label2_.Visible = 1 == 2;
            Label3_.Visible = 1 == 2;
        }

        double Coef = 1;


            SetBeautifulChart(RBC_, true, Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart, 35, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, (32.7));
           // IndexSelect--;
            RBC_.Tooltips.FormatString = "<ITEM_LABEL>, человек " + "<b><DATA_VALUE:00.##></b>";
        }

        protected void GB_DblClick(object sender, ClickEventArgs e)
        {

            ///Lastdate.Value = DD.SelectedItem.Text;
            int index = 1;
            if ((e.Cell != null))
            {

                
                Lastdate.Value = e.Cell.Row.Cells[0].Text;
                index = e.Cell.Column.Index;
                
                if (index == 0) { index = 1; }
                
                e.Cell.Row.Activate();
                e.Cell.Row.Activated = 1 == 1;
                e.Cell.Row.Selected = 1 == 1;
                //e.Cell.Column.Selected = 0 == 0;
                
                GB_.Columns[index].Selected = 1 == 1;
                IndexSelect = index;
                CBBC_.DataBind();
                LBC_.DataBind();
                RBC_.DataBind();
                //OneChart_.DataBind();
                


                setFont(10, Label3_);
                setFont(10, Label2_);
                setFont(10, Label1_);
                setFont(10, Label5_);
            //&& (e.Cell.Column.Index > 0)&& (e.Column.Index > 0)
                if (e.Cell.Column.Index != 5)
                {
                    if (e.Cell.Column.Index != 4)
                    {
                        Label3_.Text = GetString_(GB_.Columns[index].Header.Caption) + " в " + Lastdate.Value + " году, человек";
                        Label2_.Text = GetString_(GB_.Columns[index].Header.Caption) + " в " + Lastdate.Value + " году, человек";
                        Label1_.Text = GetString_(GB_.Columns[index].Header.Caption) + " в " + Lastdate.Value + " году, человек";
                        Label5_.Text = GetString_(GB_.Columns[index].Header.Caption) + " в " + Lastdate.Value + " году, человек";
                    }
                    else
                    {
                        Label3_.Text = "Число прибывших" + " в " + Lastdate.Value + " году, человек";
                        Label2_.Text = "Число прибывших" + " в " + Lastdate.Value + " году, человек";
                        Label1_.Text = "Число прибывших" + " в " + Lastdate.Value + " году, человек";
                        Label5_.Text = "Число прибывших" + " в " + Lastdate.Value + " году, человек";

                    }
                }
                else 
                {
                    Label3_.Text = "Численность постоянного населения на начало " + Lastdate.Value + "года, человек";
                    Label2_.Text = "Численность постоянного населения на начало " + Lastdate.Value + "года, человек";
                    Label1_.Text = "Численность постоянного населения на начало " + Lastdate.Value + "года, человек";
                      //  Label5_.Text = GetString_(GB_.Columns[index].Header.Caption) + " в " + Lastdate.Value + " году, человек";
                }
                Lastdate.Value = RealLastDate;
              };
              if
                  (e.Column != null)
              {
                  CBBC_.DataBind();
                  LBC_.DataBind();
                  RBC_.DataBind();



                  setFont(10, Label3_);
                  setFont(10, Label2_);
                  setFont(10, Label1_);
                  setFont(10, Label5_);
                  
                  if (IndexSelect != 4)
                  {
                      Label3_.Text = GetString_(GB_.Columns[index].Header.Caption) + " в " + Lastdate.Value + " году, человек";
                      Label2_.Text = GetString_(GB_.Columns[index].Header.Caption) + " в " + Lastdate.Value + " году, человек";
                      Label1_.Text = GetString_(GB_.Columns[index].Header.Caption) + " в " + Lastdate.Value + " году, человек";
                      Label5_.Text = GetString_(GB_.Columns[index].Header.Caption) + " в " + Lastdate.Value + " году, человек";
                  }
                  else
                  {
                      Label3_.Text = "Число прибывших" + " в " + Lastdate.Value + " году, человек";
                      Label2_.Text = "Число прибывших" + " в " + Lastdate.Value + " году, человек";
                      Label1_.Text = "Число прибывших" + " в " + Lastdate.Value + " году, человек";
                      Label5_.Text = "Число прибывших" + " в " + Lastdate.Value + " году, человек";

                  }
                  Lastdate.Value = RealLastDate;
              }

              if (index == 2)
              {
                  LBC_.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomLinear;
                  
                  CBBC_.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.LinearRange;
                  CBBC_.ColorModel.ColorBegin = Color.IndianRed;
                  CBBC_.ColorModel.ColorEnd = Color.RoyalBlue;

                  RBC_.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.LinearRange;
                  RBC_.ColorModel.ColorBegin = Color.DarkOrchid;
                  RBC_.ColorModel.ColorEnd = Color.SeaGreen;


              }

              if (index == 3)
              {
                  LBC_.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomLinear;

                  CBBC_.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.LinearRange;
                  CBBC_.ColorModel.ColorBegin = Color.IndianRed;
                  CBBC_.ColorModel.ColorEnd = Color.RoyalBlue;

                  RBC_.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.LinearRange;
                  RBC_.ColorModel.ColorBegin = Color.DarkOrchid;
                  RBC_.ColorModel.ColorEnd = Color.SeaGreen;


              }

              if (index == 4)
              {
                  LBC_.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomLinear;

                  CBBC_.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.PureRandom;
                  //CBBC_.ColorModel.ColorBegin = Color.IndianRed;
                  //CBBC_.ColorModel.ColorEnd = Color.RoyalBlue;

                  RBC_.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomLinear;
//                  RBC_.ColorModel.ColorBegin = Color.DarkOrchid;
  //                RBC_.ColorModel.ColorEnd = Color.SeaGreen;


             }

             if (index == 5)
             {
                 LBC_.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomLinear;

                 CBBC_.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.LinearRange;
                 CBBC_.ColorModel.ColorBegin = Color.IndianRed;
                 CBBC_.ColorModel.ColorEnd = Color.RoyalBlue;

                 RBC_.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.LinearRange;
                 RBC_.ColorModel.ColorBegin = Color.DarkOrchid;
                 RBC_.ColorModel.ColorEnd = Color.SeaGreen;


             }
        
        }

        protected void OneChart_DataBinding(object sender, EventArgs e)
        {
            OneChart_.DataSource = GetDSForChart("C11");

            //double Coef = 1;


            SetBeautifulChart(OneChart_, true, Infragistics.UltraChart.Shared.Styles.ChartType.ColumnChart, 8, Infragistics.UltraChart.Shared.Styles.LegendLocation.Left, (99));
           OneChart_.Axis.Y.Extent = 50;
           OneChart_.Axis.X.Labels.Visible =1==2;;
           OneChart_.Axis.X.Labels.SeriesLabels.Visible = 1 == 1;
           if (IndexSelect == 1)
           {
               Label7.Text = "<nobr/>Выберите показатель и год для отображения данных на диаграмме. Щёлкните на ячейку пересечения столбца и строки.<br/><br/>";
           }
           else
           {
               Label7.Text = "<nobr/>Выберите показатель и год для отображения данных на диаграмме. Щёлкните на ячейку пересечения столбца и строки.";
           }
CBBC_.AreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(CBBC_, -2, -2, 1 == 1, new Font("arial", 8,FontStyle.Bold), Color.Black, "<DATA_VALUE:#>", StringAlignment.Far, StringAlignment.Center, 0)); 
        }

        protected void TG__InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {

                TG_.Height = 300;
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[1].Selected = 1 == 1;
                e.Layout.AllowSortingDefault = AllowSorting.Yes;

                e.Layout.Rows[0].Hidden = 1 == 1;
            }
            catch { }

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                IndexSelect = int.Parse(IS.Value);
                IndexSelect++;
                Lastdate.Value = DD_.SelectedItem.Text;
                CC_.DataBind();
                TG_.DataBind();
                for (int i = 0; i < TG_.Columns.Count; i++)
                {
                    TG_.Columns[i].Selected = 1 == 2;
                }
                TG_.Columns[IndexSelect].Selected = 1 == 1;
                // string edizm = _GetString_(TG_.Columns[IndexSelect].Header.Caption,',');
                TCC_.Text = __GetString(TG_.Columns[IndexSelect].Header.Caption, ',') + " за " + DD_.SelectedItem.Text + " год, " + GridEd[IndexSelect - 1];


                TG_.Rows[0].Hidden = 1 == 1;
            }
            catch { }
        }

        protected void CG__InitializeLayout(object sender, LayoutEventArgs e)
        {
            //SetGridColumn(CG_, (42));
            CG_.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth*0.625);
            CG_.Height = 340;
            //e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.05);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            if (IndexSelect == 0) { IndexSelect = 1; };
                e.Layout.Bands[0].Columns[IndexSelect].Selected = 1 == 1;
            CG_.Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.11);
            CG_.Columns[3].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.09);


            e.Layout.CellClickActionDefault = CellClickAction.CellSelect;
            e.Layout.RowSelectorsDefault = RowSelectors.No;
            e.Layout.AllowSortingDefault = AllowSorting.Yes;
            
            
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            IndexSelect = int.Parse(IS2.Value);
            IndexSelect++;
            Lastdate.Value = DD2_.SelectedItem.Text;
           
            CG_.DataBind();
            IndexSelect--;
            CBC_.DataBind();
            IndexSelect++;
            SetGridColumn(CG_, (60));
            CG_.Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.11);
            CG_.Columns[3].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.09);
            CG_.Rows[0].Hidden = 1 == 1;
        }

        protected void GB__InitializeLayout(object sender, LayoutEventArgs e)
        {
            SetGridColumn(GB_, (108));
            GB_.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth*.99);
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth*0.05);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].Selected = 1 == 1;
            
            e.Layout.CellClickActionDefault = CellClickAction.CellSelect;
            e.Layout.RowSelectorsDefault = RowSelectors.No;
            e.Layout.AllowSortingDefault = AllowSorting.Yes;
            
        }

        protected void LBC__ChartDrawItem(object sender, Infragistics.UltraChart.Shared.Events.ChartDrawItemEventArgs e)
        {

        }

        protected void OneChart__DataBound(object sender, EventArgs e)
        {

        }

        protected void CC__ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }

    }
}
