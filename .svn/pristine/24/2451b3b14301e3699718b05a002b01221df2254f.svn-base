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

namespace Krista.FM.Server.Dashboards.reports.CTAT.CTAT_0004._0001
{
    public partial class _default : CustomReportPage
    {
        private CustomParam Cub { get { return (UserParams.CustomParam("cub")); } }
        private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
        private CustomParam Pokaz { get { return (UserParams.CustomParam("pokaz")); } }
        private CustomParam Region { get { return (UserParams.CustomParam("region")); } }
        private CustomParam Lastdate { get { return (UserParams.CustomParam("lastdate")); } }
        private CustomParam Firstyear { get { return (UserParams.CustomParam("firstdate")); } }
        private CustomParam Mounth { get { return (UserParams.CustomParam("mounth")); } }
        private CustomParam Mounth1 { get { return (UserParams.CustomParam("mounth1")); } }
        private string[] GLQ = new string[] { "1", "2", "3", "4", "5" };

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
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("last_date"));
                return cs.Axes[1].Positions[0].Members[0].ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public void SetBeautifulChart(UltraChart chart, bool legend, Infragistics.UltraChart.Shared.Styles.ChartType ChartType, int legendPercent, Infragistics.UltraChart.Shared.Styles.LegendLocation LegendLocation, double SizePercent)
        {
            try
            {
                chart.Width = CRHelper.GetChartWidth((int)(CustomReportConst.minScreenWidth * (SizePercent / 100)));
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

                chart.Axis.X.Extent = 100;
                chart.Axis.Y.Extent = 20;
                chart.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
                chart.Axis.X.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
                chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:###,##0.##>";
                chart.Axis.Z.Labels.Visible = 1 == 2;
            }
            catch
            { }

        }
        public void SetGridColumn(UltraWebGrid grid, double sizePercent)
        {
            try
            {

                double Width = CustomReportConst.minScreenWidth * (sizePercent / 100);
                grid.Width = CRHelper.GetGridWidth(Width);
                grid.Height = 0;
                double WidthColumn = Width / grid.Columns.Count;

                for (int i = 0; i < grid.Columns.Count; i++)
                {
                    grid.Columns[i].Width = CRHelper.GetColumnWidth(WidthColumn * 0.93);
                    grid.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                    grid.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                   
                    CRHelper.FormatNumberColumn(grid.Bands[0].Columns[i], "### ### ### ###.##");
                    grid.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                }
                grid.DisplayLayout.GroupByBox.Hidden = 1 == 1;
               // grid.Columns[0].CellStyle.BorderDetails.ColorLeft = Color.Gray;
                grid.Columns[0].Header.Caption = "Год";
                grid.DisplayLayout.NoDataMessage = "В настоящий момент данные отсутствуют";
            }
            catch { }
        }
        protected ArrayList GenMounth()
        {
            ArrayList Mounth = new ArrayList();

            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 1].[Январь");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 1].[Февраль");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 1].[Март");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 2].[Апрель");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 2].[Май");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 2].[Июнь");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 3].[Июль");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 3].[Август");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 3].[Сентябрь");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 4].[Октябрь");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 4].[Ноябрь");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 4].[Декабрь");

            return Mounth;
        }


        protected string GenSql()
        {
            Mesiac = GenMounth();

            
            string SQL_with = "  with ";
            string SQL_column = "";

            for (int i = 0; i <CountMounth; i++)
            {
                SQL_with += " member " + string.Format(Mesiac[i].ToString(), Lastdate.Value) + "-" + Lastdate.Value+"]";
                SQL_column += " " + string.Format(Mesiac[i].ToString(), Lastdate.Value) + "-" + Lastdate.Value + "],";
                SQL_with += " as " + "'" + string.Format(Mesiac[i].ToString(), Lastdate.Value)+"]'";

                SQL_with += " member " + string.Format(Mesiac[i].ToString(), Firstyear.Value) + "-" + Firstyear.Value + "]";
                SQL_column += "  " + string.Format(Mesiac[i].ToString(), Firstyear.Value) + "-" + Firstyear.Value + "],";
                SQL_with += " as " + "'" + string.Format(Mesiac[i].ToString(), Firstyear.Value) + "]'";
            }
            int ii = CountMounth;
            SQL_with += " member " + string.Format(Mesiac[ii].ToString(), Lastdate.Value) + "-" + Lastdate.Value + "]";
            SQL_column += " " + string.Format(Mesiac[ii].ToString(), Lastdate.Value) + "-" + Lastdate.Value + "],";
            SQL_with += " as " + "'" + string.Format(Mesiac[ii].ToString(), Lastdate.Value) + "]'";

            SQL_with += " member " + string.Format(Mesiac[ii].ToString(), Firstyear.Value) + "-" + Firstyear.Value + "]";
            SQL_column += "  " + string.Format(Mesiac[ii].ToString(), Firstyear.Value) + "-" + Firstyear.Value + "]";
            SQL_with += " as " + "'" + string.Format(Mesiac[ii].ToString(), Firstyear.Value) + "]'";

            string SQL = SQL_with + " select { " + SQL_column + @"
                                        }on columns ,
                                        non empty 
                                        {
                                            [Территории].[РФ Карта].[МР ГО].members 
                                        } on rows 
                                    FROM  [СТАТ_Труд_Трудовые ресурсы]  
                                    where  
                                        (
                                            [Труд].[Трудовые ресурсы].[Среднесписочная численность работников],
                                            [Группировки].[Труд_Трудовые ресурсы].[По городам и районам],
                                            [Источники данных].[Источник].[СТАТ Отчетность - Облстат],
                                            [Measures].[Нарастающий итог] 
                                        )"; 

                return SQL;

        }

        private string _GetString_(string s, char ch)
        {
            string res = "";
            int i = 0;
            for (i = s.Length - 1; s[i] != ch; i--) { res = s[i] + res; };

            return res;
        }

        private string GetString_(string s, char ch)
        {

            string res = "";
            int i = 0;
            for (i = s.Length - 1; s[i] != ch; i--) ;
            for (int j = 0; j < i; j++)
            {
                res += s[j];
            }
            return res;


        }

        protected void ForCrossJoin(LayoutEventArgs e, int span)
        {
            if (e.Layout.Bands[0].Columns.Count > 1)
            {
                ColumnHeader colHead;
                for (int i = 0; i < e.Layout.Bands[0].HeaderLayout.Count; i++)
                {
                    colHead = e.Layout.Bands[0].HeaderLayout[i] as ColumnHeader;
                    colHead.RowLayoutColumnInfo.OriginY = 1;
                }
                int dva = span;// :[)
               // if (!Load1) { e.Layout.Bands[0].HeaderLayout.Remove(e.Layout.Bands[0].HeaderLayout[0]); }
                int c = e.Layout.Bands[0].HeaderLayout.Count;
                try
                {
                    for (int i = 1; i < c; i += dva)
                    {
                        ColumnHeader ch = new ColumnHeader(true);
                        //CH = ch;
                        ch.Caption = GetString_(e.Layout.Bands[0].HeaderLayout[i].Caption, (char)59);
                        try
                        {
                            e.Layout.Bands[0].HeaderLayout[i].Caption = _GetString_(e.Layout.Bands[0].HeaderLayout[i].Caption, ' ');
                            e.Layout.Bands[0].HeaderLayout[i].Style.Wrap = true;
                            e.Layout.Bands[0].HeaderLayout[i + 1].Caption = _GetString_(e.Layout.Bands[0].HeaderLayout[i + 1].Caption, ' ');
                            e.Layout.Bands[0].HeaderLayout[i + 1].Style.Wrap = true;
                        }
                        catch
                        {
                        }


                        ch.RowLayoutColumnInfo.OriginX = i;//Позицыя по х относительно всех колумав

                        ch.RowLayoutColumnInfo.OriginY = 0;// по у
                        ch.RowLayoutColumnInfo.SpanX = dva;//Скока ячей резервировать
                        e.Layout.Bands[0].HeaderLayout.Add(ch);



                    }
                }
                catch
                {//Baanzzzaaaaaaaaaaaaaaaaaaaaaaaaaaaaaiiiiii!
                }
            }

        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
        }
        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);
        }
        int CountMounth = 4;
        public ArrayList Mesiac;
        protected override void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                Page.Title = "Отчёт";
                Label1.Text = "Среднесписочная численность работников";
                setFont(16, Label1);

                Lastdate.Value =  ELV(getLastDate(""));
                for (int i = 9; i > 0; i--)
                {
                    DropDownList1.Items.Add((i+int.Parse(Lastdate.Value)-9).ToString());
                }


                Mesiac = GenMounth();
                for(int i = 0; i<12;i++)
                {
                    DropDownList2.Items.Add(new ListItem( _GetString_(Mesiac[i].ToString(),'['),i.ToString()));
                }

                DropDownList2.SelectedIndex = 4;

                base.Page_Load(sender, e);
                Lastdate.Value = "2009";
                Firstyear.Value = "2008";

                G.DataBind();

                G_InitializeLayout(null, null);

                WebAsyncRefreshPanel1.AddLinkedRequestTrigger(Button1);
                
                //setFont(10, Label1);
                //setFont(10, Label2);
            }
        }

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            
        }

        protected void G_DataBinding(object sender, EventArgs e)
        {
                DataTable DT = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(GenSql(), "Трум пупум пупум опиЛКИ!", DT);
                G.DataSource = DT;
            

        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            
            if (e == null)
            {

                for (int i = 0; i < G.Rows.Count; i++)
                {
                    try
                    {
                        for (int j = 1; j < CountMounth + 2; j++)
                        {
                            if (double.Parse(G.Rows[i].Cells[j * 2].Text) != 0)
                            {
                                double res = (double.Parse(G.Rows[i].Cells[j * 2 - 1].Text) / double.Parse(G.Rows[i].Cells[j * 2].Text)) * 100;
                                if (Math.Round(res,2) > 100.1)
                                {
                                    G.Rows[i].Cells[j * 2].Text = Math.Round(res,1).ToString("##.0#") + "%   <img src=" + '"' + "../../../../images/1.gif" + '"' + "></img>";
                                }
                                else
                                    if (Math.Round(res,2) < 99.9)
                                    { G.Rows[i].Cells[j * 2].Text = Math.Round(res, 1).ToString("##.0#") + "%   <img src=" + '"' + "../../../../images/2.gif" + '"' + "></img>"; }
                                    else
                                    {
                                        G.Rows[i].Cells[j * 2].Text = Math.Round(res, 1).ToString("##.0#") + "%   <img src=" + '"' + "../../../../images/0.gif" + '"' + "></img>";
                                    }
                                //CRHelper.FormatNumberColumn(G.Columns[j * 2 - 1], "##.0#"); 
                                //Cells[j * 2 - 1].Text = 

                            }
                            else
                            {
                                G.Rows[i].Cells[j * 2].Text = "нет данных";

                            }

                        }
                    }
                    catch
                    { }
                }
                try
                {
                    for (int j = 1; j < CountMounth + 2; j++)
                    {
                        G.Columns[j * 2].Header.Caption = "в % к аналогичному периоду пред. года";
                        G.Columns[j * 2 - 1].Header.Caption = _GetString_(G.Columns[j * 2 - 1].Header.Caption,'-');
                    }
                }
                catch { }
                SetGridColumn(G, 101);
            }
            if (e != null)
            {
               
               
                ColumnHeader colHead;
                for (int i = 0; i < e.Layout.Bands[0].HeaderLayout.Count; i++)
                {
                    colHead = e.Layout.Bands[0].HeaderLayout[i] as ColumnHeader;
                    colHead.RowLayoutColumnInfo.OriginY = 1;
                }
                int c = e.Layout.Bands[0].HeaderLayout.Count;
                try
                {
                    for (int i = 1; i < c; i += 2)
                    {
                        ColumnHeader ch = new ColumnHeader(true);
                        //CH = ch;
                        ch.Caption = GetString_(e.Layout.Bands[0].HeaderLayout[i].Caption,'-');
    
                        ch.RowLayoutColumnInfo.OriginX = i;//Позицыя по х относительно всех колумав

                        ch.RowLayoutColumnInfo.OriginY = 0;// по у
                        ch.RowLayoutColumnInfo.SpanX = 2;//Скока ячей резервировать
                        e.Layout.Bands[0].HeaderLayout.Add(ch);



                    }
                }
                catch
                {//Baanzzzaaaaaaaaaaaaaaaaaaaaaaaaaaaaaiiiiii!
                }


            }
            
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //G.Clear();
            //G.Clear();
            //G.ResetColumns();
            //G.ResetRows();
            //G.Dispose();
            //G = new UltraWebGrid("G");
            //Controls.Add(G);
            G.Bands.Clear();
            Lastdate.Value = DropDownList1.SelectedItem.Text;
            Firstyear.Value = (int.Parse(Lastdate.Value) - 1).ToString();
            CountMounth = int.Parse(DropDownList2.SelectedItem.Value);
            G.DataBind();
            G_InitializeLayout(null, null);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Response.Redirect("../default.aspx");
        }
    }
}
